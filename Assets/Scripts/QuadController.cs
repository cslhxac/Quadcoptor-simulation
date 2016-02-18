using UnityEngine;
using System.Collections;

public class QuadController : MonoBehaviour {
	private float mass;
	private Rigidbody rigidbody;
	public float height_command;
	public float velocity_command = 0;
	public float height_gain_p;
	public float height_gain_i;
	public float height_gain_d;
	private GameObject[] blades;
	private BladeControl[] blade_controls;
	private Rigidbody[] blade_rigidbodies;
	private float blade_force_gain;
	private int n_blades;
	private float err_position_acc;
	// Use this for initialization
	void Start () {
		mass = 0;	
		foreach (Transform child in transform) {
			if (child.name == "Frame") {
				rigidbody = child.GetComponent<Rigidbody> ();
				mass += rigidbody.mass;
				blade_force_gain = child.GetComponent<QuadSimulation>().blade_force_gain;
			}
			if (child.name == "Blades") {
				n_blades = child.childCount;
				blades = new GameObject[child.childCount];
				blade_controls = new BladeControl[child.childCount];
				blade_rigidbodies = new Rigidbody[child.childCount];
				for (int i = 0; i < child.childCount; ++i) {
					blades [i] = child.GetChild (i).gameObject;
					blade_controls [i] = blades [i].GetComponent<BladeControl> ();
					blade_rigidbodies [i] = blades [i].GetComponent<Rigidbody> ();
					mass += blade_rigidbodies [i].mass;
				}
			}
		}
		height_gain_i *= Time.fixedDeltaTime;
		err_position_acc = 0;
	}
	// Update is called once per frame
	void Update () {
		float throttle = Input.GetAxis ("Vertical");
		foreach(BladeControl control in blade_controls)
		{
			//control.velocity_star = throttle*1000;
		}
		//Debug.Log (blade_rigidbodies[0].angularVelocity);
	}
	// Update is called once per frame
	void FixedUpdate() {		
		float err_position = height_command - rigidbody.position.y;
		err_position_acc += err_position;
		float err_velocity = velocity_command - rigidbody.velocity.y;
		float force_star = err_position * height_gain_p + err_velocity * height_gain_d + err_position_acc * height_gain_i;
		for(int i = 0; i < n_blades; ++i) {
			blade_controls[i].velocity_star = Mathf.Sign(force_star) * Mathf.Sqrt(Mathf.Abs(force_star) / n_blades / blade_force_gain);
		}
	}
}
