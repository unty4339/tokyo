using System;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// ゲーム内時間を管理するシングルトン
    /// 週刻みで時間を管理し、年・月・週で表示
    /// </summary>
    public class GameTimeManager : MonoBehaviour
    {
        private static GameTimeManager _instance;
        public static GameTimeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameTimeManager");
                    _instance = go.AddComponent<GameTimeManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        // 時間設定
        private const int WEEKS_PER_MONTH = 4;
        private const int WEEKS_PER_YEAR = 52;
        private const float SECONDS_PER_WEEK = 4f; // 1週 = 60秒（後で調整可能）

        // 現在の時間（週単位で累積）
        [SerializeField] float currentWeekFloat = 0f; // 浮動小数点で週を管理

        // 時間経過速度（0倍、1倍、2倍、3倍）
        private int timeScale = 1;

        // イベント
        public event Action<int, int, int> OnWeekChanged; // 年, 月, 週

        /// <summary>
        /// 現在の年（1年目から開始）
        /// </summary>
        public int CurrentYear
        {
            get
            {
                int totalWeeks = Mathf.FloorToInt(currentWeekFloat);
                return (totalWeeks / WEEKS_PER_YEAR) + 1;
            }
        }

        /// <summary>
        /// 現在の月（1月から開始、1年 = 13ヶ月）
        /// </summary>
        public int CurrentMonth
        {
            get
            {
                int totalWeeks = Mathf.FloorToInt(currentWeekFloat);
                int weeksInCurrentYear = totalWeeks % WEEKS_PER_YEAR;
                return (weeksInCurrentYear / WEEKS_PER_MONTH) + 1;
            }
        }

        /// <summary>
        /// 現在の週（1週から開始）
        /// </summary>
        public int CurrentWeek
        {
            get
            {
                int totalWeeks = Mathf.FloorToInt(currentWeekFloat);
                int weeksInCurrentYear = totalWeeks % WEEKS_PER_YEAR;
                return (weeksInCurrentYear % WEEKS_PER_MONTH) + 1;
            }
        }

        /// <summary>
        /// 時間経過速度（0倍、1倍、2倍、3倍）
        /// </summary>
        public int TimeScale
        {
            get { return timeScale; }
            set
            {
                timeScale = Mathf.Clamp(value, 0, 3);
            }
        }

        /// <summary>
        /// ポーズ中かどうか
        /// </summary>
        public bool IsPaused
        {
            get { return timeScale == 0; }
        }

        private int lastWeekInt = -1; // 前回の週（整数）を記録

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // 時間経過速度が0の場合は何もしない
            if (timeScale == 0)
            {
                return;
            }

            // 時間を進める（週単位）
            float deltaWeeks = (Time.deltaTime * timeScale) / SECONDS_PER_WEEK;
            currentWeekFloat += deltaWeeks;

            // 週が変わったかチェック
            int currentWeekInt = Mathf.FloorToInt(currentWeekFloat);
            if (currentWeekInt != lastWeekInt)
            {
                lastWeekInt = currentWeekInt;
                OnWeekChanged?.Invoke(CurrentYear, CurrentMonth, CurrentWeek);
            }
        }

        /// <summary>
        /// ポーズ
        /// </summary>
        public void Pause()
        {
            TimeScale = 0;
        }

        /// <summary>
        /// 再開（前回の速度に戻す、デフォルトは1倍）
        /// </summary>
        public void Resume()
        {
            if (timeScale == 0)
            {
                TimeScale = 1; // デフォルトは1倍速
            }
        }

        /// <summary>
        /// 時間をリセット（ゲーム開始時点に戻す）
        /// </summary>
        public void ResetTime()
        {
            currentWeekFloat = 0f;
            lastWeekInt = -1;
        }

        /// <summary>
        /// 時間表示用の文字列を取得（「i年目 j月 k週」形式）
        /// </summary>
        public string GetTimeString()
        {
            return $"{CurrentYear}年目 {CurrentMonth}月 {CurrentWeek}週";
        }
    }
}

