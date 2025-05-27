using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProyectilMultiplayer : NetworkBehaviour
{
    public float velocidad;
    public float tiempoDeVida;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;

        rb.AddForce(transform.forward * velocidad, ForceMode.Impulse);

        // Destruir en red después de cierto tiempo
        Invoke(nameof(DestruirEnRed), tiempoDeVida);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Solo el servidor gestiona colisiones

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("¡Impacto en enemigo!");
            DestruirEnRed();
        }
        else if (other.CompareTag("Ground"))
        {
            DestruirEnRed();
        }
    }

    void DestruirEnRed()
    {
        if (IsServer && gameObject.TryGetComponent(out NetworkObject netObj))
        {
            netObj.Despawn(true);
        }
    }
}
