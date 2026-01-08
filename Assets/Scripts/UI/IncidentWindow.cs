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
        [SerializeField] private Button minimizeButton;
        [SerializeField] private Transform contentArea;

        /// <summary>
        /// このウィンドウが表示しているインシデントインスタンス
        /// </summary>
        public IncidentInstance Instance { get; private set; }

        /// <summary>
        /// 選択肢の情報（IncidentWindowBuilderから設定）
        /// </summary>
        private IncidentWindowOption[] options;

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

            if (minimizeButton != null)
            {
                minimizeButton.onClick.AddListener(MinimizeWindow);
            }
        }

        /// <summary>
        /// インシデントインスタンスを設定
        /// </summary>
        public void SetIncidentInstance(IncidentInstance instance)
        {
            Instance = instance;
            
            // 選択肢ボタンのコールバックを設定
            if (options != null && options.Length > 0)
            {
                SetupOptionButtons();
            }
        }

        /// <summary>
        /// 選択肢の情報を取得
        /// </summary>
        public IncidentWindowOption[] GetOptions()
        {
            return options;
        }

        /// <summary>
        /// 選択肢の情報を設定（IncidentWindowBuilderから使用）
        /// </summary>
        public void SetOptions(IncidentWindowOption[] windowOptions)
        {
            options = windowOptions;
            
            // すでにInstanceが設定されている場合は、ここでボタンをセットアップする
            if (Instance != null && options != null && options.Length > 0)
            {
                SetupOptionButtons();
            }
        }

        /// <summary>
        /// 選択肢ボタンのコールバックを設定
        /// </summary>
        private void SetupOptionButtons()
        {
            if (contentArea == null)
            {
                throw new System.NullReferenceException("contentArea is null. It must be set before calling SetupOptionButtons.");
            }
            if (options == null)
            {
                throw new System.NullReferenceException("options is null. SetOptions must be called before setting up option buttons.");
            }

            // OptionsContainerを探す
            Transform optionsContainer = contentArea.Find("OptionsContainer");
            if (optionsContainer == null)
            {
                throw new System.NullReferenceException("OptionsContainer not found in contentArea. Make sure the OptionsContainer exists in the content area.");
            }

            // 各選択肢ボタンにコールバックを設定
            for (int i = 0; i < options.Length && i < optionsContainer.childCount; i++)
            {
                Transform buttonTransform = optionsContainer.GetChild(i);
                Button button = buttonTransform.GetComponent<Button>();
                if (button != null && options[i] != null)
                {
                    // 既存のリスナーをクリア
                    button.onClick.RemoveAllListeners();
                    
                    // 新しいリスナーを設定
                    IncidentWindowOption option = options[i];
                    button.onClick.AddListener(() =>
                    {
                        if (Instance == null)
                        {
                            throw new System.NullReferenceException("Instance is null. SetIncidentInstance must be called before using the window.");
                        }
                        if (option.OnSelected == null)
                        {
                            throw new System.NullReferenceException($"OnSelected callback is null for option at index {i}.");
                        }
                        option.OnSelected(Instance);
                    });
                }
            }
        }

        /// <summary>
        /// 解決ボタンがクリックされたときの処理
        /// </summary>
        public void OnResolve()
        {
            if (Instance == null)
            {
                throw new System.NullReferenceException("Instance is null. SetIncidentInstance must be called before resolving.");
            }
            if (incidentManager == null)
            {
                throw new System.NullReferenceException("incidentManager is null. IncidentManager.Instance must be available.");
            }
            incidentManager.ResolveIncident(Instance);

            CloseWindow();
        }

        /// <summary>
        /// 放置ボタンがクリックされたときの処理
        /// </summary>
        public void OnDismiss()
        {
            if (Instance == null)
            {
                throw new System.NullReferenceException("Instance is null. SetIncidentInstance must be called before dismissing.");
            }
            if (incidentManager == null)
            {
                throw new System.NullReferenceException("incidentManager is null. IncidentManager.Instance must be available.");
            }
            incidentManager.DismissIncident(Instance);

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

        /// <summary>
        /// ウィンドウを折りたたむ（非表示にするが解決しない）
        /// </summary>
        public void MinimizeWindow()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// ウィンドウを表示する
        /// </summary>
        public void ShowWindow()
        {
            gameObject.SetActive(true);
        }
    }
}
