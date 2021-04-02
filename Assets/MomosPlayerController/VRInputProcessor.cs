using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInputProcessor : MonoBehaviour
{
    public Vector3 headPos;
    public Vector3 rightHandPos;
    public Vector3 leftHandPos;
    public Quaternion headRot;
    public Quaternion rightHandRot;
    public Quaternion leftHandRot;
    public Vector2 leftHandJoystick;
    public Vector2 rightHandJoystick;
    public float leftHandTrigger;
    public float rightHandTrigger;
    public float leftHandGrip;
    public float rightHandGrip;
    public bool leftHandButtonX;
    public bool leftHandButtonY;
    public bool rightHandButtonA;
    public bool rightHandButtonB;
    public bool menuButton;
    public bool oculusButton;
    public bool leftHandJoystickPressed;
    public bool rightHandJoystickPressed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
