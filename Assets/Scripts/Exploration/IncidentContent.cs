using System;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントウィンドウに表示するコンテンツのタイプ
    /// </summary>
    public enum IncidentContentType
    {
        /// <summary>通常のコンテンツ（テキスト＋選択肢）</summary>
        Normal,
        /// <summary>戦闘コンテンツ</summary>
        Battle
    }

    /// <summary>
    /// インシデントウィンドウに表示する内容をカプセル化
    /// ウィンドウの子になるオブジェクトについて責任を持つ
    /// 抽象基底クラスとして、サブクラスで具体的な実装を行う
    /// </summary>
    public abstract class IncidentContent
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 画像のパス（オプショナル、null可）
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// メッセージテキスト
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// コンテンツタイプ
        /// </summary>
        public IncidentContentType Type { get; set; }

        /// <summary>
        /// このコンテンツを作成したIncidentProcessへの参照
        /// </summary>
        public IncidentProcess Process { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IncidentContent()
        {
            Type = IncidentContentType.Normal;
        }

        /// <summary>
        /// 自身の子オブジェクト（画面に配置するボタン等）を一括で作成する機能
        /// </summary>
        /// <param name="parent">親のTransform</param>
        /// <returns>作成された子オブジェクトのルートTransform</returns>
        public abstract Transform CreateChildObjects(Transform parent);

        /// <summary>
        /// 自身を作成したIncidentProcessにIncidentActionクラスを作成して渡す機能
        /// </summary>
        /// <param name="action">選択されたアクション</param>
        /// <param name="process">IncidentProcess</param>
        public virtual void OnActionSelected(IncidentAction action, IncidentProcess process)
        {
            if (action == null)
            {
                Debug.LogWarning("[IncidentContent] action is null");
                return;
            }

            if (process == null)
            {
                Debug.LogWarning("[IncidentContent] process is null");
                return;
            }

            // Processにアクションを適用
            IncidentState nextState = process.ApplyAction(action);
            if (nextState == null)
            {
                // インシデント終了
                Debug.Log("[IncidentContent] Incident ended");
                
                // インシデントプロセスを削除しマネージャからも消す、ウィンドウも閉じる
                var incidentManager = IncidentManager.Instance;
                if (incidentManager != null)
                {
                    incidentManager.ResolveIncident(process);
                }
                else
                {
                    Debug.LogWarning("[IncidentContent] IncidentManager.Instance is null. Cannot resolve incident.");
                }
            }
            else
            {
                // 次の状態が設定されたので、新しいコンテンツを作成してウィンドウを更新する必要がある
                // ただし、これはIncidentWindowまたはIncidentUIの責任になる可能性がある
                Debug.Log($"[IncidentContent] State transitioned to: {nextState.GetStateId()}");
            }
        }
    }
}
