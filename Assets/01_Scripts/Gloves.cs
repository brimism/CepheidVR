using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves : MonoBehaviour
{
    public Material glove;
    public Material hand;
    public AudioClip glovesOn;
    public AudioClip glovesOff;
    public AudioSource audioSource;

    public bool gloved = false;//start out with no gloves

    public void OnTriggerEnter(Collider other)
    {
        if(gloved==false){
            if (other.tag == "Gloves")
            {
                this.GetComponent<MeshRenderer>().material = glove;
                gloved = true;
                audioSource.PlayOneShot(glovesOn);
                this.gameObject.SendMessageUpwards("Gloves");
            }
        }
        if (gloved == true)
        {
            if (other.tag == "Trash")
            {
                this.GetComponent<MeshRenderer>().material = hand;
                gloved = false;
                audioSource.PlayOneShot(glovesOff);
                this.gameObject.SendMessageUpwards("Gloves");
                Debug.Log("sent message upwards");
            }
        }
    }
}
