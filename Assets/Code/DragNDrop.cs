using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public GameObject draggedObject;
    public Camera cam;
    //When click is pressed, search if any interactive card

    //If there is do

    //Elevate y+10

    //While click is pressed, maintain elevated + follow the mouse
    private void Update()
    {
        draggedObject.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
    }
    //When dropped

    //if placeholder below --> set on top

    //if not, return to original position

}
