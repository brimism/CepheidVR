using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pipetteInteract : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tube")
        {
            SendMessageUpwards("collectSample");
        }
        else if (other.gameObject.tag == "Cartridge")
        {
            SendMessageUpwards("depositSample");
        }
        else if (other.tag == "Trash")
        {
            SendMessageUpwards("disposePipette");
            Destroy(this.gameObject, 0.2f);
        }
    }
}
