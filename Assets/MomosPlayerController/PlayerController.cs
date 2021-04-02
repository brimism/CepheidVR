using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public VRInputProcessor inputProcessor;
    public Camera mainCamera;
    public float gravity = -9.81f;
    public float moveSpeed = 2;
    public float stickDeadzone = 0.3f;
    Vector3 fProj;
    Vector3 rProj;
    float velocityY;
    bool rotateFlicked;
    bool aPressed;
    public bool moveRelativeToFacing;
    public float rotationAngle = 30;
    float currentSelectedRotation;
    public XRProcessor XRProcessor;

    void Start() {
        velocityY = 0;
        rotateFlicked = false;
        currentSelectedRotation = 0;
        aPressed = false;
    }

    void Update()
    {
        // Calculate forward and right vectors
        if(moveRelativeToFacing) {
            fProj = Vector3.Normalize(Vector3.Scale(mainCamera.transform.forward, new Vector3(1,0,1)));
        }
        else {
            fProj = Quaternion.Euler(0, currentSelectedRotation, 0) * new Vector3(0,0,1);
        }
        rProj = Vector3.Normalize(Vector3.Cross(new Vector3(0,1,0),fProj));

        Debug.DrawRay(mainCamera.transform.position, fProj * 20,Color.red);
        Debug.DrawRay(mainCamera.transform.position, rProj * 20,Color.blue);
        ProcessSwitchMovement();
        ProcessRotate();
        ProcessMove();
    }
    void ProcessSwitchMovement() {
        if(!aPressed && inputProcessor.rightHandButtonA) {
            aPressed = true;
            moveRelativeToFacing = !moveRelativeToFacing;
        }
        if(aPressed && !inputProcessor.rightHandButtonA) {
            aPressed = false;
        }
    }
    void ProcessRotate() {
        // Preprocess input data
        Vector2 rawStickData = inputProcessor.rightHandJoystick;
        // Create stick deadzone
        if(Mathf.Abs(rawStickData.x) <= stickDeadzone) {
            rawStickData.x = 0;
        }
        if(!rotateFlicked && rawStickData.x != 0) {
            int direction = (int)Mathf.Sign(rawStickData.x); //-1 = left. 1 = right.
            mainCamera.transform.parent.RotateAround(mainCamera.transform.position,Vector3.up,rotationAngle * direction);
            currentSelectedRotation += rotationAngle * direction;
            rotateFlicked = true;
        }
        if(rawStickData.magnitude < stickDeadzone) {
            rotateFlicked = false;
        }
        
    }

    void ProcessMove() {
        // Preprocess input data
        Vector2 rawStickData = inputProcessor.leftHandJoystick;
        // Create stick dead zone
        if(Mathf.Abs(rawStickData.x) <= stickDeadzone) {
            rawStickData.x = 0;
        }
        if(Mathf.Abs(rawStickData.y) <= stickDeadzone) {
            rawStickData.y = 0;
        }
        Vector3 moveData = moveSpeed * Vector3.Normalize(fProj * rawStickData.y + rProj * rawStickData.x);
        velocityY += gravity;
        if(controller.isGrounded) {
            velocityY = 0;
        }
        moveData.y = velocityY;
        controller.Move(moveData * Time.deltaTime);
    }
    public void SendHaptics(bool leftHand, float amplitude, float duration)
    {
        UnityEngine.XR.HapticCapabilities capabilities;
        foreach (var device in XRProcessor.devices)
        {
            if (device.Key == UnityEngine.XR.XRNode.LeftHand && device.Value.Count != 0 && leftHand)
            {
                // Send haptics to left hand
                if (device.Value[0].TryGetHapticCapabilities(out capabilities))
                {
                    if (capabilities.supportsImpulse)
                    {
                        device.Value[0].SendHapticImpulse(0, amplitude, duration);
                    }
                }
                break;
            }
            else if (device.Key == UnityEngine.XR.XRNode.RightHand && device.Value.Count != 0 && !leftHand)
            {
                // Send haptics to right hand
                if (device.Value[0].TryGetHapticCapabilities(out capabilities))
                {
                    if (capabilities.supportsImpulse)
                    {
                        device.Value[0].SendHapticImpulse(0, amplitude, duration);
                    }
                }
                break;
            }
        }
    }
}
