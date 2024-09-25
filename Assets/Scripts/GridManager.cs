using UnityEngine;

namespace codechallange{
    public class GridManager : MonoBehaviour
    {
        /// <summary>
        /// Width, Height, Depth of the Grid 
        /// </summary>
        public int width, height, depth = 10; 
        public float cellSize = 1.0f; // Size of each cell
        public static GridManager Instance; // Singleton instance
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        
    /// <summary>
    /// For Debugging purpose ////////
    /// Uncomment it to see the grid in the scene view /////
    /// </summary>
//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.gray; 

//         for (int x = 0; x <= width; x++)
//         {
//             for (int z = 0; z <= depth; z++)
//             {
//                 Vector3 start = new Vector3(x * cellSize, 0, z * cellSize);
//                 Vector3 end = new Vector3(x * cellSize, height * cellSize, z * cellSize);
//                 Gizmos.DrawLine(start, end); // Draw vertical lines
//             }
//         }

//         // Draw horizontal lines in the y-z plane (depth)
//         for (int y = 0; y <= height; y++)
//         {
//             for (int z = 0; z <= depth; z++)
//             {
//                 Vector3 start = new Vector3(0, y * cellSize, z * cellSize);
//                 Vector3 end = new Vector3(width * cellSize, y * cellSize, z * cellSize);
//                 Gizmos.DrawLine(start, end); // Draw horizontal lines
//             }
//         }

//         for (int y = 0; y <= height; y++)
//         {
//             for (int x = 0; x <= width; x++)
//             {
//                 Vector3 start = new Vector3(x * cellSize, y * cellSize, 0);
//                 Vector3 end = new Vector3(x * cellSize, y * cellSize, depth * cellSize);
//                 Gizmos.DrawLine(start, end); // Draw depth lines
//             }
//         }
//     }
  }   
}