using UnityEngine;

public class Goblin : MonoBehaviour
{
    
    [Header("Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int damage = 2;
    [Range(0, 3)] [SerializeField] private float recoverySeconds;
    
    [Header("Line of Sight")]
    [SerializeField] private Transform sightPoint;
    [SerializeField] private float sightRange;
    [SerializeField] private Transform rearSightPoint;
    [SerializeField] private float rearSightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private bool isFacingRight = true;
    
    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackInterval;
    
    [Header("Debug")]
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isPlayerSighted;
    [SerializeField] private bool isPlayerInAttackRange;

    private Rigidbody2D rig;
    private Animator anim;
    
    private float iFramesCountdown;
    private float attackCountdown;
    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        attackCountdown = attackInterval;
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
        MonitorLineOfSight();
        OnMove();

        if (iFramesCountdown > 0)
        {
            iFramesCountdown -= Time.deltaTime;
        }
    }

    private void OnMove()
    {
        transform.eulerAngles = new Vector2(0, isFacingRight ? 0 : 180f);

        if (isPlayerSighted)
        {
            if (isPlayerInAttackRange)
            {
                rig.velocity = Vector2.zero;
                AttackPlayer();
            }
            else
            {
                var currentSpeed = isFacingRight ? moveSpeed : -moveSpeed;
                rig.velocity = new Vector2(currentSpeed, rig.velocity.y);
                anim.SetInteger(GoblinAnimations.AnimTransition, (int)GoblinAnimations.States.Walk);
            }
        }
        else
        {
            rig.velocity = Vector2.zero;
            anim.SetInteger(GoblinAnimations.AnimTransition, (int)GoblinAnimations.States.Idle);
        }
    }

    private void AttackPlayer()
    {
        if (iFramesCountdown > 0) return;
        if (attackCountdown > 0)
        {
            attackCountdown -= Time.deltaTime;
            return;
        }

        attackCountdown = attackInterval;
        
        anim.SetInteger(GoblinAnimations.AnimTransition, (int)GoblinAnimations.States.Attack);
        
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius);
        if (hit && hit.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hit.GetComponent<Player>().TakeDamage(damage);
        }
        
        // TODO Damage on player logic here, with a inv period? But the animation didn't even happen here...
    }

    private void MonitorLineOfSight()
    {
        Vector2 frontRayDirection = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D frontHit = Physics2D.Raycast(sightPoint.position, frontRayDirection, sightRange);
        if (frontHit.collider && frontHit.collider.CompareTag("Player"))
        {
            isPlayerSighted = true;
            float playerDistance = Vector2.Distance(transform.position, frontHit.transform.position);
            isPlayerInAttackRange = playerDistance <= attackRange;
        }
        else
        {
            Vector2 rearRayDirection = isFacingRight ? Vector2.left : Vector2.right;
            RaycastHit2D rearHit = Physics2D.Raycast(rearSightPoint.position, rearRayDirection, rearSightRange);
            if (rearHit.collider && rearHit.collider.CompareTag("Player"))
            {
                isFacingRight = !isFacingRight;
                isPlayerSighted = true;
                float playerDistance = Vector2.Distance(transform.position, rearHit.transform.position);
                isPlayerInAttackRange = playerDistance <= attackRange;
            }
            else
            {
                isPlayerSighted = false;
            }
        }
    }

    public void TakeDamage(int damageTaken)
    {
        if (iFramesCountdown > 0) return;

        currentHealth -= damageTaken;
        if (currentHealth <= 0)
        {
            isDead = true;
            moveSpeed = 0;
            anim.SetTrigger(GoblinAnimations.TriggerDiedAnim);
            Destroy(gameObject, 1f);
        }
        else
        {
            anim.SetTrigger(GoblinAnimations.TriggerHitAnim);
            iFramesCountdown = recoverySeconds;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 frontRayDirection = isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawRay(sightPoint.position, frontRayDirection * sightRange);
        
        Vector2 rearRayDirection = isFacingRight ? Vector2.left : Vector2.right;
        Gizmos.DrawRay(rearSightPoint.position, rearRayDirection * rearSightRange);

        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.gameObject.CompareTag("Player"))
        // {
        //     other.gameObject.GetComponent<Player>().TakeDamage(damage);
        // }
    }
}