using LearningOpenTK.Resources;

namespace GameApp;

public class ResourceRepository : IDisposable
{
    public Shader ShaderShader { get; }
    public Shader InterfaceShader { get; }
    public Texture DiamondTexture { get; }
    public Shader ChunkShader { get; }

    public ResourceRepository()
    {
        ShaderShader = new Shader("shader");
        InterfaceShader = new Shader("interface");
        DiamondTexture = new Texture("Diamond.png");
        ChunkShader = new Shader("chunk");
    }

    public void Load()
    {
        ShaderShader.Load();
        ShaderShader.Use();
        ShaderShader.SetInt("texture0", 0);

        InterfaceShader.Load();
        ChunkShader.Load();
        ChunkShader.Use();
        ChunkShader.SetInt("texture0", 0);
        DiamondTexture.Load();
    }


    public void Dispose()
    {
        ShaderShader.Dispose();
        InterfaceShader.Dispose();
        DiamondTexture.Dispose();
        ChunkShader.Dispose();
    }
}