using System;
using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// ClubMemberを作成するためのクラス
    /// メインロジックは一旦置いてDummyでさっくりランダムに作れるようにする
    /// </summary>
    public class ClubMemberBuilder
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// ランダムなClubMemberを作成
        /// </summary>
        public static ClubMember CreateRandom()
        {
            // ランダムな名前を生成
            string[] lastNames = { "山田", "佐藤", "鈴木", "田中", "渡辺", "伊藤", "中村", "小林", "加藤", "吉田" };
            string[] firstNames = { "太郎", "花子", "次郎", "三郎", "美咲", "さくら", "あかり", "みゆき", "ひなた", "あおい" };

            string lastName = lastNames[random.Next(lastNames.Length)];
            string firstName = firstNames[random.Next(firstNames.Length)];

            // ランダムな学年
            Grade grade = (Grade)random.Next(0, 3);

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

            // 経歴
            Career career = new Career(
                random.Next(0, 100),
                random.Next(0, 10),
                random.Next(0, 200),
                random.Next(0, 150)
            );

            // ランダムなスキル（最大2つ）
            List<Skill> skills = new List<Skill>();
            int skillCount = random.Next(1, 3);
            for (int i = 0; i < skillCount; i++)
            {
                var move = new AttackMove($"スキル{i + 1}", 30 + i * 10, 1, $"スキル{i + 1}による攻撃", new MovePriority("最初の敵を攻撃"), BattleAttribute.Melee);
                skills.Add(new ActiveSkill(1, $"スキル{i + 1}", 0, move));
            }

            // ランダムな種族値
            Species species = new Species(
                $"{lastName}{firstName}",
                50 + random.Next(0, 100),
                30 + random.Next(0, 100),
                30 + random.Next(0, 100),
                30 + random.Next(0, 100)
            );

            // ClubMemberを作成
            ClubMember member = new ClubMember(
                species,
                grade,
                levelInstance,
                iv,
                ev,
                personality,
                traits,
                career,
                skills,
                lastName,
                firstName
            );

            return member;
        }
    }
}
