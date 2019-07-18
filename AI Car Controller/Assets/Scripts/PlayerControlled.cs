using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControlled : MonoBehaviour {

    public void playerControlled()
    {
        SceneManager.LoadScene("Player Controlled"); //Load the Player car controller scene
    }

    public void aiControlled()
    {
        SceneManager.LoadScene("AI Controlled"); //Load the AI car controller scene
    }
}
