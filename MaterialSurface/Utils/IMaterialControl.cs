using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialSurface
{
    /// <summary>
    /// Control interface
    /// </summary>
    public interface IMaterialControl
    {
        MouseState MouseState { get; set; }

    }
    public class ColorConstant
    {
        public static Color DarkThemeBackgroundColor = Color.FromArgb(64, 64, 64);
    }
    public enum MouseState
    {
        HOVER,
        DOWN,
        OUT,
        UP
    }
    public enum ET
    {
        Light,
        Dark,
        Custom
    }
    public enum BoxType
    {
        Normal,
        Outlined,
        Filled
    };

}
