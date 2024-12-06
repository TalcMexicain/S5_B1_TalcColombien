using Model.Items;
using Model.Resources.Localization;

namespace Model.Characters
{
    /// <summary>
    /// Represents a character within a <see cref="Story"/>
    /// </summary>
    public abstract class Character
    {
        #region Attributes

        private string name;
        private int health;
        private int baseDamage;
        private List<Item> inventory;
        private WeaponItem? equippedWeapon;

        #endregion

        #region Properties

        /// <summary>
        /// The character's name
        /// </summary>
        public string Name 
        { 
            get => name; 
            set => name = value; 
        }

        /// <summary>
        /// The character's health points
        /// </summary>
        public int Health 
        { 
            get => health; 
            set => health = value; 
        }

        /// <summary>
        /// The character's base damage (with no weapon equipped)
        /// </summary>
        public int BaseDamage 
        { 
            get => baseDamage; 
            set => baseDamage = value; 
        }

        /// <summary>
        /// The character's list of collected items
        /// </summary>
        public List<Item> Inventory 
        { 
            get => inventory; 
            set => inventory = value; 
        }

        /// <summary>
        /// The character's equipped weapon
        /// </summary>
        public WeaponItem? EquippedWeapon 
        { 
            get => equippedWeapon; 
            set => equippedWeapon = value; 
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for the Character class.
        /// </summary>
        /// <param name="name">The name of the character</param>
        /// <param name="health">The health of the character</param>
        /// <param name="baseDamage">The base damage dealt by the character</param>
        protected Character(string name, int health, int baseDamage)
        {
            this.name = name;
            this.health = health;
            this.baseDamage = baseDamage;
            this.inventory = new List<Item>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reduces the character's health by the specified damage amount
        /// </summary>
        /// <param name="damage">The amount of damage to take</param>
        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health < 0) health = 0; 
        }

        /// <summary>
        /// Attacks a target character, dealing base damage
        /// </summary>
        /// <param name="target">The character being attacked</param>
        public void Attack(Character target)
        {
            if (equippedWeapon == null)
            {
                target.TakeDamage(baseDamage);
            }
            else
            {
                target.TakeDamage(equippedWeapon.Damage);
            }
        }

        /// <summary>
        /// Adds an item to the character's inventory
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddToInventory(Item item)
        {
            inventory.Add(item);
        }

        /// <summary>
        /// Drops an item from the character's inventory
        /// </summary>
        /// <param name="item">The item to drop</param>
        public void DropItem(Item item)
        {
            inventory.Remove(item);
        }

        /// <summary>
        /// Uses an item from the inventory
        /// </summary>
        /// <param name="item">The item to use</param>
        public void UseItem(Item item)
        {
            if (Inventory.Contains(item))
            {
                item.Use(this);
            }
        }

        /// <summary>
        /// Provides a precise description of the character depending on its current status.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Determine health status
            string healthStatus = health <= 10
                ? string.Format(AppResourcesModel.CharacterCriticalHealth, health)
                : string.Format(AppResourcesModel.CharacterHealthStatus, health);

            // Determine weapon status
            string weaponStatus;
            if (equippedWeapon == null)
            {
                weaponStatus = string.Format(AppResourcesModel.CharacterNoWeapon, baseDamage);
            }
            else
            {
                weaponStatus = string.Format(AppResourcesModel.CharacterEquippedWeapon, equippedWeapon.Name, equippedWeapon.Damage);
            }

            // Determine inventory status
            string inventoryStatus = inventory.Count == 0
                ? AppResourcesModel.CharacterInventoryEmpty
                : string.Format(AppResourcesModel.CharacterInventoryItems, string.Join(", ", inventory.Select(i => i.Name)));

            return string.Format(AppResourcesModel.CharacterDescriptionFormat, name, healthStatus, weaponStatus, inventoryStatus);
        }


        #endregion
    }
}