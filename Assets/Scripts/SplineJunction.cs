using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class SplineJunction : MonoBehaviour
{
    [SerializeField]
    SplineContainer Route;

    [SerializeField]
    List<int> node;

    [SerializeField]
    int selection;

    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Traffic"){
            //Debug.Log("Junction Hit");
            col.gameObject.GetComponent<TrafficVehicleController>().SwitchRoute(Route[node[selection]]);
            
        }

    }
}
