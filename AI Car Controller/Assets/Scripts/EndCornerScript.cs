using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityStandardAssets.Vehicles.Car;
using System.IO;

public class EndCornerScript : MonoBehaviour {

    [SerializeField]
    private GameObject theCar; //Used to store the car gameobject so it's components can be viewed

    //Enter if there's a collision
    private void OnTriggerEnter(Collider col)
    {
        //Set the endCorner variable in the AICarDistanceHolder script attached to the car object to false
        theCar.GetComponent<AICarDistanceHolder>().endCorner = false;

        //Set the distance variable in the AICarDistanceHolder script attached to the car object to
        //equal End of Corner, which tells the car that it's reached the end of the corner
        theCar.GetComponent<AICarDistanceHolder>().distance = "End of Corner";

        //Set the angleCorrect variable in the AICarDistanceHolder script attached to the car object to
        //the rotation in the y axis of the object this script is attached to
        theCar.GetComponent<AICarDistanceHolder>().angleCorrect = this.gameObject.transform.eulerAngles.y;
    }
}
