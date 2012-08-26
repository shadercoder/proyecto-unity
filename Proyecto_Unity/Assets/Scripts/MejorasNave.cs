using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
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
	public List<GameObject> AlmacenT1B;		//Mejora de almacen 1 Bio
	public GameObject AlmacenT2C;			//Mejora de almacen 2 Com
	public GameObject AlmacenT2A;			//Mejora de almacen 2 Adv
	public List<GameObject> AlmacenT2B;		//Mejora de almacen 2 Bio
	
	//Descripciones
	private List<string> descripciones;			//Todas las descripciones de las mejoras escritas debajo
	private string descripcionMejInfo			= "Usando unos sensores opticos de lo mas rudimentario la nave tendra acceso a informacion relevante en una zona limitada. Algo es algo. \n(Muestra Información Básica de las casillas)";
	private string descripcionMejHabitats		= "Un módulo de sensores de altura, temperatura, humedad, condiciones de viento y varios factores más que recopila informacion y elabora un mapa de los diferentes ecosistemas que presenta el planeta \n(Habilita el mostrar el Mapa de Habitats en Habilidades)";
	private string descripcionMejRaros			= "Utilizando un preciso medidor de radiacción estos sensores localizan los minerales más valiosos del subsuelo tras analizar las longitudes de onda mas exóticas. \n(Habilita la deteccion de Recursos en Habilidades)";
	private string descripcionMejVida			= "Utilizando tecnicas de predicción, visión térmica e introduciendo a los seres vivos marcadores a base de isotopos radioactivos (inocuos!) tendremos controlada a la fauna y la flora local con este sensor. \n(Habilita la Visualizacion de Vida en menu Habilidades)";
	private string descripcionMejMotorT1		= "Unos motores convencionales para la navegación espacial. No son lo mas optimo. No son lo mas eficiente. ¿Que si vas más rápido? Si. (Aumenta la velocidad de la nave)";
	private string descripcionMejMotorT2		= "Motores especialmente diseñados para la navegacion en orbitas geoestacionarias bajas. Cancelacion inercial, correccion de deriva gravitacional y un monton de tecnicismos. Estos si que si. \n(Mejora notablemente la velocidad de la nave)";
	private string descripcionMejAislamiento	= "Un escudo magnetico propio protege de los rayos cósmicos que azotan las zonas mas inhospitas del planeta. \n(Permite transitar cerca de los polos)";
	private string descripcionMejMotOrtbit		= "Aumenta la potencia del repulsor gravitacional y eleva la nave a una orbita superior. \n(Eleva la nave sobre el planeta)";
	private string descripcionMejAlmAv			= "Contenedores de condiciones controladas adaptados a material delicado permiten almacenar materiales sensibles a las adversas condiciones del espacio. \n(Permite almacenar Componentes Avanzados)";
	private string descripcionMejAlmBio			= "Para almacenar el material mas sensible obtenido de los serves vivos en condiciones higienicas e impolutas, a salvo de radiacciones, mimetizando la gravedad, temperatura y humedad de un planeta en un contenedor. \n(Permite almacenar Material Biológico)";
	private string descripcionMejAlm1			= "Mejorando los algoritmos de ordenación de los contenedores de la nave se obtiene un incremento en la capacidad de almacenaje. Viene con un contenedor gratis. \n(Mejora la Capazidad de Almacenamiento)";
	private string descripcionMejAlm2			= "Añadiendo contenedores de cada clase se amplia enormemente el espacio de almacenaje. (Mejora la Capacidad de Carga)";
	private string descripcionMejEner1			= "Añade un condensador con receptores de microondas para almacenar el excedente de energia producido por las centrales planetarias. \n(Aumenta la Energia Maxima disponible)";
	private string descripcionMejEner2			= "Añade un condensador en forma de anillo que aumenta la capacidad energetica de forma considerable. Además sirve de soporte para otras piezas. \n(Aumenta la Energia MAxima disponible y desbloquea otras mejoras)";
	private string descripcionMejHab1			= "[RELLENAR]habilita skill1";
	private string descripcionMejHab2			= "[RELLENAR]habilita skill2";
	
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
		
		//Inicializar las descripciones. EL ORDEN ES IMPORTANTE
		descripciones = new List<string>();
		descripciones.Add(descripcionMejInfo);
		descripciones.Add(descripcionMejHabitats);
		descripciones.Add(descripcionMejRaros);
		descripciones.Add(descripcionMejVida);
		descripciones.Add(descripcionMejMotorT1);
		descripciones.Add(descripcionMejMotorT2);
		descripciones.Add(descripcionMejAislamiento);
		descripciones.Add(descripcionMejMotOrtbit);
		descripciones.Add(descripcionMejEner1);
		descripciones.Add(descripcionMejEner2);
		descripciones.Add(descripcionMejHab1);
		descripciones.Add(descripcionMejHab2);
		descripciones.Add(descripcionMejAlm1);
		descripciones.Add(descripcionMejAlm2);
		descripciones.Add(descripcionMejAlmAv);
		descripciones.Add(descripcionMejAlmBio);
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
		MotorT1.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[4] = true;
	}
	
	public void compraMejora5() {	//Mejora de velocidad nivel 2
		controles.mejoraVelocidad2();
		MotorT1.GetComponent<MeshRenderer>().enabled = false;		
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
		
		for(int i = 0; i < AlmacenT1B.Count; i++)
		    AlmacenT1B[i].GetComponent<MeshRenderer>().enabled = true;
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
		for(int i = 0; i < AlmacenT2B.Count; i++)
		    AlmacenT2B[i].GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[15] = true;
	}
	
	public string getDescripcionMejora(int entrada) {
		return descripciones[entrada];
	}
	
	public string getNombreMejora(int entrada) {
		switch (entrada) {
		case 0: return "MejoraInfo";
		case 1: return "Deteccion de habitats";
		case 2: return "Detector de metales raros";
		case 3: return "Sensor de vida";
		case 4: return "Motor nv1";
		case 5: return "Motor nv2";
		case 6: return "Aislamiento magnetico";
		case 7: return "Orbita superior";
		case 8: return "Energia nv1";
		case 9: return "Energia nv2";
		case 10: return "Habilidades 1";
		case 11: return "Habilidades 2";
		case 12: return "Almacen nv1";
		case 13: return "Almacen nv2";
		case 14: return "Almacen de componentes";
		case 15: return "Almacen de material bio";
		default: return "Desconocida";
		}
	}
}