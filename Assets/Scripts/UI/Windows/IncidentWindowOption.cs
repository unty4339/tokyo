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
        /// コンストラクタ
        /// </summary>
        /// <param name="label">選択肢の表示名</param>
        public IncidentWindowOption(string label)
        {
            Label = label;
        }
    }
}

