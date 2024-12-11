using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Unity.VisualScripting;

public class TrafficVehicleController : MonoBehaviour
{
    //Based on Paridot sppline navigation
    [SerializeField] private SplineContainer road;

    private Spline currentSpline;

    private Rigidbody rb;
    public float speed;
    public bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        currentSpline = road.Splines[0];
    }

    public void SwitchRoute(Spline spl)
    {
        currentSpline = spl;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!hit){
            //the spline were working with
            var native = new NativeSpline(currentSpline);
            //returns nearest (point closest to the spline) and t (tangent of the line)
            float distance = SplineUtility.GetNearestPoint(native, transform.position, out float3 nearest,out float t);

            transform.position = nearest;
            
            Vector3 forward = Vector3.Normalize(native.EvaluateTangent(t));
            Vector3 up = native.EvaluateUpVector(t);

            //transform.position = Vector3.Lerp(transform.position, forward, Time.deltaTime*speed);
            
            var remappedForward = new Vector3(0,0,1);
            var remappedUp = new Vector3(0,1,0);
            var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));
            
            transform.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;

            Vector3 engineForward = transform.forward;

            if (Vector3.Dot(rb.velocity,transform.forward) < 0)
            {
                //engineForward *= -1;
            }

            //rb.velocity = rb.velocity.magnitude * engineForward;
            rb.velocity = 50 * engineForward;
        }
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Player" ){
            hit = true;
            Debug.Log("Player Hit Vehicle");
        }
        if(col.gameObject.tag == "Traffic" ){
            hit = true;
            Debug.Log("Traffic Hit Vehicle");
        }
    }
        
}
