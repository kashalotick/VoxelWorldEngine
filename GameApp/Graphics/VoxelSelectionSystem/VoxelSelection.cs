using GameApp.Utils;
using GameEngine.Core.Lifecycle;
using GameEngine.Core.Rendering;
using GameEngine.Core.Transform;
using GameEngine.Engine.Resources.Shaders;
using OpenTK.Graphics.OpenGL4;
using VoxelModule.DataStructures.Common.Structures.Vectors;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace GameApp.Graphics.VoxelSelectionSystem;

public sealed class VoxelSelection : ILoadable, IRenderable
{
    private readonly Vector3 _color;
    private readonly IShader _shader;
    private readonly Transform3D _transform3D;
    private CubeEdgeMesh _mesh;
    private Vector3Int? _voxelPosition;

    public VoxelSelection(IShader shader)
    {
        _shader = shader;
        _transform3D = new Transform3D
        {
            Scale = Vector3.One * 1.01f
        };
        _color = new Vector3(1, 1, 1);
    }

    public void Load()
    {
        _mesh = new CubeEdgeMesh((System.Numerics.Vector3)_color);
        _mesh.Load(BufferUsageHint.StaticDraw);
    }

    public void Dispose()
    {
        _mesh.Dispose();
    }

    public void Render(RenderContext context)
    {
        GL.Enable(EnableCap.DepthTest);
        if (_voxelPosition == null) return;
        _shader.Use();

        var viewProjection = _transform3D.GetModelMatrix() * context.ViewMatrix * context.ProjectionMatrix3D;
        _shader.SetMatrix4("uViewProjection", viewProjection);
        _mesh.Render();
    }

    public void SetVoxelPosition(Vector3Int? voxelPosition)
    {
        _voxelPosition = voxelPosition;
        if (voxelPosition.HasValue)
        {
            var scaleOffset = Vector3.One * 0.005f;
            _transform3D.Position = voxelPosition.Value.ToVector3() - scaleOffset;
        }
    }
}