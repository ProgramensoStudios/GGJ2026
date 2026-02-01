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
            MaskManager.Instance.activeMask = maskType;
            MaskManager.Instance.UpdateMaskVisuals();
        }
    }
}
