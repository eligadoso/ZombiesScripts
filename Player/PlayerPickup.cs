using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform mano; // Asignar desde el inspector
    private GameObject objetoEnMano;
    private void OnTriggerStay(Collider other)
    {
        // Si el collider está en el hijo, tomamos su objeto padre
        GameObject Item = other.transform.root.gameObject;

        if (objetoEnMano == null && Item.CompareTag("Interactuable") && Input.GetKeyDown(KeyCode.F))
        {
            TomarObjeto(Item);
        }
    }
    void Update()
    {
        AccionesObjeto();
    }
    void AccionesObjeto()
    {
        if(!objetoEnMano) return;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            IInteractuable interactuable = objetoEnMano.GetComponent<IInteractuable>();
            if (interactuable != null)
            {
                interactuable.Usar();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SoltarObjeto();
        }
    }
    void TomarObjeto(GameObject objeto)
    {
        objeto.transform.SetParent(mano);
        objeto.transform.localPosition = Vector3.zero;
        objeto.transform.localRotation = Quaternion.identity;
        objetoEnMano = objeto;

        // Opcional: desactivar físicas
        if (objeto.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Debug.Log($"Objeto tomado: {objeto.name}");
    }
    void SoltarObjeto()
    {
        if (objetoEnMano != null)
        {
            // Desvincular de la mano
            objetoEnMano.transform.SetParent(null);

            // Reactivar físicas
            if (objetoEnMano.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
                // Opcional: empujar un poco hacia adelante al soltar
                rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
            }

            objetoEnMano = null;
            Debug.Log("Objeto soltado");
        }
    }
}
