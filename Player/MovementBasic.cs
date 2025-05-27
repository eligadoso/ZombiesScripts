using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovementBasic : MonoBehaviour
{
    //Variables Publicas (Modificables en Unity)
    public float fuerzaSalto, sensibilidadMouse;
    public bool OcultarMouse;
    public Transform camara;
    public Rigidbody rb;
    public float speed;
    public float velocidad
    {
        get
        {
            return corriendo ? _velocidadBase * 2f : _velocidadBase;
        }
        set
        {
            _velocidadBase = value;
        }
    }

    //Variables Privadas (Solo manejadas por la logica del codigo)
    private static float rotacionX, _velocidadBase;
    protected virtual bool onGround
    {
        //LayerMask sueloMask = LayerMask.GetMask("Suelo");
        //En caso de querer agregar un requisito como suelo añadir esta variable al final del
        //metodo RayCast 1.1f,sueloMask)
        get { return Physics.Raycast(transform.position, Vector3.down, 1f); }
    }
    private bool corriendo;


    //Metodos
    protected virtual void Start()
    {
        velocidad = speed;
        rotacionX = 0f;
        DesactivarMouse(OcultarMouse);
    }
    protected virtual void Update()
    {
        MovimientoPulsaciones();
        DetecionMouseMovimiento();
        Saltar();
        Correr();
    }
    protected virtual void MovimientoPulsaciones()
    {
        // Movimiento WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direccion = transform.right * horizontal + transform.forward * vertical;
        Vector3 move = direccion * velocidad * Time.deltaTime;
        //Movimientos con el RigidBody para mantener las fisicas correctamente
        rb.MovePosition(rb.position + move);
    }
    protected virtual void DetecionMouseMovimiento()
    {
        // Movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        // Rotación horizontal del jugador
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
        if(Input.GetKey(KeyCode.LeftShift) && onGround)
        {
            corriendo = true;
            return;
        }
        corriendo = false;
    }
}
