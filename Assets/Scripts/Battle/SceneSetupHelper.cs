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

        [MenuItem("Setup/Create Time System UI", false, 20)]
        public static void CreateTimeSystemUI()
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
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<EventSystem>();
                
#if USE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
                if (eventSystemObj.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystemObj.AddComponent<InputSystemUIInputModule>();
                }
#endif
            }
            else
            {
#if USE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
                if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }
#endif
            }

            // 時間システムウィンドウの作成
            GameObject timeWindow = new GameObject("TimeSystemWindow");
            timeWindow.transform.SetParent(canvas.transform, false);
            RectTransform timeWindowRect = timeWindow.AddComponent<RectTransform>();
            timeWindowRect.anchorMin = new Vector2(0, 1);
            timeWindowRect.anchorMax = new Vector2(0, 1);
            timeWindowRect.sizeDelta = new Vector2(300, 150);
            timeWindowRect.anchoredPosition = new Vector2(150, -75);

            Image timeWindowImage = timeWindow.AddComponent<Image>();
            timeWindowImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

            // 時間表示テキスト
            GameObject timeTextObj = new GameObject("TimeText");
            timeTextObj.transform.SetParent(timeWindow.transform, false);
            Text timeText = timeTextObj.AddComponent<Text>();
            timeText.text = "1年目 1月 1週";
            timeText.font = GetFont();
            timeText.fontSize = 20;
            timeText.alignment = TextAnchor.MiddleCenter;
            timeText.color = Color.white;

            RectTransform timeTextRect = timeTextObj.GetComponent<RectTransform>();
            timeTextRect.anchorMin = new Vector2(0, 0.5f);
            timeTextRect.anchorMax = new Vector2(1, 1);
            timeTextRect.sizeDelta = Vector2.zero;
            timeTextRect.offsetMin = new Vector2(10, 10);
            timeTextRect.offsetMax = new Vector2(-10, -10);

            // ボタンエリア
            GameObject buttonArea = new GameObject("ButtonArea");
            buttonArea.transform.SetParent(timeWindow.transform, false);
            RectTransform buttonAreaRect = buttonArea.AddComponent<RectTransform>();
            buttonAreaRect.anchorMin = new Vector2(0, 0);
            buttonAreaRect.anchorMax = new Vector2(1, 0.5f);
            buttonAreaRect.sizeDelta = Vector2.zero;
            buttonAreaRect.offsetMin = new Vector2(10, 10);
            buttonAreaRect.offsetMax = new Vector2(-10, -10);

            HorizontalLayoutGroup buttonLayout = buttonArea.AddComponent<HorizontalLayoutGroup>();
            buttonLayout.spacing = 10;
            buttonLayout.padding = new RectOffset(10, 10, 10, 10);
            buttonLayout.childAlignment = TextAnchor.MiddleCenter;
            buttonLayout.childControlWidth = true;
            buttonLayout.childControlHeight = true;
            buttonLayout.childForceExpandWidth = true;
            buttonLayout.childForceExpandHeight = true;

            // ポーズボタン（0倍）
            GameObject pauseButtonObj = new GameObject("PauseButton");
            pauseButtonObj.transform.SetParent(buttonArea.transform, false);
            Image pauseButtonImage = pauseButtonObj.AddComponent<Image>();
            pauseButtonImage.color = new Color(0.5f, 0.7f, 0.5f, 1f);

            Button pauseButton = pauseButtonObj.AddComponent<Button>();

            GameObject pauseTextObj = new GameObject("Text");
            pauseTextObj.transform.SetParent(pauseButtonObj.transform, false);
            Text pauseText = pauseTextObj.AddComponent<Text>();
            pauseText.text = "ポーズ";
            pauseText.font = GetFont();
            pauseText.fontSize = 14;
            pauseText.alignment = TextAnchor.MiddleCenter;
            pauseText.color = Color.white;

            RectTransform pauseTextRect = pauseTextObj.GetComponent<RectTransform>();
            pauseTextRect.anchorMin = Vector2.zero;
            pauseTextRect.anchorMax = Vector2.one;
            pauseTextRect.sizeDelta = Vector2.zero;

            // 1倍速ボタン
            GameObject speed1xButtonObj = new GameObject("Speed1xButton");
            speed1xButtonObj.transform.SetParent(buttonArea.transform, false);
            Image speed1xButtonImage = speed1xButtonObj.AddComponent<Image>();
            speed1xButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            Button speed1xButton = speed1xButtonObj.AddComponent<Button>();

            GameObject speed1xTextObj = new GameObject("Text");
            speed1xTextObj.transform.SetParent(speed1xButtonObj.transform, false);
            Text speed1xText = speed1xTextObj.AddComponent<Text>();
            speed1xText.text = "1倍";
            speed1xText.font = GetFont();
            speed1xText.fontSize = 14;
            speed1xText.alignment = TextAnchor.MiddleCenter;
            speed1xText.color = Color.white;

            RectTransform speed1xTextRect = speed1xTextObj.GetComponent<RectTransform>();
            speed1xTextRect.anchorMin = Vector2.zero;
            speed1xTextRect.anchorMax = Vector2.one;
            speed1xTextRect.sizeDelta = Vector2.zero;

            // 2倍速ボタン
            GameObject speed2xButtonObj = new GameObject("Speed2xButton");
            speed2xButtonObj.transform.SetParent(buttonArea.transform, false);
            Image speed2xButtonImage = speed2xButtonObj.AddComponent<Image>();
            speed2xButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            Button speed2xButton = speed2xButtonObj.AddComponent<Button>();

            GameObject speed2xTextObj = new GameObject("Text");
            speed2xTextObj.transform.SetParent(speed2xButtonObj.transform, false);
            Text speed2xText = speed2xTextObj.AddComponent<Text>();
            speed2xText.text = "2倍";
            speed2xText.font = GetFont();
            speed2xText.fontSize = 14;
            speed2xText.alignment = TextAnchor.MiddleCenter;
            speed2xText.color = Color.white;

            RectTransform speed2xTextRect = speed2xTextObj.GetComponent<RectTransform>();
            speed2xTextRect.anchorMin = Vector2.zero;
            speed2xTextRect.anchorMax = Vector2.one;
            speed2xTextRect.sizeDelta = Vector2.zero;

            // 3倍速ボタン
            GameObject speed3xButtonObj = new GameObject("Speed3xButton");
            speed3xButtonObj.transform.SetParent(buttonArea.transform, false);
            Image speed3xButtonImage = speed3xButtonObj.AddComponent<Image>();
            speed3xButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            Button speed3xButton = speed3xButtonObj.AddComponent<Button>();

            GameObject speed3xTextObj = new GameObject("Text");
            speed3xTextObj.transform.SetParent(speed3xButtonObj.transform, false);
            Text speed3xText = speed3xTextObj.AddComponent<Text>();
            speed3xText.text = "3倍";
            speed3xText.font = GetFont();
            speed3xText.fontSize = 14;
            speed3xText.alignment = TextAnchor.MiddleCenter;
            speed3xText.color = Color.white;

            RectTransform speed3xTextRect = speed3xTextObj.GetComponent<RectTransform>();
            speed3xTextRect.anchorMin = Vector2.zero;
            speed3xTextRect.anchorMax = Vector2.one;
            speed3xTextRect.sizeDelta = Vector2.zero;

            // GameTimeUIコンポーネントを追加（型を動的に取得）
            System.Type timeUIType = System.Type.GetType("MonsterBattleGame.GameTimeUI, Assembly-CSharp");
            if (timeUIType == null)
            {
                // フォールバック: 名前空間なしで検索
                timeUIType = System.Type.GetType("GameTimeUI");
            }
            
            if (timeUIType == null)
            {
                Debug.LogError("GameTimeUI type not found!");
                return;
            }

            Component timeUI = timeWindow.AddComponent(timeUIType);
            var timeTextField = timeUIType.GetField("timeText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            timeTextField?.SetValue(timeUI, timeText);

            var pauseButtonField = timeUIType.GetField("pauseButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            pauseButtonField?.SetValue(timeUI, pauseButton);

            var speed1xButtonField = timeUIType.GetField("speed1xButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            speed1xButtonField?.SetValue(timeUI, speed1xButton);

            var speed2xButtonField = timeUIType.GetField("speed2xButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            speed2xButtonField?.SetValue(timeUI, speed2xButton);

            var speed3xButtonField = timeUIType.GetField("speed3xButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            speed3xButtonField?.SetValue(timeUI, speed3xButton);

            Debug.Log("Time System UI created successfully!");
        }
#endif
    }
}

