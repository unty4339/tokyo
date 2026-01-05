namespace MonsterBattleGame
{
    /// <summary>
    /// 部員種別（種族値）
    /// 各部員の種族値（Base Stats）を保持する
    /// 各部員は個別の種族値・個体値・努力値を持つ想定
    /// </summary>
    public class ClubMemberSpecies
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

        public ClubMemberSpecies()
        {
            Name = string.Empty;
        }

        public ClubMemberSpecies(string name, int baseHP, int baseAttack, int baseDefense, int baseSpeed)
        {
            Name = name;
            BaseHP = baseHP;
            BaseAttack = baseAttack;
            BaseDefense = baseDefense;
            BaseSpeed = baseSpeed;
        }
    }
}

