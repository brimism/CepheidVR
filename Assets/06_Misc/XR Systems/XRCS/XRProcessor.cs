using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRProcessor : MonoBehaviour
{
    // List of commands that will be run by the processor
    public List<Command> allCommands = new List<Command>();

    public Dictionary<XRNode, List<InputDevice>> devices = new Dictionary<XRNode, List<InputDevice>>();

    private void Start()
    {
        for(int i = 0; i < 9; i++)
        {
            devices.Add((XRNode)i, new List<InputDevice>());
        }


    }

    Bone out_bone;
    bool out_bool;
    Eyes out_eyes;
    float out_float;
    Hand out_hand;
    InputTrackingState out_input_tracking_state;
    Quaternion out_quaternion;
    Vector2 out_vector2;
    Vector3 out_vector3;

    TestResult result;


    // Update is called once per frame
    void Update()
    {
        if (allCommands.Count > 0)
        {
            foreach (Command c in allCommands)
            {
                InputDevices.GetDevicesAtXRNode(c.node, devices[c.node]);
                if(devices[c.node].Count == 1)
                {
                    // Depending on the feature we are testing, we will pass the corresponding CommonUsages member in a TryGetFeatureValue() along with the corresponding out_variable
                    // If a test is necessary, we will call RunTest with the appropriate values.
                    switch(c.feature)
                    {
                        case Feature.BATTERY_LEVEL_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.batteryLevel, out out_float))
                            {
                                if(c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.CENTER_EYE_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.centerEyeAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.CENTER_EYE_ANGULAR_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.centerEyeAngularAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.CENTER_EYE_ANGULAR_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.centerEyeAngularVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.CENTER_EYE_POSITION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.centerEyePosition, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.CENTER_EYE_ROTATION_QUATERNION:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.centerEyeRotation, out out_quaternion))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_quaternion, c.test, c.quaternion_value);
                                    c.commandToRun.RunCommand(result, val_quaternion: out_quaternion);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_quaternion: out_quaternion);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.CENTER_EYE_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.centerEyeVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.COLOR_CAMERA_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.colorCameraAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.COLOR_CAMERA_ANGULAR_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.colorCameraAngularAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.COLOR_CAMERA_ANGULAR_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.colorCameraAngularVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.COLOR_CAMERA_POSITION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.colorCameraPosition, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.COLOR_CAMERA_ROTATION_QUATERNION:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.colorCameraRotation, out out_quaternion))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_quaternion, c.test, c.quaternion_value);
                                    c.commandToRun.RunCommand(result, val_quaternion: out_quaternion);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_quaternion: out_quaternion);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.COLOR_CAMERA_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.colorCameraVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.DEVICE_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.deviceAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.DEVICE_ANGULAR_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.deviceAngularAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.DEVICE_ANGULAR_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.DEVICE_POSITION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.devicePosition, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.DEVICE_ROTATION_QUATERNION:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.deviceRotation, out out_quaternion))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_quaternion, c.test, c.quaternion_value);
                                    c.commandToRun.RunCommand(result, val_quaternion: out_quaternion);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_quaternion: out_quaternion);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.DEVICE_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.deviceVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.D_PAD_VECTOR2:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.dPad, out out_vector2))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector2, c.test, c.vector2_value);
                                    c.commandToRun.RunCommand(result, val_vector2: out_vector2);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector2: out_vector2);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.EYES_DATA_EYES:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.eyesData, out out_eyes)) 
                            {
                                    c.commandToRun.RunCommand(TestResult.NA, val_eyes: out_eyes);
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.GRIP_BUTTON_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.gripButton, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.GRIP_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.grip, out out_float))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.HAND_DATA_HAND:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.handData, out out_hand))
                            {
                                c.commandToRun.RunCommand(TestResult.NA, val_hand: out_hand);
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.INDEX_FINGER_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.indexFinger, out out_float))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.INDEX_TOUCH_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.indexTouch, out out_float))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.IS_TRACKED_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.isTracked, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.LEFT_EYE_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.leftEyeAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.LEFT_EYE_ANGULAR_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.leftEyeAngularAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.LEFT_EYE_ANGULAR_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.leftEyeAngularVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.LEFT_EYE_POSITION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.leftEyePosition, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.LEFT_EYE_ROTATION_QUATERNION:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.leftEyeRotation, out out_quaternion))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_quaternion, c.test, c.quaternion_value);
                                    c.commandToRun.RunCommand(result, val_quaternion: out_quaternion);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_quaternion: out_quaternion);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.LEFT_EYE_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.leftEyeVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.MENU_BUTTON_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.menuButton, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.MIDDLE_FINGER_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.middleFinger, out out_float))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.PINKY_FINGER_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.pinkyFinger, out out_float))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.PRIMARY_2D_AXIS_CLICK_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.primary2DAxisClick, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.PRIMARY_2D_AXIS_TOUCH_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.PRIMARY_2D_AXIS_VECTOR2:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.primary2DAxis, out out_vector2))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector2, c.test, c.vector2_value);
                                    c.commandToRun.RunCommand(result, val_vector2: out_vector2);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector2: out_vector2);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.PRIMARY_BUTTON_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.primaryButton, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.PRIMARY_TOUCH_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.primaryTouch, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.RIGHT_EYE_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.rightEyeAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.RIGHT_EYE_ANGULAR_ACCELERATION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.rightEyeAngularAcceleration, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.RIGHT_EYE_ANGULAR_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.rightEyeAngularVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.RIGHT_EYE_POSITION_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.rightEyePosition, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.RIGHT_EYE_ROTATION_QUATERNION:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.rightEyeRotation, out out_quaternion))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_quaternion, c.test, c.quaternion_value);
                                    c.commandToRun.RunCommand(result, val_quaternion: out_quaternion);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_quaternion: out_quaternion);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.RIGHT_EYE_VELOCITY_VECTOR3:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.rightEyeVelocity, out out_vector3))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector3, c.test, c.vector3_value);
                                    c.commandToRun.RunCommand(result, val_vector3: out_vector3);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector3: out_vector3);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.RING_FINGER_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.ringFinger, out out_float))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.SECONDARY_2D_AXIS_VECTOR2:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.secondary2DAxis, out out_vector2))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_vector2, c.test, c.vector2_value);
                                    c.commandToRun.RunCommand(result, val_vector2: out_vector2);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_vector2: out_vector2);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.SECONDARY_BUTTON_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.secondaryButton, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.SECONDARY_TOUCH_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.secondaryTouch, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.THUMBREST_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.thumbrest, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.THUMBTOUCH_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.thumbTouch, out out_float))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.TRACKING_STATE_INPUTTRACKINGSTATE:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.trackingState, out out_input_tracking_state))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_input_tracking_state, c.test, c.inputTrackingState_value);
                                    c.commandToRun.RunCommand(result, val_input_tracking_state: out_input_tracking_state);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.TRIGGER_BUTTON_BOOL:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.triggerButton, out out_bool))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_bool, c.test, c.bool_value);
                                    c.commandToRun.RunCommand(result, val_bool: out_bool);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_bool: out_bool);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                        case Feature.TRIGGER_FLOAT:
                            if (devices[c.node][0].TryGetFeatureValue(CommonUsages.trigger, out out_float))
                            {
                                if (c.test != Test.SEND_VALUE)
                                {
                                    result = RunTest(out_float, c.test, c.float_value);
                                    c.commandToRun.RunCommand(result, val_float: out_float);
                                }
                                else
                                {
                                    c.commandToRun.RunCommand(TestResult.NA, val_float: out_float);
                                }
                            }
                            else
                            {
                                ErrorMessage(devices[c.node][0].name + " does not have feature " + c.feature.ToString());
                            }
                            break;
                    }
                }
                else
                {
                    ErrorMessage("Invalid number of devices of XRNode type " + c.node.ToString() + ": " + devices[c.node].Count + " devices detected.");
                }
            }
        }
    }

    
    // Runs a test between val and testVal. If the test is inappropriate for the data type, an error message is returned along with result NA.
    // Test format: val (test) testVal
    TestResult RunTest(bool val, Test test, bool testVal)
    {
        switch(test)
        {
            case Test.EQUAL:
                if(val == testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.NOT_EQUAL:
                if (val != testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            default:
                ErrorMessage("Test " + test.ToString() + " does not exist for the type bool");
                return TestResult.NA;
        }
    }
    TestResult RunTest(float val, Test test, float testVal)
    {
        switch(test)
        {
            case Test.EQUAL:
                if (val == testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.NOT_EQUAL:
                if (val != testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.LESS_THAN:
                if (val < testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.LESS_THAN_OR_EQUAL:
                if (val <= testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.GREATER_THAN_OR_EQUAL:
                if (val >= testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.GREATER_THAN:
                if (val > testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            default:
                ErrorMessage("Test " + test.ToString() + " does not exist for the type float");
                return TestResult.NA;
        }
    }
    TestResult RunTest(InputTrackingState val, Test test, InputTrackingState testVal)
    {
        switch (test)
        {
            case Test.EQUAL:
                if (val == testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.NOT_EQUAL:
                if (val != testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            default:
                ErrorMessage("Test " + test.ToString() + " does not exist for the type Input Tracking State");
                return TestResult.NA;
        }
    }
    TestResult RunTest(Quaternion val, Test test, Quaternion testVal)
    {
        switch (test)
        {
            case Test.EQUAL:
                if (val == testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.NOT_EQUAL:
                if (val != testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            default:
                ErrorMessage("Test " + test.ToString() + " does not exist for the type Quaternion");
                return TestResult.NA;
        }
    }
    TestResult RunTest(Vector2 val, Test test, Vector2 testVal)
    {
        switch (test)
        {
            case Test.EQUAL:
                if (val == testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.NOT_EQUAL:
                if (val != testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            default:
                ErrorMessage("Test " + test.ToString() + " does not exist for the type Vector2");
                return TestResult.NA;
        }
    }
    TestResult RunTest(Vector3 val, Test test, Vector3 testVal)
    {
        switch (test)
        {
            case Test.EQUAL:
                if (val == testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            case Test.NOT_EQUAL:
                if (val != testVal)
                {
                    return TestResult.TRUE;
                }
                else
                {
                    return TestResult.FALSE;
                }
            default:
                ErrorMessage("Test " + test.ToString() + " does not exist for the type Vector3");
                return TestResult.NA;
        }
    }

    void ErrorMessage(string e)
    {
        Debug.Log(e);
    }

}
