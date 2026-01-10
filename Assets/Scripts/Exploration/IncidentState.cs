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
    }
}
