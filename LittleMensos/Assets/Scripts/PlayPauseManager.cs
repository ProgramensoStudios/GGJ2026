using UnityEngine;

public class PlayPauseManager : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;
    public void Pause()
    {
        Time.timeScale = 0f;
        pauseCanvas.enabled = true;
    }

    public void Play()
    {
        pauseCanvas.enabled = false;
        Time.timeScale = 1f;
    }
}
