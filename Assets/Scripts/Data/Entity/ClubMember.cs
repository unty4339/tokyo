using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// 部員（クラブメンバー）
    /// モンスターと戦うための主人公側の勢力
    /// 各部員は個別の種族値・個体値・努力値を持つ想定
    /// </summary>
    public class ClubMember
    {
        /// <summary>種別への参照（種族値）</summary>
        public Species Species { get; set; }

        /// <summary>苗字</summary>
        public string LastName { get; set; }

        /// <summary>名前</summary>
        public string FirstName { get; set; }

        /// <summary>全名（苗字 + 名前）</summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(FirstName))
                {
                    return "無名の部員";
                }
                return $"{LastName} {FirstName}";
            }
        }

        /// <summary>学年</summary>
        public Grade Grade { get; set; }

        /// <summary>レベルインスタンス</summary>
        public Level Level { get; set; }

        /// <summary>個体値</summary>
        public IndividualValue IV { get; set; }

        /// <summary>努力値</summary>
        public EffortValue EV { get; set; }

        /// <summary>特性のリスト</summary>
        public List<Trait> Traits { get; set; }

        /// <summary>人格</summary>
        public Personality Personality { get; set; }

        /// <summary>経歴</summary>
        public Career Career { get; set; }

        /// <summary>最終的な末路（在籍中はnull）</summary>
        public Fate? Fate { get; set; }

        /// <summary>ステータス</summary>
        public Status Status { get; set; }

        /// <summary>覚えている技のリスト（最大2つ）</summary>
        public List<Skill> Skills { get; set; }

        /// <summary>計算された最大HP</summary>
        public int CalculatedHP { get; private set; }

        /// <summary>計算された攻撃力</summary>
        public int CalculatedAttack { get; private set; }

        /// <summary>計算された防御力</summary>
        public int CalculatedDefense { get; private set; }

        /// <summary>計算された素早さ</summary>
        public int CalculatedSpeed { get; private set; }

        public ClubMember()
        {
            Level = new Level(1, 0);
            LastName = string.Empty;
            FirstName = string.Empty;
            IV = new IndividualValue();
            EV = new EffortValue();
            Personality = Personality.Normal;
            Traits = new List<Trait>();
            Career = new Career();
            Fate = null;
            Status = new Status();
            Skills = new List<Skill>();
        }

        public ClubMember(Species species, Grade grade, Level level, IndividualValue iv, EffortValue ev, Personality personality, List<Trait> traits, Career career, List<Skill> skills, string lastName = "", string firstName = "")
        {
            if (species == null)
            {
                throw new System.ArgumentNullException(nameof(species), "Speciesは必須です");
            }
            Species = species;
            Grade = grade;
            Level = level ?? new Level(1, 0);
            LastName = lastName ?? string.Empty;
            FirstName = firstName ?? string.Empty;
            IV = iv ?? new IndividualValue();
            EV = ev ?? new EffortValue();
            Personality = personality;
            Traits = traits ?? new List<Trait>();
            Career = career ?? new Career();
            Fate = null;
            Skills = skills != null ? (skills.Count > 2 ? skills.GetRange(0, 2) : skills) : new List<Skill>();
            CalculateStats();
            UpdateStatus();
        }

        /// <summary>
        /// ステータスを計算
        /// 仕様書の計算式に基づいて実装（Monsterと同じ計算式、性格補正なし）
        /// HP: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + Lv + 10
        /// その他: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
        /// </summary>
        public void CalculateStats()
        {
            if (Species == null)
            {
                return;
            }

            if (IV == null)
            {
                IV = new IndividualValue();
            }

            if (EV == null)
            {
                EV = new EffortValue();
            }

            // HP計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + Lv + 10
            int evHpDiv4 = (int)System.Math.Floor(EV.HP / 4.0);
            int hpInner = (Species.BaseHP * 2) + IV.HP + evHpDiv4;
            int hpCalc = (int)System.Math.Floor((hpInner * Level.CurrentLevel) / 100.0);
            CalculatedHP = hpCalc + Level.CurrentLevel + 10;

            // 攻撃計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evAttackDiv4 = (int)System.Math.Floor(EV.Attack / 4.0);
            int attackInner = (Species.BaseAttack * 2) + IV.Attack + evAttackDiv4;
            int attackCalc = (int)System.Math.Floor((attackInner * Level.CurrentLevel) / 100.0);
            CalculatedAttack = attackCalc + 5;

            // 防御計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evDefenseDiv4 = (int)System.Math.Floor(EV.Defense / 4.0);
            int defenseInner = (Species.BaseDefense * 2) + IV.Defense + evDefenseDiv4;
            int defenseCalc = (int)System.Math.Floor((defenseInner * Level.CurrentLevel) / 100.0);
            CalculatedDefense = defenseCalc + 5;

            // 素早さ計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evSpeedDiv4 = (int)System.Math.Floor(EV.Speed / 4.0);
            int speedInner = (Species.BaseSpeed * 2) + IV.Speed + evSpeedDiv4;
            int speedCalc = (int)System.Math.Floor((speedInner * Level.CurrentLevel) / 100.0);
            CalculatedSpeed = speedCalc + 5;
        }

        /// <summary>
        /// Statusを更新
        /// </summary>
        public void UpdateStatus()
        {
            if (Species != null && Level != null)
            {
                Status = Status.CalculateFromLevelAndSpecies(Level, Species, IV, EV);
            }
        }

        /// <summary>
        /// あるTraitを持っているかboolで返すメソッド
        /// </summary>
        public bool HasTrait(Trait trait)
        {
            if (trait == null || Traits == null)
            {
                return false;
            }

            return Traits.Contains(trait);
        }

        /// <summary>
        /// モンスターに変換
        /// 部員クラスを元にモンスターを作成
        /// 努力値と性格補正も反映される
        /// </summary>
        public Monster ToMonster()
        {
            if (Species == null)
            {
                throw new System.InvalidOperationException("Speciesが設定されていません");
            }

            // 部員の種族値からSpeciesを作成
            Species species = new Species(Species.Name, Species.BaseHP, Species.BaseAttack, Species.BaseDefense, Species.BaseSpeed);

            // 部員の全ての特性をMonsterに反映
            List<Trait> monsterTraits = new List<Trait>(Traits);

            // 部員のレベル、IV、努力値、性格補正、技をMonsterに反映
            Monster monster = new Monster(species, Level, IV, EV, Personality, monsterTraits, Skills);
            
            return monster;
        }
    }
}

