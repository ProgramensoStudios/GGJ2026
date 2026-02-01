using UnityEngine;

public class PatrolFoll : MonoBehaviour, IPatrolBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float distance = 3f;
    [SerializeField] float waitTime = 1.5f;
    [SerializeField] float returnSpeed = 3f;
    [SerializeField] float maxDistanceFromOrigin = 5f;

    Animator anim;
    SpriteRenderer sprite;

    Vector3 origin;
    Vector3 target;
    bool goingToTarget = true;

    float waitTimer;
    bool waiting;

    void Start()
    {
        origin = transform.position;
        target = origin + transform.right * distance;

        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Execute()
    {
        if (Vector3.Distance(transform.position, origin) > maxDistanceFromOrigin)
        {
            anim.SetBool("IsWalking", true);
            ReturnToOrigin();
            return;
        }

        if (waiting)
        {
            anim.SetBool("IsWalking", false);

            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waiting = false;
                goingToTarget = !goingToTarget;
                target = goingToTarget ? origin + transform.right * distance : origin;
            }
            return;
        }

        anim.SetBool("IsWalking", true);
        Move();
    }

    void Move()
    {
        Vector3 prevPos = transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        float dirX = target.x - prevPos.x;
        UpdateFacing(dirX);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            waiting = true;
            waitTimer = 0f;
        }
    }

    void ReturnToOrigin()
    {
        Vector3 prevPos = transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            origin,
            returnSpeed * Time.deltaTime
        );

        float dirX = origin.x - prevPos.x;
        UpdateFacing(dirX);
    }

    void UpdateFacing(float dirX)
    {
        if (dirX > 0.01f)
            sprite.flipX = false;
        else if (dirX < -0.01f)
            sprite.flipX = true;
    }

    private void OnBecameVisible()
    {
        SFXManager.Instance.Play("FollowEnemy", transform.position);
    }
}
