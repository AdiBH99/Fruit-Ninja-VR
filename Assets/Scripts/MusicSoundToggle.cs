using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSoundToggle : MonoBehaviour
{
    public Toggle soundToggle;  // Assign via Inspector for sound control
    // Start is called before the first frame update
    void Start()
    {
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
    }

    // Update is called once per frame
    private void OnSoundToggleChanged(bool isOn)
    {
        // Call the method in SoundManager to handle sound control
        AudioListener.pause = !isOn;
    }
}
    