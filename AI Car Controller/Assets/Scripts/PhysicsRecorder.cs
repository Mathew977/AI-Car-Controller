using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityStandardAssets.Vehicles.Car;
using System.IO;

public class PhysicsRecorder : MonoBehaviour
{

    public string recordDirectory { get; private set; } //Used to store the location of the files to be altered during this run of the project
    public string pathMovement { get; private set; } //Used to store the exact location of the braking file being written to and the file name
    public string turnMovement { get; private set; } //Used to store the exact location of the turning file being written to and the file name
    public int numberOfRecords { get; private set; } //Used to keep track of the number of files created so far so the AI knows how many files to read later

    // Use this for initialization
    void Start()
    {
        int fileCounter = 1;

        //Loop forever until it has created a new file
        while (true)
        {
            recordDirectory = Application.dataPath + "\\Recorded Data\\Record" + fileCounter; //Set recordDirectory to the location of the record folder coresponding to the count in the loop

            //Check if the folder doesn't exist
            if (!Directory.Exists(recordDirectory))
            {
                Directory.CreateDirectory(recordDirectory); //Create the folder since it doesn't exist yet

                pathMovement = recordDirectory + "\\Braking.txt"; //Set pathMovement to be the newly made location of the braking text file
                turnMovement = recordDirectory + "\\Turning.txt"; //Set turnMovement to be the newly made location of the turning text file

                File.Create(pathMovement); //Create the new braking file
                File.Create(turnMovement); //Create the new turning file

                break; //Exit the for loop
            }
            else
            {
                fileCounter++; //Increment the fileCounter by 1
            }
        }

        numberOfRecords = fileCounter; //Set the public numberOfRecords variable to store the number of files
    }
}