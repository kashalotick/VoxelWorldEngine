using LearningOpenTK.Resources;

namespace GameApp;

public class ResourceRepository
{
    public Shader ShaderShader { get; }
    public Shader InterfaceShader { get; }
    public Texture DiamondTexture { get; }

    public ResourceRepository()
    {
        ShaderShader = new Shader("shader");
        InterfaceShader = new Shader("interface");
        DiamondTexture = new Texture("Diamond.png");
    }

    public void Load()
    {
        ShaderShader.Load();
        InterfaceShader.Load();
        DiamondTexture.Load();
    }
}