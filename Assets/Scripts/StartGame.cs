using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject startText;
    private void Awake()
    {
        Time.timeScale = 0f;
        Mimi.gamePaused = true;
    }

    public void OnClick()
    {
        Time.timeScale = 1f;
        startButton.SetActive(false);
        startText.SetActive(false);
        Mimi.gamePaused = false;
    }
}
