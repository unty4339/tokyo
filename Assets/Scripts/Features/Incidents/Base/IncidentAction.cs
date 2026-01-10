namespace MonsterBattleGame
{
    /// <summary>
    /// あるIncidentStateに対してプレイヤーが選択した結果などを表す
    /// MonoBehaviourを継承しない純粋なデータクラス
    /// </summary>
    public class IncidentAction
    {
        /// <summary>
        /// アクションの一意なID（id代わりの文字列）
        /// </summary>
        public string ActionId { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="actionId">アクションID</param>
        public IncidentAction(string actionId)
        {
            ActionId = actionId ?? string.Empty;
        }

        /// <summary>
        /// 簡易な等価判定（ActionIdを比較）
        /// </summary>
        /// <param name="obj">比較対象</param>
        /// <returns>等価な場合はtrue</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            IncidentAction other = obj as IncidentAction;
            return other != null && ActionId == other.ActionId;
        }

        /// <summary>
        /// ハッシュコードを取得（ActionIdベース）
        /// </summary>
        /// <returns>ハッシュコード</returns>
        public override int GetHashCode()
        {
            return ActionId?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// 文字列表現を取得
        /// </summary>
        /// <returns>アクションID</returns>
        public override string ToString()
        {
            return ActionId;
        }
    }
}
