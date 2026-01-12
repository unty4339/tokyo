namespace MonsterBattleGame
{
    /// <summary>
    /// ステータスを変動するための処理をするクラス
    /// </summary>
    public class Buff
    {
        /// <summary>名称</summary>
        public string Name { get; set; }

        /// <summary>ステータス変動量</summary>
        public StatusDelta StatusDelta { get; set; }

        public Buff()
        {
            Name = string.Empty;
            StatusDelta = StatusDelta.Zero;
        }

        public Buff(string name, StatusDelta statusDelta)
        {
            Name = name;
            StatusDelta = statusDelta ?? StatusDelta.Zero;
        }

        /// <summary>
        /// Statusを受け取り、ステータス変動量を示すStatusDeltaを返すメソッド
        /// </summary>
        public StatusDelta GetStatusDelta(Status status)
        {
            return StatusDelta;
        }
    }
}
