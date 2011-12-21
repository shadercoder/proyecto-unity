using UnityEngine;
using System.Collections;

public class orbiting : MonoBehaviour {
	public Transform target;
	public Transform orbitingBody;
	public float speed;
	public Vector3 orbitDir;
	public Vector3 direction;
	
	void Awake(){
		orbitingBody = transform;
	} 
	
	// Use this for initialization
	void Start() {
		Vector3 line = target.position - transform.position;
		line.Normalize();
		Vector3.OrthoNormalize(ref line, ref direction, ref orbitDir);
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (target.position, orbitDir, speed * Time.deltaTime);
	}
}
