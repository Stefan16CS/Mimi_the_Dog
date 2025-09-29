using UnityEngine;

public class BoneLifeTime : MonoBehaviour
{
    public float lifetime = 10f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
