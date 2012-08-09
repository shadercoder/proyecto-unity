using UnityEngine;
using System.Collections;

public class MejorasNave : MonoBehaviour {
	
	//Modelos de las piezas
	public GameObject piezaUno;
	public GameObject piezaUnoNivel2;
	public GameObject piezaDos;
	
	//Variables de control
	public bool[] mejorasCompradas;
	
	
	void Start() {
		//Inicializar las mejoras hechas a false
		mejorasCompradas = new bool[10];
		mejorasCompradas[0] = false;	//
		mejorasCompradas[1] = false;	//Mejora de la velocidad de la nave 1
		mejorasCompradas[2] = false;	//Mejora de la velocidad de la nave 2
		mejorasCompradas[3] = false;	//Mejora de capacidad de recursos 1
		mejorasCompradas[4] = false;	//Mejora de capacidad de recursos 2
		mejorasCompradas[5] = false;	//Mejora de produccion de energia 1
		mejorasCompradas[6] = false;	//Mejora de produccion de energia 2
		mejorasCompradas[7] = false;	//Mejora de orbita mas alta
		mejorasCompradas[8] = false;	//
		mejorasCompradas[9] = false;	//
		//Inicializar las piezas a invisibles
		if (piezaUno != null)
			piezaUno.GetComponent<MeshRenderer>().enabled = false;
		if (piezaUnoNivel2 != null) 
			piezaUnoNivel2.GetComponent<MeshRenderer>().enabled = false;
		if (piezaDos != null) 
			piezaDos.GetComponent<MeshRenderer>().enabled = false;
	}
	
	public void compraMejora0() {
		//TODO
		mejorasCompradas[0] = true;
	}
	
	public void compraMejora1() {	//Mejora de velocidad nivel 1
		//Si ya se tiene la de nivel 2, no hace nada
		if (mejorasCompradas[2])
			return;
		Controles temp = Camera.main.gameObject.transform.parent.gameObject.GetComponent<Controles>();
		temp.mejoraVelocidad1();
		if (piezaUno != null) {
			piezaUno.GetComponent<MeshRenderer>().enabled = true;
		}
		else
			Debug.LogError("Falta por inicializar piezaUno desde el editor.");
		mejorasCompradas[1] = true;
	}
	
	public void compraMejora2() {	//Mejora de velocidad nivel 2
		//Si no tiene la de nivel 1, no hace nada
		if (!mejorasCompradas[1])
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
		mejorasCompradas[2] = true;
	}
	
	public void compraMejora3() {	//Mejora de almacen nivel 1
		//Si ya se tiene la de nivel 2, no hace nada
		if (mejorasCompradas[4])
			return;
		Principal temp = Camera.main.gameObject.GetComponent<Principal>();
		temp.setEnergiaMax(1500);
		temp.setCompBasMax(350);
		temp.setCompAdvMax(150);
		temp.setMatBioMax(75);
		mejorasCompradas[3] = true;
	}
	
	public void compraMejora4() {	//Mejora de almacen nivel 2
		//Si no tiene la de nivel 1, no hace nada
		if (!mejorasCompradas[3])
			return;
		Principal temp = Camera.main.gameObject.GetComponent<Principal>();
		temp.setEnergiaMax(2000);
		temp.setCompBasMax(400);
		temp.setCompAdvMax(200);
		temp.setMatBioMax(100);
		mejorasCompradas[4] = true;
	}
	
	public void compraMejora5() {}
	
	public void compraMejora6() {}
	
	public void compraMejora7() {
		//Si no tiene la mejora de energia de nivel 1 (mejora 5), no hace nada
//		if (!mejorasCompradas[5])
//			return;
		Controles temp = Camera.main.gameObject.transform.parent.gameObject.GetComponent<Controles>();
		temp.mejoraSubirOrbita();
		mejorasCompradas[6] = true;
	}
	
	public void compraMejora8() {}
	
	public void compraMejora9() {}
}
