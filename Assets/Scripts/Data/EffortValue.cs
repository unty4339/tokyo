namespace MonsterBattleGame
{
    /// <summary>
    /// 努力値 (Effort Values / EVs)
    /// バトルやアイテムによって後天的に蓄積される経験値のようなパラメータ
    /// </summary>
    public class EffortValue
    {
        /// <summary>HP努力値</summary>
        public int HP { get; private set; }

        /// <summary>攻撃努力値</summary>
        public int Attack { get; private set; }

        /// <summary>防御努力値</summary>
        public int Defense { get; private set; }

        /// <summary>素早さ努力値</summary>
        public int Speed { get; private set; }

        /// <summary>
        /// 全ステータスの合計努力値
        /// </summary>
        public int Total => HP + Attack + Defense + Speed;

        public EffortValue()
        {
            HP = 0;
            Attack = 0;
            Defense = 0;
            Speed = 0;
        }

        public EffortValue(int hp, int attack, int defense, int speed)
        {
            SetHP(hp);
            SetAttack(attack);
            SetDefense(defense);
            SetSpeed(speed);
        }

        /// <summary>
        /// HP努力値を設定（最大252、合計最大510の制限を考慮）
        /// </summary>
        public void SetHP(int value)
        {
            value = System.Math.Max(0, System.Math.Min(252, value));
            int currentTotal = Total - HP;
            int maxAllowed = 510 - currentTotal;
            HP = System.Math.Min(value, maxAllowed);
        }

        /// <summary>
        /// 攻撃努力値を設定（最大252、合計最大510の制限を考慮）
        /// </summary>
        public void SetAttack(int value)
        {
            value = System.Math.Max(0, System.Math.Min(252, value));
            int currentTotal = Total - Attack;
            int maxAllowed = 510 - currentTotal;
            Attack = System.Math.Min(value, maxAllowed);
        }

        /// <summary>
        /// 防御努力値を設定（最大252、合計最大510の制限を考慮）
        /// </summary>
        public void SetDefense(int value)
        {
            value = System.Math.Max(0, System.Math.Min(252, value));
            int currentTotal = Total - Defense;
            int maxAllowed = 510 - currentTotal;
            Defense = System.Math.Min(value, maxAllowed);
        }

        /// <summary>
        /// 素早さ努力値を設定（最大252、合計最大510の制限を考慮）
        /// </summary>
        public void SetSpeed(int value)
        {
            value = System.Math.Max(0, System.Math.Min(252, value));
            int currentTotal = Total - Speed;
            int maxAllowed = 510 - currentTotal;
            Speed = System.Math.Min(value, maxAllowed);
        }
    }
}

