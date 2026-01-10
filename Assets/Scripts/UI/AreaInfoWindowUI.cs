using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// エリア情報ウィンドウUIを動的に生成・管理するクラス
    /// </summary>
    public static class AreaInfoWindowUI
    {
        private static GameObject currentWindow = null;
        private static SampleExplorationArea currentArea = null;

        /// <summary>
        /// フォントを取得（日本語対応）
        /// </summary>
        private static Font GetFont()
        {
            Font font = Resources.Load<Font>("Fonts/NotoSansJP-Medium");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return font;
        }

        /// <summary>
        /// エリア情報ウィンドウを作成
        /// </summary>
        /// <param name="area">表示するエリア</param>
        /// <param name="parentCanvas">親Canvas</param>
        /// <returns>作成されたウィンドウのGameObject</returns>
        public static GameObject CreateAreaInfoWindow(ExplorationArea area, Canvas parentCanvas)
        {
            if (area == null)
            {
                Debug.LogWarning("[AreaInfoWindowUI] areaがnullです。");
                return null;
            }

            if (parentCanvas == null)
            {
                Debug.LogWarning("[AreaInfoWindowUI] parentCanvasがnullです。");
                return null;
            }

            // 既存のウィンドウが開いている場合は閉じる
            if (currentWindow != null)
            {
                CloseWindow();
            }

            // 現在のエリアを保存（SampleExplorationAreaにキャスト）
            currentArea = area as SampleExplorationArea;
            if (currentArea != null)
            {
                // エリアを選択状態に設定
                currentArea.SetSelected(true);
            }

            // ウィンドウのルートオブジェクトを作成
            GameObject windowObj = new GameObject("AreaInfoWindow");
            windowObj.transform.SetParent(parentCanvas.transform, false);
            currentWindow = windowObj;

            RectTransform windowRect = windowObj.AddComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.sizeDelta = new Vector2(600, 400);
            windowRect.anchoredPosition = Vector2.zero;

            // 背景パネル
            Image background = windowObj.AddComponent<Image>();
            background.color = new Color(0.2f, 0.2f, 0.25f, 0.95f);

            // エリア名表示用のText
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(windowObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = area.AreaName;
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.offsetMin = new Vector2(10, 0);
            titleRect.offsetMax = new Vector2(-10, -10);

            // 説明文表示用のText
            GameObject descObj = new GameObject("DescriptionText");
            descObj.transform.SetParent(windowObj.transform, false);
            Text descText = descObj.AddComponent<Text>();
            descText.text = string.IsNullOrEmpty(area.Description) ? "（説明なし）" : area.Description;
            descText.font = GetFont();
            descText.fontSize = 16;
            descText.alignment = TextAnchor.UpperLeft;
            descText.color = new Color(0.9f, 0.9f, 0.9f, 1f);

            RectTransform descRect = descObj.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.5f);
            descRect.anchorMax = new Vector2(1, 0.85f);
            descRect.sizeDelta = Vector2.zero;
            descRect.offsetMin = new Vector2(20, 10);
            descRect.offsetMax = new Vector2(-20, -10);

            // 部員アイコン配置用のGridLayoutGroup
            GameObject memberGridObj = new GameObject("MemberGridArea");
            memberGridObj.transform.SetParent(windowObj.transform, false);
            
            RectTransform memberGridRect = memberGridObj.AddComponent<RectTransform>();
            memberGridRect.anchorMin = new Vector2(0, 0.1f);
            memberGridRect.anchorMax = new Vector2(1, 0.5f);
            memberGridRect.sizeDelta = Vector2.zero;
            memberGridRect.offsetMin = new Vector2(20, 10);
            memberGridRect.offsetMax = new Vector2(-20, -10);

            GridLayoutGroup gridLayout = memberGridObj.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(120, 80);
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;

            // 部員アイコンスロットを生成
            CreateMemberIconSlots(area, memberGridObj, parentCanvas);

            // 閉じるボタン
            GameObject closeButtonObj = new GameObject("CloseButton");
            closeButtonObj.transform.SetParent(windowObj.transform, false);
            
            RectTransform closeButtonRect = closeButtonObj.AddComponent<RectTransform>();
            closeButtonRect.anchorMin = new Vector2(0.5f, 0);
            closeButtonRect.anchorMax = new Vector2(0.5f, 0);
            closeButtonRect.sizeDelta = new Vector2(120, 40);
            closeButtonRect.anchoredPosition = new Vector2(0, 15);

            Image closeButtonImage = closeButtonObj.AddComponent<Image>();
            closeButtonImage.color = new Color(0.3f, 0.3f, 0.4f, 1f);

            Button closeButton = closeButtonObj.AddComponent<Button>();
            closeButton.onClick.AddListener(() => CloseWindow());

            // ボタンのテキスト
            GameObject closeButtonTextObj = new GameObject("Text");
            closeButtonTextObj.transform.SetParent(closeButtonObj.transform, false);
            Text closeButtonText = closeButtonTextObj.AddComponent<Text>();
            closeButtonText.text = "閉じる";
            closeButtonText.font = GetFont();
            closeButtonText.fontSize = 16;
            closeButtonText.alignment = TextAnchor.MiddleCenter;
            closeButtonText.color = Color.white;

            RectTransform closeButtonTextRect = closeButtonTextObj.GetComponent<RectTransform>();
            closeButtonTextRect.anchorMin = Vector2.zero;
            closeButtonTextRect.anchorMax = Vector2.one;
            closeButtonTextRect.sizeDelta = Vector2.zero;
            closeButtonTextRect.offsetMin = Vector2.zero;
            closeButtonTextRect.offsetMax = Vector2.zero;

            return windowObj;
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        public static void CloseWindow()
        {
            // 現在のエリアの選択状態を解除
            if (currentArea != null)
            {
                currentArea.SetSelected(false);
                currentArea = null;
            }

            if (currentWindow != null)
            {
                UnityEngine.Object.Destroy(currentWindow);
                currentWindow = null;
            }
        }

        /// <summary>
        /// 部員アイコンスロットを生成
        /// </summary>
        /// <param name="area">探索エリア</param>
        /// <param name="memberGridObj">部員アイコンを配置するGridオブジェクト</param>
        /// <param name="parentCanvas">親Canvas（ダイアログ表示用）</param>
        private static void CreateMemberIconSlots(ExplorationArea area, GameObject memberGridObj, Canvas parentCanvas)
        {
            if (area == null || memberGridObj == null)
            {
                return;
            }

            var assignedMembers = area.GetAssignedMembers();
            int maxCapacity = area.MaxMemberCapacity;
            int assignedCount = assignedMembers != null ? assignedMembers.Count : 0;

            // 最大容量までスロットを作成
            for (int i = 0; i < maxCapacity; i++)
            {
                GameObject iconObj;
                ClubMember member = null;

                if (i < assignedCount && assignedMembers != null)
                {
                    // 部員が割り当てられているスロット
                    member = assignedMembers[i];
                    iconObj = MemberIconGenerator.CreateMemberIcon(member, new Vector2(120, 80));
                }
                else
                {
                    // 空のスロット
                    iconObj = MemberIconGenerator.CreateEmptyMemberIcon(new Vector2(120, 80));
                }

                if (iconObj != null)
                {
                    iconObj.transform.SetParent(memberGridObj.transform, false);

                    // Buttonコンポーネントを追加
                    Button button = iconObj.AddComponent<Button>();
                    
                    // クリック時のハンドラーを設定
                    ClubMember capturedMember = member; // クロージャー用に変数をキャプチャ
                    button.onClick.AddListener(() => OnMemberIconClicked(capturedMember, area, parentCanvas));

                    // ホバー時の視覚的フィードバック用のColorBlockを設定
                    ColorBlock colors = button.colors;
                    if (member != null)
                    {
                        colors.normalColor = new Color(0.3f, 0.3f, 0.5f, 1f);
                        colors.highlightedColor = new Color(0.4f, 0.4f, 0.6f, 1f);
                        colors.pressedColor = new Color(0.5f, 0.5f, 0.7f, 1f);
                    }
                    else
                    {
                        colors.normalColor = new Color(0.2f, 0.2f, 0.3f, 0.7f);
                        colors.highlightedColor = new Color(0.3f, 0.3f, 0.4f, 0.9f);
                        colors.pressedColor = new Color(0.4f, 0.4f, 0.5f, 1f);
                    }
                    colors.selectedColor = colors.highlightedColor;
                    colors.disabledColor = new Color(0.2f, 0.2f, 0.3f, 0.5f);
                    button.colors = colors;
                }
            }
        }

        /// <summary>
        /// 部員アイコンがクリックされたときのハンドラー
        /// </summary>
        /// <param name="currentMember">現在のスロットに割り当てられている部員（nullの場合は空スロット）</param>
        /// <param name="area">探索エリア</param>
        /// <param name="parentCanvas">親Canvas</param>
        private static async void OnMemberIconClicked(ClubMember currentMember, ExplorationArea area, Canvas parentCanvas)
        {
            if (area == null || parentCanvas == null)
            {
                return;
            }

            // 部員選択ダイアログを表示
            ClubMember selectedMember = await ClubMemberSelectionDialog.ShowDialogAsync(parentCanvas);

            // 選択結果に応じて探索メンバーを更新
            if (currentMember != null)
            {
                // 存在する部員のアイコンをクリックした場合
                // 元の部員を外す
                area.UnassignMember(currentMember);

                // 新しい部員が選択された場合は追加
                if (selectedMember != null)
                {
                    area.AssignMember(selectedMember);
                }
            }
            else
            {
                // 空の部員のアイコンをクリックした場合
                // 部員が選択された場合は追加
                if (selectedMember != null)
                {
                    area.AssignMember(selectedMember);
                }
            }

            // ウィンドウを再描画
            RefreshMemberIcons(area);
        }

        /// <summary>
        /// 部員アイコン部分を再描画
        /// </summary>
        /// <param name="area">探索エリア</param>
        private static void RefreshMemberIcons(ExplorationArea area)
        {
            if (currentWindow == null || area == null)
            {
                return;
            }

            // MemberGridAreaを検索
            Transform memberGridTransform = currentWindow.transform.Find("MemberGridArea");
            if (memberGridTransform == null)
            {
                return;
            }

            GameObject memberGridObj = memberGridTransform.gameObject;

            // 既存のアイコンを削除
            for (int i = memberGridObj.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(memberGridObj.transform.GetChild(i).gameObject);
            }

            // Canvasを取得
            Canvas parentCanvas = currentWindow.GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                return;
            }

            // アイコンスロットを再生成
            CreateMemberIconSlots(area, memberGridObj, parentCanvas);
        }
    }
}
