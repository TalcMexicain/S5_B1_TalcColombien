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
        public Enemy(string name, int health, int baseDamage) : base(name, health, baseDamage)
        {
        }
    }
}
