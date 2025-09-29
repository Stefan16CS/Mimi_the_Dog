using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject mimi;
    private Vector3 offset;
    private Vector3 cameraPosition;

    void Start()
    {
        offset = transform.position - mimi.transform.position;
        cameraPosition = transform.position;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(
        mimi.transform.position.x + offset.x, cameraPosition.y,
        cameraPosition.z);
    }
}
