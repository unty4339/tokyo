using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘テスト用コントローラー
    /// </summary>
    public class BattleTestController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BattleUI battleUI;
        [SerializeField] private Button testButton;

        private void Start()
        {
            // 戦闘UIを取得（自動検索）
            if (battleUI == null)
            {
                battleUI = FindFirstObjectByType<BattleUI>();
            }

            // テストボタンを設定
            if (testButton != null)
            {
                testButton.onClick.AddListener(OnTestButtonClick);
            }
            else
            {
                // ボタンがない場合は自動検索（自分自身のボタンを探す）
                testButton = GetComponent<Button>();
                if (testButton != null)
                {
                    testButton.onClick.AddListener(OnTestButtonClick);
                }
            }
        }

        /// <summary>
        /// テストボタンクリック時の処理
        /// </summary>
        public void OnTestButtonClick()
        {
            // BattleUIを再検索（Start()で見つからなかった場合）
            if (battleUI == null)
            {
                battleUI = FindFirstObjectByType<BattleUI>();
            }

            if (battleUI == null)
            {
                Debug.LogError("BattleUI is not assigned! Please create Battle UI first.");
                return;
            }

            // ダミーチームで戦闘を開始
            var playerTeam = DummyDataFactory.CreatePlayerTeam();
            var enemyTeam = DummyDataFactory.CreateEnemyTeam();

            battleUI.StartBattle(playerTeam, enemyTeam, OnBattleEnd);
        }

        /// <summary>
        /// 戦闘終了時のコールバック
        /// </summary>
        private void OnBattleEnd(BattleResult result)
        {
            Debug.Log($"戦闘終了: {result.Result}, ターン数: {result.TurnCount}");
            
            // 戦闘結果は現在は特に使用しない（ログ出力のみ）
            // 将来的にはリザルト画面などに使用可能
        }
    }
}

