using UnityEngine;

public class SwordScaler : MonoBehaviour
{
    public KeyCode scaleUpKey = KeyCode.UpArrow; // The key to press for growing the sword
    public KeyCode scaleDownKey = KeyCode.DownArrow; // The key to press for growing the sword
    public float growAmount = 0.05f; // Amount by which the sword grows on the Y axis
    public float maxScale = 4f; // Maximum scale limit on the Y axis
    public float minScale = 0.5f; // Maximum scale limit on the Y axis
    public float scale_fix = 0.0045f;


    void Update()
    {
        /**
        // Check if the grow key is pressed
        if (Input.Get(scaleUpKey))
        {
            ScaleSword(true);
        }
        // Check if the grow key is pressed
        if (Input.Get(scaleDownKey))
        {
            ScaleSword(false);
        }
        */
        // Optionally handle controller input (example for Oculus Touch controller)
        if (OVRInput.Get(OVRInput.Button.One)) // Adjust based on your controller setup
        {
            ScaleSword(false);
        }
        if (OVRInput.Get(OVRInput.Button.Two)) // Adjust based on your controller setup
        {
            ScaleSword(true);
        }
        // Optionally handle controller input (example for Oculus Touch controller)
        if (OVRInput.Get(OVRInput.RawButton.X)) // Adjust based on your controller setup
        {
            ScaleSword(false);
        }
        if (OVRInput.Get(OVRInput.RawButton.Y)) // Adjust based on your controller setup
        {
            ScaleSword(true);
        }

    }

    void ScaleSword(bool is_up)
    {
        // Get current scale and posiions
        Vector3 currentScale = transform.localScale;
        Vector3 originalPosition = transform.localPosition;
        // positive or negative corresponding to is_up
        // float scaleAmount = growAmount;
        // if (!is_up) {
        //     scaleAmount *= -1;
        // }

        float scaleAmount = is_up ? growAmount : -growAmount;
        // Increase the Y axis scale
        
        float newScaleY = Mathf.Clamp(currentScale.y + scaleAmount, minScale, maxScale);
        Vector3 newScale = new Vector3(currentScale.x, newScaleY, currentScale.z);
        transform.localScale = newScale;
        
        // change of 21 in scale --> change of 0.1 in position


        // Update position in Y axis
        Vector3 newPosition = transform.localPosition;
        newPosition.y = originalPosition.y + (newScale.y - currentScale.y) * scale_fix;
        transform.localPosition = newPosition;
    }
}
