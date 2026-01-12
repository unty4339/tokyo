using System;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// IncidentOptionalContent専用のインシデントウィンドウを動的に作成する静的クラス
    /// </summary>
    public static class IncidentOptionalContentWindowBuilder
    {
        /// <summary>
        /// インシデントウィンドウを動的に作成（内部メソッド）
        /// </summary>
        /// <param name="title">インシデント名（タイトル）</param>
        /// <param name="imagePath">画像のパス（オプショナル、null可）</param>
        /// <param name="messageText">表示するテキスト</param>
        /// <param name="options">選択肢の配列</param>
        /// <param name="content">IncidentContent（アクション処理用）</param>
        /// <returns>IncidentWindowコンポーネント付きGameObject</returns>
        private static GameObject CreateWindow(string title, string imagePath, string messageText, IncidentContentOption[] options, IncidentContent content)
        {
            // ルートオブジェクトを作成
            GameObject windowRoot = new GameObject("IncidentWindow");
            RectTransform rootRect = windowRoot.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(400, 300);
            rootRect.anchoredPosition = Vector2.zero;

            // IncidentWindowコンポーネントをアタッチ
            IncidentWindow windowComponent = windowRoot.AddComponent<IncidentWindow>();

            // 背景パネルを作成
            GameObject panelObj = new GameObject("Panel");
            panelObj.transform.SetParent(windowRoot.transform, false);
            RectTransform panelRect = panelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;

            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

            // タイトルテキストを作成
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panelObj.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.anchoredPosition = Vector2.zero;

            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = title;
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            // 折りたたみボタンを作成（右上）
            GameObject minimizeButtonObj = new GameObject("MinimizeButton");
            minimizeButtonObj.transform.SetParent(panelObj.transform, false);
            RectTransform minimizeButtonRect = minimizeButtonObj.AddComponent<RectTransform>();
            minimizeButtonRect.anchorMin = new Vector2(1, 1);
            minimizeButtonRect.anchorMax = new Vector2(1, 1);
            minimizeButtonRect.pivot = new Vector2(1, 1);
            minimizeButtonRect.sizeDelta = new Vector2(30, 30);
            minimizeButtonRect.anchoredPosition = new Vector2(-10, -10);

            Image minimizeButtonImage = minimizeButtonObj.AddComponent<Image>();
            minimizeButtonImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);

            Button minimizeButton = minimizeButtonObj.AddComponent<Button>();
            minimizeButton.targetGraphic = minimizeButtonImage;

            GameObject minimizeButtonTextObj = new GameObject("Text");
            minimizeButtonTextObj.transform.SetParent(minimizeButtonObj.transform, false);
            RectTransform minimizeButtonTextRect = minimizeButtonTextObj.AddComponent<RectTransform>();
            minimizeButtonTextRect.anchorMin = Vector2.zero;
            minimizeButtonTextRect.anchorMax = Vector2.one;
            minimizeButtonTextRect.sizeDelta = Vector2.zero;
            minimizeButtonTextRect.anchoredPosition = Vector2.zero;

            Text minimizeButtonText = minimizeButtonTextObj.AddComponent<Text>();
            minimizeButtonText.text = "[-]";
            minimizeButtonText.font = GetFont();
            minimizeButtonText.fontSize = 16;
            minimizeButtonText.alignment = TextAnchor.MiddleCenter;
            minimizeButtonText.color = Color.white;

            // 折りたたみボタンのクリックイベントを設定
            minimizeButton.onClick.AddListener(() =>
            {
                if (windowComponent != null)
                {
                    windowComponent.MinimizeWindow();
                }
            });

            // リフレクションでminimizeButtonフィールドを設定
            var incidentWindowType = typeof(IncidentWindow);
            var minimizeButtonField = incidentWindowType.GetField("minimizeButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            minimizeButtonField?.SetValue(windowComponent, minimizeButton);

            // 画像（オプショナル）
            if (!string.IsNullOrEmpty(imagePath))
            {
                GameObject imageObj = new GameObject("Image");
                imageObj.transform.SetParent(panelObj.transform, false);
                RectTransform imageRect = imageObj.AddComponent<RectTransform>();
                imageRect.anchorMin = new Vector2(0.1f, 0.4f);
                imageRect.anchorMax = new Vector2(0.4f, 0.7f);
                imageRect.sizeDelta = Vector2.zero;
                imageRect.anchoredPosition = Vector2.zero;

                Image image = imageObj.AddComponent<Image>();
                // 画像の読み込みは後で実装可能（Resources.Loadなど）
            }

            // メッセージテキストを作成
            GameObject messageObj = new GameObject("Message");
            messageObj.transform.SetParent(panelObj.transform, false);
            RectTransform messageRect = messageObj.AddComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.1f, 0.3f);
            messageRect.anchorMax = new Vector2(0.9f, 0.7f);
            messageRect.sizeDelta = Vector2.zero;
            messageRect.anchoredPosition = Vector2.zero;

            Text messageTextComponent = messageObj.AddComponent<Text>();
            messageTextComponent.text = messageText;
            messageTextComponent.font = GetFont();
            messageTextComponent.fontSize = 18;
            messageTextComponent.alignment = TextAnchor.MiddleCenter;
            messageTextComponent.color = Color.white;


            // 選択肢ボタンを作成
            if (options != null && options.Length > 0)
            {
                GameObject optionsContainer = new GameObject("OptionsContainer");
                optionsContainer.transform.SetParent(panelObj.transform, false);
                RectTransform optionsRect = optionsContainer.AddComponent<RectTransform>();
                optionsRect.anchorMin = new Vector2(0, 0);
                optionsRect.anchorMax = new Vector2(1, 0.25f);
                optionsRect.sizeDelta = Vector2.zero;
                optionsRect.anchoredPosition = Vector2.zero;

                // HorizontalLayoutGroupを追加してボタンを横に並べる
                HorizontalLayoutGroup layoutGroup = optionsContainer.AddComponent<HorizontalLayoutGroup>();
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                layoutGroup.spacing = 10;
                layoutGroup.padding = new RectOffset(20, 20, 10, 10);
                layoutGroup.childControlWidth = false;
                layoutGroup.childControlHeight = false;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;

                for (int i = 0; i < options.Length; i++)
                {
                    IncidentContentOption option = options[i];
                    if (option == null)
                    {
                        continue;
                    }

                    GameObject buttonObj = new GameObject($"OptionButton_{i}");
                    buttonObj.transform.SetParent(optionsContainer.transform, false);
                    RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
                    buttonRect.sizeDelta = new Vector2(200, 40);

                    Image buttonImage = buttonObj.AddComponent<Image>();
                    buttonImage.color = new Color(0.3f, 0.5f, 0.8f, 1f);

                    Button button = buttonObj.AddComponent<Button>();
                    button.targetGraphic = buttonImage;

                    GameObject buttonTextObj = new GameObject("Text");
                    buttonTextObj.transform.SetParent(buttonObj.transform, false);
                    RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
                    buttonTextRect.anchorMin = Vector2.zero;
                    buttonTextRect.anchorMax = Vector2.one;
                    buttonTextRect.sizeDelta = Vector2.zero;
                    buttonTextRect.anchoredPosition = Vector2.zero;

                    Text buttonText = buttonTextObj.AddComponent<Text>();
                    buttonText.text = option.Label;
                    buttonText.font = GetFont();
                    buttonText.fontSize = 16;
                    buttonText.alignment = TextAnchor.MiddleCenter;
                    buttonText.color = Color.white;

                    // ボタンのコールバックを設定
                    // IncidentContentのOnActionSelectedを呼び出す
                    string actionId = option.NextStateId ?? option.Label;
                    IncidentContentOption optionCopy = option; // クロージャーのためにコピー
                    button.onClick.AddListener(() =>
                    {
                        if (content != null)
                        {
                            IncidentAction action = new IncidentAction(actionId);
                            content.OnActionSelected(action);
                        }
                    });
                }
            }

            // contentAreaを設定（IncidentWindowのGetContentAreaで使用）
            windowComponent.SetContentArea(panelObj.transform);

            return windowRoot;
        }

        /// <summary>
        /// コンテンツからインシデントウィンドウを作成
        /// </summary>
        /// <param name="content">IncidentOptionalContent</param>
        /// <returns>IncidentWindowコンポーネント付きGameObject</returns>
        public static GameObject CreateWindowFromContent(IncidentOptionalContent content)
        {
            if (content == null)
            {
                Debug.LogError("[IncidentOptionalContentWindowBuilder] content is null");
                return null;
            }

            return CreateWindow(
                content.Title ?? "インシデント",
                content.ImagePath,
                content.MessageText ?? "",
                content.Options,
                content
            );
        }

        /// <summary>
        /// フォントを取得（日本語対応）
        /// </summary>
        public static Font GetFont()
        {
            Font font = Resources.Load<Font>("Fonts/NotoSansJP-Medium");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return font;
        }
    }
}
