using UnityEngine;

public class BackgroundMusic8Bit : MonoBehaviour
{
    public AudioSource musicSource;
    void Start()
    {
        musicSource.loop = true;
        musicSource.volume = 0.2f;
        musicSource.Play();
    }
    public void StopMusic() 
    {
        musicSource.Stop();
    }

}
    
   
