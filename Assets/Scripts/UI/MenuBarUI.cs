using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 画面下部のメニューバーを制御するUI
    /// </summary>
    public class MenuBarUI : MonoBehaviour
    {
        [Header("Resource Display")]
        [SerializeField] private Text reputationText;
        [SerializeField] private Text moneyText;

        [Header("Screen Navigation Buttons")]
        [SerializeField] private Button homeButton;
        [SerializeField] private Button clubPolicyButton;
        [SerializeField] private Button memberListButton;
        [SerializeField] private Button itemButton;
        [SerializeField] private Button researchButton;
        [SerializeField] private Button achievementButton;
        [SerializeField] private Button systemButton;

        [Header("Date Display")]
        [SerializeField] private Text dateText;

        [Header("Time Scale Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button speed1xButton;
        [SerializeField] private Button speed2xButton;
        [SerializeField] private Button speed3xButton;

        private GameResourceManager resourceManager;
        private GameTimeManager timeManager;
        private ScreenManager screenManager;
        private int currentSpeed = 1;

        private void Awake()
        {
            resourceManager = GameResourceManager.Instance;
            timeManager = GameTimeManager.Instance;
            screenManager = ScreenManager.Instance;

            // 資源変更イベントを購読
            if (resourceManager != null)
            {
                resourceManager.OnReputationChanged += UpdateReputationDisplay;
                resourceManager.OnMoneyChanged += UpdateMoneyDisplay;
            }

            // 画面遷移ボタンのイベント設定
            SetupScreenButtons();

            // 時間経過スケール変更ボタンのイベント設定
            SetupTimeScaleButtons();
        }

        private void OnDestroy()
        {
            // イベントの購読を解除
            if (resourceManager != null)
            {
                resourceManager.OnReputationChanged -= UpdateReputationDisplay;
                resourceManager.OnMoneyChanged -= UpdateMoneyDisplay;
            }
        }

        private void Update()
        {
            // 日付表示を更新
            if (dateText != null && timeManager != null)
            {
                dateText.text = timeManager.GetTimeString();
            }

            // 時間経過スケール変更ボタンの状態を更新
            UpdateTimeScaleButtonStates();
        }

        /// <summary>
        /// 画面遷移ボタンのイベントを設定
        /// </summary>
        private void SetupScreenButtons()
        {
            if (homeButton != null)
            {
                homeButton.onClick.AddListener(() => screenManager?.SwitchToScreen(ScreenManager.ScreenType.Home));
            }

            if (clubPolicyButton != null)
            {
                clubPolicyButton.onClick.AddListener(() => screenManager?.SwitchToScreen(ScreenManager.ScreenType.ClubPolicy));
            }

            if (memberListButton != null)
            {
                memberListButton.onClick.AddListener(() => screenManager?.SwitchToScreen(ScreenManager.ScreenType.MemberList));
            }

            if (itemButton != null)
            {
                itemButton.onClick.AddListener(() => screenManager?.SwitchToScreen(ScreenManager.ScreenType.Item));
            }

            if (researchButton != null)
            {
                researchButton.onClick.AddListener(() => screenManager?.SwitchToScreen(ScreenManager.ScreenType.Research));
            }

            if (achievementButton != null)
            {
                achievementButton.onClick.AddListener(() => screenManager?.SwitchToScreen(ScreenManager.ScreenType.Achievement));
            }

            if (systemButton != null)
            {
                systemButton.onClick.AddListener(() => screenManager?.SwitchToScreen(ScreenManager.ScreenType.System));
            }
        }

        /// <summary>
        /// 時間経過スケール変更ボタンのイベントを設定
        /// </summary>
        private void SetupTimeScaleButtons()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(() => SetTimeScale(0));
            }

            if (speed1xButton != null)
            {
                speed1xButton.onClick.AddListener(() => SetTimeScale(1));
            }

            if (speed2xButton != null)
            {
                speed2xButton.onClick.AddListener(() => SetTimeScale(2));
            }

            if (speed3xButton != null)
            {
                speed3xButton.onClick.AddListener(() => SetTimeScale(3));
            }
        }

        /// <summary>
        /// 時間経過速度を設定
        /// </summary>
        private void SetTimeScale(int scale)
        {
            if (timeManager != null)
            {
                timeManager.TimeScale = scale;
                currentSpeed = scale;
            }
        }

        /// <summary>
        /// 時間経過スケール変更ボタンの状態を更新
        /// </summary>
        private void UpdateTimeScaleButtonStates()
        {
            if (timeManager == null)
            {
                return;
            }

            int currentScale = timeManager.TimeScale;

            UpdateButtonColor(pauseButton, currentScale == 0);
            UpdateButtonColor(speed1xButton, currentScale == 1);
            UpdateButtonColor(speed2xButton, currentScale == 2);
            UpdateButtonColor(speed3xButton, currentScale == 3);
        }

        /// <summary>
        /// ボタンの色を更新（選択中は明るく、非選択中は暗く）
        /// </summary>
        private void UpdateButtonColor(Button button, bool isSelected)
        {
            if (button == null)
            {
                return;
            }

            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (isSelected)
                {
                    // 選択中は明るい色
                    buttonImage.color = new Color(0.5f, 0.7f, 0.5f, 1f);
                }
                else
                {
                    // 非選択中は暗い色
                    buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                }
            }
        }

        /// <summary>
        /// 評判表示を更新
        /// </summary>
        private void UpdateReputationDisplay(int reputation)
        {
            if (reputationText != null)
            {
                reputationText.text = $"評判: {reputation}";
            }
        }

        /// <summary>
        /// 資金表示を更新
        /// </summary>
        private void UpdateMoneyDisplay(int money)
        {
            if (moneyText != null)
            {
                moneyText.text = $"資金: {money:N0}円";
            }
        }

        /// <summary>
        /// 初期表示を更新
        /// </summary>
        public void RefreshDisplay()
        {
            if (resourceManager != null)
            {
                UpdateReputationDisplay(resourceManager.Reputation);
                UpdateMoneyDisplay(resourceManager.Money);
            }
        }
    }
}

