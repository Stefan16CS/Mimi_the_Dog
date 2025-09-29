using UnityEditor;
using UnityEngine;

public class BoneSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject bonePrefab;
    public float spawnInterval = 2f;

    private float timer;
           
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBone();
            timer = 0f;
        }
    }

    void SpawnBone()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();
        Instantiate(bonePrefab, spawnPosition, Quaternion.identity);
    }

    Vector2 GetRandomSpawnPosition()
    {
        Camera cam = Camera.main;
        float minY = -2f;
        float maxY = -4f;

        float x = GetXInFrontOfPlayer();
        float y = Random.Range(minY, maxY);

        return new Vector2(x, y);
    }

    float GetXInFrontOfPlayer()
    {
        GameObject mimi = GameObject.FindWithTag("Mimi");
        float offset = 25f;
        return mimi.transform.position.x + offset;
    }
}

