using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// サンプル探索イベント
    /// 探索エリアから発行されるイベントのサンプル実装
    /// 状態ベースシステムを使用した多段階選択肢の例
    /// </summary>
    public class SampleExplorationIncident : ExplorationIncident
    {
        private static int instanceCount = 0;

        public override string Id => $"sample_exploration_{instanceCount++}";

        public override bool IsMandatory => false;

        public override int? TimeLimitWeeks => 2; // 2週間で期限切れ

        public override Color IconColor => Color.yellow;

        public override bool CheckCondition(int year, int month, int week)
        {
            // 探索エリアから直接発行されるため、常にtrueを返す
            // 実際の判定はExplorationAreaの確率判定で行われる
            return true;
        }

        public override string GetWindowPrefabAddress()
        {
            // 動的作成を使用
            return null;
        }

        public override GameObject GetWindowPrefab()
        {
            // 後方互換性のため、古い方式もサポート
            // ただし、状態ベースシステムが優先される
            return null;
        }

        /// <summary>
        /// 初期状態を取得
        /// </summary>
        public override IncidentState GetInitialState()
        {
            string message = "探索中に何かを見つけた！";
            if (AreaAccessor != null)
            {
                var members = AreaAccessor.GetAssignedMembers();
                if (members.Count > 0)
                {
                    message += $"\n{members[0].FullName}が発見した。";
                }
            }

            // 選択肢を作成
            var options = new IncidentContentOption[]
            {
                new IncidentContentOption("戦闘する", "battle_choice"),
                new IncidentContentOption("逃げる", "flee_choice")
            };

            var initialState = new ChoiceIncidentState("initial", message, options, IncidentUrgency.Immediate, TimeLimitWeeks);
            
            // 状態遷移マップを設定
            initialState.TransitionMap = (actionId) =>
            {
                if (actionId == "battle_choice")
                {
                    // 戦闘を選択した場合
                    return CreateBattleState();
                }
                else if (actionId == "flee_choice")
                {
                    // 逃げるを選択した場合
                    return new TextIncidentState("fled", "無事に逃げ切った。", IncidentUrgency.Deferrable, TimeLimitWeeks)
                    {
                        NextStateId = "end"
                    };
                }
                return null;
            };

            return initialState;
        }


        /// <summary>
        /// 戦闘状態を作成
        /// </summary>
        private BattleIncidentState CreateBattleState()
        {
            // プレイヤーチームを作成（探索エリアの部員から）
            Team playerTeam = null;
            if (AreaAccessor != null)
            {
                var members = AreaAccessor.GetAssignedMembers();
                if (members.Count > 0)
                {
                    // 部員からモンスターを作成する必要があるが、
                    // ここでは簡単のためDummyDataFactoryを使用
                    playerTeam = DummyDataFactory.CreatePlayerTeam(10);
                }
            }

            // プレイヤーチームが作成できない場合はデフォルトチームを使用
            if (playerTeam == null)
            {
                playerTeam = DummyDataFactory.CreatePlayerTeam(10);
            }

            // 敵チームを作成
            Team enemyTeam = DummyDataFactory.CreateEnemyTeam(10);

            var battleState = new BattleIncidentState(playerTeam, enemyTeam, "battle", TimeLimitWeeks);
            
            // 戦闘終了時の遷移を設定
            battleState.OnBattleEndTransition = (result) =>
            {
                if (result != null)
                {
                    if (result.Result == BattleState.PlayerWon)
                    {
                        return new TextIncidentState("battle_won", "戦闘に勝利した！", IncidentUrgency.Deferrable, TimeLimitWeeks)
                        {
                            NextStateId = "end"
                        };
                    }
                    else
                    {
                        return new TextIncidentState("battle_lost", "戦闘に敗北した...", IncidentUrgency.Deferrable, TimeLimitWeeks)
                        {
                            NextStateId = "end"
                        };
                    }
                }
                return null;
            };

            return battleState;
        }

        public override void OnResolve(IncidentState state)
        {
            base.OnResolve(state);

            // 探索エリアの部員を操作する例
            if (AreaAccessor != null)
            {
                var members = AreaAccessor.GetAssignedMembers();
                foreach (var member in members)
                {
                    // 例：探索で経験値を獲得（レベルアップの可能性）
                    // ここではログ出力のみ
                    Debug.Log($"[SampleExplorationIncident] {member.FullName}が探索で経験を積んだ。");
                }
            }
        }
    }
}
