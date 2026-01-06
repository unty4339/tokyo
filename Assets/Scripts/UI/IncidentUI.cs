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
            if (incidentManager == null || iconContainer == null)
            {
                return;
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
                return;
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
            if (instance == null || instance.Incident == null)
            {
                return;
            }

            // 既存のウィンドウを閉じる
            CloseCurrentWindow();

            // Prefabを取得
            GameObject windowPrefab = instance.Incident.GetWindowPrefab();
            if (windowPrefab == null)
            {
                Debug.LogWarning($"Window prefab not found for incident: {instance.Incident.Id}");
                return;
            }

            // Canvasを探す（既存のCanvasまたは新規作成）
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("IncidentCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10; // 他のUIより前面に表示
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // ウィンドウをインスタンス化
            currentWindowInstance = Instantiate(windowPrefab, canvas.transform);

            // IncidentWindowコンポーネントを取得してインスタンスを設定
            IncidentWindow windowComponent = currentWindowInstance.GetComponent<IncidentWindow>();
            if (windowComponent != null)
            {
                windowComponent.SetIncidentInstance(instance);
            }
            else
            {
                Debug.LogWarning($"IncidentWindow component not found in prefab: {instance.Incident.Id}");
            }

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
