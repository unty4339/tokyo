namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘の勝敗を示すIncidentActionのサブクラス
    /// </summary>
    public class BattleResultAction : IncidentAction
    {
        /// <summary>
        /// 戦闘結果
        /// </summary>
        public BattleResult BattleResult { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="battleResult">戦闘結果</param>
        public BattleResultAction(BattleResult battleResult) : base("battle_result")
        {
            BattleResult = battleResult;
        }

        /// <summary>
        /// コンストラクタ（カスタムID付き）
        /// </summary>
        /// <param name="actionId">アクションID</param>
        /// <param name="battleResult">戦闘結果</param>
        public BattleResultAction(string actionId, BattleResult battleResult) : base(actionId)
        {
            BattleResult = battleResult;
        }

        /// <summary>
        /// 等価判定（BattleResultも考慮）
        /// </summary>
        /// <param name="obj">比較対象</param>
        /// <returns>等価な場合はtrue</returns>
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            BattleResultAction other = obj as BattleResultAction;
            if (other == null)
            {
                return false;
            }

            // BattleResultの比較（Resultプロパティで判定）
            if (BattleResult == null && other.BattleResult == null)
            {
                return true;
            }

            if (BattleResult == null || other.BattleResult == null)
            {
                return false;
            }

            return BattleResult.Result == other.BattleResult.Result;
        }

        /// <summary>
        /// ハッシュコードを取得（BattleResultも考慮）
        /// </summary>
        /// <returns>ハッシュコード</returns>
        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            if (BattleResult != null)
            {
                hash = hash * 31 + BattleResult.Result.GetHashCode();
            }
            return hash;
        }
    }
}
