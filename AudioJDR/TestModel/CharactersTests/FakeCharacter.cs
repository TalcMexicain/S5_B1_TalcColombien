using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Characters;

namespace TestModel.CharactersTests
{
    public class FakeCharacter : Character
    {
        public FakeCharacter(string name, int health, int baseDamage) : base(name, health, baseDamage)
        {
        }
    }
}
