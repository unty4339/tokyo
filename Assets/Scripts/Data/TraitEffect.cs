namespace MonsterBattleGame
{
    /// <summary>
    /// 特性の効果
    /// </summary>
    public class TraitEffect
    {
        /// <summary>効果タイプ</summary>
        public EffectType Type { get; set; }

        /// <summary>効果値</summary>
        public float Value { get; set; }

        public TraitEffect()
        {
        }

        public TraitEffect(EffectType type, float value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// 効果を適用
        /// </summary>
        public void Apply(Monster monster, BattleContext context)
        {
            switch (Type)
            {
                case EffectType.AttackBoost:
                    // 攻撃力を一時的に上昇（実際の実装では、バフシステムが必要かもしれません）
                    // ここでは簡易的にステータス再計算をトリガーする想定
                    monster.CalculateStats();
                    break;

                case EffectType.DefenseBoost:
                    // 防御力を一時的に上昇
                    monster.CalculateStats();
                    break;

                case EffectType.SpeedBoost:
                    // 素早さを一時的に上昇
                    monster.CalculateStats();
                    break;

                case EffectType.Heal:
                    // HP回復
                    int healAmount = (int)(monster.CalculatedHP * Value);
                    monster.CurrentHP = System.Math.Min(monster.CurrentHP + healAmount, monster.CalculatedHP);
                    break;
            }
        }
    }
}

