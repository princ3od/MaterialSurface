using System.Drawing;
using System.Drawing.Drawing2D;

namespace MaterialSurface
{
    class GraphicHelper
    {
        public static void DrawSymbolMark(Graphics graphics, Pen pen, float x, float y, float width, float height, int padding = 0)
        {
            graphics.DrawLine(pen, padding + x, height / 2, width / 2 - width / 12, height - padding);
            graphics.DrawLine(pen, width / 2 - width / 12, height - padding, x + width - padding, y + padding);
        }
        public static void DrawSymbolX(Graphics graphics, Pen pen, float x, float y, float width, float height, int padding = 0)
        {
            graphics.DrawLine(pen, padding + x, padding + y, x + width - padding, y + height - padding - 1);
            graphics.DrawLine(pen, x + width - padding - 1, y + padding, padding + x, y + height - padding);

        }
        public static GraphicsPath GetPillShape(float x, float y, float width, float height, int radius)
        {
            if (radius <= 0)
                radius = 1;
            if (radius * 2 > width)
                radius = (int)width / 2;
            if (radius * 2 > height)
                radius = (int)height / 2;
            GraphicsPath graphics = new GraphicsPath();

            graphics.AddArc(x, y, radius * 2, height, 270, -180);
            graphics.AddArc(x + width - radius * 2, y, radius * 2, height, -270, -180);

            graphics.CloseFigure();
            return graphics;
        }
        public static GraphicsPath GetRoundedRectangle(float x, float y, float width, float height, int radius)
        {
            if (radius <= 0)
                radius = 1;
            if (radius * 2 > width)
                radius = (int)width / 2;
            if (radius * 2 > height)
                radius = (int)height / 2;
            GraphicsPath graphics = new GraphicsPath();

            graphics.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90);

            graphics.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90);

            graphics.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90);

            graphics.AddArc(x, y, radius * 2, radius * 2, 180, 90);

            graphics.CloseFigure();
            return graphics;
        }
        public static GraphicsPath GetRoundedRectangle(Rectangle rectangle, int radius)
        {
            float x = rectangle.X, y = rectangle.Y, width = rectangle.Width, height = rectangle.Height;
            if (radius * 2 > width)
                radius = (int)width / 2;
            if (radius * 2 > height)
                radius = (int)height / 2;
            GraphicsPath graphics = new GraphicsPath();

            graphics.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90);

            graphics.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90);

            graphics.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90);

            graphics.AddArc(x, y, radius * 2, radius * 2, 180, 90);

            graphics.CloseFigure();
            return graphics;
        }
    }
}
