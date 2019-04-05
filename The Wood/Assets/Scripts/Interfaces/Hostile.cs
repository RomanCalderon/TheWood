using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public abstract class Hostile : Killable
{
    public enum AnimStates
    {
        IDLE,
        WALKING,
        RUNNING,
        ATTACKING
    }

    public int ID;
    protected CharacterStats characterStats;
    public int ExperienceReward;
    [HideInInspector] public Spawner Spawner;
    [HideInInspector] public DropTable DropTable;
    
    private Killable primaryTarget;
    private Killable currentTarget;
    private Killable obstacle;
    protected NavMeshAgent navAgent;
    [SerializeField] float lookRadius = 15f;
    [SerializeField] LayerMask detectableLayerMask;
    private Collider[] detectedColliders;
    [SerializeField] LayerMask obstacleLayerMask;

    Animator anim;
    [SerializeField] AnimStates currentAnimState;

    protected override void Start()
    {
        base.Start();

        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        SetAnimState(AnimStates.IDLE);
    }

    protected virtual void FixedUpdate()
    {
        Hunt();
    }

    #region Hunting and Attacking

    void Hunt()
    {
        if (obstacle == null)
        {
            if (FoundPrimaryTargets())
                primaryTarget = detectedColliders[0].GetComponent<Killable>();
            else
            {
                primaryTarget = null;
                obstacle = null;
                currentTarget = null;
                CancelInvoke("PerformAttack");
                return;
            }

            if (!CanNavigateToPoint(primaryTarget.transform.position))
            {
                obstacle = GetObstacle();
                return;
            }

            ChaseTarget(primaryTarget);
        }
        else
        {
            ChaseTarget(obstacle);

            if (!FoundPrimaryTargets())
            {
                primaryTarget = null;
                obstacle = null;
                currentTarget = null;
                CancelInvoke("PerformAttack");
                return;
            }
        }
    }

    bool CanNavigateToPoint(Vector3 point)
    {
        NavMeshPath checkPath = new NavMeshPath();
        navAgent.CalculatePath(point, checkPath);
        return (checkPath.status == NavMeshPathStatus.PathComplete);
    }

    Killable GetObstacle()
    {
        if (primaryTarget == null)
            return null;

        Vector3 startPoint = transform.position;
        Vector3 endPoint = primaryTarget.transform.position;
        RaycastHit hit;

        if (Physics.Linecast(startPoint, endPoint, out hit, obstacleLayerMask))
            return hit.transform.GetComponent<Killable>();

        return null;
    }

    void ChaseTarget(Killable newTarget)
    {
        if (newTarget == null)
            return;

        currentTarget = newTarget;

        navAgent.SetDestination(transform.position + (currentTarget.transform.position - transform.position) * 0.9f);
        
        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            FaceTarget();
            
            if (!IsInvoking("PerformAttack"))
                InvokeRepeating("PerformAttack", 0.1f, 1.5f);
        }
        else
            CancelInvoke("PerformAttack");
    }

    void FaceTarget()
    {
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 7f);
    }

    public void PerformAttack()
    {
        SetAnimState(AnimStates.ATTACKING);

        // Check if new path to primaryTarget exists
        CheckNewPath();
    }

    bool FoundPrimaryTargets()
    {
        detectedColliders = Physics.OverlapSphere(transform.position, lookRadius, detectableLayerMask);

        return (detectedColliders.Length > 0);
    }

    void CheckNewPath()
    {
        if (obstacle != null && CanNavigateToPoint(primaryTarget.transform.position))
        {
            obstacle = null;
            CancelInvoke("PerformAttack");
            ChaseTarget(primaryTarget);
        }
    }

    // Called in attack animation event
    public void CheckAttack()
    {
        // Check if this attack will miss or be blocked

        // If the attack connects, deal damage
        DealDamage();
    }

    private void DealDamage()
    {
        currentTarget.TakeDamage(characterStats.GetStat(BaseStat.BaseStatType.ATTACK).GetCalculatedStatValue());
    }

    #endregion


    protected override void Die()
    {
        ItemDatabase.instance.DropLoot(DropTable.GetDrop(), transform.position);
        CombatEvents.EnemyDied(this);

        if (Spawner)
            Spawner.Respawn();

        Destroy(gameObject);
    }

    private void SetAnimState(AnimStates newState)
    {
        if (newState == AnimStates.ATTACKING)
            anim.SetTrigger("Attack");

        if (newState == currentAnimState)
            return;

        currentAnimState = newState;

        switch (newState)
        {
            case AnimStates.IDLE:
                anim.SetTrigger("Idle");
                break;
            case AnimStates.WALKING:
                anim.SetTrigger("Walk");
                break;
            case AnimStates.RUNNING:
                anim.SetTrigger("Run");
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        if (primaryTarget != null)
            Gizmos.DrawLine(transform.position, primaryTarget.transform.position);
        Gizmos.color = Color.green;
        if (currentTarget != null)
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);

        if (navAgent == null)
            return;

        Gizmos.color = Color.red;
        int index = 0;
        foreach (Vector3 p in navAgent.path.corners)
        {
            Gizmos.DrawCube(p, Vector3.one * 0.1f);
            if (index < navAgent.path.corners.Length - 1)
                Gizmos.DrawLine(p, navAgent.path.corners[index + 1]);
            index++;
        }
    }

}
