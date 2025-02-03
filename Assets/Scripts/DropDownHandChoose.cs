using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownHandChoose : MonoBehaviour
{
    public TMPro.TMP_Dropdown selectHand;
    public SwordController swordController;
    // Start is called before the first frame update
    void Start()
    {
        selectHand.onValueChanged.AddListener(HandleDropdownValueChanged);
    }

    void HandleDropdownValueChanged(int value)
    {
        if (value == 0)
        {
            swordController.setHand(true); // Call Right Hand function
        }
        else if (value == 1)
        {
            swordController.setHand(false); // Call Left Hand function
        }
    }
}
