using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Velocidad de movimiento del personaje
    public float speed = 5.0f;
    public float runSpeedMultiplier = 3.0f; // Multiplicador de velocidad al correr
    private bool isRunning = false; // Indica si el personaje está corriendo
    private bool isFallen = false;
    public Rigidbody rb;

    private Animator animator; // Referencia al componente Animator
    private AudioSource audioSource; // Referencia al componente AudioSource

    private void Start()
    {
        Cursor.visible = false; // Para que no se vea el mouse
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Obtener el componente Animator
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    private void Update() 
    {
        // Capturamos la entrada de teclado
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Verificar si se está corriendo (SHIFT + W)
        if (Input.GetKey(KeyCode.LeftShift) && verticalInput > 0)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        // Actualizar los parámetros del Animator
        bool isWalking = verticalInput != 0 && !isRunning;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);

        // Debugging --> Ya funciona, tuve que destildar el boton Has Exit Time en las animaciones
        Debug.Log("isWalking: " + isWalking);
        Debug.Log("isRunning: " + isRunning);

        // Controlar el sonido de caminar
        if (isWalking && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!isWalking && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Calculamos la dirección de movimiento basada en la rotación de la cámara
        Vector3 moveDirection = (verticalInput * Camera.main.transform.forward + horizontalInput * Camera.main.transform.right).normalized;

        // Aplicar multiplicador de velocidad si está corriendo
        float currentSpeed = isRunning ? speed * runSpeedMultiplier : speed;

        // Movemos al personaje en la dirección calculada
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);

        // Levantamos al personaje caído y le devolvemos sus contraints
        if (Input.GetKeyDown(KeyCode.G) && isFallen)
        {
            transform.rotation = Quaternion.identity;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            isFallen = false;
        }
    }

    // Función para que el jugador pueda tropezar
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle1"))
        {
            rb.constraints = RigidbodyConstraints.None;
            isFallen = true;
        }
    }
}
