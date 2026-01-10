using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 部員アイコンを生成するクラス
    /// 部員データからGameObjectアイコンを動的に生成する
    /// </summary>
    public static class MemberIconGenerator
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
        /// 学年を日本語文字列に変換
        /// </summary>
        private static string GetGradeText(Grade grade)
        {
            return grade switch
            {
                Grade.FirstYear => "1年生",
                Grade.SecondYear => "2年生",
                Grade.ThirdYear => "3年生",
                _ => ""
            };
        }

        /// <summary>
        /// 部員アイコンGameObjectを生成
        /// </summary>
        /// <param name="member">部員データ</param>
        /// <param name="size">アイコンのサイズ（幅、高さ）</param>
        /// <returns>生成されたアイコンGameObject</returns>
        public static GameObject CreateMemberIcon(ClubMember member, Vector2 size)
        {
            if (member == null)
            {
                Debug.LogWarning("MemberIconGenerator: memberがnullです");
                return null;
            }

            // アイコンオブジェクトを作成
            GameObject iconObj = new GameObject("MemberIcon");
            
            // RectTransformを追加
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = size;

            // 背景画像を追加
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = new Color(0.3f, 0.3f, 0.5f, 1f);

            // 情報テキストオブジェクトを作成
            GameObject textObj = new GameObject("InfoText");
            textObj.transform.SetParent(iconObj.transform, false);
            
            // Textコンポーネントを追加
            Text infoText = textObj.AddComponent<Text>();
            
            // 学年を日本語文字列に変換
            string gradeText = GetGradeText(member.Grade);
            
            // テキストを設定（レベル、名前、学年の順）
            infoText.text = $"Lv.{member.Level}\n{member.FullName}\n{gradeText}";
            infoText.font = GetFont();
            infoText.fontSize = 14;
            infoText.alignment = TextAnchor.MiddleCenter;
            infoText.color = Color.white;

            // テキストのRectTransformを設定
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);

            return iconObj;
        }

        /// <summary>
        /// 空の部員アイコンGameObjectを生成（部員がいないスロット用）
        /// </summary>
        /// <param name="size">アイコンのサイズ（幅、高さ）</param>
        /// <returns>生成された空のアイコンGameObject</returns>
        public static GameObject CreateEmptyMemberIcon(Vector2 size)
        {
            // アイコンオブジェクトを作成
            GameObject iconObj = new GameObject("EmptyMemberIcon");
            
            // RectTransformを追加
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = size;

            // 背景画像を追加（薄めの色で表示）
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = new Color(0.2f, 0.2f, 0.3f, 0.7f);

            // 情報テキストオブジェクトを作成
            GameObject textObj = new GameObject("InfoText");
            textObj.transform.SetParent(iconObj.transform, false);
            
            // Textコンポーネントを追加
            Text infoText = textObj.AddComponent<Text>();
            
            // テキストを設定
            infoText.text = "部員なし";
            infoText.font = GetFont();
            infoText.fontSize = 14;
            infoText.alignment = TextAnchor.MiddleCenter;
            infoText.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            // テキストのRectTransformを設定
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);

            return iconObj;
        }
    }
}
