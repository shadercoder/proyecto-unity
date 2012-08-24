using UnityEngine;
using System.Collections;

public class MejorasNave : MonoBehaviour {
	
	//Modelos de las piezas
	//Sensores
	public GameObject SensoresInfo;			//Habilita barra inferior de informacion
	public GameObject SensoresHabitat;		//Deteccion de habitats
	public GameObject SensoresRaros;		//Deteccion de metales raros
	public GameObject SensoresVida;			//Deteccion de animales y vegetales
	//Motor
	public GameObject MotorT1;				//Mejora de motor nivel 1
	public GameObject MotorT2;				//Mejora de motor nivel 2
	public GameObject MotorT3;				//Mejora de aislamiento magnético (viaje por los polos)
	public GameObject MotorT4;				//Mejora de orbita
	//Energia
	public GameObject GeneradorT1;			//Mejora de energia nivel 1
	public GameObject GeneradorT2;			//Mejora de energia nivel 2
	public GameObject GenHab1;				//Desbloqueo de habilidades 1
	public GameObject GenHab2;				//Desbloqueo de habilidades 2
	//Almacen
	public GameObject AlmacenT1C;			//Mejora de almacen 1 Com
	public GameObject AlmacenT1A;			//Mejora de almacen 1 Adv
	public GameObject AlmacenT1B;			//Mejora de almacen 1 Bio
	public GameObject AlmacenT2C;			//Mejora de almacen 2 Com
	public GameObject AlmacenT2A;			//Mejora de almacen 2 Adv
	public GameObject AlmacenT2B;			//Mejora de almacen 2 Bio
	
	
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
		
		/*Inicializar las piezas a invisibles
		if (SensoresInfo != null)
			SensoresInfo.GetComponent<MeshRenderer>().enabled = false;
		if (SensoresHabitat != null) 
			SensoresHabitat.GetComponent<MeshRenderer>().enabled = false;
		if (SensoresRaros != null) 
			SensoresRaros.GetComponent<MeshRenderer>().enabled = false;
		if (SensoresVida != null) 
			SensoresVida.GetComponent<MeshRenderer>().enabled = false;
		...*/
	}
	
	//Sensores -----------------------------------------------------------------------------------
	public void compraMejora0() {	//Habilitar barra inferior de informacion
		interfaz.mejoraBarraInferior();
		SensoresInfo.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[0] = true;
	}
	
	public void compraMejora1() {	//Habilitar deteccion de habitats
		interfaz.mejoraMostrarHabitats();
		SensoresHabitat.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[1] = true;
	}
	
	public void compraMejora2() {	//Habilitar deteccion de metales raros
		interfaz.mejoraMostrarMetales();
		SensoresRaros.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[2] = true;
	}
	
	public void compraMejora3() {	//Habilitar la deteccion de animales y planta
		interfaz.mejoraMostrarSeres();
		SensoresVida.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[3] = true;
	}
	
	//Motores -----------------------------------------------------------------------------------
	public void compraMejora4() {	//Mejora de velocidad nivel 1
		controles.mejoraVelocidad1();
		ocultaMotores();
		MotorT1.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[4] = true;
	}
	
	public void compraMejora5() {	//Mejora de velocidad nivel 2
		controles.mejoraVelocidad2();
		ocultaMotores();
		MotorT2.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[5] = true;		
	}
	
	public void compraMejora6() {	//Mejora de aislamiento magnetico
		controles.mejoraAislamientoMag();
		MotorT3.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[6] = true;
	}
	
	public void compraMejora7() {	//Mejora de orbita
		controles.mejoraSubirOrbita();
		MotorT4.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[7] = true;
	}
	
	//Energia --------------------------------------------------------------------------------------
	public void compraMejora8() {	//Mejora energia nivel 1
		principal.mejoraEnergia1();
		GeneradorT1.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[8] = true;
	}
	
	public void compraMejora9() {	//Mejora energia nivel 2
		principal.mejoraEnergia2();
		GeneradorT2.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[9] = true;
	}
	
	public void compraMejora10() {	//Desbloqueo habilidades nivel 1
		//TODO
		GenHab1.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[10] = true;
	}
	
	public void compraMejora11() {	//Desbloqueo habilidades nivel 2
		//TODO
		GenHab2.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[11] = true;
	}
	
	//Almacenamiento -------------------------------------------------------------------------------
	public void compraMejora12() {	//Mejora de almacenamiento de comp adv
		principal.setCompAdvMax(75);
		AlmacenT1A.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[12] = true;		
	}
	
	public void compraMejora13() {	//Mejora de almacenamiento de mat bio	
		AlmacenT1B.GetComponent<MeshRenderer>().enabled = true;
		principal.setMatBioMax(10);
		mejorasCompradas[13] = true;	
	}
	
	public void compraMejora14() {	//Mejora de almacen nivel 1
		principal.setEnergiaMax(2000);
		principal.setCompBasMax(250);
		principal.setCompAdvMax(100);
		principal.setMatBioMax(25);
		AlmacenT1C.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[14] = true;	
	}
	
	public void compraMejora15() {	//Mejora de almacen nivel 2
		principal.setEnergiaMax(3000);
		principal.setCompBasMax(750);
		principal.setCompAdvMax(400);
		principal.setMatBioMax(75);
		AlmacenT2C.GetComponent<MeshRenderer>().enabled = true;
		AlmacenT2A.GetComponent<MeshRenderer>().enabled = true;
		AlmacenT2B.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[15] = true;
	}
	
	public void ocultaMotores(){
//		MotorT1.GetComponent<MeshRenderer>().enabled = false;
//		MotorT2.GetComponent<MeshRenderer>().enabled = false;
//		MotorT3.GetComponent<MeshRenderer>().enabled = false;
//		MotorT4.GetComponent<MeshRenderer>().enabled = false;	
	}
}