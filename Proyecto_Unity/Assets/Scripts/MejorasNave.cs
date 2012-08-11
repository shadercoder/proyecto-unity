using UnityEngine;
using System.Collections;

public class MejorasNave : MonoBehaviour {
	
	//Modelos de las piezas
	public GameObject piezaUno;
	public GameObject piezaUnoNivel2;
	public GameObject piezaDos;
	
	//Variables de control
	public bool[] mejorasCompradas;
	
	//Cache
	private Principal principal;
	private Controles controles;
	private InterfazPrincipal interfaz;
	
	
	void Start() {
		//Inicializar cache
		principal = Camera.main.gameObject.GetComponent<Principal>();
		controles = Camera.main.gameObject.transform.parent.gameObject.GetComponent<Controles>();
		interfaz = Camera.main.gameObject.GetComponent<InterfazPrincipal>();
		//Inicializar las mejoras hechas a false
		mejorasCompradas = new bool[16];
		//Sensores
		mejorasCompradas[0] = false;	//Habilita barra inferior de informacion
		mejorasCompradas[1] = false;	//Deteccion de habitats
		mejorasCompradas[2] = false;	//Deteccion de metales raros
		mejorasCompradas[3] = false;	//Deteccion de animales y vegetales
		//Motor
		mejorasCompradas[4] = false;	//Mejora de motor nivel 1
		mejorasCompradas[5] = false;	//Mejora de motor nivel 2
		mejorasCompradas[6] = false;	//Mejora de aislamiento magnético (viaje por los polos)
		mejorasCompradas[7] = false;	//Mejora de orbita
		//Energia
		mejorasCompradas[8] = false;	//Mejora de energia nivel 1
		mejorasCompradas[9] = false;	//Mejora de energia nivel 2
		mejorasCompradas[10] = false;	//Desbloqueo de habilidades 1
		mejorasCompradas[11] = false;	//Desbloqueo de habilidades 2
		//Almacen
		mejorasCompradas[12] = false;	//Mejora de almacen 1
		mejorasCompradas[13] = false;	//Mejora de almacen 2
		mejorasCompradas[14] = false;	//Desbloquear almacenamiento de componentes avanzados
		mejorasCompradas[15] = false;	//Desbloquear almacenamiento de material biologico
		
		//Inicializar las piezas a invisibles
		if (piezaUno != null)
			piezaUno.GetComponent<MeshRenderer>().enabled = false;
		if (piezaUnoNivel2 != null) 
			piezaUnoNivel2.GetComponent<MeshRenderer>().enabled = false;
		if (piezaDos != null) 
			piezaDos.GetComponent<MeshRenderer>().enabled = false;
	}
	
	//Sensores -----------------------------------------------------------------------------------
	public void compraMejora0() {	//Habilitar barra inferior de informacion
		interfaz.mejoraBarraInferior();
		mejorasCompradas[0] = true;
	}
	
	public void compraMejora1() {	//Habilitar deteccion de habitats
		interfaz.mejoraMostrarHabitats();
		mejorasCompradas[1] = true;
	}
	
	public void compraMejora2() {	//Habilitar deteccion de metales raros
		interfaz.mejoraMostrarMetales();
		mejorasCompradas[2] = true;
	}
	
	public void compraMejora3() {	//Habilitar la deteccion de animales y plantas
		interfaz.mejoraMostrarSeres();
		mejorasCompradas[3] = true;
	}
	
	//Motores -----------------------------------------------------------------------------------
	public void compraMejora4() {	//Mejora de velocidad nivel 1
		controles.mejoraVelocidad1();
		if (piezaUno != null) {
			piezaUno.GetComponent<MeshRenderer>().enabled = true;
		}
		else
			Debug.LogError("Falta por inicializar piezaUno desde el editor.");
		mejorasCompradas[4] = true;
	}
	
	public void compraMejora5() {	//Mejora de velocidad nivel 2
		controles.mejoraVelocidad2();
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
		mejorasCompradas[5] = true;		
	}
	
	public void compraMejora6() {	//Mejora de aislamiento magnetico
		controles.mejoraAislamientoMag();
		mejorasCompradas[6] = true;
	}
	
	public void compraMejora7() {	//Mejora de orbita
		controles.mejoraSubirOrbita();
		mejorasCompradas[7] = true;
	}
	
	//Energia --------------------------------------------------------------------------------------
	public void compraMejora8() {	//Mejora energia nivel 1
		principal.mejoraEnergia1();
		mejorasCompradas[8] = true;
	}
	
	public void compraMejora9() {	//Mejora energia nivel 2
		principal.mejoraEnergia2();
		mejorasCompradas[9] = true;
	}
	
	public void compraMejora10() {	//Desbloqueo habilidades nivel 1
		//TODO
		mejorasCompradas[10] = true;
	}
	
	public void compraMejora11() {	//Desbloqueo habilidades nivel 2
		//TODO
		mejorasCompradas[11] = true;
	}
	
	//Almacenamiento -------------------------------------------------------------------------------
	public void compraMejora12() {	//Mejora de almacen nivel 1
		principal.setEnergiaMax(2000);
		principal.setCompBasMax(350);
		principal.setCompAdvMax(200);
//		principal.setMatBioMax(75);
		mejorasCompradas[12] = true;
		
	}
	
	public void compraMejora13() {	//Mejora de almacen nivel 2
		principal.setEnergiaMax(3000);
		principal.setCompBasMax(600);
		principal.setCompAdvMax(400);
		principal.setMatBioMax(50);
		mejorasCompradas[13] = true;
	}
	
	public void compraMejora14() {
		principal.setCompAdvMax(75);
		mejorasCompradas[14] = true;
	}
	
	public void compraMejora15() {
		principal.setMatBioMax(10);
		mejorasCompradas[15] = true;
	}
}
