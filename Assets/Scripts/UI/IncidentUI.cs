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
        private Dictionary<IncidentInstance, IncidentIconUI> iconMap = new Dictionary<IncidentInstance, IncidentIconUI>();
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
        private void OnIncidentOccurred(IncidentInstance instance)
        {
            UpdateIcons();
        }

        /// <summary>
        /// インシデントが解決されたときの処理
        /// </summary>
        private void OnIncidentResolved(IncidentInstance instance)
        {
            UpdateIcons();
        }

        /// <summary>
        /// インシデントが放置されたときの処理
        /// </summary>
        private void OnIncidentDismissed(IncidentInstance instance)
        {
            // アイコンは残すので何もしない
        }

        /// <summary>
        /// インシデントが期限切れになったときの処理
        /// </summary>
        private void OnIncidentExpired(IncidentInstance instance)
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
            var currentInstanceIds = new HashSet<string>(activeIncidents.Select(inst => inst.Incident.Id));

            // 削除されたインシデントのアイコンを削除
            var toRemove = iconMap.Where(kvp => !currentInstanceIds.Contains(kvp.Key.Incident.Id)).ToList();
            foreach (var kvp in toRemove)
            {
                if (kvp.Value != null && kvp.Value.gameObject != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
                iconMap.Remove(kvp.Key);
            }

            // 新しいインシデントのアイコンを追加
            foreach (var instance in activeIncidents)
            {
                if (!iconMap.ContainsKey(instance))
                {
                    CreateIcon(instance);
                }
            }
        }

        /// <summary>
        /// アイコンを作成
        /// </summary>
        private void CreateIcon(IncidentInstance instance)
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
                image.color = instance.Incident.IconColor;

                Button button = iconObj.AddComponent<Button>();
            }

            IncidentIconUI iconUI = iconObj.GetComponent<IncidentIconUI>();
            if (iconUI == null)
            {
                iconUI = iconObj.AddComponent<IncidentIconUI>();
            }

            iconUI.SetIncidentInstance(instance);
            iconUI.OnIconClicked += OpenWindow;

            iconMap[instance] = iconUI;
        }

        /// <summary>
        /// インシデントウィンドウを開く
        /// </summary>
        public void OpenWindow(IncidentInstance instance)
        {
            if (instance == null)
            {
                throw new System.ArgumentNullException(nameof(instance), "instance cannot be null.");
            }
            if (instance.Incident == null)
            {
                throw new System.NullReferenceException("instance.Incident is null. IncidentInstance must have a valid Incident.");
            }

            // 既にウィンドウが存在する場合
            if (instance.WindowPrefabInstance != null)
            {
                IncidentWindow existingWindow = instance.WindowPrefabInstance.GetComponent<IncidentWindow>();
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

            // Prefabを取得
            GameObject windowPrefab = instance.Incident.GetWindowPrefab();
            if (windowPrefab == null)
            {
                throw new System.NullReferenceException($"Window prefab not found for incident: {instance.Incident.Id}. Make sure GetWindowPrefab() returns a valid prefab.");
            }

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

            // ウィンドウをインスタンス化
            currentWindowInstance = Instantiate(windowPrefab, canvas.transform);

            // IncidentWindowコンポーネントを取得してインスタンスを設定
            IncidentWindow windowComponent = currentWindowInstance.GetComponent<IncidentWindow>();
            if (windowComponent == null)
            {
                throw new System.NullReferenceException($"IncidentWindow component not found in prefab: {instance.Incident.Id}. The window prefab must have an IncidentWindow component.");
            }
            windowComponent.SetIncidentInstance(instance);

            instance.WindowPrefabInstance = currentWindowInstance;
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
