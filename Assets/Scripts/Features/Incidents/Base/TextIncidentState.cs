namespace MonsterBattleGame
{
    /// <summary>
    /// テキスト表示のみのインシデント状態
    /// </summary>
    public class TextIncidentState : IncidentState
    {
        /// <summary>
        /// 状態ID
        /// </summary>
        private string stateId;

        /// <summary>
        /// 表示するテキスト
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stateId">状態ID</param>
        /// <param name="text">表示するテキスト</param>
        /// <param name="urgency">緊急度（デフォルトはDeferrable）</param>
        public TextIncidentState(string stateId, string text, IncidentUrgency urgency = IncidentUrgency.Deferrable)
        {
            this.stateId = stateId;
            StateName = stateId;
            Text = text;
            Urgency = urgency;
        }

        /// <summary>
        /// 状態の一意なIDを取得
        /// </summary>
        /// <returns>状態ID</returns>
        public override string GetStateId()
        {
            return stateId;
        }
    }
}
