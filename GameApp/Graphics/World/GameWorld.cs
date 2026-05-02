using GameApp.Utils;
using LearningOpenTK.Core.Lifecycle;
using LearningOpenTK.Core.Rendering;
using LearningOpenTK.Engine.Resources.Shaders;
using LearningOpenTK.Engine.Resources.Textures.Array;
using LearningOpenTK.Engine.World;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Chunks;
using VoxelWorldEngine.Core.Serialization;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;

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

    private readonly Dictionary<Vector3Int, WorldObject<ChunkMesh>> _chunks = new();
    private readonly GameWorldMaterial _material;
    private readonly VoxelWorld _voxelWorld;

    private int _previousChunksWithMesh = 0;
    private float[] _tileOffsets;

    // TODO: temp repository usage here
    public GameWorld(VoxelWorld voxelWorld, GameWorldMaterial material, IChunkMementoRepository chunkRepository)
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

            var model = chunk.Value.Transform3D.GetModelMatrix();
            _material.Shader.SetMatrix4("model", model);

            var normalMatrix = chunk.Value.Transform3D.GetCubeNormalMatrix();
            _material.Shader.SetMatrix3("normalMatrix", normalMatrix);


            chunk.Value.Mesh.Render();
        }
    }

    public void AddChunk(Chunk chunk)
    {
        if (chunk.ChunkMesh.Vertices.Length == 0)
        {
        }

        // TODO: make proxy from no empty objects
        var wo = ConvertToWorldObject(chunk);
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

    // TODO: temporary?????
    private WorldObject<ChunkMesh> ConvertToWorldObject(Chunk chunk)
    {
        ChunkMesh? mesh = null;
        if (chunk.ChunkMesh.Vertices.Length > 0)
        {
            mesh = new ChunkMesh(chunk.ChunkMesh.Vertices, chunk.ChunkMesh.Indices);
        }
        // var mesh = new ChunkMesh(chunk.ChunkMesh.Vertices, chunk.ChunkMesh.Indices);

        var obj = new WorldObject<ChunkMesh>(mesh, _material.Shader, _material.TextureArray);

        obj.Transform3D.Position = chunk.GlobalPosition.ToVector3();
        return obj;
    }


    public void Update(float deltaTime)
    {
        //
    }
}