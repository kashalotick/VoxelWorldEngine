using System.Numerics;
using LearningOpenTK.Core;
using LearningOpenTK.Entities.World;
using LearningOpenTK.Meshes;
using LearningOpenTK.Resources;
using VoxelWorldEngine.Core.Builders;
using VoxelWorldEngine.DataStructures.Common.Structures.Vectors;
using VoxelWorldEngine.DataStructures.Special.Structures.Chunks;
using VoxelWorldEngine.DataStructures.Special.Structures.Vertices;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ConsoleApp1;

public class Program
{
    public static void Main(string[] args)
    {
        var game = new Game(1200, 900, "Voxel engine test");
        game.SetDefaultScene((w, h, input) => new DemoScene(w, h, input, GenerateWorldObjectList));
        game.Run();
    }

    public static List<WorldObject> GenerateWorldObjectList(Shader shader, Texture texture)
    {
        var chunks = GenerateChunks();
        var worldObjectList = new List<WorldObject>();

        foreach (var chunk in chunks)
        {
            var worldObject = CreateWorldObject(chunk, shader, texture);
            worldObjectList.Add(worldObject);
        }

        return worldObjectList;
    }


    public static WorldObject CreateWorldObject(Chunk chunk, Shader shader, Texture texture)
    {
        var mesh = new WorldObjectMesh(FlattenVertices(chunk.Mesh.Vertices), chunk.Mesh.Indices.ToArray());
        var obj = new WorldObject(mesh, shader, texture);

        obj.Transform.Position = (Vector3)(System.Numerics.Vector3)chunk.GlobalPosition;
        // obj.Transform.Position -= Vector3.UnitY * 10;
        // obj.Transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);
        return obj;
    }

    public static float[] FlattenVertices(List<Vertex> vertices)
    {
        var result = new float[vertices.Count * 11]; // 3+3+3+2
        int idx = 0;

        foreach (var v in vertices)
        {
            void AddVec3(System.Numerics.Vector3 vec)
            {
                result[idx++] = vec.X;
                result[idx++] = vec.Y;
                result[idx++] = vec.Z;
            }

            void AddVec2(Vector2 vec)
            {
                result[idx++] = vec.X;
                result[idx++] = vec.Y;
            }

            AddVec3(v.Position);
            AddVec3(v.Normal);
            AddVec3(v.Color);
            AddVec2(v.Uv);
        }

        return result;
    }


    public static List<Chunk> GenerateChunks()
    {
        const int seed = 123456;
        var chunks = new List<Chunk>();

        var chunkBuilder = new ChunkBuilder(seed);
        List<Vector3Int> chunkCoords =
        [
            // new(0, 0, 0),
            // new(0, 0, 1),
            new(0, -1, 1),

            new(0, -1, 0),
            new(0, 0, 0),

        ];
        foreach (var c in chunkCoords)
        {
            chunks.Add(chunkBuilder.Build(c));
        }
        

        return chunks;
    }
}