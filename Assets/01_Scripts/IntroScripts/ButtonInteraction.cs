using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonInteraction : MonoBehaviour
{
    public bool lobby;


    private int timesPressed = 0;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (lobby)
                this.SendMessageUpwards("nextRoom");
            else
            {
                this.SendMessageUpwards("teachSwipeAway");
                this.SendMessageUpwards("nextRoom");
            }
            
        }
    }
}
