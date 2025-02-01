using GoRogue.FOV;
using Roguelike.Entities.Actors;
using Roguelike.Screens;
using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Entities
{
    internal sealed class ActorManager
    {
        private readonly Dictionary<Point, Actor> _actors = [];
        public readonly EntityManager EntityComponent;

        public ActorManager()
        {
            EntityComponent = new()
            {
                SkipExistsChecks = true
            };
        }

        public bool Add(Actor actor)
        {
            if (ExistsAt(actor.Position)) return false;
            _actors[actor.Position] = actor;

            actor.PositionChanged += UpdateActorPositionWithinManager;
            EntityComponent.Add(actor);

            return true;
        }

        public bool Remove(Actor actor)
        {
            if (!ExistsAt(actor.Position)) return false;
            _actors.Remove(actor.Position);

            actor.PositionChanged -= UpdateActorPositionWithinManager;
            EntityComponent.Remove(actor);

            return true;
        }

        public Actor Get(Point point)
        {
            if (_actors.TryGetValue(point, out Actor actor))
                return actor;
            return null;
        }

        public bool ExistsAt(Point point)
        {
            return _actors.ContainsKey(point);
        }

        public bool Contains(Actor actor)
        {
            return _actors.TryGetValue(actor.Position, out var actorAtPos) && actorAtPos.Equals(actor);
        }

        public void Clear()
        {
            foreach (var actor in _actors.Values)
            {
                _ = Remove(actor);
            }
        }

        public void UpdateVisibility(IFOV fieldOfView = null)
        {
            var fov = fieldOfView ?? ScreenContainer.Instance.World.Player.FieldOfView;
            foreach (var actor in _actors)
            {
                actor.Value.IsVisible = fov.BooleanResultView[actor.Key];
            }
        }

        private void UpdateActorPositionWithinManager(object sender, ValueChangedEventArgs<Point> e)
        {
            if (e.OldValue == e.NewValue) return;
            var actor = (Actor)sender;

            // Remove from previous
            _actors.Remove(e.OldValue);

            // Check if the new position is occupied
            if (ExistsAt(e.NewValue))
            {
                throw new Exception($"Cannot move actor to {e.NewValue} another actor already exists there.");
            }

            _actors.Add(e.NewValue, actor);
        }
    }
}
