using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// インシデントの基底クラス
    /// 各インシデントタイプはこのクラスを継承して実装する
    /// </summary>
    public abstract class Incident
    {
        /// <summary>
        /// AddressableManagerからPrefabを読み込む（ヘルパーメソッド）
        /// </summary>
        /// <param name="address">Prefabのアドレス</param>
        /// <returns>読み込まれたPrefab。読み込みに失敗した場合はnull</returns>
        protected GameObject LoadPrefabFromAddressable(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            try
            {
                var addressableManager = AddressableManager.Instance;
                if (addressableManager == null)
                {
                    Debug.LogError($"[Incident] AddressableManagerが見つかりません: {Id}");
                    return null;
                }

                return addressableManager.LoadAsset<GameObject>(address);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Incident] Prefabの読み込みに失敗しました: {address}, エラー: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// インシデントの一意なID
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// 必須インシデントかどうか（必須インシデントは発生時に自動的にポーズする）
        /// </summary>
        public abstract bool IsMandatory { get; }

        /// <summary>
        /// 時限（週単位）。nullの場合は無制限
        /// </summary>
        public abstract int? TimeLimitWeeks { get; }

        /// <summary>
        /// アイコンの色（インシデントタイプに応じて変更可能）
        /// </summary>
        public virtual Color IconColor => Color.white;

        /// <summary>
        /// 条件をチェックしてインシデントが発生すべきかを判定
        /// </summary>
        /// <param name="year">現在の年</param>
        /// <param name="month">現在の月</param>
        /// <param name="week">現在の週</param>
        /// <returns>発生すべき場合はtrue</returns>
        public abstract bool CheckCondition(int year, int month, int week);

        /// <summary>
        /// ウィンドウ用のPrefabを取得（派生クラスで実装）
        /// </summary>
        /// <returns>ウィンドウPrefab</returns>
        public abstract GameObject GetWindowPrefab();

        /// <summary>
        /// AddressableManagerで読み込むPrefabのアドレスを取得（オプショナル）
        /// nullを返す場合は動的作成を使用
        /// </summary>
        /// <returns>Prefabのアドレス。動的作成の場合はnull</returns>
        public virtual string GetWindowPrefabAddress()
        {
            return null;
        }

        /// <summary>
        /// インシデントが解決されたときに呼ばれる
        /// 派生クラスでオーバーライドしてカスタム処理を実装
        /// </summary>
        /// <param name="state">解決されたIncidentState</param>
        public virtual void OnResolve(IncidentState state)
        {
            // デフォルトは空実装
        }
    }
}
