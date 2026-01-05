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
        public ClubMemberSpecies Species { get; set; }

        /// <summary>学年</summary>
        public Grade Grade { get; set; }

        /// <summary>レベル</summary>
        public int Level { get; set; }

        /// <summary>個体値</summary>
        public IndividualValue IV { get; set; }

        /// <summary>努力値</summary>
        public EffortValue EV { get; set; }

        /// <summary>特性のリスト</summary>
        public List<Trait> Traits { get; set; }

        /// <summary>人格</summary>
        public Personality Personality { get; set; }

        /// <summary>経歴</summary>
        public History History { get; set; }

        /// <summary>覚えている技のリスト</summary>
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
            Level = 1;
            IV = new IndividualValue();
            EV = new EffortValue();
            Personality = new Personality();
            Traits = new List<Trait>();
            Skills = new List<Skill>();
        }

        public ClubMember(ClubMemberSpecies species, Grade grade, int level, IndividualValue iv, EffortValue ev, Personality personality, List<Trait> traits, History history, List<Skill> skills)
        {
            Species = species;
            Grade = grade;
            Level = level;
            IV = iv ?? new IndividualValue();
            EV = ev ?? new EffortValue();
            Personality = personality ?? new Personality();
            Traits = traits ?? new List<Trait>();
            History = history;
            Skills = skills ?? new List<Skill>();
            CalculateStats();
        }

        // 後方互換性のためのコンストラクタ（既存コードとの互換性）
        public ClubMember(Grade grade, int level, IndividualValue iv, List<Trait> traits, Personality personality, History history, List<Skill> skills)
        {
            Grade = grade;
            Level = level;
            IV = iv ?? new IndividualValue();
            EV = new EffortValue();
            Personality = personality ?? new Personality();
            Traits = traits ?? new List<Trait>();
            History = history;
            Skills = skills ?? new List<Skill>();
            CalculateStats();
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
                // Speciesが設定されていない場合は、デフォルト値を使用（後方互換性のため）
                int baseHP = 100;
                int baseAttack = 50;
                int baseDefense = 50;
                int baseSpeed = 50;

                // 学年による補正（学年が上がるほどステータスが上がる）
                int gradeBonus = (int)Grade * 10;

                CalculatedHP = baseHP + (Level * 10) + (IV?.HP ?? 0) * 2 + gradeBonus;
                CalculatedAttack = baseAttack + (Level * 2) + (IV?.Attack ?? 0) * 1 + gradeBonus;
                CalculatedDefense = baseDefense + (Level * 2) + (IV?.Defense ?? 0) * 1 + gradeBonus;
                CalculatedSpeed = baseSpeed + (Level * 1) + (IV?.Speed ?? 0) * 1 + gradeBonus;
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
            int hpCalc = (int)System.Math.Floor((hpInner * Level) / 100.0);
            CalculatedHP = hpCalc + Level + 10;

            // 攻撃計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evAttackDiv4 = (int)System.Math.Floor(EV.Attack / 4.0);
            int attackInner = (Species.BaseAttack * 2) + IV.Attack + evAttackDiv4;
            int attackCalc = (int)System.Math.Floor((attackInner * Level) / 100.0);
            CalculatedAttack = attackCalc + 5;

            // 防御計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evDefenseDiv4 = (int)System.Math.Floor(EV.Defense / 4.0);
            int defenseInner = (Species.BaseDefense * 2) + IV.Defense + evDefenseDiv4;
            int defenseCalc = (int)System.Math.Floor((defenseInner * Level) / 100.0);
            CalculatedDefense = defenseCalc + 5;

            // 素早さ計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evSpeedDiv4 = (int)System.Math.Floor(EV.Speed / 4.0);
            int speedInner = (Species.BaseSpeed * 2) + IV.Speed + evSpeedDiv4;
            int speedCalc = (int)System.Math.Floor((speedInner * Level) / 100.0);
            CalculatedSpeed = speedCalc + 5;
        }

        /// <summary>
        /// モンスターに変換
        /// 部員クラスを元にモンスターを作成
        /// 努力値と性格補正も反映される
        /// </summary>
        public Monster ToMonster()
        {
            // 部員の種族値からMonsterSpeciesを作成
            MonsterSpecies species;
            if (Species != null)
            {
                species = new MonsterSpecies(Species.Name, Species.BaseHP, Species.BaseAttack, Species.BaseDefense, Species.BaseSpeed);
            }
            else
            {
                // Speciesが設定されていない場合は、計算済みステータスから逆算（後方互換性のため）
                int baseHP = CalculatedHP - (Level * 10) - ((IV?.HP ?? 0) * 2);
                int baseAttack = CalculatedAttack - (Level * 2) - ((IV?.Attack ?? 0) * 1);
                int baseDefense = CalculatedDefense - (Level * 2) - ((IV?.Defense ?? 0) * 1);
                int baseSpeed = CalculatedSpeed - (Level * 1) - ((IV?.Speed ?? 0) * 1);

                // 負の値にならないように調整
                baseHP = System.Math.Max(1, baseHP);
                baseAttack = System.Math.Max(1, baseAttack);
                baseDefense = System.Math.Max(1, baseDefense);
                baseSpeed = System.Math.Max(1, baseSpeed);

                species = new MonsterSpecies("部員", baseHP, baseAttack, baseDefense, baseSpeed);
            }

            // 部員の全ての特性をMonsterに反映
            List<Trait> monsterTraits = new List<Trait>(Traits);

            // 部員のレベル、IV、努力値、性格補正、技をMonsterに反映
            Monster monster = new Monster(species, Level, IV, EV, Personality, monsterTraits, Skills);
            
            return monster;
        }
    }
}

