using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Characters
{
    /// <summary>
    /// The player, represents the user
    /// </summary>
    public class Player : Character
    {
        /// <summary>
        /// Constructs a player
        /// </summary>
        /// <param name="name">the player's name</param>
        /// <param name="health">the player's health</param>
        /// <param name="baseDamage">the player's base damage</param>
        public Player(string name, int health, int baseDamage) : base(name, health, baseDamage)
        {
        }

        /// <summary>
        /// Parameterless constructor intented for serialization only
        /// </summary>
        public Player() { }
    }
}
