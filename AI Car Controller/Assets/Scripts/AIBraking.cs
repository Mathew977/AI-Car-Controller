using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using System.IO;

public class AIBraking : MonoBehaviour {
    public string recordDirectory { get; private set; } //Used to store the location of the logged data to be read
    public float fastSpeed { private set; get; } //Used to store the speed that determines if the car is currently going fast or not
    public float mediumSpeed { private set; get; } //Used to store the speed that determines if the car is currently going at a medium speed or not
    public float slowSpeed { private set; get; } //Used to store the speed that determines if the car is currently going slow or not
    public float yesBrakeFast { private set; get; } //Used to store the probability of the car braking while going fast
    public float yesBrakeNormal { private set; get; } //Used to store the probability of the car braking while going at a normal speed
    public float yesBrakeSlow { private set; get; } //Used to store the probability of the car braking while going at a slow speed
    public float yesBrakeFar { private set; get; } //Used to store the probability of the car braking while being far away from the corner
    public float yesBrakeMedium { private set; get; } //Used to store the probability of the car braking while being a medium distance from the corner
    public float yesBrakeClose { private set; get; } //Used to store the probability of the car braking while being close to the corner
    public float yesBrake { private set; get; } //Used to store the amount of times the files say the car would brake divided by the total number of lines saved to the text files
    public float noBrakeFast { private set; get; } //Used to store the probability of the car not braking while going fast
    public float noBrakeNormal { private set; get; } //Used to store the probability of the car not braking while going at a normal speed
    public float noBrakeSlow { private set; get; } //Used to store the probability of the car not braking while going at a slow speed
    public float noBrakeFar { private set; get; } //Used to store the probability of the car not braking while being far away from the corner
    public float noBrakeMedium { private set; get; } //Used to store the probability of the car braking while being a medium distance from the corner
    public float noBrakeClose { private set; get; } //Used to store the probability of the car braking while being close to the corner
    public float noBrake { private set; get; } //Used to store the amount of times the files say the car wouldn't brake divided by the total number of lines saved to the text files

    // Use this for initialization
    void Start () {
        
        List<float> speedList = new List<float>(); //Used to store the speed values read in from the log files
        List<string> trueSpeedList = new List<string>(); //Used to store the speed values in string form

        List<string> distanceList = new List<string>(); //Used to store the distance values read in from the log files
        List<string> trueDistanceList = new List<string>(); //Used to store the distance values in string form

        List<string> brakeList = new List<string>(); //Used to store the brake values read in from the log files
        List<string> trueBrakeList = new List<string>(); //Used to store the brake values in string form

        //Variables used to store the total amount of each from the data read in from the files
        float yBrakeFast = 0, yBrakeNormal = 0, yBrakeSlow = 0;
        float yBrakeFar = 0, yBrakeMedium = 0, yBrakeClose = 0;
        float yBrake = 0;

        float nBrakeFast = 0, nBrakeNormal = 0, nBrakeSlow = 0;
        float nBrakeFar = 0, nBrakeMedium = 0, nBrakeClose = 0;
        float nBrake = 0;

        string[] splitRecord; //Used to split a string

        int fileCounter = 1; //Stores the total number of files to be read

        while (true)
        {
            recordDirectory = Application.dataPath + "\\Recorded Data\\Record" + fileCounter; //Set recordDirectory to the location of a record folder

            //Check if the folder doesn't exist
            if (!Directory.Exists(recordDirectory))
            {
                break; //Exit the for loop
            }
            else
            {
                fileCounter++; //Increment the filrCounter by 1
            }
        }

        //Loop for the number of files found
        for (int i = 1; i < fileCounter; i++)
        {
            string path = Application.dataPath + "\\Recorded Data\\Record" + i + "\\Braking.txt"; //Set path to the location of the Braking text file in the current record folder being looked at
            
            //Open the file to be read from
            using (StreamReader sr = new StreamReader(path))
            {
                //Loop while the end of the file hasn't been found
                while (sr.Peek() >= 0)
                {
                    splitRecord = sr.ReadLine().Split(','); //Split the line read in from the file at the commas

                    distanceList.Add(splitRecord[0]); //Add the first part of the line to the distanceList
                    speedList.Add(float.Parse(splitRecord[1])); //Add the second part of the line to the speedList after converting it to a float
                    brakeList.Add(splitRecord[2]); //Add the third part of the line to the brakeList
                }
            }
        }

        float averageSpeed = 0; //Used to store the average speed of the all the speeds read in from the files
        float maxSpeed = 0; //Used to store the max speed calculated using the average speed and the fastest speed
        float minSpeed = 0; //Used to store the minimum speed calculated using the average speed and the slowest speed

        //Loop for the number of items in the speedList
        for (int i = 0; i < speedList.Count; i++)
        {
            averageSpeed += speedList[i]; //Add the speed being looked at to the averageSpeed

            //Check if the speed being looked at is larger than the current stored maxSpeed
            if (speedList[i] > maxSpeed)
                maxSpeed = speedList[i]; //Set maxSpeed to the speed being looked at
            else
            {
                //Check if the speed being looked at is less than the current stored minSpeed
                if (speedList[i] < minSpeed)
                    minSpeed = speedList[i]; //Set minSpeed to the speed being looked at
                else if (minSpeed == 0) //Check if this is the first speed being looked
                    minSpeed = speedList[i]; //Set the starting value of minSpeed
            }
        }

        float dif; //Used to calculate the mediumSpeed and the slowSpeed

        averageSpeed /= speedList.Count; //Divide all the speeds added together by the total number of speeds to get the average speed

        mediumSpeed = averageSpeed; //Set mediumSpeed to be the same as the averageSpeed

        dif = mediumSpeed - minSpeed; //Calculate the difference between the mediumSpeed and the minSpeed

        slowSpeed = averageSpeed - (dif / 2); //Calculate slowSpeed by subtracted the difference between the mediumSpeed and minSpeed divided by 2 from the averageSpeed to give a range (Slowest speed to this value = slow)

        dif = maxSpeed - mediumSpeed; //Calculate the difference between the mediumSpeed and the maxSpeed

        fastSpeed = averageSpeed + (dif / 2); //Calculate fastSpeed by subtracted the difference between the mediumSpeed and maxSpeed divided by 2 from the averageSpeed to give a range (Fastest speed to this value = fast)

        //Loop for the number of distances stored
        for (int i = 0; i < distanceList.Count; i++)
        {
            //Check if the current record being looked at has the car braking
            if (brakeList[i] == "True" || brakeList[i] == "paused")
            {
                //Add the string BRAKE to the trueBrakeList
                trueBrakeList.Add("BRAKE");
                yBrake++; //Increment the yBrake value by 1

                //Check if the current record being looked at has the car moving slowly
                if (speedList[i] <= slowSpeed)
                {
                    //Add the string SLOW to the trueSpeedList
                    trueSpeedList.Add("SLOW");
                    yBrakeSlow++; //Increment the yBrakeSlow by 1
                }
                //Check if the current record being looked at has the car moving fast
                else if (speedList[i] >= fastSpeed)
                {
                    //Add the string FAST to the trueSpeedList
                    trueSpeedList.Add("FAST");
                    yBrakeFast++; //Increment the yBrakeFast by 1
                }
                else
                {
                    //Add the string NORMAL to the trueSpeedList
                    trueSpeedList.Add("NORMAL");
                    yBrakeNormal++; //Increment the yBrakeNormal by 1
                }
                
                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 1" || distanceList[i] == "Waypoint 2")
                {
                    //Add the string FAR to the trueDistanceList
                    trueDistanceList.Add("FAR");
                    yBrakeFar++; //Increment the yBrakeFar by 1
                }
                //Check if the current record being looked at has the car at Waypoint 3 or Waypoint 4
                if (distanceList[i] == "Waypoint 3" || distanceList[i] == "Waypoint 4")
                {
                    //Add the string MEDIUM to the trueDistanceList
                    trueDistanceList.Add("MEDIUM");
                    yBrakeMedium++; //Increment the yBrakeMedium by 1
                }
                //Check if the current record being looked at has the car at Waypoint 5 or Waypoint 6
                if (distanceList[i] == "Waypoint 5" || distanceList[i] == "Waypoint 6")
                {
                    //Add the string CLOSE to the trueDistanceList
                    trueDistanceList.Add("CLOSE");
                    yBrakeClose++; //Increment the yBrakeClose by 1
                }
            }
            else
            {
                //Add the string ACCELERATE to the trueBrakeList
                trueBrakeList.Add("ACCELERATE");
                nBrake++; //Increment the nBrake by 1

                //Check if the current record being looked at has the car moving slowly
                if (speedList[i] <= slowSpeed)
                {
                    //Add the string SLOW to the trueSpeedList
                    trueSpeedList.Add("SLOW");
                    nBrakeSlow++; //Increment nBrakeSlow by 1
                }
                //Check if the current record being looked at has the car moving fast
                else if (speedList[i] >= fastSpeed)
                {
                    //Add the string FAST to the trueSpeedList
                    trueSpeedList.Add("FAST");
                    nBrakeFast++; //Increment nBrakeFast by 1
                }
                else
                {
                    //Add the string NORMAL to the trueSpeedList
                    trueSpeedList.Add("NORMAL");
                    nBrakeNormal++; //Increment nBrakeNormal by 1
                }
                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 1" || distanceList[i] == "Waypoint 2")
                {
                    //Add the string Far to the trueDistanceList
                    trueDistanceList.Add("FAR");
                    nBrakeFar++;
                }
                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 3" || distanceList[i] == "Waypoint 4")
                {
                    //Add the string Far to the trueDistanceList
                    trueDistanceList.Add("MEDIUM");
                    nBrakeMedium++; //Increment nBrakeMedium by 1
                }
                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 5" || distanceList[i] == "Waypoint 6")
                {
                    //Add the string Far to the trueDistanceList
                    trueDistanceList.Add("CLOSE");
                    nBrakeClose++; //Increment nBrakeClose by 1
                }
            }
        }
        
        yesBrake = yBrake / distanceList.Count; //Calculate the probability of braking
        noBrake = nBrake / distanceList.Count; //Calculate the probability of not braking

        yesBrakeClose = yBrakeClose / yBrake; //Calculate the probability of braking while the car is close
        yesBrakeMedium = yBrakeMedium / yBrake; //Calculate the probability of braking while the car is at a medium distance
        yesBrakeFar = yBrakeFar / yBrake; //Calculate the probability of braking while the car is far

        noBrakeClose = nBrakeClose / nBrake; //Calculate the probability of not braking while the car is close
        noBrakeMedium = nBrakeMedium / nBrake; //Calculate the probability of not braking while the car is at a medium distance
        noBrakeFar = nBrakeFar / nBrake; //Calculate the probability of not braking while the car is far

        yesBrakeFast = yBrakeFast / yBrake; //Calculate the probability of braking while the car is fast
        yesBrakeNormal = yBrakeNormal / yBrake; //Calculate the probability of braking while the car is moving at a medium speed
        yesBrakeSlow = yBrakeSlow / yBrake; //Calculate the probability of braking while the car is slow

        noBrakeFast = nBrakeFast / nBrake; //Calculate the probability of not braking while the car is fast
        noBrakeNormal = nBrakeNormal / nBrake; //Calculate the probability of not braking while the car is moving at a medium speed
        noBrakeSlow = nBrakeSlow / nBrake; //Calculate the probability of not braking while the car is slow
    }
}