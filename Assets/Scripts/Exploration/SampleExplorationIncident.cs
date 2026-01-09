using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// サンプル探索イベント
    /// 探索エリアから発行されるイベントのサンプル実装
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
            // 動的作成を使用
            IncidentWindowOption[] options = new IncidentWindowOption[]
            {
                new IncidentWindowOption("閉じる", (instance) =>
                {
                    // インシデントを解決
                    if (instance != null && IncidentManager.Instance != null)
                    {
                        IncidentManager.Instance.ResolveIncident(instance);
                    }
                })
            };

            string message = "探索中に何かを見つけた！";
            if (AreaAccessor != null)
            {
                var members = AreaAccessor.GetAssignedMembers();
                if (members.Count > 0)
                {
                    message += $"\n{members[0].FullName}が発見した。";
                }
            }

            return IncidentWindowBuilder.CreateWindow(
                "探索イベント",
                null,
                message,
                options
            );
        }

        public override void OnResolve(IncidentInstance instance)
        {
            base.OnResolve(instance);

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
