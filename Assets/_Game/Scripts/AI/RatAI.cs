using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Realistic rodent behavior AI with state machine
/// Handles detection, fleeing, patrolling, and death
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HeatSignature))]
public class RatAI : MonoBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] private RatSize size = RatSize.Medium;
    [SerializeField] private RatType type = RatType.Drone;
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float sprintMultiplier = 2.5f;
    [SerializeField] private int healthPoints = 1;

    [Header("Behavior Settings")]
    [SerializeField] private float idleDuration = 3f;
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float alertRadius = 5f;
    [SerializeField] private float hearingRange = 8f;

    [Header("Detection")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionAngle = 120f;

    private NavMeshAgent navAgent;
    private HeatSignature heatSignature;
    private Animator animator;
    private AIState currentState;
    private Vector3 homePosition;
    private float stateTimer;
    private bool isDead;
    private int maxHealthPoints;

    public enum RatSize { Small, Medium, Large }
    public enum RatType { Drone, NestMother, Alpha }

    private enum AIState
    {
        Idle,           // Standing still, looking around
        Feeding,        // Eating, vulnerable
        Patrolling,     // Wandering around home area
        Investigating,  // Checking out a sound
        Alerted,        // Sensed danger, frozen
        Fleeing,        // Running to cover
        Hidden          // In unreachable hiding spot
    }

    // Properties
    public bool IsDead => isDead;
    public RatType Type => type;
    public RatSize Size => size;
    public AIState CurrentAIState => currentState;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        heatSignature = GetComponent<HeatSignature>();
        animator = GetComponentInChildren<Animator>();

        ConfigureForSize();
        ConfigureForType();
        homePosition = transform.position;
        maxHealthPoints = healthPoints;
    }

    private void ConfigureForSize()
    {
        float sizeMultiplier = size switch
        {
            RatSize.Small => 0.7f,
            RatSize.Medium => 1.0f,
            RatSize.Large => 1.3f,
            _ => 1.0f
        };

        transform.localScale = Vector3.one * sizeMultiplier;
        navAgent.speed = baseSpeed * sizeMultiplier;
        healthPoints = size == RatSize.Large ? 2 : 1;
    }

    private void ConfigureForType()
    {
        switch (type)
        {
            case RatType.Drone:
                // Standard rat - no special stats
                break;

            case RatType.NestMother:
                healthPoints = 3;
                baseSpeed *= 0.8f; // Slower
                transform.localScale *= 1.5f; // Larger
                break;

            case RatType.Alpha:
                healthPoints = 2;
                baseSpeed *= 1.2f; // Faster
                alertRadius *= 1.5f; // More aware
                break;
        }

        navAgent.speed = baseSpeed;
    }

    private void Start()
    {
        TransitionToState(AIState.Idle);
    }

    private void Update()
    {
        if (isDead) return;

        stateTimer -= Time.deltaTime;

        // Check for threats
        if (DetectDanger())
        {
            OnDangerDetected();
        }

        // Execute current state behavior
        ExecuteState();

        // State transitions
        if (stateTimer <= 0f)
        {
            DetermineNextState();
        }
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                ExecuteIdle();
                break;
            case AIState.Feeding:
                ExecuteFeeding();
                break;
            case AIState.Patrolling:
                ExecutePatrolling();
                break;
            case AIState.Investigating:
                ExecuteInvestigating();
                break;
            case AIState.Alerted:
                ExecuteAlerted();
                break;
            case AIState.Fleeing:
                ExecuteFleeing();
                break;
            case AIState.Hidden:
                ExecuteHidden();
                break;
        }
    }

    private void ExecuteIdle()
    {
        // Occasionally look around
        if (Random.value < 0.02f)
        {
            float randomAngle = Random.Range(-60f, 60f);
            transform.Rotate(0f, randomAngle, 0f);
        }

        animator?.SetFloat("Speed", 0f);
    }

    private void ExecuteFeeding()
    {
        // Vulnerable state - head down
        animator?.SetBool("Feeding", true);
    }

    private void ExecutePatrolling()
    {
        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            // Pick new patrol point
            Vector3 randomPoint = homePosition.RandomPointInRadius(patrolRadius);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            {
                navAgent.SetDestination(hit.position);
            }
        }

        animator?.SetFloat("Speed", navAgent.velocity.magnitude);
    }

    private void ExecuteInvestigating()
    {
        // Move towards point of interest
        if (!navAgent.pathPending && navAgent.remainingDistance < 1f)
        {
            // Investigated, return to normal
            TransitionToState(AIState.Idle);
        }

        animator?.SetFloat("Speed", navAgent.velocity.magnitude);
    }

    private void ExecuteAlerted()
    {
        // Frozen in place
        navAgent.isStopped = true;
        animator?.SetTrigger("Alert");
    }

    private void ExecuteFleeing()
    {
        // Sprint to nearest cover
        navAgent.speed = baseSpeed * sprintMultiplier;

        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            TransitionToState(AIState.Hidden);
        }

        animator?.SetFloat("Speed", navAgent.velocity.magnitude);
        animator?.SetBool("Sprinting", true);
    }

    private void ExecuteHidden()
    {
        // In cover, very hard to hit
        // Maybe peek out occasionally
        if (Random.value < 0.001f)
        {
            // Return to patrol
            TransitionToState(AIState.Patrolling);
        }
    }

    private void DetermineNextState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                // Random choice: stay idle, feed, or patrol
                float roll = Random.value;
                if (roll < 0.4f)
                    TransitionToState(AIState.Feeding);
                else if (roll < 0.7f)
                    TransitionToState(AIState.Patrolling);
                else
                    TransitionToState(AIState.Idle);
                break;

            case AIState.Feeding:
                TransitionToState(AIState.Idle);
                break;

            case AIState.Patrolling:
                TransitionToState(AIState.Idle);
                break;

            case AIState.Alerted:
                // After freeze, flee
                TransitionToState(AIState.Fleeing);
                break;
        }
    }

    private void TransitionToState(AIState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case AIState.Idle:
                stateTimer = Random.Range(2f, 5f);
                navAgent.isStopped = true;
                break;
            case AIState.Feeding:
                stateTimer = Random.Range(3f, 6f);
                navAgent.isStopped = true;
                break;
            case AIState.Patrolling:
                stateTimer = Random.Range(5f, 10f);
                navAgent.isStopped = false;
                navAgent.speed = baseSpeed;
                break;
            case AIState.Investigating:
                stateTimer = 10f;
                navAgent.isStopped = false;
                break;
            case AIState.Alerted:
                stateTimer = Random.Range(0.3f, 0.8f);
                break;
            case AIState.Fleeing:
                stateTimer = 5f;
                navAgent.isStopped = false;
                FindNearestCover();
                break;
            case AIState.Hidden:
                stateTimer = Random.Range(20f, 40f);
                navAgent.isStopped = true;
                break;
        }
    }

    private bool DetectDanger()
    {
        // Check for player in view
        Collider[] nearby = Physics.OverlapSphere(transform.position, alertRadius, playerLayer);
        if (nearby.Length > 0)
        {
            Vector3 directionToPlayer = (nearby[0].transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < detectionAngle * 0.5f)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDangerDetected()
    {
        if (currentState != AIState.Alerted && currentState != AIState.Fleeing && currentState != AIState.Hidden)
        {
            TransitionToState(AIState.Alerted);
        }
    }

    public void OnHeardSound(Vector3 soundPosition, float loudness)
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, soundPosition);

        if (distance < hearingRange * loudness)
        {
            if (loudness > 0.7f) // Gunshot
            {
                OnDangerDetected();
            }
            else // Quieter sound
            {
                TransitionToState(AIState.Investigating);
                navAgent.SetDestination(soundPosition);
            }
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint, bool isWeakPoint)
    {
        if (isDead) return;

        healthPoints -= damage;
        heatSignature?.OnHit(hitPoint, isWeakPoint);

        if (healthPoints <= 0)
        {
            Die(isWeakPoint);
        }
        else
        {
            // Damaged but not dead - flee!
            OnDangerDetected();
        }
    }

    private void Die(bool wasWeakPointKill)
    {
        isDead = true;
        navAgent.enabled = false;
        animator?.SetTrigger("Death");
        heatSignature?.OnDeath();

        // Notify other rats
        NotifyNearbyRats();

        // Publish event
        EventBus.Publish(new TargetKilledEvent
        {
            Target = gameObject,
            IsWeakPoint = wasWeakPointKill,
            ScoreGained = CalculateScore(wasWeakPointKill)
        });

        // Handle death cleanup
        StartCoroutine(HandleDeath());

        Debug.Log($"[RatAI] {gameObject.name} ({type}) died");
    }

    private int CalculateScore(bool wasWeakPointKill)
    {
        int baseScore = Constants.BASE_SCORE_PER_KILL;

        // Type multiplier
        switch (type)
        {
            case RatType.NestMother:
                baseScore *= 3;
                break;
            case RatType.Alpha:
                baseScore *= 2;
                break;
        }

        // Weak point bonus
        if (wasWeakPointKill)
        {
            baseScore += Constants.HEADSHOT_BONUS;
        }

        return baseScore;
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(5f);

        // Fade out thermal signature (already handled by HeatSignature component)
        // Return to pool or destroy
        ObjectPooler pooler = ServiceLocator.Instance.TryGet<ObjectPooler>();
        if (pooler != null)
        {
            pooler.Despawn(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void NotifyNearbyRats()
    {
        Collider[] nearbyRats = Physics.OverlapSphere(transform.position, alertRadius * 2f);
        foreach (var col in nearbyRats)
        {
            RatAI rat = col.GetComponent<RatAI>();
            if (rat != null && rat != this && !rat.isDead)
            {
                rat.OnDangerDetected();
            }
        }
    }

    private void FindNearestCover()
    {
        // Find hiding spot away from player
        GameObject player = GameObject.FindGameObjectWithTag(Constants.TAG_PLAYER);
        if (player == null)
        {
            // Just flee in random direction
            Vector3 randomDirection = Random.insideUnitSphere.normalized;
            Vector3 targetPosition = transform.position + randomDirection * 10f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, 10f, NavMesh.AllAreas))
            {
                navAgent.SetDestination(hit.position);
            }
            return;
        }

        Vector3 fleeDirection = (transform.position - player.transform.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * 10f;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(fleeTarget, out navHit, 10f, NavMesh.AllAreas))
        {
            navAgent.SetDestination(navHit.position);
        }
    }

    // LOD System support
    public void SetLODLevel(int lodLevel, float updateRate)
    {
        switch (lodLevel)
        {
            case 0: // High detail
                enabled = true;
                if (navAgent != null) navAgent.enabled = true;
                break;
            case 1: // Medium detail
                enabled = true;
                if (navAgent != null) navAgent.enabled = true;
                break;
            case 2: // Low detail
                enabled = true;
                if (navAgent != null) navAgent.enabled = false;
                break;
            case 3: // Disabled
                enabled = false;
                if (navAgent != null) navAgent.enabled = false;
                break;
        }
    }
}
