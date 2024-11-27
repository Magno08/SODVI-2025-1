using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private CharacterController _player;
    private float _moveSpeed;
    private Vector3 _moveAxis;
    private Vector3 _camForward, _camRight, _moveDir;
    private Camera _camera;
    private float gravity = -9.81f; // Gravedad
    private float verticalVelocity = 0f; // Velocidad vertical del jugador

    [HideInInspector] public bool canMove = true; // Variable pública para controlar el movimiento desde otros scripts

    public List<Transform> buildPoints; // Lista de puntos donde se pueden construir los cubos
    public float buildRange = 5f; // Distancia para poder construir el cubo
    private Transform currentBuildPoint; // El buildPoint actual donde se está construyendo

    public BarricadeConstruction barricadeConstruction; // Referencia al script de construcción de barricada
    private PlayerItemCollector playerItemCollector; // Referencia al script de recolección de objetos

    void Awake()
    {
        _player = GetComponent<CharacterController>();
        _moveSpeed = 10f; // Ajusta la velocidad según sea necesario
        _camera = Camera.main;
        playerItemCollector = GetComponent<PlayerItemCollector>(); // Obtener la referencia al script PlayerItemCollector
    }

    private void Update()
    {
        if (!canMove) return; // Si no puede moverse, detener la ejecución

        // Verifica si el jugador está dentro del rango de construcción de algún buildPoint
        Transform previousBuildPoint = currentBuildPoint;
        currentBuildPoint = GetClosestBuildPointInRange();

        if (currentBuildPoint != null && SimpleInput.GetButtonDown("Fire2"))
        {
            // Intentar construir usando el script de construcción solo si tiene suficientes coleccionables
            if (playerItemCollector != null && playerItemCollector.GetItemCount() >= 5)
            {
                playerItemCollector.UseItems(5); // Descontar los coleccionables necesarios
                barricadeConstruction.AttemptToBuild();

            }
            else
            {
                Debug.Log("No hay suficientes coleccionables para construir.");
            }
        }

        // Si el jugador se aleja del punto de construcción, ocultar el slider de progreso y reiniciar su valor
        if (previousBuildPoint != currentBuildPoint && barricadeConstruction != null && barricadeConstruction.buildProgressSlider != null)
        {
            barricadeConstruction.buildProgressSlider.gameObject.SetActive(false);
            barricadeConstruction.buildProgressSlider.value = 0f;
        }

        // Manejar rotación de la barricada
        if (Input.GetKey(KeyCode.Q)) // Rotar en sentido antihorario
        {
            barricadeConstruction.rotationAngle -= 90f * Time.deltaTime; // Rota lentamente con el tiempo (puedes ajustar la velocidad)
        }
        if (Input.GetKey(KeyCode.E)) // Rotar en sentido horario
        {
            barricadeConstruction.rotationAngle += 90f * Time.deltaTime; // Rota lentamente con el tiempo (puedes ajustar la velocidad)
        }

        // Obtener entrada del jugador
        _moveAxis = new Vector3(SimpleInput.GetAxis("Horizontal"), 0, SimpleInput.GetAxis("Vertical"));

        // Calcular la dirección en relación con la cámara
        cameraDirection();
        _moveDir = _moveAxis.x * _camRight + _moveAxis.z * _camForward;
        _moveDir.y = 0; // Asegurar que no haya componente vertical en el movimiento

        // Rotar el personaje si hay movimiento significativo
        if (_moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDir);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Aplicar gravedad
        if (_player.isGrounded)
        {
            verticalVelocity = -1f; // Mantener al jugador pegado al suelo
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Aplicar gravedad si está en el aire
        }

        // Movimiento final del jugador
        Vector3 finalMove = _moveDir * _moveSpeed;
        finalMove.y = verticalVelocity;

        _player.Move(finalMove * Time.deltaTime);
    }

    private void cameraDirection()
    {
        // Obtener las direcciones de la cámara
        _camForward = _camera.transform.forward;
        _camRight = _camera.transform.right;

        // Ignorar inclinación vertical
        _camForward.y = 0;
        _camRight.y = 0;

        // Normalizar las direcciones
        _camForward.Normalize();
        _camRight.Normalize();
    }

    private Transform GetClosestBuildPointInRange()
    {
        Transform closestPoint = null;
        float closestDistance = buildRange;

        foreach (Transform buildPoint in buildPoints)
        {
            float distance = Vector3.Distance(transform.position, buildPoint.position);
            if (distance <= buildRange && (closestPoint == null || distance < closestDistance))
            {
                closestPoint = buildPoint;
                closestDistance = distance;
            }
        }

        return closestPoint;
    }
}
