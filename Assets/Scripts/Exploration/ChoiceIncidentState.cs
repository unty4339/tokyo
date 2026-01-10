namespace MonsterBattleGame
{
    /// <summary>
    /// 選択肢のあるインシデント状態
    /// </summary>
    public class ChoiceIncidentState : IncidentState
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
        /// 選択肢の配列
        /// </summary>
        public IncidentContentOption[] Options { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stateId">状態ID</param>
        /// <param name="text">表示するテキスト</param>
        /// <param name="options">選択肢の配列</param>
        /// <param name="urgency">緊急度（デフォルトはImmediate）</param>
        public ChoiceIncidentState(string stateId, string text, IncidentContentOption[] options = null, IncidentUrgency urgency = IncidentUrgency.Immediate)
        {
            this.stateId = stateId;
            StateName = stateId;
            Text = text;
            Options = options;
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
