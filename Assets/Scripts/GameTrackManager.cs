using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTrackManager : MonoBehaviour
{
    public GameObject AllNodes;
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
            Debug.Log("Finishline");
            if(col.gameObject.GetComponent<PlayerTrackManager>().collected.Count == nodes.Count){
                Debug.Log("New Lap or Finish");
            };
            
        }

    }
}
