using UnityEngine;
using System.Collections;

public class QuadSimulation : MonoBehaviour {
	private float mass;
	private Rigidbody rigidbody;
	private float throttle;
    private GameObject[] blades;
    private BladeControl[] blade_controls;
    private Rigidbody[] blade_rigidbodies;
    private int blades_count;    
	public float blade_p;
	public float blade_d;
	public float blade_force_gain;
	//private Vector3[] blade_angular_momentum;
	Vector3 ComputeMomentOfInertia(Quaternion inertiaTensorRotation,Vector3 inertiaTensorDiagonal,Vector3 angularVelocity){
		Vector3 tmp = Quaternion.Inverse(inertiaTensorRotation) * angularVelocity;
		for (int i = 0; i < 3; ++i) {
			tmp [i] *= inertiaTensorDiagonal [i] * 10;
		}
		tmp = inertiaTensorRotation * tmp;
		return tmp;
	}
	Vector3 ComputeAngularVelocity(Quaternion inertiaTensorRotation,Vector3 inertiaTensorDiagonal,Vector3 angularMomentum){
		Vector3 tmp = Quaternion.Inverse(inertiaTensorRotation) * angularMomentum;
		for (int i = 0; i < 3; ++i) {
			tmp [i] /= inertiaTensorDiagonal [i];
		}
		tmp = inertiaTensorRotation * tmp;
		return tmp;
	}
    // Use this for initialization
    void Start()
    {
		blade_force_gain = (float)((double)blade_p * blade_d * blade_d * blade_d / Mathf.Pow(10.0F,10.0F) * 0.278013851766 / 0.0166666666666667 / 0.0166666666666667);
		rigidbody = GetComponent<Rigidbody>();
        mass = rigidbody.mass;
		foreach (Transform child in transform.parent)
            if (child.name == "Blades")
            {
                blades_count = child.childCount;
                blades = new GameObject[child.childCount];
                blade_controls = new BladeControl[child.childCount];
                blade_rigidbodies = new Rigidbody[child.childCount];
				//blade_angular_momentum = new Vector3[child.childCount];
                for (int i = 0; i < child.childCount; ++i)
                {
                    blades[i] = child.GetChild(i).gameObject;
                    blade_controls[i] = blades[i].GetComponent<BladeControl>();
                    blade_rigidbodies[i] = blades[i].GetComponent<Rigidbody>();
					blade_rigidbodies [i].ResetInertiaTensor();
					//blade_angular_momentum[i] = ComputeMomentOfInertia(blade_rigidbodies[i].inertiaTensorRotation,blade_rigidbodies[i].inertiaTensor,blade_rigidbodies[i].angularVelocity);					
					//Debug.Log(blade_rigidbodies[i].inertiaTensor.ToString("F6"));
				}
            }
		//Debug.Log(rigidbody.inertiaTensorRotation);
		//Debug.Log(rigidbody.inertiaTensor.ToString("F6"));
		rigidbody.maxAngularVelocity = 100;
    }

	void FixedUpdate(){
		Vector3 blade_angular_momentum_accum = new Vector3(0,0,0);
		//Vector3 blade_force = /*rigidbody.rotation * */blade_rigidbodies[0].angularVelocity * (blade_rigidbodies[0].angularVelocity.magnitude * blade_force_gain * blade_controls [0].direction);

        for (int i = 0;i < blade_rigidbodies.GetLength(0);++i)
        {
			Vector3 blade_force = /*rigidbody.rotation */ blade_rigidbodies[i].angularVelocity * (blade_rigidbodies[i].angularVelocity.magnitude * blade_force_gain * blade_controls [i].direction);
			rigidbody.AddForceAtPosition(blade_force,blade_rigidbodies[i].position);
			//Conservation of angular momentum
			blade_angular_momentum_accum += blade_controls[i].angular_momentum_change;
        }
		//rigidbody.angularVelocity = Vector3.up;
		//rigidbody.angularVelocity += ComputeAngularVelocity(rigidbody.inertiaTensorRotation,rigidbody.inertiaTensor,blade_angular_momentum_accum);
		//Debug.Log(blade_angular_momentum_accum);
		//Debug.Log(ComputeAngularVelocity(rigidbody.inertiaTensorRotation,rigidbody.inertiaTensor,blade_angular_momentum_accum));
	}
}
