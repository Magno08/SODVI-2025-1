using UnityEngine;

public class EnemyActions : MonoBehaviour
{
    public enum State { Idle, Chasing, Attacking }
    public State CurrentState { get; private set; } = State.Idle;

    public Transform CurrentTarget { get; private set; } // Objetivo actual del enemigo (jugador u obstáculo)
    public int damageToObstacle = 10; // Daño que inflige a los obstáculos
    public int damageToPlayer = 5; // Daño que inflige al jugador
    public float attackCooldown = 1f; // Tiempo de espera entre ataques

    private float lastAttackTime = 0f; // Tiempo del último ataque

    private void Start()
    {
        CurrentState = State.Idle;
    }

    private void Update()
    {
        // Transiciones de estados cuando el enemigo está Idle o persiguiendo
        if (CurrentState == State.Idle)
        {
            FindTarget();
        }
        else if (CurrentState == State.Chasing && (CurrentTarget == null || CurrentTarget.gameObject == null))
        {
            CurrentState = State.Idle;
            CurrentTarget = null;
        }
    }

    private void FindTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distanceToPlayer <= GetComponent<EnemyMovement>().chaseRange)
        {
            CurrentTarget = playerObject.transform;
            CurrentState = State.Chasing;
        }
        else
        {
            Collider[] obstaclesInRange = Physics.OverlapSphere(transform.position, GetComponent<EnemyMovement>().chaseRange, GetComponent<EnemyMovement>().obstacleLayer);
            if (obstaclesInRange.Length > 0)
            {
                CurrentTarget = obstaclesInRange[0].transform;
                CurrentState = State.Chasing;
            }
        }
    }

    public void SetStateToChasing()
    {
        if (CurrentTarget != null)
        {
            CurrentState = State.Chasing;
        }
    }

    public void SetStateToAttacking(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("El objetivo de ataque es nulo.");
            return;
        }

        CurrentTarget = target;
        CurrentState = State.Attacking;

        if (target.CompareTag("Player"))
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null && Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackPlayer(playerHealth);
                SetStateToChasing(); // Cambiar al estado de persecución después de atacar al jugador
            }
        }
        else if ((GetComponent<EnemyMovement>().obstacleLayer.value & (1 << target.gameObject.layer)) > 0)
        {
            Obstacle obstacle = target.GetComponent<Obstacle>();
            if (obstacle != null && Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackObstacle(obstacle);
                SetStateToChasing(); // Cambiar al estado de persecución después de atacar al obstáculo
            }
        }
    }

    private void AttackPlayer(PlayerHealth playerHealth)
    {
        Debug.Log("Atacando al jugador");
        playerHealth.TakeDamage(damageToPlayer);
    }

    private void AttackObstacle(Obstacle obstacle)
    {
        if (obstacle == null)
        {
            Debug.LogWarning("El obstáculo es nulo al intentar atacar.");
            return;
        }

        Debug.Log("Atacando al obstáculo");
        obstacle.TakeDamage(damageToObstacle);

        if (obstacle.health <= 0)
        {
            CurrentTarget = null;
            CurrentState = State.Idle; // Volver a buscar un nuevo objetivo si el obstáculo es destruido
        }
    }
}
