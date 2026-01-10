using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterBattleGame
{
    /// <summary>
    /// サンプル探索エリアの実装クラス
    /// 基本的な割り当て/解除ロジックを実装
    /// </summary>
    public class SampleExplorationArea : ExplorationArea, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // 色の定義
        private static readonly Color HoverColor = new Color(0xBE / 255f, 0xBB / 255f, 0xA9 / 255f, 1f); // #BEBBA9
        private static readonly Color SelectedColor = new Color(0xBE / 255f, 0xB4 / 255f, 0x79 / 255f, 1f); // #BEB479

        // レンダラーコンポーネント
        private SpriteRenderer spriteRenderer;
        private MeshRenderer meshRenderer;
        private Renderer currentRenderer;

        // 元の色を保存
        private Color originalColor;
        private bool hasOriginalColor = false;

        // 状態管理
        private bool isHovering = false;
        private bool isSelected = false;

        protected override void Awake()
        {
            base.Awake();

            // レンダラーを取得
            spriteRenderer = GetComponent<SpriteRenderer>();
            meshRenderer = GetComponent<MeshRenderer>();

            // どちらかのレンダラーを取得
            if (spriteRenderer != null)
            {
                currentRenderer = spriteRenderer;
                originalColor = spriteRenderer.color;
                hasOriginalColor = true;
            }
            else if (meshRenderer != null)
            {
                currentRenderer = meshRenderer;
                // MeshRendererの場合はマテリアルの色を取得
                if (meshRenderer.material != null && meshRenderer.material.HasProperty("_Color"))
                {
                    originalColor = meshRenderer.material.color;
                    hasOriginalColor = true;
                }
            }
            else
            {
                Debug.LogWarning($"[SampleExplorationArea] {gameObject.name} にSpriteRendererまたはMeshRendererが見つかりません。");
            }
        }

        /// <summary>
        /// 毎週のイベントを作成
        /// サンプル実装として、基本的な探索イベントを返す
        /// </summary>
        protected override ExplorationIncident CreateWeeklyEvent(int year, int month, int week)
        {
            // サンプルイベントを作成
            return new SampleExplorationIncident();
        }

        /// <summary>
        /// オブジェクトがクリックされたときに呼ばれる
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // HomeCanvasを取得
            var screenManager = ScreenManager.Instance;
            if (screenManager == null)
            {
                Debug.LogWarning("[SampleExplorationArea] ScreenManagerが見つかりません。");
                return;
            }

            Canvas homeCanvas = screenManager.HomeCanvas;
            if (homeCanvas == null)
            {
                Debug.LogWarning("[SampleExplorationArea] HomeCanvasが見つかりません。");
                return;
            }

            // エリア情報ウィンドウを作成
            AreaInfoWindowUI.CreateAreaInfoWindow(this, homeCanvas);
            
            // 選択状態に設定
            SetSelected(true);
        }

        /// <summary>
        /// マウスがオブジェクトに入ったときに呼ばれる
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
            UpdateColor();
        }

        /// <summary>
        /// マウスがオブジェクトから出たときに呼ばれる
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            UpdateColor();
        }

        /// <summary>
        /// 選択状態を設定（ウィンドウが開いている間）
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateColor();
        }

        /// <summary>
        /// 色を更新する
        /// 優先順位: 選択状態 > ホバー状態 > 通常状態
        /// </summary>
        private void UpdateColor()
        {
            if (currentRenderer == null || !hasOriginalColor)
            {
                return;
            }

            Color targetColor;

            if (isSelected)
            {
                // 選択状態（ウィンドウ表示中）
                targetColor = SelectedColor;
            }
            else if (isHovering)
            {
                // ホバー状態
                targetColor = HoverColor;
            }
            else
            {
                // 通常状態
                targetColor = originalColor;
            }

            // レンダラーの種類に応じて色を設定
            if (spriteRenderer != null)
            {
                spriteRenderer.color = targetColor;
            }
            else if (meshRenderer != null && meshRenderer.material != null)
            {
                if (meshRenderer.material.HasProperty("_Color"))
                {
                    meshRenderer.material.color = targetColor;
                }
            }
        }
    }
}
