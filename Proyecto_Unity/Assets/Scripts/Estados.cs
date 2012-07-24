using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using ;

public class Estados : MonoBehaviour {

	//Variables ---------------------------------------------------------------------------------------------------------------------------
	
	//Públicas
	public GameObject camaraReparaciones;								//Para mostrar las opciones de las reparaciones de la nave
	public GameObject camaraPrincipal;									//Para mostrar el mundo completo (menos escenas especiales)
	public GUISkin estiloGUI;											//Los estilos diferentes para la GUI, configurables desde el editor
	public GUISkin estiloGUI_Nuevo;										//Estilos para la GUI, nueva versión
	public GameObject objetoOceano;										//El objeto que representa la esfera del oceano
	public GameObject objetoRoca;										//El objeto que representa la esfera de la roca
	public GameObject objetoPlanta;										//El objeto que representa la esfera de las plantas
	public Texture2D texPlantas;										//La textura donde se pintan las plantas 
	public float tiempoPincel					= 0.001f;				//Incremento de tiempo para aplicar el pincel
	public float tiempoTooltip 					= 0.75f;				//Tiempo que tarda en aparecer el tooltip	
	public GameObject sonidoAmbiente;									//El objeto que va a contener la fuente del audio de ambiente
	public GameObject sonidoFX;											//El objeto que va a contener la fuente de efectos de audio
	public int energia = 100;											//Cantidad de energia almacenada en la nave
	public int energiaDif = 10;											//Incremento o decremento por turno de energia
	public int componentesBasicos = 25;									//Cantidad de componentes basicos alojados en la nave
	public int componentesBasicosDif = 0;								//Incremento o decremento por turno de componentes basicos
	public int componentesAvanzados = 0;								//Cantidad de componentes avanzados alojados en la nave
	public int componentesAvanzadosDif = 0;								//Incremento o decremento por turno de componentes avanzados
	public int materialBiologico = 0;									//Cantidad de material biologico alojado en la nave
	public int materialBiologicoDif = 0;								//Incremento o decremento por turno de material biologico
		
	//Botones grandes
	private bool menuAltera						= false;				//Variables de control de los botones grandes
	private bool menuCamara						= false;				//de la interfaz del menu izquierdo
	private bool menuOpcion						= false;
		//Botones pequeños
	private bool botonPequePincel				= false;				//Variables de control de los botones pequeños
	private bool botonPequeSubir				= false;				//de la interfaz del menu izquierdo
	private bool botonPequeBajar				= false;
	private bool botonPequeAllanar				= false;
		//Ventanas de visualizacion
	private bool objetivoAlcanzado				= false;				//Si es true, muestra una ventana con informacion del objetivo pulsado
	
	//Privadas del script
	private T_estados estado 					= T_estados.principal;	//Los estados por los que pasa el juego
	public Vida vida;													//Tablero lógico del algoritmo		
	private GameObject contenedorTexturas;								//El contenedor de las texturas de la primera escena
	public float escalaTiempo					= 1.0f;					//La escala temporal a la que se updateará todo
		//Pinceles
	private bool activarPinceles				= false;				//Variable de control para pintar sobre la textura	
	private int seleccionPincel 				= 0;					//la selección del pincel a utilizar
	private float ultimoPincel					= 0.0f;					//Ultimo pincel aplicado
		//Filtros -----------------
	private float tiempoCasilla					= 0.5f;					//Cuanto tiempo tiene que pasar entre casilla y casilla
	private float ultimaCasilla					= 0.0f;					//Momento en el que se lanzo el ultimo rayo
		//Filtro de especies 
	private bool infoEspecies					= false;				//Indica si se muestra la info de las especies de la casilla
	private string infoEspecies_Hab;									//La primera linea a mostrar en la casilla de info de especies
	private string infoEspecies_Esp;									//La segunda linea de la casilla info especies
		//Filtro de elementos
	private bool infoElems						= false;				//Indica si se muestra la info de los elementos de la casilla
	private string infoElems_Elem;										//Primera linea a mostrar de la info de elementos
		//Elemento seleccionado
	private Ser seleccionado;											//El ser que se ha seleccionado (puede ser un animal o un edificio)
	private bool tipoEdificio;											//True si es un edificio, false en otro caso (animal o planta)
	private string infoSeleccionado				= "";					//La informacion referente al elemento seleccionado con el click izquierdo del raton.
		//Algoritmo vida
	private float tiempoPaso					= 0.0f;					//El tiempo que lleva el paso actual del algoritmo
	public int numPasos							= 0;					//Numero de pasos del algoritmo ejecutados
	private bool algoritmoActivado				= false;				//Se encuentra activado el algoritmo de la vida?
		
	//Tooltips
	private Vector3 posicionMouse 				= Vector3.zero;			//Guarda la ultima posicion del mouse		
	private bool activarTooltip 				= false;				//Controla si se muestra o no el tooltip	
	private float ultimoMov 					= 0.0f;					//Ultima vez que se movio el mouse		
	
	//Menus para guardar
	private Vector2 posicionScroll 				= Vector2.zero;			//La posicion en la que se encuentra la ventana con scroll
	private int numSaves 						= 0;					//El numero de saves diferentes que hay en el directorio respectivo
	private int numSavesExtra 					= 0;					//Numero de saves que hay que no se ven al primer vistazo en la scrollview
	private string[] nombresSaves;										//Los nombres de los ficheros de savegames guardados

	//Tipos especiales ----------------------------------------------------------------------------------------------------------------------
	 
	//Añadir los que hagan falta mas tarde
	enum T_estados {inicial, principal, reparaciones, filtros, guardar, opciones, salir};
	
	//Funciones auxiliares -----------------------------------------------------------------------------------------------------------------------
	
	private IEnumerator corutinaPincel() {
		//Interacción con los pinceles
		if (Time.realtimeSinceStartup >= ultimoPincel + tiempoPincel) {
			ultimoPincel = Time.realtimeSinceStartup;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				bool temp = false;
				if (botonPequeSubir)
					temp = true;
				FuncTablero.pintaPincel(hit, seleccionPincel, temp);
				Texture2D texTemp = hit.transform.renderer.sharedMaterial.mainTexture as Texture2D;
				texTemp.Apply();
			}
		}
		yield return new WaitForEndOfFrame();
	}
	
	private void actualizaInfoEspecies() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) {
			Vector2 coordTemp = hit.textureCoord;
			Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			coordTemp.x = (int)((int)(coordTemp.x * tex.width) / FuncTablero.getRelTexTabAncho());
			coordTemp.y = (int)((int)(coordTemp.y * tex.height) / FuncTablero.getRelTexTabAlto());
			Vegetal veg = vida.tablero[(int)coordTemp.x, (int)coordTemp.y].vegetal;
			Animal anim = vida.tablero[(int)coordTemp.x, (int)coordTemp.y].animal;
			T_habitats hab = vida.tablero[(int)coordTemp.x, (int)coordTemp.y].habitat;
			infoEspecies_Hab = "Habitat: " + hab.ToString() + "";
			infoEspecies_Esp = "";
			if (anim != null)
				infoEspecies_Esp += "Animal: " + anim.especie.nombre + "\n";
			if (veg != null)
				infoEspecies_Esp += "Planta: " + veg.especie.nombre;
		}
		else {
			infoEspecies_Esp = "";
			infoEspecies_Hab = "";
		}
	}
	
	private void actualizaInfoElems() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) {
			Vector2 coordTemp = hit.textureCoord;
			Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			coordTemp.x = (int)((int)(coordTemp.x * tex.width) / FuncTablero.getRelTexTabAncho());
			coordTemp.y = (int)((int)(coordTemp.y * tex.height) / FuncTablero.getRelTexTabAlto());
			T_elementos[] elem = vida.tablero[(int)coordTemp.x, (int)coordTemp.y].elementos;
			if (elem.Length > 0) {
				infoElems_Elem = "Encontrado " + elem[0].ToString();
				for (int i = 1; i < elem.Length; i++) {
					infoElems_Elem += ", " + elem[i].ToString() + "\n";	
				}
			}
			else 
				infoElems_Elem = "";
		}
		else {
			infoElems_Elem = "";
		}
	}
	
	public void getInfoSeresCasilla() {
		if (objetivoAlcanzado)
			return;
		RaycastHit hit;
		bool alcanzado = false;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) {
			Vector2 coordTemp = hit.textureCoord;
			Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			coordTemp.x = (int)((int)(coordTemp.x * tex.width) / FuncTablero.getRelTexTabAncho());
			coordTemp.y = (int)((int)(coordTemp.y * tex.height) / FuncTablero.getRelTexTabAlto());
			Casilla elem = vida.tablero[(int)coordTemp.y, (int)coordTemp.x];
			if (elem.animal != null) {
				infoSeleccionado = "";
				infoSeleccionado += "\t Animal \n";		
				infoSeleccionado += "Especie: " + elem.animal.especie.nombre + ".\n";
				infoSeleccionado += "Vive en: ";
				List<T_habitats> habitats = elem.animal.especie.habitats;
				if (habitats.Count > 0) {
					infoSeleccionado += habitats[0].ToString();
					for(int i = 1; i < habitats.Count; i++) {
						infoSeleccionado += ", " + habitats[i].ToString();
					}
				}
				infoSeleccionado += ".\n";
				infoSeleccionado += "Comida restante: " + elem.animal.reserva.ToString() + ".\n";
				alcanzado = true;
			} 
			else if (elem.edificio != null) {
				infoSeleccionado = "";
				infoSeleccionado += "\t Edificio \n";		
				infoSeleccionado += "Tipo: " + elem.edificio.tipo.nombre + ".\n";
				/*
				 * Aqui hay que añadir el acceso a las caracteristicas propias del edificio: sliders de produccion, tipo de
				 * materiales recogidos, encendido/apagado, etc.
				 * */
				alcanzado = true;
			}
			else if (elem.vegetal != null) {
				infoSeleccionado = "";
				infoSeleccionado += "\t Vegetal \n";		
				infoSeleccionado += "Especie: " + elem.vegetal.especie.nombre + ".\n";
				infoSeleccionado += "Vive en: ";
				List<T_habitats> habitats = elem.vegetal.especie.habitats;
				if (habitats.Count > 0) {
					infoSeleccionado += habitats[0].ToString();
					for(int i = 1; i < habitats.Count; i++) {
						infoSeleccionado += ", " + habitats[i].ToString();
					}
				}
				infoSeleccionado += ".\n";
				infoSeleccionado += "Poblacion: " + elem.vegetal.numVegetales.ToString() + ".\n";
				alcanzado = true;
			}
			else {
				infoSeleccionado = "";
				alcanzado = false;
			}
		}
		if (alcanzado) {
			//Se activa la visualizacion de la correspondiente ventana
			objetivoAlcanzado = true;
		}
	}
	
	//Introduce un ser en el tablero al hacer click
	public void insertaSerTablero(Ser introducir) {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) {
			Vector2 coordTemp = hit.textureCoord;
			Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			coordTemp.x = (int)((int)(coordTemp.x * tex.width) / FuncTablero.getRelTexTabAncho());
			coordTemp.y = (int)((int)(coordTemp.y * tex.height) / FuncTablero.getRelTexTabAlto());
			Casilla elem = vida.tablero[(int)coordTemp.y, (int)coordTemp.x];
			if (elem.animal == null && elem.edificio == null && elem.vegetal != null) {
				if (introducir is Animal) {
					Animal animalTemp = (Animal)introducir;
					vida.anadeAnimal(animalTemp.especie, (int)coordTemp.y, (int)coordTemp.x);
				}
				else if (introducir is Vegetal) {
					Vegetal vegetalTemp = (Vegetal)introducir;
					vida.anadeVegetal(vegetalTemp.especie, (int)coordTemp.y, (int)coordTemp.x);
				}
				else if (introducir is Edificio) {
					Edificio edificioTemp = (Edificio)introducir;
					vida.anadeEdificio(edificioTemp.tipo, (int)coordTemp.y, (int)coordTemp.x);
				}
			}
		}
	}
	
	
	//Funciones principales ----------------------------------------------------------------------------------------------------------------------
	private void creacionInicial() {
		Debug.Log("Creando planeta de cero en creacionInicial");
		//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
		Texture2D texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		Texture2D texturaAgua = objetoOceano.renderer.sharedMaterial.mainTexture as Texture2D;
		
		Color[] pixels = new Color[texturaBase.width * texturaBase.height];
		FuncTablero.inicializa(texturaBase);
		
		pixels = FuncTablero.ruidoTextura();											//Se crea el ruido para la textura base y normales...
		pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);			//Se suaviza el borde lateral...
		pixels = FuncTablero.suavizaPoloTex(pixels, texturaBase.height / 20);			//Se suavizan los polos...
		
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();
		
		
		pixels = FuncTablero.calculaTexAgua(pixels);
		texturaAgua.SetPixels(pixels);
		texturaAgua.Apply();
		
		MeshFilter filter = objetoRoca.GetComponent<MeshFilter>();
		MeshFilter filter2 = objetoPlanta.GetComponent<MeshFilter>();
		Mesh meshTemp = filter.mesh;
		meshTemp = FuncTablero.extruyeVertices(meshTemp, texturaBase, 0.5f, objetoRoca.transform.position);
		filter.mesh = meshTemp;
		filter2.mesh = meshTemp;
		estado = T_estados.principal;
	}
	
	//Update y transiciones de estados -------------------------------------------------------------------------------------------------------
	
	void Awake() {		
		Random.seed = System.DateTime.Now.Millisecond;
		contenedorTexturas = GameObject.FindGameObjectWithTag("Carga");
		if (contenedorTexturas == null) {
			creacionInicial();
		}
		else {
			//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez			
			Texture2D texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
			texturaBase = temp.texturaBase;
			texturaBase.Apply();
			objetoOceano.transform.localScale = temp.escalaOceano;
		}
		Audio_Ambience ambiente = sonidoAmbiente.GetComponent<Audio_Ambience>();
		Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX>();
		if (PlayerPrefs.GetInt("MusicaOn") == 1)
			ambiente.activado = true;
		else
			ambiente.activado = false;
		ambiente.volumen = PlayerPrefs.GetFloat("MusicaVol");
		if (PlayerPrefs.GetInt("SfxOn") == 1)
			efectos.activado = true;
		else
			efectos.activado = false;
		efectos.volumen = PlayerPrefs.GetFloat("SfxVol");
		
		Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		Mesh mesh = objetoRoca.GetComponent<MeshFilter>().sharedMesh;
		Casilla[,] tablero = FuncTablero.iniciaTablero(tex, mesh);
		vida = new Vida(tablero, texPlantas, objetoRoca.transform);				
		
		numSaves = SaveLoad.FileCount();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames();
		numSavesExtra = numSaves - 3;
		if (numSavesExtra < 0)
			numSavesExtra = 0;	
	}
	
	void Start()
	{
		creacionEspeciesEdificios();
	}
	
	void FixedUpdate() {
		//Algoritmo de vida		
		tiempoPaso += Time.deltaTime;		
		if(algoritmoActivado && tiempoPaso > 1.0f) {		//El 1.0f significa que se ejecuta un paso cada 1.0 segundos, cuando la escala temporal esta a 1.0
			vida.algoritmoVida();
			numPasos ++;
			tiempoPaso = 0.0f;
		}
	}
	
	void Update () {		
		switch (estado) {
			case T_estados.inicial:
				creacionInicial();
				break;
				
			case T_estados.principal:				
				//Controlamos los clicks de raton en este estado solamente
				if (Input.GetMouseButtonDown(0)) {
					getInfoSeresCasilla();
				}
				break;
				
			case T_estados.filtros:
				break;
				
			case T_estados.reparaciones:
				break;
				
			case T_estados.opciones:
				Time.timeScale = 0;
				break;
				
			case T_estados.guardar:
				Time.timeScale = 0;
				break;
				
			case T_estados.salir:
				Application.LoadLevel("Escena_Inicial");
				break;
				
			default:
				//Error!
				Debug.LogError("Estado del juego desconocido! La variable contiene: " + estado);
				break;
		}
		
		//Control del tooltip
		if (Input.mousePosition != posicionMouse) {
			posicionMouse = Input.mousePosition;
			activarTooltip = false;
			ultimoMov = Time.realtimeSinceStartup;
		}
		else {
			if (Time.realtimeSinceStartup >= ultimoMov + tiempoTooltip) {
				activarTooltip = true;
			}
		}
		if (GUI.changed) {
			
			//Control del timescale
			Time.timeScale = escalaTiempo;
			Time.fixedDeltaTime = 0.02f * escalaTiempo;
		}
		
		//Estado de los pinceles
		if (botonPequeSubir || botonPequeBajar || botonPequeAllanar) 
			activarPinceles = true;
		else
			activarPinceles = false;
		
		//Info de la casilla
		if ((infoEspecies || infoElems) && (Time.realtimeSinceStartup > ultimaCasilla + tiempoCasilla)) {
			//Especies
			if (infoEspecies) {
				actualizaInfoEspecies();
			}
			//Elementos
			else {
				actualizaInfoElems();
			}
		}
		//Debug solo
		if(Input.GetKeyDown(KeyCode.V)) 
			if(algoritmoActivado)
				algoritmoActivado = false;
			else
				algoritmoActivado = true;
		
	}
	
	void creacionEspeciesEdificios()
	{		
		List<T_habitats> habitats1 = new List<T_habitats>();
		habitats1.Add(T_habitats.plain);
		habitats1.Add(T_habitats.sand);
		habitats1.Add(T_habitats.coast);
		habitats1.Add(T_habitats.mountain);
		habitats1.Add(T_habitats.hill);
		habitats1.Add(T_habitats.sea);
		habitats1.Add(T_habitats.volcanic);

		ModelosEdificios modelosEdificios = GameObject.FindGameObjectWithTag("ModelosEdificios").GetComponent<ModelosEdificios>();		
		ModelosVegetales modelosVegetales = GameObject.FindGameObjectWithTag("ModelosVegetales").GetComponent<ModelosVegetales>();		
		ModelosAnimales modelosAnimales = GameObject.FindGameObjectWithTag("ModelosAnimales").GetComponent<ModelosAnimales>();		
				
		TipoEdificio tipoEdif1 = new TipoEdificio("fabricaCompBas",habitats1,modelosEdificios.fabCompBas);
		vida.anadeTipoEdificio(tipoEdif1);
		TipoEdificio tipoEdif2 = new TipoEdificio("centralEnergia",habitats1,modelosEdificios.centralEnergia);
		vida.anadeTipoEdificio(tipoEdif2);
		TipoEdificio tipoEdif3 = new TipoEdificio("granja",habitats1,modelosEdificios.granja);
		vida.anadeTipoEdificio(tipoEdif3);
		TipoEdificio tipoEdif4 = new TipoEdificio("fabricaCompAdv",habitats1,modelosEdificios.fabCompAdv);
		vida.anadeTipoEdificio(tipoEdif4);
		TipoEdificio tipoEdif5 = new TipoEdificio("centralEnergiaAdv",habitats1,modelosEdificios.centralEnergiaAdv);
		vida.anadeTipoEdificio(tipoEdif5);
		
		/* Vegetales */
		EspecieVegetal especieV1 = new EspecieVegetal("seta",1000,50,50,50,0.1f,8,habitats1,0,modelosVegetales.setas);
		vida.anadeEspecieVegetal(especieV1);
		EspecieVegetal especieV2 = new EspecieVegetal("flor",1000,50,50,20,0.1f,15,habitats1,1,modelosVegetales.flores);
		vida.anadeEspecieVegetal(especieV2);
		EspecieVegetal especieV3 = new EspecieVegetal("caña",1000,50,50,20,0.1f,12,habitats1,2,modelosVegetales.canas);
		vida.anadeEspecieVegetal(especieV3);
		EspecieVegetal especieV4 = new EspecieVegetal("arbusto",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.arbustos);
		vida.anadeEspecieVegetal(especieV4);
		EspecieVegetal especieV5 = new EspecieVegetal("estromatolito",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.estromatolitos);
		vida.anadeEspecieVegetal(especieV5);
		EspecieVegetal especieV6 = new EspecieVegetal("cactus",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.cactus);
		vida.anadeEspecieVegetal(especieV6);
		EspecieVegetal especieV7 = new EspecieVegetal("palmera",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.palmeras);
		vida.anadeEspecieVegetal(especieV7);
		EspecieVegetal especieV8 = new EspecieVegetal("pino",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.pinos);
		vida.anadeEspecieVegetal(especieV8);
		EspecieVegetal especieV9 = new EspecieVegetal("ciprés",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.cipreses);
		vida.anadeEspecieVegetal(especieV9);
		EspecieVegetal especieV10 = new EspecieVegetal("pino alto",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.pinosAltos);
		vida.anadeEspecieVegetal(especieV10);
		
		/* Herbivoros */
		EspecieAnimal especieH1 = new EspecieAnimal("herbivoro1",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,modelosAnimales.herbivoro1);
		vida.anadeEspecieAnimal(especieH1);
		EspecieAnimal especieH2 = new EspecieAnimal("herbivoro2",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,modelosAnimales.herbivoro2);
		vida.anadeEspecieAnimal(especieH2);
		EspecieAnimal especieH3 = new EspecieAnimal("herbivoro3",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,modelosAnimales.herbivoro3);
		vida.anadeEspecieAnimal(especieH3);
		EspecieAnimal especieH4 = new EspecieAnimal("herbivoro4",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,modelosAnimales.herbivoro4);
		vida.anadeEspecieAnimal(especieH4);
		EspecieAnimal especieH5 = new EspecieAnimal("herbivoro5",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,T_habitats.hill,modelosAnimales.herbivoro5);
		vida.anadeEspecieAnimal(especieH5);
		
		/* Carnivoros */
		EspecieAnimal especieC1 = new EspecieAnimal("carnivoro1",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,modelosAnimales.carnivoro1);
		vida.anadeEspecieAnimal(especieC1);
		EspecieAnimal especieC2 = new EspecieAnimal("carnivoro2",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,modelosAnimales.carnivoro2);
		vida.anadeEspecieAnimal(especieC2);
		EspecieAnimal especieC3 = new EspecieAnimal("carnivoro3",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,modelosAnimales.carnivoro3);
		vida.anadeEspecieAnimal(especieC3);
		EspecieAnimal especieC4 = new EspecieAnimal("carnivoro4",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,modelosAnimales.carnivoro4);
		vida.anadeEspecieAnimal(especieC4);
		EspecieAnimal especieC5 = new EspecieAnimal("carnivoro5",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,T_habitats.hill,modelosAnimales.carnivoro5);
		vida.anadeEspecieAnimal(especieC5);	
	}	
}
