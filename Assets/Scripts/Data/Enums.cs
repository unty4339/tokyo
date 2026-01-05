namespace MonsterBattleGame
{
    /// <summary>
    /// 攻撃範囲
    /// </summary>
    public enum AttackRange
    {
        /// <summary>単体攻撃</summary>
        Single,
        /// <summary>全体攻撃</summary>
        All
    }

    /// <summary>
    /// 特性の発動条件タイプ
    /// </summary>
    public enum ConditionType
    {
        /// <summary>HPが一定以下</summary>
        HPBelow,
        /// <summary>HPが一定以上</summary>
        HPAbove,
        /// <summary>特定ターン</summary>
        TurnNumber
    }

    /// <summary>
    /// 特性の効果タイプ
    /// </summary>
    public enum EffectType
    {
        /// <summary>攻撃力上昇</summary>
        AttackBoost,
        /// <summary>防御力上昇</summary>
        DefenseBoost,
        /// <summary>素早さ上昇</summary>
        SpeedBoost,
        /// <summary>回復</summary>
        Heal
    }

    /// <summary>
    /// 戦闘状態
    /// </summary>
    public enum BattleState
    {
        /// <summary>未開始</summary>
        NotStarted,
        /// <summary>進行中</summary>
        InProgress,
        /// <summary>プレイヤー勝利</summary>
        PlayerWon,
        /// <summary>プレイヤー敗北</summary>
        PlayerLost
    }

    /// <summary>
    /// 学年
    /// </summary>
    public enum Grade
    {
        /// <summary>1年生</summary>
        FirstYear,
        /// <summary>2年生</summary>
        SecondYear,
        /// <summary>3年生</summary>
        ThirdYear
    }
}

