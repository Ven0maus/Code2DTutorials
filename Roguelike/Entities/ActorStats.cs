using Roguelike.Entities.Actors;
using Roguelike.Screens;
using System;

namespace Roguelike.Entities
{
    internal sealed class ActorStats
    {
        private int _health = 1;
        public int Health
        {
            get => _health;
            set => _health = Math.Max(0, Math.Min(value, MaxHealth));
        }

        public int MaxHealth { get; private set; }
        public int Attack { get; private set; } = 1;
        public int Defense { get; private set; } = 0;
        public int DodgeChance { get; private set; } = 0;
        public int CritChance { get; private set; } = 0;

        public Actor Parent { get; }

        public ActorStats(Actor actor, int maxHealth)
        {
            Parent = actor;
            MaxHealth = Math.Max(1, maxHealth);
            Health = MaxHealth;
        }

        public void Set(int? atk = null, int? def = null, int? dodge = null, int? crit = null)
        {
            Attack = Math.Max(1, atk ?? Attack);
            Defense = Math.Max(0, def ?? Defense);
            DodgeChance = Math.Max(0, Math.Min(dodge ?? DodgeChance, 50)); // 50% max dodge chance
            CritChance = Math.Max(0, crit ?? CritChance);

            if (Parent is Player)
                ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }
    }
}
