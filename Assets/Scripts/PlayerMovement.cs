using UnityEngine;
using System.Collections;

/// <summary>
/// Handles grid-based player movement with smooth interpolation.
/// Detects RoomExit triggers using Unity's OnTriggerEnter2D system.
/// 
/// Requirements:
/// - Rigidbody2D (Kinematic, Gravity Scale = 0)
/// - Player tag assigned to GameObject
/// - DungeonRoomManager present in scene
/// </summary>
public class PlayerMovementTriggerBased : MonoBehaviour
{
    #region Inspector Fields
    
    [Header("Movement Settings")]
    [Tooltip("Size of one grid cell in Unity units")]
    [SerializeField] private float gridSize = 1f;
    
    [Tooltip("Movement speed in tiles per second")]
    [SerializeField] private float moveSpeed = 5f;
    
    #endregion
    
    #region Private Fields
    
    private DungeonRoomManager roomManager;
    private Vector3 targetPos;
    private bool isMoving = false;
    
    #endregion
    
    #region Unity Lifecycle
    
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
    
    #endregion
    
    #region Movement Logic
    
    /// <summary>
    /// Processes WASD input and initiates movement
    /// </summary>
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
    
    /// <summary>
    /// Initiates movement in specified direction
    /// </summary>
    /// <param name="direction">Normalized direction vector</param>
    private void Move(Vector3 direction)
    {
        targetPos += direction * gridSize;
        StartCoroutine(SmoothMove());
    }

    /// <summary>
    /// Smoothly interpolates player position to target over time
    /// </summary>
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

    /// <summary>
    /// Snaps player position to nearest grid point
    /// Called on Start to ensure proper grid alignment
    /// </summary>
    private void SnapToGrid()
    {
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(x, y, transform.position.z);
        targetPos = transform.position;
    }
    
    #endregion
    
    #region Trigger Detection
    
    /// <summary>
    /// Called when player collider enters a trigger collider
    /// Detects RoomExit and initiates scene transition
    /// </summary>
    /// <param name="other">The collider that was entered</param>
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
    
    #endregion
    
    #region Validation
    
    /// <summary>
    /// Validates Rigidbody2D component and auto-configures if needed
    /// </summary>
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
    
    #endregion
} 
