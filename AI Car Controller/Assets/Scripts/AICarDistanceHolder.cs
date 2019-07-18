using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarDistanceHolder : MonoBehaviour {

    public string distance { get; set; } //Used to store the last waypoint the AI car made contact with
    public float angleCorrect { get; set; } //Used to store the angle the AI car needs to correct itself to match when exiting a corner
    public bool endCorner { get; set; } //Used to store whether or not the AI car has reached the end of the corner
    public bool finishLine { get; set; } //Used to store whether or not the AI car has hit the finish line
}