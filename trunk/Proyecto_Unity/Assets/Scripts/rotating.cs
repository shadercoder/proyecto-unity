using UnityEngine;
using System.Collections;

public class rotating : MonoBehaviour {
	public Transform target;
	public float velocidad;
	public Vector3 eje;
	
	// Use this for initialization
	void Start() {
		target = transform;
	}
	
	// Update is called once per frame
	void Update() {
		// rotar en el eje a velocidad
		target.Rotate(eje * velocidad * Time.deltaTime);

	}
}