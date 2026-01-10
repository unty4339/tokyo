using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントコンテンツを作成するファクトリークラス
    /// </summary>
    public static class IncidentContentFactory
    {
        /// <summary>
        /// 状態からコンテンツを作成
        /// </summary>
        /// <param name="state">インシデントの状態</param>
        /// <param name="process">IncidentProcess</param>
        /// <returns>作成されたコンテンツ</returns>
        public static IncidentContent CreateContent(IncidentState state, IncidentProcess process)
        {
            if (state == null)
            {
                Debug.LogError("[IncidentContentFactory] state is null");
                return null;
            }

            if (process == null)
            {
                Debug.LogError("[IncidentContentFactory] process is null");
                return null;
            }

            // 戦闘状態の場合は特別処理
            if (state is BattleIncidentState battleState)
            {
                var content = CreateBattleContent(battleState);
                if (content != null)
                {
                    content.Process = process;
                }
                return content;
            }

            // 通常の状態の場合は、ExplorationIncidentに委譲
            // ただし、BattleIncidentStateの場合は既に上で処理されているのでここには来ない
            if (process.Incident is ExplorationIncident explorationIncident)
            {
                return explorationIncident.CreateContentFromState(process);
            }

            Debug.LogWarning($"[IncidentContentFactory] Unsupported incident type: {process.Incident?.GetType().Name}");
            return null;
        }

        /// <summary>
        /// 戦闘用コンテンツを作成
        /// </summary>
        /// <param name="battleState">戦闘状態</param>
        /// <returns>戦闘用コンテンツ</returns>
        public static IncidentContent CreateBattleContent(BattleIncidentState battleState)
        {
            if (battleState == null)
            {
                Debug.LogError("[IncidentContentFactory] battleState is null");
                return null;
            }

            var content = new IncidentContent
            {
                Title = "戦闘",
                Type = IncidentContentType.Battle,
                MessageText = "戦闘中..."
            };

            return content;
        }
    }
}
