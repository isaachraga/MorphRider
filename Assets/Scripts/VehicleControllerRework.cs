using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleControllerRework : MonoBehaviour
{
    //Bike Vars
    public Transform[] BikeWheels = new Transform[2];
    public Transform BikeBodyMesh;
    
    public AnimationCurve bikeLeanCurve;
    public AnimationCurve bikeTurnCurve;
    public AnimationCurve bikeFrictionCurve;
    public AnimationCurve bikeDriftFrictionCurve;
    
    public float bMaxSpeed, bAccelaration, bBrake, bTurn, bDriftLimit, bSlide, bTilt, bSlideTranSpeed;
    
    
    public Vector3 slidePos, slideRot, velocity;
    

    //Car Vars
    public Transform CarBodyMesh;
    public Transform[] CarWheels = new Transform[4];
    
    public AnimationCurve carLeanCurve;
    public AnimationCurve carTurnCurve;
    public AnimationCurve carFrictionCurve;
    public AnimationCurve carDriftFrictionCurve;
    
    public float cMaxSpeed, cAccelaration, cBrake, cTurn, cDriftLimit, cTilt;

    //Using Vars
    public LayerMask drivableSurface;
    public RaycastHit hit;
    public Rigidbody rb, GOrb;
    public PlayerInput playerInput;
    public PhysicMaterial frictionMaterial;
    private float radius, horizontalInput, accelerateInput, brakeInput, slideInput, morphInput;
    private float maxSpeed, accelaration, brake, turn, driftLimit, slide, tilt, slideTranSpeed;
    public bool drifting, sliding, morphFlag, bike;
    private Vector3 origin;
    
    void Start()
    {
        radius = rb.GetComponent<SphereCollider>().radius;
        rb.centerOfMass = Vector3.zero;
        assignBike();
    }

    // Update is called once per frame
    void Update()
    {
        //Control mapping to variables
        horizontalInput = playerInput.actions["Steering"].ReadValue<Vector2>().x;
        accelerateInput = playerInput.actions["Accelerate"].ReadValue<float>();
        brakeInput = playerInput.actions["Brake"].ReadValue<float>();
        slideInput = playerInput.actions["Slide"].ReadValue<float>();
        morphInput = playerInput.actions["Morph"].ReadValue<float>();

        Visuals();
        AudioManager();

    }

    void FixedUpdate(){

        if(bike){
            velocity = GOrb.transform.InverseTransformDirection(GOrb.velocity);
        }
        
        InputControls();
    }

    void InputControls(){

        if(morphInput > 0.1f && !morphFlag){
                Morph();
        } else if(morphFlag && morphInput == 0){
            morphFlag = false;
        }

        DriftCheck();
       
        if(bike){
            if(slideInput > 0.1f){
                sliding = true;
            }else{
                sliding = false;
            }

            if (Mathf.Abs(velocity.x) > 0)
            {  
                
                //changes friction according to sideways speed of bike
                if(drifting){
                    //print("drifting");
                    frictionMaterial.dynamicFriction = bikeDriftFrictionCurve.Evaluate(Mathf.Abs(velocity.x / 100));
                } else{
                    frictionMaterial.dynamicFriction = bikeFrictionCurve.Evaluate(Mathf.Abs(velocity.x / 100));
                    //print("not drifting");
                }
                
            }


            if (GroundedCheck())
            {
                //turnlogic
                float sign = Mathf.Sign(velocity.z);
                float TurnMultiplyer = bikeTurnCurve.Evaluate(velocity.magnitude / maxSpeed);
                
                    if (accelerateInput > 0.1f || velocity.z > 1)
                    {
                        GOrb.AddTorque(Vector3.up * horizontalInput * sign * turn * 10 * TurnMultiplyer);
                    }
                    //need to figure out if drifting is mixed with breaking is mixed with reverse
                    //else if (brakeInput > 0.1f || bikeVelocity.z < -1)
                    else if ( velocity.z < -1)
                    {
                        GOrb.AddTorque(Vector3.up * horizontalInput * sign * turn * 10 * TurnMultiplyer);
                    }

                    //brakelogic
                    //if (Input.GetAxis("Jump") > 0.1f)
                    if (!GroundedCheck())
                    {
                        rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    }
                    else
                    {
                        rb.constraints = RigidbodyConstraints.None;
                    }

                    

                    //accelaration logic

                    
                    
                    if (Mathf.Abs(accelerateInput) > 0.1f)
                    {
                        rb.velocity = Vector3.Lerp(rb.velocity, GOrb.transform.forward * accelerateInput * maxSpeed, accelaration / 10 * Time.deltaTime);
                    } else if (Mathf.Abs(brakeInput) > 0.1f){
                        rb.velocity = Vector3.Lerp(rb.velocity, GOrb.transform.forward * 0, brake / 10 * Time.deltaTime);
                    }

                    //body tilt
                    GOrb.MoveRotation(Quaternion.Slerp(GOrb.rotation, Quaternion.FromToRotation(GOrb.transform.up, hit.normal) * GOrb.transform.rotation, 0.09f));
                
                
            }
            else
            {
                GOrb.MoveRotation(Quaternion.Slerp(GOrb.rotation, Quaternion.FromToRotation(GOrb.transform.up, Vector3.up) * GOrb.transform.rotation, 0.02f));
            }
        } else{
            
            //Car Inputs
            if (Mathf.Abs(velocity.x) > 0)
            {  
                
                //changes friction according to sideways speed of bike
                if(drifting){
                    //print("drifting");
                    frictionMaterial.dynamicFriction = carDriftFrictionCurve.Evaluate(Mathf.Abs(velocity.x / 100));
                } else{
                    frictionMaterial.dynamicFriction = carFrictionCurve.Evaluate(Mathf.Abs(velocity.x / 100));
                    //print("not drifting");
                }
                
            }


            if (GroundedCheck())
            {
                //turnlogic
                float sign = Mathf.Sign(velocity.z);
                float TurnMultiplyer = carTurnCurve.Evaluate(velocity.magnitude / maxSpeed);
                if(sliding){
                    print("sliding");
                    rb.velocity = Vector3.Lerp(rb.velocity, GOrb.transform.forward * 0, slide / 10 * Time.deltaTime);
                }else{
                    if (accelerateInput > 0.1f || velocity.z > 1)
                    {
                        GOrb.AddTorque(Vector3.up * horizontalInput * sign * turn * 10 * TurnMultiplyer);
                    }
                    //need to figure out if drifting is mixed with breaking is mixed with reverse
                    //else if (brakeInput > 0.1f || bikeVelocity.z < -1)
                    else if ( velocity.z < -1)
                    {
                        GOrb.AddTorque(Vector3.up * horizontalInput * sign * turn * 10 * TurnMultiplyer);
                    }

                    //brakelogic
                    //if (Input.GetAxis("Jump") > 0.1f)
                    if (!GroundedCheck())
                    {
                        rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    }
                    else
                    {
                        rb.constraints = RigidbodyConstraints.None;
                    }

                    

                    //accelaration logic

                    
                    
                    if (Mathf.Abs(accelerateInput) > 0.1f)
                    {
                        rb.velocity = Vector3.Lerp(rb.velocity, GOrb.transform.forward * accelerateInput * maxSpeed, accelaration / 10 * Time.deltaTime);
                    } else if (Mathf.Abs(brakeInput) > 0.1f){
                        rb.velocity = Vector3.Lerp(rb.velocity, GOrb.transform.forward * 0, brake / 10 * Time.deltaTime);
                    }

                    //body tilt
                    GOrb.MoveRotation(Quaternion.Slerp(GOrb.rotation, Quaternion.FromToRotation(GOrb.transform.up, hit.normal) * GOrb.transform.rotation, 0.09f));
                }
                
            }
            else
            {
                GOrb.MoveRotation(Quaternion.Slerp(GOrb.rotation, Quaternion.FromToRotation(GOrb.transform.up, Vector3.up) * GOrb.transform.rotation, 0.02f));
            }
        }
       

    }
    public void AudioManager()
    {
        //Need audio implementation
    }

    public void Visuals()
    {
        //Bike Visual Controller
        if(bike){
            //Match wheel to rigidbody rotation
            BikeWheels[0].localRotation = rb.transform.localRotation;
            BikeWheels[1].localRotation = rb.transform.localRotation;

            if(sliding){
                //Lerp to sliding position
                BikeBodyMesh.localRotation = Quaternion.Slerp(BikeBodyMesh.localRotation, Quaternion.Euler(slideRot.x, slideRot.y, slideRot.z), slideTranSpeed * Time.deltaTime);
                BikeBodyMesh.localPosition = Vector3.Lerp(BikeBodyMesh.localPosition, new Vector3(slidePos.x, slidePos.y, slidePos.z), slideTranSpeed * Time.deltaTime);
        
            }else{
                //lerp back from sliding if sliding
                BikeBodyMesh.localPosition = Vector3.Lerp(BikeBodyMesh.localPosition, new Vector3(0, -0.379f, -0.81f), slideTranSpeed * Time.deltaTime);
                if(BikeBodyMesh.localRotation.y > 0f){
                    BikeBodyMesh.localRotation = Quaternion.Slerp(BikeBodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 4f * Time.deltaTime);
                }

                if (velocity.z > 1)
                {
                    //handles leaning when going forward
                    BikeBodyMesh.localRotation = Quaternion.Slerp(BikeBodyMesh.localRotation, Quaternion.Euler(0,
                                    BikeBodyMesh.localRotation.eulerAngles.y, tilt * horizontalInput * bikeLeanCurve.Evaluate(velocity.z / maxSpeed)), 4f * Time.deltaTime);
                }
                else
                {
                    //handles leaning when going backwards
                    BikeBodyMesh.localRotation = Quaternion.Slerp(BikeBodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 4f * Time.deltaTime);
                }
            }
        } else{
            //Car Body Visual Controller

            //CarWheels[0].localRotation = rb.transform.localRotation;
            //CarWheels[1].localRotation = rb.transform.localRotation;

            
            //BikeBodyMesh.localPosition = Vector3.Lerp(BikeBodyMesh.localPosition, new Vector3(0, -0.379f, -0.81f), slideTranSpeed * Time.deltaTime);


            if (velocity.z > 1)
            {
                //handles leaning when going forward
                
            }
            else
            {
                //handles leaning when going backwards
            }
            
        }
            
            



    }

    void Morph(){

        morphFlag = true;
        
        if(bike){
            bike = false;
            assignCar();
            BikeBodyMesh.gameObject.SetActive(false);
            CarBodyMesh.gameObject.SetActive(true);
            //assign car vars to controlls
        } else{
            bike = true;
            assignBike();
            BikeBodyMesh.gameObject.SetActive(true);
            CarBodyMesh.gameObject.SetActive(false);
            //assign bike vars to controlls
        }
        
        
    }

    void DriftCheck(){
        //Handles drifting

        //Start drift until steering corrects to specified angle
        if(velocity.z > 0.1f && Mathf.Abs(velocity.x) > 0.1f && brakeInput > 0.1f &&!drifting){
            drifting = true;
            //frictionMaterial.dynamicFriction = driftFrictionCurve.Evaluate(Mathf.Abs(velocity.x / 100));
        }
        else if(Math.Abs(velocity.x) <= driftLimit && drifting){
            
            drifting = false;
            //frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(velocity.x / 100));
        }
    }

    public bool GroundedCheck() //checks for if vehicle is grounded or not
    {
        origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
        var direction = -transform.up;
        var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

        if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void SetVelocity(Vector3 velo){
            rb.velocity = velo;
    }

    void assignBike(){
        maxSpeed = bMaxSpeed;
        accelaration = bAccelaration;
        brake = bBrake;
        turn = bTurn;
        driftLimit = bDriftLimit;
        slide = bSlide;
        slideTranSpeed = bSlideTranSpeed;
        tilt = bTilt;
    }

    void assignCar(){
        maxSpeed = cMaxSpeed;
        accelaration = cAccelaration;
        brake = cBrake;
        turn = cTurn;
        driftLimit = cDriftLimit;
        tilt = cTilt;
    }
    
    
}
