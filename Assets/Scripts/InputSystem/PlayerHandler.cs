using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerInput playerInput;
    private PrometeoCarController2 controller;
    int index;
    private InputActionReference Accelerate, Steering, Brake, Slide;
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        index = playerInput.playerIndex;
        Accelerate = InputActionReference.Create(playerInput.actions["Accelerate"]);
      Steering = InputActionReference.Create(playerInput.actions["Steering"]);
      Brake = InputActionReference.Create(playerInput.actions["Brake"]);
      Slide = InputActionReference.Create(playerInput.actions["Slide"]);
    }



    private void OnEnable(){
        Accelerate.action.performed += AccelerateVehicle;
        Accelerate.action.canceled += AccelerateVehicleStop;
        Brake.action.performed += BrakeVehicle;
        Brake.action.canceled += BrakeVehicleStop;
        
    }
    private void OnDisable(){
        Accelerate.action.performed -= AccelerateVehicle;
        Accelerate.action.canceled -= AccelerateVehicleStop;
        Brake.action.performed -= BrakeVehicle;
        Brake.action.canceled -= BrakeVehicleStop;
        
    }

    public void AccelerateVehicle(InputAction.CallbackContext obj){
      Debug.Log(index+": Gas");

    }
    private void AccelerateVehicleStop(InputAction.CallbackContext obj){
      Debug.Log(index + ": GasOFF");

    }

    private void BrakeVehicle(InputAction.CallbackContext obj){
      Debug.Log(index+ ": Handbrake");

    }
    private void BrakeVehicleStop(InputAction.CallbackContext obj){
      Debug.Log(index+": HandbrakeOFF");

    }

   
}
