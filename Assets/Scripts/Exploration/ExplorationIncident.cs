using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 探索エリア用のインシデント基底クラス
    /// 探索エリアから発行されるイベントで、部員を操作可能にする
    /// </summary>
    public abstract class ExplorationIncident : Incident
    {
        /// <summary>
        /// 探索エリアへのアクセサー
        /// イベントが部員を操作するために使用される
        /// </summary>
        protected IExplorationAreaAccessor AreaAccessor { get; private set; }

        /// <summary>
        /// 探索エリアへのアクセサーを設定
        /// イベント発行時に呼び出される
        /// </summary>
        /// <param name="accessor">探索エリアへのアクセサー</param>
        public void SetAreaAccessor(IExplorationAreaAccessor accessor)
        {
            AreaAccessor = accessor;
        }

        /// <summary>
        /// インシデントが解決されたときに呼ばれる
        /// 探索エリア用のカスタム処理を実装可能
        /// </summary>
        /// <param name="process">解決されたIncidentProcess</param>
        public override void OnResolve(IncidentProcess process)
        {
            base.OnResolve(process);
            // 派生クラスでオーバーライドしてカスタム処理を実装
        }

        /// <summary>
        /// 初期状態を取得
        /// 派生クラスで実装する必要がある
        /// </summary>
        /// <param name="process">IncidentProcess（オプショナル、後方互換性のためnull可）</param>
        /// <returns>初期状態</returns>
        public abstract IncidentState GetInitialState(IncidentProcess process);


        /// <summary>
        /// 現在の状態からコンテンツを作成
        /// </summary>
        /// <param name="process">IncidentProcess</param>
        /// <returns>作成されたコンテンツ</returns>
        public virtual IncidentContent CreateContentFromState(IncidentProcess process)
        {
            if (process == null || process.CurrentState == null)
            {
                Debug.LogWarning("[ExplorationIncident] process or CurrentState is null");
                return null;
            }

            // デフォルト実装：基本的なコンテンツを作成
            // 派生クラスでオーバーライドしてカスタマイズ可能
            var content = new IncidentOptionalContent
            {
                Title = Id,
                MessageText = "イベントが発生しました。",
                Process = process
            };

            return content;
        }


        /// <summary>
        /// コンテンツの操作から次の状態を計算
        /// 派生クラスで実装する必要がある
        /// </summary>
        /// <param name="process">IncidentProcess</param>
        /// <param name="actionId">選択されたアクションのID（選択肢のNextStateIdなど）</param>
        /// <returns>次の状態。nullの場合はインシデント終了</returns>
        public abstract IncidentState CalculateNextState(IncidentProcess process, string actionId);


        /// <summary>
        /// 現在の状態でポーズが必要かチェック
        /// </summary>
        /// <param name="process">IncidentProcess</param>
        /// <returns>ポーズが必要な場合はtrue</returns>
        public virtual bool ShouldPause(IncidentProcess process)
        {
            if (process == null || process.CurrentState == null)
            {
                return IsMandatory; // 状態がない場合はIsMandatoryに従う
            }

            return process.CurrentState.Urgency == IncidentUrgency.Immediate;
        }
    }
}
