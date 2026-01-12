using System;
using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// Weaponインスタンスを作成するためのクラス
    /// </summary>
    public class WeaponBuilder
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// ランダムなWeaponを作成
        /// </summary>
        public static Weapon CreateRandom()
        {
            string[] weaponNames = { "木の剣", "鉄の剣", "鋼の剣", "魔法の剣", "伝説の剣" };
            string name = weaponNames[random.Next(weaponNames.Length)];

            // ランダムなレアリティ
            WeaponRarity rarity = (WeaponRarity)random.Next(0, 4);

            // ランダムな属性
            BattleAttribute attribute = (BattleAttribute)random.Next(0, 3);

            // ランダムなAttackMove
            AttackMove attackMove = new AttackMove(
                $"{name}の攻撃",
                30 + random.Next(0, 70),
                1 + random.Next(0, 3),
                $"{name}による攻撃",
                new MovePriority("最初の敵を攻撃"),
                attribute
            );

            // ランダムなスキル（1-3個）
            List<Skill> skills = new List<Skill>();
            int skillCount = random.Next(1, 4);
            for (int i = 0; i < skillCount; i++)
            {
                var move = new AttackMove($"武器スキル{i + 1}", 20 + i * 10, 1, $"武器スキル{i + 1}による攻撃", new MovePriority("最初の敵を攻撃"), attribute);
                skills.Add(new ActiveSkill(1, $"武器スキル{i + 1}", 0, move));
            }

            string description = $"{name}は{rarity}レアリティの{attribute}属性の武器です。";

            return new Weapon(name, attackMove, rarity, attribute, skills, description);
        }

        /// <summary>
        /// 指定されたパラメータでWeaponを作成
        /// </summary>
        public static Weapon Create(string name, WeaponRarity rarity, BattleAttribute attribute, int power, List<Skill> skills = null)
        {
            AttackMove attackMove = new AttackMove(
                $"{name}の攻撃",
                power,
                1,
                $"{name}による攻撃",
                new MovePriority("最初の敵を攻撃"),
                attribute
            );

            if (skills == null || skills.Count == 0)
            {
                var move = new AttackMove($"{name}の基本攻撃", power, 1, $"{name}による基本攻撃", new MovePriority("最初の敵を攻撃"), attribute);
                skills = new List<Skill> { new ActiveSkill(1, $"{name}の基本攻撃", 0, move) };
            }

            string description = $"{name}は{rarity}レアリティの{attribute}属性の武器です。";

            return new Weapon(name, attackMove, rarity, attribute, skills, description);
        }
    }
}
