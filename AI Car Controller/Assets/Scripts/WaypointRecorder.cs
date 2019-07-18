using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityStandardAssets.Vehicles.Car;
using System.IO;

public class WaypointRecorder : MonoBehaviour
{

    [SerializeField]
    private GameObject theCar; //Used to store the car gameobject so it's components can be viewed

    [SerializeField]
    private GameObject recorder; //Used to store the recorder gameobject so it's components can be viewed

    private PhysicsRecorder record; //Used to get the record script component from the recorder game object

    private float currentVelocity; //Used to calculate the current velocity of the car

    private CarController carController; //Used to get the CarController script component from the car game object
    
    private bool completed = false; //Used to determine if the waypoint has already been hit by the car
    private void OnTriggerEnter(Collider col)
    {
        //Check if the checkpoint hasn't already been done
        if (!completed)
        {
            //Destroy(this.gameObject); //Destroy the waypoint with this script attached to it

            completed = true; //Set completed to be true so the car can't hit it again
            carController = theCar.GetComponent<CarController>(); //Grab the car's CarController component
            record = recorder.GetComponent<PhysicsRecorder>(); //Grab the recorder's PhysicsRecorder component

            //############################SAVE BRAKE FILE################################################
            StreamWriter wBrake = File.AppendText(record.pathMovement); //Open the braking file to be wrote to

            bool braking = false; //Used to determine if the car is currently braking or not
            bool paused = false; //Used to determine if the car is not braking or accelerating

            //Check if the car is accelerating
            if (carController.accelIn > 0)
                braking = false; //Set braking to false
            else if (carController.accelIn < 0) //Check if the car is braking
                braking = true; //Set braking to true
            else
                paused = true; //Set paused to true

            //Calculate the current velocity of the car by grabbing its current velocities in the x, y, and z and then performing pythagoras with them
            currentVelocity = (float)System.Math.Sqrt((theCar.GetComponent<Rigidbody>().velocity.x *
                theCar.GetComponent<Rigidbody>().velocity.x) + (theCar.GetComponent<Rigidbody>().velocity.y *
                theCar.GetComponent<Rigidbody>().velocity.y) + (theCar.GetComponent<Rigidbody>().velocity.z *
                theCar.GetComponent<Rigidbody>().velocity.z));

            //Check if the car isn't braking or accelerating
            if (paused == false)
                wBrake.WriteLine(this.gameObject.name + "," + currentVelocity + "," + braking); //Write the name of the waypoint the car has hit, the current velocity of the car and whether or not the car is braking to the file
            else
                wBrake.WriteLine(this.gameObject.name + "," + currentVelocity + "," + "paused"); //Write the name of the waypoint the car has hit, the current velocity of the car and paused to say that the car wasn't braking or accelerating
            //############################SAVE BRAKE FILE################################################

            //############################SAVE TURN FILE#################################################
            StreamWriter wTurn = File.AppendText(record.turnMovement); //Open the turning file to be wrote to

            float steer; //Used to store the turning value of the car

            //Check if the car is turning left
            if (carController.steeringIn < 0)
                steer = carController.steeringIn * -1; //Set steer to be equal to the steering value of the car multiplied by -1 to make it positive
            else
                steer = carController.steeringIn; //Set steer to be equal to the steering value of the car

            wTurn.WriteLine(this.gameObject.name + "," + currentVelocity + "," + steer); //Write the name of the waypoint the car has hit, the current velocity of the car and the steering value of the car
            //############################SAVE TURN FILE#################################################

            //Close both of the files that were opened
            wBrake.Close();
            wTurn.Close();
        }
    }
}