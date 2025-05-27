using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Globalization;
[RequireComponent(typeof(NetworkObject))]
public class MovementBasicMultiplayer : NetworkBehaviour
{
    // Variables públicas (configurables en el Inspector)
    public float fuerzaSalto, sensibilidadMouse;
    public bool OcultarMouse;
    public Transform camara;
    public Rigidbody rb;
    public float speed;

    public float velocidad
    {
        get { return corriendo ? _velocidadBase * 2f : _velocidadBase; }
        set { _velocidadBase = value; }
    }

    // Variables privadas
    private static float rotacionX, _velocidadBase;
    protected virtual bool onGround => Physics.Raycast(transform.position, Vector3.down, 1f);
    private bool corriendo;
    // Métodos
    protected virtual void Start()
    {
        velocidad = speed;
        rotacionX = 0f;

        if (!IsOwner) return;

        DesactivarMouse(OcultarMouse);
    }

    protected virtual void Update()
    {
        if (!IsOwner) return;

        MovimientoPulsaciones();
        DetecionMouseMovimiento();
        Saltar();
        Correr();
    }

    protected virtual void MovimientoPulsaciones()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direccion = transform.right * horizontal + transform.forward * vertical;
        Vector3 move = direccion * velocidad * Time.deltaTime;
        rb.MovePosition(rb.position + move);
    }

    protected virtual void DetecionMouseMovimiento()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        transform.Rotate(Vector3.up * mouseX);
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);
        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
    }

    protected virtual void DesactivarMouse(bool estado)
    {
        if (!estado) return;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected virtual bool Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
        return onGround;
    }

    protected virtual void Correr()
    {
        corriendo = Input.GetKey(KeyCode.LeftShift) && onGround;
    }

}
