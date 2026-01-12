using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// ステータス値を格納するクラス
    /// 最大HP、現HP、SP、攻撃力、防御力、素早さの値を格納する
    /// </summary>
    public class Status
    {
        /// <summary>最大HP</summary>
        public int MaxHP { get; set; }

        /// <summary>現在のHP</summary>
        public int CurrentHP { get; set; }

        /// <summary>SP</summary>
        public int SP { get; set; }

        /// <summary>攻撃力</summary>
        public int Attack { get; set; }

        /// <summary>防御力</summary>
        public int Defense { get; set; }

        /// <summary>素早さ</summary>
        public int Speed { get; set; }

        public Status()
        {
            MaxHP = 0;
            CurrentHP = 0;
            SP = 0;
            Attack = 0;
            Defense = 0;
            Speed = 0;
        }

        public Status(int maxHP, int currentHP, int sp, int attack, int defense, int speed)
        {
            MaxHP = maxHP;
            CurrentHP = currentHP;
            SP = sp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
        }

        /// <summary>
        /// LevelとSpeciesクラスから動的に計算更新する
        /// </summary>
        public static Status CalculateFromLevelAndSpecies(Level level, Species species, IndividualValue iv, EffortValue ev)
        {
            if (species == null || level == null)
            {
                return new Status();
            }

            if (iv == null)
            {
                iv = new IndividualValue();
            }

            if (ev == null)
            {
                ev = new EffortValue();
            }

            int currentLevel = level.CurrentLevel;

            // HP計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + Lv + 10
            int evHpDiv4 = (int)System.Math.Floor(ev.HP / 4.0);
            int hpInner = (species.BaseHP * 2) + iv.HP + evHpDiv4;
            int hpCalc = (int)System.Math.Floor((hpInner * currentLevel) / 100.0);
            int maxHP = hpCalc + currentLevel + 10;

            // 攻撃計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evAttackDiv4 = (int)System.Math.Floor(ev.Attack / 4.0);
            int attackInner = (species.BaseAttack * 2) + iv.Attack + evAttackDiv4;
            int attackCalc = (int)System.Math.Floor((attackInner * currentLevel) / 100.0);
            int attack = attackCalc + 5;

            // 防御計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evDefenseDiv4 = (int)System.Math.Floor(ev.Defense / 4.0);
            int defenseInner = (species.BaseDefense * 2) + iv.Defense + evDefenseDiv4;
            int defenseCalc = (int)System.Math.Floor((defenseInner * currentLevel) / 100.0);
            int defense = defenseCalc + 5;

            // 素早さ計算式: ⌊((Base × 2 + IV + ⌊EV/4⌋) × Lv) / 100⌋ + 5
            int evSpeedDiv4 = (int)System.Math.Floor(ev.Speed / 4.0);
            int speedInner = (species.BaseSpeed * 2) + iv.Speed + evSpeedDiv4;
            int speedCalc = (int)System.Math.Floor((speedInner * currentLevel) / 100.0);
            int speed = speedCalc + 5;

            return new Status(maxHP, maxHP, 0, attack, defense, speed);
        }


        /// <summary>
        /// WeaponとTraitのリストを受け取って修正後のStatusを計算する
        /// </summary>
        public Status ApplyWeaponAndTraits(Weapon weapon, System.Collections.Generic.List<Trait> traits)
        {
            Status result = new Status(MaxHP, CurrentHP, SP, Attack, Defense, Speed);

            // Weaponの効果を適用（実装は後で）
            if (weapon != null)
            {
                // Weaponによるステータス補正を適用
            }

            // Traitの効果を適用（実装は後で）
            if (traits != null)
            {
                foreach (var trait in traits)
                {
                    if (trait != null)
                    {
                        // Traitによるステータス補正を適用
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Buffのリストを受け取って修正後のStatusを計算する
        /// </summary>
        public Status ApplyBuffs(System.Collections.Generic.List<Buff> buffs)
        {
            Status result = new Status(MaxHP, CurrentHP, SP, Attack, Defense, Speed);

            if (buffs != null)
            {
                foreach (var buff in buffs)
                {
                    if (buff != null)
                    {
                        StatusDelta delta = buff.GetStatusDelta(this);
                        result.MaxHP += delta.HP;
                        result.CurrentHP += delta.HP;
                        result.SP += delta.SP;
                        result.Attack += delta.Attack;
                        result.Defense += delta.Defense;
                        result.Speed += delta.Speed;
                    }
                }
            }

            return result;
        }
    }
}
