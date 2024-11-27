using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f; // Salud máxima del enemigo
    public GameObject collectiblePrefab; // Prefab del coleccionable que dejará caer el enemigo
    public Transform dropPoint; // Punto desde donde se dejará caer el coleccionable (por defecto, puede ser el enemigo)

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // Inicializar la salud del enemigo
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount; // Reducir la salud del enemigo
        Debug.Log("Salud del enemigo: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Lógica adicional, como animaciones de muerte, si es necesario
        Debug.Log("El enemigo ha muerto.");

        // Dejar caer un coleccionable si se ha asignado un prefab
        if (collectiblePrefab != null)
        {
            Instantiate(collectiblePrefab, dropPoint != null ? dropPoint.position : transform.position, Quaternion.identity);
        }

        // Destruir al enemigo
        Destroy(gameObject);
    }
}
