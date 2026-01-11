using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 選択肢を持つインシデントコンテンツ
    /// タイトル、メッセージ、画像、選択肢を受け取ってゲームオブジェクトを作成する
    /// </summary>
    public class IncidentOptionalContent : IncidentContent
    {
        /// <summary>
        /// 選択肢の配列（オプショナル、null可）
        /// </summary>
        public IncidentContentOption[] Options { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IncidentOptionalContent()
        {
            Type = IncidentContentType.Normal;
        }

        /// <summary>
        /// 選択肢があるかチェック
        /// </summary>
        /// <returns>選択肢がある場合はtrue</returns>
        public bool HasOptions()
        {
            return Options != null && Options.Length > 0;
        }

        /// <summary>
        /// 自身の子オブジェクト（画面に配置するボタン等）を一括で作成する機能
        /// </summary>
        /// <param name="parent">親のTransform</param>
        /// <returns>作成された子オブジェクトのルートTransform</returns>
        public override Transform CreateChildObjects(Transform parent)
        {
            if (parent == null)
            {
                Debug.LogError("[IncidentOptionalContent] parent is null");
                return null;
            }

            // コンテンツエリアを作成
            GameObject contentArea = new GameObject("ContentArea");
            contentArea.transform.SetParent(parent, false);
            RectTransform contentRect = contentArea.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;
            contentRect.anchoredPosition = Vector2.zero;

            // タイトルを作成
            if (!string.IsNullOrEmpty(Title))
            {
                GameObject titleObj = new GameObject("Title");
                titleObj.transform.SetParent(contentArea.transform, false);
                RectTransform titleRect = titleObj.AddComponent<RectTransform>();
                titleRect.anchorMin = new Vector2(0, 0.8f);
                titleRect.anchorMax = new Vector2(1, 1);
                titleRect.sizeDelta = Vector2.zero;
                titleRect.anchoredPosition = Vector2.zero;

                Text titleText = titleObj.AddComponent<Text>();
                titleText.text = Title;
                titleText.font = IncidentOptionalContentWindowBuilder.GetFont();
                titleText.fontSize = 24;
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.color = Color.white;
            }

            // メッセージテキストを作成
            if (!string.IsNullOrEmpty(MessageText))
            {
                GameObject messageObj = new GameObject("Message");
                messageObj.transform.SetParent(contentArea.transform, false);
                RectTransform messageRect = messageObj.AddComponent<RectTransform>();
                messageRect.anchorMin = new Vector2(0.1f, 0.3f);
                messageRect.anchorMax = new Vector2(0.9f, 0.7f);
                messageRect.sizeDelta = Vector2.zero;
                messageRect.anchoredPosition = Vector2.zero;

                Text messageTextComponent = messageObj.AddComponent<Text>();
                messageTextComponent.text = MessageText;
                messageTextComponent.font = IncidentOptionalContentWindowBuilder.GetFont();
                messageTextComponent.fontSize = 18;
                messageTextComponent.alignment = TextAnchor.MiddleCenter;
                messageTextComponent.color = Color.white;
            }

            // 選択肢ボタンを作成
            Debug.Log($"HasOptions: {HasOptions()}");
            Debug.Log($"Options: {Options.Length}");
            if (HasOptions())
            {
                GameObject optionsContainer = new GameObject("OptionsContainer");
                optionsContainer.transform.SetParent(contentArea.transform, false);
                RectTransform optionsRect = optionsContainer.AddComponent<RectTransform>();
                optionsRect.anchorMin = new Vector2(0, 0);
                optionsRect.anchorMax = new Vector2(1, 0.25f);
                optionsRect.sizeDelta = Vector2.zero;
                optionsRect.anchoredPosition = Vector2.zero;

                HorizontalLayoutGroup layoutGroup = optionsContainer.AddComponent<HorizontalLayoutGroup>();
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                layoutGroup.spacing = 10;
                layoutGroup.padding = new RectOffset(20, 20, 10, 10);
                layoutGroup.childControlWidth = false;
                layoutGroup.childControlHeight = false;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;

                for (int i = 0; i < Options.Length; i++)
                {
                    var option = Options[i];
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
                    buttonText.font = IncidentOptionalContentWindowBuilder.GetFont();
                    buttonText.fontSize = 16;
                    buttonText.alignment = TextAnchor.MiddleCenter;
                    buttonText.color = Color.white;

                    // ボタンクリック時にIncidentActionを生成してIncidentManagerに渡す
                    string actionId = option.NextStateId ?? option.Label;
                    button.onClick.AddListener(() =>
                    {
                        OnActionSelected(new IncidentAction(actionId));
                    });
                }
            }

            return contentArea.transform;
        }
    }
}
