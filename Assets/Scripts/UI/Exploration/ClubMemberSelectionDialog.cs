using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 部員選択ダイアログボックス
    /// staticメソッドで作成・表示し、async/awaitで選択結果を待ち受ける
    /// </summary>
    public static class ClubMemberSelectionDialog
    {
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
        /// 部員選択ダイアログを表示（条件なし）
        /// </summary>
        /// <param name="dialogCanvas">ダイアログを表示するCanvas</param>
        /// <returns>選択された部員、または閉じた場合はnull</returns>
        public static Task<ClubMember> ShowDialogAsync(Canvas dialogCanvas)
        {
            return ShowDialogAsync(dialogCanvas, null);
        }

        /// <summary>
        /// 部員選択ダイアログを表示（条件付き）
        /// </summary>
        /// <param name="dialogCanvas">ダイアログを表示するCanvas</param>
        /// <param name="filter">部員のフィルタリング条件（nullの場合は全員表示）</param>
        /// <returns>選択された部員、または閉じた場合はnull</returns>
        public static async Task<ClubMember> ShowDialogAsync(Canvas dialogCanvas, Func<ClubMember, bool> filter)
        {
            if (dialogCanvas == null)
            {
                Debug.LogWarning("[ClubMemberSelectionDialog] dialogCanvasがnullです。");
                return null;
            }

            // 非同期処理の制御用
            TaskCompletionSource<ClubMember> tcs = new TaskCompletionSource<ClubMember>();

            // ダイアログウィンドウを作成
            GameObject windowObj = CreateDialogWindow(dialogCanvas, filter, tcs);

            // 選択または閉じるまで待機
            return await tcs.Task;
        }

        /// <summary>
        /// ダイアログウィンドウを作成
        /// </summary>
        private static GameObject CreateDialogWindow(Canvas dialogCanvas, Func<ClubMember, bool> filter, TaskCompletionSource<ClubMember> tcs)
        {
            // ウィンドウのルートオブジェクトを作成
            GameObject windowObj = new GameObject("ClubMemberSelectionDialog");
            windowObj.transform.SetParent(dialogCanvas.transform, false);

            RectTransform windowRect = windowObj.AddComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.sizeDelta = new Vector2(800, 600);
            windowRect.anchoredPosition = new Vector2(0, 50);

            // 背景パネル
            Image background = windowObj.AddComponent<Image>();
            background.color = new Color(0.2f, 0.2f, 0.25f, 0.95f);

            // タイトルテキスト
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(windowObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "部員を選択";
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.9f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.offsetMin = new Vector2(10, 0);
            titleRect.offsetMax = new Vector2(-10, -10);

            // 部員アイコン配置用のGridLayoutGroup
            GameObject memberGridObj = new GameObject("MemberGridArea");
            memberGridObj.transform.SetParent(windowObj.transform, false);

            RectTransform memberGridRect = memberGridObj.AddComponent<RectTransform>();
            memberGridRect.anchorMin = new Vector2(0, 0.1f);
            memberGridRect.anchorMax = new Vector2(1, 0.85f);
            memberGridRect.sizeDelta = Vector2.zero;
            memberGridRect.offsetMin = new Vector2(20, 60);
            memberGridRect.offsetMax = new Vector2(-20, -10);

            GridLayoutGroup gridLayout = memberGridObj.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(150, 100);
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;

            // 部員リストを取得してフィルタリング
            var manager = ClubMemberManager.Instance;
            var allMembers = manager.Members;
            List<ClubMember> filteredMembers = new List<ClubMember>();

            if (filter != null)
            {
                foreach (var member in allMembers)
                {
                    if (member != null && filter(member))
                    {
                        filteredMembers.Add(member);
                    }
                }
            }
            else
            {
                filteredMembers.AddRange(allMembers);
            }

            // 部員アイコンを生成
            if (filteredMembers.Count > 0)
            {
                foreach (var member in filteredMembers)
                {
                    CreateMemberIconButton(member, memberGridObj, windowObj, tcs);
                }
            }
            else
            {
                // 部員がいない場合の表示
                GameObject emptyTextObj = new GameObject("EmptyText");
                emptyTextObj.transform.SetParent(memberGridObj.transform, false);
                Text emptyText = emptyTextObj.AddComponent<Text>();
                emptyText.text = "表示できる部員がいません";
                emptyText.font = GetFont();
                emptyText.fontSize = 16;
                emptyText.alignment = TextAnchor.MiddleCenter;
                emptyText.color = new Color(0.7f, 0.7f, 0.7f, 1f);

                RectTransform emptyRect = emptyTextObj.GetComponent<RectTransform>();
                emptyRect.sizeDelta = new Vector2(300, 40);
            }

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
            closeButton.onClick.AddListener(() => CloseDialog(windowObj, tcs, null));

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
        /// 部員アイコンにButtonを追加して作成
        /// </summary>
        private static void CreateMemberIconButton(ClubMember member, GameObject parent, GameObject windowObj, TaskCompletionSource<ClubMember> tcs)
        {
            // MemberIconGeneratorでアイコンを作成
            GameObject iconObj = MemberIconGenerator.CreateMemberIcon(member, new Vector2(150, 100));
            if (iconObj == null)
            {
                return;
            }

            iconObj.transform.SetParent(parent.transform, false);

            // Buttonコンポーネントを追加
            Button button = iconObj.AddComponent<Button>();
            button.onClick.AddListener(() => CloseDialog(windowObj, tcs, member));

            // ホバー時の視覚的フィードバック用のColorBlockを設定
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.3f, 0.3f, 0.5f, 1f);
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.6f, 1f);
            colors.pressedColor = new Color(0.5f, 0.5f, 0.7f, 1f);
            colors.selectedColor = new Color(0.4f, 0.4f, 0.6f, 1f);
            colors.disabledColor = new Color(0.2f, 0.2f, 0.3f, 0.5f);
            button.colors = colors;
        }

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        private static void CloseDialog(GameObject windowObj, TaskCompletionSource<ClubMember> tcs, ClubMember selectedMember)
        {
            // 結果を設定
            if (tcs != null && !tcs.Task.IsCompleted)
            {
                tcs.SetResult(selectedMember);
            }

            // ウィンドウを破棄
            if (windowObj != null)
            {
                UnityEngine.Object.Destroy(windowObj);
            }
        }
    }
}
