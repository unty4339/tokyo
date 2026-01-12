namespace MonsterBattleGame
{
    /// <summary>
    /// バフ技のクラス
    /// </summary>
    public class BuffMove : Move
    {
        /// <summary>説明文のstring</summary>
        public string Description { get; set; }

        /// <summary>MovePriorityインスタンス</summary>
        public MovePriority Priority { get; set; }

        /// <summary>StatusDeltaを返すメソッド用のプロパティ</summary>
        public StatusDelta StatusDelta { get; set; }

        public BuffMove() : base()
        {
            Description = string.Empty;
            Priority = new MovePriority();
            StatusDelta = StatusDelta.Zero;
        }

        public BuffMove(string name, string description, MovePriority priority, StatusDelta statusDelta) : base(name)
        {
            Description = description;
            Priority = priority ?? new MovePriority();
            StatusDelta = statusDelta ?? StatusDelta.Zero;
        }

        /// <summary>
        /// StatusDeltaクラスを返す
        /// </summary>
        public StatusDelta GetStatusDelta()
        {
            return StatusDelta;
        }
    }
}
