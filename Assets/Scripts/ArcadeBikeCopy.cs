using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArcadeBP2
{
    public class ArcadeBikeCopy : MonoBehaviour
    {
        public enum groundCheck { rayCast, sphereCaste };
        public enum MovementMode { Velocity, AngularVelocity };
        public MovementMode movementMode;
        public groundCheck GroundCheck;
        public LayerMask drivableSurface;

        public float MaxSpeed, accelaration, brake, turn, driftLimit, slide;

        public bool drifting, sliding;
        public Vector3 slidePos, slideRot;
        public Rigidbody rb, bikeBody;



        
        public RaycastHit hit;
        public AnimationCurve frictionCurve;
        public AnimationCurve driftFrictionCurve;
        public AnimationCurve turnCurve;
        public AnimationCurve leanCurve;
        public PhysicMaterial frictionMaterial;
        [Header("Visuals")]
        public Transform BodyMesh;
        public Transform Handle;
        public Transform[] Wheels = new Transform[2];
        
        public Vector3 bikeVelocity;

        [Range(-70, 70)]
        public float BodyTilt;
        [Header("Audio settings")]
        public AudioSource engineSound;
        [Range(0, 1)]
        public float minPitch;
        [Range(1, 5)]
        public float MaxPitch;
        public AudioSource SkidSound;

        public float skidWidth;


        private float radius, horizontalInput, accelerateInput, brakeInput;
        private Vector3 origin;

        public PlayerInput playerInput;

        private void Start()
        {
            radius = rb.GetComponent<SphereCollider>().radius;
            if (movementMode == MovementMode.AngularVelocity)
            {
                Physics.defaultMaxAngularSpeed = 150;
            }
            rb.centerOfMass = Vector3.zero;
        }
        private void Update()
        {
            //horizontalInput = Input.GetAxis("Horizontal"); //turning input
           // verticalInput = Input.GetAxis("Vertical");     //accelaration input
            horizontalInput = playerInput.actions["Steering"].ReadValue<Vector2>().x;
            accelerateInput = playerInput.actions["Accelerate"].ReadValue<float>();
            brakeInput = playerInput.actions["Brake"].ReadValue<float>();
            //Debug.Log("V: "+ accelerateInput);
            Visuals();
            AudioManager();

        }
        public void AudioManager()
        {
            engineSound.pitch = Mathf.Lerp(minPitch, MaxPitch, Mathf.Abs(bikeVelocity.z) / MaxSpeed);
            if (Mathf.Abs(bikeVelocity.x) > 10 && grounded())
            {
                SkidSound.mute = false;
            }
            else
            {
                SkidSound.mute = true;
            }
        }


        void FixedUpdate()
        {
           
            bikeVelocity = bikeBody.transform.InverseTransformDirection(bikeBody.velocity);
            
            if(Input.GetKey(KeyCode.I)){
                sliding = true;
            }else{
                sliding = false;
            }

            DriftCheck();

            if (Mathf.Abs(bikeVelocity.x) > 0)
            {  
                
                //changes friction according to sideways speed of bike
                if(drifting){
                    //print("drifting");
                    frictionMaterial.dynamicFriction = driftFrictionCurve.Evaluate(Mathf.Abs(bikeVelocity.x / 100));
                } else{
                    frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(bikeVelocity.x / 100));
                    //print("not drifting");
                }
                
            }


            if (grounded())
            {
                //turnlogic
                float sign = Mathf.Sign(bikeVelocity.z);
                float TurnMultiplyer = turnCurve.Evaluate(bikeVelocity.magnitude / MaxSpeed);
                if(sliding){
                    print("sliding");
                    rb.velocity = Vector3.Lerp(rb.velocity, bikeBody.transform.forward * 0, slide / 10 * Time.deltaTime);
                }else{
                    if (accelerateInput > 0.1f || bikeVelocity.z > 1)
                    //if ( bikeVelocity.z > 1)
                    {
                        bikeBody.AddTorque(Vector3.up * horizontalInput * sign * turn * 10 * TurnMultiplyer);
                    }
                    //need to figure out if drifting is mixed with breaking is mixed with reverse
                    //else if (brakeInput > 0.1f || bikeVelocity.z < -1)
                    else if ( bikeVelocity.z < -1)
                    {
                        bikeBody.AddTorque(Vector3.up * horizontalInput * sign * turn * 10 * TurnMultiplyer);
                    }

                    //brakelogic
                    //if (Input.GetAxis("Jump") > 0.1f)
                    if (!grounded())
                    {
                        rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    }
                    else
                    {
                        rb.constraints = RigidbodyConstraints.None;
                    }

                    

                    //accelaration logic

                    if (movementMode == MovementMode.AngularVelocity)
                    {
                        if (Mathf.Abs(accelerateInput) > 0.1f)
                        {
                            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, bikeBody.transform.right * accelerateInput * MaxSpeed / radius, accelaration * Time.deltaTime);
                        } else if(Mathf.Abs(brakeInput) > 0.1f){
                            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, bikeBody.transform.right * brakeInput * MaxSpeed / radius, brake * Time.deltaTime);
                        }
                    }
                    else if (movementMode == MovementMode.Velocity)
                    {
                        //if (Mathf.Abs(verticalInput) > 0.1f && Input.GetAxis("Jump") < 0.1f)
                        if (Mathf.Abs(accelerateInput) > 0.1f)
                        {
                            rb.velocity = Vector3.Lerp(rb.velocity, bikeBody.transform.forward * accelerateInput * MaxSpeed, accelaration / 10 * Time.deltaTime);
                        } else if (Mathf.Abs(brakeInput) > 0.1f){
                            rb.velocity = Vector3.Lerp(rb.velocity, bikeBody.transform.forward * 0, brake / 10 * Time.deltaTime);
                            //print("braking");
                        }
                    }

                    //body tilt
                    bikeBody.MoveRotation(Quaternion.Slerp(bikeBody.rotation, Quaternion.FromToRotation(bikeBody.transform.up, hit.normal) * bikeBody.transform.rotation, 0.09f));
                    }
                
            }
            else
            {
                bikeBody.MoveRotation(Quaternion.Slerp(bikeBody.rotation, Quaternion.FromToRotation(bikeBody.transform.up, Vector3.up) * bikeBody.transform.rotation, 0.02f));
            }

        }

        public void DriftCheck(){
            //print("Bike: "+bikeVelocity.x);
            
            //print("bike v: "+bikeVelocity.x);
            //if velocity is greater than XXX, turning, breaking, not already drifting
            if(bikeVelocity.z > 0.1f && Mathf.Abs(bikeVelocity.x) > 0.1f && brakeInput > 0.1f &&!drifting){
                drifting = true;
            } //else if(Math.Abs(bikeVelocity.x) <= 11f && drifting){
            else if(Math.Abs(bikeVelocity.x) <= driftLimit && drifting){
                
                drifting = false;
            }
        }
        public void Visuals()
        {
            Handle.localRotation = Quaternion.Slerp(Handle.localRotation, Quaternion.Euler(Handle.localRotation.eulerAngles.x,
                                   20 * horizontalInput, Handle.localRotation.eulerAngles.z), 15f * Time.deltaTime);

            Wheels[0].localRotation = rb.transform.localRotation;
            Wheels[1].localRotation = rb.transform.localRotation;

            //Body
            if(sliding){
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(slideRot.x, slideRot.y, slideRot.z), 6f * Time.deltaTime);
                BodyMesh.localPosition = Vector3.Lerp(BodyMesh.localPosition, new Vector3(slidePos.x, slidePos.y, slidePos.z), 6f * Time.deltaTime);
           
            }else{
                BodyMesh.localPosition = Vector3.Lerp(BodyMesh.localPosition, new Vector3(0, -0.379f, -0.81f), 6f * Time.deltaTime);
                if(BodyMesh.localRotation.y > 0f){
                    BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 4f * Time.deltaTime);
                }
                if (bikeVelocity.z > 1)
                {
                    BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0,
                                    BodyMesh.localRotation.eulerAngles.y, BodyTilt * horizontalInput * leanCurve.Evaluate(bikeVelocity.z / MaxSpeed)), 4f * Time.deltaTime);
                }
                else
                {
                    BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 4f * Time.deltaTime);
                }
            }
            



        }

        public bool grounded() //checks for if vehicle is grounded or not
        {
            origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
            var direction = -transform.up;
            var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

            if (GroundCheck == groundCheck.rayCast)
            {
                if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (GroundCheck == groundCheck.sphereCaste)
            {
                if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else { return false; }
        }

        private void OnDrawGizmos()
        {
            //debug gizmos
            radius = rb.GetComponent<SphereCollider>().radius;
            float width = 0.02f;
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(rb.transform.position + ((radius + width) * Vector3.down), new Vector3(2 * radius, 2 * width, 4 * radius));
                if (GetComponent<BoxCollider>())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
                }
                if (GetComponent<CapsuleCollider>())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(transform.position, GetComponent<CapsuleCollider>().bounds.size);
                }

            }

        }

    }
}
