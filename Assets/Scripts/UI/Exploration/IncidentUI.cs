using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントアイコン表示エリアを制御するUIコンポーネント
    /// 画面右下にアクティブなインシデントのアイコンを表示
    /// </summary>
    public class IncidentUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform iconContainer;
        [SerializeField] private GameObject iconPrefab;

        private IncidentManager incidentManager;
        private Dictionary<IncidentState, IncidentIcon> iconMap = new Dictionary<IncidentState, IncidentIcon>();
        private GameObject currentWindowInstance;

        private void Awake()
        {
            incidentManager = IncidentManager.Instance;
            
            if (incidentManager != null)
            {
                incidentManager.OnIncidentOccurred += OnIncidentOccurred;
                incidentManager.OnIncidentResolved += OnIncidentResolved;
                incidentManager.OnIncidentDismissed += OnIncidentDismissed;
                incidentManager.OnIncidentExpired += OnIncidentExpired;
            }

            // 初期状態のアイコンを更新
            UpdateIcons();
        }

        private void OnDestroy()
        {
            if (incidentManager != null)
            {
                incidentManager.OnIncidentOccurred -= OnIncidentOccurred;
                incidentManager.OnIncidentResolved -= OnIncidentResolved;
                incidentManager.OnIncidentDismissed -= OnIncidentDismissed;
                incidentManager.OnIncidentExpired -= OnIncidentExpired;
            }
        }

        /// <summary>
        /// インシデントが発生したときの処理
        /// </summary>
        private void OnIncidentOccurred(IncidentState state)
        {
            UpdateIcons();
        }

        /// <summary>
        /// インシデントが解決されたときの処理
        /// </summary>
        private void OnIncidentResolved(IncidentState state)
        {
            UpdateIcons();
        }

        /// <summary>
        /// インシデントが放置されたときの処理
        /// </summary>
        private void OnIncidentDismissed(IncidentState state)
        {
            // アイコンは残すので何もしない
        }

        /// <summary>
        /// インシデントが期限切れになったときの処理
        /// </summary>
        private void OnIncidentExpired(IncidentState state)
        {
            UpdateIcons();
        }

        /// <summary>
        /// アイコンを更新
        /// </summary>
        public void UpdateIcons()
        {
            if (incidentManager == null)
            {
                throw new System.NullReferenceException("incidentManager is null. IncidentManager.Instance must be available.");
            }
            if (iconContainer == null)
            {
                throw new System.NullReferenceException("iconContainer is null. It must be assigned in the inspector.");
            }

            var activeStates = incidentManager.ActiveStates;
            var currentStateIds = new HashSet<string>(activeStates.Select(state => state.GetStateId()));

            // 削除されたインシデントのアイコンを削除
            var toRemove = iconMap.Where(kvp => !currentStateIds.Contains(kvp.Key.GetStateId())).ToList();
            foreach (var kvp in toRemove)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Remove();
                }
                iconMap.Remove(kvp.Key);
            }

            // 新しいインシデントのアイコンを追加または更新
            foreach (var state in activeStates)
            {
                if (!iconMap.ContainsKey(state))
                {
                    CreateIcon(state);
                }
                else
                {
                    // 既存のアイコンの見た目を更新（Urgencyが変更された可能性があるため）
                    iconMap[state].UpdateAppearance();
                }
            }
        }

        /// <summary>
        /// アイコンを作成
        /// </summary>
        private void CreateIcon(IncidentState state)
        {
            if (iconContainer == null)
            {
                throw new System.NullReferenceException("iconContainer is null. It must be assigned in the inspector.");
            }

            GameObject iconObj;
            if (iconPrefab != null)
            {
                iconObj = Instantiate(iconPrefab, iconContainer);
            }
            else
            {
                // Prefabが設定されていない場合は動的に作成
                iconObj = new GameObject("IncidentIcon");
                iconObj.transform.SetParent(iconContainer, false);

                RectTransform rectTransform = iconObj.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(50, 50);

                Image image = iconObj.AddComponent<Image>();
                Incident incident = incidentManager.GetIncidentForState(state);
                image.color = incident != null ? incident.IconColor : Color.white;

                Button button = iconObj.AddComponent<Button>();
            }

            IncidentIcon icon = iconObj.GetComponent<IncidentIcon>();
            if (icon == null)
            {
                icon = iconObj.AddComponent<IncidentIcon>();
            }

            icon.SetIncidentState(state);
            icon.OnIconClicked += OpenWindow;

            iconMap[state] = icon;
            
            // IncidentManagerにアイコンを登録
            if (incidentManager != null)
            {
                incidentManager.SetIconForState(state, icon);
            }
        }

        /// <summary>
        /// インシデントウィンドウを開く
        /// </summary>
        public void OpenWindow(IncidentState state)
        {
            if (state == null)
            {
                throw new System.ArgumentNullException(nameof(state), "state cannot be null.");
            }

            var incidentManager = IncidentManager.Instance;
            if (incidentManager == null)
            {
                throw new System.NullReferenceException("IncidentManager.Instance is null.");
            }

            // 既にウィンドウが存在する場合
            GameObject existingWindowObj = incidentManager.GetWindowForState(state);
            if (existingWindowObj != null)
            {
                IncidentWindow existingWindow = existingWindowObj.GetComponent<IncidentWindow>();
                if (existingWindow != null)
                {
                    // ウィンドウが非表示の場合は表示する
                    if (!existingWindow.gameObject.activeSelf)
                    {
                        existingWindow.ShowWindow();
                    }
                    // 既に表示されている場合は何もしない（または前面に持ってくる）
                    return;
                }
            }

            // 既存のウィンドウを閉じる
            CloseCurrentWindow();

            // Canvasを探す（MenuCanvasを優先）
            Canvas canvas = null;
            Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var c in allCanvases)
            {
                if (c.name == "MenuCanvas")
                {
                    canvas = c;
                    break;
                }
            }

            if (canvas == null)
            {
                throw new System.Exception("MenuCanvas not found. Please create MenuCanvas first.");
            }

            // コンテンツを作成
            IncidentContent content = state.CreateContent();
            if (content == null)
            {
                Debug.LogWarning($"[IncidentUI] Failed to create content for state: {state.GetStateId()}");
                return;
            }

            content.State = state;

            // ウィンドウを作成
            GameObject windowObj = new GameObject("IncidentWindow");
            windowObj.transform.SetParent(canvas.transform, false);
            RectTransform windowRect = windowObj.AddComponent<RectTransform>();
            windowRect.sizeDelta = new Vector2(400, 300);
            windowRect.anchoredPosition = Vector2.zero;

            IncidentWindow windowComponent = windowObj.AddComponent<IncidentWindow>();
            
            // 背景パネルを作成
            GameObject panelObj = new GameObject("Panel");
            panelObj.transform.SetParent(windowObj.transform, false);
            RectTransform panelRect = panelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;

            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

            // コンテンツエリアを設定
            windowComponent.SetContentArea(panelObj.transform);
            windowComponent.SetContent(content);
            windowComponent.SetState(state);

            currentWindowInstance = windowObj;
            incidentManager.SetWindowForState(state, windowObj);
        }

        /// <summary>
        /// 現在開いているウィンドウを閉じる
        /// </summary>
        private void CloseCurrentWindow()
        {
            if (currentWindowInstance != null)
            {
                Destroy(currentWindowInstance);
                currentWindowInstance = null;
            }
        }
    }
}
