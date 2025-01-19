using Model.Characters;
using Model.Resources.Localization;

namespace Model.Items
{
    /// <summary>
    /// Class representing a consumable item, typically used to heal a <see cref="Character"/>, extends <see cref="Item"/>
    /// </summary>
    public class ConsumableItem : Item
    {
        #region Attributes

        private int healAmount;

        #endregion

        #region Properties

        /// <summary>
        /// The amount of health given by the item upon use
        /// </summary>
        public int HealAmount 
        { 
            get => healAmount; 
            set => healAmount = value; 
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of a consumableItem. refer to <see cref="Item"/>
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="healAmount">The amount of health given by the item upon use</param>
        public ConsumableItem(string name, int healAmount) : base(name)
        {
            this.healAmount = healAmount;
        }

        /// <summary>
        /// Parameterless constructor for serialization
        /// </summary>
        public ConsumableItem() { }

        #endregion

        #region Methods

        /// <summary>
        /// Modifies the specified <see cref="Character"/>'s health
        /// </summary>
        /// <param name="character">The character using the item</param>
        /// <returns>A string indicating that the item was used</returns>
        internal override string Use(Character character)
        {
            character.Health += healAmount; 
            return string.Format(AppResourcesModel.UsedConsumableItemFormat,character.Name,healAmount,this.Name);
        }

        public override string ToString()
        {
            return string.Format(AppResourcesModel.ConsumableItemDescription, Name, HealAmount);
        }

        #endregion
    }
}
