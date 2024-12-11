using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTrackManager : MonoBehaviour
{
    public GameObject AllNodes;
    public GameManager gm;
    public List<GameObject> nodes;
    // Start is called before the first frame update
    void Start(){
        
        foreach (Transform n in AllNodes.GetComponentsInChildren<Transform>()){
            if(n.gameObject != AllNodes.gameObject){
                nodes.Add(n.gameObject);
            }
            
        }
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Player"){
            //Debug.Log("Finishline");
            if(col.gameObject.GetComponent<PlayerTrackManager>().collected.Count == nodes.Count){
                Debug.Log("New Lap or Finish");
                col.gameObject.GetComponent<PlayerTrackManager>().collected.Clear();
                col.gameObject.GetComponent<VehicleControllerRework>().lap++;
                if(col.GetComponent<VehicleControllerRework>().lap == 2){
                    Time.timeScale = 0;
                    gm.enableWin(col.gameObject.GetComponent<VehicleControllerRework>().PlayerNum.ToString());
                }
                
            };
            
        }

    }
}
