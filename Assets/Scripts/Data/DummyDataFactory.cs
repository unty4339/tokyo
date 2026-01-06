using System;
using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// ダミーデータを生成するファクトリークラス
    /// テストや開発用のダミーデータを簡単に作成できます
    /// </summary>
    public static class DummyDataFactory
    {
        private static readonly Random random = new Random();

        #region モンスター種別生成

        /// <summary>
        /// ダミーのモンスター種別を作成
        /// </summary>
        public static MonsterSpecies CreateDummySpecies(string name, int baseHP, int baseAttack, int baseDefense, int baseSpeed)
        {
            return new MonsterSpecies(name, baseHP, baseAttack, baseDefense, baseSpeed);
        }

        /// <summary>
        /// デフォルトのモンスター種別リストを作成
        /// </summary>
        public static List<MonsterSpecies> CreateDefaultSpecies()
        {
            return new List<MonsterSpecies>
            {
                // ファイター型（攻撃重視）
                new MonsterSpecies("ファイター", 80, 120, 60, 80),
                // タンク型（防御・HP重視）
                new MonsterSpecies("ガーディアン", 150, 60, 120, 50),
                // スピード型（素早さ重視）
                new MonsterSpecies("スカウト", 70, 80, 50, 140),
                // バランス型（全体的にバランス）
                new MonsterSpecies("バランサー", 100, 90, 90, 90),
                // 攻撃特化型（攻撃と素早さ重視）
                new MonsterSpecies("アサシン", 65, 130, 45, 130)
            };
        }

        /// <summary>
        /// デフォルトのモンスター種別を名前で取得
        /// </summary>
        public static MonsterSpecies GetDefaultSpeciesByName(string name)
        {
            var speciesList = CreateDefaultSpecies();
            return speciesList.Find(s => s.Name == name) ?? speciesList[0];
        }

        #endregion

        #region 個体値生成

        /// <summary>
        /// ランダムな個体値を作成
        /// </summary>
        public static IndividualValue CreateRandomIV(int min = 0, int max = 100)
        {
            return new IndividualValue(
                random.Next(min, max + 1),
                random.Next(min, max + 1),
                random.Next(min, max + 1),
                random.Next(min, max + 1)
            );
        }

        /// <summary>
        /// 平均的な個体値を作成
        /// </summary>
        public static IndividualValue CreateAverageIV()
        {
            return new IndividualValue(50, 50, 50, 50);
        }

        /// <summary>
        /// 最大個体値を作成
        /// </summary>
        public static IndividualValue CreateMaxIV()
        {
            return new IndividualValue(100, 100, 100, 100);
        }

        /// <summary>
        /// カスタム個体値を作成
        /// </summary>
        public static IndividualValue CreateIV(int hp, int attack, int defense, int speed)
        {
            return new IndividualValue(hp, attack, defense, speed);
        }

        #endregion

        #region 技生成

        /// <summary>
        /// ダミーの技を作成
        /// </summary>
        public static Skill CreateDummySkill(string name, int power, AttackRange range, int cooldown)
        {
            return new Skill(name, power, range, cooldown);
        }

        /// <summary>
        /// デフォルトの技リストを作成
        /// </summary>
        public static List<Skill> CreateDefaultSkills()
        {
            return new List<Skill>
            {
                // 単体攻撃技
                new Skill("パンチ", 30, AttackRange.Single, 0),
                new Skill("キック", 40, AttackRange.Single, 0),
                new Skill("ストライク", 50, AttackRange.Single, 1),
                new Skill("ヘビーブロー", 60, AttackRange.Single, 2),

                // 全体攻撃技
                new Skill("エリアブラスト", 35, AttackRange.All, 2),
                new Skill("メガブラスト", 45, AttackRange.All, 3),

                // 特殊技
                new Skill("連撃", 25, AttackRange.Single, 1),
                new Skill("必殺技", 80, AttackRange.Single, 4)
            };
        }

        /// <summary>
        /// 技を名前で取得
        /// </summary>
        public static Skill GetSkillByName(string name)
        {
            var skills = CreateDefaultSkills();
            return skills.Find(s => s.Name == name) ?? skills[0];
        }

        /// <summary>
        /// モンスター用のデフォルト技セットを作成（3-4技）
        /// </summary>
        public static List<Skill> CreateDefaultSkillSet()
        {
            var allSkills = CreateDefaultSkills();
            return new List<Skill>
            {
                allSkills[0], // パンチ
                allSkills[1], // キック
                allSkills[2], // ストライク
                allSkills[4]  // エリアブラスト
            };
        }

        #endregion

        #region 特性生成

        /// <summary>
        /// ダミーの特性を作成
        /// </summary>
        public static Trait CreateDummyTrait(string name, List<TraitCondition> conditions = null, List<TraitEffect> effects = null)
        {
            var trait = new Trait(name);
            if (conditions != null)
            {
                trait.Conditions = conditions;
            }
            if (effects != null)
            {
                trait.Effects = effects;
            }
            return trait;
        }

        /// <summary>
        /// デフォルトの特性リストを作成
        /// </summary>
        public static List<Trait> CreateDefaultTraits()
        {
            return new List<Trait>
            {
                // HP回復型特性
                CreateDummyTrait(
                    "自己回復",
                    new List<TraitCondition> { new TraitCondition(ConditionType.HPBelow, 0.5f) },
                    new List<TraitEffect> { new TraitEffect(EffectType.Heal, 0.2f) }
                ),

                // 攻撃強化型特性
                CreateDummyTrait(
                    "戦闘態勢",
                    new List<TraitCondition> { new TraitCondition(ConditionType.HPAbove, 0.8f) },
                    new List<TraitEffect> { new TraitEffect(EffectType.AttackBoost, 1.1f) }
                ),

                // 防御強化型特性
                CreateDummyTrait(
                    "堅守",
                    new List<TraitCondition> { new TraitCondition(ConditionType.HPBelow, 0.3f) },
                    new List<TraitEffect> { new TraitEffect(EffectType.DefenseBoost, 1.2f) }
                ),

                // 素早さ強化型特性
                CreateDummyTrait(
                    "俊敏",
                    new List<TraitCondition>(),
                    new List<TraitEffect> { new TraitEffect(EffectType.SpeedBoost, 1.1f) }
                ),

                // 特性なし（ダミー）
                CreateDummyTrait("なし", new List<TraitCondition>(), new List<TraitEffect>())
            };
        }

        /// <summary>
        /// 特性を名前で取得
        /// </summary>
        public static Trait GetTraitByName(string name)
        {
            var traits = CreateDefaultTraits();
            return traits.Find(t => t.Name == name) ?? traits[traits.Count - 1]; // デフォルトは「なし」
        }

        #endregion

        #region モンスター生成

        /// <summary>
        /// カスタムモンスターを作成
        /// </summary>
        public static Monster CreateDummyMonster(
            MonsterSpecies species,
            int level,
            IndividualValue iv,
            Trait trait,
            List<Skill> skills)
        {
            return new Monster(species, level, iv, trait, skills);
        }

        /// <summary>
        /// デフォルト構成でモンスターを作成
        /// </summary>
        public static Monster CreateDefaultMonster(string speciesName, int level = 10)
        {
            var species = GetDefaultSpeciesByName(speciesName);
            var iv = CreateAverageIV();
            var trait = GetTraitByName("なし");
            var skills = CreateDefaultSkillSet();

            return new Monster(species, level, iv, trait, skills);
        }

        /// <summary>
        /// ランダム構成でモンスターを作成
        /// </summary>
        public static Monster CreateRandomMonster(int level = 10)
        {
            var speciesList = CreateDefaultSpecies();
            var species = speciesList[random.Next(speciesList.Count)];

            var iv = CreateRandomIV(30, 70);
            var traitList = CreateDefaultTraits();
            var trait = traitList[random.Next(traitList.Count)];

            var allSkills = CreateDefaultSkills();
            var skillCount = random.Next(3, 5);
            var skills = new List<Skill>();
            for (int i = 0; i < skillCount; i++)
            {
                var skill = allSkills[random.Next(allSkills.Count)];
                if (!skills.Contains(skill))
                {
                    skills.Add(skill);
                }
            }

            return new Monster(species, level, iv, trait, skills);
        }

        /// <summary>
        /// 強力なモンスターを作成（最大個体値、強力な技）
        /// </summary>
        public static Monster CreateStrongMonster(string speciesName, int level = 20)
        {
            var species = GetDefaultSpeciesByName(speciesName);
            var iv = CreateMaxIV();
            var trait = GetTraitByName("戦闘態勢");
            var skills = new List<Skill>
            {
                GetSkillByName("ストライク"),
                GetSkillByName("ヘビーブロー"),
                GetSkillByName("必殺技"),
                GetSkillByName("メガブラスト")
            };

            return new Monster(species, level, iv, trait, skills);
        }

        #endregion

        #region チーム生成

        /// <summary>
        /// カスタムチームを作成
        /// </summary>
        public static Team CreateCustomTeam(List<Monster> monsters)
        {
            return new Team(monsters);
        }

        /// <summary>
        /// プレイヤーチームを作成（バランス型、3体）
        /// </summary>
        public static Team CreatePlayerTeam(int level = 10)
        {
            var monsters = new List<Monster>
            {
                CreateDefaultMonster("ファイター", level),
                CreateDefaultMonster("ガーディアン", level),
                CreateDefaultMonster("スカウト", level)
            };

            return new Team(monsters);
        }

        /// <summary>
        /// 敵チームを作成（バランス型、3体）
        /// </summary>
        public static Team CreateEnemyTeam(int level = 10)
        {
            var monsters = new List<Monster>
            {
                CreateDefaultMonster("バランサー", level),
                CreateDefaultMonster("アサシン", level),
                CreateDefaultMonster("ファイター", level)
            };

            return new Team(monsters);
        }

        /// <summary>
        /// ランダムな敵チームを作成
        /// </summary>
        public static Team CreateRandomEnemyTeam(int level = 10)
        {
            var monsters = new List<Monster>
            {
                CreateRandomMonster(level),
                CreateRandomMonster(level),
                CreateRandomMonster(level)
            };

            return new Team(monsters);
        }

        /// <summary>
        /// 強力な敵チームを作成
        /// </summary>
        public static Team CreateStrongEnemyTeam(int level = 20)
        {
            var monsters = new List<Monster>
            {
                CreateStrongMonster("アサシン", level),
                CreateStrongMonster("ファイター", level),
                CreateStrongMonster("スカウト", level)
            };

            return new Team(monsters);
        }

        #endregion

        #region 部員生成

        /// <summary>
        /// カスタム部員を作成
        /// </summary>
        public static ClubMember CreateDummyClubMember(
            Grade grade,
            int level,
            IndividualValue iv,
            List<Trait> traits,
            Personality personality,
            History history,
            List<Skill> skills)
        {
            return new ClubMember(grade, level, iv, traits, personality, history, skills);
        }

        /// <summary>
        /// デフォルト構成で部員を作成
        /// </summary>
        public static ClubMember CreateDefaultClubMember(Grade grade = Grade.FirstYear, int level = 10, string lastName = "", string firstName = "")
        {
            var iv = CreateAverageIV();
            var traits = new List<Trait> { GetTraitByName("なし") };
            var personality = new Personality();
            var history = new History();
            var skills = CreateDefaultSkillSet();

            // 部員の名前が指定されている場合、ClubMemberSpeciesを作成
            ClubMemberSpecies species = null;
            if (!string.IsNullOrEmpty(lastName) || !string.IsNullOrEmpty(firstName))
            {
                string fullName = string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName) 
                    ? "部員" 
                    : $"{lastName} {firstName}".Trim();
                species = new ClubMemberSpecies(fullName, 100, 50, 50, 50);
            }

            var member = new ClubMember(grade, level, iv, traits, personality, history, skills, lastName, firstName);
            if (species != null)
            {
                member.Species = species;
            }

            return member;
        }

        /// <summary>
        /// ランダム構成で部員を作成
        /// </summary>
        public static ClubMember CreateRandomClubMember(int level = 10, string lastName = "", string firstName = "")
        {
            // ランダムな学年を選択
            var grades = new[] { Grade.FirstYear, Grade.SecondYear, Grade.ThirdYear };
            var grade = grades[random.Next(grades.Length)];

            var iv = CreateRandomIV(30, 70);
            
            // ランダムな特性を1-3個選択
            var traitList = CreateDefaultTraits();
            var traitCount = random.Next(1, 4);
            var traits = new List<Trait>();
            for (int i = 0; i < traitCount; i++)
            {
                var trait = traitList[random.Next(traitList.Count)];
                if (!traits.Contains(trait))
                {
                    traits.Add(trait);
                }
            }
            if (traits.Count == 0)
            {
                traits.Add(GetTraitByName("なし"));
            }

            var personality = new Personality();
            var history = new History();

            var allSkills = CreateDefaultSkills();
            var skillCount = random.Next(3, 5);
            var skills = new List<Skill>();
            for (int i = 0; i < skillCount; i++)
            {
                var skill = allSkills[random.Next(allSkills.Count)];
                if (!skills.Contains(skill))
                {
                    skills.Add(skill);
                }
            }

            // 部員の名前が指定されている場合、ClubMemberSpeciesを作成
            ClubMemberSpecies species = null;
            if (!string.IsNullOrEmpty(lastName) || !string.IsNullOrEmpty(firstName))
            {
                string fullName = string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName) 
                    ? "部員" 
                    : $"{lastName} {firstName}".Trim();
                species = new ClubMemberSpecies(fullName, 100, 50, 50, 50);
            }

            var member = new ClubMember(grade, level, iv, traits, personality, history, skills, lastName, firstName);
            if (species != null)
            {
                member.Species = species;
            }

            return member;
        }

        #endregion

        #region 戦闘生成

        /// <summary>
        /// テスト用の戦闘を作成
        /// </summary>
        public static Battle CreateTestBattle()
        {
            var playerTeam = CreatePlayerTeam();
            var enemyTeam = CreateEnemyTeam();
            var battle = new Battle(playerTeam, enemyTeam);
            battle.Initialize();
            return battle;
        }

        /// <summary>
        /// カスタム戦闘を作成
        /// </summary>
        public static Battle CreateCustomBattle(Team playerTeam, Team enemyTeam)
        {
            var battle = new Battle(playerTeam, enemyTeam);
            battle.Initialize();
            return battle;
        }

        /// <summary>
        /// 強力な敵とのテスト戦闘を作成
        /// </summary>
        public static Battle CreateHardTestBattle(int playerLevel = 10, int enemyLevel = 20)
        {
            var playerTeam = CreatePlayerTeam(playerLevel);
            var enemyTeam = CreateStrongEnemyTeam(enemyLevel);
            var battle = new Battle(playerTeam, enemyTeam);
            battle.Initialize();
            return battle;
        }

        #endregion
    }
}

