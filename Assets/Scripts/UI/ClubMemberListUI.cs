using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 部員一覧ウィンドウのUI制御
    /// </summary>
    public class ClubMemberListUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject listWindow;
        [SerializeField] private Transform memberGridArea;
        [SerializeField] private Button closeButton;

        [Header("Member Card Prefab")]
        [SerializeField] private GameObject memberCardPrefab;

        private List<GameObject> memberCardObjects = new List<GameObject>();

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

        private void Awake()
        {
            if (listWindow != null)
            {
                listWindow.SetActive(false);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseWindow);
            }
        }

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        public void OpenWindow()
        {
            if (listWindow != null)
            {
                listWindow.SetActive(true);
                UpdateMemberList();
            }
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        public void CloseWindow()
        {
            if (listWindow != null)
            {
                listWindow.SetActive(false);
            }
        }

        /// <summary>
        /// 閉じるボタンのイベントハンドラ
        /// </summary>
        private void OnCloseWindow()
        {
            CloseWindow();
        }

        /// <summary>
        /// 部員リストを更新
        /// </summary>
        public void UpdateMemberList()
        {
            if (memberGridArea == null)
            {
                return;
            }

            // 既存のカードを削除
            ClearMemberCards();

            // 部員マネージャーから部員リストを取得
            var manager = ClubMemberManager.Instance;
            var members = manager.Members;

            // 各部員のカードを作成
            foreach (var member in members)
            {
                CreateMemberCard(member);
            }
        }

        /// <summary>
        /// 部員カードを作成
        /// </summary>
        private void CreateMemberCard(ClubMember member)
        {
            if (memberGridArea == null)
            {
                return;
            }

            // カードオブジェクトを作成
            GameObject cardObj = new GameObject("MemberCard");
            cardObj.transform.SetParent(memberGridArea, false);

            RectTransform cardRect = cardObj.AddComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(150, 100);

            // 背景画像
            Image cardImage = cardObj.AddComponent<Image>();
            cardImage.color = new Color(0.3f, 0.3f, 0.5f, 1f);

            // 情報テキスト
            GameObject textObj = new GameObject("InfoText");
            textObj.transform.SetParent(cardObj.transform, false);
            Text infoText = textObj.AddComponent<Text>();
            
            // 学年を日本語文字列に変換
            string gradeText = member.Grade switch
            {
                Grade.FirstYear => "1年生",
                Grade.SecondYear => "2年生",
                Grade.ThirdYear => "3年生",
                _ => ""
            };
            
            infoText.text = $"{member.FullName}\n{gradeText}\nLv.{member.Level}";
            infoText.font = GetFont();
            infoText.fontSize = 14;
            infoText.alignment = TextAnchor.MiddleCenter;
            infoText.color = Color.white;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);

            memberCardObjects.Add(cardObj);
        }

        /// <summary>
        /// 部員カードをクリア
        /// </summary>
        private void ClearMemberCards()
        {
            foreach (var cardObj in memberCardObjects)
            {
                if (cardObj != null)
                {
                    Destroy(cardObj);
                }
            }
            memberCardObjects.Clear();
        }
    }
}

