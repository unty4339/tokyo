using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 個別のインシデントアイコンのUIコンポーネント
    /// </summary>
    public class IncidentIconUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Button clickButton;

        /// <summary>
        /// このアイコンが表示しているインシデントプロセス
        /// </summary>
        public IncidentProcess Process { get; private set; }

        /// <summary>
        /// クリック時のコールバック
        /// </summary>
        public System.Action<IncidentProcess> OnIconClicked;

        private void Awake()
        {
            // フィールドが設定されていない場合は自動的に取得
            if (clickButton == null)
            {
                clickButton = GetComponent<Button>();
            }

            if (iconImage == null)
            {
                iconImage = GetComponent<Image>();
            }

            if (clickButton != null)
            {
                clickButton.onClick.AddListener(OnButtonClicked);
            }
        }

        /// <summary>
        /// インシデントプロセスを設定
        /// </summary>
        public void SetIncidentProcess(IncidentProcess process)
        {
            Process = process;
            
            if (process != null && process.Incident != null && iconImage != null)
            {
                iconImage.color = process.Incident.IconColor;
            }
        }

        /// <summary>
        /// ボタンがクリックされたときの処理
        /// </summary>
        private void OnButtonClicked()
        {
            if (Process == null)
            {
                throw new System.NullReferenceException("Process is null. SetIncidentProcess must be called before the icon can be clicked.");
            }
            OnIconClicked?.Invoke(Process);
        }
    }
}
