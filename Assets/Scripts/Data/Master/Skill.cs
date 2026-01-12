using System.Linq;

namespace MonsterBattleGame
{
    /// <summary>
    /// 技の抽象クラス
    /// </summary>
    public abstract class Skill
    {
        /// <summary>レベルを示すint</summary>
        public int Level { get; set; }

        /// <summary>名称</summary>
        public string Name { get; set; }

        public Skill()
        {
            Level = 1;
            Name = string.Empty;
        }

        public Skill(int level, string name)
        {
            Level = level;
            Name = name;
        }

        /// <summary>
        /// 説明文を返すメソッド
        /// </summary>
        public abstract string GetDescription();

        /// <summary>
        /// 技を使用できるかチェック
        /// </summary>
        public virtual bool CanUse(Monster monster)
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

