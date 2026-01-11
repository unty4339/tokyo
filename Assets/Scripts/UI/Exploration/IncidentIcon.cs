using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 画面に置くインシデントの状況を示すアイコンの管理を行う
    /// 現在のインシデント状態が即時の解決が必要か後回しにできるかで見た目が変わる
    /// インシデントが解決されると削除される
    /// </summary>
    public class IncidentIcon : MonoBehaviour
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

        /// <summary>
        /// 即時解決が必要な場合の色
        /// </summary>
        private Color immediateColor = new Color(1f, 0.2f, 0.2f, 1f); // 赤

        /// <summary>
        /// 後回しにできる場合の色
        /// </summary>
        private Color deferrableColor = new Color(0.5f, 0.5f, 1f, 1f); // 青

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
        /// <param name="state">IncidentState</param>
        public void SetIncidentState(IncidentState state)
        {
            State = state;

            if (state != null)
            {
                UpdateAppearance();
            }
        }

        /// <summary>
        /// 見た目を更新（Urgencyに応じて色を変更）
        /// </summary>
        public void UpdateAppearance()
        {
            if (State == null || iconImage == null)
            {
                return;
            }

            // 現在の状態のUrgencyを確認
            IncidentUrgency urgency = State.Urgency;

            // Urgencyに応じて色を変更
            if (urgency == IncidentUrgency.Immediate)
            {
                iconImage.color = immediateColor;
            }
            else
            {
                iconImage.color = deferrableColor;
            }

            // Incident定義のIconColorも考慮する場合
            var incidentManager = IncidentManager.Instance;
            if (incidentManager != null)
            {
                Incident incident = incidentManager.GetIncidentForState(State);
                if (incident != null)
                {
                    // 基色をIncidentのIconColorに設定し、Urgencyに応じて調整
                    Color baseColor = incident.IconColor;
                    if (urgency == IncidentUrgency.Immediate)
                    {
                        // 即時解決が必要な場合は少し赤みを追加
                        iconImage.color = new Color(
                            Mathf.Min(1f, baseColor.r + 0.3f),
                            baseColor.g * 0.7f,
                            baseColor.b * 0.7f,
                            baseColor.a
                        );
                    }
                    else
                    {
                        iconImage.color = baseColor;
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
                Debug.LogWarning("[IncidentIcon] State is null");
                return;
            }

            OnIconClicked?.Invoke(State);
        }

        /// <summary>
        /// アイコンを削除（インシデント解決時に呼ばれる）
        /// </summary>
        public void Remove()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
