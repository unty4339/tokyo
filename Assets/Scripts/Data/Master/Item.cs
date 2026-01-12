using UnityEngine;

namespace MonsterBattleGame
{
    /// <summary>
    /// 収拾可能なものを表す抽象クラス
    /// </summary>
    public abstract class Item
    {
        /// <summary>アイテム名</summary>
        public string Name { get; set; }

        public Item()
        {
            Name = string.Empty;
        }

        public Item(string name)
        {
            Name = name;
        }

        /// <summary>
        /// アイテム名stringを返すメソッド
        /// </summary>
        public virtual string GetItemName()
        {
            return Name;
        }

        /// <summary>
        /// アイコン用ゲームオブジェクトを返すメソッド
        /// </summary>
        public abstract GameObject GetIcon();

        /// <summary>
        /// 説明文stringを返すメソッド
        /// </summary>
        public abstract string GetDescription();

        /// <summary>
        /// ItemTagインスタンスを返すメソッド
        /// </summary>
        public abstract ItemTag GetItemTag();
    }
}
