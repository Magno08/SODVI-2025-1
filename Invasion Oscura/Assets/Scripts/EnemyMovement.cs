using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float chaseRange = 15f; // Distancia máxima para perseguir al jugador
    public LayerMask obstacleLayer; // Capa para identificar los obstáculos

    private NavMeshAgent agent;
    private EnemyActions enemyActions; // Referencia al script de acciones del enemigo

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyActions = GetComponent<EnemyActions>();

        // Asignar automáticamente el jugador si no está configurado
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("No se encontró un objeto con la etiqueta 'Player'. Asegúrate de que el jugador tenga esta etiqueta.");
            }
        }

        if (enemyActions == null)
        {
            Debug.LogError("No se encontró el componente EnemyActions en el enemigo.");
        }
    }

    void Update()
    {
        if (enemyActions != null)
        {
            // Si el enemigo está en estado de ataque pero no tiene un objetivo, vuelve a perseguir
            if (enemyActions.CurrentState == EnemyActions.State.Attacking && enemyActions.CurrentTarget == null)
            {
                enemyActions.SetStateToChasing();
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }

            // Lógica de persecución normal
            if (enemyActions.CurrentState == EnemyActions.State.Chasing || enemyActions.CurrentState == EnemyActions.State.Idle)
            {
                if (enemyActions.CurrentTarget != null)
                {
                    agent.isStopped = false;
                    agent.SetDestination(enemyActions.CurrentTarget.position);
                }
                else
                {
                    agent.isStopped = true;
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (enemyActions == null) return; // Evitar acciones si no hay un EnemyActions válido

        // Si colisiona con el jugador o un obstáculo, pasar al estado de ataque
        if (collision.gameObject.CompareTag("Player") || (obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            agent.isStopped = true; // Detener el movimiento del agente
            enemyActions.SetStateToAttacking(collision.transform);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (enemyActions == null) return; // Evitar acciones si no hay un EnemyActions válido

        // Si el enemigo sigue en contacto, se asegura de continuar atacando
        if (collision.gameObject.CompareTag("Player") || (obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            enemyActions.SetStateToAttacking(collision.transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (enemyActions == null) return;

        // Si deja de colisionar con un obstáculo o el jugador
        if (collision.gameObject.CompareTag("Player") || (obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (enemyActions.CurrentTarget != null && enemyActions.CurrentTarget == collision.transform)
            {
                enemyActions.SetStateToChasing(); // Volver al estado de persecución
            }

            agent.isStopped = false; // Asegurar que el movimiento se reanude
            agent.SetDestination(player.position); // Reasignar el destino
        }
    }
}
