using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 資源を表すクラス
    /// Itemクラスのサブクラス
    /// </summary>
    public class Resource : Item
    {
        /// <summary>現在の保持数を表すint</summary>
        public int Count { get; set; }

        /// <summary>説明文</summary>
        public string Description { get; set; }

        /// <summary>アイコン用ゲームオブジェクト（簡易実装）</summary>
        private GameObject icon;

        public Resource() : base()
        {
            Count = 0;
            Description = string.Empty;
            icon = null;
        }

        public Resource(string name, int count, string description) : base(name)
        {
            Count = count;
            Description = description;
            icon = null;
        }

        public override GameObject GetIcon()
        {
            // 簡易実装: nullを返す（後で実装）
            return icon;
        }

        public override string GetDescription()
        {
            return Description;
        }

        public override ItemTag GetItemTag()
        {
            return ItemTag.Resource;
        }
    }
}
