using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class VRCommand : MonoBehaviour
{
    // Runs when input [XRNode] returns a value of []
    public abstract void RunCommand(TestResult tr,
                                    Bone val_bone = new Bone(),
                                    bool val_bool = false,
                                    Eyes val_eyes = new Eyes(),
                                    float val_float = 0.0f,
                                    Hand val_hand = new Hand(),
                                    InputTrackingState val_input_tracking_state = InputTrackingState.None,
                                    Quaternion val_quaternion = new Quaternion(),
                                    Vector2 val_vector2 = new Vector2(),
                                    Vector3 val_vector3 = new Vector3());
}
