using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandRot_CMD : VRCommand
{
    public VRInputProcessor inputProcessor;
    public override void RunCommand(TestResult tr, UnityEngine.XR.Bone val_bone = default(UnityEngine.XR.Bone), bool val_bool = false, UnityEngine.XR.Eyes val_eyes = default(UnityEngine.XR.Eyes), float val_float = 0, UnityEngine.XR.Hand val_hand = default(UnityEngine.XR.Hand), UnityEngine.XR.InputTrackingState val_input_tracking_state = UnityEngine.XR.InputTrackingState.None, Quaternion val_quaternion = default(Quaternion), Vector2 val_vector2 = default(Vector2), Vector3 val_vector3 = default(Vector3)){
        inputProcessor.rightHandRot = val_quaternion;
    }
}
