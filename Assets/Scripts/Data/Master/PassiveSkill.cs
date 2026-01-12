namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘中に使わないスキル（戦闘報酬が増えるなど）
    /// </summary>
    public class PassiveSkill : Skill
    {
        /// <summary>説明文</summary>
        public string Description { get; set; }

        public PassiveSkill() : base()
        {
            Description = string.Empty;
        }

        public PassiveSkill(int level, string name, string description) : base(level, name)
        {
            Description = description;
        }

        /// <summary>
        /// 説明文を返すメソッド
        /// </summary>
        public override string GetDescription()
        {
            return Description;
        }
    }
}
