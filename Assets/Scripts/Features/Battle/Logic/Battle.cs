using System.Collections.Generic;
using System.Linq;

namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘
    /// </summary>
    public class Battle
    {
        /// <summary>プレイヤーチーム</summary>
        public Team PlayerTeam { get; set; }

        /// <summary>敵チーム</summary>
        public Team EnemyTeam { get; set; }

        /// <summary>行動順序（素早さ順）</summary>
        public List<Monster> ActionOrder { get; private set; }

        /// <summary>現在のターン数</summary>
        public int CurrentTurn { get; private set; }

        /// <summary>戦闘状態</summary>
        public BattleState State { get; private set; }

        /// <summary>現在行動中のモンスターのインデックス</summary>
        private int currentActionIndex;

        public Battle()
        {
            ActionOrder = new List<Monster>();
            State = BattleState.NotStarted;
            CurrentTurn = 0;
            currentActionIndex = 0;
        }

        public Battle(Team playerTeam, Team enemyTeam)
        {
            PlayerTeam = playerTeam;
            EnemyTeam = enemyTeam;
            ActionOrder = new List<Monster>();
            State = BattleState.NotStarted;
            CurrentTurn = 0;
            currentActionIndex = 0;
        }

        /// <summary>
        /// 戦闘開始時の初期化
        /// </summary>
        public void Initialize()
        {
            if (PlayerTeam == null || EnemyTeam == null)
            {
                return;
            }

            // 各モンスターのステータスを計算
            foreach (var monster in PlayerTeam.Monsters)
            {
                monster.CalculateStats();
                monster.InitializeForBattle();
            }

            foreach (var monster in EnemyTeam.Monsters)
            {
                monster.CalculateStats();
                monster.InitializeForBattle();
            }

            // 行動順序を計算
            CalculateActionOrder();

            State = BattleState.InProgress;
            CurrentTurn = 1;
            currentActionIndex = 0;
        }

        /// <summary>
        /// 行動順序を計算
        /// </summary>
        public void CalculateActionOrder()
        {
            ActionOrder.Clear();

            // 両チームのすべてのモンスターを取得
            var allMonsters = new List<Monster>();
            if (PlayerTeam != null)
            {
                allMonsters.AddRange(PlayerTeam.Monsters);
            }
            if (EnemyTeam != null)
            {
                allMonsters.AddRange(EnemyTeam.Monsters);
            }

            // 素早さ順にソート（同じ素早さの場合はランダム）
            ActionOrder = allMonsters
                .Where(m => !m.IsDefeated())
                .OrderByDescending(m => m.CalculatedSpeed)
                .ThenBy(m => System.Guid.NewGuid()) // 同速の場合はランダム
                .ToList();
        }

        /// <summary>
        /// ターン実行
        /// </summary>
        public void ExecuteTurn()
        {
            if (State != BattleState.InProgress)
            {
                return;
            }

            // 10ターン制限チェック（CurrentTurnは1から始まるので、>10で判定）
            if (CurrentTurn > 10)
            {
                State = BattleState.PlayerLost;
                return;
            }

            // 行動順序を再計算（戦闘不能になったモンスターを除外）
            CalculateActionOrder();

            if (ActionOrder.Count == 0)
            {
                return;
            }

            // 各モンスターが行動
            var monstersToAct = new List<Monster>(ActionOrder);
            foreach (var attacker in monstersToAct)
            {
                if (attacker.IsDefeated())
                {
                    continue;
                }

                // クールタイムを進める
                attacker.ReduceCooldowns();

                // 攻撃対象を決定
                Monster target = SelectTarget(attacker);
                if (target != null)
                {
                    // 使用する技を決定（先頭から順に使用可能な技を探す）
                    Skill skillToUse = null;
                    foreach (var skill in attacker.Skills)
                    {
                        if (skill.CanUse(attacker))
                        {
                            skillToUse = skill;
                            break;
                        }
                    }

                    if (skillToUse != null)
                    {
                        UseSkill(attacker, target, skillToUse);
                    }
                }

                // 勝敗判定
                if (CheckBattleEnd())
                {
                    return;
                }
            }

            CurrentTurn++;
            currentActionIndex = 0;
        }

        /// <summary>
        /// 攻撃対象を選択
        /// </summary>
        private Monster SelectTarget(Monster attacker)
        {
            // 攻撃者がどのチームに属するか判定
            bool isPlayerMonster = PlayerTeam != null && PlayerTeam.Monsters.Contains(attacker);
            Team targetTeam = isPlayerMonster ? EnemyTeam : PlayerTeam;

            if (targetTeam == null)
            {
                return null;
            }

            var activeTargets = targetTeam.GetActiveMonsters();
            if (activeTargets.Count == 0)
            {
                return null;
            }

            // 簡易的に最初の生きているモンスターをターゲットにする
            // 実際のゲームでは、AIやプレイヤー入力に応じて選択
            return activeTargets[0];
        }

        /// <summary>
        /// 技を使用
        /// </summary>
        public void UseSkill(Monster attacker, Monster target, Skill skill)
        {
            if (attacker == null || target == null || skill == null)
            {
                return;
            }

            if (!skill.CanUse(attacker))
            {
                return;
            }

            // ダメージ計算
            int damage = CalculateDamage(attacker, target, skill);
            target.TakeDamage(damage);

            // 全体攻撃の場合は敵チーム全員にダメージ
            var activeSkill = skill as ActiveSkill;
            if (activeSkill != null && activeSkill.Move is AttackMove attackMove && attackMove.TargetCount > 1)
            {
                bool isPlayerMonster = PlayerTeam != null && PlayerTeam.Monsters.Contains(attacker);
                Team enemyTeam = isPlayerMonster ? EnemyTeam : PlayerTeam;

                if (enemyTeam != null)
                {
                    foreach (var enemy in enemyTeam.GetActiveMonsters())
                    {
                        if (enemy != target)
                        {
                            int allDamage = CalculateDamage(attacker, enemy, skill);
                            enemy.TakeDamage(allDamage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ダメージ計算
        /// </summary>
        private int CalculateDamage(Monster attacker, Monster target, Skill skill)
        {
            // 簡易的なダメージ計算式
            // 実際のゲームではより複雑な計算式を使用することもあります
            var activeSkill = skill as ActiveSkill;
            int power = 0;
            if (activeSkill?.Move is AttackMove attackMove)
            {
                power = attackMove.Power;
            }
            int baseDamage = attacker.CalculatedAttack + power - target.CalculatedDefense;
            return System.Math.Max(1, baseDamage); // 最低1ダメージ
        }

        /// <summary>
        /// 勝敗判定
        /// </summary>
        public bool CheckBattleEnd()
        {
            // 10ターン制限チェック
            if (CurrentTurn > 10)
            {
                State = BattleState.PlayerLost;
                return true;
            }

            if (PlayerTeam.IsDefeated())
            {
                State = BattleState.PlayerLost;
                return true;
            }

            if (EnemyTeam.IsDefeated())
            {
                State = BattleState.PlayerWon;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 戦闘状態を設定
        /// </summary>
        public void SetState(BattleState newState)
        {
            State = newState;
        }

        /// <summary>
        /// ターンを増やす
        /// </summary>
        public void IncrementTurn()
        {
            CurrentTurn++;
        }

        /// <summary>
        /// 10ターン制限をチェック
        /// </summary>
        public bool CheckTurnLimit(int maxTurns)
        {
            if (CurrentTurn > maxTurns)
            {
                State = BattleState.PlayerLost;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 戦闘結果を取得
        /// </summary>
        public BattleResult GetBattleResult()
        {
            var playerHP = new Dictionary<Monster, int>();
            var enemyHP = new Dictionary<Monster, int>();

            if (PlayerTeam != null)
            {
                foreach (var monster in PlayerTeam.Monsters)
                {
                    playerHP[monster] = monster.CurrentHP;
                }
            }

            if (EnemyTeam != null)
            {
                foreach (var monster in EnemyTeam.Monsters)
                {
                    enemyHP[monster] = monster.CurrentHP;
                }
            }

            return new BattleResult(State, playerHP, enemyHP, CurrentTurn);
        }
    }
}

