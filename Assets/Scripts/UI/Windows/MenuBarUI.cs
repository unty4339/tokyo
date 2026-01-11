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

        [Header("Time Progress Bar")]
        [SerializeField] private TimeProgressBar weekProgressBar;

        [Header("Time Scale Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button speed1xButton;
        [SerializeField] private Button speed2xButton;
        [SerializeField] private Button speed3xButton;

        [Header("Incident Settings")]
        [SerializeField] private Toggle autoPauseToggle;
        [SerializeField] private Toggle autoOpenWindowToggle;

        private GameResourceManager resourceManager;
        private GameTimeManager timeManager;
        private ScreenManager screenManager;
        private IncidentManager incidentManager;
        private IncidentUI incidentUI;
        private int currentSpeed = 1;
        
        // トグルボタンの状態
        private bool isAutoPauseEnabled = false;
        private bool isAutoOpenWindowEnabled = false;
        private bool hasOpenedWindowForCurrentIncidents = false;

        private void Awake()
        {
            resourceManager = GameResourceManager.Instance;
            timeManager = GameTimeManager.Instance;
            screenManager = ScreenManager.Instance;
            incidentManager = IncidentManager.Instance;
            incidentUI = FindFirstObjectByType<IncidentUI>();

            // 資源変更イベントを購読
            if (resourceManager != null)
            {
                resourceManager.OnReputationChanged += UpdateReputationDisplay;
                resourceManager.OnMoneyChanged += UpdateMoneyDisplay;
            }

            // インシデント発生イベントを購読
            if (incidentManager != null)
            {
                incidentManager.OnIncidentOccurred += OnIncidentOccurred;
            }

            // 週変更イベントを購読（フラグリセット用）
            if (timeManager != null)
            {
                timeManager.OnWeekChanged += OnWeekChanged;
            }

            // 画面遷移ボタンのイベント設定
            SetupScreenButtons();

            // 時間経過スケール変更ボタンのイベント設定
            SetupTimeScaleButtons();

            // トグルボタンのイベント設定
            SetupToggleButtons();
        }

        private void OnDestroy()
        {
            // イベントの購読を解除
            if (resourceManager != null)
            {
                resourceManager.OnReputationChanged -= UpdateReputationDisplay;
                resourceManager.OnMoneyChanged -= UpdateMoneyDisplay;
            }

            if (incidentManager != null)
            {
                incidentManager.OnIncidentOccurred -= OnIncidentOccurred;
            }

            if (timeManager != null)
            {
                timeManager.OnWeekChanged -= OnWeekChanged;
            }
        }

        private void Update()
        {
            // 日付表示を更新
            if (dateText != null && timeManager != null)
            {
                dateText.text = timeManager.GetTimeString();
            }

            // 週プログレスバーを更新
            if (weekProgressBar != null && timeManager != null)
            {
                float progress = timeManager.GetWeekProgress();
                weekProgressBar.SetProgress(progress);
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
        /// トグルボタンのイベントを設定
        /// </summary>
        private void SetupToggleButtons()
        {
            if (autoPauseToggle != null)
            {
                autoPauseToggle.onValueChanged.AddListener(OnAutoPauseToggleChanged);
                isAutoPauseEnabled = autoPauseToggle.isOn;
            }

            if (autoOpenWindowToggle != null)
            {
                autoOpenWindowToggle.onValueChanged.AddListener(OnAutoOpenWindowToggleChanged);
                isAutoOpenWindowEnabled = autoOpenWindowToggle.isOn;
            }
        }

        /// <summary>
        /// 自動ポーズトグルの値が変更されたときの処理
        /// </summary>
        private void OnAutoPauseToggleChanged(bool isOn)
        {
            isAutoPauseEnabled = isOn;
        }

        /// <summary>
        /// 自動ウィンドウオープントグルの値が変更されたときの処理
        /// </summary>
        private void OnAutoOpenWindowToggleChanged(bool isOn)
        {
            isAutoOpenWindowEnabled = isOn;
            // トグルがオフになったら、フラグをリセット（次回のインシデント発生時にウィンドウを開けるように）
            if (!isOn)
            {
                hasOpenedWindowForCurrentIncidents = false;
            }
        }

        /// <summary>
        /// 週が変わったときの処理（フラグリセット用）
        /// </summary>
        private void OnWeekChanged(int year, int month, int week)
        {
            // 週が変わったら、ウィンドウオープンフラグをリセット
            hasOpenedWindowForCurrentIncidents = false;
        }

        /// <summary>
        /// インシデントが発生したときの処理
        /// </summary>
        private void OnIncidentOccurred(IncidentState state)
        {
            if (state == null)
            {
                return;
            }

            // 自動ポーズ機能
            if (isAutoPauseEnabled && timeManager != null)
            {
                timeManager.Pause();
            }

            // 自動ウィンドウオープン機能
            // 複数インシデントが同時発生した場合は最初の1つだけ開く
            if (isAutoOpenWindowEnabled && incidentUI != null && !hasOpenedWindowForCurrentIncidents)
            {
                incidentUI.OpenWindow(state);
                hasOpenedWindowForCurrentIncidents = true;
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

