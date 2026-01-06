using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 部員一覧を開くボタンのコントローラー
    /// </summary>
    public class OpenMemberListController : MonoBehaviour
    {
        [SerializeField] private Button openButton;

        private void Awake()
        {
            if (openButton != null)
            {
                openButton.onClick.AddListener(OnOpenMemberList);
            }
        }

        /// <summary>
        /// 部員一覧を開く
        /// </summary>
        private void OnOpenMemberList()
        {
            ClubMemberListUI listUI = FindFirstObjectByType<ClubMemberListUI>(FindObjectsInactive.Include);
            if (listUI != null)
            {
                listUI.OpenWindow();
            }
            else
            {
                Debug.LogWarning("ClubMemberListUI not found. Please create Club Member List UI first.");
            }
        }
    }
}

