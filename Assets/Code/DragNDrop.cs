using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public Camera cam;
    private bool isDragging = false;  // Bandera para saber si se est� arrastrando un objeto
    private Transform draggedObject;  // El objeto que est� siendo arrastrado
    private Vector3 dragOffset;  // El offset de la posici�n del objeto en relaci�n con la posici�n del rat�n
    private float objectZDepth;  // La profundidad del objeto en el eje Z en el espacio de la pantalla
    private float fixedYPosition;  // El valor fijo de la posici�n en el eje Y del objeto durante el arrastre
    public float placementHeightOffset = 1f;  // Altura adicional para colocar el objeto por encima de un "Place"
    private Color originalColor;  // El color original del objeto para restaurarlo despu�s de cambiar su color
    private bool isOverPlace = false;  // Indica si el objeto est� sobre un objeto con la etiqueta "Place"

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Si se presiona el bot�n izquierdo del rat�n
        {
            HandleMouseDown();  // Inicia el proceso de arrastre
        }

        if (isDragging)  // Si el objeto est� siendo arrastrado
        {
            HandleMouseDrag();  // Actualiza la posici�n del objeto mientras se arrastra
        }

        if (Input.GetMouseButtonUp(0))  // Si se suelta el bot�n del rat�n
        {
            HandleMouseUp();  // Detiene el arrastre y coloca el objeto en su nueva posici�n o lo suelta
        }
    }

    // Esta funci�n maneja lo que sucede cuando se presiona el bot�n del rat�n
    private void HandleMouseDown()
    {
        RaycastHit hit;
        if (RaycastFromMouse(out hit) && hit.collider.CompareTag("Carta"))  // Verifica si el objeto que el rayo toca es una "Carta"
        {
            StartDragging(hit.collider.transform);  // Comienza el arrastre si es una "Carta"
        }
    }

    // Esta funci�n maneja el movimiento del objeto mientras se est� arrastrando
    private void HandleMouseDrag()
    {
        Vector3 mousePosition = GetMouseWorldPosition();  // Convierte la posici�n del rat�n a coordenadas del mundo

        if (IsMouseOverPlace(out RaycastHit hitBelow))  // Verifica si el objeto est� sobre un "Place"
        {
            if (!isOverPlace)  // Si no estaba sobre un "Place" antes
            {
                ChangeColor(Color.black);  // Cambia el color del objeto a negro
                isOverPlace = true;  // Marca que ahora est� sobre un "Place"
            }
        }
        else
        {
            if (isOverPlace)  // Si estaba sobre un "Place" pero ya no lo est�
            {
                RevertColor();  // Restaura el color original del objeto
                isOverPlace = false;  // Indica que ya no est� sobre un "Place"
            }
        }

        UpdateObjectPosition(mousePosition);  // Actualiza la posici�n del objeto mientras se arrastra
    }

    // Esta funci�n maneja lo que sucede cuando se suelta el bot�n del rat�n
    private void HandleMouseUp()
    {
        if (draggedObject != null)  // Verifica si hay un objeto siendo arrastrado
        {
            if (IsMouseOverPlace(out RaycastHit hitBelow))  // Si el objeto est� sobre un "Place"
            {
                SnapToPlace(hitBelow);  // Coloca el objeto sobre el "Place"
            }
            StopDragging();  // Detiene el arrastre
        }
    }

    // Comienza el arrastre del objeto seleccionado
    private void StartDragging(Transform objectToDrag)
    {
        draggedObject = objectToDrag;  // Asigna el objeto que se est� arrastrando
        isDragging = true;  // Indica que se est� arrastrando
        objectZDepth = cam.WorldToScreenPoint(draggedObject.position).z;  // Obtiene la profundidad Z del objeto en la pantalla
        fixedYPosition = draggedObject.position.y + 1;  // ??? Fija la posici�n Y del objeto a su posici�n original m�s 1, no est� claro por qu� se suma 1
        Vector3 mousePosition = GetMouseWorldPosition();  // Obtiene la posici�n del rat�n en el mundo
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
        isDragging = false;  // Indica que ya no se est� arrastrando
        if (isOverPlace)
        {
            RevertColor();  // Restaura el color si estaba sobre un "Place"
            isOverPlace = false;  // Resetea el indicador de "Place"
        }
        draggedObject = null;  // Libera el objeto que estaba siendo arrastrado
    }

    // Actualiza la posici�n del objeto mientras se arrastra
    private void UpdateObjectPosition(Vector3 mousePosition)
    {
        // Actualiza solo las posiciones X y Z, manteniendo el eje Y fijo
        draggedObject.position = new Vector3(mousePosition.x + dragOffset.x, fixedYPosition, mousePosition.z + dragOffset.z);
    }

    // Convierte la posici�n del rat�n en pantalla a una posici�n en el mundo
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectZDepth);  // Usa la profundidad Z capturada
        return cam.ScreenToWorldPoint(mouseScreenPosition);  // Convierte la posici�n de la pantalla a posici�n en el mundo
    }

    // Realiza un raycast desde la posici�n del rat�n
    private bool RaycastFromMouse(out RaycastHit hit)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);  // Crea un rayo desde la c�mara hacia la posici�n del rat�n
        return Physics.Raycast(ray, out hit);  // Verifica si el rayo toca un objeto
    }

    // Verifica si el objeto est� sobre un "Place"
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
        Vector3 placePosition = hitBelow.collider.transform.position;  // Obtiene la posici�n del "Place"
        draggedObject.position = new Vector3(placePosition.x, placePosition.y + placementHeightOffset, placePosition.z);  // Coloca el objeto ligeramente por encima del "Place"
    }
}
