namespace Roguelike
{
    internal static class Extensions
    {
        /// <summary>
        /// Calculates the specified percentage of the given integer value.
        /// </summary>
        /// <param name="value">The base value from which the percentage will be calculated.</param>
        /// <param name="percentage">The percentage to calculate from the base value.</param>
        /// <returns>The calculated percentage of the base value as an integer.</returns>
        internal static int Percent(this int value, int percentage)
        {
            return (int)(value / (float)100 * percentage);
        }
    }
}
