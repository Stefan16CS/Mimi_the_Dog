using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [Header("References")]
    private float length, startpos, yAxisPos;
    public new GameObject camera;
    public float scrollMultiply;
    void Start()
    {
        startpos = transform.position.x; 
        yAxisPos = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float dist = (camera.transform.position.x * scrollMultiply);
        float temp = (camera.transform.position.x * (1 - scrollMultiply));
        float ydist = (camera.transform.position.y * scrollMultiply);

        transform.position = new Vector3(startpos + dist, yAxisPos + ydist, transform.position.z);
        if (temp > startpos + length) startpos += length;
    }
}
