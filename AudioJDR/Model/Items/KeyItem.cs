using Model.Characters;
using Model.Resources.Localization;

namespace Model.Items
{
    /// <summary>
    /// Class representing a "Key" item, typically used to unlock an <see cref="Option"/>, extends <see cref="Item"/>
    /// </summary>
    public class KeyItem : Item
    {
        #region Constructor

        /// <summary>
        /// Constructor of a keyItem. refer to <see cref="Item"/>
        /// </summary>
        /// <param name="name">The name of the item</param>
        public KeyItem(string name) : base(name)
        {
        }

        /// <summary>
        /// Parameterless constructor for serialization
        /// </summary>
        public KeyItem() { }

        #endregion

        #region Methods

        /// <summary>
        /// Uses the item if the specified <see cref="Character"/>
        /// </summary>
        /// <param name="character">The character using the item</param>
        /// <returns>A string indicating that the item was used</returns>
        internal override string Use(Character character)
        {
            return string.Format(AppResourcesModel.UsedKeyItemFormat, character.Name, this.Name);
        }

        public override string ToString()
        {
            return string.Format(AppResourcesModel.KeyItemDescription, Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyItem other)
            {
                return this.Name == other.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
        
        #endregion
    }
}
