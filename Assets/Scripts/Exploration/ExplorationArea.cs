using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 探索エリアの抽象基底クラス
    /// 部員の割り当て、毎週の確率判定によるイベント発行を管理する
    /// </summary>
    public abstract class ExplorationArea : MonoBehaviour, IExplorationAreaAccessor
    {
        [SerializeField]
        private int maxMemberCapacity = 3;

        [SerializeField]
        private List<ClubMember> assignedMembers = new List<ClubMember>();

        [SerializeField]
        private string description = "";

        /// <summary>
        /// 割り当てられる部員の最大数
        /// </summary>
        public int MaxMemberCapacity
        {
            get { return maxMemberCapacity; }
            set { maxMemberCapacity = Mathf.Max(0, value); }
        }

        /// <summary>
        /// エリアの名称
        /// </summary>
        public string AreaName => gameObject.name;

        /// <summary>
        /// エリアの説明文
        /// </summary>
        public string Description => description;

        /// <summary>
        /// 現在割り当てられている部員の数
        /// </summary>
        public int AssignedMemberCount => assignedMembers.Count;

        /// <summary>
        /// 部員を追加できるかどうか
        /// </summary>
        public bool CanAssignMember => assignedMembers.Count < maxMemberCapacity;

        private GameTimeManager timeManager;
        private IncidentManager incidentManager;

        /// <summary>
        /// 毎週のイベント発生確率（0.0～1.0）
        /// 派生クラスでオーバーライド可能
        /// </summary>
        protected virtual float BaseEventProbability => 0.1f;

        protected virtual void Awake()
        {
            timeManager = GameTimeManager.Instance;
            incidentManager = IncidentManager.Instance;
        }

        protected virtual void OnEnable()
        {
            if (timeManager != null)
            {
                timeManager.OnWeekChanged += OnWeekChanged;
            }
        }

        protected virtual void OnDisable()
        {
            if (timeManager != null)
            {
                timeManager.OnWeekChanged -= OnWeekChanged;
            }
        }

        /// <summary>
        /// 週が変わったときに呼ばれる
        /// 割り当てられている部員に基づいて確率判定を行い、イベントを発行する
        /// </summary>
        private void OnWeekChanged(int year, int month, int week)
        {
            if (assignedMembers.Count == 0)
            {
                return; // 部員が割り当てられていない場合は何もしない
            }

            // 確率判定
            float probability = CalculateEventProbability(year, month, week);
            if (UnityEngine.Random.Range(0f, 1f) < probability)
            {
                // イベントを発行
                ExplorationIncident incident = CreateWeeklyEvent(year, month, week);
                if (incident != null)
                {
                    // アクセサーを設定
                    incident.SetAreaAccessor(this);

                    // IncidentManagerに直接イベントを発火（既存システムとの統合）
                    if (incidentManager != null)
                    {
                        incidentManager.TriggerIncidentDirectly(incident, year, month, week);
                    }
                }
            }
        }

        /// <summary>
        /// イベント発生確率を計算
        /// 割り当てられている部員のステータスに基づいて確率を調整
        /// </summary>
        /// <param name="year">現在の年</param>
        /// <param name="month">現在の月</param>
        /// <param name="week">現在の週</param>
        /// <returns>イベント発生確率（0.0～1.0）</returns>
        protected virtual float CalculateEventProbability(int year, int month, int week)
        {
            if (assignedMembers.Count == 0)
            {
                return 0f;
            }

            // 基本確率
            float probability = BaseEventProbability;

            // 部員のステータスに基づいて確率を調整
            // 例：部員の平均レベルが高いほど確率が上がる
            float averageLevel = (float)assignedMembers.Average(m => m.Level);
            float levelBonus = (averageLevel - 10f) * 0.01f; // レベル10を基準に±1%ずつ

            // 部員の数に基づいて確率を調整
            float memberCountBonus = (assignedMembers.Count - 1) * 0.05f; // 1人を基準に+5%ずつ

            probability += levelBonus + memberCountBonus;

            // 確率を0.0～1.0の範囲に制限
            return Mathf.Clamp01(probability);
        }

        /// <summary>
        /// 毎週のイベントを作成
        /// 派生クラスで実装して、エリア固有のイベントを返す
        /// </summary>
        /// <param name="year">現在の年</param>
        /// <param name="month">現在の月</param>
        /// <param name="week">現在の週</param>
        /// <returns>作成されたイベント。イベントが発生しない場合はnull</returns>
        protected abstract ExplorationIncident CreateWeeklyEvent(int year, int month, int week);

        #region IExplorationAreaAccessor実装

        /// <summary>
        /// 割り当てられている部員のリストを取得（読み取り専用）
        /// </summary>
        public IReadOnlyList<ClubMember> GetAssignedMembers()
        {
            return assignedMembers.AsReadOnly();
        }

        /// <summary>
        /// 部員を割り当てる
        /// 派生クラスで実装して、エリア固有の割り当てロジックを追加可能
        /// </summary>
        /// <param name="member">割り当てる部員</param>
        /// <returns>割り当てに成功した場合はtrue</returns>
        public virtual bool AssignMember(ClubMember member)
        {
            if (member == null)
            {
                Debug.LogWarning("[ExplorationArea] 部員がnullです。");
                return false;
            }

            if (assignedMembers.Contains(member))
            {
                Debug.LogWarning($"[ExplorationArea] 部員 {member.FullName} は既に割り当てられています。");
                return false;
            }

            if (assignedMembers.Count >= maxMemberCapacity)
            {
                Debug.LogWarning($"[ExplorationArea] 最大割り当て数 {maxMemberCapacity} に達しています。");
                return false;
            }

            assignedMembers.Add(member);
            OnMemberAssigned(member);
            return true;
        }

        /// <summary>
        /// 部員の割り当てを解除する
        /// 派生クラスで実装して、エリア固有の解除ロジックを追加可能
        /// </summary>
        /// <param name="member">解除する部員</param>
        /// <returns>解除に成功した場合はtrue</returns>
        public virtual bool UnassignMember(ClubMember member)
        {
            if (member == null)
            {
                Debug.LogWarning("[ExplorationArea] 部員がnullです。");
                return false;
            }

            if (!assignedMembers.Remove(member))
            {
                Debug.LogWarning($"[ExplorationArea] 部員 {member.FullName} は割り当てられていません。");
                return false;
            }

            OnMemberUnassigned(member);
            return true;
        }

        #endregion

        #region 派生クラス用のフックメソッド

        /// <summary>
        /// 部員が割り当てられたときに呼ばれる
        /// 派生クラスでオーバーライドしてカスタム処理を実装可能
        /// </summary>
        /// <param name="member">割り当てられた部員</param>
        protected virtual void OnMemberAssigned(ClubMember member)
        {
            // デフォルトは空実装
        }

        /// <summary>
        /// 部員の割り当てが解除されたときに呼ばれる
        /// 派生クラスでオーバーライドしてカスタム処理を実装可能
        /// </summary>
        /// <param name="member">解除された部員</param>
        protected virtual void OnMemberUnassigned(ClubMember member)
        {
            // デフォルトは空実装
        }

        #endregion
    }
}
