using System.Linq;

namespace MonsterBattleGame
{
    /// <summary>
    /// 技
    /// </summary>
    public class Skill
    {
        /// <summary>名称</summary>
        public string Name { get; set; }

        /// <summary>攻撃力</summary>
        public int Power { get; set; }

        /// <summary>攻撃範囲</summary>
        public AttackRange Range { get; set; }

        /// <summary>クールタイム</summary>
        public int Cooldown { get; set; }

        public Skill()
        {
            Name = string.Empty;
        }

        public Skill(string name, int power, AttackRange range, int cooldown)
        {
            Name = name;
            Power = power;
            Range = range;
            Cooldown = cooldown;
        }

        /// <summary>
        /// 技を使用できるかチェック
        /// </summary>
        public bool CanUse(Monster monster)
        {
            if (monster == null)
            {
                return false;
            }

            // クールタイムが残っている場合は使用不可
            if (monster.SkillCooldowns != null && monster.SkillCooldowns.ContainsKey(this))
            {
                int remainingCooldown = monster.SkillCooldowns[this];
                if (remainingCooldown > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

