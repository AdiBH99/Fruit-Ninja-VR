using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class QuitGame : MonoBehaviour
{
    // This method will be called when the Quit button is pressed
    public void Quit()
    {
        // If running in the Unity Editor, this will stop the play mode
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a standalone build, this will quit the application
        UnityEngine.Application.Quit();
#endif
    }
}
