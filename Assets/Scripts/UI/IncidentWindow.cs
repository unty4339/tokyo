using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントウィンドウの基本機能を提供するコンポーネント
    /// Prefabのルートに配置され、解決/放置処理を担当
    /// </summary>
    public class IncidentWindow : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button resolveButton;
        [SerializeField] private Button dismissButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform contentArea;

        /// <summary>
        /// このウィンドウが表示しているインシデントインスタンス
        /// </summary>
        public IncidentInstance Instance { get; private set; }

        private IncidentManager incidentManager;

        private void Awake()
        {
            incidentManager = IncidentManager.Instance;

            if (resolveButton != null)
            {
                resolveButton.onClick.AddListener(OnResolve);
            }

            if (dismissButton != null)
            {
                dismissButton.onClick.AddListener(OnDismiss);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnDismiss); // 閉じるボタンは放置として扱う
            }
        }

        /// <summary>
        /// インシデントインスタンスを設定
        /// </summary>
        public void SetIncidentInstance(IncidentInstance instance)
        {
            Instance = instance;
        }

        /// <summary>
        /// 解決ボタンがクリックされたときの処理
        /// </summary>
        public void OnResolve()
        {
            if (Instance != null && incidentManager != null)
            {
                incidentManager.ResolveIncident(Instance);
            }

            CloseWindow();
        }

        /// <summary>
        /// 放置ボタンがクリックされたときの処理
        /// </summary>
        public void OnDismiss()
        {
            if (Instance != null && incidentManager != null)
            {
                incidentManager.DismissIncident(Instance);
            }

            CloseWindow();
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        private void CloseWindow()
        {
            if (Instance != null)
            {
                Instance.WindowPrefabInstance = null;
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// コンテンツエリアを取得（派生クラスや外部から使用可能）
        /// </summary>
        public Transform GetContentArea()
        {
            return contentArea;
        }

        /// <summary>
        /// コンテンツエリアを設定（IncidentWindowBuilderから使用）
        /// </summary>
        public void SetContentArea(Transform area)
        {
            contentArea = area;
        }
    }
}
