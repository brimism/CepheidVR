using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swabInteract : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Nostril")
        {
            SendMessageUpwards("insertSwab");
        }
        else if(other.tag == "Tube")
        {
            SendMessageUpwards("breakSwab");
        }
        else if(other.tag == "Trash")
        {
            SendMessageUpwards("disposeSwab");
            Destroy(this.gameObject, 0.2f);
        }
    }
}
