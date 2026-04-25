using System.Runtime.InteropServices;
using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Engine.Resources.Textures;
using LearningOpenTK.Engine.Resources.Textures.Array;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Serialization;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace GameApp.Content;

// TODO: inherit some scene collection class idk (common for scene)
public record GameWorldMaterial(
    IShader Shader,
    ITextureArray TextureArray,
    float TileScale
);

public class GameWorld : ILoadable, IRenderable
{
    private GameWorldMaterial _material;
    private float[] _tileOffsets;
    private VoxelWorld _voxelWorld;
    private IChunkMementoRepository _chunkRepository;

    private Dictionary<Vector3Int, WorldObject<ChunkMesh>> _chunks = new();

    // TODO: temp repository usage here
    public GameWorld(VoxelWorld voxelWorld, GameWorldMaterial material, IChunkMementoRepository chunkRepository)
    {
        _voxelWorld = voxelWorld;
        _material = material;
        _chunkRepository = chunkRepository;
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

        obj.Transform3D.Position = (Vector3)(System.Numerics.Vector3)chunk.GlobalPosition;
        return obj;
    }


    public void Update(float deltaTime)
    {
        //
    }

    private int _previousChunksWithMesh = 0;

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
                var globalPos = (Vector3)(System.Numerics.Vector3)chunkData.GlobalPosition; // TODO
                if (!context.IsInFrustum(globalPos, globalPos + new Vector3(Chunk.ChunkSize)))
                    continue;
            }

            var model = chunk.Value.Transform3D.GetModelMatrix();
            _material.Shader.SetMatrix4("model", model);

            var normalMatrix = chunk.Value.Transform3D.GetCubeNormalMatrix(); // TODO: make caching
            _material.Shader.SetMatrix3("normalMatrix", normalMatrix);


            chunk.Value.Mesh.Render();
        }
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
}