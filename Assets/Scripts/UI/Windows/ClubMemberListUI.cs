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

            // MemberIconGeneratorを使用してアイコンを生成
            GameObject iconObj = MemberIconGenerator.CreateMemberIcon(member, new Vector2(150, 100));
            if (iconObj != null)
            {
                iconObj.transform.SetParent(memberGridArea, false);
                memberCardObjects.Add(iconObj);
            }
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

