using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerPrueba : MonoBehaviour
{
    private CharacterController _player;
    private float _moveSpeed = 10f;
    private Vector3 _moveDir;
    private float gravity = -9.81f; // Gravedad
    private float verticalVelocity = 0f; // Velocidad vertical del jugador

    private Camera _camera;

    [HideInInspector] public bool canMove = true; // Variable pública para controlar el movimiento desde otros scripts

    void Awake()
    {
        _player = GetComponent<CharacterController>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (!canMove) return; // Si no puede moverse, detener la ejecución

        // Obtener entrada del jugador
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Dirección en relación a la cámara
        Vector3 forward = _camera.transform.forward;
        Vector3 right = _camera.transform.right;

        // Ignorar la componente vertical
        forward.y = 0;
        right.y = 0;

        // Normalizar las direcciones
        forward.Normalize();
        right.Normalize();

        // Calcular la dirección de movimiento
        _moveDir = (forward * vertical + right * horizontal).normalized;

        // Aplicar rotación solo si hay movimiento
        if (_moveDir.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Aplicar la gravedad de manera controlada
        if (_player.isGrounded)
        {
            verticalVelocity = -1f; // Mantener al jugador pegado al suelo
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Aplicar gravedad en el aire
        }

        // Aplicar movimiento en el CharacterController
        Vector3 finalMove = _moveDir * _moveSpeed;
        finalMove.y = verticalVelocity;

        _player.Move(finalMove * Time.deltaTime);
    }
}
