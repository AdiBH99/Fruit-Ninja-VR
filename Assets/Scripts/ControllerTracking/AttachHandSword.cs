using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AttachHandSword : MonoBehaviour
{
    public InputDeviceCharacteristics leftControllerCharacteristics;
    public InputDeviceCharacteristics rightControllerCharacteristics;
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;
    public GameObject swordPrefab;

    private InputDevice leftController;
    private InputDevice rightController;
    private GameObject leftHandInstance;
    private GameObject rightHandInstance;
    private GameObject swordInstance;

    void Start()
    {
        InitializeControllers();
    }

    void InitializeControllers()
    {
        List<InputDevice> devices = new List<InputDevice>();
        
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            leftController = devices[0];
            leftHandInstance = Instantiate(leftHandPrefab, transform);
        }

        devices.Clear();
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            rightController = devices[0];
            rightHandInstance = Instantiate(rightHandPrefab, transform);
            swordInstance = Instantiate(swordPrefab, rightHandInstance.transform);
        }
    }

    void Update()
    {
        if (leftController.isValid)
        {
            Vector3 leftPosition;
            Quaternion leftRotation;
            if (leftController.TryGetFeatureValue(CommonUsages.devicePosition, out leftPosition) &&
                leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out leftRotation))
            {
                leftHandInstance.transform.localPosition = leftPosition;
                leftHandInstance.transform.localRotation = leftRotation;
            }
        }
        else
        {
            InitializeControllers();
        }

        if (rightController.isValid)
        {
            Vector3 rightPosition;
            Quaternion rightRotation;
            if (rightController.TryGetFeatureValue(CommonUsages.devicePosition, out rightPosition) &&
                rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out rightRotation))
            {
                rightHandInstance.transform.localPosition = rightPosition;
                rightHandInstance.transform.localRotation = rightRotation;
            }
        }
        else
        {
            InitializeControllers();
        }
    }
}
