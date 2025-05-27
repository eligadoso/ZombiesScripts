using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerPickupMultiplayer : NetworkBehaviour
{
    public Transform mano;
    private GameObject objetoEnMano;

    private void Start()
    {
        if (mano == null)
            mano = transform.Find("Hand");
    }

    void Update()
    {
        if (!IsOwner) return;
        AccionesObjeto();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOwner) return;

        GameObject item = other.transform.root.gameObject;

        if (objetoEnMano == null && item.CompareTag("Interactuable") && Input.GetKeyDown(KeyCode.F))
        {
            var netObj = item.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                ulong networkObjectId = netObj.NetworkObjectId;
                TomarObjetoServerRpc(networkObjectId);
            }
        }
    }

    void AccionesObjeto()
    {
        if (!objetoEnMano) return;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            IInteractuable interactuable = objetoEnMano.GetComponent<IInteractuable>();
            interactuable?.Usar();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SoltarObjetoServerRpc();
        }
    }

    [ServerRpc]
    void TomarObjetoServerRpc(ulong networkObjectId, ServerRpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(rpcParams.Receive.SenderClientId, out var client))
            return;

        GameObject jugador = client.PlayerObject.gameObject;
        PlayerPickupMultiplayer script = jugador.GetComponent<PlayerPickupMultiplayer>();

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var objNet))
            return;

        GameObject objeto = objNet.gameObject;

        // Reparent el objeto a la mano directamente
        objeto.transform.SetParent(script.mano);
        objeto.transform.localPosition = Vector3.zero;
        objeto.transform.localRotation = Quaternion.identity;

        if (objeto.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        SetObjetoClientRpc(networkObjectId, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    void SetObjetoClientRpc(ulong networkObjectId, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var objNet))
        {
            objetoEnMano = objNet.gameObject;
        }
    }

    [ServerRpc]
    void SoltarObjetoServerRpc(ServerRpcParams rpcParams = default)
    {
        if (objetoEnMano == null) return;

        var netObj = objetoEnMano.GetComponent<NetworkObject>();
        ulong netId = netObj.NetworkObjectId;

        // Remover el objeto de la mano
        objetoEnMano.transform.SetParent(null);

        if (objetoEnMano.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
        }

        objetoEnMano = null;
        SoltarObjetoClientRpc(netId);
    }

    [ClientRpc]
    void SoltarObjetoClientRpc(ulong networkObjectId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var objNet))
            return;

        GameObject objeto = objNet.gameObject;
        objeto.transform.SetParent(null);

        if (objeto.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        if (objetoEnMano == objeto)
        {
            objetoEnMano = null;
        }
    }
}
