using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotorcycleBall : MonoBehaviour
{
    float moveInput, steerInput;
    public float maxSpeed, acceleration, steerStrength;
    public Rigidbody sphereRB;
    public PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        moveInput = playerInput.actions["Accelerate"].ReadValue<float>();
        if(playerInput.actions["Steering"].ReadValue<Vector2>().x>0){
          steerInput = -1;
          
        } else{
          steerInput = 1;
        }
    }
    private void FixedUpdate(){
        Movement();
        transform.position = sphereRB.transform.position;
    }

    void Movement(){
        sphereRB.velocity = Vector3.Lerp(sphereRB.velocity, maxSpeed * moveInput * transform.forward, Time.fixedDeltaTime*acceleration);
    }
    void Rotation(){
        transform.Rotate(0, steerInput *moveInput *steerStrength *Time.fixedDeltaTime, 0, Space.World);
    }
}
