using GoRogue.Pathing;
using Roguelike.Entities;
using Roguelike.Entities.Actors;
using Roguelike.Screens;
using SadRogue.Primitives;
using System.Linq;

namespace Roguelike.Logic
{
    internal static class GameLogic
    {
        private static Player Player => ScreenContainer.Instance.World.Player;
        private static ActorManager ActorManager => ScreenContainer.Instance.World.ActorManager;
        private static FastAStar Pathfinder => ScreenContainer.Instance.World.Pathfinder;

        /// <summary>
        /// A tick is executed once the player attempts to move to the target position.
        /// <br>This doesn't mean the movement was succesful. The real position can be retrieved from the Player's Position property.</br>
        /// </summary>
        /// <param name="intendedPosition">The position the player was trying to move to on this tick.</param>
        internal static void Tick(Point intendedPosition)
        {
            if (HandleStairs(intendedPosition))
                return;

            HandlePathfindingAndCombat(intendedPosition);
        }

        private static bool HandleStairs(Point intendedPosition)
        {
            var hasMoved = Player.Position == intendedPosition;
            if (!hasMoved) return false;

            // Check if we moved onto a stairs down tile
            var tile = ScreenContainer.Instance.World.Tilemap[intendedPosition.ToIndex(ScreenContainer.Instance.World.Tilemap.Width)];
            if (tile.Type == World.TileType.StairsDown)
            {
                // Generate a new world / dungeon when going stairs down
                ScreenContainer.Instance.World.Generate();
                return true;
            }
            return false;
        }

        private static void HandlePathfindingAndCombat(Point intendedPosition)
        {
            var hasMoved = Player.Position == intendedPosition;
            var npcAtIntendedPosition = ActorManager.Get(intendedPosition);

            if (!hasMoved && npcAtIntendedPosition != null)
            {
                // The player didn't move but an actor is at the intended position, so we attempted to move into the actor
                // This counts as an attack from the player to the actor
                MeleeCombatLogic.Attack(Player, npcAtIntendedPosition);
            }

            // Calculate for each actor in the player's FOV, to move one tile towards the player.
            // If any actor is next to the player and they attempt to move onto the player, the actor will attack the player.
            var npcsInFov = Player.FieldOfView.CurrentFOV
                .Where(ActorManager.ExistsAt)
                .Select(ActorManager.Get)
                .Where(a => a != Player)
                .ToArray();

            foreach (var npcInFov in npcsInFov)
            {
                var shortestPath = Pathfinder.ShortestPath(npcInFov.Position, Player.Position);
                if (shortestPath == null || shortestPath.Length == 0) continue;

                // Move npc towards the player
                var nextStep = shortestPath.GetStep(0);

                if (!npcInFov.Move(nextStep.X, nextStep.Y))
                {
                    // Check if npc ran into the player
                    if (nextStep == Player.Position)
                        MeleeCombatLogic.Attack(npcInFov, Player);
                }
            }
        }
    }
}
