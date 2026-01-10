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
        private Dictionary<IncidentProcess, IncidentIcon> iconMap = new Dictionary<IncidentProcess, IncidentIcon>();
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
        private void OnIncidentOccurred(IncidentProcess process)
        {
            UpdateIcons();
        }

        /// <summary>
        /// インシデントが解決されたときの処理
        /// </summary>
        private void OnIncidentResolved(IncidentProcess process)
        {
            UpdateIcons();
        }

        /// <summary>
        /// インシデントが放置されたときの処理
        /// </summary>
        private void OnIncidentDismissed(IncidentProcess process)
        {
            // アイコンは残すので何もしない
        }

        /// <summary>
        /// インシデントが期限切れになったときの処理
        /// </summary>
        private void OnIncidentExpired(IncidentProcess process)
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

            var activeIncidents = incidentManager.ActiveIncidents;
            var currentProcessIds = new HashSet<string>(activeIncidents.Select(process => process.Incident.Id));

            // 削除されたインシデントのアイコンを削除
            var toRemove = iconMap.Where(kvp => !currentProcessIds.Contains(kvp.Key.Incident.Id)).ToList();
            foreach (var kvp in toRemove)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.Remove();
                }
                iconMap.Remove(kvp.Key);
            }

            // 新しいインシデントのアイコンを追加または更新
            foreach (var process in activeIncidents)
            {
                if (!iconMap.ContainsKey(process))
                {
                    CreateIcon(process);
                }
                else
                {
                    // 既存のアイコンの見た目を更新（Urgencyが変更された可能性があるため）
                    iconMap[process].UpdateAppearance();
                }
            }
        }

        /// <summary>
        /// アイコンを作成
        /// </summary>
        private void CreateIcon(IncidentProcess process)
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
                image.color = process.Incident != null ? process.Incident.IconColor : Color.white;

                Button button = iconObj.AddComponent<Button>();
            }

            IncidentIcon icon = iconObj.GetComponent<IncidentIcon>();
            if (icon == null)
            {
                icon = iconObj.AddComponent<IncidentIcon>();
            }

            icon.SetIncidentProcess(process);
            icon.OnIconClicked += OpenWindow;

            iconMap[process] = icon;
        }

        /// <summary>
        /// インシデントウィンドウを開く
        /// </summary>
        public void OpenWindow(IncidentProcess process)
        {
            if (process == null)
            {
                throw new System.ArgumentNullException(nameof(process), "process cannot be null.");
            }
            if (process.Incident == null)
            {
                throw new System.NullReferenceException("process.Incident is null. IncidentProcess must have a valid Incident.");
            }

            // 既にウィンドウが存在する場合
            if (process.WindowPrefabInstance != null)
            {
                IncidentWindow existingWindow = process.WindowPrefabInstance.GetComponent<IncidentWindow>();
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

            // ExplorationIncidentの場合はcontentベースで作成
            if (process.Incident is ExplorationIncident explorationIncident)
            {
                // 初期状態が設定されていない場合は設定
                if (process.CurrentState == null)
                {
                    IncidentState initialState = explorationIncident.GetInitialState(process);
                    if (initialState != null)
                    {
                        process.ApplyAction(new IncidentAction("initial"));
                        // 状態を直接設定（ApplyActionは既に呼ばれているが、念のため）
                        // 実際には、ApplyActionで状態が設定されるので、ここでは確認のみ
                    }
                }

                // コンテンツを作成
                IncidentContent content = explorationIncident.CreateContentFromState(process);
                if (content == null)
                {
                    Debug.LogWarning($"[IncidentUI] Failed to create content for incident: {process.Incident.Id}");
                    return;
                }

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
                windowComponent.SetProcess(process);

                currentWindowInstance = windowObj;
                process.WindowPrefabInstance = windowObj;
            }
            else
            {
                // 既存の方法（Prefabベース）を使用
                GameObject windowPrefab = process.Incident.GetWindowPrefab();
                if (windowPrefab == null)
                {
                    throw new System.NullReferenceException($"Window prefab not found for incident: {process.Incident.Id}. Make sure GetWindowPrefab() returns a valid prefab.");
                }

                // ウィンドウをインスタンス化
                currentWindowInstance = Instantiate(windowPrefab, canvas.transform);

                // IncidentWindowコンポーネントを取得
                IncidentWindow windowComponent = currentWindowInstance.GetComponent<IncidentWindow>();
                if (windowComponent != null)
                {
                    windowComponent.SetProcess(process);
                }

                process.WindowPrefabInstance = currentWindowInstance;
            }
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
