namespace MonsterBattleGame
{
    /// <summary>
    /// 特別な特性を表す抽象クラス
    /// 主にステータスを変化させる
    /// 一旦枠だけ実装する
    /// </summary>
    public abstract class Trait
    {
        /// <summary>特性の名前</summary>
        public string Name { get; set; }

        /// <summary>特性の説明</summary>
        public abstract string Description { get; }

        public Trait()
        {
            Name = string.Empty;
        }

        public Trait(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 基本的なTraitの実装
    /// </summary>
    public class BasicTrait : Trait
    {
        private string _description;

        public override string Description => _description;

        public BasicTrait() : base()
        {
            _description = string.Empty;
        }

        public BasicTrait(string name, string description) : base(name)
        {
            _description = description;
        }
    }
}

