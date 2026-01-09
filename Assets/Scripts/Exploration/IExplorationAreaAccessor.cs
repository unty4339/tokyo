using System.Collections.Generic;

namespace MonsterBattleGame
{
    /// <summary>
    /// 探索エリアへのアクセスを提供するインターフェース
    /// イベントが部員を操作するために使用される
    /// </summary>
    public interface IExplorationAreaAccessor
    {
        /// <summary>
        /// 割り当てられている部員のリストを取得（読み取り専用）
        /// </summary>
        /// <returns>割り当てられている部員のリスト</returns>
        IReadOnlyList<ClubMember> GetAssignedMembers();

        /// <summary>
        /// 部員を割り当てる
        /// </summary>
        /// <param name="member">割り当てる部員</param>
        /// <returns>割り当てに成功した場合はtrue</returns>
        bool AssignMember(ClubMember member);

        /// <summary>
        /// 部員の割り当てを解除する
        /// </summary>
        /// <param name="member">解除する部員</param>
        /// <returns>解除に成功した場合はtrue</returns>
        bool UnassignMember(ClubMember member);
    }
}
