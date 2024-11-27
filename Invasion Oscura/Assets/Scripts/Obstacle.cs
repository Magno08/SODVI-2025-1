using UnityEngine;
using UnityEngine.UI; // Para trabajar con UI
using System;

public class Obstacle : MonoBehaviour
{
    public float health = 50f; // Vida inicial del obstáculo
    public Slider healthBar;   // Referencia al Slider de la vida del obstáculo

    public event Action OnDestroyed; // Evento que se dispara cuando el obstáculo es destruido

    void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("Slider de salud no asignado en el obstáculo.");
        }
        else
        {
            // Inicializar la barra de vida
            healthBar.maxValue = health;
            healthBar.value = health;
        }
    }

    // Método para aplicar daño al obstáculo
    public void TakeDamage(float damage)
    {
        health -= damage; // Reducir vida

        // Actualizar la barra de vida
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        Debug.Log("Vida del obstáculo: " + health);

        if (health <= 0)
        {
            // Si la vida llega a 0 o menos, destruir el obstáculo
            OnDestroyed?.Invoke(); // Invocar el evento antes de destruir el objeto
            Destroy(gameObject);
            Debug.Log("El obstáculo ha sido destruido.");
        }
    }
}
