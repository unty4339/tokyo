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
        public override IncidentState GetInitialState(IncidentProcess process)
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

            return new ChoiceIncidentState("initial", message, options, IncidentUrgency.Immediate);
        }

        /// <summary>
        /// 現在の状態からコンテンツを作成
        /// </summary>
        public override IncidentContent CreateContentFromState(IncidentProcess process)
        {
            if (process == null || process.CurrentState == null)
            {
                return null;
            }

            var state = process.CurrentState;

            // 戦闘状態の場合は特別処理
            if (state is BattleIncidentState battleState)
            {
                var content = IncidentContentFactory.CreateBattleContent(battleState);
                if (content != null)
                {
                    content.Process = process;
                }
                return content;
            }

            // テキスト状態
            if (state is TextIncidentState textState)
            {
                return new IncidentOptionalContent
                {
                    Title = "探索イベント",
                    MessageText = textState.Text,
                    Process = process,
                    Options = new IncidentContentOption[]
                    {
                        new IncidentContentOption("閉じる", "end")
                    }
                };
            }

            // 選択肢状態
            if (state is ChoiceIncidentState choiceState)
            {
                return new IncidentOptionalContent
                {
                    Title = "探索イベント",
                    MessageText = choiceState.Text,
                    Process = process,
                    Options = choiceState.Options
                };
            }

            return null;
        }

        /// <summary>
        /// コンテンツの操作から次の状態を計算
        /// </summary>
        public override IncidentState CalculateNextState(IncidentProcess process, string actionId)
        {
            if (process == null || process.CurrentState == null)
            {
                return null;
            }

            string currentStateId = process.CurrentState.GetStateId();

            // 初期状態からの遷移
            if (currentStateId == "initial")
            {
                if (actionId == "battle_choice")
                {
                    // 戦闘を選択した場合
                    return CreateBattleState();
                }
                else if (actionId == "flee_choice")
                {
                    // 逃げるを選択した場合
                    return new TextIncidentState("fled", "無事に逃げ切った。", IncidentUrgency.Deferrable);
                }
            }

            // 戦闘状態からの遷移
            if (currentStateId == "battle" && process.CurrentState is BattleIncidentState battleState)
            {
                if (battleState.IsBattleFinished && battleState.Result != null)
                {
                    // 戦闘結果に応じた状態を返す
                    if (battleState.Result.Result == BattleState.PlayerWon)
                    {
                        return new TextIncidentState("battle_won", "戦闘に勝利した！", IncidentUrgency.Deferrable);
                    }
                    else
                    {
                        return new TextIncidentState("battle_lost", "戦闘に敗北した...", IncidentUrgency.Deferrable);
                    }
                }
            }

            // 終了状態
            if (actionId == "end")
            {
                // nullを返すとインシデントが終了する
                return null;
            }

            return null;
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

            return new BattleIncidentState(playerTeam, enemyTeam, "battle");
        }

        public override void OnResolve(IncidentProcess process)
        {
            base.OnResolve(process);

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
