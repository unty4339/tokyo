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
        /// このアイコンが表示しているインシデントインスタンス
        /// </summary>
        public IncidentInstance Instance { get; private set; }

        /// <summary>
        /// クリック時のコールバック
        /// </summary>
        public System.Action<IncidentInstance> OnIconClicked;

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
        /// インシデントインスタンスを設定
        /// </summary>
        public void SetIncidentInstance(IncidentInstance instance)
        {
            Instance = instance;
            
            if (instance != null && instance.Incident != null && iconImage != null)
            {
                iconImage.color = instance.Incident.IconColor;
            }
        }

        /// <summary>
        /// ボタンがクリックされたときの処理
        /// </summary>
        private void OnButtonClicked()
        {
            if (Instance != null)
            {
                OnIconClicked?.Invoke(Instance);
            }
        }
    }
}
