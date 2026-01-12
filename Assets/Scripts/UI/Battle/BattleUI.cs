using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘画面のUI制御
    /// </summary>
    public class BattleUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject battleWindow;
        [SerializeField] private Button skipButton;
        [SerializeField] private Text turnText;
        [SerializeField] private Text logText;
        [SerializeField] private ScrollRect logScrollRect;
        [SerializeField] private Transform playerTeamArea;
        [SerializeField] private Transform enemyTeamArea;

        [Header("Monster Display Prefab")]
        [SerializeField] private GameObject monsterDisplayPrefab;

        private Battle currentBattle;
        private System.Action<BattleResult> onBattleEnd;
        private bool isSkipping = false;
        private const int maxTurns = 10;
        private List<GameObject> playerMonsterDisplays = new List<GameObject>();
        private List<GameObject> enemyMonsterDisplays = new List<GameObject>();

        /// <summary>
        /// フォントを取得（日本語対応）
        /// </summary>
        private static Font GetFont()
        {
            // Assets/Fonts以下の日本語フォントを読み込む
            Font font = Resources.Load<Font>("Fonts/NotoSansJP-Medium");
            if (font == null)
            {
                // フォントが見つからない場合は、LegacyRuntime.ttfを使用
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return font;
        }

        private void Awake()
        {
            if (battleWindow != null)
            {
                battleWindow.SetActive(false);
            }

            if (skipButton != null)
            {
                skipButton.onClick.AddListener(SkipBattle);
            }
        }

        /// <summary>
        /// 戦闘を開始
        /// </summary>
        public void StartBattle(Team playerTeam, Team enemyTeam, System.Action<BattleResult> callback)
        {
            if (battleWindow == null)
            {
                Debug.LogError("BattleWindow is not assigned!");
                return;
            }

            onBattleEnd = callback;
            isSkipping = false;

            // 戦闘インスタンスを作成
            currentBattle = new Battle(playerTeam, enemyTeam);
            currentBattle.Initialize();

            // UI表示
            battleWindow.SetActive(true);
            UpdateUI();

            // モンスター表示を作成
            CreateMonsterDisplays(playerTeam, enemyTeam);

            // ターン実行開始
            StartCoroutine(ExecuteBattleSequence());
        }

        /// <summary>
        /// モンスター表示を作成
        /// </summary>
        private void CreateMonsterDisplays(Team playerTeam, Team enemyTeam)
        {
            // 既存の表示を削除
            ClearMonsterDisplays();

            // プレイヤーチーム表示
            if (playerTeam != null && playerTeamArea != null)
            {
                foreach (var monster in playerTeam.Monsters)
                {
                    CreateMonsterDisplay(monster, playerTeamArea, playerMonsterDisplays);
                }
            }

            // 敵チーム表示
            if (enemyTeam != null && enemyTeamArea != null)
            {
                foreach (var monster in enemyTeam.Monsters)
                {
                    CreateMonsterDisplay(monster, enemyTeamArea, enemyMonsterDisplays);
                }
            }
        }

        /// <summary>
        /// モンスター表示を作成
        /// </summary>
        private void CreateMonsterDisplay(Monster monster, Transform parent, List<GameObject> displayList)
        {
            if (monsterDisplayPrefab == null)
            {
                // プレハブがない場合は簡易表示を作成
                GameObject display = new GameObject($"Monster_{monster.Species.Name}");
                display.transform.SetParent(parent, false);

                // テキストコンポーネントを追加
                Text nameText = display.AddComponent<Text>();
                nameText.text = $"{monster.Species.Name}\nLv.{monster.Level}\nHP: {monster.CurrentHP}/{monster.CalculatedHP}";
                nameText.font = GetFont();
                nameText.fontSize = 14;
                nameText.alignment = TextAnchor.MiddleCenter;

                // RectTransform設定
                RectTransform rectTransform = display.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(150, 100);

                displayList.Add(display);
            }
            else
            {
                GameObject display = Instantiate(monsterDisplayPrefab, parent);
                displayList.Add(display);
                // プレハブの場合は、MonsterDisplayコンポーネントがあれば初期化
                // ここでは簡易実装のため、直接テキストを設定
            }
        }

        /// <summary>
        /// モンスター表示をクリア
        /// </summary>
        private void ClearMonsterDisplays()
        {
            foreach (var display in playerMonsterDisplays)
            {
                if (display != null)
                {
                    Destroy(display);
                }
            }
            playerMonsterDisplays.Clear();

            foreach (var display in enemyMonsterDisplays)
            {
                if (display != null)
                {
                    Destroy(display);
                }
            }
            enemyMonsterDisplays.Clear();
        }

        /// <summary>
        /// 戦闘シーケンスを実行
        /// </summary>
        private IEnumerator ExecuteBattleSequence()
        {
            while (currentBattle != null && currentBattle.State == BattleState.InProgress)
            {
                if (isSkipping)
                {
                    // スキップ中はログ出力付きで高速実行（勝敗がつくまで実行）
                    while (currentBattle.State == BattleState.InProgress && currentBattle.CurrentTurn <= maxTurns)
                    {
                        // 10ターン制限チェック
                        if (currentBattle.CheckTurnLimit(maxTurns))
                        {
                            break;
                        }

                        yield return StartCoroutine(ExecuteTurnWithLogs());
                        
                        // スキップ中も1フレーム待機してフリーズを防止
                        yield return null;
                    }
                }
                else
                {
                    // アニメーション付きでターン実行
                    yield return StartCoroutine(ExecuteTurnWithAnimation());

                    // ターン間の待機
                    yield return StartCoroutine(WaitForSecondsOrSkip(0.5f));
                    if (isSkipping) continue;
                }

                UpdateUI();
            }

            // 戦闘終了
            OnBattleEnd();
        }

        /// <summary>
        /// 指定時間待機、ただしスキップフラグが立った場合は即座に終了
        /// </summary>
        /// <param name="duration">待機時間（秒）</param>
        private IEnumerator WaitForSecondsOrSkip(float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration && !isSkipping)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// ターンを実行（ログ出力付き）
        /// </summary>
        private IEnumerator ExecuteTurnWithLogs()
        {
            if (currentBattle == null)
            {
                yield break;
            }

            // 行動順序を再計算
            currentBattle.CalculateActionOrder();

            // 各モンスターが行動
            var actionOrder = currentBattle.ActionOrder.ToList();
            foreach (var attacker in actionOrder)
            {
                if (attacker.IsDefeated() || currentBattle.State != BattleState.InProgress)
                {
                    continue;
                }

                // クールタイムを進める
                attacker.ReduceCooldowns();

                // 攻撃対象を決定
                Monster target = SelectTarget(attacker);
                if (target == null)
                {
                    continue;
                }

                // 使用する技を決定
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
                    // 技使用のログ
                    AddLog($"{attacker.Species.Name}は「{skillToUse.Name}」を使った！");

                    // ダメージ前のHPを保存
                    int targetHPBefore = target.CurrentHP;

                    // 技を使用
                    currentBattle.UseSkill(attacker, target, skillToUse);

                    // ダメージ表示
                    int damage = targetHPBefore - target.CurrentHP;
                    if (damage > 0)
                    {
                        AddLog($"{target.Species.Name}に{damage}のダメージ！");
                        if (!isSkipping)
                        {
                            yield return StartCoroutine(WaitForSecondsOrSkip(0.5f));
                        }
                    }

                    // モンスター表示を更新
                    UpdateMonsterDisplays();
                    if (!isSkipping)
                    {
                        yield return StartCoroutine(WaitForSecondsOrSkip(0.3f));
                    }

                    // 戦闘不能判定
                    if (target.IsDefeated())
                    {
                        AddLog($"{target.Species.Name}は戦闘不能になった！");
                        if (!isSkipping)
                        {
                            yield return StartCoroutine(WaitForSecondsOrSkip(0.5f));
                        }
                    }

                    // 勝敗判定
                    if (currentBattle.CheckBattleEnd())
                    {
                        break;
                    }
                }
            }

            // ターン数を増やす
            if (currentBattle.State == BattleState.InProgress)
            {
                currentBattle.IncrementTurn();

                // 10ターン制限チェック
                if (currentBattle.CheckTurnLimit(maxTurns))
                {
                    AddLog("10ターン経過しました。敗北です。");
                }
            }
        }

        /// <summary>
        /// ターンをアニメーション付きで実行
        /// </summary>
        private IEnumerator ExecuteTurnWithAnimation()
        {
            yield return StartCoroutine(ExecuteTurnWithLogs());
        }

        /// <summary>
        /// 攻撃対象を選択（簡易実装）
        /// </summary>
        private Monster SelectTarget(Monster attacker)
        {
            if (currentBattle == null)
            {
                return null;
            }

            bool isPlayerMonster = currentBattle.PlayerTeam != null && 
                                   currentBattle.PlayerTeam.Monsters.Contains(attacker);
            Team targetTeam = isPlayerMonster ? currentBattle.EnemyTeam : currentBattle.PlayerTeam;

            if (targetTeam == null)
            {
                return null;
            }

            var activeTargets = targetTeam.GetActiveMonsters();
            if (activeTargets.Count == 0)
            {
                return null;
            }

            // 簡易的に最初の生きているモンスターをターゲット
            return activeTargets[0];
        }

        /// <summary>
        /// UIを更新
        /// </summary>
        private void UpdateUI()
        {
            if (currentBattle == null)
            {
                return;
            }

            // ターン表示を更新
            if (turnText != null)
            {
                turnText.text = $"ターン: {currentBattle.CurrentTurn} / {maxTurns}";
            }

            // モンスター表示を更新
            UpdateMonsterDisplays();
        }

        /// <summary>
        /// モンスター表示を更新
        /// </summary>
        private void UpdateMonsterDisplays()
        {
            if (currentBattle == null)
            {
                return;
            }

            // プレイヤーチーム表示を更新
            if (currentBattle.PlayerTeam != null && playerMonsterDisplays.Count > 0)
            {
                for (int i = 0; i < currentBattle.PlayerTeam.Monsters.Count && i < playerMonsterDisplays.Count; i++)
                {
                    UpdateMonsterDisplay(currentBattle.PlayerTeam.Monsters[i], playerMonsterDisplays[i]);
                }
            }

            // 敵チーム表示を更新
            if (currentBattle.EnemyTeam != null && enemyMonsterDisplays.Count > 0)
            {
                for (int i = 0; i < currentBattle.EnemyTeam.Monsters.Count && i < enemyMonsterDisplays.Count; i++)
                {
                    UpdateMonsterDisplay(currentBattle.EnemyTeam.Monsters[i], enemyMonsterDisplays[i]);
                }
            }
        }

        /// <summary>
        /// モンスター表示を更新
        /// </summary>
        private void UpdateMonsterDisplay(Monster monster, GameObject display)
        {
            if (display == null || monster == null)
            {
                return;
            }

            Text text = display.GetComponent<Text>();
            if (text != null)
            {
                string status = monster.IsDefeated() ? "（戦闘不能）" : "";
                text.text = $"{monster.Species.Name}{status}\nLv.{monster.Level}\nHP: {monster.CurrentHP}/{monster.CalculatedHP}";
                
                // 戦闘不能の場合は色を変更
                if (monster.IsDefeated())
                {
                    text.color = Color.gray;
                }
                else
                {
                    text.color = Color.white;
                }
            }
        }

        /// <summary>
        /// ログを追加
        /// </summary>
        private void AddLog(string message)
        {
            if (logText != null)
            {
                logText.text += message + "\n";
                
                // レイアウトを強制更新してテキストのサイズを計算
                Canvas.ForceUpdateCanvases();
                
                // スクロールを最下部に移動（最新のログが見えるように）
                if (logScrollRect != null)
                {
                    // 次のフレームでスクロール位置を更新（レイアウト計算後に実行）
                    StartCoroutine(ScrollToBottom());
                }
            }
            else
            {
                Debug.Log(message);
            }
        }

        /// <summary>
        /// スクロールを最下部に移動
        /// </summary>
        private System.Collections.IEnumerator ScrollToBottom()
        {
            yield return null; // 1フレーム待機してレイアウト計算を完了させる
            
            if (logScrollRect != null)
            {
                // verticalNormalizedPosition = 0 で最下部
                logScrollRect.verticalNormalizedPosition = 0f;
            }
        }

        /// <summary>
        /// 戦闘をスキップ
        /// </summary>
        public void SkipBattle()
        {
            isSkipping = true;
            if (skipButton != null)
            {
                skipButton.interactable = false;
            }
        }

        /// <summary>
        /// 戦闘終了処理
        /// </summary>
        private void OnBattleEnd()
        {
            if (currentBattle == null)
            {
                return;
            }

            // 最終結果を表示
            BattleResult result = currentBattle.GetBattleResult();
            string resultMessage = result.Result == BattleState.PlayerWon ? "勝利！" : "敗北...";
            AddLog($"\n=== 戦闘終了 ===\n{resultMessage}");

            // コールバックを呼び出し
            if (onBattleEnd != null)
            {
                onBattleEnd(result);
            }

            // UIを非表示にする（自動で閉じるか、ボタンで閉じるかは要件次第）
            // battleWindow.SetActive(false);
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        public void CloseWindow()
        {
            if (battleWindow != null)
            {
                battleWindow.SetActive(false);
            }
            ClearMonsterDisplays();
        }
    }
}

