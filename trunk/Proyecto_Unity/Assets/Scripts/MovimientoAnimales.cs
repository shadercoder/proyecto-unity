using UnityEngine;
using System.Collections;

public class MovimientoAnimales : MonoBehaviour {
	
	//Movimiento
	private float tiempoMovimiento		= 0.0f;					//El tiempo que debe tardar en hacer el movimiento
	private float tiempoInicioMov		= 0.0f;					//El momento en el que se inicio el movimiento
	private Vector3 posInicio			= Vector3.zero;			//La posicion inicial del movimiento
	private Vector3 posObjetivo			= Vector3.zero;			//La posicion a la que debe moverse
	private bool enMovimiento			= false;				//Si el objeto esta en movimiento o no lo esta
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (enMovimiento) {
			if (this.transform.position != posObjetivo) {
				this.transform.position = Vector3.Lerp(posInicio, posObjetivo, (Time.time - tiempoInicioMov) / tiempoMovimiento);
				Vector3 norm = this.transform.position - this.transform.parent.position;
				this.transform.rotation = Quaternion.LookRotation(norm);
//				this.animation.CrossFade("move");
			}
			else {
				enMovimiento = false;
			}
		}
	}
	
	public void moverAnimal(Vector3 puntoIn, float tiempoIn) {
		posObjetivo = puntoIn;
		posInicio = this.transform.position;
		tiempoInicioMov = Time.time;
		tiempoMovimiento = tiempoIn;
		enMovimiento = true;
	}
}
