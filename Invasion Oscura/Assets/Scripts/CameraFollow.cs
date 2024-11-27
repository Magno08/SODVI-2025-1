using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El objetivo que la cámara debe seguir
    public Vector3 offset = new Vector3(0, 5, -10); // Offset de la posición de la cámara respecto al objetivo
    public float smoothSpeed = 0.125f; // Velocidad de suavizado

    private void LateUpdate()
    {
        if (target == null) return;

        // Posición deseada de la cámara con offset
        Vector3 desiredPosition = target.position + offset;

        // Movimiento suave de la cámara desde su posición actual hacia la posición deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Actualizar la posición de la cámara
        transform.position = smoothedPosition;

        // Opcional: Apuntar siempre hacia el objetivo
        transform.LookAt(target);
    }
}
