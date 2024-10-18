using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public Camera cam;
    private bool isDragging = false;  // Bandera para saber si se está arrastrando un objeto
    private Transform draggedObject;  // El objeto que está siendo arrastrado
    private Vector3 dragOffset;  // El offset de la posición del objeto en relación con la posición del ratón
    private float objectZDepth;  // La profundidad del objeto en el eje Z en el espacio de la pantalla
    private float fixedYPosition;  // El valor fijo de la posición en el eje Y del objeto durante el arrastre
    public float placementHeightOffset = 1f;  // Altura adicional para colocar el objeto por encima de un "Place"
    private Color originalColor;  // El color original del objeto para restaurarlo después de cambiar su color
    private bool isOverPlace = false;  // Indica si el objeto está sobre un objeto con la etiqueta "Place"

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Si se presiona el botón izquierdo del ratón
        {
            HandleMouseDown();  // Inicia el proceso de arrastre
        }

        if (isDragging)  // Si el objeto está siendo arrastrado
        {
            HandleMouseDrag();  // Actualiza la posición del objeto mientras se arrastra
        }

        if (Input.GetMouseButtonUp(0))  // Si se suelta el botón del ratón
        {
            HandleMouseUp();  // Detiene el arrastre y coloca el objeto en su nueva posición o lo suelta
        }
    }

    // Esta función maneja lo que sucede cuando se presiona el botón del ratón
    private void HandleMouseDown()
    {
        RaycastHit hit;
        if (RaycastFromMouse(out hit) && hit.collider.CompareTag("Carta"))  // Verifica si el objeto que el rayo toca es una "Carta"
        {
            StartDragging(hit.collider.transform);  // Comienza el arrastre si es una "Carta"
        }
    }

    // Esta función maneja el movimiento del objeto mientras se está arrastrando
    private void HandleMouseDrag()
    {
        Vector3 mousePosition = GetMouseWorldPosition();  // Convierte la posición del ratón a coordenadas del mundo

        if (IsMouseOverPlace(out RaycastHit hitBelow))  // Verifica si el objeto está sobre un "Place"
        {
            if (!isOverPlace)  // Si no estaba sobre un "Place" antes
            {
                ChangeColor(Color.black);  // Cambia el color del objeto a negro
                isOverPlace = true;  // Marca que ahora está sobre un "Place"
            }
        }
        else
        {
            if (isOverPlace)  // Si estaba sobre un "Place" pero ya no lo está
            {
                RevertColor();  // Restaura el color original del objeto
                isOverPlace = false;  // Indica que ya no está sobre un "Place"
            }
        }

        UpdateObjectPosition(mousePosition);  // Actualiza la posición del objeto mientras se arrastra
    }

    // Esta función maneja lo que sucede cuando se suelta el botón del ratón
    private void HandleMouseUp()
    {
        if (draggedObject != null)  // Verifica si hay un objeto siendo arrastrado
        {
            if (IsMouseOverPlace(out RaycastHit hitBelow))  // Si el objeto está sobre un "Place"
            {
                SnapToPlace(hitBelow);  // Coloca el objeto sobre el "Place"
            }
            StopDragging();  // Detiene el arrastre
        }
    }

    // Comienza el arrastre del objeto seleccionado
    private void StartDragging(Transform objectToDrag)
    {
        draggedObject = objectToDrag;  // Asigna el objeto que se está arrastrando
        isDragging = true;  // Indica que se está arrastrando
        objectZDepth = cam.WorldToScreenPoint(draggedObject.position).z;  // Obtiene la profundidad Z del objeto en la pantalla
        fixedYPosition = draggedObject.position.y + 1;  // ??? Fija la posición Y del objeto a su posición original más 1, no está claro por qué se suma 1
        Vector3 mousePosition = GetMouseWorldPosition();  // Obtiene la posición del ratón en el mundo
        dragOffset = draggedObject.position - new Vector3(mousePosition.x, 0, mousePosition.z);  // ??? El offset se calcula solo para los ejes X y Z, ignorando el Y

        Renderer renderer = draggedObject.GetComponent<Renderer>();  // Obtiene el Renderer del objeto para cambiar su color
        if (renderer != null)
        {
            originalColor = renderer.material.color;  // Guarda el color original del objeto
        }
    }

    // Detiene el proceso de arrastre
    private void StopDragging()
    {
        isDragging = false;  // Indica que ya no se está arrastrando
        if (isOverPlace)
        {
            RevertColor();  // Restaura el color si estaba sobre un "Place"
            isOverPlace = false;  // Resetea el indicador de "Place"
        }
        draggedObject = null;  // Libera el objeto que estaba siendo arrastrado
    }

    // Actualiza la posición del objeto mientras se arrastra
    private void UpdateObjectPosition(Vector3 mousePosition)
    {
        // Actualiza solo las posiciones X y Z, manteniendo el eje Y fijo
        draggedObject.position = new Vector3(mousePosition.x + dragOffset.x, fixedYPosition, mousePosition.z + dragOffset.z);
    }

    // Convierte la posición del ratón en pantalla a una posición en el mundo
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZDepth);  // Usa la profundidad Z capturada
        return cam.ScreenToWorldPoint(mouseScreenPosition);  // Convierte la posición de la pantalla a posición en el mundo
    }

    // Realiza un raycast desde la posición del ratón
    private bool RaycastFromMouse(out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // Crea un rayo desde la cámara hacia la posición del ratón
        return Physics.Raycast(ray, out hit);  // Verifica si el rayo toca un objeto
    }

    // Verifica si el objeto está sobre un "Place"
    private bool IsMouseOverPlace(out RaycastHit hitBelow)
    {
        Ray downwardRay = new Ray(draggedObject.position, Vector3.down);  // Crea un rayo hacia abajo desde el objeto arrastrado
        return Physics.Raycast(downwardRay, out hitBelow) && hitBelow.collider.CompareTag("Place");  // Verifica si toca un objeto con la etiqueta "Place"
    }

    // Cambia el color del objeto arrastrado
    private void ChangeColor(Color color)
    {
        Renderer renderer = draggedObject.GetComponent<Renderer>();  // Obtiene el Renderer del objeto
        if (renderer != null)
        {
            renderer.material.color = color;  // Cambia el color del material del objeto
        }
    }

    // Restaura el color original del objeto arrastrado
    private void RevertColor()
    {
        Renderer renderer = draggedObject.GetComponent<Renderer>();  // Obtiene el Renderer del objeto
        if (renderer != null)
        {
            renderer.material.color = originalColor;  // Restaura el color original del material del objeto
        }
    }

    // Coloca el objeto sobre el "Place"
    private void SnapToPlace(RaycastHit hitBelow)
    {
        Vector3 placePosition = hitBelow.collider.transform.position;  // Obtiene la posición del "Place"
        draggedObject.position = new Vector3(placePosition.x, placePosition.y + placementHeightOffset, placePosition.z);  // Coloca el objeto ligeramente por encima del "Place"
    }
}
