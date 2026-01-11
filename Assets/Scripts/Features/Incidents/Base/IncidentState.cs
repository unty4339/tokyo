namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントの緊急度（即時解決が必要か後回しにできるか）
    /// </summary>
    public enum IncidentUrgency
    {
        /// <summary>即時解決が必要（ポーズ解除不可）</summary>
        Immediate,
        /// <summary>後回しにできる（ポーズ解除可能）</summary>
        Deferrable
    }

    /// <summary>
    /// インシデントの状態を表す基底クラス
    /// 各状態はウィンドウに表示するコンテンツを生成できる
    /// </summary>
    public abstract class IncidentState
    {
        /// <summary>
        /// 状態の名称
        /// </summary>
        public string StateName { get; protected set; }

        /// <summary>
        /// インシデントの緊急度（即時解決が必要か後回しにできるか）
        /// </summary>
        public IncidentUrgency Urgency { get; protected set; }

        /// <summary>
        /// 後回しにできる場合、何週で期限切れになるか（週数）
        /// nullの場合は期限なし
        /// </summary>
        public int? TimeLimitWeeks { get; protected set; }

        /// <summary>
        /// 状態の一意なIDを取得
        /// </summary>
        /// <returns>状態ID</returns>
        public abstract string GetStateId();

        /// <summary>
        /// この状態で強制ポーズが必要かを取得
        /// UrgencyがImmediateの場合はtrue、Deferrableの場合はfalse
        /// </summary>
        /// <returns>ポーズが必要な場合はtrue</returns>
        public virtual bool RequiresPause()
        {
            return Urgency == IncidentUrgency.Immediate;
        }

        /// <summary>
        /// 現在の状態が即時の解決が必要かどうかを判定
        /// 外部から参照する際はこのメソッドで判定させる
        /// </summary>
        /// <returns>即時解決が必要な場合はtrue</returns>
        public virtual bool IsImmediately()
        {
            return Urgency == IncidentUrgency.Immediate;
        }

        /// <summary>
        /// IncidentContentを作成する機能
        /// </summary>
        /// <returns>作成されたコンテンツ</returns>
        public abstract IncidentContent CreateContent();

        /// <summary>
        /// IncidentActionインスタンスを受け取り、別のIncidentStateインスタンスかnullを返す
        /// 受け取るIncidentActionインスタンスがnullのときは時間切れを意味する
        /// 戻り値としてnullへの遷移は終端状態（終了）を示す
        /// </summary>
        /// <param name="action">アクション。nullの場合は時間切れを意味する</param>
        /// <returns>次の状態。nullの場合はインシデント終了</returns>
        public abstract IncidentState Translate(IncidentAction action);
    }
}
