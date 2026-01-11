using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントコンテンツを作成するファクトリークラス
    /// </summary>
    public static class IncidentContentFactory
    {
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

            var content = new IncidentOptionalContent
            {
                Title = "戦闘",
                Type = IncidentContentType.Battle,
                MessageText = "戦闘中..."
            };

            return content;
        }
    }
}
