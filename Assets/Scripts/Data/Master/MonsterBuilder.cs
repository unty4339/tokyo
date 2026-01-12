using System;
using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// Monsterを作成するためのクラス
    /// ClubMemberからの変換とは別に直接作成するロジックもいる
    /// </summary>
    public class MonsterBuilder
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// ClubMemberからMonsterを作成
        /// </summary>
        public static Monster CreateFromClubMember(ClubMember member)
        {
            if (member == null)
            {
                return null;
            }

            // ClubMemberのSpeciesからSpeciesを作成
            Species species = null;
            if (member.Species != null)
            {
                species = new Species(
                    member.Species.Name,
                    member.Species.BaseHP,
                    member.Species.BaseAttack,
                    member.Species.BaseDefense,
                    member.Species.BaseSpeed
                );
            }

            // Monsterを作成
            Monster monster = new Monster(
                species,
                member.Level,
                member.IV,
                member.EV,
                member.Personality,
                member.Traits,
                member.Skills
            );

            return monster;
        }

        /// <summary>
        /// 直接Monsterを作成
        /// </summary>
        public static Monster CreateRandom(Species species = null)
        {
            if (species == null)
            {
                species = new Species(
                    "モンスター",
                    80,
                    60,
                    60,
                    60
                );
            }

            // ランダムなレベル（1-50）
            int level = random.Next(1, 51);
            Level levelInstance = new Level(level, 0);

            // ランダムな個体値
            IndividualValue iv = new IndividualValue(
                random.Next(0, 32),
                random.Next(0, 32),
                random.Next(0, 32),
                random.Next(0, 32)
            );

            // ランダムな努力値
            EffortValue ev = new EffortValue(
                random.Next(0, 253),
                random.Next(0, 253),
                random.Next(0, 253),
                random.Next(0, 253)
            );

            // ランダムな性格
            Personality personality = (Personality)random.Next(0, 5);

            // ランダムな特性（0-2個）
            List<Trait> traits = new List<Trait>();
            int traitCount = random.Next(0, 3);
            for (int i = 0; i < traitCount; i++)
            {
                traits.Add(new BasicTrait($"特性{i + 1}", $"特性{i + 1}の説明"));
            }

            // ランダムなスキル（1-4個）
            List<Skill> skills = new List<Skill>();
            int skillCount = random.Next(1, 5);
            for (int i = 0; i < skillCount; i++)
            {
                var move = new AttackMove($"スキル{i + 1}", 30 + i * 10, 1, $"スキル{i + 1}による攻撃", new MovePriority("最初の敵を攻撃"), BattleAttribute.Melee);
                skills.Add(new ActiveSkill(1, $"スキル{i + 1}", 0, move));
            }

            // Monsterを作成
            Monster monster = new Monster(
                species,
                levelInstance,
                iv,
                ev,
                personality,
                traits,
                skills
            );

            return monster;
        }
    }
}
