using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// エリア用のオブジェクトを管理するクラス
    /// 子オブジェクトにSampleExplorationAreaをアタッチし、マテリアルを適用する
    /// </summary>
    public class AreaManager : MonoBehaviour
    {
        [Header("マテリアル設定")]
        [SerializeField]
        private Material areaMaterial;

        private void Awake()
        {
            // 全ての子オブジェクトを取得（自分自身は除外）
            Transform[] childTransforms = GetComponentsInChildren<Transform>();
            
            foreach (Transform childTransform in childTransforms)
            {
                // 自分自身はスキップ
                if (childTransform == transform)
                {
                    continue;
                }

                GameObject childObject = childTransform.gameObject;

                // SampleExplorationAreaのアタッチ処理
                SampleExplorationArea existingArea = childObject.GetComponent<SampleExplorationArea>();
                if (existingArea != null)
                {
                    Debug.LogWarning($"[AreaManager] {childObject.name} には既にSampleExplorationAreaがアタッチされています。スキップします。");
                }
                else
                {
                    childObject.AddComponent<SampleExplorationArea>();
                    Debug.Log($"[AreaManager] {childObject.name} にSampleExplorationAreaをアタッチしました。");
                }

                // マテリアルの適用処理
                SpriteRenderer spriteRenderer = childObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    if (areaMaterial != null)
                    {
                        spriteRenderer.material = areaMaterial;
                        Debug.Log($"[AreaManager] {childObject.name} にマテリアルを適用しました。");
                    }
                    else
                    {
                        Debug.LogWarning($"[AreaManager] マテリアルが設定されていません。{childObject.name} にはマテリアルを適用しませんでした。");
                    }
                }
                else
                {
                    Debug.LogWarning($"[AreaManager] {childObject.name} にSpriteRendererがアタッチされていません。マテリアルを適用できませんでした。");
                }
            }
        }
    }
}
