using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleGame
{
    /// <summary>
    /// 時間プログレスバーを表示するUIコンポーネント
    /// 進捗値（0~1）を受け取り、プログレスバーを表示する
    /// </summary>
    public class TimeProgressBar : MonoBehaviour
    {
        [Header("Progress Bar")]
        [SerializeField] private Image fillImage;

        /// <summary>
        /// 進捗を設定（0~1の範囲）
        /// </summary>
        /// <param name="progress">進捗値（0.0 ~ 1.0）</param>
        public void SetProgress(float progress)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Clamp01(progress);
            }
        }
    }
}