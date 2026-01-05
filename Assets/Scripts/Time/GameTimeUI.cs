using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// ゲーム内時間を表示し、時間経過速度をコントロールするUI
    /// </summary>
    public class GameTimeUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text timeText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button speed1xButton;
        [SerializeField] private Button speed2xButton;
        [SerializeField] private Button speed3xButton;

        private GameTimeManager timeManager;
        private int currentSpeed = 1; // 現在選択中の速度

        private void Awake()
        {
            timeManager = GameTimeManager.Instance;

            // ボタンのイベントを設定
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

        private void Update()
        {
            // 時間表示を更新
            if (timeText != null && timeManager != null)
            {
                timeText.text = timeManager.GetTimeString();
            }

            // ボタンの状態を更新（現在選択中の速度を視覚的に表示）
            UpdateButtonStates();
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
        /// ボタンの状態を更新（選択中の速度を強調表示）
        /// </summary>
        private void UpdateButtonStates()
        {
            if (timeManager == null)
            {
                return;
            }

            int currentScale = timeManager.TimeScale;

            // 各ボタンの色を更新
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
    }
}

