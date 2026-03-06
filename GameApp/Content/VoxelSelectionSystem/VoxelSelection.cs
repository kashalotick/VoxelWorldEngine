using System.Drawing;
using System.Numerics;
using LearningOpenTK.Core.Components;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Graphics.OpenGL4;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace GameApp.Content.VoxelSelectionSystem;

public sealed class VoxelSelection : ILoadable, IRenderable
{
    private IShader _shader;
    private Vector3Int? _voxelPosition;
    private Transform3D _transform3D;
    private Vector3 _color;
    private CubeEdgeMesh _mesh;

    public VoxelSelection(IShader shader)
    {
        _shader = shader;
        _transform3D = new Transform3D
        {
            Scale = Vector3.One * 1.01f,
        };
        _color = new Vector3(1, 1, 1);
    }

    public void Load()
    {
        _mesh = new CubeEdgeMesh((System.Numerics.Vector3)_color);
        _mesh.Load(BufferUsageHint.StaticDraw);
    }

    public void SetVoxelPosition(Vector3Int? voxelPosition)
    {
        _voxelPosition = voxelPosition;
        if (voxelPosition != null)
        {
            var scaleOffset = Vector3.One * 0.005f;
            _transform3D.Position = (Vector3)(System.Numerics.Vector3)voxelPosition - scaleOffset;
            

        }
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

    public void Dispose()
    {
        _mesh.Dispose();
    }

}