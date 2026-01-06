using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// ダミー部員を追加するボタンのコントローラー
    /// </summary>
    public class AddDummyMemberController : MonoBehaviour
    {
        [SerializeField] private Button addButton;

        private void Awake()
        {
            if (addButton != null)
            {
                addButton.onClick.AddListener(OnAddDummyMembers);
            }
        }

        /// <summary>
        /// ダミー部員を追加
        /// </summary>
        private void OnAddDummyMembers()
        {
            var manager = ClubMemberManager.Instance;
            
            // 複数人のダミー部員を追加（例：5人）
            int count = 5;
            for (int i = 0; i < count; i++)
            {
                int memberNumber = manager.GetMemberCount() + 1;
                string lastName = "ダミー";
                string firstName = $"{memberNumber}号";
                
                var member = DummyDataFactory.CreateRandomClubMember(level: 10, lastName: lastName, firstName: firstName);
                manager.AddMember(member);
            }

            Debug.Log($"ダミー部員を{count}人追加しました。現在の部員数: {manager.GetMemberCount()}");
        }
    }
}

