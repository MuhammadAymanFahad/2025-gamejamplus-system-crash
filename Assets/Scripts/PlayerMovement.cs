using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float moveSpeed = 5f;

    private DungeonRoomManager roomManager;
    private Vector3 targetPos;
    private bool isMoving = false;
    void Start()
    {
        roomManager = FindFirstObjectByType<DungeonRoomManager>();

        targetPos = transform.position;
        SnapToGrid();
    }

    private void CheckForRoomTrigger()
    {
        Collider2D hit = Physics2D.OverlapPoint(targetPos);

        if (hit != null && hit.CompareTag("RoomExit"))
        {
            roomManager.PlayerEnteredRoom();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            HandleInput();
        }
    }

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
        private void Move(Vector3 direction)
    {
        targetPos += direction * gridSize;
        StartCoroutine(SmoothMove());
    }

    IEnumerator SmoothMove()
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsedTime =  0f;
        float duration = gridSize / moveSpeed;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    void SnapToGrid()
    {
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(x, y, transform.position.z);
        targetPos = transform.position;
    }

}
