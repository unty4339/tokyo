using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 武器を表すクラス
    /// Itemクラスのサブクラス
    /// </summary>
    public class Weapon : Item
    {
        /// <summary>AttackMoveインスタンス</summary>
        public AttackMove AttackMove { get; set; }

        /// <summary>WeaponRarityインスタンス</summary>
        public WeaponRarity Rarity { get; set; }

        /// <summary>BattleAttributeインスタンス</summary>
        public BattleAttribute Attribute { get; set; }

        /// <summary>1つ以上のSkillインスタンス</summary>
        public List<Skill> Skills { get; set; }

        /// <summary>説明文</summary>
        public string Description { get; set; }

        /// <summary>アイコン用ゲームオブジェクト（簡易実装）</summary>
        private GameObject icon;

        public Weapon() : base()
        {
            AttackMove = null;
            Rarity = WeaponRarity.N;
            Attribute = BattleAttribute.Melee;
            Skills = new List<Skill>();
            Description = string.Empty;
            icon = null;
        }

        public Weapon(string name, AttackMove attackMove, WeaponRarity rarity, BattleAttribute attribute, List<Skill> skills, string description) : base(name)
        {
            AttackMove = attackMove;
            Rarity = rarity;
            Attribute = attribute;
            Skills = skills ?? new List<Skill>();
            Description = description;
            icon = null;
        }

        public override GameObject GetIcon()
        {
            // 簡易実装: nullを返す（後で実装）
            return icon;
        }

        public override string GetDescription()
        {
            return Description;
        }

        public override ItemTag GetItemTag()
        {
            return ItemTag.Weapon;
        }
    }
}
