using GameApp.Utils;
using GameEngine.Core.Lifecycle;
using GameEngine.Core.Rendering;
using GameEngine.Core.Transform;
using GameEngine.Engine.Resources.Shaders;
using GameEngine.Engine.Resources.Textures.Array;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelModule.Core;
using VoxelModule.Core.Chunks;
using VoxelModule.Core.Serialization;
using VoxelModule.DataStructures.Common.Structures.Vectors;

namespace GameApp.Graphics.World;

// TODO: inherit some scene collection class idk (common for scene)
public record GameWorldMaterial(
    IShader Shader,
    ITextureArray TextureArray,
    float TileScale
);

public class GameWorld : ILoadable, IRenderable
{
    private readonly Dictionary<Vector3Int, ChunkObject> _chunks = new();
    private readonly GameWorldMaterial _material;

    private int _previousChunksWithMesh = 0;
    private float[] _tileOffsets;

    // TODO: temp repository usage here
    public GameWorld(GameWorldMaterial material)
    {
        _material = material;
    }

    public void Load()
    {
        _material.Shader.Use();

        foreach (var chunk in _chunks.Values)
        {
            chunk.Load();
        }
    }

    public void Dispose()
    {
        foreach (var pair in _chunks)
        {
            pair.Value.Dispose();
        }
    }

    public void Render(RenderContext context)
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);

        _material.TextureArray.Use(TextureUnit.Texture0);
        _material.Shader.Use();
        _material.Shader.SetMatrix4("view", context.ViewMatrix);
        _material.Shader.SetMatrix4("projection", context.ProjectionMatrix3D);
        _material.Shader.SetVector3("viewPos", context.CameraPosition);
        _material.Shader.SetFloat("tileScale", _material.TileScale);


        foreach (var chunk in _chunks)
        {
            if (chunk.Value.Mesh == null) continue;


            var globalPos = chunk.Value.Transform.Position;
            if (!context.IsInFrustum(globalPos, globalPos + new Vector3(Chunk.ChunkSize)))
                continue;


            var model = chunk.Value.Transform.GetModelMatrix();
            _material.Shader.SetMatrix4("model", model);

            var normalMatrix = chunk.Value.Transform.GetCubeNormalMatrix();
            _material.Shader.SetMatrix3("normalMatrix", normalMatrix);


            chunk.Value.Mesh.Render();
        }
    }

    public void OnAddChunk(Chunk chunk)
    {
        var wo = new ChunkObject(chunk);
        _chunks[chunk.Position] = wo;
        if (wo.Mesh != null)
        {
            wo.Load();
        }
    }

    public void OnUpdateChunk(Chunk chunk)
    {
        if (!_chunks.ContainsKey(chunk.Position))
        {
            OnAddChunk(chunk);
        }

        var wo = _chunks[chunk.Position];
        if (wo.Mesh != null)
        {
            wo.Mesh.UpdateVertices(chunk.ChunkMesh.Vertices);
            wo.Mesh.UpdateIndices(chunk.ChunkMesh.Indices);
        }
        else
        {
            OnAddChunk(chunk);
        }
    }

    public void OnRemoveChunk(Chunk chunk)
    {
        _chunks[chunk.Position].Dispose();
        _chunks.Remove(chunk.Position);
    }


    private class ChunkObject : ILoadable
    {
        public ChunkMesh? Mesh;
        public Transform3D Transform;

        public ChunkObject(Chunk chunk)
        {
            if (chunk.ChunkMesh.Vertices.Length == 0)
            {
                Mesh = null;
            }
            else
            {
                Mesh = new ChunkMesh(chunk.ChunkMesh.Vertices, chunk.ChunkMesh.Indices);
            }

            Transform = new Transform3D()
            {
                Position = chunk.GlobalPosition.ToVector3(),
            };
        }

        public void Load()
        {
            if (Mesh == null)
            {
                Console.WriteLine("! Warning: Try to load null mesh");
                return;
            }

            Mesh.Load(BufferUsageHint.DynamicDraw);
        }

        public void Dispose()
        {
            Mesh?.Dispose();
        }
    }
}
