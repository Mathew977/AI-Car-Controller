using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    private GameObject theCar; //Used to store the car gameobject so it's components can be viewed

    //Enter if there's a collision
    private void OnTriggerEnter(Collider col)
    {
        //Set the finshLine variable in the AICarDistanceHolder script attached to the car object to true
        theCar.GetComponent<AICarDistanceHolder>().finishLine = true;

        //Move the object this script is attached to down so it can't be seen or interacted with anymore
        this.gameObject.transform.Translate(0, -100, 0, Space.Self);
    }
}
