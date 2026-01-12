namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘中に使える行動を増やすスキル
    /// </summary>
    public class ActiveSkill : Skill
    {
        /// <summary>消費SPを示すint</summary>
        public int SPCost { get; set; }

        /// <summary>Moveインスタンス</summary>
        public Move Move { get; set; }

        public ActiveSkill() : base()
        {
            SPCost = 0;
            Move = null;
        }

        public ActiveSkill(int level, string name, int spCost, Move move) : base(level, name)
        {
            SPCost = spCost;
            Move = move;
        }

        /// <summary>
        /// 説明文を返すメソッド
        /// </summary>
        public override string GetDescription()
        {
            return $"{Name}: SP消費{SPCost}";
        }
    }
}
