using UnityEngine;

public class RoomExitScript : MonoBehaviour
{
    
    [Header("Visual Settings")]
    [Tooltip("Color of the trigger area gizmo in Scene view")]
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);
    
    private bool playerInside = false;
    
    void Start()
    {
        ValidateSetup();
    }
    

    

    /// Called when a 2D collider enters this trigger collider
    /// Tracks when player enters the room exit area

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }
    

    /// Called when a 2D collider exits this trigger collider
    /// Tracks when player leaves the room exit area
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
    
    
    /// Validates that the GameObject is properly configured as a room exit
    private void ValidateSetup()
    {
        // Validate tag assignment
        if (!CompareTag("RoomExit"))
        {
            Debug.LogError($"[RoomExitScript] GameObject '{gameObject.name}' is missing 'RoomExit' tag. " +
                          "Please assign the 'RoomExit' tag in the Inspector.", this);
            return;
        }
        
        // Validate BoxCollider2D component
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError($"[RoomExitScript] GameObject '{gameObject.name}' is missing BoxCollider2D component. " +
                          "Please add a BoxCollider2D and enable 'Is Trigger'.", this);
            return;
        }
        
        // Validate trigger setting
        if (!collider.isTrigger)
        {
            Debug.LogError($"[RoomExitScript] GameObject '{gameObject.name}' has a BoxCollider2D but 'Is Trigger' is not enabled. " +
                          "Please check 'Is Trigger' in the BoxCollider2D component.", this);
        }
    }
    
    

    /// Draws visual representation of the trigger area in Scene view
    void OnDrawGizmos()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        
        if (boxCollider != null)
        {
            // Calculate center position accounting for collider offset
            Vector3 center = transform.position + (Vector3)boxCollider.offset;
            
            // Draw filled cube (color changes when player is inside)
            Gizmos.color = playerInside ? Color.green : gizmoColor;
            Gizmos.DrawCube(center, boxCollider.size);
            
            // Draw wireframe border for precise visualization
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, boxCollider.size);
        }
        else
        {
            // Fallback visualization if no collider is present
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
        
        // Draw directional arrow indicating exit
        Gizmos.color = Color.green;
        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = transform.position + Vector3.up * 0.7f;
        Gizmos.DrawLine(arrowStart, arrowEnd);
    }
    
}