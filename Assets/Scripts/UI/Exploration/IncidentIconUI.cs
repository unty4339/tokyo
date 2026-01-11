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
        /// このアイコンが表示しているIncidentState
        /// </summary>
        public IncidentState State { get; private set; }

        /// <summary>
        /// クリック時のコールバック
        /// </summary>
        public System.Action<IncidentState> OnIconClicked;

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
        /// IncidentStateを設定
        /// </summary>
        public void SetIncidentState(IncidentState state)
        {
            State = state;
            
            if (state != null && iconImage != null)
            {
                var incidentManager = IncidentManager.Instance;
                if (incidentManager != null)
                {
                    Incident incident = incidentManager.GetIncidentForState(state);
                    if (incident != null)
                    {
                        iconImage.color = incident.IconColor;
                    }
                }
            }
        }

        /// <summary>
        /// ボタンがクリックされたときの処理
        /// </summary>
        private void OnButtonClicked()
        {
            if (State == null)
            {
                throw new System.NullReferenceException("State is null. SetIncidentState must be called before the icon can be clicked.");
            }
            OnIconClicked?.Invoke(State);
        }
    }
}
