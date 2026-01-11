using System;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントコンテンツの選択肢
    /// </summary>
    public class IncidentContentOption
    {
        /// <summary>
        /// 選択肢の表示ラベル
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 遷移先のstate ID（オプショナル）
        /// </summary>
        public string NextStateId { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="label">表示ラベル</param>
        /// <param name="nextStateId">遷移先のstate ID</param>
        public IncidentContentOption(string label, string nextStateId = null)
        {
            Label = label;
            NextStateId = nextStateId;
        }
    }
}
