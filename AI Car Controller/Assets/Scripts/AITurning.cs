using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using System.IO;

public class AITurning : MonoBehaviour
{
    [SerializeField]
    private GameObject theCar; //Used to store the car gameobject so it's components can be viewed
    public string recordDirectory { get; private set; } //Used to store the location of the logged data to be read
    public float fastSpeed { private set; get; } //Used to store the speed that determines if the car is currently going fast or not
    public float mediumSpeed { private set; get; } //Used to store the speed that determines if the car is currently going at a medium speed or not
    public float slowSpeed { private set; get; } //Used to store the speed that determines if the car is currently going slow or not
    public float largeTurn { private set; get; } //Used to store the amount of turning that determines if the car is currently turning a lot
    public float mediumTurn { private set; get; } //Used to store the amount of turning that determines if the car is currently turning a medium amount
    public float smallTurn { private set; get; } //Used to store the amount of turning that determines if the car is currently turning a small amount
    public float yesTurnFast { private set; get; } //Used to store the probability of the car turning while going fast
    public float yesTurnNormal { private set; get; } //Used to store the probability of the car turning while going at a normal speed
    public float yesTurnSlow { private set; get; } //Used to store the probability of the car turning while going slow
    public float yesTurnFar { private set; get; } //Used to store the probability of the car turning while being far away from the corner
    public float yesTurnMedium { private set; get; } //Used to store the probability of the car turning while being a medium distance away from the corner
    public float yesTurnClose { private set; get; } //Used to store the probability of the car turning while being close to the corner
    public float yesTurn { private set; get; } //Used to store the amount of times the files say the car would turn divided by the total number of lines saved to the text files
    public float noTurnFast { private set; get; } //Used to store the probability of the car not turning while going fast
    public float noTurnNormal { private set; get; } //Used to store the probability of the car not turning while going at a normal speed
    public float noTurnSlow { private set; get; } //Used to store the probability of the car not turning while going slow
    public float noTurnFar { private set; get; } //Used to store the probability of the car not turning while being far away from the corner
    public float noTurnMedium { private set; get; } //Used to store the probability of the car not turning while being at a medium distance away from the corner
    public float noTurnClose { private set; get; } //Used to store the probability of the car not turning while being close to the corner
    public float noTurn { private set; get; } //Used to store the amount of times the files say the car wouldn't turn divided by the total number of lines saved to the text files

    public float ySTFast { private set; get; } //Used to store the probability of the car turning a small amount while going fast
    public float ySTNormal { private set; get; } //Used to store the probability of the car turning a small amount while going at a normal speed
    public float ySTSlow { private set; get; } //Used to store the probability of the car turning a small amount while going slow
    public float ySTFar { private set; get; } //Used to store the probability of the car turning a small amount while being far away from the corner
    public float ySTMedium { private set; get; } //Used to store the probability of the car turning a small amount while being a medium distance away from the corner
    public float ySTClose { private set; get; } //Used to store the probability of the car turning a small amount while being close to the corner
    public float yST { private set; get; } //Used to store the amount of times the files say the car would turn a small amount divided by the total number of yesTurns

    public float yMTFast { private set; get; } //Used to store the probability of the car turning a medium amount while going fast
    public float yMTNormal { private set; get; } //Used to store the probability of the car turning a medium amount while going at a normal speed
    public float yMTSlow { private set; get; } //Used to store the probability of the car turning a medium amount while going slow
    public float yMTFar { private set; get; } //Used to store the probability of the car turning a medium amount while being far away from the corner
    public float yMTMedium { private set; get; } //Used to store the probability of the car turning a medium amount while being a medium distance away from the corner
    public float yMTClose { private set; get; } //Used to store the probability of the car turning a medium amount while being close to the corner
    public float yMT { private set; get; } //Used to store the amount of times the files say the car would turn a medium amount divided by the total number of yesTurns

    public float yLTFast { private set; get; } //Used to store the probability of the car turning a large amount while going fast
    public float yLTNormal { private set; get; } //Used to store the probability of the car turning a large amount while going at a normal speed
    public float yLTSlow { private set; get; } //Used to store the probability of the car turning a large amount while going slow
    public float yLTFar { private set; get; } //Used to store the probability of the car turning a large amount while being far away from the corner
    public float yLTMedium { private set; get; } //Used to store the probability of the car turning a large amount while being a medium distance away from the corner
    public float yLTClose { private set; get; } //Used to store the probability of the car turning a large amount while being close to the corner
    public float yLT { private set; get; } //Used to store the amount of times the files say the car would turn a large amount divided by the total number of yesTurns

    // Start is called before the first frame update
    void Start()
    {
        List<float> speedList = new List<float>(); //Used to store the speed values read in from the log files
        List<string> trueSpeedList = new List<string>(); //Used to store the speed values in string form

        List<string> distanceList = new List<string>(); //Used to store the distance values read in from the log files
        List<string> trueDistanceList = new List<string>(); //Used to store the distance values in string form

        List<float> turnList = new List<float>(); //Used to store the turn values read in from the log files
        List<string> trueTurnList = new List<string>(); //Used to store the turn values in string form

        //Variables used to store the total amount of each from the data read in from the files
        float ySFast = 0, ySNormal = 0, ySSlow = 0;
        float ySFar = 0, ySMedium = 0, ySClose = 0;
        float yS = 0;

        float yMFast = 0, yMNormal = 0, yMSlow = 0;
        float yMFar = 0, yMMedium = 0, yMClose = 0;
        float yM = 0;

        float yLFast = 0, yLNormal = 0, yLSlow = 0;
        float yLFar = 0, yLMedium = 0, yLClose = 0;
        float yL = 0;

        float yTurnFast = 0, yTurnNormal = 0, yTurnSlow = 0;
        float yTurnFar = 0, yTurnMedium = 0, yTurnClose = 0;

        float yTurn = 0;

        float nTurnFast = 0, nTurnNormal = 0, nTurnSlow = 0;
        float nTurnFar = 0, nTurnMedium = 0, nTurnClose = 0;

        float nTurn = 0;

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
            string path = Application.dataPath + "\\Recorded Data\\Record" + i + "\\Turning.txt"; //Set path to the location of the Braking text file in the current record folder being looked at

            //Open the file to be read from
            using (StreamReader sr = new StreamReader(path))
            {
                //Loop while the end of the file hasn't been found
                while (sr.Peek() >= 0)
                {
                    splitRecord = sr.ReadLine().Split(','); //Split the line read in from the file at the commas

                    distanceList.Add(splitRecord[0]); //Add the first part of the line to the distanceList
                    speedList.Add(float.Parse(splitRecord[1])); //Add the second part of the line to the speedList after converting it to a float
                    turnList.Add(float.Parse(splitRecord[2])); //Add the third part of the line to the turnList

                    //Check if the turn value read in from the file is a value other than 0
                    if (float.Parse(splitRecord[2]) != 0.0)
                    {
                        trueTurnList.Add("True"); //Add the text True to the trueTurnList list
                    }
                    else
                    {
                        trueTurnList.Add("False"); //Add the text False to the trueTurnList list
                    }
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
            //Check if the current record being looked at has the car turning
            if (trueTurnList[i] == "True")
            {
                yTurn++; //Increment yTurn by 1

                //Check if the current record being looked at has the car moving slowly
                if (speedList[i] <= slowSpeed)
                {
                    //Add the string SLOW to the trueSpeedList
                    trueSpeedList.Add("SLOW");
                    yTurnSlow++; //Increment yTurnSlow by 1
                }
                //Check if the current record being looked at has the car moving fast
                else if (speedList[i] >= fastSpeed)
                {
                    //Add the string FAST to the trueSpeedList
                    trueSpeedList.Add("FAST");
                    yTurnFast++; //Increment yTurnFast by 1
                }
                else
                {
                    //Add the string NORMAL to the trueSpeedList
                    trueSpeedList.Add("NORMAL");
                    yTurnNormal++; //Increment yTurnNormal by 1
                }

                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 1" || distanceList[i] == "Waypoint 2")
                {
                    //Add the string FAR to the trueDistanceList
                    trueDistanceList.Add("FAR");
                    yTurnFar++; //Increment yTurnFar by 1
                }
                //Check if the current record being looked at has the car at Waypoint 3 or Waypoint 4
                if (distanceList[i] == "Waypoint 3" || distanceList[i] == "Waypoint 4")
                {
                    //Add the string MEDIUM to the trueDistanceList
                    trueDistanceList.Add("MEDIUM");
                    yTurnMedium++; //Increment yTurnMedium by 1
                }
                //Check if the current record being looked at has the car at Waypoint 5 or Waypoint 6
                if (distanceList[i] == "Waypoint 5" || distanceList[i] == "Waypoint 6")
                {
                    //Add the string CLOSE to the trueDistanceList
                    trueDistanceList.Add("CLOSE");
                    yTurnClose++; //Increment yTurnClose by 1
                }
            }
            else
            {
                nTurn++; //Increment nTurn by 1

                //Check if the current record being looked at has the car moving slowly
                if (speedList[i] <= slowSpeed)
                {
                    //Add the string SLOW to the trueSpeedList
                    trueSpeedList.Add("SLOW");
                    nTurnSlow++; //Increment nTurnSlow by 1
                }
                //Check if the current record being looked at has the car moving fast
                else if (speedList[i] >= fastSpeed)
                {
                    //Add the string FAST to the trueSpeedList
                    trueSpeedList.Add("FAST");
                    nTurnFast++; //Increment nTurnFast by 1
                }
                else
                {
                    //Add the string NORMAL to the trueSpeedList
                    trueSpeedList.Add("NORMAL");
                    nTurnNormal++; //Increment nTurnNormal by 1
                }

                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 1" || distanceList[i] == "Waypoint 2")
                {
                    //Add the string FAR to the trueDistanceList
                    trueDistanceList.Add("FAR");
                    nTurnFar++; //Increment nTurnFar by 1
                }
                //Check if the current record being looked at has the car at Waypoint 3 or Waypoint 4
                if (distanceList[i] == "Waypoint 3" || distanceList[i] == "Waypoint 4")
                {
                    //Add the string MEDIUM to the trueDistanceList
                    trueDistanceList.Add("MEDIUM");
                    nTurnMedium++; //Increment nTurnMedium by 1
                }
                //Check if the current record being looked at has the car at Waypoint 5 or Waypoint 6
                if (distanceList[i] == "Waypoint 5" || distanceList[i] == "Waypoint 6")
                {
                    //Add the string CLOSE to the trueDistanceList
                    trueDistanceList.Add("CLOSE");
                    nTurnClose++; //Increment nTurnClose by 1
                }
            }
        }

        yesTurn = yTurn / distanceList.Count; //Calculate the probability of turning
        noTurn = nTurn / distanceList.Count; //Calculate the probability of not turning

        yesTurnClose = yTurnClose / yTurn; //Calculate the probability of turning while the car is close
        yesTurnMedium = yTurnMedium / yTurn; //Calculate the probability of turning while the car is at a medium distance
        yesTurnFar = yTurnFar / yTurn; //Calculate the probability of turning while the car is far

        noTurnClose = nTurnClose / nTurn; //Calculate the probability of not turning while the car is close
        noTurnMedium = nTurnMedium / nTurn; //Calculate the probability of not turning while the car is at a medium distance
        noTurnFar = nTurnFar / nTurn; //Calculate the probability of not turning while the car is far

        yesTurnFast = yTurnFast / yTurn; //Calculate the probability of turning while the car is fast
        yesTurnNormal = yTurnNormal / yTurn; //Calculate the probability of turning while the car is moving at a medium speed
        yesTurnSlow = yTurnSlow / yTurn; //Calculate the probability of turning while the car is slow

        noTurnFast = nTurnFast / nTurn; //Calculate the probability of not turning while the car is fast
        noTurnNormal = nTurnNormal / nTurn; //Calculate the probability of not turning while the car is moving at a medium speed
        noTurnSlow = nTurnSlow / nTurn; //Calculate the probability of not turning while the car is slow

        float maxTurn = 0; //Used to store the largest turn recorded
        float averageTurn = 0; //Used to store the average turning value
        float minTurn = 0; //Used to store the smallest turn recorded
        float posTurn; //Used as a temporary value equaling each turnList as needed

        //Loop for the number of items in the turnList
        for (int i = 0; i < turnList.Count; i++)
        {
            //set posTurn to be the value of the current turnList being looked at
            posTurn = turnList[i];

            //Check if the turn value is a negative value
            if (posTurn < 0)
                posTurn *= -1; //Make posTurn into a positive value

            averageTurn += posTurn; //Add posTurn to the averageTurn

            //Check if posTurn is greater than the current highest recorded turn
            if (posTurn > maxTurn)
                maxTurn = posTurn; //Set maxTurn to be equal to posTurn
            else
            {
                //Check if posTurn is less than the lowest recorded turn but still greater than 0
                if (posTurn < minTurn && posTurn > 0.0)
                    minTurn = posTurn; //Set minTurn to be equal to posTurn
                else if (minTurn == 0.0) //Check if minTurn currently equals 0
                    minTurn = posTurn; //Set minTurn to be equal to posTurn
            }
        }

        averageTurn /= turnList.Count; //Divide all the turns added together by the total number of turns to get the average

        dif = averageTurn - minTurn; //Set dif to be the difference between the averageTurn and the minTurn

        smallTurn = averageTurn - (dif / 2); //Calculate smallTurn by subtracting half of dif from the averageTurn

        dif = maxTurn - averageTurn; //Set dif to be the difference between the maxTurn and the averageTurn

        largeTurn = averageTurn + (dif / 2); //Calculate largeTurn by adding half of dif to the averageTurn

        //Loop for the total number of turns in the turnList
        for (int i = 0; i < turnList.Count; i++)
        {
            //Check if the turn value being looked at is a negative
            if (turnList[i] < 0)
                turnList[i] *= -1; //Set the value to be a positive

            //Check if the turn being looked at is a large turn
            if (turnList[i] >= largeTurn)
            {
                yL++; //Increment yL by 1

                //Check if the current record being looked at has the car moving slowly
                if (speedList[i] <= slowSpeed)
                {
                    yLSlow++; //Increment yLSlow by 1
                }
                //Check if the current record being looked at has the car moving fast
                else if (speedList[i] >= fastSpeed)
                {
                    yLFast++; //Increment yLFast by 1
                }
                else
                {
                    yLNormal++; //Increment yLNormal by 1
                }

                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 1" || distanceList[i] == "Waypoint 2")
                {
                    yLFar++; //Increment yLFar by 1
                }
                //Check if the current record being looked at has the car at Waypoint 3 or Waypoint 4
                if (distanceList[i] == "Waypoint 3" || distanceList[i] == "Waypoint 4")
                {
                    yLMedium++; //Increment yLMedium by 1
                }
                //Check if the current record being looked at has the car at Waypoint 5 or Waypoint 6
                if (distanceList[i] == "Waypoint 5" || distanceList[i] == "Waypoint 6")
                {
                    yLClose++; //Increment yLClose by 1
                }
            }
            //Check if the turn being looked at is a small turn and is greater than 0
            else if (turnList[i] <= smallTurn && turnList[i] > 0)
            {
                yS++; //Increment yS by 1

                //Check if the current record being looked at has the car moving slowly
                if (speedList[i] <= slowSpeed)
                {
                    ySSlow++; //Increment ySSlow by 1
                }
                //Check if the current record being looked at has the car moving fast
                else if (speedList[i] >= fastSpeed)
                {
                    ySFast++; //Increment ySFast by 1
                }
                else
                {
                    ySNormal++; //Increment ySNormal by 1
                }

                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 1" || distanceList[i] == "Waypoint 2")
                {
                    ySFar++; //Increment ySFar by 1
                }
                //Check if the current record being looked at has the car at Waypoint 3 or Waypoint 4
                if (distanceList[i] == "Waypoint 3" || distanceList[i] == "Waypoint 4")
                {
                    ySMedium++; //Increment ySMedium by 1
                }
                //Check if the current record being looked at has the car at Waypoint 5 or Waypoint 6
                if (distanceList[i] == "Waypoint 5" || distanceList[i] == "Waypoint 6")
                {
                    ySClose++; //Increment ySClose by 1
                }
            }
            //Check if the turn being looked at is a medium turn
            else if (turnList[i] < largeTurn && turnList[i] > smallTurn)
            {
                yM++; //Increment yM by 1

                //Check if the current record being looked at has the car moving slowly
                if (speedList[i] <= slowSpeed)
                {
                    yMSlow++; //Increment yMSlow by 1
                }
                //Check if the current record being looked at has the car moving fast
                else if (speedList[i] >= fastSpeed)
                {
                    yMFast++; //Increment yMFast by 1
                }
                else
                {
                    yMNormal++; //Increment yMNormal by 1
                }

                //Check if the current record being looked at has the car at Waypoint 1 or Waypoint 2
                if (distanceList[i] == "Waypoint 1" || distanceList[i] == "Waypoint 2")
                {
                    yMFar++; //Increment yMFar by 1
                }
                //Check if the current record being looked at has the car at Waypoint 3 or Waypoint 4
                if (distanceList[i] == "Waypoint 3" || distanceList[i] == "Waypoint 4")
                {
                    yMMedium++; //Increment yMMedium by 1
                }
                //Check if the current record being looked at has the car at Waypoint 5 or Waypoint 6
                if (distanceList[i] == "Waypoint 5" || distanceList[i] == "Waypoint 6")
                {
                    yMClose++; //Increment yMClose by 1
                }
            }
        }

        yST = yS / yTurn; //Calculate the probability of the car turning a small amount when turning
        yMT = yM / yTurn; //Calculate the probability of the car turning a medium amount when turning
        yLT = yL / yTurn; //Calculate the probability of the car turning a large amount when turning

        ySTClose = ySClose / yS; //Calculate the probability of the car turning a small amount while the car is close and turning
        ySTMedium = ySMedium / yS; //Calculate the probability of the car turning a small amount while the car is at a medium distance and turning
        ySTFar = ySFar / yS; //Calculate the probability of the car turning a small amount while the car is far and turning

        yMTClose = yMClose / yM; //Calculate the probability of the car turning a medium amount while the car is close and turning
        yMTMedium = yMMedium / yM; //Calculate the probability of the car turning a medium amount while the car is at a medium distance and turning
        yMTFar = yMFar / yM; //Calculate the probability of the car turning a medium amount while the car is far and turning

        yLTClose = yLClose / yL; //Calculate the probability of the car turning a large amount while the car is close and turning
        yLTMedium = yLMedium / yL; //Calculate the probability of the car turning a large amount while the car is at a medium distance and turning
        yLTFar = yLFar / yL; //Calculate the probability of the car turning a large amount while the car is far and turning

        ySTFast = ySFast / yS; //Calculate the probability of the car turning a small amount while the car is fast and turning
        ySTNormal = ySNormal / yS; //Calculate the probability of the car turning a small amount while the car is moving at a normal speed and turning
        ySTSlow = ySSlow / yS; //Calculate the probability of the car turning a small amount while the car is slow and turning

        yMTFast = yMFast / yM; //Calculate the probability of the car turning a medium amount while the car is fast and turning
        yMTNormal = yMNormal / yM; //Calculate the probability of the car turning a medium amount while the car is moving at a normal speed and turning
        yMTSlow = yMSlow / yM; //Calculate the probability of the car turning a medium amount while the car is slow and turning

        yLTFast = yLFast / yL; //Calculate the probability of the car turning a large amount while the car is fast and turning
        yLTNormal = yLNormal / yL; //Calculate the probability of the car turning a large amount while the car is moving at a normal speed and turning
        yLTSlow = yLSlow / yL; //Calculate the probability of the car turning a large amount while the car is slow and turning

        Debug.Log("TEST VALUES");
        Debug.Log("SMALL TURNING: " + yST + " MEDIUM TURNING: " + yMT + " LARGE TURNING: " + yLT);

        Debug.Log("Turn Fast Small: " + ySTFast + " Turn Fast Medium: " + yMTFast + " Turn Fast Large" + yLTFast);
        Debug.Log("Turn Normal Small: " + ySTNormal + " Turn Normal Medium: " + yMTNormal + " Turn Normal Large" + yLTNormal);
        Debug.Log("Turn Slow Small: " + ySTSlow + " Turn Slow Medium: " + yMTSlow + " Turn Slow Large" + yLTSlow);

        Debug.Log("Turn Close Small: " + ySTClose + " Turn Close Medium: " + yMTClose + " Turn Close Large" + yLTClose);
        Debug.Log("Turn Medium Small: " + ySTMedium + " Turn Medium Medium: " + yMTMedium + " Turn Medium Large" + yLTMedium);
        Debug.Log("Turn Far Small: " + ySTFar + " Turn Far Medium: " + yMTFar + " Turn Far Large" + yLTFar);
    }
}