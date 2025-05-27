using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilBasics : MonoBehaviour
{
    public float velocidad;
    public float tiempoDeVida;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero; // Asegura que parte sin impulso

        // Aplicar impulso al proyectil hacia adelante desde su rotación local
        rb.AddForce(transform.forward * velocidad, ForceMode.Impulse);

        // Destruir después de cierto tiempo
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("¡Impacto en enemigo!");
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
