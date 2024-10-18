using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public Vector3 initialPosition; 
    public Camera cam;
    private float vectory;
    //When click is pressed, search if any interactive card
    //void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            //If there is do
    //            //Debug.Log("Algo encontrada!");
    //            if (hit.collider.CompareTag("Carta"))
    //            {
    //                vectory = Input.mousePosition.y - 2.5f;
    //                //Debug.Log("Carta encontrada!");
    //                hit.collider.gameObject.transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, vectory, 5));

    //            }
    //        }
    //        if (Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
    //        {

    //        }
    //    }
    //}
    void Update()
    {
        // called the first time mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            initialPosition = transform.position;

            Vector3 rayPoint = ray.GetPoint(0);

            // Not sure but this might get you a slightly better value for distance
            distance = Vector3.Distance(transform.position, rayPoint);
        }

        // called while button stays pressed
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            Ball.MovePosition(initialPosition + new Vector3(rayPoint.x, 0, 0));
        }
    }


    //Elevate y+10

    //While click is pressed, maintain elevated + follow the mouse

    //When dropped 

    //if placeholder below --> set on top

    //if not, return to original position
    //make the function but leave it empty


}
