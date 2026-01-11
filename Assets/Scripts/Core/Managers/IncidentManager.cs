using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントの発生・管理を行うシングルトン
    /// IncidentStateインスタンスを保持している
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
        /// アクティブなインシデント状態のリスト
        /// </summary>
        private List<IncidentState> activeStates = new List<IncidentState>();

        /// <summary>
        /// 各IncidentStateの開始週を管理する辞書
        /// </summary>
        private Dictionary<IncidentState, int> stateStartWeeks = new Dictionary<IncidentState, int>();

        /// <summary>
        /// 各IncidentStateに対応するIncidentIconを管理する辞書
        /// </summary>
        private Dictionary<IncidentState, IncidentIcon> stateIcons = new Dictionary<IncidentState, IncidentIcon>();

        /// <summary>
        /// 各IncidentStateに対応するウィンドウインスタンスを管理する辞書
        /// </summary>
        private Dictionary<IncidentState, GameObject> stateWindows = new Dictionary<IncidentState, GameObject>();

        /// <summary>
        /// 各IncidentStateに対応するIncident定義を管理する辞書
        /// </summary>
        private Dictionary<IncidentState, Incident> stateIncidents = new Dictionary<IncidentState, Incident>();

        /// <summary>
        /// 登録されているインシデント定義のリスト
        /// </summary>
        private List<Incident> registeredIncidents = new List<Incident>();

        private GameTimeManager timeManager;

        // イベント
        public event Action<IncidentState> OnIncidentOccurred;
        public event Action<IncidentState> OnIncidentResolved;
        public event Action<IncidentState> OnIncidentDismissed;
        public event Action<IncidentState> OnIncidentExpired;

        /// <summary>
        /// アクティブなインシデント状態のリストを取得（読み取り専用）
        /// </summary>
        public IReadOnlyList<IncidentState> ActiveStates => activeStates;

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
        /// IncidentStateインスタンスを登録する
        /// </summary>
        /// <param name="state">登録するIncidentState</param>
        /// <param name="startWeek">開始週（累積週数）</param>
        /// <param name="incident">対応するIncident定義（オプショナル）</param>
        public void Register(IncidentState state, int startWeek, Incident incident = null)
        {
            if (state == null)
            {
                Debug.LogWarning("[IncidentManager] state is null");
                return;
            }

            if (activeStates.Contains(state))
            {
                Debug.LogWarning($"[IncidentManager] state {state.GetStateId()} is already registered");
                return;
            }

            activeStates.Add(state);
            stateStartWeeks[state] = startWeek;
            if (incident != null)
            {
                stateIncidents[state] = incident;
            }

            // IncidentIconを作成
            CreateIconForState(state, incident);

            // 必須インシデント、または即時解決が必要なインシデントの場合は自動的にポーズ
            if (timeManager != null)
            {
                if ((incident != null && incident.IsMandatory) || HasImmediateIncidents())
                {
                    timeManager.Pause();
                }
            }

            OnIncidentOccurred?.Invoke(state);
        }

        /// <summary>
        /// IncidentStateに対応するIncidentIconを作成
        /// </summary>
        private void CreateIconForState(IncidentState state, Incident incident)
        {
            // IncidentUIが存在する場合は、そこでアイコンを作成する
            // ここでは辞書に登録のみ行う（実際の作成はIncidentUIで行う）
            // 仕様書によるとIncidentManagerがアイコン作成を担当するが、
            // UIの実装詳細はIncidentUIに委譲する
        }

        /// <summary>
        /// 条件をチェックして新しいインシデントを発生
        /// </summary>
        private void CheckAndTriggerIncidents(int year, int month, int week, int totalWeeks)
        {
            foreach (var incident in registeredIncidents)
            {
                // 既にアクティブなインシデントはスキップ
                if (activeStates.Any(state => stateIncidents.ContainsKey(state) && stateIncidents[state].Id == incident.Id))
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
            IncidentState initialState = null;
            
            if (incident is ExplorationIncident explorationIncident)
            {
                // ExplorationIncidentの場合は初期状態を取得
                initialState = explorationIncident.GetInitialState();
            }
            else
            {
                // その他のIncidentタイプの場合、GetInitialStateメソッドがあれば呼び出す
                var method = incident.GetType().GetMethod("GetInitialState", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (method != null && method.ReturnType == typeof(IncidentState))
                {
                    initialState = method.Invoke(incident, null) as IncidentState;
                }
            }

            if (initialState != null)
            {
                Register(initialState, totalWeeks, incident);
            }
            else
            {
                Debug.LogWarning($"[IncidentManager] Failed to get initial state for incident: {incident.Id}");
            }
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
            if (activeStates.Any(state => stateIncidents.ContainsKey(state) && stateIncidents[state].Id == incident.Id))
            {
                Debug.Log($"[IncidentManager] インシデント {incident.Id} は既にアクティブです。");
                return;
            }

            int totalWeeks = CalculateTotalWeeks(year, month, week);
            
            // 初期状態が指定されていない場合は取得
            if (initialState == null)
            {
                if (incident is ExplorationIncident explorationIncident)
                {
                    initialState = explorationIncident.GetInitialState();
                }
                else
                {
                    // その他のIncidentタイプの場合、GetInitialStateメソッドがあれば呼び出す
                    var method = incident.GetType().GetMethod("GetInitialState", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (method != null && method.ReturnType == typeof(IncidentState))
                    {
                        initialState = method.Invoke(incident, null) as IncidentState;
                    }
                }
            }

            if (initialState != null)
            {
                Register(initialState, totalWeeks, incident);
            }
            else
            {
                Debug.LogWarning($"[IncidentManager] Failed to get initial state for incident: {incident.Id}");
            }
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
            var expiredStates = new List<IncidentState>();
            
            foreach (var state in activeStates)
            {
                if (!stateStartWeeks.ContainsKey(state))
                {
                    continue;
                }

                int startWeek = stateStartWeeks[state];
                int? timeLimitWeeks = state.TimeLimitWeeks;
                
                if (timeLimitWeeks.HasValue)
                {
                    int expiryWeek = startWeek + timeLimitWeeks.Value;
                    if (totalWeeks >= expiryWeek)
                    {
                        // 必須インシデントでない場合のみ期限切れ
                        if (!stateIncidents.ContainsKey(state) || !stateIncidents[state].IsMandatory)
                        {
                            expiredStates.Add(state);
                        }
                    }
                }
            }
            
            foreach (var expired in expiredStates)
            {
                Unregister(expired);
                OnIncidentExpired?.Invoke(expired);
            }
        }

        /// <summary>
        /// ひとつでも即時解決が必要なインシデントがあるかどうかを判定
        /// </summary>
        /// <returns>即時解決が必要なインシデントがある場合はtrue</returns>
        public bool HasImmediateIncidents()
        {
            return activeStates.Any(state => state.IsImmediately());
        }

        /// <summary>
        /// インシデントを解決（アイコンを消す）
        /// </summary>
        public void ResolveIncident(IncidentState state)
        {
            if (state == null || !activeStates.Contains(state))
            {
                return;
            }

            // インシデントの解決処理を実行
            if (stateIncidents.ContainsKey(state))
            {
                stateIncidents[state].OnResolve(state);
            }

            Unregister(state);
            OnIncidentResolved?.Invoke(state);
        }

        /// <summary>
        /// インシデントを放置（アイコンは残す）
        /// </summary>
        public void DismissIncident(IncidentState state)
        {
            if (state == null || !activeStates.Contains(state))
            {
                return;
            }

            OnIncidentDismissed?.Invoke(state);

            // ウィンドウを破棄
            if (stateWindows.ContainsKey(state) && stateWindows[state] != null)
            {
                Destroy(stateWindows[state]);
                stateWindows.Remove(state);
            }
        }

        /// <summary>
        /// インシデント状態を登録解除
        /// </summary>
        private void Unregister(IncidentState state)
        {
            activeStates.Remove(state);
            stateStartWeeks.Remove(state);
            
            // アイコンを削除
            if (stateIcons.ContainsKey(state))
            {
                if (stateIcons[state] != null)
                {
                    stateIcons[state].Remove();
                }
                stateIcons.Remove(state);
            }

            // ウィンドウを削除
            if (stateWindows.ContainsKey(state))
            {
                if (stateWindows[state] != null)
                {
                    Destroy(stateWindows[state]);
                }
                stateWindows.Remove(state);
            }

            stateIncidents.Remove(state);
        }

        /// <summary>
        /// アクションを適用して状態遷移を実行
        /// </summary>
        /// <param name="state">現在の状態</param>
        /// <param name="action">アクション。nullの場合は時間切れを意味する</param>
        /// <returns>次の状態。nullの場合はインシデント終了</returns>
        public IncidentState ApplyAction(IncidentState state, IncidentAction action)
        {
            if (state == null || !activeStates.Contains(state))
            {
                Debug.LogWarning("[IncidentManager] state is null or not registered");
                return null;
            }

            IncidentState nextState = state.Translate(action);
            
            if (nextState == null)
            {
                // インシデント終了
                ResolveIncident(state);
                return null;
            }
            else
            {
                // 状態を更新
                int startWeek = stateStartWeeks.ContainsKey(state) ? stateStartWeeks[state] : 0;
                Incident incident = stateIncidents.ContainsKey(state) ? stateIncidents[state] : null;
                
                Unregister(state);
                Register(nextState, startWeek, incident);
                
                // アイコンの見た目を更新
                if (stateIcons.ContainsKey(nextState))
                {
                    stateIcons[nextState].UpdateAppearance();
                }
                
                return nextState;
            }
        }

        /// <summary>
        /// 状態に対応するIncidentを取得
        /// </summary>
        public Incident GetIncidentForState(IncidentState state)
        {
            return stateIncidents.ContainsKey(state) ? stateIncidents[state] : null;
        }

        /// <summary>
        /// 状態に対応するウィンドウを取得
        /// </summary>
        public GameObject GetWindowForState(IncidentState state)
        {
            return stateWindows.ContainsKey(state) ? stateWindows[state] : null;
        }

        /// <summary>
        /// 状態に対応するウィンドウを設定
        /// </summary>
        public void SetWindowForState(IncidentState state, GameObject window)
        {
            if (stateWindows.ContainsKey(state) && stateWindows[state] != null)
            {
                Destroy(stateWindows[state]);
            }
            stateWindows[state] = window;
        }

        /// <summary>
        /// 状態に対応するアイコンを設定
        /// </summary>
        public void SetIconForState(IncidentState state, IncidentIcon icon)
        {
            stateIcons[state] = icon;
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
