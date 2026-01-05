namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘コンテキスト
    /// </summary>
    public class BattleContext
    {
        /// <summary>戦闘への参照</summary>
        public Battle Battle { get; set; }

        /// <summary>ターン数</summary>
        public int TurnNumber { get; set; }

        /// <summary>現在行動中のモンスター</summary>
        public Monster CurrentActor { get; set; }

        public BattleContext()
        {
            TurnNumber = 0;
        }

        public BattleContext(Battle battle, int turnNumber, Monster currentActor)
        {
            Battle = battle;
            TurnNumber = turnNumber;
            CurrentActor = currentActor;
        }
    }
}

