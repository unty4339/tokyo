namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘中に発動し敵味方に作用する技の抽象クラス
    /// </summary>
    public abstract class Move
    {
        /// <summary>名称</summary>
        public string Name { get; set; }

        public Move()
        {
            Name = string.Empty;
        }

        public Move(string name)
        {
            Name = name;
        }
    }
}
