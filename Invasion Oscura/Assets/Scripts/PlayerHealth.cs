using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthSlider; // Slider de la vida del jugador
    private Canvas deathCanvas; // Canvas que se mostrará cuando el jugador muera

    void Start()
    {
        currentHealth = maxHealth;

        // Buscar el Canvas en la escena por su nombre o etiqueta
        deathCanvas = GameObject.Find("DeathCanvas")?.GetComponent<Canvas>();
        
        if (deathCanvas != null)
        {
            deathCanvas.enabled = false; // Asegurarse de que el canvas de muerte esté desactivado al inicio
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log("Jugador ha recibido daño. Salud actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("El jugador ha muerto");
        // Mostrar el canvas de muerte
        if (deathCanvas != null)
        {
            deathCanvas.enabled = true;
        }

        // Detener el tiempo del juego
        Time.timeScale = 0f;

        // Esperar a que el jugador presione un botón para volver al menú
    }

    public void RestartGame()
    {
        // Reiniciar el tiempo del juego
        Time.timeScale = 1f;
        // Cargar la escena del menú principal
        SceneManager.LoadScene("MainMenu");
    }
}
