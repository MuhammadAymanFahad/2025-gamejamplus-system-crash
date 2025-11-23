using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Player transform
    [SerializeField] private bool autoFindPlayer = true; // Auto cari player saat start
    
    [Header("Follow Settings")]
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float smoothSpeed = 5f; // Semakin besar, semakin cepat
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // Offset dari player (Z harus negatif untuk 2D)
    
    [Header("Grid Snap (Untuk Pixel Perfect)")]
    [SerializeField] private bool snapToGrid = false;
    [SerializeField] private float pixelsPerUnit = 32f; // Sesuai dengan sprite settings
    
    [Header("Boundary Settings (Opsional)")]
    [SerializeField] private bool useBoundary = false;
    [SerializeField] private Vector2 minBoundary = new Vector2(-10, -10);
    [SerializeField] private Vector2 maxBoundary = new Vector2(10, 10);
    
    [Header("Dead Zone (Opsional)")]
    [SerializeField] private bool useDeadZone = false;
    [SerializeField] private Vector2 deadZoneSize = new Vector2(2f, 2f); // Area dimana kamera tidak bergerak
    
    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        
        // Auto cari player jika belum di-assign
        if (target == null && autoFindPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Camera: Player found and assigned!");
            }
            else
            {
                Debug.LogWarning("Camera: Player not found! Make sure player has 'Player' tag.");
            }
        }
        
        // Set posisi awal kamera ke player (tanpa smooth)
        if (target != null)
        {
            Vector3 initialPos = target.position + offset;
            initialPos = ApplyBoundaries(initialPos);
            transform.position = initialPos;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 desiredPosition = CalculateDesiredPosition();
        
        if (smoothFollow)
        {
            // Smooth follow menggunakan Lerp
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            // Direct follow (instant)
            transform.position = desiredPosition;
        }
        
        // Snap ke pixel grid jika diaktifkan
        if (snapToGrid)
        {
            SnapToPixelGrid();
        }
    }
    
    Vector3 CalculateDesiredPosition()
    {
        Vector3 targetPos = target.position;
        
        // Dead zone logic
        if (useDeadZone)
        {
            Vector3 currentPos = transform.position;
            float deltaX = targetPos.x - currentPos.x;
            float deltaY = targetPos.y - currentPos.y;
            
            // Hanya bergerak jika player keluar dari dead zone
            if (Mathf.Abs(deltaX) > deadZoneSize.x / 2)
            {
                targetPos.x = currentPos.x + (deltaX - Mathf.Sign(deltaX) * deadZoneSize.x / 2);
            }
            else
            {
                targetPos.x = currentPos.x;
            }
            
            if (Mathf.Abs(deltaY) > deadZoneSize.y / 2)
            {
                targetPos.y = currentPos.y + (deltaY - Mathf.Sign(deltaY) * deadZoneSize.y / 2);
            }
            else
            {
                targetPos.y = currentPos.y;
            }
        }
        
        Vector3 desiredPosition = targetPos + offset;
        desiredPosition = ApplyBoundaries(desiredPosition);
        
        return desiredPosition;
    }
    
    Vector3 ApplyBoundaries(Vector3 position)
    {
        if (!useBoundary) return position;
        
        // Clamp posisi kamera dalam boundary
        position.x = Mathf.Clamp(position.x, minBoundary.x, maxBoundary.x);
        position.y = Mathf.Clamp(position.y, minBoundary.y, maxBoundary.y);
        
        return position;
    }
    
    void SnapToPixelGrid()
    {
        // Snap ke pixel perfect grid
        Vector3 pos = transform.position;
        float gridUnitSize = 1f / pixelsPerUnit;
        
        pos.x = Mathf.Round(pos.x / gridUnitSize) * gridUnitSize;
        pos.y = Mathf.Round(pos.y / gridUnitSize) * gridUnitSize;
        
        transform.position = pos;
    }
    
    // Public method untuk set target baru
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    // Public method untuk set boundaries
    public void SetBoundaries(Vector2 min, Vector2 max)
    {
        minBoundary = min;
        maxBoundary = max;
        useBoundary = true;
    }
    
    // Visualisasi boundaries dan dead zone di Scene view
    void OnDrawGizmosSelected()
    {
        // Draw boundaries
        if (useBoundary)
        {
            Gizmos.color = Color.red;
            Vector3 bottomLeft = new Vector3(minBoundary.x, minBoundary.y, 0);
            Vector3 topRight = new Vector3(maxBoundary.x, maxBoundary.y, 0);
            Vector3 topLeft = new Vector3(minBoundary.x, maxBoundary.y, 0);
            Vector3 bottomRight = new Vector3(maxBoundary.x, minBoundary.y, 0);
            
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
        }
        
        // Draw dead zone
        if (useDeadZone && target != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0));
        }
    }
}