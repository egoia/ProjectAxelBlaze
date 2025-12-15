using NUnit.Framework;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed; 
    public float steering_speed;
    public bool isIA = true;

    Rigidbody rigidbody;
    InputSystem_Actions inputActions;
    NeuralNetwork brain;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isIA)
        {
            brain = new NeuralNetwork(3,5,4);
        }
        else
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.Enable();
        }
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float turnInput;
        float moveInput;
        if(isIA)
        {
            float[] input = {transform.position.x, transform.position.y, transform.position.z};
            float[] output = brain.Propagation(input);
            turnInput = output[2] - output[3];
            moveInput = output[0] - output[1];
        }
        else
        {
            turnInput =  inputActions.Player.Turn.ReadValue<float>();
            moveInput = inputActions.Player.Move.ReadValue<float>();
        } 
        Quaternion rotation = Quaternion.Euler(0, turnInput*steering_speed*Time.fixedDeltaTime,0);
        rigidbody.MoveRotation(transform.rotation * rotation);

        Vector3 movement = transform.forward * speed * moveInput;
        rigidbody.linearVelocity = new Vector3(movement.x, rigidbody.linearVelocity.y, movement.z);
    }
}
