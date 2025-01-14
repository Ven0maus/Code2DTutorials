using SadRogue.Primitives;

namespace Roguelike.Entities.Actors
{
    internal class Goblin : Actor
    {
        public Goblin(Point position) : 
            base(Color.Green, Color.Transparent, 'g', 1, maxHealth: 5)
        {
            Name = "Goblin";
            Position = position;

            // Set base stats
            Stats.Set(atk: 2, dodge: 5);
        }
    }
}
