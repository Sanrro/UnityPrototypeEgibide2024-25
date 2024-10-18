using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public Camera cam;
    private bool isDragging = false;
    private Transform draggedObject;
    private Vector3 dragOffset;
    private float objectZDepth;
    public float yOffset = 1f; // Smaller offset for more accurate following
    public float placementHeightOffset = 1f; // Offset to place the object above the "Place" item

    void Update()
    {
        // When the left mouse button is clicked, try to grab an object
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform a raycast to detect the object under the mouse
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Carta"))
                {
                    draggedObject = hit.collider.transform;
                    isDragging = true;

                    // Capture the Z depth of the object when clicked to maintain its depth
                    objectZDepth = cam.WorldToScreenPoint(draggedObject.position).z;

                    // Calculate the drag offset (difference between object and mouse position)
                    Vector3 mousePosition = GetMouseWorldPosition();
                    dragOffset = draggedObject.position - mousePosition;
                }
            }
        }

        // If dragging an object
        if (isDragging)
        {
            // Get the mouse position in world space
            Vector3 mousePosition = GetMouseWorldPosition();

            // Adjust the object's position to follow the mouse accurately, adding a small Y offset
            draggedObject.position = mousePosition + dragOffset + new Vector3(0, yOffset, 0);
        }

        // When the left mouse button is released, stop dragging
        if (Input.GetMouseButtonUp(0) && draggedObject != null)
        {
            isDragging = false;

            // Perform a raycast downwards to check for a "Place" item below the dragged object
            Ray downwardRay = new Ray(draggedObject.position, Vector3.down);
            RaycastHit hitBelow;

            if (Physics.Raycast(downwardRay, out hitBelow))
            {
                if (hitBelow.collider.CompareTag("Place"))
                {
                    // Snap the dragged object above the "Place" item
                    Vector3 placePosition = hitBelow.collider.transform.position;
                    draggedObject.position = new Vector3(placePosition.x, placePosition.y + placementHeightOffset, placePosition.z);
                }
            }

            // Clear the reference to the dragged object
            draggedObject = null;
        }
    }

    // Function to get the mouse position in world space with the correct Z depth
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZDepth);
        return cam.ScreenToWorldPoint(mouseScreenPosition);
    }
}
