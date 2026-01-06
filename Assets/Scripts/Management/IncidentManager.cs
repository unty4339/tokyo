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
        /// アクティブなインシデントのリスト
        /// </summary>
        private List<IncidentInstance> activeIncidents = new List<IncidentInstance>();

        /// <summary>
        /// 登録されているインシデント定義のリスト
        /// </summary>
        private List<Incident> registeredIncidents = new List<Incident>();

        private GameTimeManager timeManager;

        // イベント
        public event Action<IncidentInstance> OnIncidentOccurred;
        public event Action<IncidentInstance> OnIncidentResolved;
        public event Action<IncidentInstance> OnIncidentDismissed;
        public event Action<IncidentInstance> OnIncidentExpired;

        /// <summary>
        /// アクティブなインシデントのリストを取得（読み取り専用）
        /// </summary>
        public IReadOnlyList<IncidentInstance> ActiveIncidents => activeIncidents;

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
                if (activeIncidents.Any(inst => inst.Incident.Id == incident.Id))
                {
                    continue;
                }

                // 条件をチェック
                if (incident.CheckCondition(year, month, week))
                {
                    TriggerIncident(incident, totalWeeks);
                }
            }
        }

        /// <summary>
        /// インシデントを発生させる
        /// </summary>
        private void TriggerIncident(Incident incident, int totalWeeks)
        {
            var instance = new IncidentInstance(incident, totalWeeks);
            activeIncidents.Add(instance);

            // 必須インシデントの場合は自動的にポーズ
            if (incident.IsMandatory && timeManager != null)
            {
                timeManager.Pause();
            }

            OnIncidentOccurred?.Invoke(instance);
        }

        /// <summary>
        /// 期限切れのインシデントをチェック
        /// </summary>
        private void CheckExpiredIncidents(int totalWeeks)
        {
            var expiredIncidents = activeIncidents.Where(inst => inst.IsExpired(totalWeeks) && !inst.Incident.IsMandatory).ToList();
            
            foreach (var expired in expiredIncidents)
            {
                activeIncidents.Remove(expired);
                OnIncidentExpired?.Invoke(expired);
            }
        }

        /// <summary>
        /// インシデントを解決（アイコンを消す）
        /// </summary>
        public void ResolveIncident(IncidentInstance instance)
        {
            if (instance == null || !activeIncidents.Contains(instance))
            {
                return;
            }

            activeIncidents.Remove(instance);
            OnIncidentResolved?.Invoke(instance);

            // ウィンドウを破棄
            if (instance.WindowPrefabInstance != null)
            {
                Destroy(instance.WindowPrefabInstance);
                instance.WindowPrefabInstance = null;
            }
        }

        /// <summary>
        /// インシデントを放置（アイコンは残す）
        /// </summary>
        public void DismissIncident(IncidentInstance instance)
        {
            if (instance == null || !activeIncidents.Contains(instance))
            {
                return;
            }

            OnIncidentDismissed?.Invoke(instance);

            // ウィンドウを破棄
            if (instance.WindowPrefabInstance != null)
            {
                Destroy(instance.WindowPrefabInstance);
                instance.WindowPrefabInstance = null;
            }
        }
    }
}
