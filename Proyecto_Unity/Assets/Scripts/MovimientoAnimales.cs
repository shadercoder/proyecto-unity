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
			if (this.transform.parent.position != posObjetivo) {
				this.transform.parent.position = Vector3.Lerp(posInicio, posObjetivo, (Time.time - tiempoInicioMov) / tiempoMovimiento);
				Vector3 norm = this.transform.parent.position - this.transform.parent.parent.position;
				Vector3 direccion = posObjetivo - this.transform.parent.position;
				this.transform.parent.rotation = Quaternion.LookRotation(norm, direccion);
				
				this.animation.Play("mover");
			}
			else {
				this.animation.Stop();
				enMovimiento = false;
			}
		}
	}
	
	public void moverAnimal(Vector3 puntoIn, float tiempoIn) {
		posObjetivo = puntoIn;
		posInicio = this.transform.parent.position;
		tiempoInicioMov = Time.time;
		tiempoMovimiento = tiempoIn;
		enMovimiento = true;
	}
	
	public void hazAnimacion(tipoEstadoAnimal estadoIn) {
		switch (estadoIn) {
		case tipoEstadoAnimal.comer:
			this.animation.Play("comer");
			break;
		case tipoEstadoAnimal.descansar:
			this.transform.RotateAroundLocal(Vector3.forward, 90);
			int temp = Random.Range(0, 3);
			switch (temp) {
			case 0: 
				this.animation.Play("descansar1");
				break;
			case 1:
				this.animation.Play("descansar2");
				break;
			case 2:
				this.animation.Play("descansar3");
				break;
			default:
				this.animation.Play("descansar1");
				break;
			}			
			break;
		case tipoEstadoAnimal.morir:
			this.animation.Play("morir");
			break;
		case tipoEstadoAnimal.nacer:
			this.animation.Play("nacer");
			break;
		default:
			break;
		}
	}
}
