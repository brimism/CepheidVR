using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vacuumPack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "VacuumPack")
        {
            SendMessageUpwards("vacuumPackOpen");
        }
    }
}
