using UnityEngine;
using UnityEngine.SceneManagement;
public class RestartGame : MonoBehaviour
{
    public void ReStartGame() 
    {
        SceneManager.LoadScene("MimiTheDog", LoadSceneMode.Single);
        Time.timeScale = 1.0f;
    }
}
