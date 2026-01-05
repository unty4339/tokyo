using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// 所有しているモンスター
    /// </summary>
    public class Monster
    {
        /// <summary>種別への参照</summary>
        public MonsterSpecies Species { get; set; }

        /// <summary>レベル</summary>
        public int Level { get; set; }

        /// <summary>個体値</summary>
        public IndividualValue IV { get; set; }

        /// <summary>努力値</summary>
        public EffortValue EV { get; set; }

        /// <summary>性格補正</summary>
        public Personality Personality { get; set; }

        /// <summary>特性のリスト</summary>
        public List<Trait> Traits { get; set; }

        /// <summary>特性（互換性のため、Traitsの最初の要素を返す）</summary>
        public Trait Trait
        {
            get
            {
                if (Traits == null || Traits.Count == 0)
                {
                    return null;
                }
                return Traits[0];
            }
            set
            {
                if (Traits == null)
                {
                    Traits = new List<Trait>();
                }
                if (value != null)
                {
                    if (Traits.Count == 0)
                    {
                        Traits.Add(value);
                    }
                    else
                    {
                        Traits[0] = value;
                    }
                }
                else if (Traits.Count > 0)
                {
                    Traits.RemoveAt(0);
                }
            }
        }

        /// <summary>覚えている技のリスト</summary>
        public List<Skill> Skills { get; set; }

        /// <summary>現在のHP（戦闘中）</summary>
        public int CurrentHP { get; set; }

        /// <summary>技ごとのクールタイム状態（戦闘中）</summary>
        public Dictionary<Skill, int> SkillCooldowns { get; set; }

        /// <summary>計算された最大HP</summary>
        public int CalculatedHP { get; private set; }

        /// <summary>計算された攻撃力</summary>
        public int CalculatedAttack { get; private set; }

        /// <summary>計算された防御力</summary>
        public int CalculatedDefense { get; private set; }

        /// <summary>計算された素早さ</summary>
        public int CalculatedSpeed { get; private set; }

        public Monster()
        {
            Level = 1;
            IV = new IndividualValue();
            EV = new EffortValue();
            Personality = new Personality();
            Traits = new List<Trait>();
            Skills = new List<Skill>();
            SkillCooldowns = new Dictionary<Skill, int>();
        }

        public Monster(MonsterSpecies species, int level, IndividualValue iv, Trait trait, List<Skill> skills)
        {
            Species = species;
            Level = level;
            IV = iv ?? new IndividualValue();
            EV = new EffortValue();
            Personality = new Personality();
            Traits = new List<Trait>();
            if (trait != null)
            {
                Traits.Add(trait);
            }
            Skills = skills ?? new List<Skill>();
            SkillCooldowns = new Dictionary<Skill, int>();
            CalculateStats();
            CurrentHP = CalculatedHP;
        }

        public Monster(MonsterSpecies species, int level, IndividualValue iv, List<Trait> traits, List<Skill> skills)
        {
            Species = species;
            Level = level;
            IV = iv ?? new IndividualValue();
            EV = new EffortValue();
            Personality = new Personality();
            Traits = traits ?? new List<Trait>();
            Skills = skills ?? new List<Skill>();
            SkillCooldowns = new Dictionary<Skill, int>();
            CalculateStats();
            CurrentHP = CalculatedHP;
        }

        public Monster(MonsterSpecies species, int level, IndividualValue iv, EffortValue ev, Personality personality, List<Trait> traits, List<Skill> skills)
        {
            Species = species;
            Level = level;
            IV = iv ?? new IndividualValue();
            EV = ev ?? new EffortValue();
            Personality = personality ?? new Personality();
            Traits = traits ?? new List<Trait>();
            Skills = skills ?? new List<Skill>();
            SkillCooldowns = new Dictionary<Skill, int>();
            CalculateStats();
            CurrentHP = CalculatedHP;
        }

        /// <summary>
        /// ステータスを計算
        /// 仕様書の計算式に基づいて実装（性格補正なし）
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
        /// 戦闘開始時の初期化
        /// </summary>
        public void InitializeForBattle()
        {
            CurrentHP = CalculatedHP;
            SkillCooldowns.Clear();
            foreach (var skill in Skills)
            {
                SkillCooldowns[skill] = 0;
            }
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        public void TakeDamage(int damage)
        {
            CurrentHP = System.Math.Max(0, CurrentHP - damage);
        }

        /// <summary>
        /// 戦闘不能かチェック
        /// </summary>
        public bool IsDefeated()
        {
            return CurrentHP <= 0;
        }

        /// <summary>
        /// クールタイムを進める
        /// </summary>
        public void ReduceCooldowns()
        {
            var skillsToUpdate = new List<Skill>(SkillCooldowns.Keys);
            foreach (var skill in skillsToUpdate)
            {
                if (SkillCooldowns[skill] > 0)
                {
                    SkillCooldowns[skill]--;
                }
            }
        }
    }
}

