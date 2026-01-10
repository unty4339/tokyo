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
        /// コンストラクタ
        /// </summary>
        /// <param name="playerTeam">プレイヤーチーム</param>
        /// <param name="enemyTeam">敵チーム</param>
        /// <param name="stateId">状態ID（オプショナル）</param>
        public BattleIncidentState(Team playerTeam, Team enemyTeam, string stateId = "battle")
        {
            PlayerTeam = playerTeam;
            EnemyTeam = enemyTeam;
            this.stateId = stateId;
            StateName = stateId;
            Urgency = IncidentUrgency.Immediate; // 戦闘は常に即時解決が必要
            IsBattleStarted = false;
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
    }
}
