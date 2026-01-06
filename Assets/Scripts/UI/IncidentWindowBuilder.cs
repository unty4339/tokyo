using System;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントウィンドウを動的に作成する静的クラス
    /// </summary>
    public static class IncidentWindowBuilder
    {
        /// <summary>
        /// インシデントウィンドウを動的に作成
        /// </summary>
        /// <param name="title">インシデント名（タイトル）</param>
        /// <param name="imagePath">画像のパス（オプショナル、null可）</param>
        /// <param name="messageText">表示するテキスト</param>
        /// <param name="options">選択肢の配列</param>
        /// <returns>IncidentWindowコンポーネント付きGameObject</returns>
        public static GameObject CreateWindow(string title, string imagePath, string messageText, IncidentWindowOption[] options)
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
                optionsRect.anchorMax = new Vector2(1, 0.3f);
                optionsRect.sizeDelta = Vector2.zero;
                optionsRect.anchoredPosition = Vector2.zero;

                // VerticalLayoutGroupを追加してボタンを縦に並べる
                VerticalLayoutGroup layoutGroup = optionsContainer.AddComponent<VerticalLayoutGroup>();
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                layoutGroup.spacing = 10;
                layoutGroup.padding = new RectOffset(20, 20, 10, 10);
                ContentSizeFitter sizeFitter = optionsContainer.AddComponent<ContentSizeFitter>();
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                for (int i = 0; i < options.Length; i++)
                {
                    IncidentWindowOption option = options[i];
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

                    // ボタンクリック時のコールバックを設定
                    // IncidentInstanceは後でSetIncidentInstanceで設定される
                    button.onClick.AddListener(() =>
                    {
                        if (windowComponent.Instance != null && option.OnSelected != null)
                        {
                            option.OnSelected(windowComponent.Instance);
                        }
                    });
                }
            }

            // contentAreaを設定（IncidentWindowのGetContentAreaで使用）
            windowComponent.SetContentArea(panelObj.transform);

            return windowRoot;
        }

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
    }
}

