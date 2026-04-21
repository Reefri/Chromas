using Godot;

namespace Com.IsartDigital.Chromaberation
{
    public partial class ColorProp
    {
        public static Color NORMAL_COLOR = new Color(1, 1, 1);
        public static Color COLORBLIND_COLOR = new Color(0.5f, 0.7f, 1);
        public static Color CORRECTEDCOLORBLIND_COLOR = new Color(0.7f, 0.5f, 0.35f) / 0.7f;
    }
}
