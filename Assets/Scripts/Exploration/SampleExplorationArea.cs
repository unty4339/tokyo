using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// サンプル探索エリアの実装クラス
    /// 基本的な割り当て/解除ロジックを実装
    /// </summary>
    public class SampleExplorationArea : ExplorationArea
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
    }
}
