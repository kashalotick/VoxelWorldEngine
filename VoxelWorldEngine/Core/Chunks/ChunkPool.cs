// namespace VoxelWorldEngine.Core.Chunks;
//
// public class ChunkPool
// {
//     private Stack<Chunk> _pool;
//
//
//     public ChunkPool()
//     {
//         _pool = new Stack<Chunk>();
//         
//     }
//
//     public Chunk GetChunk()
//     {
//         if (_pool.Count > 0)
//         {
//             var chunk = _pool.Pop();
//
//         }
//         
//         return new Chunk(null);
//     }
//     
//     public void ReturnChunk(Chunk chunk)
//     {
//         
//     }
//
//     public void Clear()
//     {
//         _pool.Clear();
//     }
//     
//     
// }
//
// // TODO: refactor in Chunk as state machine
// public enum ChunkState
// {
//     Empty,
//     Queued,
//     Loading,
//     Active,
// }
// public class ChunkTemp
// {
//     public ChunkState State;
//
//     public void SetState(ChunkState state)
//     {
//         State = state;
//     }
//
//     public ChunkTemp()
//     {
//         State = ChunkState.Empty;
//     }
// }