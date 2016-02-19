using UnityEngine;
using System.Collections;

public class BladeControl : MonoBehaviour {
    private float mass;
    private Rigidbody rigidbody;
    public float velocity_star;
    public float direction;
	public Vector3 angular_momentum_change;
	Vector3 ComputeMomentOfInertia(Quaternion inertiaTensorRotation,Vector3 inertiaTensorDiagonal,Vector3 angularVelocity){
		Vector3 tmp = Quaternion.Inverse(inertiaTensorRotation) * angularVelocity;
		for (int i = 0; i < 3; ++i) {
			tmp [i] *= inertiaTensorDiagonal [i] * 10;
		}
		tmp = inertiaTensorRotation * tmp;
		return tmp;
	}
    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
		rigidbody.maxAngularVelocity = 1000;
		angular_momentum_change = new Vector3(0, 0, 0);
    }

	// Update is called once per frame
	void FixedUpdate() {
		Vector3 target_speed = (rigidbody.rotation * Vector3.up) * velocity_star * direction;
		angular_momentum_change = ComputeMomentOfInertia(rigidbody.inertiaTensorRotation,rigidbody.inertiaTensor,target_speed - rigidbody.angularVelocity);
		rigidbody.angularVelocity = target_speed;
    }
}
