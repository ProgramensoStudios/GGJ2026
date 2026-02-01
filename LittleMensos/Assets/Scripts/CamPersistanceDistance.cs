using UnityEngine;

public class CamPersistanceDistance : MonoBehaviour
{
    [SerializeField] private Transform cam;

    private float zOffset;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        // distancia inicial en Z
        zOffset = transform.position.z - cam.position.z;
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.z = cam.position.z + zOffset;
        transform.position = pos;
    }
}
