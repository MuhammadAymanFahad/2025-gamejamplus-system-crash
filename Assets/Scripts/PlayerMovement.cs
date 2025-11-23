using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour
{
    
    [Header("Movement Settings")]
    [Tooltip("Size of one grid cell in Unity units")]
    [SerializeField] private float gridSize = 1f;
    
    [Tooltip("Movement speed in tiles per second")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Collision Detection")]
    [Tooltip("Layer mask for obstacles (walls, tilemap obstacles)")]
    [SerializeField] private LayerMask obstacleLayer;
    
    [Tooltip("Distance to raycast for obstacle detection")]
    [SerializeField] private float raycastDistance = 1.2f;
    
    [Tooltip("Show debug rays in Scene view")]
    [SerializeField] private bool showDebugRays = true;
    
    
    private DungeonRoomManager roomManager;
    private Vector3 targetPos;
    private bool isMoving = false;

    void Start()
    {
        // Find room manager in scene
        roomManager = FindFirstObjectByType<DungeonRoomManager>();
        
        // Validate and setup Rigidbody2D
        ValidateRigidbody();
        
        // Initialize position on grid
        targetPos = transform.position;
        SnapToGrid();
    }

    void Update()
    {
        // Only accept input when not moving
        if (!isMoving)
        {
            HandleInput();
        }
    }
    
    /// Processes WASD input and initiates movement
    private void HandleInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector3.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Vector3.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Vector3.right;
        }

        if (direction != Vector3.zero)
        {
            Move(direction);
        }
    }
    
    /// Checks for obstacles using raycast before allowing movement
    private void Move(Vector3 direction)
    {
        // Check if the target direction is blocked by an obstacle
        if (!IsDirectionBlocked(direction))
        {
            targetPos += direction * gridSize;
            StartCoroutine(SmoothMove());
        }
        // If blocked, player simply doesn't move (no error, no feedback)
    }
    
    /// Checks if movement in a direction is blocked by an obstacle
    /// Uses Physics2D.Raycast to detect collisions
    private bool IsDirectionBlocked(Vector3 direction)
    {
        // Cast ray from player position in the given direction
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,      // Start from player position
            direction,               // Direction to check
            raycastDistance,         // Distance to check
            obstacleLayer            // Only check obstacle layer
        );
        
        // Draw debug ray for visualization
        if (showDebugRays)
        {
            Color rayColor = hit.collider != null ? Color.red : Color.green;
            Debug.DrawRay(transform.position, direction * raycastDistance, rayColor, 0.1f);
        }
        
        // Return true if something was hit (blocked)
        return hit.collider != null;
    }

    /// Smoothly interpolates player position to target over time
    private IEnumerator SmoothMove()
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        float duration = gridSize / moveSpeed;

        // Lerp from start to target position
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to exact target position
        transform.position = targetPos;
        isMoving = false;
    }

    /// Snaps player position to nearest grid point
    /// Called on Start to ensure proper grid alignment
    private void SnapToGrid()
    {
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(x, y, transform.position.z);
        targetPos = transform.position;
    }
    

    /// Called when player collider enters a trigger collider
    /// Detects RoomExit and initiates scene transition
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RoomExit"))
        {
            if (roomManager != null)
            {
                roomManager.PlayerEnteredRoom();
            }
        }
    }
    
    /// Validates Rigidbody2D component and auto-configures if needed
    private void ValidateRigidbody()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            // Auto-add Rigidbody2D if missing
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
        }
        else
        {
            // Ensure correct configuration
            if (rb.bodyType != RigidbodyType2D.Kinematic)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            
            if (rb.gravityScale != 0)
            {
                rb.gravityScale = 0;
            }
        }
    }
    
    
    /// Draws visual debugging aids in Scene view
    /// Shows raycast directions and their collision status
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        // Draw current position marker
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        
        // Draw target position marker
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(targetPos, Vector3.one * gridSize * 0.9f);
        
        // Draw raycasts in all 4 directions
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        
        foreach (Vector2 dir in directions)
        {
            // Check if direction is blocked
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                dir,
                raycastDistance,
                obstacleLayer
            );
            
            // Green if clear, red if blocked
            Gizmos.color = hit.collider != null ? new Color(1, 0, 0, 0.5f) : new Color(0, 1, 0, 0.5f);
            
            // Draw line
            Vector3 endPoint = transform.position + (Vector3)dir * raycastDistance;
            Gizmos.DrawLine(transform.position, endPoint);
            
            // Draw endpoint marker
            Gizmos.DrawWireSphere(endPoint, 0.1f);
        }
    }
}