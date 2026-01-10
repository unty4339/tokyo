using System;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 入学インシデント
    /// 毎年4月1週目に発生し、既存部員の学年を上げ、1年生を追加する
    /// </summary>
    public class EnrollmentIncident : Incident
    {
        public override string Id => "enrollment";

        public override bool IsMandatory => true;

        public override int? TimeLimitWeeks => null;

        public override bool CheckCondition(int year, int month, int week)
        {
            // 4月1週目に発生
            return month == 4 && week == 1;
        }

        public override string GetWindowPrefabAddress()
        {
            // 動的作成を使用
            return null;
        }

        public override GameObject GetWindowPrefab()
        {
            // AddressableManagerで読み込む場合
            string address = GetWindowPrefabAddress();
            if (!string.IsNullOrEmpty(address))
            {
                GameObject prefab = LoadPrefabFromAddressable(address);
                if (prefab != null)
                {
                    return prefab;
                }
            }

            // 動的作成を使用
            IncidentWindowOption[] options = new IncidentWindowOption[]
            {
                new IncidentWindowOption("閉じる", (process) =>
                {
                    // インシデントを解決
                    if (process != null && IncidentManager.Instance != null)
                    {
                        IncidentManager.Instance.ResolveIncident(process);
                    }
                })
            };

            return IncidentWindowBuilder.CreateWindow(
                "入学",
                null,
                "部員が入った",
                options
            );
        }

        public override void OnResolve(IncidentProcess process)
        {
            var memberManager = ClubMemberManager.Instance;
            if (memberManager == null)
            {
                Debug.LogWarning("[EnrollmentIncident] ClubMemberManagerが見つかりません");
                return;
            }

            // 既存部員の学年を1つ上げる
            memberManager.PromoteAllMembers();

            // 1年生を3-5人ランダムで追加
            System.Random random = new System.Random();
            int newMemberCount = random.Next(3, 6); // 3-5人

            for (int i = 0; i < newMemberCount; i++)
            {
                int memberNumber = memberManager.GetMemberCount() + 1;
                string lastName = "新入";
                string firstName = $"{memberNumber}号";

                var newMember = DummyDataFactory.CreateDefaultClubMember(
                    Grade.FirstYear,
                    level: 10,
                    lastName: lastName,
                    firstName: firstName
                );

                memberManager.AddMember(newMember);
            }

            Debug.Log($"[EnrollmentIncident] 1年生を{newMemberCount}人追加しました。");
        }
    }
}

