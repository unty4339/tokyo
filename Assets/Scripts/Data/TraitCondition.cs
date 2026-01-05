namespace MonsterBattleGame
{
    /// <summary>
    /// 特性の発動条件
    /// </summary>
    public class TraitCondition
    {
        /// <summary>条件タイプ</summary>
        public ConditionType Type { get; set; }

        /// <summary>閾値</summary>
        public float Threshold { get; set; }

        public TraitCondition()
        {
        }

        public TraitCondition(ConditionType type, float threshold)
        {
            Type = type;
            Threshold = threshold;
        }

        /// <summary>
        /// 条件が満たされているかチェック
        /// </summary>
        public bool IsSatisfied(Monster monster, BattleContext context)
        {
            switch (Type)
            {
                case ConditionType.HPBelow:
                    if (monster.CalculatedHP > 0)
                    {
                        float hpPercentage = (float)monster.CurrentHP / monster.CalculatedHP;
                        return hpPercentage <= Threshold;
                    }
                    return false;

                case ConditionType.HPAbove:
                    if (monster.CalculatedHP > 0)
                    {
                        float hpPercentage = (float)monster.CurrentHP / monster.CalculatedHP;
                        return hpPercentage >= Threshold;
                    }
                    return false;

                case ConditionType.TurnNumber:
                    return context != null && context.TurnNumber == (int)Threshold;

                default:
                    return false;
            }
        }
    }
}

