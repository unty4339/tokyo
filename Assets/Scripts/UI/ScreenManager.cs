using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 各画面のCanvasを管理し、画面切り替えを制御する
    /// </summary>
    public class ScreenManager : MonoBehaviour
    {
        private static ScreenManager _instance;
        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ScreenManager");
                    _instance = go.AddComponent<ScreenManager>();
                }
                return _instance;
            }
        }

        [Header("Screen Canvases")]
        [SerializeField] private Canvas clubPolicyCanvas;
        [SerializeField] private Canvas clubMemberListCanvas;
        [SerializeField] private Canvas itemCanvas;
        [SerializeField] private Canvas researchCanvas;
        [SerializeField] private Canvas achievementCanvas;
        [SerializeField] private Canvas systemCanvas;

        private Dictionary<ScreenType, Canvas> screenCanvases = new Dictionary<ScreenType, Canvas>();
        private ScreenType currentScreen = ScreenType.Home;

        public enum ScreenType
        {
            Home,
            ClubPolicy,
            MemberList,
            Item,
            Research,
            Achievement,
            System
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializeScreenCanvases();
        }

        /// <summary>
        /// 画面用Canvasを初期化
        /// </summary>
        private void InitializeScreenCanvases()
        {
            screenCanvases.Clear();

            if (clubPolicyCanvas != null)
            {
                screenCanvases[ScreenType.ClubPolicy] = clubPolicyCanvas;
            }

            if (clubMemberListCanvas != null)
            {
                screenCanvases[ScreenType.MemberList] = clubMemberListCanvas;
            }

            if (itemCanvas != null)
            {
                screenCanvases[ScreenType.Item] = itemCanvas;
            }

            if (researchCanvas != null)
            {
                screenCanvases[ScreenType.Research] = researchCanvas;
            }

            if (achievementCanvas != null)
            {
                screenCanvases[ScreenType.Achievement] = achievementCanvas;
            }

            if (systemCanvas != null)
            {
                screenCanvases[ScreenType.System] = systemCanvas;
            }

            // 初期状態ではすべて非表示
            SwitchToScreen(ScreenType.Home);
        }

        /// <summary>
        /// 画面を切り替える
        /// </summary>
        public void SwitchToScreen(ScreenType screenType)
        {
            currentScreen = screenType;

            // すべてのCanvasを非表示にする
            foreach (var canvas in screenCanvases.Values)
            {
                if (canvas != null)
                {
                    canvas.gameObject.SetActive(false);
                }
            }

            // ホーム画面の場合は何も表示しない
            if (screenType == ScreenType.Home)
            {
                return;
            }

            // 選択された画面のCanvasを表示
            if (screenCanvases.TryGetValue(screenType, out Canvas targetCanvas))
            {
                if (targetCanvas != null)
                {
                    targetCanvas.gameObject.SetActive(true);

                    // MemberList画面の場合は、ClubMemberListUIのウィンドウを開く
                    if (screenType == ScreenType.MemberList)
                    {
                        ClubMemberListUI listUI = targetCanvas.GetComponentInChildren<ClubMemberListUI>(true);
                        if (listUI != null)
                        {
                            listUI.OpenWindow();
                        }
                        else
                        {
                            Debug.LogWarning("ClubMemberListUI component not found in MemberList canvas.");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Screen canvas for {screenType} not found.");
            }
        }

        /// <summary>
        /// 現在の画面を取得
        /// </summary>
        public ScreenType GetCurrentScreen()
        {
            return currentScreen;
        }

        /// <summary>
        /// Canvasを登録する（外部から設定用）
        /// </summary>
        public void RegisterCanvas(ScreenType screenType, Canvas canvas)
        {
            if (canvas == null)
            {
                return;
            }

            screenCanvases[screenType] = canvas;

            // フィールドにも設定（Inspector表示用）
            switch (screenType)
            {
                case ScreenType.ClubPolicy:
                    clubPolicyCanvas = canvas;
                    break;
                case ScreenType.MemberList:
                    clubMemberListCanvas = canvas;
                    break;
                case ScreenType.Item:
                    itemCanvas = canvas;
                    break;
                case ScreenType.Research:
                    researchCanvas = canvas;
                    break;
                case ScreenType.Achievement:
                    achievementCanvas = canvas;
                    break;
                case ScreenType.System:
                    systemCanvas = canvas;
                    break;
            }
        }
    }
}

