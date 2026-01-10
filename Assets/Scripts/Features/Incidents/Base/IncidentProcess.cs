using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 一連のインシデントについて責任を持つ
    /// IncidentInstanceを置き換える
    /// MonoBehaviourを継承しない
    /// </summary>
    public class IncidentProcess
    {
        /// <summary>
        /// このプロセスの元となるインシデント定義
        /// </summary>
        public Incident Incident { get; private set; }

        /// <summary>
        /// 初期状態となるIncidentStateクラス
        /// </summary>
        public IncidentState InitialState { get; set; }

        /// <summary>
        /// 現行のIncidentStateクラス
        /// </summary>
        public IncidentState CurrentState { get; set; }

        /// <summary>
        /// インシデントが発生した週（累積週数）
        /// </summary>
        public int StartWeek { get; private set; }

        /// <summary>
        /// 期限切れとなる週（累積週数）。TimeLimitWeeksがnullの場合はnull
        /// </summary>
        public int? ExpiryWeek { get; private set; }

        /// <summary>
        /// ウィンドウPrefabのインスタンス
        /// </summary>
        public GameObject WindowPrefabInstance { get; set; }

        /// <summary>
        /// 状態履歴（オプショナル、デバッグ用）
        /// </summary>
        public List<IncidentState> StateHistory { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="incident">インシデント定義</param>
        /// <param name="initialState">初期状態</param>
        /// <param name="startWeek">開始週（累積週数）</param>
        public IncidentProcess(Incident incident, IncidentState initialState, int startWeek)
        {
            Incident = incident;
            InitialState = initialState;
            CurrentState = initialState;
            StartWeek = startWeek;
            StateHistory = new List<IncidentState>();

            if (incident != null && incident.TimeLimitWeeks.HasValue)
            {
                ExpiryWeek = startWeek + incident.TimeLimitWeeks.Value;
            }
            else
            {
                ExpiryWeek = null;
            }

            if (initialState != null)
            {
                StateHistory.Add(initialState);
            }
        }

        /// <summary>
        /// 期限切れかどうかをチェック
        /// </summary>
        /// <param name="currentWeek">現在の週（累積週数）</param>
        /// <returns>期限切れの場合はtrue</returns>
        public bool IsExpired(int currentWeek)
        {
            if (!ExpiryWeek.HasValue)
            {
                return false; // 時限なし
            }
            return currentWeek >= ExpiryWeek.Value;
        }

        /// <summary>
        /// 現在の状態からIncidentContentを作成する機能
        /// ExplorationIncidentに委譲する
        /// </summary>
        /// <returns>作成されたコンテンツ</returns>
        public IncidentContent CreateContentFromState()
        {
            return CreateContentFromState(CurrentState);
        }

        /// <summary>
        /// 特定のIncidentStateからIncidentContentを作成する機能
        /// ExplorationIncidentに委譲する
        /// </summary>
        /// <param name="state">状態</param>
        /// <returns>作成されたコンテンツ</returns>
        public IncidentContent CreateContentFromState(IncidentState state)
        {
            if (state == null)
            {
                Debug.LogWarning("[IncidentProcess] state is null");
                return null;
            }

            if (Incident is ExplorationIncident explorationIncident)
            {
                // ExplorationIncidentのメソッドを使用（IncidentProcessを渡す）
                return explorationIncident.CreateContentFromState(this);
            }

            Debug.LogWarning($"[IncidentProcess] Unsupported incident type: {Incident?.GetType().Name}");
            return null;
        }

        /// <summary>
        /// 特定のIncidentStateのときに特定のIncidentActionを受け取ると別のIncidentStateを作成し保持する機能（状態遷移の表現）
        /// ExplorationIncidentに委譲する
        /// </summary>
        /// <param name="fromState">現在の状態</param>
        /// <param name="action">アクション</param>
        /// <returns>次の状態。nullの場合はインシデント終了</returns>
        public IncidentState TransitionState(IncidentState fromState, IncidentAction action)
        {
            if (fromState == null || action == null)
            {
                Debug.LogWarning("[IncidentProcess] fromState or action is null");
                return null;
            }

            if (Incident is ExplorationIncident explorationIncident)
            {
                // ExplorationIncidentのCalculateNextStateメソッドを使用（IncidentProcessを渡す）
                return explorationIncident.CalculateNextState(this, action.ActionId);
            }

            Debug.LogWarning($"[IncidentProcess] Unsupported incident type: {Incident?.GetType().Name}");
            return null;
        }

        /// <summary>
        /// 状態を遷移させる（TransitionStateを呼び出し、CurrentStateを更新）
        /// </summary>
        /// <param name="action">アクション</param>
        /// <returns>遷移後の状態。nullの場合はインシデント終了</returns>
        public IncidentState ApplyAction(IncidentAction action)
        {
            if (action == null)
            {
                Debug.LogWarning("[IncidentProcess] action is null");
                return null;
            }

            IncidentState nextState = TransitionState(CurrentState, action);
            if (nextState != null)
            {
                CurrentState = nextState;
                StateHistory.Add(nextState);
            }

            return nextState;
        }

        /// <summary>
        /// 現在が戦闘状態かチェック
        /// </summary>
        /// <returns>戦闘状態の場合はtrue</returns>
        public bool IsBattleState()
        {
            return CurrentState is BattleIncidentState;
        }
    }
}
