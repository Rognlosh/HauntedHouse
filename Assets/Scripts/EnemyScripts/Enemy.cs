using UnityEngine;
using HountedHouse.Utils;

/// <summary>
/// Базовый класс для всех врагов.
/// Реализует конечный автомат состояний: Idle → Roaming → Chasing.
/// Наследники (Ghost, Flame) переопределяют поведение отдельных состояний.
/// </summary>
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Состояние")]
    [SerializeField] protected State startingState;

    [Header("Блуждание")]
    [SerializeField] protected float roamingDistanceMax = 5f;
    [SerializeField] protected float roamingDistanceMin = 3f;
    [SerializeField] protected float roamingTimerMax = 2f;

    [Header("Преследование")]
    [SerializeField] protected float chasingDistance = 5f;
    [SerializeField] protected float chasingSpeedMultiplier = 1f;

    protected enum State { Idle, Roaming, Chasing, Attacking }

    protected UnityEngine.AI.NavMeshAgent navMeshAgent;
    protected State currentState;
    protected float roamingTimer;
    protected Vector3 roamPosition;
    protected Vector3 startingPosition;
    protected float roamingSpeed;
    protected float chasingSpeed;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        currentState = startingState;
        roamingSpeed = navMeshAgent.speed;
        chasingSpeed = navMeshAgent.speed * chasingSpeedMultiplier;
    }

    protected virtual void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        if (!GameManager.Instance.GameIsActive) return;
        StateHandler();
    }

    private void StateHandler()
    {
        switch (currentState)
        {
            case State.Roaming:
                roamingTimer -= Time.deltaTime;
                if (roamingTimer <= 0f)
                {
                    Roaming();
                    roamingTimer = roamingTimerMax;
                }
                CheckCurrentState();
                break;

            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;

            case State.Attacking:
                break;

            case State.Idle:
            default:
                break;
        }
    }

    protected virtual void ChasingTarget()
    {
        navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    protected virtual void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = distanceToPlayer <= chasingDistance ? State.Chasing : State.Roaming;

        if (newState == currentState) return;

        if (newState == State.Chasing)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.speed = chasingSpeed;
        }
        else
        {
            roamingTimer = 0f;
            navMeshAgent.speed = roamingSpeed;
        }

        currentState = newState;
    }

    protected virtual void Roaming()
    {
        roamPosition = GetRoamingPosition();
        navMeshAgent.SetDestination(roamPosition);
    }

    protected virtual Vector3 GetRoamingPosition()
    {
        return startingPosition + Utils.GetRandomDir() * Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (PlayerStatuses.Instance.IsImmune) return;

        GameManager.Instance.LoseGame();
    }
}