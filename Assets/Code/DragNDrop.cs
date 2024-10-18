using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public Camera cam;
    private bool isDragging = false;
    private Transform draggedObject;
    private Vector3 dragOffset;
    private float objectZDepth;
    private float fixedYPosition;
    public float placementHeightOffset = 1f;
    private Color originalColor;
    private bool isOverPlace = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }

        if (isDragging)
        {
            HandleMouseDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleMouseUp();
        }
    }

    private void HandleMouseDown()
    {
        RaycastHit hit;
        if (RaycastFromMouse(out hit) && hit.collider.CompareTag("Carta"))
        {
            StartDragging(hit.collider.transform);
        }
    }

    private void HandleMouseDrag()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        if (IsMouseOverPlace(out RaycastHit hitBelow))
        {
            if (!isOverPlace)
            {
                ChangeColor(Color.black);
                isOverPlace = true;
            }
        }
        else
        {
            if (isOverPlace)
            {
                RevertColor();
                isOverPlace = false;
            }
        }

        UpdateObjectPosition(mousePosition);
    }

    private void HandleMouseUp()
    {
        if (draggedObject != null)
        {
            if (IsMouseOverPlace(out RaycastHit hitBelow))
            {
                SnapToPlace(hitBelow);
            }
            StopDragging();
        }
    }

    private void StartDragging(Transform objectToDrag)
    {
        draggedObject = objectToDrag;
        isDragging = true;
        objectZDepth = cam.WorldToScreenPoint(draggedObject.position).z;
        fixedYPosition = draggedObject.position.y + 1; 
        Vector3 mousePosition = GetMouseWorldPosition();
        dragOffset = draggedObject.position - new Vector3(mousePosition.x, 0, mousePosition.z);

        Renderer renderer = draggedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalColor = renderer.material.color;
        }
    }

    private void StopDragging()
    {
        isDragging = false;
        if (isOverPlace)
        {
            RevertColor();
            isOverPlace = false;
        }
        draggedObject = null;
    }

    private void UpdateObjectPosition(Vector3 mousePosition)
    {
        draggedObject.position = new Vector3(
            mousePosition.x + dragOffset.x,
            fixedYPosition,  // Keep Y-axis fixed
            mousePosition.z + dragOffset.z
        );
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZDepth);
        return cam.ScreenToWorldPoint(mouseScreenPosition);
    }

    private bool RaycastFromMouse(out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit);
    }

    private bool IsMouseOverPlace(out RaycastHit hitBelow)
    {
        Ray downwardRay = new Ray(draggedObject.position, Vector3.down);
        return Physics.Raycast(downwardRay, out hitBelow) && hitBelow.collider.CompareTag("Place");
    }

    private void ChangeColor(Color color)
    {
        Renderer renderer = draggedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;

        }
    }

    private void RevertColor()
    {
        Renderer renderer = draggedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = originalColor;
        }
    }

    private void SnapToPlace(RaycastHit hitBelow)
    {
        Vector3 placePosition = hitBelow.collider.transform.position;
        draggedObject.position = new Vector3(placePosition.x, placePosition.y + placementHeightOffset, placePosition.z);
    }
}
