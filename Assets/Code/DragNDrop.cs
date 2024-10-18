using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public Camera cam;
    private bool isDragging = false;
    private Transform draggedObject;
    private Vector3 dragOffset;
    private float objectZDepth;
    public float yOffset = 1f; // Slight elevation for smooth following
    public float placementHeightOffset = 1f; // Offset to place the object above the "Place" item

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

    // Function to handle when the mouse button is pressed down
    private void HandleMouseDown()
    {
        RaycastHit hit;
        if (RaycastFromMouse(out hit) && hit.collider.CompareTag("Carta"))
        {
            StartDragging(hit.collider.transform);
        }
    }

    // Function to handle the dragging behavior
    private void HandleMouseDrag()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        UpdateObjectPosition(mousePosition);
    }

    // Function to handle when the mouse button is released
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
        //Anadir aqui logica de devolucion de la carta a su lugar original
    }

    // Start dragging the selected object
    private void StartDragging(Transform objectToDrag)
    {
        draggedObject = objectToDrag;
        isDragging = true;
        objectZDepth = cam.WorldToScreenPoint(draggedObject.position).z;
        Vector3 mousePosition = GetMouseWorldPosition();
        dragOffset = draggedObject.position - mousePosition;
    }

    // Stop dragging and reset the state
    private void StopDragging()
    {
        isDragging = false;
        draggedObject = null;
    }

    // Function to update the object's position while dragging
    private void UpdateObjectPosition(Vector3 mousePosition)
    {
        draggedObject.position = mousePosition + dragOffset + new Vector3(0, yOffset, 0);
    }

    // Function to convert mouse screen position to world position
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZDepth);
        return cam.ScreenToWorldPoint(mouseScreenPosition);
    }

    // Function to perform a raycast from the mouse position
    private bool RaycastFromMouse(out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit);
    }

    // Function to check if the object is over a "Place" object
    private bool IsMouseOverPlace(out RaycastHit hitBelow)
    {
        Ray downwardRay = new Ray(draggedObject.position, Vector3.down);
        return Physics.Raycast(downwardRay, out hitBelow) && hitBelow.collider.CompareTag("Place");
    }

    // Function to snap the dragged object above the "Place" item
    private void SnapToPlace(RaycastHit hitBelow)
    {
        Vector3 placePosition = hitBelow.collider.transform.position;
        draggedObject.position = new Vector3(placePosition.x, placePosition.y + placementHeightOffset, placePosition.z);
    }
}
