using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Displays;

namespace Meadow.Foundation.MyExtensions
{

    // Extend the GraphicsLibrary 
    public class GraphicsLibraryEx : MicroGraphics
    {
        private new readonly IPixelDisplay display;
        public GraphicsLibraryEx(IPixelDisplay display) : base(display)
        {
            this.display = display;
            Rotation = RotationType._180Degrees;
            base.CurrentFont = new Font12x20();
        }

        public void DrawBigCenteredText(string text, Color color, bool clear = true, bool show = true)
        {
            if (clear)
                Clear(true);

            ScaleFactor big = ScaleFactor.X3;
            DrawText(((int)display.Width - CurrentFont.Width * text.Length * (int)big) / 2,
                      ((int)display.Height - CurrentFont.Height * (int)big) / 2,
                      text, color, big);
            if (show)
                Show();
        }

        public void DrawBitmap(int x, int y, int width, int height, byte[] bitmap, Color color)
        {
            base.DrawBitmap(x, y, width, height, bitmap, color);
        }
    }
}