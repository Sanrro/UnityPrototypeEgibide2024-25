using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public Camera cam;
    private bool isDragging = false;
    private Transform draggedObject;
    private Vector3 dragOffset;
    private float objectZDepth;
    private float fixedYPosition; // Store the fixed Y-axis position of the object
    public float placementHeightOffset = 1f; // Offset to place the object above the "Place" item
    private Color originalColor; // Store the original color of the object
    private bool isOverPlace = false; // Track if object is over "Place"

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

        // Check if the object is over a "Place" item and change color if true
        if (IsMouseOverPlace(out RaycastHit hitBelow))
        {
            if (!isOverPlace)
            {
                // Change the color to black when first over "Place"
                ChangeColor(Color.black);
                isOverPlace = true;
            }
        }
        else
        {
            if (isOverPlace)
            {
                // Revert the color when not over "Place" anymore
                RevertColor();
                isOverPlace = false;
            }
        }

        // Update the object's position while keeping the Y-axis fixed
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
    }

    // Start dragging the selected object
    private void StartDragging(Transform objectToDrag)
    {
        draggedObject = objectToDrag;
        isDragging = true;
        objectZDepth = cam.WorldToScreenPoint(draggedObject.position).z;
        fixedYPosition = draggedObject.position.y + 1; // Capture the Y position of the object to fix it and add the distance of the action of "picking"
        Vector3 mousePosition = GetMouseWorldPosition();
        dragOffset = draggedObject.position - new Vector3(mousePosition.x, 0, mousePosition.z); // Adjust offset for X and Z only

        // Get the original color of the object (assuming the object has a Renderer component)
        Renderer renderer = draggedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalColor = renderer.material.color;
        }
    }

    // Stop dragging and reset the state
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

    // Function to update the object's position while dragging
    private void UpdateObjectPosition(Vector3 mousePosition)
    {
        // Only update the X and Z positions, keep the Y-axis fixed
        draggedObject.position = new Vector3(
            mousePosition.x + dragOffset.x,
            fixedYPosition,  // Keep Y-axis fixed
            mousePosition.z + dragOffset.z
        );
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

    // Function to change the color of the dragged object
    private void ChangeColor(Color color)
    {
        Renderer renderer = draggedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;

        }
    }

    // Function to revert the color of the dragged object to its original color
    private void RevertColor()
    {
        Renderer renderer = draggedObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = originalColor;
        }
    }

    // Function to snap the dragged object above the "Place" item
    private void SnapToPlace(RaycastHit hitBelow)
    {
        Vector3 placePosition = hitBelow.collider.transform.position;
        draggedObject.position = new Vector3(placePosition.x, placePosition.y + placementHeightOffset, placePosition.z);
    }
}
