using UnityEngine;

public class CameraObstructionDetector : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LayerMask obstructionMask;

    private FadeObstacles _currentObstacle;

    // === DEBUG ===
    private bool _isHitting;
    private Vector3 _hitPoint;

    void LateUpdate()
    {
        Vector3 dir = player.position - transform.position;
        float distance = dir.magnitude;

        if (Physics.Raycast(transform.position, dir.normalized, out RaycastHit hit, distance, obstructionMask))
        {
            _isHitting = true;
            _hitPoint = hit.point;

            FadeObstacles fade = hit.collider.GetComponent<FadeObstacles>();

            if (fade != null && fade != _currentObstacle)
            {
                if (_currentObstacle != null)
                    _currentObstacle.FadeIn();

                fade.FadeOut();
                _currentObstacle = fade;
            }
        }
        else
        {
            _isHitting = false;

            if (_currentObstacle != null)
            {
                _currentObstacle.FadeIn();
                _currentObstacle = null;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = _isHitting ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, player.position);

        if (_isHitting)
        {
            Gizmos.DrawSphere(_hitPoint, 0.1f);
        }
    }
}
