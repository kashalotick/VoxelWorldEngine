using GameApp.Utils;
using LearningOpenTK.Core.Lifecycle;
using LearningOpenTK.Core.Rendering;
using LearningOpenTK.Core.Transform;
using LearningOpenTK.Engine.Resources.Shaders;
using LearningOpenTK.Engine.Resources.Textures.Array;
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
    private readonly IChunkMementoRepository _chunkRepository;

    private readonly Dictionary<Vector3Int, ChunkObject> _chunks = new();
    private readonly GameWorldMaterial _material;
    private readonly VoxelWorld _voxelWorld;

    private int _previousChunksWithMesh = 0;
    private float[] _tileOffsets;

    // TODO: temp repository usage here
    public GameWorld(
        VoxelWorld voxelWorld,
        GameWorldMaterial material,
        IChunkMementoRepository chunkRepository
    )
    {
        _voxelWorld = voxelWorld;
        _material = material;
        _chunkRepository = chunkRepository;
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
            var chunk = _voxelWorld.Chunks[pair.Key];

            if (chunk.IsDirty)
            {
                _chunkRepository.Save(chunk.Save());
            }


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

        var chunks = _chunks.Count;

        foreach (var chunk in _chunks)
        {
            if (chunk.Value.Mesh == null) continue;

            if (_voxelWorld.Chunks.TryGetValue(chunk.Key, out var chunkData))
            {
                var globalPos = chunkData.GlobalPosition.ToVector3();
                if (!context.IsInFrustum(globalPos, globalPos + new Vector3(Chunk.ChunkSize)))
                    continue;
            }

            var model = chunk.Value.Transform.GetModelMatrix();
            _material.Shader.SetMatrix4("model", model);

            var normalMatrix = chunk.Value.Transform.GetCubeNormalMatrix();
            _material.Shader.SetMatrix3("normalMatrix", normalMatrix);


            chunk.Value.Mesh.Render();
        }
    }

    public void AddChunk(Chunk chunk)
    {
        if (chunk.ChunkMesh.Vertices.Length == 0) { }

        // TODO: make proxy from no empty objects
        var wo = new ChunkObject(chunk);
        _chunks[chunk.Position] = wo;
        if (wo.Mesh != null)
        {
            wo.Load();
        }
    }

    public void UpdateChunk(Chunk chunk)
    {
        if (!_chunks.ContainsKey(chunk.Position))
        {
            AddChunk(chunk);
        }

        var wo = _chunks[chunk.Position];
        if (wo.Mesh != null)
        {
            wo.Mesh.UpdateVertices(chunk.ChunkMesh.Vertices);
            wo.Mesh.UpdateIndices(chunk.ChunkMesh.Indices);
        }
        else
        {
            AddChunk(chunk);
        }
    }

    public void RemoveChunk(Chunk chunk)
    {
        if (chunk.IsDirty)
        {
            _chunkRepository.Save(chunk.Save());
        }

        _chunks[chunk.Position].Dispose();
        _chunks.Remove(chunk.Position);
    }
    
    
    private class ChunkObject : ILoadable
    {
        public ChunkMesh Mesh;
        public Transform3D Transform;

        public ChunkObject(Chunk chunk)
        {
            Mesh = new ChunkMesh(chunk.ChunkMesh.Vertices, chunk.ChunkMesh.Indices);
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
