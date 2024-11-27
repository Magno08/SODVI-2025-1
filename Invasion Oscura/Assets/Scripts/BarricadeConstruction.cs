using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BarricadeConstruction : MonoBehaviour
{
    public Slider buildProgressSlider; // Referencia al slider del progreso
    public GameObject barricadePrefab; // Prefab de la barricada
    public float buildTime = 5f; // Tiempo necesario para construir
    public float rotationAngle; // Ángulo de rotación para la barricada
    public int requiredCollectibles = 5; // Cantidad de coleccionables necesarios para construir
    public List<Transform> buildPoints; // Lista de puntos donde se pueden construir las barricadas

    private Button buildButton; // Botón de construcción en la pantalla

    private bool isBuilding = false; // Indica si está en proceso de construcción
    private Transform currentBuildPoint; // Punto de construcción actual
    private Coroutine buildCoroutine; // Para rastrear la construcción en curso

    private PlayerItemCollector playerItemCollector; // Referencia al script de recolección del jugador

    void Start()
    {
        // Buscar el slider de progreso si no está asignado
        if (buildProgressSlider == null)
        {
            GameObject sliderObject = GameObject.Find("BuildProgressSlider");
            if (sliderObject != null)
            {
                buildProgressSlider = sliderObject.GetComponent<Slider>();
                buildProgressSlider.gameObject.SetActive(false); // Asegúrate de que el slider esté oculto inicialmente
            }
            else
            {
                Debug.LogError("No se encontró el objeto 'BuildProgressSlider' en la escena.");
            }
        }

        // Buscar el script PlayerItemCollector para acceder a los coleccionables del jugador
        playerItemCollector = FindFirstObjectByType<PlayerItemCollector>();
        if (playerItemCollector == null)
        {
            Debug.LogError("No se encontró el script PlayerItemCollector en la escena.");
        }

        // Buscar el botón de construcción en la escena por su nombre
        GameObject buildButtonObject = GameObject.Find("BuildButton");
        if (buildButtonObject != null)
        {
            buildButton = buildButtonObject.GetComponent<Button>();
            if (buildButton != null)
            {
                buildButton.onClick.AddListener(AttemptToBuild); // Vincula la función AttemptToBuild al botón
            }
            else
            {
                Debug.LogError("No se pudo encontrar el componente Button en el objeto 'BuildButton'.");
            }
        }
        else
        {
            Debug.LogError("No se encontró el botón de construcción en la escena. Asegúrate de que el botón tenga el nombre correcto.");
        }
    }

    // Método público para asignar el slider de progreso desde otros scripts
    public void SetBuildProgressSlider(Slider newSlider)
    {
        if (newSlider != null)
        {
            buildProgressSlider = newSlider;
            buildProgressSlider.gameObject.SetActive(false); // Asegúrate de que esté oculto inicialmente
            Debug.Log("Slider de progreso actualizado desde GameManager.");
        }
        else
        {
            Debug.LogError("Se intentó asignar un slider de progreso nulo.");
        }
    }

    // Método que intenta iniciar la construcción
    public void AttemptToBuild()
    {
        if (isBuilding)
        {
            Debug.Log("Ya se está construyendo.");
            return;
        }

        // Asegurarse de que el punto de construcción esté asignado y sea válido
        currentBuildPoint = GetClosestBuildPoint();
        if (currentBuildPoint == null)
        {
            Debug.LogError("No se encontró un punto de construcción cercano o no es válido.");
            return;
        }

        // Verificar si el jugador tiene suficientes coleccionables
        if (playerItemCollector != null && playerItemCollector.GetItemCount() >= requiredCollectibles)
        {
            // Descontar los coleccionables necesarios
            Debug.Log("Coleccionables disponibles antes de construir: " + playerItemCollector.GetItemCount());
            playerItemCollector.UseItems(requiredCollectibles);
            Debug.Log("Coleccionables después de descontar: " + playerItemCollector.GetItemCount());

            Debug.Log("Inicio de construcción en: " + currentBuildPoint.name);

            // Iniciar el proceso de construcción
            buildCoroutine = StartCoroutine(BuildProcess());
        }
        else
        {
            Debug.Log("No hay suficientes coleccionables para construir.");
        }
    }

    private IEnumerator BuildProcess()
    {
        isBuilding = true;

        if (buildProgressSlider != null)
        {
            buildProgressSlider.value = 0f; // Inicializar el valor del slider
            buildProgressSlider.gameObject.SetActive(true); // Mostrar el slider solo cuando comienza la construcción
        }

        float elapsedTime = 0f;

        while (elapsedTime < buildTime)
        {
            if (currentBuildPoint == null || Vector3.Distance(transform.position, currentBuildPoint.position) > 5f)
            {
                Debug.LogError("El punto de construcción se perdió o el jugador se alejó demasiado.");
                ResetConstruction();
                yield break;
            }

            elapsedTime += Time.deltaTime; // Incrementa el tiempo transcurrido
            float progress = Mathf.Clamp01(elapsedTime / buildTime); // Calcula el progreso en un rango de 0 a 1

            if (buildProgressSlider != null)
            {
                buildProgressSlider.value = progress; // Actualiza el slider con el progreso
                Debug.Log($"Progreso de construcción: {progress * 100}%");
            }

            yield return null;
        }

        // Finalizar la construcción
        CompleteConstruction();
    }

    private void CompleteConstruction()
    {
        isBuilding = false;

        if (buildProgressSlider != null)
        {
            buildProgressSlider.value = 1f; // Asegúrate de que esté al máximo
            buildProgressSlider.gameObject.SetActive(false); // Oculta el slider al finalizar la construcción
        }

        if (currentBuildPoint != null)
        {
            Vector3 buildPosition = currentBuildPoint.position;

            Vector3 positionOffset = new Vector3(0.5f, -2, -3); // Ajuste de posición
            Quaternion buildRotation = Quaternion.Euler(0, rotationAngle + 270f, 0);

            Instantiate(barricadePrefab, buildPosition + positionOffset, buildRotation);
            Debug.Log("Construcción completada en posición: " + (buildPosition + positionOffset) + " con rotación: " + buildRotation.eulerAngles);
        }
        else
        {
            Debug.LogError("No se pudo completar la construcción debido a un punto de construcción inválido.");
        }
    }

    public void ResetConstruction()
    {
        isBuilding = false;

        if (buildProgressSlider != null)
        {
            buildProgressSlider.value = 0f;
            buildProgressSlider.gameObject.SetActive(false); // Ocultar el slider
            Debug.Log("Construcción reiniciada. Slider oculto y valor restablecido a 0.");
        }

        if (buildCoroutine != null)
        {
            StopCoroutine(buildCoroutine);
            buildCoroutine = null;
        }
    }

    // Método para obtener el punto de construcción más cercano
    private Transform GetClosestBuildPoint()
    {
        Transform closestPoint = null;
        float closestDistance = float.MaxValue;

        foreach (Transform buildPoint in buildPoints)
        {
            float distance = Vector3.Distance(transform.position, buildPoint.position);
            if (distance < closestDistance)
            {
                closestPoint = buildPoint;
                closestDistance = distance;
            }
        }

        return closestPoint;
    }
}
