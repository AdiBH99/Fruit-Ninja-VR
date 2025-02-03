using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSoundToggle : MonoBehaviour
{
    public Toggle soundToggle;  // Assign via Inspector for sound control
    public SoundController soundController;
    // Start is called before the first frame update
    void Start()
    {
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        // Call the method in SoundManager to handle sound control
        soundController.is_sound_on = isOn;
    }
}
