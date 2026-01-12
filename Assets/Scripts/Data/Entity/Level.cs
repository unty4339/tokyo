namespace MonsterBattleGame
{
    /// <summary>
    /// レベルと経験値を管理するクラス
    /// </summary>
    public class Level
    {
        /// <summary>現在のレベル</summary>
        public int CurrentLevel { get; private set; }

        /// <summary>現在の経験値</summary>
        public int CurrentEXP { get; private set; }

        /// <summary>
        /// 次のレベルに必要な経験値
        /// 簡易実装: レベル * 100
        /// </summary>
        public int RequiredEXP
        {
            get { return CurrentLevel * 100; }
        }

        public Level()
        {
            CurrentLevel = 1;
            CurrentEXP = 0;
        }

        public Level(int level, int exp)
        {
            CurrentLevel = level;
            CurrentEXP = exp;
        }

        /// <summary>
        /// 経験値を追加し、レベルが上がった場合はTrueを返す
        /// </summary>
        /// <param name="exp">追加する経験値</param>
        /// <returns>レベルが上がった場合はTrue</returns>
        public bool AddEXPPoint(int exp)
        {
            if (exp <= 0)
            {
                return false;
            }

            bool leveledUp = false;
            CurrentEXP += exp;

            // レベルアップ判定
            while (CurrentEXP >= RequiredEXP)
            {
                CurrentEXP -= RequiredEXP;
                CurrentLevel++;
                leveledUp = true;
            }

            return leveledUp;
        }

        /// <summary>
        /// レベルを直接設定（デバッグ用）
        /// </summary>
        public void SetLevel(int level)
        {
            CurrentLevel = System.Math.Max(1, level);
        }
    }
}
