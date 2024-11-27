using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f; // Cantidad de daño que la bala inflige al enemigo
    public float lifetime = 5f; // Tiempo de vida del proyectil antes de destruirse automáticamente

    void Start()
    {
        // Destruir la bala después de un tiempo para evitar que se quede en la escena indefinidamente
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Comprobar si el proyectil colisiona con un enemigo
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Aplicar el daño al enemigo
            }

            // Destruir el proyectil después de impactar con el enemigo
            Destroy(gameObject);
        }
        else
        {
            // Destruir el proyectil al impactar cualquier otro objeto que no sea el enemigo
            Destroy(gameObject);
        }
    }
}
