namespace MonsterBattleGame
{
    /// <summary>
    /// 個体値 (Individual Values / IVs)
    /// モンスター一匹ごとに設定された才能の差
    /// 範囲: 0 ～ 31 （32進数で管理されるため）
    /// </summary>
    public class IndividualValue
    {
        private int _hp;
        private int _attack;
        private int _defense;
        private int _speed;

        /// <summary>HP個体値（0-31の範囲）</summary>
        public int HP
        {
            get => _hp;
            set => _hp = System.Math.Max(0, System.Math.Min(31, value));
        }

        /// <summary>攻撃個体値（0-31の範囲）</summary>
        public int Attack
        {
            get => _attack;
            set => _attack = System.Math.Max(0, System.Math.Min(31, value));
        }

        /// <summary>防御個体値（0-31の範囲）</summary>
        public int Defense
        {
            get => _defense;
            set => _defense = System.Math.Max(0, System.Math.Min(31, value));
        }

        /// <summary>素早さ個体値（0-31の範囲）</summary>
        public int Speed
        {
            get => _speed;
            set => _speed = System.Math.Max(0, System.Math.Min(31, value));
        }

        public IndividualValue()
        {
            _hp = 0;
            _attack = 0;
            _defense = 0;
            _speed = 0;
        }

        public IndividualValue(int hp, int attack, int defense, int speed)
        {
            HP = hp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
        }
    }
}

