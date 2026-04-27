using OpenTK.Mathematics;

namespace GameApp.Utils;

public static class ColorStyle
{
    public static readonly Vector4 PauseBackground = new(0, 0, 0, 0.7f);
    public static readonly Vector4 Transparent = new(0);


    public static readonly Vector4 Background = new(0.063f, 0.063f, 0.063f, 1);

    public static readonly Vector4 White = new(1);
    public static readonly Vector4 Gray = new(0.5f, 0.5f, 0.5f, 1);
    public static readonly Vector4 Black = new(0, 0, 0, 1);

    public static readonly Vector4 Red = new(0.937f, 0.259f, 0.259f, 1);
    public static readonly Vector4 RedLight = new(1.0f, 0.435f, 0.435f, 1);

    public static readonly Vector4 GreenLight = new(0.690f, 0.984f, 0.612f, 1);

    public static readonly Vector4 Yellow = new(1.0f, 0.992f, 0.569f, 1);

    public static class Field
    {
        public static readonly Vector4 Background = new(0.027f, 0.027f, 0.027f, 1);
        public static readonly Vector4 BackgroundHover = new(0.1f, 0.1f, 0.1f, 1);
        public static readonly Vector4 BackgroundFocus = new(0.2f, 0.2f, 0.2f, 1);

        public static readonly Vector4 Text = White;

        public static readonly Vector4 Placeholder = new(0.2f, 0.2f, 0.2f, 1);
    }
}