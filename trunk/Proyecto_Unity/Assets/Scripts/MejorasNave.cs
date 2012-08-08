using UnityEngine;
using System.Collections;

public class MejorasNave : MonoBehaviour {
	
	//Modelos de las piezas
	public GameObject piezaUno;
	public GameObject piezaUnoNivel2;
	public GameObject piezaDos;
	
	//Variables de control
	private bool[] mejorasCompradas;
	
	
	void Start() {
		//Inicializar las mejoras hechas a false
		mejorasCompradas = new bool[10];
		mejorasCompradas[0] = false;	//Mejora de la velocidad de la nave 1
		mejorasCompradas[1] = false;	//Mejora de la velocidad de la nave 2
		mejorasCompradas[2] = false;	//Mejora de capacidad de recursos 1
		mejorasCompradas[3] = false;	//Mejora de capacidad de recursos 2
		mejorasCompradas[4] = false;	//Mejora de produccion de energia 1
		mejorasCompradas[5] = false;	//Mejora de produccion de energia 2
		mejorasCompradas[6] = false;	//Mejora de orbita mas alta
		mejorasCompradas[7] = false;	//
		mejorasCompradas[8] = false;	//
		mejorasCompradas[9] = false;	//
		//Inicializar las piezas a invisibles
		if (piezaUno != null)
			piezaUno.GetComponent<MeshRenderer>().enabled = false;
		if (piezaUnoNivel2 != null) 
			piezaUnoNivel2.GetComponent<MeshRenderer>().enabled = false;
		if (piezaDos != null) 
			piezaDos.GetComponent<MeshRenderer>().enabled = false;
//		Debug.Log("Metodo Start() de MejorasNave completado.");
	}
	
	public void compraMejora1() {
		if (mejorasCompradas[1])
			return;
		Controles temp = Camera.main.gameObject.transform.parent.gameObject.GetComponent<Controles>();
		temp.mejoraVelocidad1();
		if (piezaUno != null) {
			piezaUno.GetComponent<MeshRenderer>().enabled = true;
		}
		else
			Debug.LogError("Falta por inicializar piezaUno desde el editor.");
		mejorasCompradas[0] = true;
	}
	
	public void compraMejora2() {
		if (!mejorasCompradas[0])
			return;
		Controles temp = Camera.main.gameObject.transform.parent.gameObject.GetComponent<Controles>();
		temp.mejoraVelocidad2();
		if (piezaUnoNivel2 != null) {
			piezaUnoNivel2.GetComponent<MeshRenderer>().enabled = true;
		}
		else
			Debug.LogError("Falta por inicializar piezaUnoNivel2 desde el editor.");
		if (piezaUno != null) {
			piezaUno.GetComponent<MeshRenderer>().enabled = false;
		}
		else
			Debug.LogError("Falta por inicializar piezaUno desde el editor.");
		mejorasCompradas[1] = true;
	}
	
	public void compraMejora3() {}
	
	public void compraMejora4() {}
	
	public void compraMejora5() {}
	
	public void compraMejora6() {
		Controles temp = Camera.main.gameObject.transform.parent.gameObject.GetComponent<Controles>();
		temp.mejoraSubirOrbita();
		mejorasCompradas[5] = true;
	}
	
	public void compraMejora7() {}
	
	public void compraMejora8() {}
	
	public void compraMejora9() {}
}
