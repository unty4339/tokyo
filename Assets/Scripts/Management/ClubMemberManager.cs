using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 部員を管理するマネージャークラス（シングルトン）
    /// </summary>
    public class ClubMemberManager : MonoBehaviour
    {
        private static ClubMemberManager instance;

        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static ClubMemberManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObj = new GameObject("ClubMemberManager");
                    instance = managerObj.AddComponent<ClubMemberManager>();
                    DontDestroyOnLoad(managerObj);
                }
                return instance;
            }
        }

        /// <summary>
        /// 部員のリスト
        /// </summary>
        private List<ClubMember> members = new List<ClubMember>();

        /// <summary>
        /// 部員リストを取得
        /// </summary>
        public List<ClubMember> Members
        {
            get { return new List<ClubMember>(members); }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 部員を追加
        /// </summary>
        public void AddMember(ClubMember member)
        {
            if (member != null)
            {
                members.Add(member);
            }
        }

        /// <summary>
        /// 部員数を取得
        /// </summary>
        public int GetMemberCount()
        {
            return members.Count;
        }

        /// <summary>
        /// 全部員をクリア
        /// </summary>
        public void ClearMembers()
        {
            members.Clear();
        }
    }
}

