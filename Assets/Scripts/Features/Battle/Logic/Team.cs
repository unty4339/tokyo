using System.Collections.Generic;
using System.Linq;

namespace MonsterBattleGame
{
    /// <summary>
    /// チーム
    /// </summary>
    public class Team
    {
        /// <summary>モンスターのリスト（最大3体）</summary>
        public List<Monster> Monsters { get; set; }

        public Team()
        {
            Monsters = new List<Monster>();
        }

        public Team(List<Monster> monsters)
        {
            Monsters = monsters ?? new List<Monster>();
        }

        /// <summary>
        /// 全滅判定
        /// </summary>
        public bool IsDefeated()
        {
            return Monsters == null || Monsters.Count == 0 || Monsters.All(m => m.IsDefeated());
        }

        /// <summary>
        /// 戦闘可能なモンスターを取得
        /// </summary>
        public List<Monster> GetActiveMonsters()
        {
            return Monsters.Where(m => !m.IsDefeated()).ToList();
        }
    }
}

