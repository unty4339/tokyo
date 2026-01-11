using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 卒業インシデント
    /// 毎年3月1週目に発生し、3年生の部員を全て削除する
    /// </summary>
    public class GraduationIncident : Incident
    {
        public override string Id => "graduation";

        public override bool IsMandatory => true;

        public override int? TimeLimitWeeks => null;

        public override bool CheckCondition(int year, int month, int week)
        {
            // 3月1週目に発生
            return month == 3 && week == 1;
        }

        public override string GetWindowPrefabAddress()
        {
            // 動的作成を使用
            return null;
        }

        public override GameObject GetWindowPrefab()
        {
            // このメソッドは使用されない（IncidentStateベースのシステムを使用）
            return null;
        }

        /// <summary>
        /// 初期状態を取得（IncidentManagerから呼ばれる）
        /// </summary>
        public IncidentState GetInitialState()
        {
            return new TextIncidentState("graduation", "部員が卒業した", IncidentUrgency.Immediate)
            {
                NextStateId = "end"
            };
        }

        public override void OnResolve(IncidentState state)
        {
            var memberManager = ClubMemberManager.Instance;
            if (memberManager == null)
            {
                Debug.LogWarning("[GraduationIncident] ClubMemberManagerが見つかりません");
                return;
            }

            // 3年生の部員を全て取得
            List<ClubMember> thirdYearMembers = memberManager.GetMembersByGrade(Grade.ThirdYear);

            // 3年生を全て削除
            foreach (var member in thirdYearMembers)
            {
                memberManager.RemoveMember(member);
            }

            Debug.Log($"[GraduationIncident] 3年生を{thirdYearMembers.Count}人削除しました。");
        }
    }
}

