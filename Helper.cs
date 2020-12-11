using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Material_Design_for_Winform
{
    class Helper
    {
        public static GraphicsPath drawRoundedRec(float x, float y, float width, float height, int round)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(x + round, y, x + width - (round * 2), y); //Vẽ khung
            gp.AddArc(x + width - (round * 2), y, round * 2, round * 2, 270, 90); //Bo tròn góc

            gp.AddLine(x + width, y + round, x + width, y + height - (round * 2));
            gp.AddArc(x + width - (round * 2), y + height - (round * 2), round * 2, round * 2, 0, 90);

            gp.AddLine(x + width - (round * 2), y + height, x + round, y + height);
            gp.AddArc(x, y + height - (round * 2), round * 2, round * 2, 90, 90);

            gp.AddLine(x, y + height - (round * 2), x, y + round);
            gp.AddArc(x, y, round * 2, round * 2, 180, 90);

            gp.CloseFigure();
            return gp;
        }
        public static int Round(decimal number)
        {
            return (int)Math.Round(number, 0, MidpointRounding.AwayFromZero);
        }
        
    }
}
