using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

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
        internal static int PercentageOf(this int value, int percentage)
        {
            return (int)Math.Round(value * percentage / 100.0);
        }

        /// <summary>
        /// Gets the neighboring points based on the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="includeDiagonals"></param>
        /// <returns></returns>
        internal static IEnumerable<Point> GetNeighborPoints(this Point point, bool includeDiagonals)
        {
            return (includeDiagonals ? _diagonalDirections : _directionalDirections).Select(d => new Point(point.X + d.X, point.Y + d.Y));
        }

        private static readonly Point[] _directionalDirections =
        [
            new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)
        ];

        private static readonly Point[] _diagonalDirections =
        [
            new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1),
            new Point(-1, -1), new Point(-1, 1), new Point(1, -1), new Point(1, 1)
        ];

        internal static void DrawBorderWithTitle(this ICellSurface surface, string title, Color borderColor, Color titleColor)
        {
            // Draw borders
            var shapeParams = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThick, new ColoredGlyph(borderColor), ignoreBorderBackground: true);
            surface.DrawBox(new Rectangle(0, 0, surface.Width, surface.Height), shapeParams);

            // Print title
            surface.Print(surface.Width / 2 - title.Length / 2, 0, new ColoredString(title, titleColor, Color.Transparent));
        }
    }
}
