using System.Numerics;
using VoxelModule.Core.Raycasting;
using VoxelModule.Utils;
using Chunk = VoxelModule.Engine.Chunks.Chunk;
using Vector3Int = VoxelModule.Core.Vectors.Vector3Int;

namespace VoxelModule.Engine;

public struct RayGridEnumerator
{
    private readonly Vector3 _dir;
    private readonly int _gridSize;
    private readonly float _maxDistance;
    
    public Vector3Int CurrentPos { get; private set; }
    public Vector3Int Step { get; private set; }
    public Vector3 TMax { get; private set; }
    public Vector3 TDelta { get; private set; }
    public float T { get; private set; }

    public RayGridEnumerator(Ray ray, int gridSize)
    {
        _gridSize = gridSize;
        _dir = ray.Direction;
        _maxDistance = ray.Length;
        T = 0;
        
        CurrentPos = Chunk.GlobalToChunk(ray.Origin.FloorToVector3Int());

        Step = new Vector3Int(
            _dir.X >= 0 ? 1 : -1,
            _dir.Y >= 0 ? 1 : -1,
            _dir.Z >= 0 ? 1 : -1
        );

        TDelta = new Vector3(
            MathF.Abs(_gridSize / _dir.X),
            MathF.Abs(_gridSize / _dir.Y),
            MathF.Abs(_gridSize / _dir.Z)
        );

        var chunkOrigin = CurrentPos * _gridSize;
        TMax = new Vector3(
            _dir.X >= 0 ? (chunkOrigin.X + _gridSize - ray.Origin.X) / _dir.X : (chunkOrigin.X - ray.Origin.X) / _dir.X,
            _dir.Y >= 0 ? (chunkOrigin.Y + _gridSize - ray.Origin.Y) / _dir.Y : (chunkOrigin.Y - ray.Origin.Y) / _dir.Y,
            _dir.Z >= 0 ? (chunkOrigin.Z + _gridSize - ray.Origin.Z) / _dir.Z : (chunkOrigin.Z - ray.Origin.Z) / _dir.Z
        );
    }

    public bool MoveNext()
    {
        if (T > _maxDistance) return false;

        // Логіка вибору наступної осі
        if (TMax.X < TMax.Y && TMax.X < TMax.Z)
        {
            T = TMax.X;
            var nextPos = CurrentPos;
            nextPos.X += Step.X;
            CurrentPos = nextPos;
            var nextTMax = TMax;
            nextTMax.X += TDelta.X;
            TMax = nextTMax;
        }
        else if (TMax.Y < TMax.Z)
        {
            T = TMax.Y;
            var nextPos = CurrentPos;
            nextPos.Y += Step.Y;
            CurrentPos = nextPos;
            var nextTMax = TMax;
            nextTMax.Y += TDelta.Y;
            TMax = nextTMax;
        }
        else
        {
            T = TMax.Z;
            var nextPos = CurrentPos;
            nextPos.Z += Step.Z;
            CurrentPos = nextPos;
            var nextTMax = TMax;
            nextTMax.Z += TDelta.Z;
            TMax = nextTMax;
        }

        return T <= _maxDistance;
    }
}