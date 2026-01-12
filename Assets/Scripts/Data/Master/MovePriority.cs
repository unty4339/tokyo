using System.Collections.Generic;
using System.Linq;

namespace MonsterBattleGame
{
    /// <summary>
    /// 発動対象の優先順位を表すクラス
    /// </summary>
    public class MovePriority
    {
        /// <summary>説明文を表すstring</summary>
        public string Description { get; set; }

        public MovePriority()
        {
            Description = string.Empty;
        }

        public MovePriority(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Monsterのリストを受け取ってMonsterを返すメソッド
        /// デフォルト実装: 最初のモンスターを返す
        /// </summary>
        public virtual Monster SelectTarget(List<Monster> monsters)
        {
            if (monsters == null || monsters.Count == 0)
            {
                return null;
            }

            // デフォルト実装: 最初の生きているモンスターを返す
            return monsters.FirstOrDefault(m => m != null && !m.IsDefeated());
        }
    }
}
