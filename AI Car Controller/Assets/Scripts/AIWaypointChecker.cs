using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWaypointChecker : MonoBehaviour
{
    [SerializeField]
    private GameObject car; //Used to store the car gameobject so it's components can be viewed
    
    //Enter if there's a collision
    private void OnTriggerEnter(Collider col)
    {
        //Set the distance variable in the AICarDistanceHolder script attached to the car object to
        //equal the name of the object this script is attached to
        car.GetComponent<AICarDistanceHolder>().distance = this.gameObject.name;
    }
}
