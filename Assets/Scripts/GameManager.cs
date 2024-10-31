using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private int PlayerCount, P1DeviceID, P2DeviceID;
    [SerializeField] GameObject playerPref;
    GameObject p1, p2;
    // Start is called before the first frame update
    void Awake(){
        PlayerCount = PlayerPrefs.GetInt("PlayerCount");
        P1DeviceID = PlayerPrefs.GetInt("P1DeviceID");
        p1 = Instantiate(playerPref, new Vector3(63f,.66f,205f), Quaternion.identity);
        //Debug.Log("P1: "+P1DeviceID+" || P2: "+P2DeviceID);
        p1.GetComponent<PlayerInput>().SwitchCurrentControlScheme(InputSystem.GetDeviceById(P1DeviceID));
        p1.GetComponent<VehicleControllerRework>().PlayerNum = 1;
        
        if(PlayerCount == 2){
            P2DeviceID = PlayerPrefs.GetInt("P2DeviceID");
            p2 = Instantiate(playerPref, new Vector3(71f,.66f,205f), Quaternion.identity);
            p2.GetComponent<PlayerInput>().SwitchCurrentControlScheme(InputSystem.GetDeviceById(P2DeviceID));
            p2.GetComponent<VehicleControllerRework>().PlayerNum = 2;
            p1.GetComponentInChildren<Camera>().rect = new Rect(0f, 0.5f,1f,0.5f);
            p2.GetComponentInChildren<Camera>().rect = new Rect(0f, 0f,1f,0.5f);
        }

    }

    
}
