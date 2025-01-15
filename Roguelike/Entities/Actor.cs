using Roguelike.Entities.Actors;
using Roguelike.Screens;
using SadConsole.Entities;
using SadRogue.Primitives;
using System.Linq;

namespace Roguelike.Entities
{
    internal abstract class Actor : Entity
    {
        protected Actor(Color foreground, Color background, int glyph, int zIndex, int maxHealth) : base(foreground, background, glyph, zIndex)
        {
            Stats = new ActorStats(this, maxHealth);
        }

        public bool IsAlive => Stats.Health > 0;
        public ActorStats Stats { get; }

        public virtual bool Move(int x, int y)
        {
            var tilemap = ScreenContainer.Instance.World.Tilemap;
            var actorManager = ScreenContainer.Instance.World.ActorManager;

            if (!IsAlive || (Position.X == x && Position.Y == y)) return false;
            
            // If the position is out of bounds, don't allow movement
            if (!tilemap.InBounds(x, y)) return false;
            
            // If another actor already exists at the location, don't allow movement
            if (actorManager.ExistsAt((x, y))) return false;

            // Don't allow movement for these cases
            var obstruction = tilemap[x, y].Obstruction;
            switch (obstruction)
            {
                case World.ObstructionType.FullyBlocked:
                case World.ObstructionType.MovementBlocked:
                    return false;
            }

            // Set new position
            Position = new Point(x, y);
            return true;
        }

        public bool Move(Direction direction)
        {
            var position = Position + direction;
            return Move(position.X, position.Y);
        }

        public virtual void ApplyDamage(int health)
        {
            Stats.Health -= health;

            if (!IsAlive && ScreenContainer.Instance.World.ActorManager.Contains(this))
            {
                OnDeath();
            }
        }

        protected virtual void OnDeath()
        {
            // Remove from manager so its no longer rendered
            ScreenContainer.Instance.World.ActorManager.Remove(this);
            MessagesScreen.WriteLine($"{Name} has died.");
        }
    }
}
