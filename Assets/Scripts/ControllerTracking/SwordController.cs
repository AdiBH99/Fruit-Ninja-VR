using UnityEngine;

public class SwordController : MonoBehaviour
{
    public GameObject rightController; // Reference to the right controller object
    public GameObject leftController; // Reference to the right controller object
    public GameObject swordPrefab; // Reference to the sword prefab
    public Vector3 positionOffsetRight = new Vector3(0f, 0.65f, 0.53f); // Position offset for fine-tuning
    public Quaternion rotationOffsetRight = Quaternion.Euler(0.5f, -0.05f, 0f); // Rotation offset for fine-tuning
    public Vector3 positionOffsetLeft = new Vector3(0f, 0.65f, 0.53f); // Position offset for fine-tuning
    public Quaternion rotationOffsetLeft = Quaternion.Euler(0.5f, -0.05f, 0f); // Rotation offset for fine-tuning
    public GameObject rightSwordInstance;
    public GameObject leftSwordInstance;
    public bool is_right = true;
    

    void Start()
    {
        // Instantiate the sword and set it as a child of the right controller
        rightSwordInstance = Instantiate(swordPrefab, rightController.transform);
        leftSwordInstance = Instantiate(swordPrefab, rightController.transform);
        // swordInstance.transform.SetParent(rightController.transform, false);
        // swordInstance.transform.localPosition = new Vector3(2f, 0, 0);
        // swordInstance.transform.localRotation = Quaternion.Euler(90, 90, 90);
        rightSwordInstance.transform.localPosition = positionOffsetRight;
        rightSwordInstance.transform.localRotation = rotationOffsetRight;
        leftSwordInstance.transform.localPosition = positionOffsetLeft;
        leftSwordInstance.transform.localRotation = rotationOffsetLeft;
        UpdateHand();

    }

    void Update()
    {
        // Update the position and rotation of the sword to match the right controller
        // Update the position and rotation of the sword to match the right controller
        rightSwordInstance.transform.position = rightController.transform.position + rightController.transform.TransformVector(positionOffsetRight);
        rightSwordInstance.transform.rotation = rightController.transform.rotation * rotationOffsetRight;
        leftSwordInstance.transform.position = leftController.transform.position + leftController.transform.TransformVector(positionOffsetLeft);
        leftSwordInstance.transform.rotation = leftController.transform.rotation * rotationOffsetLeft;

    }

    void UpdateHand()
    {
        rightSwordInstance.SetActive(is_right);
        leftSwordInstance.SetActive(!is_right);
    }

    public void setHand(bool _is_right)
    {
        is_right = _is_right;
        UpdateHand();
    }

}
