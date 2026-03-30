using LearningOpenTK.Core.DTO;
using LearningOpenTK.Core.Primitives;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Resources.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;

namespace GameApp.Content;

// TODO: inherit some scene collection class idk (common for scene)

public class GameWorld : ILoadable, IRenderable
{
    private IShader _worldShader;
    private ITexture _worldTexture;
    private World _world;

    private Dictionary<Vector3Int, WorldObject> _chunks = new();

    public GameWorld(World world, IShader shader, ITexture texture)
    {
        _world = world;
        _worldShader = shader;
        _worldTexture = texture;
    }

    public void AddChunk(Chunk chunk)
    {
        if (chunk.Mesh.Vertices.Length == 0)
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
        throw new NotImplementedException();
    }

    public void RemoveChunk(Vector3Int chunkPosition)
    {
        _chunks[chunkPosition].Dispose();
        _chunks.Remove(chunkPosition);
    }

    // TODO: temporary?????
    private WorldObject ConvertToWorldObject(Chunk chunk)
    {
        ChunkMesh? mesh = null;
        if (chunk.Mesh.Vertices.Length > 0)
        {
            mesh = new ChunkMesh(chunk.Mesh.Vertices, chunk.Mesh.Indices);
        }
        // var mesh = new ChunkMesh(chunk.Mesh.Vertices, chunk.Mesh.Indices);

        var obj = new WorldObject(mesh, _worldShader, _worldTexture);

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

        _worldTexture.Use(TextureUnit.Texture0);
        _worldShader.Use();

        _worldShader.SetMatrix4("view", context.ViewMatrix);
        _worldShader.SetMatrix4("projection", context.ProjectionMatrix3D);
        _worldShader.SetVector3("viewPos", context.CameraPosition);

        var chunks = _chunks.Count;

        foreach (var chunk in _chunks)
        {
            if (chunk.Value.Mesh == null) continue;
            
            if (_world.Chunks.TryGetValue(chunk.Key, out var chunkData))
            {
                var globalPos = (Vector3)(System.Numerics.Vector3)chunkData.GlobalPosition; // TODO
                if (!context.IsInFrustum(globalPos, globalPos + new Vector3(Chunk.ChunkSize)))
                    continue;
            }

            var model = chunk.Value.Transform3D.GetModelMatrix();
            _worldShader.SetMatrix4("model", model);
            
            var normalMatrix = chunk.Value.Transform3D.GetCubeNormalMatrix(); // TODO: make caching
            _worldShader.SetMatrix3("normalMatrix", normalMatrix);


            chunk.Value.Mesh.Render();
        }
    }

    public void Load()
    {
        foreach (var chunk in _chunks.Values)
        {
            chunk.Load();
        }
    }

    public void Dispose()
    {
        foreach (var chunk in _chunks.Values)
        {
            chunk.Dispose();
        }
    }
}