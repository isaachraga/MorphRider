using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackManager : MonoBehaviour
{
    public List<TrackNode> collected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddNode(TrackNode tn){
        if(!collected.Contains(tn)){
            collected.Add(tn);
        }
    }
}
