using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsAssign : MonoBehaviour
{
    public GameObject playerSetup;
    int playerCount = 1;
    GameObject p1, p2;
    public TMP_Text Player1, Player2;
    float accelerateInput, brakeInput, start;
    // Start is called before the first frame update
    void Start()
    {
        p1 = Instantiate(playerSetup, this.transform.position, Quaternion.identity);
        
        Player1.text = "Player 1\nController: "+ p1.GetComponent<PlayerInput>().devices[0];
        Player2.text = "";
    }

    void Update(){
        Player1.text = "Player 1\nController: "+ p1.GetComponent<PlayerInput>().devices[0];
        if(p2 != null){
            Player2.text = "Player 1\nController: "+ p2.GetComponent<PlayerInput>().devices[0];
        }
        
        
        if(playerCount == 1 && InputSystem.devices.Count == 2){
            Player2.text = "Press 'R1' to assign Player 2 controller";
        } else if(InputSystem.devices.Count == 3 && playerCount == 1){
            Player2.text = "Press 'south Button' to join";
            if(p2 == null){
                //Debug.Log("MAde p2");
                p1.GetComponent<PlayerInput>().SwitchCurrentControlScheme(InputSystem.devices[1]);
                p2 = Instantiate(playerSetup, this.transform.position, Quaternion.identity);
                p2.GetComponent<PlayerInput>().SwitchCurrentControlScheme(InputSystem.devices[2]);
            }
        }
        if(p2 != null){
            accelerateInput = p2.GetComponent<PlayerInput>().actions["Accelerate"].ReadValue<float>();
        }
        brakeInput = p1.GetComponent<PlayerInput>().actions["Brake"].ReadValue<float>();
        start = p1.GetComponent<PlayerInput>().actions["Start"].ReadValue<float>();
        
        //Debug.Log(InputSystem.devices.Count);
        InputManager();
    }

    void InputManager(){
        if(brakeInput > 0.1f){
            if(playerCount == 1 && InputSystem.devices.Count == 2){
                
                p2 = Instantiate(playerSetup, this.transform.position, Quaternion.identity);
                Player2.text = "Player 2\nController: "+ p2.GetComponent<PlayerInput>().devices[0];
                ++playerCount;
            }
        }

        if(accelerateInput > 0.1f){
            if(playerCount == 1 && InputSystem.devices.Count == 3){
                 ++playerCount;
            }
        }

        if(start > 0.1f){
            StartGame();
        }
    }

    public void StartGame(){
        PlayerPrefs.SetInt("PlayerCount", playerCount);
        PlayerPrefs.SetInt("P1DeviceID",  p1.GetComponent<PlayerInput>().devices[0].deviceId);
        if(playerCount == 2){
            PlayerPrefs.SetInt("P2DeviceID",  p2.GetComponent<PlayerInput>().devices[0].deviceId);
        
        }
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
