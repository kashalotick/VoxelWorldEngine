// using System.Collections;
//
// namespace VoxelWorldEngine.DataStructures.LinearOctree;
//
// public class LinearOctreeLayerEnumerator
// {
//     // public IEnumerator<LinearOctreeNode[]> GetEnumerator()
//     // {
//     //     throw new NotImplementedException();
//     // }
//     //
//     // IEnumerator IEnumerable.GetEnumerator()
//     // {
//     //     return GetEnumerator();
//     // }
//
//     public void ForeachLayer(LinearOctree octree)
//     {
//         var queue = new Queue<LinearOctreeNode>();
//         
//         var root = octree.GetNode(octree.RootIndex);
//         queue.Enqueue(root);
//         
//         // var indent = 0;
//         while (queue.Count > 0)
//         {
//             var node = queue.Dequeue();
//
//             if (node.IsLeaf)
//             {
//                 // process
//             }
//             else
//             {
//                 // indent++;
//                 // if (indent > 2)
//                 // {
//                 //     // process as leaf
//                 // }
//                 for (int i = 0; i < 8; i++)
//                 {
//                     var childIndex = node.GetChildIndex(i);
//                     var child = octree.GetNode(childIndex);
//                     queue.Enqueue(child);
//                 }
//             }
//         }
//
//     }
//     
//     
//     public void ForeachLeaf(LinearOctree octree)
//     {
//         
//     }
// }