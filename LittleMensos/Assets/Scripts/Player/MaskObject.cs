using UnityEngine;

public class MaskObject : MonoBehaviour
{
    public MaskType maskType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MaskManager.Instance.UnlockMask(maskType);
            Destroy(gameObject);
        }
    }
}
