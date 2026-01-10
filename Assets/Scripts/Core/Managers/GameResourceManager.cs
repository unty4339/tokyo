using System;
using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// ゲーム内の資源（評判、資金など）を管理するシングルトン
    /// </summary>
    public class GameResourceManager : MonoBehaviour
    {
        private static GameResourceManager _instance;
        public static GameResourceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameResourceManager");
                    _instance = go.AddComponent<GameResourceManager>();
                }
                return _instance;
            }
        }

        [SerializeField] private int reputation = 0;
        [SerializeField] private int money = 0;

        // イベント
        public event Action<int> OnReputationChanged;
        public event Action<int> OnMoneyChanged;

        /// <summary>
        /// 現在の評判
        /// </summary>
        public int Reputation
        {
            get { return reputation; }
            private set
            {
                if (reputation != value)
                {
                    reputation = value;
                    OnReputationChanged?.Invoke(reputation);
                }
            }
        }

        /// <summary>
        /// 現在の資金
        /// </summary>
        public int Money
        {
            get { return money; }
            private set
            {
                if (money != value)
                {
                    money = value;
                    OnMoneyChanged?.Invoke(money);
                }
            }
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
            }
        }

        /// <summary>
        /// 評判を設定
        /// </summary>
        public void SetReputation(int value)
        {
            Reputation = Mathf.Max(0, value);
        }

        /// <summary>
        /// 評判を追加
        /// </summary>
        public void AddReputation(int amount)
        {
            SetReputation(Reputation + amount);
        }

        /// <summary>
        /// 評判を減らす
        /// </summary>
        public void SubtractReputation(int amount)
        {
            SetReputation(Reputation - amount);
        }

        /// <summary>
        /// 資金を設定
        /// </summary>
        public void SetMoney(int value)
        {
            Money = Mathf.Max(0, value);
        }

        /// <summary>
        /// 資金を追加
        /// </summary>
        public void AddMoney(int amount)
        {
            SetMoney(Money + amount);
        }

        /// <summary>
        /// 資金を減らす（不足する場合はfalseを返す）
        /// </summary>
        public bool SpendMoney(int amount)
        {
            if (Money >= amount)
            {
                SetMoney(Money - amount);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 資源をリセット（初期値に戻す）
        /// </summary>
        public void ResetResources()
        {
            SetReputation(0);
            SetMoney(0);
        }
    }
}

