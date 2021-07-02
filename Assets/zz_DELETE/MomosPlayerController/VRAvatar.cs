using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAvatar : MonoBehaviour
{
    public VRInputProcessor inputs;

    public Transform head;
    public Transform rightHand;
    public Transform leftHand;

    void Update()
    {
        head.localPosition = inputs.headPos;
        head.localRotation = inputs.headRot;
        rightHand.localPosition = inputs.rightHandPos;
        rightHand.localRotation = inputs.rightHandRot;
        leftHand.localPosition = inputs.leftHandPos;
        leftHand.localRotation = inputs.leftHandRot;
    }
}
