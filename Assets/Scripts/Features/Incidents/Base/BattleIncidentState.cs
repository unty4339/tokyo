using System;

namespace MonsterBattleGame
{
    /// <summary>
    /// 戦闘を表す特別なインシデント状態
    /// </summary>
    public class BattleIncidentState : IncidentState
    {
        /// <summary>
        /// プレイヤーチーム
        /// </summary>
        public Team PlayerTeam { get; set; }

        /// <summary>
        /// 敵チーム
        /// </summary>
        public Team EnemyTeam { get; set; }

        /// <summary>
        /// 戦闘結果（戦闘終了後に設定される）
        /// </summary>
        public BattleResult Result { get; private set; }

        /// <summary>
        /// 戦闘が開始されたかどうか
        /// </summary>
        public bool IsBattleStarted { get; private set; }

        /// <summary>
        /// 戦闘が終了したかどうか
        /// </summary>
        public bool IsBattleFinished => Result != null;

        /// <summary>
        /// 状態ID
        /// </summary>
        private string stateId;

        /// <summary>
        /// 戦闘結果に基づく次の状態を作成する関数
        /// </summary>
        public System.Func<BattleResult, IncidentState> OnBattleEndTransition { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="playerTeam">プレイヤーチーム</param>
        /// <param name="enemyTeam">敵チーム</param>
        /// <param name="stateId">状態ID（オプショナル）</param>
        /// <param name="timeLimitWeeks">期限（週数）。nullの場合は期限なし</param>
        public BattleIncidentState(Team playerTeam, Team enemyTeam, string stateId = "battle", int? timeLimitWeeks = null)
        {
            PlayerTeam = playerTeam;
            EnemyTeam = enemyTeam;
            this.stateId = stateId;
            StateName = stateId;
            Urgency = IncidentUrgency.Immediate; // 戦闘は常に即時解決が必要
            IsBattleStarted = false;
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
        /// この状態で強制ポーズが必要かを取得
        /// 戦闘中は常にポーズが必要（UrgencyがImmediateのため常にtrue）
        /// </summary>
        /// <returns>true</returns>
        public override bool RequiresPause()
        {
            return Urgency == IncidentUrgency.Immediate;
        }

        /// <summary>
        /// 戦闘を開始
        /// </summary>
        /// <param name="onBattleEnd">戦闘終了時のコールバック</param>
        public void StartBattle(Action<BattleResult> onBattleEnd)
        {
            if (IsBattleStarted)
            {
                UnityEngine.Debug.LogWarning("[BattleIncidentState] Battle already started");
                return;
            }

            IsBattleStarted = true;

            // BattleUIを取得して戦闘を開始
            var battleUI = UnityEngine.Object.FindFirstObjectByType<BattleUI>();
            if (battleUI == null)
            {
                UnityEngine.Debug.LogError("[BattleIncidentState] BattleUI not found");
                onBattleEnd?.Invoke(null);
                return;
            }

            // 戦闘終了時に結果を設定するコールバックをラップ
            battleUI.StartBattle(PlayerTeam, EnemyTeam, (result) =>
            {
                SetBattleResult(result);
                onBattleEnd?.Invoke(result);
            });
        }

        /// <summary>
        /// 戦闘結果を設定
        /// </summary>
        /// <param name="result">戦闘結果</param>
        public void SetBattleResult(BattleResult result)
        {
            Result = result;
        }

        /// <summary>
        /// IncidentContentを作成
        /// </summary>
        /// <returns>作成されたコンテンツ</returns>
        public override IncidentContent CreateContent()
        {
            return IncidentContentFactory.CreateBattleContent(this);
        }

        /// <summary>
        /// 状態遷移
        /// </summary>
        /// <param name="action">アクション。nullの場合は時間切れを意味する。戦闘結果の場合はBattleResultAction</param>
        /// <returns>次の状態。nullの場合はインシデント終了</returns>
        public override IncidentState Translate(IncidentAction action)
        {
            // 時間切れの場合
            if (action == null)
            {
                return null; // 終了
            }

            // 戦闘結果アクションの場合
            if (action is BattleResultAction battleResultAction)
            {
                if (OnBattleEndTransition != null)
                {
                    return OnBattleEndTransition(battleResultAction.BattleResult);
                }
                // デフォルトの処理：勝敗に応じた状態を返す
                if (battleResultAction.BattleResult != null)
                {
                    if (battleResultAction.BattleResult.Result == BattleState.PlayerWon)
                    {
                        return new TextIncidentState("battle_won", "戦闘に勝利した！", IncidentUrgency.Deferrable);
                    }
                    else
                    {
                        return new TextIncidentState("battle_lost", "戦闘に敗北した...", IncidentUrgency.Deferrable);
                    }
                }
            }

            // その他のアクションの場合は終了しない（継続）
            return null;
        }
    }
}
