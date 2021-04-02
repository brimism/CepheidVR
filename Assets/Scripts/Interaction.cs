using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Material glove;
    public Material hand;
    public AudioClip glovesOn;
    public AudioClip glovesOff;
    public AudioClip vacuumPackOpen;
    public AudioSource audioSource;
    public MeshRenderer handMesh;

    public bool gloved = false;//start out with no gloves
    

    //whenever the hands are activated and collide with a trigger, they check the tag on the object and send a broadcast to the manager script

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gloves")
        {
            if (gloved == false)
            {
                handMesh.material = glove;
                gloved = true;
                this.gameObject.SendMessageUpwards("Gloves");
            }
        }
        else if (other.tag == "Vacuum")
        {
            this.gameObject.SendMessageUpwards("vacuumPackOpen");

        }
        else if (other.tag == "swabPacket")
        {
            this.gameObject.SendMessageUpwards("swabPackOpen");
        }
        else if (other.tag == "Swab")
        {
            this.gameObject.SendMessageUpwards("pickUpSwab");
        }
        else if (other.tag == "Pressure")
        {
            this.gameObject.SendMessageUpwards("rotateSwab");
            this.gameObject.SendMessageUpwards("Rotate");
        }
        else if (other.tag == "Tube")
        {
            this.gameObject.SendMessageUpwards("pickUpTube", this.gameObject.name);
            //Debug.Log(this.gameObject.name);
        }
        else if (other.tag == "Cap")
        {
            this.gameObject.SendMessageUpwards("pickUpTubeCap");
            Debug.Log("cap");
        }
        else if (other.tag == "Trash")
        {
            if (gloved == true)
            {
                handMesh.material = hand;
                gloved = false;
                this.gameObject.SendMessageUpwards("Gloves");
                //Debug.Log("sent message upwards");
            }
        }
        else if (other.tag == "Door")
        {
            //Debug.Log("poke door");
            this.gameObject.SendMessageUpwards("openDoor");
            this.gameObject.SendMessageUpwards("FadeOut");
        }
        else if (other.tag == "CartridgeCap")
        {
            this.gameObject.SendMessageUpwards("cartridgeCap");
        }
        else if (other.tag == "Pipette")
        {
            this.gameObject.SendMessageUpwards("pickUpPipette");
        }
        else if (other.tag == "Cartridge")
        {
            this.gameObject.SendMessageUpwards("pickUpCartridge");
        }
        else if (other.tag == "Button")
        {
            Debug.Log("button pressed");
            this.gameObject.SendMessageUpwards("buttonPress");
        }
        else if (other.tag == "GeneXpertHandle")
        {
            this.SendMessageUpwards("geneXpertHandleTouch");
        }
        else if (other.tag == "GeneXpertCartridge")
        {
            this.SendMessageUpwards("pickUpCartridge");
        }
        else if (other.tag == "TestObject")
        {
            this.SendMessageUpwards("pickUpObject");
        }
        else if (other.tag == "TestPlaceholder")
        {
            this.SendMessageUpwards("placeObject");
        }
        else if (other.tag == "Chart")
        {
            this.SendMessageUpwards("pickUpChart");
        }
    }
}