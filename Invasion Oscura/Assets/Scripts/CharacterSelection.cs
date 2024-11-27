using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public List<GameObject> characters; // Lista de personajes para seleccionar
    private int selectedCharacterIndex = 0; // Índice del personaje seleccionado

    public void NextCharacter()
    {
        // Cambiar al siguiente personaje en la lista
        selectedCharacterIndex++;
        if (selectedCharacterIndex >= characters.Count)
        {
            selectedCharacterIndex = 0;
        }

        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        // Cambiar al personaje anterior en la lista
        selectedCharacterIndex--;
        if (selectedCharacterIndex < 0)
        {
            selectedCharacterIndex = characters.Count - 1;
        }

        UpdateCharacterDisplay();
    }

    public void ConfirmCharacter()
    {
        // Guardar el personaje seleccionado y cargar la escena principal del juego
        PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
        SceneManager.LoadScene("GameScene"); // Asegúrate de que el nombre sea el correcto
    }

    private void UpdateCharacterDisplay()
    {
        // Mostrar el personaje seleccionado en la pantalla (activar solo uno a la vez)
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].SetActive(i == selectedCharacterIndex);
        }
    }

    private void Start()
    {
        // Inicializar mostrando el primer personaje
        UpdateCharacterDisplay();
    }
}
