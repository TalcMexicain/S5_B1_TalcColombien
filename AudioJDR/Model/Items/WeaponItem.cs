using Model.Characters;
using Model.Resources.Localization;

namespace Model.Items
{
    /// <summary>
    /// Class representing a weapon item, typically used be equipped by a <see cref="Character"/>, extends <see cref="Item"/>
    /// </summary>
    public class WeaponItem : Item
    {
        #region Attributes

        private int damage;

        #endregion

        #region Properties

        /// <summary>
        /// The damage dealt by the weapon
        /// </summary>
        public int Damage
        {
            get => damage;
            set => damage = value;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for a WeaponItem.
        /// </summary>
        /// <param name="name">The name of the weapon</param>
        /// <param name="damage">The damage dealt by the weapon</param>
        public WeaponItem(string name, int damage) : base(name)
        {
            this.damage = damage;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Equips the weapon for the specified character.
        /// </summary>
        /// <param name="character">The character equpping the weapon</param>
        /// <returns>A string indicating that the weapon was equipped</returns>
        internal override string Use(Character character)
        {
            string outcome = string.Format(AppResourcesModel.CantEquipWeaponItemFormat, character.Name, this.Name);
            if(character.EquippedWeapon == null)
            {
                character.EquippedWeapon = this;
                outcome = string.Format(AppResourcesModel.EquippedWeaponItemFormat, character.Name, this.Name);
            }
            else if(character.EquippedWeapon == this) 
            {
                character.EquippedWeapon = null;
                outcome = string.Format(AppResourcesModel.UnEquippedWeaponItemFormat, character.Name, this.Name);
            }
            return outcome;
        }

        #endregion
    }
}