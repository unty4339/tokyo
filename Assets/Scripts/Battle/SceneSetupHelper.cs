#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if USE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace MonsterBattleGame
{
    /// <summary>
    /// シーンセットアップヘルパー
    /// MenuItemでUIオブジェクトを自動生成
    /// </summary>
    public class SceneSetupHelper : MonoBehaviour
    {
        /// <summary>
        /// フォントを取得（日本語対応）
        /// </summary>
        private static Font GetFont()
        {
#if UNITY_EDITOR
            // Editorの場合はAssetDatabaseを使用してフォントを読み込む
            Font font = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/NotoSansJP-Medium.ttf");
            if (font == null)
            {
                // 見つからない場合は、Resourcesから読み込む
                font = Resources.Load<Font>("Fonts/NotoSansJP-Medium");
            }
            if (font == null)
            {
                // それでも見つからない場合は、LegacyRuntime.ttfを使用
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return font;
#else
            // 実行時はResourcesから読み込む
            Font font = Resources.Load<Font>("Fonts/NotoSansJP-Medium");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return font;
#endif
        }

#if UNITY_EDITOR
        [MenuItem("Setup/Create Battle UI", false, 10)]
        public static void CreateBattleUI()
        {
            // Canvasの取得または作成
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // EventSystemの取得または作成
            EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                // EventSystemが存在しない場合は作成
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<EventSystem>();
                
                // InputSystemUIInputModuleを追加
#if USE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
                if (eventSystemObj.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystemObj.AddComponent<InputSystemUIInputModule>();
                }
#endif
            }
            else
            {
                // 既存のEventSystemにInputSystemUIInputModuleがあるか確認
#if USE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
                if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }
#endif
            }

            // 戦闘ウィンドウの作成
            GameObject battleWindow = new GameObject("BattleWindow");
            battleWindow.transform.SetParent(canvas.transform, false);
            RectTransform battleWindowRect = battleWindow.AddComponent<RectTransform>();
            battleWindowRect.anchorMin = new Vector2(0.5f, 0.5f);
            battleWindowRect.anchorMax = new Vector2(0.5f, 0.5f);
            battleWindowRect.sizeDelta = new Vector2(800, 600);
            battleWindowRect.anchoredPosition = Vector2.zero;

            Image battleWindowImage = battleWindow.AddComponent<Image>();
            battleWindowImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

            // タイトルテキスト
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(battleWindow.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "戦闘";
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 40);
            titleRect.anchoredPosition = new Vector2(0, -20);

            // ターン表示テキスト
            GameObject turnObj = new GameObject("TurnText");
            turnObj.transform.SetParent(battleWindow.transform, false);
            Text turnText = turnObj.AddComponent<Text>();
            turnText.text = "ターン: 0 / 10";
            turnText.font = GetFont();
            turnText.fontSize = 18;
            turnText.alignment = TextAnchor.MiddleLeft;
            turnText.color = Color.white;

            RectTransform turnRect = turnObj.GetComponent<RectTransform>();
            turnRect.anchorMin = new Vector2(0, 1);
            turnRect.anchorMax = new Vector2(1, 1);
            turnRect.sizeDelta = new Vector2(0, 30);
            turnRect.anchoredPosition = new Vector2(10, -60);

            // プレイヤーチームエリア
            GameObject playerArea = new GameObject("PlayerTeamArea");
            playerArea.transform.SetParent(battleWindow.transform, false);
            RectTransform playerRect = playerArea.AddComponent<RectTransform>();
            playerRect.anchorMin = new Vector2(0, 0.5f);
            playerRect.anchorMax = new Vector2(0.5f, 1);
            playerRect.sizeDelta = Vector2.zero;
            playerRect.offsetMin = new Vector2(10, 10);
            playerRect.offsetMax = new Vector2(-10, -10);

            VerticalLayoutGroup playerLayout = playerArea.AddComponent<VerticalLayoutGroup>();
            playerLayout.spacing = 10;
            playerLayout.padding = new RectOffset(10, 10, 10, 10);
            playerLayout.childAlignment = TextAnchor.MiddleCenter;

            // 敵チームエリア
            GameObject enemyArea = new GameObject("EnemyTeamArea");
            enemyArea.transform.SetParent(battleWindow.transform, false);
            RectTransform enemyRect = enemyArea.AddComponent<RectTransform>();
            enemyRect.anchorMin = new Vector2(0.5f, 0.5f);
            enemyRect.anchorMax = new Vector2(1, 1);
            enemyRect.sizeDelta = Vector2.zero;
            enemyRect.offsetMin = new Vector2(10, 10);
            enemyRect.offsetMax = new Vector2(-10, -10);

            VerticalLayoutGroup enemyLayout = enemyArea.AddComponent<VerticalLayoutGroup>();
            enemyLayout.spacing = 10;
            enemyLayout.padding = new RectOffset(10, 10, 10, 10);
            enemyLayout.childAlignment = TextAnchor.MiddleCenter;

            // ログ表示エリア
            GameObject logArea = new GameObject("LogArea");
            logArea.transform.SetParent(battleWindow.transform, false);
            RectTransform logRect = logArea.AddComponent<RectTransform>();
            logRect.anchorMin = new Vector2(0, 0);
            logRect.anchorMax = new Vector2(1, 0.5f);
            logRect.sizeDelta = Vector2.zero;
            logRect.offsetMin = new Vector2(10, 10);
            logRect.offsetMax = new Vector2(-10, -10);

            Image logBackground = logArea.AddComponent<Image>();
            logBackground.color = new Color(0, 0, 0, 0.5f);

            // Maskコンポーネントを追加してviewportとして機能させる
            Mask viewportMask = logArea.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            // Contentオブジェクトを作成（スクロール可能なコンテンツ領域）
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(logArea.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            
            // ContentのRectTransform設定（上部アンカー、動的な高さ調整が可能に）
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 0);
            contentRect.anchoredPosition = Vector2.zero;

            // ContentLayoutGroupを追加してテキストの高さを自動調整
            VerticalLayoutGroup contentLayout = contentObj.AddComponent<VerticalLayoutGroup>();
            contentLayout.childAlignment = TextAnchor.UpperLeft;
            contentLayout.childControlHeight = false;
            contentLayout.childControlWidth = true;
            contentLayout.childForceExpandHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.spacing = 0;
            contentLayout.padding = new RectOffset(10, 10, 10, 10);

            // ContentSizeFitterを追加してコンテンツの高さを自動調整
            ContentSizeFitter contentSizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            // ログテキストオブジェクトを作成（contentの子として）
            GameObject logTextObj = new GameObject("LogText");
            logTextObj.transform.SetParent(contentObj.transform, false);
            Text logText = logTextObj.AddComponent<Text>();
            logText.text = "";
            logText.font = GetFont();
            logText.fontSize = 14;
            logText.alignment = TextAnchor.UpperLeft;
            logText.color = Color.white;

            RectTransform logTextRect = logTextObj.GetComponent<RectTransform>();
            logTextRect.anchorMin = new Vector2(0, 1);
            logTextRect.anchorMax = new Vector2(1, 1);
            logTextRect.pivot = new Vector2(0, 1);
            logTextRect.sizeDelta = new Vector2(0, 0);
            logTextRect.anchoredPosition = Vector2.zero;

            // TextのContentSizeFitterを追加
            ContentSizeFitter textSizeFitter = logTextObj.AddComponent<ContentSizeFitter>();
            textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            textSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            // スクロールビューを設定
            ScrollRect scrollRect = logArea.AddComponent<ScrollRect>();
            scrollRect.content = contentRect;
            scrollRect.viewport = logRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.verticalScrollbar = null;
            scrollRect.horizontalScrollbar = null;

            // スキップボタン
            GameObject skipButtonObj = new GameObject("SkipButton");
            skipButtonObj.transform.SetParent(battleWindow.transform, false);
            Image skipButtonImage = skipButtonObj.AddComponent<Image>();
            skipButtonImage.color = new Color(0.3f, 0.3f, 0.7f, 1f);

            Button skipButton = skipButtonObj.AddComponent<Button>();

            RectTransform skipRect = skipButtonObj.GetComponent<RectTransform>();
            skipRect.anchorMin = new Vector2(0.5f, 0);
            skipRect.anchorMax = new Vector2(0.5f, 0);
            skipRect.sizeDelta = new Vector2(150, 40);
            skipRect.anchoredPosition = new Vector2(0, 50);

            GameObject skipTextObj = new GameObject("Text");
            skipTextObj.transform.SetParent(skipButtonObj.transform, false);
            Text skipText = skipTextObj.AddComponent<Text>();
            skipText.text = "スキップ";
            skipText.font = GetFont();
            skipText.fontSize = 18;
            skipText.alignment = TextAnchor.MiddleCenter;
            skipText.color = Color.white;

            RectTransform skipTextRect = skipTextObj.GetComponent<RectTransform>();
            skipTextRect.anchorMin = Vector2.zero;
            skipTextRect.anchorMax = Vector2.one;
            skipTextRect.sizeDelta = Vector2.zero;

            // BattleUIコンポーネントを追加
            BattleUI battleUI = battleWindow.AddComponent<BattleUI>();
            
            // リフレクションでフィールドを設定（簡易的な方法）
            var battleUIType = typeof(BattleUI);
            var battleWindowField = battleUIType.GetField("battleWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            battleWindowField?.SetValue(battleUI, battleWindow);

            var turnTextField = battleUIType.GetField("turnText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            turnTextField?.SetValue(battleUI, turnText);

            var logTextField = battleUIType.GetField("logText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            logTextField?.SetValue(battleUI, logText);

            var logScrollRectField = battleUIType.GetField("logScrollRect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            logScrollRectField?.SetValue(battleUI, scrollRect);

            var skipButtonField = battleUIType.GetField("skipButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            skipButtonField?.SetValue(battleUI, skipButton);

            var playerAreaField = battleUIType.GetField("playerTeamArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            playerAreaField?.SetValue(battleUI, playerArea.transform);

            var enemyAreaField = battleUIType.GetField("enemyTeamArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            enemyAreaField?.SetValue(battleUI, enemyArea.transform);

            Debug.Log("Battle UI created successfully!");
        }

        [MenuItem("Setup/Create Test Controller", false, 11)]
        public static void CreateTestController()
        {
            // Canvasの取得
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("Canvas not found. Please create Battle UI first.");
                return;
            }

            // テストボタンの作成
            GameObject buttonObj = new GameObject("TestBattleButton");
            buttonObj.transform.SetParent(canvas.transform, false);

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);

            Button button = buttonObj.AddComponent<Button>();

            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 1);
            buttonRect.anchorMax = new Vector2(0, 1);
            buttonRect.sizeDelta = new Vector2(150, 50);
            buttonRect.anchoredPosition = new Vector2(100, -50);

            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            Text buttonText = buttonTextObj.AddComponent<Text>();
            buttonText.text = "戦闘テスト";
            buttonText.font = GetFont();
            buttonText.fontSize = 18;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;

            RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.sizeDelta = Vector2.zero;

            // BattleTestControllerコンポーネントを追加
            BattleTestController controller = buttonObj.AddComponent<BattleTestController>();

            // BattleUIを検索
            BattleUI battleUI = FindFirstObjectByType<BattleUI>();
            if (battleUI == null)
            {
                Debug.LogWarning("BattleUI not found. Please create Battle UI first.");
            }

            // リフレクションでフィールドを設定
            var controllerType = typeof(BattleTestController);
            var buttonField = controllerType.GetField("testButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            buttonField?.SetValue(controller, button);

            var battleUIField = controllerType.GetField("battleUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            battleUIField?.SetValue(controller, battleUI);

            Debug.Log("Test Controller created successfully!");
        }

        [MenuItem("Setup/Add Dummy Members Button", false, 30)]
        public static void CreateAddDummyMembersButton()
        {
            // Canvasの取得
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("Canvas not found. Please create Canvas first.");
                return;
            }

            // ダミー部員追加ボタンの作成
            GameObject buttonObj = new GameObject("AddDummyMembersButton");
            buttonObj.transform.SetParent(canvas.transform, false);

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.4f, 0.4f, 0.8f, 1f);

            Button button = buttonObj.AddComponent<Button>();

            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 1);
            buttonRect.anchorMax = new Vector2(0, 1);
            buttonRect.sizeDelta = new Vector2(200, 50);
            buttonRect.anchoredPosition = new Vector2(100, -100);

            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            Text buttonText = buttonTextObj.AddComponent<Text>();
            buttonText.text = "ダミー部員追加";
            buttonText.font = GetFont();
            buttonText.fontSize = 18;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;

            RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.sizeDelta = Vector2.zero;

            // AddDummyMemberControllerコンポーネントを追加
            AddDummyMemberController controller = buttonObj.AddComponent<AddDummyMemberController>();

            // リフレクションでフィールドを設定
            var controllerType = typeof(AddDummyMemberController);
            var buttonField = controllerType.GetField("addButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            buttonField?.SetValue(controller, button);

            Debug.Log("Add Dummy Members Button created successfully!");
        }

        [MenuItem("Setup/Create Club Member List UI", false, 31)]
        public static void CreateClubMemberListUI()
        {
            // 別キャンバスを作成
            GameObject canvasObj = new GameObject("ClubMemberListCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1; // 戦闘ウィンドウより前面に表示
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // EventSystemの取得または作成
            EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<EventSystem>();
                
#if USE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
                if (eventSystemObj.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystemObj.AddComponent<InputSystemUIInputModule>();
                }
#endif
            }

            // 部員一覧ウィンドウの作成
            GameObject listWindow = new GameObject("ClubMemberListWindow");
            listWindow.transform.SetParent(canvas.transform, false);
            RectTransform listWindowRect = listWindow.AddComponent<RectTransform>();
            listWindowRect.anchorMin = new Vector2(0.5f, 0.5f);
            listWindowRect.anchorMax = new Vector2(0.5f, 0.5f);
            listWindowRect.sizeDelta = new Vector2(900, 700);
            listWindowRect.anchoredPosition = Vector2.zero;

            Image listWindowImage = listWindow.AddComponent<Image>();
            listWindowImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

            // タイトルテキスト
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(listWindow.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "部員一覧";
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 40);
            titleRect.anchoredPosition = new Vector2(0, -20);

            // スクロールビューエリア（Viewport）
            GameObject scrollArea = new GameObject("ScrollArea");
            scrollArea.transform.SetParent(listWindow.transform, false);
            RectTransform scrollRect = scrollArea.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.sizeDelta = Vector2.zero;
            scrollRect.offsetMin = new Vector2(10, 60);
            scrollRect.offsetMax = new Vector2(-10, -60);

            Image scrollAreaImage = scrollArea.AddComponent<Image>();
            scrollAreaImage.color = new Color(0, 0, 0, 0.3f);

            Mask viewportMask = scrollArea.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            // グリッドエリア（Content）
            GameObject gridArea = new GameObject("MemberGridArea");
            gridArea.transform.SetParent(scrollArea.transform, false);
            RectTransform gridRect = gridArea.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0, 1);
            gridRect.anchorMax = new Vector2(1, 1);
            gridRect.pivot = new Vector2(0.5f, 1);
            gridRect.sizeDelta = new Vector2(0, 0);
            gridRect.anchoredPosition = Vector2.zero;

            GridLayoutGroup gridLayout = gridArea.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(150, 100);
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 5;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.padding = new RectOffset(10, 10, 10, 10);

            ContentSizeFitter contentSizeFitter = gridArea.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            // ScrollRectコンポーネントを追加
            ScrollRect scrollRectComponent = scrollArea.AddComponent<ScrollRect>();
            scrollRectComponent.content = gridRect;
            scrollRectComponent.viewport = scrollRect;
            scrollRectComponent.horizontal = false;
            scrollRectComponent.vertical = true;
            scrollRectComponent.movementType = ScrollRect.MovementType.Elastic;
            scrollRectComponent.elasticity = 0.1f;
            scrollRectComponent.inertia = true;
            scrollRectComponent.decelerationRate = 0.135f;
            scrollRectComponent.scrollSensitivity = 1.0f;

            // 閉じるボタン
            GameObject closeButtonObj = new GameObject("CloseButton");
            closeButtonObj.transform.SetParent(listWindow.transform, false);
            Image closeButtonImage = closeButtonObj.AddComponent<Image>();
            closeButtonImage.color = new Color(0.5f, 0.2f, 0.2f, 1f);

            Button closeButton = closeButtonObj.AddComponent<Button>();

            RectTransform closeRect = closeButtonObj.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.5f, 0);
            closeRect.anchorMax = new Vector2(0.5f, 0);
            closeRect.sizeDelta = new Vector2(150, 40);
            closeRect.anchoredPosition = new Vector2(0, 20);

            GameObject closeTextObj = new GameObject("Text");
            closeTextObj.transform.SetParent(closeButtonObj.transform, false);
            Text closeText = closeTextObj.AddComponent<Text>();
            closeText.text = "閉じる";
            closeText.font = GetFont();
            closeText.fontSize = 18;
            closeText.alignment = TextAnchor.MiddleCenter;
            closeText.color = Color.white;

            RectTransform closeTextRect = closeTextObj.GetComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.sizeDelta = Vector2.zero;

            // ClubMemberListUIコンポーネントを追加
            ClubMemberListUI listUI = listWindow.AddComponent<ClubMemberListUI>();

            // リフレクションでフィールドを設定
            var listUIType = typeof(ClubMemberListUI);
            var listWindowField = listUIType.GetField("listWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            listWindowField?.SetValue(listUI, listWindow);

            var memberGridAreaField = listUIType.GetField("memberGridArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            memberGridAreaField?.SetValue(listUI, gridArea.transform);

            var closeButtonField = listUIType.GetField("closeButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            closeButtonField?.SetValue(listUI, closeButton);

            // 閉じるボタンでホーム画面に戻るように設定
            closeButton.onClick.AddListener(() =>
            {
                ScreenManager screenManager = ScreenManager.Instance;
                if (screenManager != null)
                {
                    screenManager.SwitchToScreen(ScreenManager.ScreenType.Home);
                }
            });

            // 初期状態では非表示
            canvas.gameObject.SetActive(false);

            // ScreenManagerに登録
            ScreenManager screenManagerInstance = ScreenManager.Instance;
            screenManagerInstance.RegisterCanvas(ScreenManager.ScreenType.MemberList, canvas);

            Debug.Log("Club Member List UI created successfully!");
        }

        [MenuItem("Setup/Create Menu Bar", false, 40)]
        public static void CreateMenuBar()
        {
            // MenuCanvasの作成
            GameObject menuCanvasObj = new GameObject("MenuCanvas");
            Canvas menuCanvas = menuCanvasObj.AddComponent<Canvas>();
            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            menuCanvas.sortingOrder = 10; // 他のCanvasより前面に表示
            menuCanvasObj.AddComponent<CanvasScaler>();
            menuCanvasObj.AddComponent<GraphicRaycaster>();

            // EventSystemの取得または作成
            EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<EventSystem>();
                
#if USE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
                if (eventSystemObj.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystemObj.AddComponent<InputSystemUIInputModule>();
                }
#endif
            }

            // メニューバーの背景
            GameObject menuBarObj = new GameObject("MenuBar");
            menuBarObj.transform.SetParent(menuCanvas.transform, false);
            RectTransform menuBarRect = menuBarObj.AddComponent<RectTransform>();
            menuBarRect.anchorMin = new Vector2(0, 0);
            menuBarRect.anchorMax = new Vector2(1, 0);
            menuBarRect.sizeDelta = new Vector2(0, 90);
            menuBarRect.anchoredPosition = new Vector2(0, 45);

            Image menuBarImage = menuBarObj.AddComponent<Image>();
            menuBarImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            // 水平レイアウトグループ
            HorizontalLayoutGroup menuBarLayout = menuBarObj.AddComponent<HorizontalLayoutGroup>();
            menuBarLayout.spacing = 10;
            menuBarLayout.padding = new RectOffset(10, 10, 10, 10);
            menuBarLayout.childAlignment = TextAnchor.MiddleLeft;
            menuBarLayout.childControlWidth = false;
            menuBarLayout.childControlHeight = true;
            menuBarLayout.childForceExpandWidth = false;
            menuBarLayout.childForceExpandHeight = true;

            // 1. 評判・資金表示エリア
            GameObject resourceAreaObj = new GameObject("ResourceArea");
            resourceAreaObj.transform.SetParent(menuBarObj.transform, false);
            RectTransform resourceAreaRect = resourceAreaObj.AddComponent<RectTransform>();
            resourceAreaRect.sizeDelta = new Vector2(160, 0);

            HorizontalLayoutGroup resourceLayout = resourceAreaObj.AddComponent<HorizontalLayoutGroup>();
            resourceLayout.spacing = 5;
            resourceLayout.padding = new RectOffset(10, 10, 0, 0);
            resourceLayout.childAlignment = TextAnchor.MiddleLeft;
            resourceLayout.childControlWidth = false;
            resourceLayout.childControlHeight = false;
            resourceLayout.childForceExpandWidth = false;
            resourceLayout.childForceExpandHeight = false;

            // 評判表示
            GameObject reputationObj = new GameObject("ReputationText");
            reputationObj.transform.SetParent(resourceAreaObj.transform, false);
            Text reputationText = reputationObj.AddComponent<Text>();
            reputationText.text = "評判: 0";
            reputationText.font = GetFont();
            reputationText.fontSize = 16;
            reputationText.alignment = TextAnchor.MiddleLeft;
            reputationText.color = Color.white;

            RectTransform reputationRect = reputationObj.GetComponent<RectTransform>();
            reputationRect.sizeDelta = new Vector2(60, 30);

            // 資金表示
            GameObject moneyObj = new GameObject("MoneyText");
            moneyObj.transform.SetParent(resourceAreaObj.transform, false);
            Text moneyText = moneyObj.AddComponent<Text>();
            moneyText.text = "資金: 0円";
            moneyText.font = GetFont();
            moneyText.fontSize = 16;
            moneyText.alignment = TextAnchor.MiddleLeft;
            moneyText.color = Color.white;

            RectTransform moneyRect = moneyObj.GetComponent<RectTransform>();
            moneyRect.sizeDelta = new Vector2(90, 30);

            // 2. 画面遷移ボタンエリア
            GameObject buttonAreaObj = new GameObject("ButtonArea");
            buttonAreaObj.transform.SetParent(menuBarObj.transform, false);
            RectTransform buttonAreaRect = buttonAreaObj.AddComponent<RectTransform>();
            buttonAreaRect.sizeDelta = new Vector2(600, 0);

            HorizontalLayoutGroup buttonAreaLayout = buttonAreaObj.AddComponent<HorizontalLayoutGroup>();
            buttonAreaLayout.spacing = 5;
            buttonAreaLayout.padding = new RectOffset(10, 10, 0, 0);
            buttonAreaLayout.childAlignment = TextAnchor.MiddleLeft;
            buttonAreaLayout.childControlWidth = false;
            buttonAreaLayout.childControlHeight = true;
            buttonAreaLayout.childForceExpandWidth = false;
            buttonAreaLayout.childForceExpandHeight = true;

            // 画面遷移ボタンを作成する関数
            Button CreateScreenButton(string buttonName, string buttonText, Vector2 size)
            {
                GameObject buttonObj = new GameObject(buttonName);
                buttonObj.transform.SetParent(buttonAreaObj.transform, false);
                Image buttonImage = buttonObj.AddComponent<Image>();
                buttonImage.color = new Color(0.3f, 0.3f, 0.5f, 1f);

                Button button = buttonObj.AddComponent<Button>();

                RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
                buttonRect.sizeDelta = size;

                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(buttonObj.transform, false);
                Text text = textObj.AddComponent<Text>();
                text.text = buttonText;
                text.font = GetFont();
                text.fontSize = 14;
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.white;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;

                return button;
            }

            Vector2 buttonSize = new Vector2(80, 40);

            Button homeButton = CreateScreenButton("HomeButton", "ホーム", buttonSize);
            Button clubPolicyButton = CreateScreenButton("ClubPolicyButton", "部活方針", buttonSize);
            Button memberListButton = CreateScreenButton("MemberListButton", "部員一覧", buttonSize);
            Button itemButton = CreateScreenButton("ItemButton", "アイテム", buttonSize);
            Button researchButton = CreateScreenButton("ResearchButton", "研究開発", buttonSize);
            Button achievementButton = CreateScreenButton("AchievementButton", "実績", buttonSize);
            Button systemButton = CreateScreenButton("SystemButton", "システム", buttonSize);

            // 3. 日付表示エリア
            GameObject dateAreaObj = new GameObject("DateArea");
            dateAreaObj.transform.SetParent(menuBarObj.transform, false);
            RectTransform dateAreaRect = dateAreaObj.AddComponent<RectTransform>();
            dateAreaRect.sizeDelta = new Vector2(60, 0);

            GameObject dateObj = new GameObject("DateText");
            dateObj.transform.SetParent(dateAreaObj.transform, false);
            Text dateText = dateObj.AddComponent<Text>();
            dateText.text = "1年目 1月 1週";
            dateText.font = GetFont();
            dateText.fontSize = 16;
            dateText.alignment = TextAnchor.MiddleCenter;
            dateText.color = Color.white;

            RectTransform dateRect = dateObj.GetComponent<RectTransform>();
            dateRect.anchorMin = Vector2.zero;
            dateRect.anchorMax = Vector2.one;
            dateRect.sizeDelta = Vector2.zero;
            dateRect.offsetMin = new Vector2(5, 0);
            dateRect.offsetMax = new Vector2(-5, 0);

            // 4. 時間経過スケール変更ボタンエリア
            GameObject timeScaleAreaObj = new GameObject("TimeScaleArea");
            timeScaleAreaObj.transform.SetParent(menuBarObj.transform, false);
            RectTransform timeScaleAreaRect = timeScaleAreaObj.AddComponent<RectTransform>();
            timeScaleAreaRect.sizeDelta = new Vector2(280, 0);

            HorizontalLayoutGroup timeScaleLayout = timeScaleAreaObj.AddComponent<HorizontalLayoutGroup>();
            timeScaleLayout.spacing = 5;
            timeScaleLayout.padding = new RectOffset(10, 10, 0, 0);
            timeScaleLayout.childAlignment = TextAnchor.MiddleLeft;
            timeScaleLayout.childControlWidth = false;
            timeScaleLayout.childControlHeight = true;
            timeScaleLayout.childForceExpandWidth = false;
            timeScaleLayout.childForceExpandHeight = true;

            // 時間経過スケール変更ボタンを作成する関数
            Button CreateTimeScaleButton(string buttonName, string buttonText, Vector2 size)
            {
                GameObject buttonObj = new GameObject(buttonName);
                buttonObj.transform.SetParent(timeScaleAreaObj.transform, false);
                Image buttonImage = buttonObj.AddComponent<Image>();
                buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

                Button button = buttonObj.AddComponent<Button>();

                RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
                buttonRect.sizeDelta = size;

                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(buttonObj.transform, false);
                Text text = textObj.AddComponent<Text>();
                text.text = buttonText;
                text.font = GetFont();
                text.fontSize = 12;
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.white;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;

                return button;
            }

            Button pauseButton = CreateTimeScaleButton("PauseButton", "ポーズ", new Vector2(60, 50));
            Button speed1xButton = CreateTimeScaleButton("Speed1xButton", "1倍", new Vector2(60, 50));
            Button speed2xButton = CreateTimeScaleButton("Speed2xButton", "2倍", new Vector2(60, 50));
            Button speed3xButton = CreateTimeScaleButton("Speed3xButton", "3倍", new Vector2(60, 50));

            // MenuBarUIコンポーネントを追加
            MenuBarUI menuBarUI = menuBarObj.AddComponent<MenuBarUI>();

            // リフレクションでフィールドを設定
            var menuBarUIType = typeof(MenuBarUI);
            var reputationTextField = menuBarUIType.GetField("reputationText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            reputationTextField?.SetValue(menuBarUI, reputationText);

            var moneyTextField = menuBarUIType.GetField("moneyText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            moneyTextField?.SetValue(menuBarUI, moneyText);

            var homeButtonField = menuBarUIType.GetField("homeButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            homeButtonField?.SetValue(menuBarUI, homeButton);

            var clubPolicyButtonField = menuBarUIType.GetField("clubPolicyButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            clubPolicyButtonField?.SetValue(menuBarUI, clubPolicyButton);

            var memberListButtonField = menuBarUIType.GetField("memberListButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            memberListButtonField?.SetValue(menuBarUI, memberListButton);

            var itemButtonField = menuBarUIType.GetField("itemButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            itemButtonField?.SetValue(menuBarUI, itemButton);

            var researchButtonField = menuBarUIType.GetField("researchButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            researchButtonField?.SetValue(menuBarUI, researchButton);

            var achievementButtonField = menuBarUIType.GetField("achievementButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            achievementButtonField?.SetValue(menuBarUI, achievementButton);

            var systemButtonField = menuBarUIType.GetField("systemButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            systemButtonField?.SetValue(menuBarUI, systemButton);

            var dateTextField = menuBarUIType.GetField("dateText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dateTextField?.SetValue(menuBarUI, dateText);

            var pauseButtonField = menuBarUIType.GetField("pauseButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            pauseButtonField?.SetValue(menuBarUI, pauseButton);

            var speed1xButtonField = menuBarUIType.GetField("speed1xButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            speed1xButtonField?.SetValue(menuBarUI, speed1xButton);

            var speed2xButtonField = menuBarUIType.GetField("speed2xButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            speed2xButtonField?.SetValue(menuBarUI, speed2xButton);

            var speed3xButtonField = menuBarUIType.GetField("speed3xButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            speed3xButtonField?.SetValue(menuBarUI, speed3xButton);

            // 初期表示を更新
            menuBarUI.RefreshDisplay();

            Debug.Log("Menu Bar created successfully!");
        }

        [MenuItem("Setup/Create All Screen Canvases", false, 41)]
        public static void CreateAllScreenCanvases()
        {
            CreateClubPolicyCanvas();
            CreateItemCanvas();
            CreateResearchCanvas();
            CreateAchievementCanvas();
            CreateSystemCanvas();

            // 部員一覧Canvasが既に存在するかチェック
            Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Canvas memberListCanvas = null;
            foreach (var canvas in allCanvases)
            {
                if (canvas.name == "ClubMemberListCanvas")
                {
                    memberListCanvas = canvas;
                    break;
                }
            }

            // 部員一覧Canvasが存在しない場合は作成
            if (memberListCanvas == null)
            {
                CreateClubMemberListUI();
            }
            else
            {
                // 既存のCanvasをScreenManagerに登録
                ScreenManager screenManager = ScreenManager.Instance;
                screenManager.RegisterCanvas(ScreenManager.ScreenType.MemberList, memberListCanvas);
            }

            Debug.Log("All screen canvases created successfully!");
        }

        private static void CreateClubPolicyCanvas()
        {
            GameObject canvasObj = new GameObject("ClubPolicyCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // 画面用の基本UI（後で拡張可能）
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(canvas.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;
            contentRect.offsetMin = new Vector2(0, 90); // メニューバーの高さ分
            contentRect.offsetMax = Vector2.zero;

            Image contentImage = contentObj.AddComponent<Image>();
            contentImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(contentObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "部活方針";
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 50);
            titleRect.anchoredPosition = new Vector2(0, -25);

            canvas.gameObject.SetActive(false);

            // ScreenManagerに登録
            ScreenManager screenManager = ScreenManager.Instance;
            screenManager.RegisterCanvas(ScreenManager.ScreenType.ClubPolicy, canvas);

            Debug.Log("Club Policy Canvas created successfully!");
        }

        private static void CreateItemCanvas()
        {
            GameObject canvasObj = new GameObject("ItemCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(canvas.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;
            contentRect.offsetMin = new Vector2(0, 90);
            contentRect.offsetMax = Vector2.zero;

            Image contentImage = contentObj.AddComponent<Image>();
            contentImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(contentObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "アイテム";
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 50);
            titleRect.anchoredPosition = new Vector2(0, -25);

            canvas.gameObject.SetActive(false);

            ScreenManager screenManager = ScreenManager.Instance;
            screenManager.RegisterCanvas(ScreenManager.ScreenType.Item, canvas);

            Debug.Log("Item Canvas created successfully!");
        }

        private static void CreateResearchCanvas()
        {
            GameObject canvasObj = new GameObject("ResearchCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(canvas.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;
            contentRect.offsetMin = new Vector2(0, 90);
            contentRect.offsetMax = Vector2.zero;

            Image contentImage = contentObj.AddComponent<Image>();
            contentImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(contentObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "研究開発";
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 50);
            titleRect.anchoredPosition = new Vector2(0, -25);

            canvas.gameObject.SetActive(false);

            ScreenManager screenManager = ScreenManager.Instance;
            screenManager.RegisterCanvas(ScreenManager.ScreenType.Research, canvas);

            Debug.Log("Research Canvas created successfully!");
        }

        private static void CreateAchievementCanvas()
        {
            GameObject canvasObj = new GameObject("AchievementCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(canvas.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;
            contentRect.offsetMin = new Vector2(0, 90);
            contentRect.offsetMax = Vector2.zero;

            Image contentImage = contentObj.AddComponent<Image>();
            contentImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(contentObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "実績";
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 50);
            titleRect.anchoredPosition = new Vector2(0, -25);

            canvas.gameObject.SetActive(false);

            ScreenManager screenManager = ScreenManager.Instance;
            screenManager.RegisterCanvas(ScreenManager.ScreenType.Achievement, canvas);

            Debug.Log("Achievement Canvas created successfully!");
        }

        private static void CreateSystemCanvas()
        {
            GameObject canvasObj = new GameObject("SystemCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(canvas.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;
            contentRect.offsetMin = new Vector2(0, 90);
            contentRect.offsetMax = Vector2.zero;

            Image contentImage = contentObj.AddComponent<Image>();
            contentImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(contentObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "システム";
            titleText.font = GetFont();
            titleText.fontSize = 24;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 50);
            titleRect.anchoredPosition = new Vector2(0, -25);

            canvas.gameObject.SetActive(false);

            ScreenManager screenManager = ScreenManager.Instance;
            screenManager.RegisterCanvas(ScreenManager.ScreenType.System, canvas);

            Debug.Log("System Canvas created successfully!");
        }

        // Setup/CreateWindows/サブメニュー
        [MenuItem("Setup/CreateWindows/Club Member List", false, 50)]
        public static void CreateClubMemberListWindow()
        {
            CreateClubMemberListUI();
        }

        [MenuItem("Setup/CreateWindows/Club Policy", false, 51)]
        public static void CreateClubPolicyWindow()
        {
            CreateClubPolicyCanvas();
        }

        [MenuItem("Setup/CreateWindows/Item", false, 52)]
        public static void CreateItemWindow()
        {
            CreateItemCanvas();
        }

        [MenuItem("Setup/CreateWindows/Research", false, 53)]
        public static void CreateResearchWindow()
        {
            CreateResearchCanvas();
        }

        [MenuItem("Setup/CreateWindows/Achievement", false, 54)]
        public static void CreateAchievementWindow()
        {
            CreateAchievementCanvas();
        }

        [MenuItem("Setup/CreateWindows/System", false, 55)]
        public static void CreateSystemWindow()
        {
            CreateSystemCanvas();
        }
#endif
    }
}

