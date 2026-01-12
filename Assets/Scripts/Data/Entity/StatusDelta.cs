namespace MonsterBattleGame
{
    /// <summary>
    /// ステータスの変動量を表すクラス
    /// 負値を許容している
    /// </summary>
    public class StatusDelta
    {
        /// <summary>HP変動量</summary>
        public int HP { get; set; }

        /// <summary>SP変動量</summary>
        public int SP { get; set; }

        /// <summary>攻撃力変動量</summary>
        public int Attack { get; set; }

        /// <summary>防御力変動量</summary>
        public int Defense { get; set; }

        /// <summary>素早さ変動量</summary>
        public int Speed { get; set; }

        /// <summary>ゼロのStatusDelta</summary>
        public static StatusDelta Zero => new StatusDelta(0, 0, 0, 0, 0);

        public StatusDelta()
        {
            HP = 0;
            SP = 0;
            Attack = 0;
            Defense = 0;
            Speed = 0;
        }

        public StatusDelta(int hp, int sp, int attack, int defense, int speed)
        {
            HP = hp;
            SP = sp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
        }

        /// <summary>
        /// StatusDelta * int の演算子オーバーロード
        /// </summary>
        public static StatusDelta operator *(StatusDelta delta, int multiplier)
        {
            return new StatusDelta(
                delta.HP * multiplier,
                delta.SP * multiplier,
                delta.Attack * multiplier,
                delta.Defense * multiplier,
                delta.Speed * multiplier
            );
        }
    }
}
