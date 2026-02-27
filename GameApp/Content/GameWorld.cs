using System.Numerics;
using LearningOpenTK.Core;
using LearningOpenTK.Core.Interfaces;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Meshes;
using OpenTK.Graphics.OpenGL4;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace GameApp.Content;

// TODO: inherit some scene collection class idk (common for scene)

public class GameWorld : IGameEntry
{
    private ResourceRepository _resources;
    private Dictionary<Vector3Int, WorldObject> _chunks = new();

    public GameWorld(ResourceRepository resources)
    {
        _resources = resources;
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
        var obj = new WorldObject(mesh, _resources.ChunkShader, _resources.DiamondTexture);

        obj.Transform.Position = (Vector3)(System.Numerics.Vector3)chunk.GlobalPosition;
        return obj;
    }
    
    // TODO: temporary????? update mesh class to Mesh<TVertex, TIndex> where TVertex, TIndex : struct
    // private float[] FlattenVertices(List<ChunkVertex> vertices)
    // {
    //     var result = new float[vertices.Count * 11]; // 3+3+3+2
    //     int idx = 0;
    //
    //     foreach (var v in vertices)
    //     {
    //         void AddVec3(System.Numerics.Vector3 vec)
    //         {
    //             result[idx++] = vec.X;
    //             result[idx++] = vec.Y;
    //             result[idx++] = vec.Z;
    //         }
    //
    //         void AddVec2(Vector2 vec)
    //         {
    //             result[idx++] = vec.X;
    //             result[idx++] = vec.Y;
    //         }
    //
    //         AddVec3(v.Position);
    //         AddVec3(v.Normal);
    //         AddVec3(v.Color);
    //         AddVec2(v.Uv);
    //     }
    //
    //     return result;
    // }




    public void Update(float deltaTime)
    {
      //
    }

    public void Render(RenderContext context)
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        foreach (var chunk in _chunks.Values)
        {
            chunk.Render(context);
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