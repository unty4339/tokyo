namespace MonsterBattleGame
{
    /// <summary>
    /// 攻撃技のクラス
    /// </summary>
    public class AttackMove : Move
    {
        /// <summary>威力のint</summary>
        public int Power { get; set; }

        /// <summary>攻撃対象数のint</summary>
        public int TargetCount { get; set; }

        /// <summary>説明文のstring</summary>
        public string Description { get; set; }

        /// <summary>MovePriorityインスタンス</summary>
        public MovePriority Priority { get; set; }

        /// <summary>BattleAttributeインスタンス</summary>
        public BattleAttribute Attribute { get; set; }

        public AttackMove() : base()
        {
            Power = 0;
            TargetCount = 1;
            Description = string.Empty;
            Priority = new MovePriority();
            Attribute = BattleAttribute.Melee;
        }

        public AttackMove(string name, int power, int targetCount, string description, MovePriority priority, BattleAttribute attribute) : base(name)
        {
            Power = power;
            TargetCount = targetCount;
            Description = description;
            Priority = priority ?? new MovePriority();
            Attribute = attribute;
        }
    }
}
