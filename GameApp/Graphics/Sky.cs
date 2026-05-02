using LearningOpenTK.Core.Lifecycle;
using LearningOpenTK.Core.Rendering;
using LearningOpenTK.Engine.Resources.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameApp.Content;

public class Sky : OnceLoadable
{
    private readonly IShader _shader;
    private readonly Vector3 _skyBottom;

    private readonly Vector3 _skyTop;
    private int _vao;


    public Sky(IShader shader, Vector3 skyTop, Vector3 skyBottom)
    {
        _shader = shader;
        _skyTop = skyTop;
        _skyBottom = skyBottom;
    }

    protected override void Load()
    {
        _vao = GL.GenVertexArray();

        _shader.Use();
        _shader.SetVector3("uSkyTop", _skyTop);
        _shader.SetVector3("uSkyBottom", _skyBottom);
    }

    public void Render(RenderContext context)
    {
        GL.DepthMask(false);


        _shader.Use();
        var skyView = new Matrix4(new Matrix3(context.ViewMatrix));
        var invViewProj = (skyView * context.ProjectionMatrix3D).Inverted();
        _shader.SetMatrix4("uInvViewProj", invViewProj);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        GL.DepthMask(true);
    }

    protected override void ReleaseUnmanagedResources()
    {
        if (_vao != 0)
        {
            GL.DeleteVertexArray(_vao);
            _vao = 0;
        }
    }
}