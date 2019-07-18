using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class AIController : MonoBehaviour
    {
        private int turnCount = 0; //Used to store the turn the car is on
        private bool nextTurn = true; //Used to tell the car when it's at the next turn

        [SerializeField]
        private GameObject theCar; //Used to store the car gameobject so it's components can be viewed

        [SerializeField]
        private GameObject finish; //Used to store the finish gameobject so it's components can be viewed

        private CarController m_Car; // the car controller we want to use

        private AIBraking aiBraking; //Used to store the aiBraking script component

        private AITurning aiTurning; //Used to store the aiTurning script component

        private AICarDistanceHolder aiCarDistanceHolder; //Used to store the aiCarDistanceHolder script component

        private float correctAngleChecker; //Used to determine if the AI car needs to turn
        public string distance { get; set; } //Used to determine at what point on the corner the car is
        public float angleCorrect { get; set; } //Used to tell the car what angle it needs to get to when exiting the corner
        public double speed { get; set; } //Used to store the current speed of the car
        public bool finishLine { get; set; } //Used to determine if the car has crossed the finish line
        public int lapCount { get; private set; } //Used to store the number of laps the car has completed
        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            aiBraking = GetComponent<AIBraking>();
            aiTurning = GetComponent<AITurning>();
            aiCarDistanceHolder = GetComponent<AICarDistanceHolder>();

            aiCarDistanceHolder.endCorner = false;
        }


        private void FixedUpdate()
        {
            angleCorrect = -1; //Initialise the angleCorrect value to -1
            finishLine = false; //Set finishLine to false as the car doesn't start at the finish line

            string carSpeed; //Usd to store the current speed of the car as a string eg. fast, slow, normal
            string carDistance; //Usd to store the current distance of the car as a string eg. close, far, medium

            List<float> yesBrake = new List<float>(); //Used to store all the necessary values for the naive bayes calculations for braking
            List<float> noBrake = new List<float>(); //Used to store all the necessary values for the naive bayes calculations for not braking

            List<float> yesTurn = new List<float>(); //Used to store all the necessary values for the naive bayes calculations for turning
            List<float> noTurn = new List<float>(); //Used to store all the necessary values for the naive bayes calculations for not turning

            List<float> yST = new List<float>(); //Used to store all the necessary values for the naive bayes calculations for small turning
            List<float> yMT = new List<float>(); //Used to store all the necessary values for the naive bayes calculations for medium turning
            List<float> yLT = new List<float>(); //Used to store all the necessary values for the naive bayes calculations for large turning

            double brake = 0.0f; //Used for the final calculation of Naive Bayes for yes braking
            double dontBrake = 0.0f; //Used for the final calculation of Naive Bayes for no braking

            double turn = 0.0f; //Used for the final calculation of Naive Bayes for yes turning
            double dontTurn = 0.0f; //Used for the final calculation of Naive Bayes for no turning

            double sTurn = 0.0f; //Used for the final calculation of Naive Bayes for small turning
            double mTurn = 0.0f; //Used for the final calculation of Naive Bayes for medium turning
            double lTurn = 0.0f; //Used for the final calculation of Naive Bayes for large turning

            distance = aiCarDistanceHolder.distance; //Grab the distance stored in the aiCarDistanceHolder script

            //Set the finishLine to be the same as the finishLine in the aiCarDistanceHolder script
            finishLine = aiCarDistanceHolder.finishLine;

            //Calculate the current speed of the AI car using pythagoras
            speed = (float)System.Math.Sqrt((theCar.GetComponent<Rigidbody>().velocity.x *
    theCar.GetComponent<Rigidbody>().velocity.x) + (theCar.GetComponent<Rigidbody>().velocity.y *
    theCar.GetComponent<Rigidbody>().velocity.y) + (theCar.GetComponent<Rigidbody>().velocity.z *
    theCar.GetComponent<Rigidbody>().velocity.z));

            //Check if the AI car is moving slow, fast or in between
            //Then set carSpeed to SLOW, FAST, or NORMAL depending on the speed
            if (speed <= aiBraking.slowSpeed)
                carSpeed = "SLOW";
            else if (speed >= aiBraking.fastSpeed)
                carSpeed = "FAST";
            else
                carSpeed = "NORMAL";

            //Check if the AI car has gone around every corner and then set the turnCount back to 0
            if (turnCount >= 8)
                turnCount = 0;
                
            
            //Check if the AI car has made contact with the start of a corner and nextTurn is true
            if (distance == "Waypoint 1" && nextTurn)
            {
                //Check if the finish line is moved out of position
                if (finish.transform.position.y < 0)
                {
                    //Move the finish line up by 100
                    finish.transform.Translate(0, 100, 0, Space.Self);
                }

                turnCount++; //Increment the number of turns by 1
                correctAngleChecker = 0; //Set correctAngleChecker to 0
                nextTurn = false; //Reset nextTurn to false
            }
            
            //Check if the AI car has reached the final waypoint of the corner
            if (distance == "Waypoint 6")
            {
                correctAngleChecker = 0; //Set correctAngleChecker to 0
                nextTurn = true; //Set nextTurn to true
            }

            //Check if the AI car has finished the corner
            if (distance == "End of Corner")
            {
                //Set angleCorrect to the same angle as the finish line's y rotation
                angleCorrect = aiCarDistanceHolder.angleCorrect;
            }

            //Check if finishLine is ture, which means the AI car has hit the finish line
            if (finishLine)
            {
                lapCount++; //Increment the lapCount by 1
                finishLine = false; //Set finishLine to false
                aiCarDistanceHolder.finishLine = false; //Set the finshLine in the aiCarDistanceHolder to false
            }

            //Check if the last waypoint the AI car hit was waypoint 1 or waypoint 2
            if (distance == "Waypoint 1" || distance == "Waypoint 2")
                carDistance = "FAR"; //Set carDistance to FAR
            //Check if the last waypoint the AI car hit was waypoint 3 or waypoint 4
            else if (distance == "Waypoint 3" || distance == "Waypoint 4")
                carDistance = "MEDIUM"; //Set carDistance to MEDIUM
            else
                carDistance = "CLOSE"; //Set carDistance to CLOSE

            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            //Check if the distance isn't null
            if (distance != null)
            {
                //Add the value calculated in the yesTurn variable in the aiTurning script to the yesTurn list
                yesTurn.Add(aiTurning.yesTurn);
                //Add the value calculated in the noTurn variable in the aiTurning script to the noTurn list
                noTurn.Add(aiTurning.noTurn);

                //Check if the AI car is moving slowly
                if (carSpeed == "SLOW")
                {
                    //Add the value calculated in the yesTurnSlow variable in the aiTurning script to the yesTurn list
                    yesTurn.Add(aiTurning.yesTurnSlow);
                    //Add the value calculated in the noTurnSlow variable in the aiTurning script to the noTurn list
                    noTurn.Add(aiTurning.noTurnSlow);
                }
                //Check if the AI car is moving fast
                else if (carSpeed == "FAST")
                {
                    //Add the value calculated in the yesTurnFast variable in the aiTurning script to the yesTurn list
                    yesTurn.Add(aiTurning.yesTurnFast);
                    //Add the value calculated in the noTurnFast variable in the aiTurning script to the noTurn list
                    noTurn.Add(aiTurning.noTurnFast);
                }
                //Check if the AI car is moving at a normal speed
                else
                {
                    //Add the value calculated in the yesTurnNormal variable in the aiTurning script to the yesTurn list
                    yesTurn.Add(aiTurning.yesTurnNormal);
                    //Add the value calculated in the noTurnNormal variable in the aiTurning script to the noTurn list
                    noTurn.Add(aiTurning.noTurnNormal);
                }

                //Check if the AI car is far away from the corner
                if (carDistance == "FAR")
                {
                    //Add the value calculated in the yesTurnFar variable in the aiTurning script to the yesTurn list
                    yesTurn.Add(aiTurning.yesTurnFar);
                    //Add the value calculated in the noTurnFar variable in the aiTurning script to the noTurn list
                    noTurn.Add(aiTurning.noTurnFar);
                }
                //Check if the AI car is a medium distance away from the corner
                else if (carDistance == "MEDIUM")
                {
                    //Add the value calculated in the yesTurnMedium variable in the aiTurning script to the yesTurn list
                    yesTurn.Add(aiTurning.yesTurnMedium);
                    //Add the value calculated in the noTurnMedium variable in the aiTurning script to the noTurn list
                    noTurn.Add(aiTurning.noTurnMedium);
                }
                //Check if the AI car is close to the corner
                else
                {
                    //Add the value calculated in the yesTurnClose variable in the aiTurning script to the yesTurn list
                    yesTurn.Add(aiTurning.yesTurnClose);
                    //Add the value calculated in the noTurnClose variable in the aiTurning script to the noTurn list
                    noTurn.Add(aiTurning.noTurnClose);
                }
            }

            //Loop for the number of items added to the yesTurn list
            for (int i = 0; i < yesTurn.Count; i++)
            {
                //Add the logarithms of the values stored in the yesTurn list to perform the final naive bayes calculation
                //The variables are added together in this case instead of being multiplied,
                //This is done because to multiply the logarithms of numbers they need to be added together,
                //It is required to add them together instead
                //Then do the same for the dontTurn variables
                turn += Math.Log10(yesTurn[i]);
                dontTurn += Math.Log10(noTurn[i]);
            }

            bool turning = false; //Used to store whether or not the car needs to turn, set turning to false

            //Check if turn is greater than dontTurn
            if (turn > dontTurn)
                turning = true; //Set turning to true

            //Check if distance has a value and turning is true
            if (distance != null && turning)
            {
                //Add the value calculated in the yST 
                //variable in the aiTurning script to the yST list
                yST.Add(aiTurning.yST);
                //Add the value calculated in the yMT 
                //variable in the aiTurning script to the yMT list
                yMT.Add(aiTurning.yMT);
                //Add the value calculated in the yLT 
                //variable in the aiTurning script to the yLT list
                yLT.Add(aiTurning.yLT);

                //Check if the AI car is moving slowly
                if (carSpeed == "SLOW")
                {
                    //Add the value calculated in the ySTSlow 
                    //variable in the aiTurning script to the yST list
                    //Do this for yMTSlow into yMT and yLTSlow into yLT as well
                    yST.Add(aiTurning.ySTSlow);
                    yMT.Add(aiTurning.yMTSlow);
                    yLT.Add(aiTurning.yLTSlow);
                }
                //Check if the AI car is moving fast
                else if (carSpeed == "FAST")
                {
                    //Add the value calculated in the ySTFast
                    //variable in the aiTurning script to the yST list
                    //Do this for yMTFast into yMT and yLTFast into yLT as well
                    yST.Add(aiTurning.ySTFast);
                    yMT.Add(aiTurning.yMTFast);
                    yLT.Add(aiTurning.yLTFast);
                }
                else
                {
                    //Add the value calculated in the ySTNormal 
                    //variable in the aiTurning script to the yST list
                    //Do this for yMTNormal into yMT and yLTNormal into yLT as well
                    yST.Add(aiTurning.ySTNormal);
                    yMT.Add(aiTurning.yMTNormal);
                    yLT.Add(aiTurning.yLTNormal);
                }

                //Check if the AI car is far away from the corner
                if (carDistance == "FAR")
                {
                    //Add the value calculated in the ySTFar 
                    //variable in the aiTurning script to the yST list
                    //Do this for yMTFar into yMT and yLTFar into yLT as well
                    yST.Add(aiTurning.ySTFar);
                    yMT.Add(aiTurning.yMTFar);
                    yLT.Add(aiTurning.yLTFar);
                }
                //Check if the car is a medium distance from the corner
                else if (carDistance == "MEDIUM")
                {
                    //Add the value calculated in the ySTMedium 
                    //variable in the aiTurning script to the yST list
                    //Do this for yMTMedium into yMT and yLTMedium into yLT as well
                    yST.Add(aiTurning.ySTMedium);
                    yMT.Add(aiTurning.yMTMedium);
                    yLT.Add(aiTurning.yLTMedium);
                }
                else
                {
                    //Add the value calculated in the ySTClose
                    //variable in the aiTurning script to the yST list
                    //Do this for yMTClose into yMT and yLTClose into yLT as well
                    yST.Add(aiTurning.ySTClose);
                    yMT.Add(aiTurning.yMTClose);
                    yLT.Add(aiTurning.yLTClose);
                }

                //Check if the last waypoint the car hit was waypoint 1
                //Used for debugging
                if (distance == "Waypoint 1")
                {
                    Debug.Log("CAR VALUES");
                    Debug.Log("Distance: Far");
                    if (speed >= aiBraking.fastSpeed)
                        Debug.Log("Speed: " + speed + " = Fast");
                    else if (speed <= aiBraking.slowSpeed)
                        Debug.Log("Speed: " + speed + " = Slow");
                    else
                        Debug.Log("Speed: " + speed + " = Normal");
                }

                //Loop for the number of items added to yST
                for (int i = 0; i < yST.Count; i++)
                {
                    //Add the logarithms of the values stored in the yST list to perform the final naive bayes calculation
                    //The variables are added together in this case instead of being multiplied,
                    //This is done because to multiply the logarithms of numbers they need to be added together,
                    //It is required to add them together instead
                    //Then do the same for the mTurn and lTurn variables
                    sTurn += Math.Log10(yST[i]);
                    mTurn += Math.Log10(yMT[i]);
                    lTurn += Math.Log10(yLT[i]);
                }

                //Check if the last waypoint the car hit was waypoint 1
                //Used for debugging
                if (distance == "Waypoint 1")
                {
                    Debug.Log("Values Used for a Small Turn: " + Math.Log10(yST[2]) + ", " + Math.Log10(yST[1]) + ", " + Math.Log10(yST[0]));
                    Debug.Log("Values Used for a Medium Turn: " + Math.Log10(yMT[2]) + ", " + Math.Log10(yMT[1]) + ", " + Math.Log10(yMT[0]));
                    Debug.Log("Values Used for a Large Turn: " + Math.Log10(yLT[2]) + ", " + Math.Log10(yLT[1]) + ", " + Math.Log10(yLT[0]));

                    Debug.Log("Small Turning VALUE: " + sTurn + " Medium Turning VALUE: " + mTurn + " Large Turning VALUE: " + lTurn);
                }

                //Compare the sTurn, mTurn and lTurn variables
                if (sTurn > mTurn && sTurn > lTurn)
                    Debug.Log("SMALL TURN");
                else if (mTurn > sTurn && mTurn > lTurn)
                    Debug.Log("MEDIUM TURN");
                else if (lTurn > sTurn && lTurn > mTurn)
                    Debug.Log("LARGE TURN");
            }

            //Check if distance isn't null
            if (distance != null)
            {
                //Add the value calculated in the yesBrake 
                //variable in the aiTurning script to the yesBrake list
                yesBrake.Add(aiBraking.yesBrake);
                //Add the value calculated in the noBrake 
                //variable in the aiTurning script to the noBrake list
                noBrake.Add(aiBraking.noBrake);

                //Check if the AI car is moving slowly
                if (carSpeed == "SLOW")
                {
                    //Add the value calculated in the yesBrakeSlow 
                    //variable in the aiTurning script to the yesBrake list
                    //Do this for noBrakeSlow into noBrake as well
                    yesBrake.Add(aiBraking.yesBrakeSlow);
                    noBrake.Add(aiBraking.noBrakeSlow);
                }
                //Check if the AI car is moving fast
                else if (carSpeed == "FAST")
                {
                    //Add the value calculated in the yesBrakeFast
                    //variable in the aiTurning script to the yesBrake list
                    //Do this for noBrakeFast into noBrake as well
                    yesBrake.Add(aiBraking.yesBrakeFast);
                    noBrake.Add(aiBraking.noBrakeFast);
                }
                else
                {
                    //Add the value calculated in the yesBrakeNormal
                    //variable in the aiTurning script to the yesBrake list
                    //Do this for noBrakeNormal into noBrake as well
                    yesBrake.Add(aiBraking.yesBrakeNormal);
                    noBrake.Add(aiBraking.noBrakeNormal);
                }

                //Check if the AI car is far away from the corner
                if (carDistance == "FAR")
                {
                    //Add the value calculated in the yesBrakeFar
                    //variable in the aiTurning script to the yesBrake list
                    //Do this for noBrakeFar into noBrake as well
                    yesBrake.Add(aiBraking.yesBrakeFar);
                    noBrake.Add(aiBraking.noBrakeFar);
                }
                //Check if the AI car is a meidum distance from the corner
                else if (carDistance == "MEDIUM")
                {
                    //Add the value calculated in the yesBrakeMedium
                    //variable in the aiTurning script to the yesBrake list
                    //Do this for noBrakeMedium into noBrake as well
                    yesBrake.Add(aiBraking.yesBrakeMedium);
                    noBrake.Add(aiBraking.noBrakeMedium);
                }
                else
                {
                    //Add the value calculated in the yesBrakeClose
                    //variable in the aiTurning script to the yesBrake list
                    //Do this for noBrakeClose into noBrake as well
                    yesBrake.Add(aiBraking.yesBrakeClose);
                    noBrake.Add(aiBraking.noBrakeClose);
                }
            }

            //Loop for the number of items added to yesBrake
            for (int i = 0; i < yesBrake.Count; i++)
            {
                //Add the logarithms of the values stored in the brake list to perform the final naive bayes calculation
                //The variables are added together in this case instead of being multiplied,
                //This is done because to multiply the logarithms of numbers they need to be added together,
                //It is required to add them together instead
                //Then do the same for the dontBrake variables
                brake += Math.Log10(yesBrake[i]);
                dontBrake += Math.Log10(noBrake[i]);
            }

            //Check if the car hasn't reached the end of the corner
            if (aiCarDistanceHolder.endCorner == false)
            {
                h = 0;
            }
            else
            {
                //Check if turn is greater then dontTurn
                if (turn > dontTurn)
                {
                    //Compare the sTurn, mTurn and lTurn variables
                    if (sTurn > mTurn && sTurn > lTurn)
                    {
                        //Check if the AI car is on the 3rd or 5th turn
                        if (turnCount == 3 || turnCount == 5)
                            h = -0.3f; //Set h to -0.3 so the car turns left by a small amount
                        else
                            h = 0.3f; //Set h to 0.3 so the car turns left by a small amount
                    }
                    else if (mTurn > sTurn && mTurn > lTurn)
                    {
                        //Check if the AI car is on the 3rd or 5th turn
                        if (turnCount == 3 || turnCount == 5)
                            h = -0.6f; //Set h to -0.6 so the car turns left by a medium amount
                        else
                            h = 0.6f;//Set h to 0.6 so the car turns left by a medium amount
                    }
                    else if (lTurn > sTurn && lTurn > mTurn)
                    {
                        //Check if the AI car is on the 3rd or 5th turn
                        if (turnCount == 3 || turnCount == 5)
                            h = -1.0f; //Set h to -1.0 so the car turns left by a Large amount
                        else
                            h = 1.0f; //Set h to 1.0 so the car turns left by a Large amount
                    }
                }
                //Check if dontTunr is larger than turn
                else if (dontTurn > turn)
                {
                    h = 0; //Set h to 0 so the car doesn't rotate
                }
            }
            
            //Check if the end of the corner hasn't been hit
            if (aiCarDistanceHolder.endCorner == false)
            {
                //Check if angleCorrect isn't -1 so it hasn't been altered yet
                if (angleCorrect != -1)
                {
                    float angleDif = 0; //Used to store the difference between the y rotation of the car and the end of the corner
                    float tempCarAngle = 0; //Used to store the y rotation of the car

                    //Set tempCarAngle to be the same as the y rotation of the car
                    tempCarAngle = theCar.transform.eulerAngles.y;

                    //Check if the angleCorrect is 0
                    if (angleCorrect == 0)
                    {
                        if (tempCarAngle < 360 && tempCarAngle > 300)
                            angleCorrect = 360;
                    }
                    else
                    {
                        //Check if the tempCarAngle is less than 0
                        if (tempCarAngle < 0)
                            tempCarAngle = 360 + tempCarAngle;
                    }

                    //Set angleDif to be the difference between angleCorrect and tempCarAngle
                    angleDif = angleCorrect - tempCarAngle;

                    //Check if correctAngleChecker is 0
                    if (correctAngleChecker == 0)
                        correctAngleChecker = angleDif; //Set correctAngleChecker to angleDif

                    //Check if angleDif is greater than 0
                    if (angleDif > 0)
                    {
                        //Check if the AI car is on the opposite side of the correction angle than is started at
                        //So if the angleCorrection is 180 and the car is 189
                        //then if the car's rotation changes to be less than 180 turn right slightly
                        if (correctAngleChecker < 0 && angleDif > 0 || correctAngleChecker > 0 && angleDif < 0)
                            h = 0.1f; //Set h to 0.1 so the car turns right a little bit
                        else
                            h = 0.50f; //Set h to 0.5 so the car turns right a medium amount
                    }
                    else if (angleDif < 0)
                    {
                        //Check if the AI car is on the opposite side of the correction angle than is started at
                        //So if the angleCorrection is 190 and the car is 189
                        //then if the car's rotation changes to be greater than 190 turn left slightly
                        if (correctAngleChecker < 0 && angleDif > 0 || correctAngleChecker > 0 && angleDif < 0)
                            h = -0.1f; //Set h to 0.1 so the car turns left a little bit
                        else
                            h = -0.50f; //Set h to 0.5 so the car turns left a medium amount
                    }
                    else
                        h = 0; //Set h to 0 so the car doesn't turn
                }
                else
                {
                    h = 0; //Set h to 0 so the car doesn't turn
                }
                v = 1; //Set v to 1 so the car accelerates
            }
            else
            {
                //Check if brake is grater than dontBrake
                if (brake > dontBrake)
                {
                    v = -1; //Set v to -1 so the car brakes
                }
                else if (dontBrake > brake)
                {
                    v = 1; //Set v to 1 so the car accelerates
                }
            }

#if !MOBILE_INPUT

            m_Car.Move(h, v, v, 0f);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}