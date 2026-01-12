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

    /// <summary>
    /// 部員の性格を表すenumクラス
    /// 経歴や台詞を生成する際に使う
    /// </summary>
    public enum Personality
    {
        /// <summary>普通</summary>
        Normal,
        /// <summary>積極的</summary>
        Active,
        /// <summary>消極的</summary>
        Passive,
        /// <summary>冷静</summary>
        Calm,
        /// <summary>熱血</summary>
        Passionate
    }

    /// <summary>
    /// 各部員の最終的な末路を表すenumクラス
    /// 在籍中の生徒はnull
    /// </summary>
    public enum Fate
    {
        /// <summary>卒業</summary>
        Graduated,
        /// <summary>退学</summary>
        DroppedOut,
        /// <summary>転校</summary>
        Transferred,
        /// <summary>死亡</summary>
        Deceased
    }

    /// <summary>
    /// 特筆する経験を示すenumクラス
    /// </summary>
    public enum CareerEvent
    {
        /// <summary>初勝利</summary>
        FirstVictory,
        /// <summary>連勝記録</summary>
        WinningStreak,
        /// <summary>大勝利</summary>
        GreatVictory,
        /// <summary>初戦闘</summary>
        FirstBattle,
        /// <summary>初妊娠</summary>
        FirstPregnancy
    }

    /// <summary>
    /// MoveSkillで追加されるスキルがどのタイミングで発動するかのenum
    /// </summary>
    public enum MoveTiming
    {
        /// <summary>ターン開始時</summary>
        TurnStart,
        /// <summary>ターン終了時</summary>
        TurnEnd,
        /// <summary>攻撃前</summary>
        BeforeAttack,
        /// <summary>攻撃後</summary>
        AfterAttack,
        /// <summary>被ダメージ時</summary>
        OnDamage,
        /// <summary>回復時</summary>
        OnHeal
    }

    /// <summary>
    /// Itemの種類を表すenumクラス
    /// </summary>
    public enum ItemTag
    {
        /// <summary>資源</summary>
        Resource,
        /// <summary>武器</summary>
        Weapon,
        /// <summary>像</summary>
        Statue,
        /// <summary>ゼリー</summary>
        Jelly
    }

    /// <summary>
    /// 武器のレアリティを表すenumクラス
    /// </summary>
    public enum WeaponRarity
    {
        /// <summary>ノーマル</summary>
        N,
        /// <summary>レア</summary>
        R,
        /// <summary>スーパーレア</summary>
        SR,
        /// <summary>レジェンダリーレア</summary>
        LR
    }

    /// <summary>
    /// 属性を表すenumクラス
    /// </summary>
    public enum BattleAttribute
    {
        /// <summary>近接</summary>
        Melee,
        /// <summary>長柄</summary>
        Pole,
        /// <summary>遠距離</summary>
        Ranged
    }
}

