using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントウィンドウの基本機能を提供するコンポーネント
    /// ウィンドウの開閉やサイズの変更、ウィンドウオブジェクトの削除の責任を持つ
    /// ひとつのIncidentContentを受け取って作成される
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
        /// このウィンドウが表示しているIncidentContent
        /// </summary>
        public IncidentContent Content { get; private set; }

        /// <summary>
        /// このウィンドウが表示しているIncidentProcess
        /// </summary>
        public IncidentProcess Process { get; private set; }

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
        /// IncidentContentを設定してウィンドウを初期化
        /// </summary>
        /// <param name="content">IncidentContent</param>
        public void SetContent(IncidentContent content)
        {
            if (content == null)
            {
                Debug.LogError("[IncidentWindow] content is null");
                return;
            }

            Content = content;
            Process = content.Process;

            // コンテンツエリアをクリア
            if (contentArea != null)
            {
                foreach (Transform child in contentArea)
                {
                    Destroy(child.gameObject);
                }

                // IncidentContentのCreateChildObjectsメソッドを呼び出して子オブジェクトを作成
                content.CreateChildObjects(contentArea);
            }
        }

        /// <summary>
        /// IncidentProcessを設定（後方互換性のため）
        /// </summary>
        /// <param name="process">IncidentProcess</param>
        public void SetProcess(IncidentProcess process)
        {
            Process = process;
        }


        /// <summary>
        /// 解決ボタンがクリックされたときの処理
        /// </summary>
        public void OnResolve()
        {
            if (Process == null)
            {
                Debug.LogWarning("[IncidentWindow] Process is null. Cannot resolve incident.");
                return;
            }
            if (incidentManager == null)
            {
                Debug.LogWarning("[IncidentWindow] incidentManager is null. IncidentManager.Instance must be available.");
                return;
            }
            incidentManager.ResolveIncident(Process);

            CloseWindow();
        }

        /// <summary>
        /// 放置ボタンがクリックされたときの処理
        /// </summary>
        public void OnDismiss()
        {
            if (Process == null)
            {
                Debug.LogWarning("[IncidentWindow] Process is null. Cannot dismiss incident.");
                return;
            }
            if (incidentManager == null)
            {
                Debug.LogWarning("[IncidentWindow] incidentManager is null. IncidentManager.Instance must be available.");
                return;
            }
            incidentManager.DismissIncident(Process);

            CloseWindow();
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        public void CloseWindow()
        {
            if (Process != null)
            {
                Process.WindowPrefabInstance = null;
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
        /// コンテンツエリアを設定
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

        /// <summary>
        /// コンテンツを更新（新しいコンテンツで再作成）
        /// </summary>
        /// <param name="content">更新するコンテンツ</param>
        public void UpdateContent(IncidentContent content)
        {
            SetContent(content);
        }

        /// <summary>
        /// ウィンドウのサイズを変更
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        public void ResizeWindow(float width, float height)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(width, height);
            }
        }

        /// <summary>
        /// ウィンドウの位置を変更
        /// </summary>
        /// <param name="position">位置</param>
        public void SetPosition(Vector2 position)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position;
            }
        }
    }
}
