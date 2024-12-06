using Model.Characters;

namespace Model.Items
{
    /// <summary>
    /// Abstract class representing an item
    /// </summary>
    public abstract class Item
    {
        #region Attributes

        private int idItem;
        protected string name;

        #endregion

        #region Properties

        /// <summary>
        /// The item's id
        /// </summary>
        public int IdItem 
        { 
            get => idItem; 
            set => idItem = value; 
        }

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name 
        { 
            get => name; 
            set => name = value; 
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Item's constructor, all items have a name
        /// </summary>
        /// <param name="name">the name of the item</param>
        protected Item(string name)
        {
            this.name = name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Uses the item for the specified <see cref="Character"/>
        /// </summary>
        /// <param name="character">The character using the item</param>
        /// <returns>A string indicating that the item was used</returns>
        internal abstract string Use(Character character);

        #endregion
    }
}
