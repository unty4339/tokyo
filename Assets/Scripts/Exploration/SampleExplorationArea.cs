using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterBattleGame
{
    /// <summary>
    /// サンプル探索エリアの実装クラス
    /// 基本的な割り当て/解除ロジックを実装
    /// </summary>
    public class SampleExplorationArea : ExplorationArea, IPointerClickHandler
    {
        /// <summary>
        /// 毎週のイベントを作成
        /// サンプル実装として、基本的な探索イベントを返す
        /// </summary>
        protected override ExplorationIncident CreateWeeklyEvent(int year, int month, int week)
        {
            // サンプルイベントを作成
            return new SampleExplorationIncident();
        }

        /// <summary>
        /// オブジェクトがクリックされたときに呼ばれる
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("クリック");
        }
    }
}
