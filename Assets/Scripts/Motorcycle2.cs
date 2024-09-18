using UnityEngine;
using UnityEngine.InputSystem;

public class MotoController : MonoBehaviour {

	public WheelCollider WColForward;
	public WheelCollider WColBack;

	public Transform wheelF;
	public Transform wheelB;

	public Transform CenterOfMass;

	public float maxSteerAngle = 45;
	public float maxMotorTorque = 200;
	public float maxForwardBrake = 400;
	public float maxBackBrake = 400;

	public float wheelRadius = 0.3f;
	public float wheelOffset = 0.1f;

	public float steerSensitivity = 30;
	public float controlAngle = 25;
	public float controlOmega = 30;

	public float lowSpeed = 8;
	public float highSpeed = 25;

	private WheelData[] wheels;

	private Transform thisTransform;

	private PlayerInput playerInput;

	public class WheelData {

		public WheelData(Transform transform, WheelCollider collider) {
			wheelTransform = transform;
			wheelCollider = collider;
			wheelStartPos = transform.transform.localPosition;
		}

		public Transform wheelTransform;
		public WheelCollider wheelCollider;
		public Vector3 wheelStartPos;
		public float rotation = 0f;
	}

	public struct MotoInput {
		public float steer;
		public float acceleration;
		public float brakeForward;
		public float brakeBack;
	}

	void Start() {
		playerInput = GetComponent<PlayerInput>();
		GetComponent<Rigidbody>().centerOfMass = CenterOfMass.localPosition;

		wheels = new WheelData[2];
		wheels[0] = new WheelData(wheelF, WColForward);
		wheels[1] = new WheelData(wheelB, WColBack);

		thisTransform = GetComponent<Transform>();
	}

	void FixedUpdate() {
		var input = new MotoInput();

		if(playerInput.actions["Accelerate"].ReadValue<float>() > 0){
			Debug.Log("PAGas");
			input.acceleration = 1;
		}

		if(playerInput.actions["Steering"].ReadValue<Vector2>().x>0){
          input.steer -= 1;
          
        } else{
          input.steer += 1;
        }

		if(playerInput.actions["Brake"].ReadValue<float>() > 0){
        
          Debug.Log("Handbrake");
		  input.brakeBack = 0.3f;
		  input.brakeForward = 0.8f;
          
      }
		/*if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input.acceleration = 1;
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input.steer += 1;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input.steer -= 1;

		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
			input.brakeBack = 0.3f;
			input.brakeForward = 0.8f;
		}
		if (Input.GetKey(KeyCode.Space)) {
			input.brakeBack = 1f;
		}

		motoMove(motoControl(input));
		updateWheels();

		if (Input.GetKeyDown(KeyCode.Backspace)) {
			pickUp();
		}*/

		motoMove(motoControl(input));
		updateWheels();

		if (speedVal < 0.01f && thisTransform.up.y < 0.2f && Mathf.Abs(prevOmega) < 0.001f) {
			pickUp();
		}
	}

	private void pickUp() {
		Transform t = GetComponent<Transform>();
		t.position = t.position + new Vector3(0, 0.2f, 0);
		t.rotation = new Quaternion(0, 0, 0, 1);
	}

	private Vector3 prevPos = new Vector3();
	private float prevAngle = 0;
	private float prevOmega = 0;
	private float speedVal = 0;
	private float prevSteer = 0f;

	private MotoInput motoControl(MotoInput input) {
		var posNow = thisTransform.position;
		var speed = (posNow - prevPos) / Time.fixedDeltaTime;
		prevPos = posNow;

		speedVal = speed.magnitude;
		var moveForward = speed.normalized;

		var angle = Vector3.Dot(moveForward, Vector3.Cross(thisTransform.up, new Vector3(0, 1, 0)));
		var omega = (angle - prevAngle) / Time.fixedDeltaTime;
		prevAngle = angle;
		prevOmega = omega;
		

		float lowSpeed = 8f;
		float highSpeed = 25f;
		

		if (speedVal < lowSpeed) {
			float t = speedVal / lowSpeed;
			input.steer *= t*t;
			omega *= t*t;
			angle = angle * (2 - t);
			input.acceleration += Mathf.Abs(angle) * 3 * (1 - t);
		}

		if (speedVal > highSpeed) {
			float t = speedVal / highSpeed;
			if (omega * angle < 0f) {
				omega *= t;
				
			}
		}

		input.steer *= (1 - 2.5f * angle * angle);

		// Adjust the control factors to smooth out the steering input
		float smoothSteer = input.steer * steerSensitivity + angle * controlAngle + omega * controlOmega;

		// Apply a smooth factor to the steering input to avoid sudden changes
		float smoothFactor = 0.1f; // Adjust as needed
		input.steer = Mathf.Lerp(prevSteer, smoothSteer, smoothFactor);

		// Clamp the steering input within a reasonable range
		float steerDelta = 10 * Time.fixedDeltaTime;
		input.steer = Mathf.Clamp(input.steer, prevSteer - steerDelta, prevSteer + steerDelta);
		prevSteer = input.steer;
		return input;
	}

	private void motoMove(MotoInput input) {
		WColForward.steerAngle = Mathf.Clamp(input.steer, -1, 1) * maxSteerAngle;

		WColForward.brakeTorque = maxForwardBrake * input.brakeForward;
		WColBack.brakeTorque = maxBackBrake * input.brakeBack;

		WColBack.motorTorque = maxMotorTorque * input.acceleration;
	}

	private void updateWheels() {
		float delta = Time.fixedDeltaTime;

		foreach (WheelData w in wheels) {
			WheelHit hit;

			Vector3 localPos = w.wheelTransform.localPosition;
			if (w.wheelCollider.GetGroundHit(out hit)) {
				localPos.y -= Vector3.Dot(w.wheelTransform.position - hit.point, transform.up) - wheelRadius;
				w.wheelTransform.localPosition = localPos;
			} else {
				localPos.y = w.wheelStartPos.y - wheelOffset;
			}
			// w.wheelTransform.localPosition = localPos;

			w.rotation = Mathf.Repeat(w.rotation + delta * w.wheelCollider.rpm * 360.0f / 60.0f, 360f);
			w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.wheelCollider.steerAngle, 0.0f);
			
		}
	}

	void OnGUI() {
		GUI.color = Color.black;
		var area = new Rect(0, 0, 100, 50);
		GUI.Label(area, speedVal.ToString("f1") + " m/s" + "\nangle = " + prevAngle.ToString("f3") + "\nangle' = " + prevOmega.ToString("f3"));
	}
}