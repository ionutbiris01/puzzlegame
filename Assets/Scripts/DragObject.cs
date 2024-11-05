using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private float zCoord;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
        zCoord = mainCamera.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            transform.position = newPosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePoint);
        return worldPosition;
    }
}
