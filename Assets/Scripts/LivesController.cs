using UnityEngine;
using TMPro;

public class LivesController: MonoBehaviour
{
    public TextMeshProUGUI lives_text;
    // public TextMeshProUGUI live2_icon;
    // public TextMeshProUGUI live3_icon;
    public int lives = 3;
    public bool isLiveMode = false;
    
    public void Start()
    {
        UpdateLiveIcons();
    }

    public void Update()
    {
        UpdateLiveIcons();
    }

    void UpdateLiveIcons()
    {
        if (isLiveMode)
        {
            lives_text.text = $"Lives: {lives}";
        }
        else
        {
            lives_text.text = "";
        }
    }

    public void ReduceLife()
    {
        if (isLiveMode && lives > 0)
        {
            lives -= 1;
        }
        
    }
}
