using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    [SerializeField]
    private Text speed; //Used to store the speed text to be output on the screen
    [SerializeField]
    private Text lap; //Used to store the lap text to be output on the screen
    [SerializeField]
    private GameObject theCar; //Used to store the car gameobject so it's components can be viewed

    private Vector3 carVelocity;  //Used to grab and store the car's current velocity

    private float actualSpeed; //Used to store the current speed of the car

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Grab the velocity on the rigidbody of the car
        carVelocity = theCar.GetComponent<Rigidbody>().velocity;

        //Use pythagoras to determine the speed of the car
        actualSpeed = (float)System.Math.Sqrt((carVelocity.x * carVelocity.x) + (carVelocity.y * carVelocity.y) + (carVelocity.z * carVelocity.z));

        //Set the speed text to be the Speed: and the speed of the car
        speed.text = "Speed: " + System.Math.Round(actualSpeed, 2).ToString();

        //Set the lap text to be the Lap: and the current lap the car is on
        lap.text = "Lap: " + theCar.GetComponent<AIController>().lapCount;
    }
}
