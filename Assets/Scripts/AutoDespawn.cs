using UnityEngine;

public class AutoDespawn : MonoBehaviour
{
    public float lifetime = 10f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
