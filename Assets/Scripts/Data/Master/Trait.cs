using System.Collections.Generic;
using System.Linq;

namespace MonsterBattleGame
{
    /// <summary>
    /// 特性
    /// </summary>
    public class Trait
    {
        /// <summary>名称</summary>
        public string Name { get; set; }

        /// <summary>発動条件のリスト</summary>
        public List<TraitCondition> Conditions { get; set; }

        /// <summary>効果のリスト</summary>
        public List<TraitEffect> Effects { get; set; }

        public Trait()
        {
            Name = string.Empty;
            Conditions = new List<TraitCondition>();
            Effects = new List<TraitEffect>();
        }

        public Trait(string name)
        {
            Name = name;
            Conditions = new List<TraitCondition>();
            Effects = new List<TraitEffect>();
        }

        /// <summary>
        /// 条件をチェック
        /// </summary>
        public bool CheckConditions(Monster monster, BattleContext context)
        {
            if (Conditions == null || Conditions.Count == 0)
            {
                return true; // 条件がない場合は常に発動可能
            }

            return Conditions.All(condition => condition.IsSatisfied(monster, context));
        }

        /// <summary>
        /// 効果を適用
        /// </summary>
        public void ApplyEffects(Monster monster, BattleContext context)
        {
            if (Effects == null || Effects.Count == 0)
            {
                return;
            }

            foreach (var effect in Effects)
            {
                effect.Apply(monster, context);
            }
        }
    }
}

