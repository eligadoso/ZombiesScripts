using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class MovementController : MovementBasicMultiplayer
{
    public NetworkVariable<Vector3> networkedPosition = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    protected override void Start()
    {
        base.Start();

        if (camara == null)
            camara = GetComponentInChildren<Camera>()?.transform;

        if (IsOwner)
        {
            if (camara != null)
            {
                camara.gameObject.SetActive(true);
                var listener = camara.GetComponent<AudioListener>();
                if (listener) listener.enabled = true;
            }
            DesactivarMouse(OcultarMouse);
        }
        else
        {
            // Desactivar cámara y físicas para los que no son dueños
            if (camara != null)
            {
                camara.gameObject.SetActive(false);
                var listener = camara.GetComponent<AudioListener>();
                if (listener) listener.enabled = false;
            }

            if (rb != null)
                rb.isKinematic = true;
        }

        Debug.Log($"[Player] Spawn completado. Owner: {OwnerClientId}, Posición: {transform.position}");
    }

    protected override void Update()
    {
        if (!IsOwner)
        {
            transform.position = Vector3.Lerp(transform.position, networkedPosition.Value, Time.deltaTime * 10f);
            return;
        }

        base.Update(); // ejecuta lógica base (movimiento, salto, etc.)
        networkedPosition.Value = transform.position;
    }

    protected override bool Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            SaltarServerRpc();
        }
        return onGround;
    }

    [ServerRpc]
    private void SaltarServerRpc()
    {
        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
    }

}
