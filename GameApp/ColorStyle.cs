using OpenTK.Mathematics;

namespace GameApp;

public static class ColorStyle
{
    public static readonly Vector3 Background = new Vector3(0.071f, 0.063f, 0.059f);
    
    public static readonly Vector3 White = new (1);
    public static readonly Vector3 Gray = new (0.5f);
    public static readonly Vector3 Black = new (0);

    public static readonly Vector3 Red = new (0.937f, 0.259f, 0.259f);
    public static readonly Vector3 RedLight = new  (1.0f, 0.435f, 0.435f);
    
    public static readonly Vector3 GreenLight = new (0.690f, 0.984f, 0.612f);
    
    public static readonly Vector3 Yellow =new (1.0f, 0.992f, 0.569f);
}