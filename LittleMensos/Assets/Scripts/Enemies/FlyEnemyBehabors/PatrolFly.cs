using UnityEngine;

public class PatrolFly : MonoBehaviour, IPatrolBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float patrolRadius = 2f;
    [SerializeField] float returnSpeed = 3f;

    private Animator anim;

    Vector3 originalPos;

    void Start()
    {
        originalPos = transform.position;
        anim = GetComponent<Animator>();
    }

    public void Execute()
    {
        float distanceToOrigin = Vector3.Distance(transform.position, originalPos);
        anim.SetBool("IsFollow", false);
        anim.SetBool("IsAttack", false);

        if (distanceToOrigin > patrolRadius)
        {
            ReturnToOrigin();
        }
        else
        {
            PatrolCurve();
        }
    }

    void PatrolCurve()
    {
        float vertical = Mathf.Sin(Time.time * -2f);
        float horizontal = Mathf.Sin(Time.time);

        Vector3 move =
            transform.up * vertical +
            transform.right * horizontal;

        transform.position += move * speed * Time.deltaTime;
    }

    void ReturnToOrigin()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            originalPos,
            returnSpeed * Time.deltaTime
        );
    }

    private void OnBecameVisible()
    {
        SFXManager.Instance.Play("FlyEnemy", transform.position );
    }
}
