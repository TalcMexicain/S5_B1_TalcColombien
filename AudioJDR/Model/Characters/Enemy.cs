using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Characters
{
    /// <summary>
    /// An enemy to the player - unused
    /// </summary>
    public class Enemy : Character
    {
        /// <summary>
        /// Constructs an enemy
        /// </summary>
        /// <param name="name">the enemy's name</param>
        /// <param name="health">the enemy's health</param>
        /// <param name="baseDamage">the enemy's base damage</param>
        public Enemy(string name, int health, int baseDamage) : base(name, health, baseDamage)
        {
        }

        /// <summary>
        /// Parameterless constructor intented for serialization only
        /// </summary>
        public Enemy() { }
    }
}
