using UnityEngine;

public class AttackFly : MonoBehaviour, IAttackBehaviour
{
    [Header("Speeds")]
    [SerializeField] float horizontalSpeed = 4f;
    [SerializeField] float gravity = 12f;

    [Header("Limits")]
    [SerializeField] float minY = -2f;
    [SerializeField] float exitHeight = 3f;

    Vector3 horizontalDir;
    float verticalSpeed;
    bool exiting;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnEnter(EnemyBrain brain)
    {
        Vector3 dir = (brain.player.position - transform.position).normalized;
        horizontalDir = new Vector3(dir.x, 0f, dir.z).normalized;

        verticalSpeed = 0f;
        exiting = false;
    }

    public void Execute()
    {

        anim.SetBool("IsAttack", true);
        anim.SetBool("IsFollow", false);
        if (!exiting)
            verticalSpeed -= gravity * Time.deltaTime;
        else
            verticalSpeed = Mathf.Lerp(verticalSpeed, 4f, Time.deltaTime * 2f);

        Vector3 move =
            horizontalDir * horizontalSpeed +
            Vector3.up * verticalSpeed;

        transform.position += move * Time.deltaTime;

        if (!exiting && transform.position.y <= minY)
            exiting = true;
    }

    public bool IsFinished()
    {
        return exiting && transform.position.y >= exitHeight;
    }
}