namespace MonsterBattleGame
{
    /// <summary>
    /// モンスター種別（種族値）
    /// 種族値（Base Stats）のみを保持する
    /// 個体値・努力値・レベル・性格補正はMonsterクラスで管理される
    /// </summary>
    public class MonsterSpecies
    {
        /// <summary>名称</summary>
        public string Name { get; set; }

        /// <summary>基礎体力（種族値）</summary>
        public int BaseHP { get; set; }

        /// <summary>基礎攻撃力（種族値）</summary>
        public int BaseAttack { get; set; }

        /// <summary>基礎防御力（種族値）</summary>
        public int BaseDefense { get; set; }

        /// <summary>基礎素早さ（種族値）</summary>
        public int BaseSpeed { get; set; }

        public MonsterSpecies()
        {
            Name = string.Empty;
        }

        public MonsterSpecies(string name, int baseHP, int baseAttack, int baseDefense, int baseSpeed)
        {
            Name = name;
            BaseHP = baseHP;
            BaseAttack = baseAttack;
            BaseDefense = baseDefense;
            BaseSpeed = baseSpeed;
        }
    }
}

