using System;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントウィンドウの選択肢を表すクラス
    /// </summary>
    public class IncidentWindowOption
    {
        /// <summary>
        /// 選択肢の表示名
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 選択時のコールバック（IncidentInstanceを引数に取る）
        /// </summary>
        public Action<IncidentInstance> OnSelected { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="label">選択肢の表示名</param>
        /// <param name="onSelected">選択時のコールバック</param>
        public IncidentWindowOption(string label, Action<IncidentInstance> onSelected)
        {
            Label = label;
            OnSelected = onSelected;
        }
    }
}

