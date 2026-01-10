using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントの発生・管理を行うシングルトン
    /// </summary>
    public class IncidentManager : MonoBehaviour
    {
        private static IncidentManager _instance;
        public static IncidentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("IncidentManager");
                    _instance = go.AddComponent<IncidentManager>();
                }
                return _instance;
            }
        }

        /// <summary>
        /// アクティブなインシデントのリスト（IncidentProcess）
        /// </summary>
        private List<IncidentProcess> activeIncidents = new List<IncidentProcess>();

        /// <summary>
        /// 登録されているインシデント定義のリスト
        /// </summary>
        private List<Incident> registeredIncidents = new List<Incident>();

        private GameTimeManager timeManager;

        // イベント
        public event Action<IncidentProcess> OnIncidentOccurred;
        public event Action<IncidentProcess> OnIncidentResolved;
        public event Action<IncidentProcess> OnIncidentDismissed;
        public event Action<IncidentProcess> OnIncidentExpired;

        /// <summary>
        /// アクティブなインシデントのリストを取得（読み取り専用）
        /// </summary>
        public IReadOnlyList<IncidentProcess> ActiveIncidents => activeIncidents;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            timeManager = GameTimeManager.Instance;
            if (timeManager != null)
            {
                timeManager.OnWeekChanged += OnWeekChanged;
            }
        }

        private void Start()
        {
            // 季節インシデントを登録
            RegisterSeasonalIncident();
        }

        private void OnDestroy()
        {
            if (timeManager != null)
            {
                timeManager.OnWeekChanged -= OnWeekChanged;
            }
        }

        /// <summary>
        /// 週が変わったときに呼ばれる
        /// </summary>
        private void OnWeekChanged(int year, int month, int week)
        {
            // 累積週数を計算
            int totalWeeks = CalculateTotalWeeks(year, month, week);
            
            // 条件をチェックして新しいインシデントを発生
            CheckAndTriggerIncidents(year, month, week, totalWeeks);
            
            // 期限切れのインシデントをチェック
            CheckExpiredIncidents(totalWeeks);
        }

        /// <summary>
        /// 累積週数を計算（年、月、週から）
        /// </summary>
        private int CalculateTotalWeeks(int year, int month, int week)
        {
            // GameTimeManagerの定数を使用
            const int WEEKS_PER_MONTH = 4;
            const int WEEKS_PER_YEAR = 52;
            
            int totalWeeks = (year - 1) * WEEKS_PER_YEAR + (month - 1) * WEEKS_PER_MONTH + (week - 1);
            return totalWeeks;
        }

        /// <summary>
        /// インシデント定義を登録
        /// </summary>
        public void RegisterIncident(Incident incident)
        {
            if (incident != null && !registeredIncidents.Contains(incident))
            {
                registeredIncidents.Add(incident);
            }
        }

        /// <summary>
        /// 条件をチェックして新しいインシデントを発生
        /// </summary>
        private void CheckAndTriggerIncidents(int year, int month, int week, int totalWeeks)
        {
            foreach (var incident in registeredIncidents)
            {
                // 既にアクティブなインシデントはスキップ
                if (activeIncidents.Any(process => process.Incident.Id == incident.Id))
                {
                    continue;
                }

                // 条件をチェック
                if (incident.CheckCondition(year, month, week))
                {
                    Debug.Log($"[IncidentManager] {incident.Id} が発生しました。");
                    TriggerIncident(incident, totalWeeks);
                }
            }
        }

        /// <summary>
        /// インシデントを発生させる
        /// </summary>
        private void TriggerIncident(Incident incident, int totalWeeks)
        {
            // 初期状態を取得
            IncidentState initialState = null;
            IncidentProcess process = null;
            
            if (incident is ExplorationIncident explorationIncident)
            {
                // ExplorationIncidentの場合は、IncidentProcessを作成してから初期状態を取得
                // 暫定的にnullの初期状態でIncidentProcessを作成
                process = new IncidentProcess(incident, null, totalWeeks);
                
                // GetInitialStateはIncidentProcessを受け取るように変更されているため、processを渡す
                initialState = explorationIncident.GetInitialState(process);
                
                // 初期状態を設定
                if (initialState != null)
                {
                    process.CurrentState = initialState;
                    process.InitialState = initialState;
                    if (!process.StateHistory.Contains(initialState))
                    {
                        process.StateHistory.Add(initialState);
                    }
                }
            }
            else
            {
                // ExplorationIncident以外の場合は、初期状態なしで作成
                process = new IncidentProcess(incident, null, totalWeeks);
            }

            activeIncidents.Add(process);

            // 必須インシデント、または即時解決が必要なインシデントの場合は自動的にポーズ
            if (timeManager != null)
            {
                if (incident.IsMandatory || HasImmediateIncidents())
                {
                    timeManager.Pause();
                }
            }

            OnIncidentOccurred?.Invoke(process);
        }

        /// <summary>
        /// インシデントを直接発生させる（外部から呼び出し可能）
        /// 探索エリアなどから確率判定で直接イベントを発火する場合に使用
        /// </summary>
        /// <param name="incident">発生させるインシデント</param>
        /// <param name="initialState">初期状態（オプショナル）</param>
        /// <param name="year">現在の年</param>
        /// <param name="month">現在の月</param>
        /// <param name="week">現在の週</param>
        public void TriggerIncidentDirectly(Incident incident, IncidentState initialState, int year, int month, int week)
        {
            if (incident == null)
            {
                Debug.LogWarning("[IncidentManager] インシデントがnullです。");
                return;
            }

            // 既にアクティブなインシデントはスキップ
            if (activeIncidents.Any(process => process.Incident.Id == incident.Id))
            {
                Debug.Log($"[IncidentManager] インシデント {incident.Id} は既にアクティブです。");
                return;
            }

            int totalWeeks = CalculateTotalWeeks(year, month, week);
            
            // IncidentProcessを作成（初期状態は後で設定）
            var process = new IncidentProcess(incident, initialState, totalWeeks);
            
            // 初期状態が指定されていない場合は取得
            if (initialState == null && incident is ExplorationIncident explorationIncident)
            {
                initialState = explorationIncident.GetInitialState(process);
                
                // 初期状態を設定
                if (initialState != null)
                {
                    process.CurrentState = initialState;
                    process.InitialState = initialState;
                    if (!process.StateHistory.Contains(initialState))
                    {
                        process.StateHistory.Add(initialState);
                    }
                }
            }

            activeIncidents.Add(process);

            // 必須インシデント、または即時解決が必要なインシデントの場合は自動的にポーズ
            if (timeManager != null)
            {
                if (incident.IsMandatory || HasImmediateIncidents())
                {
                    timeManager.Pause();
                }
            }

            OnIncidentOccurred?.Invoke(process);
        }

        /// <summary>
        /// インシデントを直接発生させる（初期状態なしのバージョン）
        /// </summary>
        /// <param name="incident">発生させるインシデント</param>
        /// <param name="year">現在の年</param>
        /// <param name="month">現在の月</param>
        /// <param name="week">現在の週</param>
        public void TriggerIncidentDirectly(Incident incident, int year, int month, int week)
        {
            TriggerIncidentDirectly(incident, null, year, month, week);
        }

        /// <summary>
        /// 期限切れのインシデントをチェック
        /// </summary>
        private void CheckExpiredIncidents(int totalWeeks)
        {
            var expiredIncidents = activeIncidents.Where(process => process.IsExpired(totalWeeks) && !process.Incident.IsMandatory).ToList();
            
            foreach (var expired in expiredIncidents)
            {
                activeIncidents.Remove(expired);
                OnIncidentExpired?.Invoke(expired);
            }
        }

        /// <summary>
        /// ひとつでも即時解決が必要なインシデントがあるかどうかを判定
        /// </summary>
        /// <returns>即時解決が必要なインシデントがある場合はtrue</returns>
        public bool HasImmediateIncidents()
        {
            return activeIncidents.Any(process =>
            {
                if (process.CurrentState != null)
                {
                    return process.CurrentState.Urgency == IncidentUrgency.Immediate;
                }
                else if (process.InitialState != null)
                {
                    return process.InitialState.Urgency == IncidentUrgency.Immediate;
                }
                // 状態がない場合は、必須インシデントかどうかで判定
                return process.Incident != null && process.Incident.IsMandatory;
            });
        }

        /// <summary>
        /// インシデントを解決（アイコンを消す）
        /// </summary>
        public void ResolveIncident(IncidentProcess process)
        {
            if (process == null || !activeIncidents.Contains(process))
            {
                return;
            }

            // インシデントの解決処理を実行
            if (process.Incident != null)
            {
                process.Incident.OnResolve(process);
            }

            activeIncidents.Remove(process);
            OnIncidentResolved?.Invoke(process);

            // ウィンドウを破棄
            if (process.WindowPrefabInstance != null)
            {
                Destroy(process.WindowPrefabInstance);
                process.WindowPrefabInstance = null;
            }
        }

        /// <summary>
        /// インシデントを放置（アイコンは残す）
        /// </summary>
        public void DismissIncident(IncidentProcess process)
        {
            if (process == null || !activeIncidents.Contains(process))
            {
                return;
            }

            OnIncidentDismissed?.Invoke(process);

            // ウィンドウを破棄
            if (process.WindowPrefabInstance != null)
            {
                Destroy(process.WindowPrefabInstance);
                process.WindowPrefabInstance = null;
            }
        }

        /// <summary>
        /// 季節インシデントを登録する
        /// </summary>
        public void RegisterSeasonalIncident()
        {
            RegisterIncident(new EnrollmentIncident());
            RegisterIncident(new GraduationIncident());
        }
    }
}
