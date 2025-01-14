using Roguelike.Entities;
using Roguelike.Screens;
using System;

namespace Roguelike.Logic
{
    internal static class MeleeCombatLogic
    {
        internal static void Attack(Actor attacker, Actor defender)
        {
            var damage = CalculateDamage(attacker.Stats, defender.Stats);

            if (damage > 0)
            {
                defender.ApplyDamage(damage);
                System.Console.WriteLine($"{attacker.Name} has attacked {defender.Name} for {damage} damage.");
            }
            else
            {
                System.Console.WriteLine("The attack was dodged!");
            }
        }

        internal static int CalculateDamage(ActorStats attacker, ActorStats defender)
        {
            var random = ScreenContainer.Instance.Random;

            // Dodge chance
            if (random.Next(0, 100) < defender.DodgeChance)
            {
                return 0; // No damage dealt
            }

            // Critical Hit Check
            bool isCriticalHit = random.Next(0, 100) < attacker.CritChance;
            if (isCriticalHit)
                System.Console.WriteLine("Critical hit!");

            float critMultiplier = isCriticalHit ? 1.5f : 1.0f;

            // Base Damage (Proportional Scaling with randomness)
            int baseDamage = (int)Math.Round((float)attacker.Attack * attacker.Attack / (attacker.Attack + defender.Defense));
            baseDamage = random.Next((int)Math.Floor(baseDamage * 0.85), (int)Math.Ceiling(baseDamage * 1.15)); // Add 15% variance
            baseDamage = (int)Math.Round(baseDamage * critMultiplier); // Apply critical hit multiplier

            return Math.Max(1, baseDamage);
        }
    }
}
