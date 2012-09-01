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
	
	//Descripciones mejoras
	private List<string> descripciones;			//Todas las descripciones de las mejoras escritas debajo
	private string descripcionMejInfo			= "Usando unos sensores opticos de lo mas rudimentario la nave tendra acceso a informacion relevante en una zona limitada. Algo es algo. \n(Muestra Informacion Basica de las casillas)";
	private string descripcionMejHabitats		= "Un modulo de sensores de altura, temperatura, humedad, condiciones de viento y varios factores mas que recopila informacion y elabora un mapa de los diferentes ecosistemas que presenta el planeta \n(Habilita el mostrar el Mapa de Habitats en Habilidades)";
	private string descripcionMejRaros			= "Utilizando un preciso medidor de radiaccion estos sensores localizan los minerales mas valiosos del subsuelo tras analizar las longitudes de onda mas exoticas. \n(Habilita la deteccion de Recursos en Habilidades)";
	private string descripcionMejVida			= "Utilizando tecnicas de prediccion, vision termica e introduciendo a los seres vivos marcadores a base de isotopos radioactivos (inocuos!) tendremos controlada a la fauna y la flora local con este sensor. \n(Habilita la Visualizacion de Vida en menu Habilidades)";
	private string descripcionMejMotorT1		= "Unos motores convencionales para la navegacion espacial. No son lo mas optimo. No son lo mas eficiente. ¿Que si vas mas rapido? Si. (Aumenta la velocidad de la nave)";
	private string descripcionMejMotorT2		= "Motores especialmente diseñados para la navegacion en orbitas geoestacionarias bajas. Cancelacion inercial, correccion de deriva gravitacional y un monton de tecnicismos. Estos si que si. \n(Mejora notablemente la velocidad de la nave)";
	private string descripcionMejAislamiento	= "Un escudo magnetico propio protege de los rayos cosmicos que azotan las zonas mas inhospitas del planeta. \n(Permite transitar cerca de los polos)";
	private string descripcionMejMotOrtbit		= "Aumenta la potencia del repulsor gravitacional y eleva la nave a una orbita superior. \n(Eleva la nave sobre el planeta)";
	private string descripcionMejAlmAv			= "Contenedores de condiciones controladas adaptados a material delicado permiten almacenar materiales sensibles a las adversas condiciones del espacio. \n(Permite almacenar Componentes Avanzados)";
	private string descripcionMejAlmBio			= "Para almacenar el material mas sensible obtenido de los serves vivos en condiciones higienicas e impolutas, a salvo de radiacciones, mimetizando la gravedad, temperatura y humedad de un planeta en un contenedor. \n(Permite almacenar Material Biologico)";
	private string descripcionMejAlm1			= "Mejorando los algoritmos de ordenacion de los contenedores de la nave se obtiene un incremento en la capacidad de almacenaje. Viene con un contenedor gratis. \n(Mejora la Capazidad de Almacenamiento)";
	private string descripcionMejAlm2			= "Añadiendo contenedores de cada clase se amplia enormemente el espacio de almacenaje. (Mejora la Capacidad de Carga)";
	private string descripcionMejEner1			= "Añade un condensador con receptores de microondas para almacenar el excedente de energia producido por las centrales planetarias. \n(Aumenta la Energia Maxima disponible)";
	private string descripcionMejEner2			= "Añade un condensador en forma de anillo que aumenta la capacidad energetica de forma considerable. Ademas sirve de soporte para otras piezas. \n(Aumenta la Energia MAxima disponible y desbloquea otras mejoras)";
	private string descripcionMejHab1			= "Un array de paneles solares capta la energia de la estrella durante el dia, y la almacena para su posterior uso. (Habilita el Foco Solar)";
	private string descripcionMejHab2			= "[RELLENAR]habilita skill2";
	
	//Descripciones habilidades
	private List<string> habilidades;			//Todas las descripciones de las habilidades
	private string descripcionHab1				= "El Foco Solar es totalmente ecologico. Utiliza la energia captada cuando la nave esta bajo la accion directa de la radiaccion solar y la redirige a las zonas donde es interesante. Aun no se ha comprobado si este fenomeno es perjudicial para los animales, pero de momento no hemos recibido ninguna queja.";
	private string descripcionHab2				= "[RELLENAR] Hab 2";
	private string descripcionHab3				= "[RELLENAR] Hab 3";
	private string descripcionHab4				= "[RELLENAR] Hab 4";
	private string descripcionHab5				= "[RELLENAR] Hab 5";
	
	//Costes
	private List<List<int>> costesMejoras;
	private List<int> costeMej0;
	private List<int> costeMej1;
	private List<int> costeMej2;
	private List<int> costeMej3;
	private List<int> costeMej4;
	private List<int> costeMej5;
	private List<int> costeMej6;
	private List<int> costeMej7;
	private List<int> costeMej8;
	private List<int> costeMej9;
	private List<int> costeMej10;
	private List<int> costeMej11;
	private List<int> costeMej12;
	private List<int> costeMej13;
	private List<int> costeMej14;
	private List<int> costeMej15;
	
	//Variables de control
	public bool[] mejorasCompradas;
	
	//Cache
	private Principal principal;
	private Controles controles;
	private InterfazPrincipal interfaz;
	
	void Awake() {
		costeMej0 = new List<int>();		//Sensores 1 (info)
		costeMej0.Add(700);	//Coste energia
		costeMej0.Add(250);	//Coste comp bas
		costeMej0.Add(0);	//Coste comp adv
		costeMej0.Add(0);	//Coste mat bio
		
		costeMej1 = new List<int>();		//Sensores 2 (habitats)
		costeMej1.Add(800);	//Coste energia
		costeMej1.Add(150);	//Coste comp bas
		costeMej1.Add(0);	//Coste comp adv
		costeMej1.Add(0);	//Coste mat bio
		
		costeMej2 = new List<int>();		//Sensores 3 (recursos)
		costeMej2.Add(900);	//Coste energia
		costeMej2.Add(300);	//Coste comp bas
		costeMej2.Add(0);	//Coste comp adv
		costeMej2.Add(0);	//Coste mat bio
		
		costeMej3 = new List<int>();		//Sensores 4 (animales)
		costeMej3.Add(1000);//Coste energia
		costeMej3.Add(350);	//Coste comp bas
		costeMej3.Add(0);	//Coste comp adv
		costeMej3.Add(0);	//Coste mat bio
		
		costeMej4 = new List<int>();		//Motor 1
		costeMej4.Add(800);	//Coste energia
		costeMej4.Add(280);	//Coste comp bas
		costeMej4.Add(0);	//Coste comp adv
		costeMej4.Add(0);	//Coste mat bio
		
		costeMej5 = new List<int>();		//Motor 2
		costeMej5.Add(2000);	//Coste energia
		costeMej5.Add(700);		//Coste comp bas
		costeMej5.Add(0);		//Coste comp adv
		costeMej5.Add(0);		//Coste mat bio
		
		costeMej6 = new List<int>();		//Motor 3 (aislamiento)
		costeMej6.Add(700);	//Coste energia
		costeMej6.Add(200);	//Coste comp bas
		costeMej6.Add(0);	//Coste comp adv
		costeMej6.Add(0);	//Coste mat bio
		
		costeMej7 = new List<int>();		//Motor 4 (orbita)
		costeMej7.Add(2000);	//Coste energia
		costeMej7.Add(600);		//Coste comp bas
		costeMej7.Add(60);		//Coste comp adv
		costeMej7.Add(0);		//Coste mat bio
		
		costeMej8 = new List<int>();		//Energia 1
		costeMej8.Add(1000);	//Coste energia
		costeMej8.Add(120);		//Coste comp bas
		costeMej8.Add(0);		//Coste comp adv
		costeMej8.Add(0);		//Coste mat bio
		
		costeMej9 = new List<int>();		//Energia 2
		costeMej9.Add(3000);	//Coste energia
		costeMej9.Add(450);		//Coste comp bas
		costeMej9.Add(25);		//Coste comp adv
		costeMej9.Add(0);		//Coste mat bio
		
		costeMej10 = new List<int>();		//Energia 3 (habs 1)
		costeMej10.Add(4500);	//Coste energia
		costeMej10.Add(550);	//Coste comp bas
		costeMej10.Add(250);	//Coste comp adv
		costeMej10.Add(0);		//Coste mat bio
		
		costeMej11 = new List<int>();		//Energia 4 (habs 2)
		costeMej11.Add(7500);	//Coste energia
		costeMej11.Add(1500);	//Coste comp bas
		costeMej11.Add(500);	//Coste comp adv
		costeMej11.Add(0);		//Coste mat bio
		
		costeMej12 = new List<int>();		//Almacen 1 (comp adv)
		costeMej12.Add(950);	//Coste energia
		costeMej12.Add(300);	//Coste comp bas
		costeMej12.Add(0);		//Coste comp adv
		costeMej12.Add(10);		//Coste mat bio
		
		costeMej13 = new List<int>();		//Almacen 2 (mat bio)
		costeMej13.Add(900);	//Coste energia
		costeMej13.Add(340);	//Coste comp bas
		costeMej13.Add(0);		//Coste comp adv
		costeMej13.Add(0);		//Coste mat bio
		
		costeMej14 = new List<int>();		//Almacen 3 (carga 1)
		costeMej14.Add(800);	//Coste energia
		costeMej14.Add(380);	//Coste comp bas
		costeMej14.Add(70);		//Coste comp adv
		costeMej14.Add(10);		//Coste mat bio
		
		costeMej15 = new List<int>();		//Almacen 4 (carga 2)
		costeMej15.Add(2600);	//Coste energia
		costeMej15.Add(450);	//Coste comp bas
		costeMej15.Add(250);	//Coste comp adv
		costeMej15.Add(25);		//Coste mat bio
	}
	
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
			//Y habilidades...
		habilidades = new List<string>();
		habilidades.Add(descripcionHab1);
		habilidades.Add(descripcionHab2);
		habilidades.Add(descripcionHab3);
		habilidades.Add(descripcionHab4);
		habilidades.Add(descripcionHab5);
		
		//Inicializar los costes. EL ORDEN ES IMPORTANTE
		costesMejoras = new List<List<int>>();
		costesMejoras.Add(costeMej0);
		costesMejoras.Add(costeMej1);
		costesMejoras.Add(costeMej2);
		costesMejoras.Add(costeMej3);
		costesMejoras.Add(costeMej4);
		costesMejoras.Add(costeMej5);
		costesMejoras.Add(costeMej6);
		costesMejoras.Add(costeMej7);
		costesMejoras.Add(costeMej8);
		costesMejoras.Add(costeMej9);
		costesMejoras.Add(costeMej10);
		costesMejoras.Add(costeMej11);
		costesMejoras.Add(costeMej12);
		costesMejoras.Add(costeMej13);
		costesMejoras.Add(costeMej14);
		costesMejoras.Add(costeMej15);
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
		principal.setEnergiaMax(3500);
		principal.setCompBasMax(1200);
		principal.setCompAdvMax(300);
		principal.setMatBioMax(25);
		AlmacenT1C.GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[14] = true;	
	}
	
	public void compraMejora15() {	//Mejora de almacen nivel 2
		principal.setEnergiaMax(9000);
		principal.setCompBasMax(2500);
		principal.setCompAdvMax(1000);
		principal.setMatBioMax(100);
		AlmacenT2C.GetComponent<MeshRenderer>().enabled = true;
		AlmacenT2A.GetComponent<MeshRenderer>().enabled = true;
		for(int i = 0; i < AlmacenT2B.Count; i++)
		    AlmacenT2B[i].GetComponent<MeshRenderer>().enabled = true;
		mejorasCompradas[15] = true;
	}
	
	public string getDescripcionMejora(int entrada) {
		if (entrada < 0 || entrada >= descripciones.Count)
			return "";
		return descripciones[entrada];
	}
	
	public string getDescripcionHabilidad(int entrada) {
		if (entrada < 0 || entrada >= habilidades.Count)
			return "";
		return habilidades[entrada];
	}
	
	public List<int> getCosteMejora(int entrada) {
		if (entrada < 0 || entrada >= costesMejoras.Count)
			return null;
		return costesMejoras[entrada];
	}
}