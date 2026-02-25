using OpenTK.Mathematics;
using VoxelWorldEngine.Core;
using VoxelWorldEngine.Core.Worlds;
using Vector3Int = VoxelWorldEngine.DataStructures.Common.Structures.Vectors.Vector3Int;

namespace ConsoleApp1;

public class ChunkLoadingSystem : ISystem
{
    private Player _player;

    private const int ChunkSize = 16;
    private Vector3i _activeChunkPosition;
    private World _world;

    public ChunkLoadingSystem(World world, Player player)
    {
        _world = world;
        _player = player;
    }

    public void FixedUpdate(double deltaTime)
    {
        // var observerPosition = new Vector3Int((int)_player.Transform.Position.X, 0, (int)_player.Transform.Position.Z);
        // var observer = new Observer(observerPosition, 2);
        //
        // _world.Update(deltaTime, observer);
    }
}