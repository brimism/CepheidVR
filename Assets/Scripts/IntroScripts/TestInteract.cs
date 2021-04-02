using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteract : MonoBehaviour
{
    public GameObject testObj, placeholder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == placeholder)
        {
            this.SendMessageUpwards("placeObject");
        }
        if (other.gameObject == testObj)
        {
            Debug.Log("grabbed object");
            this.SendMessageUpwards("pickUpObject");
        }
        if (other.gameObject.tag == "Clipboard")
        {
            this.SendMessageUpwards("grabbedBoard");
        }
    }
}
