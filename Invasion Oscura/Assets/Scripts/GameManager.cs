using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<GameObject> characterPrefabs; // Prefabs de todos los personajes
    public Transform spawnPoint; // Lugar donde el personaje será instanciado al iniciar el juego
    public CameraFollow cameraFollow; // Referencia al script CameraFollow de la cámara
    public Slider healthSlider; // Referencia al Slider de la vida del jugador
    public Slider buildProgressSlider; // Referencia al Slider de la construcción
    public TMP_Text collectableText; // Referencia al contador de coleccionables (TextMeshPro)

    private void Start()
    {
        // Obtener el índice del personaje seleccionado
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);

        // Instanciar el personaje en el punto de inicio
        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterPrefabs.Count)
        {
            GameObject character = Instantiate(characterPrefabs[selectedCharacterIndex], spawnPoint.position, spawnPoint.rotation);

            // Actualizar la cámara para que siga al personaje instanciado
            if (cameraFollow != null)
            {
                cameraFollow.target = character.transform;
            }

            // Configurar la UI en el script del personaje instanciado
            PlayerHealth playerHealth = character.GetComponent<PlayerHealth>();
            if (playerHealth != null && healthSlider != null)
            {
                playerHealth.healthSlider = healthSlider; // Asignar el Slider de vida
            }

            PlayerItemCollector playerItemCollector = character.GetComponent<PlayerItemCollector>();
            if (playerItemCollector != null && collectableText != null)
            {
                playerItemCollector.SetCollectableText(collectableText); // Asignar el contador de coleccionables utilizando un método para mantener lógica interna
            }

            BarricadeConstruction barricadeConstruction = character.GetComponent<BarricadeConstruction>();
            if (barricadeConstruction != null && buildProgressSlider != null)
            {
                barricadeConstruction.SetBuildProgressSlider(buildProgressSlider); // Asignar el Slider de construcción utilizando un método para mantener lógica interna
            }
        }
        else
        {
            Debug.LogWarning("Índice de personaje seleccionado fuera de rango.");
        }
    }
}
