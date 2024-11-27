using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float chaseRange = 15f; // Distancia máxima para perseguir al jugador
    public float attackRange = 3f; // Distancia de ataque
    public int damageToObstacle = 10; // Daño que inflige a los obstáculos
    public int damageToPlayer = 5; // Daño que inflige al jugador
    public float attackCooldown = 1f; // Tiempo de espera entre ataques
    public LayerMask obstacleLayer; // Capa para identificar los obstáculos

    private NavMeshAgent agent;
    private float lastAttackTime = 0f; // Tiempo del último ataque
    private Transform currentTarget; // Objetivo actual del enemigo (obstáculo o jugador)
    private enum State { Idle, Chasing, Attacking }
    private State currentState = State.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (currentState == State.Idle || currentState == State.Chasing)
        {
            FindAndChaseTarget();
        }
    }

    void FindAndChaseTarget()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Priorizar al jugador si está en rango de persecución
        if (distanceToPlayer <= chaseRange)
        {
            currentTarget = player;
            currentState = State.Chasing;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Buscar obstáculos cercanos
            Collider[] obstaclesInRange = Physics.OverlapSphere(transform.position, chaseRange, obstacleLayer);
            if (obstaclesInRange.Length > 0)
            {
                currentTarget = obstaclesInRange[0].transform;
                currentState = State.Chasing;
                agent.isStopped = false;
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                currentTarget = null;
                currentState = State.Idle;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Atacar al jugador si está en contacto
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null && Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackPlayer(playerHealth);
            }
        }

        // Atacar al obstáculo si está en contacto
        if ((obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
            if (obstacle != null && Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackObstacle(obstacle);
            }
        }
    }

    void AttackPlayer(PlayerHealth playerHealth)
    {
        Debug.Log("Atacando al jugador");
        playerHealth.TakeDamage(damageToPlayer);
    }

    void AttackObstacle(Obstacle obstacle)
    {
        Debug.Log("Atacando al obstáculo");
        obstacle.TakeDamage(damageToObstacle);
        if (obstacle.health <= 0)
        {
            currentTarget = null;
            currentState = State.Idle; // Volver a buscar un nuevo objetivo si el obstáculo es destruido
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detener el movimiento al entrar en contacto con un objetivo
        if (collision.gameObject.CompareTag("Player") || (obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            agent.isStopped = true; // Detener el movimiento del agente al atacar
            currentState = State.Attacking;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Reactivar el movimiento si se deja de estar en contacto con el objetivo
        if (collision.gameObject.CompareTag("Player") || (obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            agent.isStopped = false; // Reactivar el movimiento cuando ya no haya contacto
            currentState = State.Chasing; // Volver al estado de persecución
        }
    }
}
