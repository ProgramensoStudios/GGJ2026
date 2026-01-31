using UnityEngine;

public class ParallaxSystem : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxPower;
    [SerializeField] private bool isBackgroundStatic;
    [SerializeField] private Transform playerpos;

    private Transform cameraTransform;
    private Vector3 lastCamPos;

    void Start()
    {
        cameraTransform = Camera.main.transform;   
        lastCamPos = cameraTransform.position;
    }

    private void LateUpdate()
    {
        if (isBackgroundStatic)
        {
            transform.position = new Vector3(playerpos.position.x, playerpos.position.y, transform.position.z);
            return;
        }
        Vector3 deltaMovement = cameraTransform.position - lastCamPos;
        transform.position += new Vector3(deltaMovement.x * parallaxPower.x, deltaMovement.y * parallaxPower.y);
        lastCamPos = cameraTransform.position;
    }
}
