using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerItemCollector : MonoBehaviour
{
    public TMP_Text itemCounterText; // Referencia al texto en el Canvas que muestra el contador de objetos
    private int itemCount = 5; // Contador de objetos recogidos

    void Start()
    {
        UpdateItemCounterUI(); // Actualizar el contador de objetos al inicio
    }

    void OnTriggerEnter(Collider other)
    {
        // Comprobar si el objeto tiene la etiqueta "Collectible"
        if (other.gameObject.CompareTag("Collectible"))
        {
            itemCount++; // Incrementar el contador de objetos
            UpdateItemCounterUI(); // Actualizar la interfaz de usuario
            Destroy(other.gameObject); // Destruir el objeto recogido
        }
    }

    void UpdateItemCounterUI()
    {
        if (itemCounterText != null)
        {
            itemCounterText.text = "Items: " + itemCount; // Actualizar el texto del contador
        }
    }

    public int GetItemCount()
    {
        Debug.Log("Cantidad de coleccionables actuales: " + itemCount);
        return itemCount; // Devolver la cantidad actual de ítems
    }

    public void UseItems(int amount)
    {
        if (itemCount >= amount)
        {
            itemCount -= amount; // Descontar los ítems usados
            Debug.Log("Coleccionables usados: " + amount + ", Coleccionables restantes: " + itemCount);
            UpdateItemCounterUI(); // Actualizar la interfaz de usuario
        }
        else
        {
            Debug.LogError("Intento de usar más coleccionables de los disponibles.");
        }
    }

    public void SetCollectableText(TMP_Text newItemCounterText)
    {
        itemCounterText = newItemCounterText; // Asignar dinámicamente el contador de coleccionables
        UpdateItemCounterUI(); // Asegurarse de que el UI se actualice con la nueva referencia
    }
}
