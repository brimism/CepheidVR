﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tubeInteract : MonoBehaviour
{
    public GameObject barcode;
    LineRenderer lineRenderer;

    // Start is called before the first frame update

    public void Start()
    {
        //lineRenderer = this.GetComponent<LineRenderer>();
    }

    public void Update()
    {
        //lineRenderer.positionCount=2;
        //lineRenderer.SetPosition(0, transform.position);
        //lineRenderer.SetPosition(1, transform.forward * 20 + transform.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cap")
        {
            //Debug.Log("cap trigger");
            this.gameObject.SendMessageUpwards("closeTube");
        }
        else if (other.tag == "Trash")
        {
            SendMessageUpwards("disposeTube");
            //Destroy(this.gameObject, 0.2f);
        }
        else if (other.tag == "GeneXScan")
        {
            float angle = Vector3.Angle(other.transform.forward, barcode.transform.forward);
            Debug.Log(angle);
            if (angle >0.0f && angle < 90.0f)
                SendMessageUpwards("scanTube");
        }
    }
}
