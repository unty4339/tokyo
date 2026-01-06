using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// アクティブなインシデントのインスタンス
    /// インシデントが発生したときに作成される
    /// </summary>
    public class IncidentInstance
    {
        /// <summary>
        /// このインスタンスの元となるインシデント定義
        /// </summary>
        public Incident Incident { get; set; }

        /// <summary>
        /// インシデントが発生した週（累積週数）
        /// </summary>
        public int StartWeek { get; set; }

        /// <summary>
        /// 期限切れとなる週（累積週数）。TimeLimitWeeksがnullの場合はnull
        /// </summary>
        public int? ExpiryWeek { get; set; }

        /// <summary>
        /// ウィンドウPrefabのインスタンス
        /// </summary>
        public GameObject WindowPrefabInstance { get; set; }

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        public IncidentInstance(Incident incident, int startWeek)
        {
            Incident = incident;
            StartWeek = startWeek;
            
            if (incident.TimeLimitWeeks.HasValue)
            {
                ExpiryWeek = startWeek + incident.TimeLimitWeeks.Value;
            }
            else
            {
                ExpiryWeek = null;
            }
        }

        /// <summary>
        /// 期限切れかどうかをチェック
        /// </summary>
        /// <param name="currentWeek">現在の週（累積週数）</param>
        /// <returns>期限切れの場合はtrue</returns>
        public bool IsExpired(int currentWeek)
        {
            if (!ExpiryWeek.HasValue)
            {
                return false; // 時限なし
            }
            return currentWeek >= ExpiryWeek.Value;
        }
    }
}
