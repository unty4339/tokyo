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
        /// <param name="instance">解決されたインシデントインスタンス</param>
        public override void OnResolve(IncidentInstance instance)
        {
            base.OnResolve(instance);
            // 派生クラスでオーバーライドしてカスタム処理を実装
        }
    }
}
