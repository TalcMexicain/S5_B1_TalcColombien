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
        /// Property used by the serializer to differenciate the Items by type.
        /// </summary>
        public string Type => GetType().Name;

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

        /// <summary>
        /// Parameterless constructor for serialization
        /// </summary>
        public Item() { }

        #endregion

        #region Methods

        /// <summary>
        /// Uses the item for the specified <see cref="Character"/>
        /// </summary>
        /// <param name="character">The character using the item</param>
        /// <returns>A string indicating that the item was used</returns>
        internal abstract string Use(Character character);

        /// <summary>
        /// Provides an accurate description of the item, according to its specific type and attributes
        /// </summary>
        /// <returns>A string containing the description of the item</returns>
        public override abstract string ToString();

        #endregion
    }
}
