using System.Numerics;
using LearningOpenTK.Components;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Interfaces;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Meshes;
using LearningOpenTK.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace GameApp.Content;

// TODO: inherit some scene collection class idk (common for scene)

public class GameWorld : IGameEntry
{
    private IShader _worldShader;
    private ITexture _worldTexture;
    
    private Dictionary<Vector3Int, WorldObject> _chunks = new();

    public GameWorld(IShader shader, ITexture texture)
    {
        _worldShader = shader;
        _worldTexture = texture;
    }

    public void AddChunk(Chunk chunk)
    {
        if (chunk.Mesh.Vertices.Count == 0)
        {
        }
        // TODO: make proxy from no empty objects
        var wo = ConvertToWorldObject(chunk);
        _chunks[chunk.Position] = wo;
        wo.Load();
        
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
        var mesh = new ChunkMesh(chunk.Mesh.Vertices.ToArray(), chunk.Mesh.Indices.ToArray());
        var obj = new WorldObject(mesh, _worldShader, _worldTexture);

        obj.Transform.Position = (Vector3)(System.Numerics.Vector3)chunk.GlobalPosition;
        return obj;
    }


    public void Update(float deltaTime)
    {
      //
    }

    public void Render(RenderContext context)
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        
        _worldTexture.Use(TextureUnit.Texture0);
        _worldShader.Use();

        _worldShader.SetMatrix4("view", context.ViewMatrix);
        _worldShader.SetMatrix4("projection", context.ProjectionMatrix3D);
        _worldShader.SetVector3("viewPos", context.CameraPosition);

        
        foreach (var chunk in _chunks.Values)
        {
            var model = chunk.Transform.GetModelMatrix();
            _worldShader.SetMatrix4("model", model);
            var normalMatrix = new Matrix3(chunk.Transform.GetModelMatrix());
            normalMatrix = normalMatrix.Inverted();
            normalMatrix = normalMatrix.Transposed();
            _worldShader.SetMatrix3("normalMatrix", normalMatrix);

            
            chunk.Mesh.Render();
            // chunk.Render(context);
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