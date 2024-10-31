using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackNode : MonoBehaviour
{
    [SerializeField]
    public int NodeID;
    // Start is called before the first frame update
    


    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Player"){
            Debug.Log("Player "+ col.gameObject.GetComponent<VehicleControllerRework>().PlayerNum + " node " + NodeID);
            col.gameObject.GetComponent<PlayerTrackManager>().collected.Add(this);
            
        }

    }
}
