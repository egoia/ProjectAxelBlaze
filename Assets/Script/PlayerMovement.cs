using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed; 
    public float steering_speed;

    Rigidbody rigidbody;
    InputSystem_Actions inputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float turnInput =  inputActions.Player.Turn.ReadValue<float>();
        Quaternion rotation = Quaternion.Euler(0, turnInput*steering_speed*Time.fixedDeltaTime,0);
        rigidbody.MoveRotation(transform.rotation * rotation);
        //rigidbody.angularVelocity = new Vector3(0, turnInput*steering_speed,0);
        //transform.Rotate(new Vector3(0, turnInput*steering_speed,0));

        float moveInput = inputActions.Player.Move.ReadValue<float>();
        Vector3 movement = transform.forward * speed * moveInput;
        rigidbody.linearVelocity = new Vector3(movement.x, rigidbody.linearVelocity.y, movement.z);
    }
}
