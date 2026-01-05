using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘結果
    /// </summary>
    public class BattleResult
    {
        /// <summary>勝敗結果</summary>
        public BattleState Result { get; set; }

        /// <summary>プレイヤーチームの残HP（モンスター別）</summary>
        public Dictionary<Monster, int> PlayerTeamRemainingHP { get; set; }

        /// <summary>敵チームの残HP（モンスター別）</summary>
        public Dictionary<Monster, int> EnemyTeamRemainingHP { get; set; }

        /// <summary>終了時のターン数</summary>
        public int TurnCount { get; set; }

        public BattleResult()
        {
            PlayerTeamRemainingHP = new Dictionary<Monster, int>();
            EnemyTeamRemainingHP = new Dictionary<Monster, int>();
        }

        public BattleResult(BattleState result, Dictionary<Monster, int> playerHP, Dictionary<Monster, int> enemyHP, int turnCount)
        {
            Result = result;
            PlayerTeamRemainingHP = playerHP ?? new Dictionary<Monster, int>();
            EnemyTeamRemainingHP = enemyHP ?? new Dictionary<Monster, int>();
            TurnCount = turnCount;
        }
    }
}

