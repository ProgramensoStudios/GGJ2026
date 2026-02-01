using UnityEngine;

public class RolaDangerPlay : MonoBehaviour
{

    private void OnBecameVisible()
    {
        MaskManager.Instance.isInDanger = true;
    }

    private void OnBecameInvisible()
    {
        MaskManager.Instance.isInDanger = false;
    }
}
