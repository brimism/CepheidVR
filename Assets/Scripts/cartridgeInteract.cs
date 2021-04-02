using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cartridgeInteract : MonoBehaviour
{
    public float angle;
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collision");
        if(other.gameObject.tag == "GeneXScan")
        {
            float angle = Vector3.Angle(other.transform.forward, transform.forward);
            Debug.Log(angle);
            if (angle < 15.0f)
                SendMessageUpwards("scanCartridge");
        }
        else if (other.gameObject.tag == "tableCartridge")
        {
            SendMessageUpwards("putDownCartridge");
        }
        else if (other.gameObject.tag == "GeneXpertCartridge")
        {
            SendMessageUpwards("insertCartridge");
        }
        else if (other.gameObject.tag == "Trash")
        {
            SendMessageUpwards("disposeCartridge");
        }
    }
}
