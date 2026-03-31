using UnityEngine;
using UnityEngine.Events;

public class MazePlayerMove : MonoBehaviour
{
    public LayerMask boundsLayer, wallLayer, winLayer;
    private Vector3 lastPos;
    public float tileDistance = 0.5f;
    private bool completed;
    public UnityEvent OnMazeComplete;
    private void Awake()
    {
        completed = false;
        lastPos = transform.position;
    }


    public void TryMoveUp() => TryMove(new Vector3(0, 0, -tileDistance));
    public void TryMoveDown() => TryMove(new Vector3(0, 0, tileDistance));
    public void TryMoveLeft() => TryMove(new Vector3(tileDistance, 0, 0));
    public void TryMoveRight() => TryMove(new Vector3(-tileDistance, 0, 0));

    private void TryMove(Vector3 offset)
    {
        transform.position += offset;

        if (IsValidPosition())
        {
            lastPos = transform.position;
            bool isInEnd = Physics.CheckSphere(transform.position, 0.1f, winLayer);
            if (isInEnd) { WinMaze(); }
        }
        else
        {
            transform.position = lastPos;
            Debug.Log("Blocked! Moving back.");
        }
    }

    private bool IsValidPosition()
    {
        bool isInBounds = Physics.CheckSphere(transform.position, 0.1f, boundsLayer);
        bool isInWall = Physics.CheckSphere(transform.position, 0.1f, wallLayer);

        return isInBounds && !isInWall;
    }

    private void WinMaze()
    {
        completed = true;
        OnMazeComplete?.Invoke();
    }
}
