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
        /// 選択時のコールバック
        /// 引数: IncidentProcess, 選択した選択肢のID（NextStateId）
        /// </summary>
        public Action<IncidentProcess, string> OnSelected { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="label">表示ラベル</param>
        /// <param name="nextStateId">遷移先のstate ID</param>
        /// <param name="onSelected">選択時のコールバック（オプショナル）</param>
        public IncidentContentOption(string label, string nextStateId = null, Action<IncidentProcess, string> onSelected = null)
        {
            Label = label;
            NextStateId = nextStateId;
            OnSelected = onSelected;
        }
    }
}
