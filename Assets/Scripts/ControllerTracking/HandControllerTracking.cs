using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandControllerTracking : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    private InputDevice targetDevice;

    void Start()
    {
        InitializeController();
    }

    void Update()
    {
        if (!targetDevice.isValid)
        {
            InitializeController();
        }
        else
        {
            Vector3 position;
            Quaternion rotation;

            if (targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out position) &&
                targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
            {
                transform.localPosition = position;
                transform.localRotation = rotation;

                Debug.Log($"Controller Position: {position}");
                Debug.Log($"Controller Rotation: {rotation}");
            }
        }
    }

    void InitializeController()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }
}
