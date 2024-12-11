using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private int PlayerCount, P1DeviceID, P2DeviceID;
    public float targetTime = 5;
    public float raceTime = 0;
    public bool DisableControls = true;
    public bool singlePlayer = true;
    [SerializeField] GameObject playerPref, mainMenuButton, p1Speed, p2Speed;
    [SerializeField] TMP_Text timer, winText, raceTimer;
    GameObject p1, p2;
    TimeSpan timeSpan;
    // Start is called before the first frame update
    void Awake(){
        p1Speed.SetActive(false);
        p2Speed.SetActive(false);
        winText.text = "";
        PlayerCount = PlayerPrefs.GetInt("PlayerCount");
        P1DeviceID = PlayerPrefs.GetInt("P1DeviceID");
        p1 = Instantiate(playerPref, new Vector3(63f,.66f,205f), Quaternion.identity);
        //Debug.Log("P1: "+P1DeviceID+" || P2: "+P2DeviceID);
        p1.GetComponent<PlayerInput>().SwitchCurrentControlScheme(InputSystem.GetDeviceById(P1DeviceID));
        p1.GetComponent<VehicleControllerRework>().PlayerNum = 1;
        p1.GetComponent<VehicleControllerRework>().hud = p1Speed;
        p1.GetComponent<VehicleControllerRework>().hud.transform.position= new Vector3(8, -1, 0);
        
        if(PlayerCount == 2){
            p1.GetComponent<VehicleControllerRework>().hud.transform.position= new Vector3(8, Screen.height/2+20, 0);
            singlePlayer = false;
            P2DeviceID = PlayerPrefs.GetInt("P2DeviceID");
            p2 = Instantiate(playerPref, new Vector3(71f,.66f,205f), Quaternion.identity);
            p2.GetComponent<PlayerInput>().SwitchCurrentControlScheme(InputSystem.GetDeviceById(P2DeviceID));
            p2.GetComponent<VehicleControllerRework>().PlayerNum = 2;
            p1.GetComponentInChildren<Camera>().rect = new Rect(0f, 0.5f,1f,0.5f);
            p2.GetComponentInChildren<Camera>().rect = new Rect(0f, 0f,1f,0.5f);
            p2.GetComponent<VehicleControllerRework>().hud = p2Speed;
        }

    }

    void Update(){
        if (DisableControls){
            targetTime -= Time.deltaTime;
            timer.text = ((int) targetTime+1).ToString();

            if (targetTime <= 0.0f)
            {
                timer.text = "";
                DisableControls = false;
                p1Speed.SetActive(true);
                if(PlayerCount == 2){
                    p2Speed.SetActive(true);
                }
            }
        }

        if(singlePlayer && !DisableControls){
            raceTime += Time.deltaTime;
            timeSpan = TimeSpan.FromSeconds(raceTime);
            string timeText = string.Format("{0:D2}:{1:D2}:{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            raceTimer.text = timeText.ToString();
        }


        

        

        
    }

    public void enableWin(string winnerID){
        
        if (singlePlayer){
            winText.text = "Final Time: "+timeSpan.ToString();
            if(raceTime < PlayerPrefs.GetFloat("Time") || PlayerPrefs.GetFloat("Time") == 0){
                PlayerPrefs.SetFloat("Time", raceTime);
                winText.text+= " - New Fastest Time";
            }
        }else{
            mainMenuButton.SetActive(true);
            winText.text = "Player "+ winnerID +" wins";
        }
        
        

    }

    void OnDestroy(){
        Time.timeScale = 1;
    }

    
}
