using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace codechallange
{
    public class PipeManager : MonoBehaviour
    {
        public GameObject pipePrefab;  // Reference to your pipe prefab
        public GameObject bendPrefab;  // Reference to your sphere bend prefab
        public float pipeSpawnDelay = 0.5f;  // Delay between each pipe spawn
       
       
        private int minStraightPipeLength = 3;  // Minimum number of pipes in a straight line
        private int maxStraightPipeLength = 8;  
        private Vector3 currentGridPosition;   
        private Vector3 direction;             // Current direction the pipe
        private int pipesInCurrentDirection = 0;  
        private int currentStraightPipeLength;         
        private List<Vector3> occupiedPositions = new List<Vector3>();  // This list contains the positions of cells that are already occupied by pipes or bends //
        private List<Vector3> availablePositions = new List<Vector3>();   // This list contains for the available cell in the grid ///
        


        private Renderer currentPipeRenderer;  // Renderer for changing color
        private List<GameObject> currentSegmentPipes = new List<GameObject>();  // This list is used for the tracking of segments//
        private List<GameObject> allPipesAndBends = new List<GameObject>();  /// The list is used for the storing all the pipes and bend 

        
        private void Start()
        {
            // Initialize the available positions list with all grid cells
            InitGrid();
            
            ChangeDirection();
            SetRandomStraightLength();

            // Start the coroutine to spawn the pipes
            StartCoroutine(SpawnPipeRoutine());
        }
        
        void InitGrid(){
             for (int x = 0; x < GridManager.Instance.width; x++)
            {
                for (int y = 0; y < GridManager.Instance.height; y++)
                {
                    for (int z = 0; z < GridManager.Instance.depth; z++)
                    {
                        availablePositions.Add(new Vector3(x, y, z));
                    }
                }
            }
            currentGridPosition = new Vector3(0, 0, 0);
            occupiedPositions.Add(currentGridPosition);
            availablePositions.Remove(currentGridPosition);

        }

        IEnumerator SpawnPipeRoutine()
        {
            GameObject pipe;
            Color currentSegmentColor = Color.red; // Default color for the segment

            while (true)
            {
                // Check if we need to change direction or the next position is invalid and this is for spawning the bend
                if (pipesInCurrentDirection >= currentStraightPipeLength || !IsNextPositionValid())
                {
                    var bend = Instantiate(bendPrefab, GetCenteredPosition(currentGridPosition), Quaternion.identity);
                    bend.GetComponent<Renderer>().material.color = currentSegmentColor; 
                    allPipesAndBends.Add(bend);

                    // Mark the bend position as occupied
                    occupiedPositions.Add(currentGridPosition);
                    availablePositions.Remove(currentGridPosition);  
                    currentSegmentPipes.Add(bend); 

                    // Mark the current pipe position as occupied and remove it from available positions(cells) 
                     if (occupiedPositions.Count >= 1000)
                    {
                        ClearAllPipesAndRestart();
                        Debug.Log("Reached 1000 occupied cells");
                        yield break;  
                    }

                    // Reset direction and straight pipe length
                    pipesInCurrentDirection = 0;

                    // Condition for the Deadend /
                    if (IsDeadEnd())
                    {
                        Debug.Log("Deadend Occurs");
                        currentSegmentColor = new Color(Random.value, Random.value, Random.value); // Generate a new random color
                        
                        RespawnAtRandomPosition();
                        currentSegmentPipes.Clear();

                        // Instantiate a new pipe at the new position
                        pipe = Instantiate(pipePrefab, GetCenteredPosition(currentGridPosition), GetPipeRotation(direction));
                        currentPipeRenderer = pipe.GetComponent<Renderer>(); 
                        currentPipeRenderer.material.color = currentSegmentColor;

                        // Add the new pipe to the current segment pipes
                        currentSegmentPipes.Add(pipe);
                        allPipesAndBends.Add(pipe);

                    }
                    else
                    {
                        ChangeDirection();
                        SetRandomStraightLength();
                    }
                }
                // This one is for spawning the pipe //
                else
                {
                    // Instantiate a straight pipe at the current position
                    pipe = Instantiate(pipePrefab, GetCenteredPosition(currentGridPosition), GetPipeRotation(direction));
                    allPipesAndBends.Add(pipe);

                    currentPipeRenderer = pipe.GetComponent<Renderer>();
                    currentPipeRenderer.material.color = currentSegmentColor; 

                    // Mark the current pipe position as occupied and remove it from available positions(cells) 
                    occupiedPositions.Add(currentGridPosition);
                    availablePositions.Remove(currentGridPosition); 

                    // Check if occupied cells have reached the limit
                    if (occupiedPositions.Count >= 1000)
                    {
                        Debug.Log("Reached 1000 occupied cells");
                        ClearAllPipesAndRestart();
                        yield break; 

                    }

                    currentSegmentPipes.Add(pipe);    // Add the pipe to the current segment pipes list
                     
                    currentGridPosition += direction;  // Move to the next grid cell in the current directions

                    pipesInCurrentDirection++;

                    yield return new WaitForSeconds(pipeSpawnDelay);
                }
            }
        }
        
        
        private void ClearAllPipesAndRestart()
        {
            foreach (var pipe in allPipesAndBends)
            {
                Destroy(pipe); // Destroy each pipe or bend
            }
            // Clear the occupied and available positions
            occupiedPositions.Clear();
            availablePositions.Clear();
            InitGrid();
        
            StartCoroutine(SpawnPipeRoutine());
        }

        // Check if no valid directions exist (dead-end detection)
        private bool IsDeadEnd()
        {
            Vector3[] directions = new Vector3[] {
                Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back
            };

            // Check all possible directions
            foreach (Vector3 dir in directions)
            {
                if (IsNextPositionValid(dir))
                {
                    return false;
                }
            }

            return true; 
        }

        // Respawn pipe at a random available position on the grid
        private void RespawnAtRandomPosition()
        {
            if (availablePositions.Count > 0)
            {
                currentGridPosition = availablePositions[Random.Range(0, availablePositions.Count)];
            }
        }

        private void ChangeDirection()
        {
            Vector3[] directions = new Vector3[] {
                Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back
            };

            Vector3 newDirection;

            do
            {
                newDirection = directions[Random.Range(0, directions.Length)];
            }
            while (newDirection == -direction || !IsNextPositionValid(newDirection));

            direction = newDirection;
        }

        // Centering pipe and bend in the cell 
        private Vector3 GetCenteredPosition(Vector3 gridPosition)
        {
            float halfCellSize = GridManager.Instance.cellSize / 2f;
            return new Vector3(
                gridPosition.x * GridManager.Instance.cellSize + halfCellSize,
                gridPosition.y * GridManager.Instance.cellSize + halfCellSize,
                gridPosition.z * GridManager.Instance.cellSize + halfCellSize
            );
        }

        private Quaternion GetPipeRotation(Vector3 dir)
        {
            if (dir == Vector3.forward || dir == Vector3.back)
            {
                return Quaternion.Euler(90, 0, 0); 
            }
            else if (dir == Vector3.right || dir == Vector3.left)
            {
                return Quaternion.Euler(0, 0, 90); 
            }
            else if (dir == Vector3.up || dir == Vector3.down)
            {
                return Quaternion.identity; 
            }

            return Quaternion.identity; // Default rotation if no direction is matched
        }

        private void SetRandomStraightLength()
        {
            currentStraightPipeLength = Random.Range(minStraightPipeLength, maxStraightPipeLength + 1);
        }

        // Check if the next grid position is valid (inside the grid and not occupied)
        private bool IsNextPositionValid()
        {
            Vector3 nextPosition = currentGridPosition + direction;
            return IsPositionInsideGrid(nextPosition) && !IsPositionOccupied(nextPosition);
        }

        // Check if the next position in a given direction is valid
        private bool IsNextPositionValid(Vector3 testDirection)
        {
            Vector3 nextPosition = currentGridPosition + testDirection;
            return IsPositionInsideGrid(nextPosition) && !IsPositionOccupied(nextPosition);
        }

        private bool IsPositionInsideGrid(Vector3 position)
        {
            return position.x >= 0 && position.x < GridManager.Instance.width &&
                   position.y >= 0 && position.y < GridManager.Instance.height &&
                   position.z >= 0 && position.z < GridManager.Instance.depth;
        }

        // Check if the position is already occupied by another pipe or bend
        private bool IsPositionOccupied(Vector3 position)
        {
            return occupiedPositions.Contains(position);
        }
    }
}
