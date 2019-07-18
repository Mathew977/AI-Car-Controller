using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerIndicatorScript : MonoBehaviour
{
    [SerializeField]
    private GameObject theCar; //Used to store the car gameobject so it's components can be viewed

    //Enter if there's a collision
    private void OnTriggerEnter(Collider col)
    {
        //Set the endCorner variable in the AICarDistanceHolder script attached to the car object to true
        theCar.GetComponent<AICarDistanceHolder>().endCorner = true;
    }
}
