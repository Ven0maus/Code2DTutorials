using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace Roguelike.Screens
{
    internal class MessagesScreen : ScreenSurface
    {
        private readonly List<string> _messages = [];
        private readonly ScreenSurface _messageSurface;

        public MessagesScreen(int width, int height) : base(width, height)
        {
            // We will use a surface with a smaller width font, so we have more room for showing text

            // Since our parent is 16x16 and the IBM font is 8x16 we must translate the width properly to match the original size
            int translatedWidth = (width - 1) * 2; // *2 to match the original 16x16 -1 to exclude borders left/right
            int translatedHeight = height - 2; // Exclude 2 to substract the top/bottom borders
            _messageSurface = new ScreenSurface(translatedWidth, translatedHeight)
            {
                Font = Game.Instance.Fonts["IBM_8x16"], // Use default sadconsole font
                Position = new Point(1, 1) // Position within the border
            };

            // You can use this so see if the surface fits within the screen
            //_messageSurface.Fill(background: Color.Blue);

            // Add to children so the parent can handle its rendering
            Children.Add(_messageSurface);

            // Draw the main border around the message screen
            Surface.DrawBorderWithTitle("Messages", Color.Gray, Color.Magenta);
        }

        public void AddMessage(string message)
        {
            // Remove the oldest message if we arrived at our limit
            if (_messages.Count == _messageSurface.Height - 2)
                _messages.RemoveAt(0);

            _messages.Add(message);

            // Re-draw
            DrawMessages();
        }

        public static void WriteLine(string message)
        {
            // Static shortcut
            ScreenContainer.Instance.Messages.AddMessage(message);
        }

        private void DrawMessages()
        {
            _messageSurface.Surface.Clear();

            // Print the "oldest" message at the top, newest at the bottom
            var startPos = new Point(2, 1);
            for (int i=0; i < _messages.Count; i++)
            {
                // Print the message with the given color
                var message = _messages[i];
                _messageSurface.Surface.Print(startPos.X, startPos.Y, message);
                startPos += Direction.Down;
            }
        }
    }
}
