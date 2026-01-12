using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// 各部員の経歴を保存するためのクラス
    /// </summary>
    public class Career
    {
        /// <summary>性交回数</summary>
        public int SexCount { get; set; }

        /// <summary>妊娠回数</summary>
        public int PregnancyCount { get; set; }

        /// <summary>戦闘回数</summary>
        public int BattleCount { get; set; }

        /// <summary>勝利回数</summary>
        public int VictoryCount { get; set; }

        /// <summary>CareerEventインスタンスのリスト</summary>
        public List<CareerEvent> Events { get; set; }

        public Career()
        {
            SexCount = 0;
            PregnancyCount = 0;
            BattleCount = 0;
            VictoryCount = 0;
            Events = new List<CareerEvent>();
        }

        public Career(int sexCount, int pregnancyCount, int battleCount, int victoryCount, List<CareerEvent> events = null)
        {
            SexCount = sexCount;
            PregnancyCount = pregnancyCount;
            BattleCount = battleCount;
            VictoryCount = victoryCount;
            Events = events ?? new List<CareerEvent>();
        }
    }
}
