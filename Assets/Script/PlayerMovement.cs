using NUnit.Framework;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed; 
    public float steering_speed;
    public bool isIA = true;
    [HideInInspector] public bool isPlaying = true;

    Rigidbody rigidbody;
    InputSystem_Actions inputActions;
    public NeuralNetwork brain;
    public float[] neuralInput;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isIA)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.Enable();
        }
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float turnInput = 0;
        float moveInput = 0;
        if (isPlaying)
        {
            if(isIA)
            {
                float[] output = brain.Propagation(neuralInput);
                turnInput = output[1] *2 - 1;
                moveInput = output[0] *2 - 1;
            }
            else
            {
                turnInput =  inputActions.Player.Turn.ReadValue<float>();
                moveInput = inputActions.Player.Move.ReadValue<float>();
            } 
        }
        Quaternion rotation = Quaternion.Euler(0, turnInput*steering_speed*Time.fixedDeltaTime,0);
        rigidbody.MoveRotation(transform.rotation * rotation);

        Vector3 movement = transform.forward * speed * moveInput;
        rigidbody.linearVelocity = new Vector3(movement.x, rigidbody.linearVelocity.y, movement.z);
    }
}
