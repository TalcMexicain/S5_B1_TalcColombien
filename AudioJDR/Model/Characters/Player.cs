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
        public Player(string name, int health, int baseDamage) : base(name, health, baseDamage)
        {
        }
    }
}
