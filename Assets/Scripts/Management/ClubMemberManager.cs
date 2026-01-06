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

        /// <summary>
        /// 部員を削除
        /// </summary>
        /// <param name="member">削除する部員</param>
        public void RemoveMember(ClubMember member)
        {
            if (member != null && members.Contains(member))
            {
                members.Remove(member);
            }
        }

        /// <summary>
        /// 全部員の学年を1つ上げる（3年生はそのまま）
        /// </summary>
        public void PromoteAllMembers()
        {
            foreach (var member in members)
            {
                if (member == null)
                {
                    continue;
                }

                switch (member.Grade)
                {
                    case Grade.FirstYear:
                        member.Grade = Grade.SecondYear;
                        break;
                    case Grade.SecondYear:
                        member.Grade = Grade.ThirdYear;
                        break;
                    case Grade.ThirdYear:
                        // 3年生はそのまま
                        break;
                }
            }
        }

        /// <summary>
        /// 指定学年の部員リストを取得
        /// </summary>
        /// <param name="grade">学年</param>
        /// <returns>指定学年の部員リスト</returns>
        public List<ClubMember> GetMembersByGrade(Grade grade)
        {
            return members.FindAll(m => m != null && m.Grade == grade);
        }
    }
}

