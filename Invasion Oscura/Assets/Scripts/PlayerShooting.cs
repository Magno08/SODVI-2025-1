using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab del proyectil que se disparará
    public Transform firePoint; // Punto desde donde se disparará el proyectil
    public float projectileSpeed = 20f; // Velocidad del proyectil

    public void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Crear el proyectil en el punto de disparo con la misma rotación que el jugador
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            // Añadir velocidad al proyectil
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * projectileSpeed;
            }

            Debug.Log("Proyectil disparado desde el jugador");
        }
        else
        {
            Debug.LogError("Faltan referencias: asegúrate de asignar el prefab del proyectil y el punto de disparo.");
        }
    }
}
