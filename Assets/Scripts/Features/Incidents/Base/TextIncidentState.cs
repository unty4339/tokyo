using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// テキスト表示のみのインシデント状態
    /// </summary>
    public class TextIncidentState : IncidentState
    {
        /// <summary>
        /// 状態ID
        /// </summary>
        private string stateId;

        /// <summary>
        /// 表示するテキスト
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 次の状態ID（オプショナル）
        /// </summary>
        public string NextStateId { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="stateId">状態ID</param>
        /// <param name="text">表示するテキスト</param>
        /// <param name="urgency">緊急度（デフォルトはDeferrable）</param>
        /// <param name="timeLimitWeeks">期限（週数）。nullの場合は期限なし</param>
        public TextIncidentState(string stateId, string text, IncidentUrgency urgency = IncidentUrgency.Deferrable, int? timeLimitWeeks = null)
        {
            this.stateId = stateId;
            StateName = stateId;
            Text = text;
            Urgency = urgency;
            TimeLimitWeeks = timeLimitWeeks;
        }

        /// <summary>
        /// 状態の一意なIDを取得
        /// </summary>
        /// <returns>状態ID</returns>
        public override string GetStateId()
        {
            return stateId;
        }

        /// <summary>
        /// IncidentContentを作成
        /// </summary>
        /// <returns>作成されたコンテンツ</returns>
        public override IncidentContent CreateContent()
        {
            return new IncidentOptionalContent
            {
                Title = StateName,
                MessageText = Text,
                Options = new IncidentContentOption[]
                {
                    new IncidentContentOption("閉じる", "end")
                }
            };
        }

        /// <summary>
        /// 状態遷移
        /// </summary>
        /// <param name="action">アクション。nullの場合は時間切れを意味する</param>
        /// <returns>次の状態。nullの場合はインシデント終了</returns>
        public override IncidentState Translate(IncidentAction action)
        {
            // 時間切れの場合
            if (action == null)
            {
                return null; // 終了
            }

            // "end"アクションの場合は終了
            if (action.ActionId == "end")
            {
                return null;
            }

            // デフォルトでは終了しない（継続）
            return null;
        }
    }
}
