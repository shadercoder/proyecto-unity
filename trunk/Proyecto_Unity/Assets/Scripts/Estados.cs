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
	
	//Modelos
	public GameObject edificio1;										//El modelo del edificio numero 1
	public GameObject edificio2;										//El modelo del edificio numero 2
	public GameObject edificio3;										//El modelo del edificio numero 3
	public GameObject edificio4;										//El modelo del edificio numero 4
	public GameObject edificio5;										//El modelo del edificio numero 5
	public GameObject vegetal1;											//El modelo del vegetal numero 1
	public GameObject vegetal2;											//El modelo del vegetal numero 2
	public GameObject vegetal3;											//El modelo del vegetal numero 3
	public GameObject vegetal4;											//El modelo del vegetal numero 4
	public GameObject vegetal5;											//El modelo del vegetal numero 5
	public GameObject vegetal6;											//El modelo del vegetal numero 6
	public GameObject vegetal7;											//El modelo del vegetal numero 7
	public GameObject vegetal8;											//El modelo del vegetal numero 8
	public GameObject vegetal9;											//El modelo del vegetal numero 9
	public GameObject vegetal10;										//El modelo del vegetal numero 10
	public GameObject herbivoro1;										//El modelo del herbivoro numero 1
	public GameObject herbivoro2;										//El modelo del herbivoro numero 2
	public GameObject herbivoro3;										//El modelo del herbivoro numero 3
	public GameObject herbivoro4;										//El modelo del herbivoro numero 4
	public GameObject herbivoro5;										//El modelo del herbivoro numero 5
	public GameObject carnivoro1;										//El modelo del carnivoro numero 1
	public GameObject carnivoro2;										//El modelo del carnivoro numero 2
	public GameObject carnivoro3;										//El modelo del carnivoro numero 3
	public GameObject carnivoro4;										//El modelo del carnivoro numero 4
	public GameObject carnivoro5;										//El modelo del carnivoro numero 5
	//public GameObject[] modelos;										//Array con todos los modelos	
	
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
	private float cuantoW;							//Minima unidad de medida de la interfaz a lo ancho
	private float cuantoH;							//Minima unidad de medida de la interfaz a lo alto
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
		//meshTemp = FuncTablero.extruyeVertices(meshTemp, texturaBase, 0.5f, objetoRoca.transform.position);
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
		
		/* DATOS */
		/*modelos = new GameObject[25];
		modelos[0] = edificio1;
		modelos[1] = edificio2;
		modelos[2] = edificio3;
		modelos[3] = edificio4;
		modelos[4] = edificio5;
		modelos[5] = vegetal1;
		modelos[6] = vegetal2;
		modelos[7] = vegetal3;
		modelos[8] = vegetal4;
		modelos[9] = vegetal5;
		modelos[10] = vegetal6;
		modelos[11] = vegetal7;
		modelos[12] = vegetal8;
		modelos[13] = vegetal9;
		modelos[14] = vegetal10;
		modelos[15] = herbivoro1;
		modelos[16] = herbivoro2;
		modelos[17] = herbivoro3;
		modelos[18] = herbivoro4;
		modelos[19] = herbivoro5;
		modelos[20] = carnivoro1;
		modelos[21] = carnivoro2;
		modelos[22] = carnivoro3;
		modelos[23] = carnivoro4;
		modelos[24] = carnivoro5;*/
		
		List<T_habitats> habitats1 = new List<T_habitats>();
		habitats1.Add(T_habitats.plain);
		habitats1.Add(T_habitats.sand);
		habitats1.Add(T_habitats.coast);
		
		/* Edificios */
		TipoEdificio tipoEdif1 = new TipoEdificio("fabricaCompBas",habitats1,edificio1);
		vida.anadeTipoEdificio(tipoEdif1);
		TipoEdificio tipoEdif2 = new TipoEdificio("centralEnergia",habitats1,edificio2);
		vida.anadeTipoEdificio(tipoEdif2);
		TipoEdificio tipoEdif3 = new TipoEdificio("granja",habitats1,edificio3);
		vida.anadeTipoEdificio(tipoEdif3);
		TipoEdificio tipoEdif4 = new TipoEdificio("fabricaCompAdv",habitats1,edificio4);
		vida.anadeTipoEdificio(tipoEdif4);
		TipoEdificio tipoEdif5 = new TipoEdificio("centralEnergiaAdv",habitats1,edificio5);
		vida.anadeTipoEdificio(tipoEdif5);
		
		/* Vegetales */
		EspecieVegetal especieV1 = new EspecieVegetal("vegetal1",1000,50,50,50,0.1f,8,habitats1,0,vegetal1);
		vida.anadeEspecieVegetal(especieV1);
		EspecieVegetal especieV2 = new EspecieVegetal("vegetal2",1000,50,50,20,0.1f,15,T_habitats.mountain,1,vegetal2);
		vida.anadeEspecieVegetal(especieV2);
		EspecieVegetal especieV3 = new EspecieVegetal("vegetal3",1000,50,50,20,0.1f,12,T_habitats.hill,2,vegetal3);
		vida.anadeEspecieVegetal(especieV3);
		EspecieVegetal especieV4 = new EspecieVegetal("vegetal4",1000,50,50,20,0.1f,12,T_habitats.coast,3,vegetal4);
		vida.anadeEspecieVegetal(especieV4);
		EspecieVegetal especieV5 = new EspecieVegetal("vegetal5",1000,50,50,20,0.1f,12,T_habitats.plain,3,vegetal5);
		vida.anadeEspecieVegetal(especieV5);
		EspecieVegetal especieV6 = new EspecieVegetal("vegetal6",1000,50,50,20,0.1f,12,T_habitats.coast,3,vegetal6);
		vida.anadeEspecieVegetal(especieV6);
		EspecieVegetal especieV7 = new EspecieVegetal("vegetal7",1000,50,50,20,0.1f,12,T_habitats.plain,3,vegetal7);
		vida.anadeEspecieVegetal(especieV7);
		EspecieVegetal especieV8 = new EspecieVegetal("vegetal8",1000,50,50,20,0.1f,12,T_habitats.coast,3,vegetal8);
		vida.anadeEspecieVegetal(especieV8);
		EspecieVegetal especieV9 = new EspecieVegetal("vegetal9",1000,50,50,20,0.1f,12,T_habitats.hill,3,vegetal9);
		vida.anadeEspecieVegetal(especieV9);
		EspecieVegetal especieV10 = new EspecieVegetal("vegetal10",1000,50,50,20,0.1f,12,T_habitats.mountain,3,vegetal10);
		vida.anadeEspecieVegetal(especieV10);
		
		/* Herbivoros */
		EspecieAnimal especieH1 = new EspecieAnimal("herbivoro1",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,herbivoro1);
		vida.anadeEspecieAnimal(especieH1);
		EspecieAnimal especieH2 = new EspecieAnimal("herbivoro2",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,herbivoro2);
		vida.anadeEspecieAnimal(especieH2);
		EspecieAnimal especieH3 = new EspecieAnimal("herbivoro3",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,herbivoro3);
		vida.anadeEspecieAnimal(especieH3);
		EspecieAnimal especieH4 = new EspecieAnimal("herbivoro4",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,herbivoro4);
		vida.anadeEspecieAnimal(especieH4);
		EspecieAnimal especieH5 = new EspecieAnimal("herbivoro5",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,T_habitats.hill,herbivoro5);
		vida.anadeEspecieAnimal(especieH5);
		
		/* Carnivoros */
		EspecieAnimal especieC1 = new EspecieAnimal("carnivoro1",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,carnivoro1);
		vida.anadeEspecieAnimal(especieC1);
		EspecieAnimal especieC2 = new EspecieAnimal("carnivoro2",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,carnivoro2);
		vida.anadeEspecieAnimal(especieC2);
		EspecieAnimal especieC3 = new EspecieAnimal("carnivoro3",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,carnivoro3);
		vida.anadeEspecieAnimal(especieC3);
		EspecieAnimal especieC4 = new EspecieAnimal("carnivoro4",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,carnivoro4);
		vida.anadeEspecieAnimal(especieC4);
		EspecieAnimal especieC5 = new EspecieAnimal("carnivoro5",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,T_habitats.hill,carnivoro5);
		vida.anadeEspecieAnimal(especieC5);	
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
	
	//Funciones OnGUI---------------------------------------------------------------------------------------------------------------------------
	
	void OnGUIXXX() {
		GUI.skin = estiloGUI;
		switch (estado) {
			case T_estados.inicial:
				GUI.skin = estiloGUI;
				GUI.Box(new Rect(cuantoW * 21, cuantoH * 14, cuantoW * 6, cuantoH * 2), "Re-Generando!");
				break;
			case T_estados.opciones:
				GUI.skin = estiloGUI;
				menuOpciones();
				break;
			case T_estados.principal:
				GUI.skin = estiloGUI_Nuevo;
				menuIzquierdaHex();
				if (objetivoAlcanzado) {
					ventanaInfo();
				}
				break;
			case T_estados.guardar:
				GUI.skin = estiloGUI;
				menuGuardar();
				break;
			case T_estados.reparaciones:
				GUI.skin = estiloGUI;
				menuReparaciones();
				break;
			default:						
				break;
		}
		
		//Control pincel en textura
		//TODO alguna forma de evitar el raycast cuando colisione con la GUI
		if (activarPinceles && Input.GetMouseButton(0) && ((Event.current.type == EventType.MouseDown) || (Event.current.type == EventType.MouseDrag))) {
			StartCoroutine(corutinaPincel());
		}
		
		//Debug del algoritmo
		if(GUI.Button(new Rect(cuantoW * 40, cuantoH * 24,cuantoW * 8,cuantoH * 1), "Activar/Desactivar") || Input.GetKeyDown(KeyCode.V)) 
			if(algoritmoActivado)
				algoritmoActivado = false;
			else
				algoritmoActivado = true;
		
		//Informacion del debug del algoritmo
		if(GUI.Button(new Rect(cuantoW * 40, cuantoH * 23,cuantoW * 8,cuantoH * 1), "Introducir animal")) 			
		{
			int x=0,y=0;
			vida.buscaPosicionVaciaAnimal(T_habitats.plain,ref x,ref y);
			EspecieAnimal especie = new EspecieAnimal("comemusgo"+vida.numEspeciesAnimales,10,1000,0,5,5,5,tipoAlimentacionAnimal.herbivoro,T_habitats.plain, carnivoro1);//GameObject.FindGameObjectWithTag("velociraptor"));			
			vida.anadeEspecieAnimal(especie);						
			vida.anadeAnimal(especie,x,y);	
			Debug.Log("Introducido animal "+especie.nombre+" en la posicion:   "+"x: "+x+"   y: "+y);		
		}
		GUI.Box(new Rect(cuantoW * 40, cuantoH * 25,cuantoW * 8,cuantoH * 6),new GUIContent("Algoritmo Especies","debug"));
		GUI.Label(new Rect(cuantoW * 41, cuantoH * 26,cuantoW * 7,cuantoH * 2),"Num vegetales: "+vida.vegetales.Count);
		GUI.Label(new Rect(cuantoW * 41, cuantoH * 27,cuantoW * 7,cuantoH * 2),"Num animales: "+vida.animales.Count);
		GUI.Label(new Rect(cuantoW * 41, cuantoH * 28,cuantoW * 7,cuantoH * 2),"Num pasos: "+numPasos);
			
		
		//Info de la casilla
		//Especies
		if (infoEspecies) {
			infoCasillaEspecie();
		}
		//Elementos
		else if (infoElems) {
			infoCasillaElems();
		}		
		
		//Tooltip
		if (activarTooltip) {
			float longitud = GUI.tooltip.Length;
			if (longitud == 0.0f) {
				return;
			}
			else {
				longitud *= 8.5f;
			}
			float posx = Input.mousePosition.x;
			float posy = Input.mousePosition.y;
			if (posx > (Screen.width / 2)) {
				posx -= 215;
			}
			else {
				posx += 15;
			}
			if (posy > (Screen.height / 2)) {
				posy += 20;
			}		
			Rect pos = new Rect(posx, Screen.height - posy, longitud, 25);
			GUI.Box(pos, "");
			GUI.Label(pos, GUI.tooltip);
		}		
	}
	
	private void menuIzquierdaHex() {
		barraInformacion(new Rect(cuantoW,cuantoH,cuantoW * 8, cuantoH * 3));
		escalaTiempo = sliderTiempoCompuesto(new Rect(cuantoW,cuantoH * 4, cuantoW * 8, cuantoH * 3), escalaTiempo);
		botonHexCompuestoAltera(new Rect(cuantoW, cuantoH * 7, cuantoW * 8, cuantoH * 3));
		botonHexCompuestoCamara(new Rect(cuantoW, cuantoH * 10, cuantoW * 8, cuantoH * 3));
		botonHexCompuestoOpciones(new Rect(cuantoW, cuantoH * 13, cuantoW * 8, cuantoH * 3));
		GUI.Box(new Rect(cuantoW, cuantoH * 16, cuantoW * 8, cuantoH * 3), "", "BarraAbajo");
	}
	
	private void menuOpciones() {
		Control_Raton script;
		GUILayout.BeginArea(new Rect(cuantoW * 20, cuantoH * 12, cuantoW * 8, cuantoH * 6));
		GUILayout.BeginVertical();
		if (GUILayout.Button(new GUIContent("Salir", "Salir del juego"), "boton_menu_1")) {
			Time.timeScale = 1.0f;
			script = transform.parent.GetComponent<Control_Raton>();
			script.setInteraccion(true);
			estado = T_estados.salir;
		}
		if (GUILayout.Button(new GUIContent("Guardar", "Guardar la partida"), "boton_menu_2")) {
			Time.timeScale = 1.0f;
			nombresSaves = SaveLoad.getFileNames();
			estado = T_estados.guardar;
		}
		if (GUILayout.Button(new GUIContent("Volver", "Volver al juego"), "boton_menu_4")) {
			Time.timeScale = 1.0f;
			script = transform.parent.GetComponent<Control_Raton>();
			script.setInteraccion(true);
			estado = T_estados.principal;
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	private void menuGuardar() {
		Control_Raton script;
		GUI.Box(new Rect(cuantoW * 14, cuantoH * 7, cuantoW * 20, cuantoH * 16), "");
		posicionScroll = GUI.BeginScrollView(new Rect(cuantoW * 14, cuantoH * 8, cuantoW * 20, cuantoH * 14), posicionScroll, new Rect(0, 0, cuantoW * 20, cuantoH * 4 * numSavesExtra));
		if (GUI.Button(new Rect(cuantoW, 0, cuantoW * 18, cuantoH * 4), new GUIContent("Nueva partida salvada", "Guardar una nueva partida"))) {
			ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
			string fecha = System.DateTime.Now.ToString().Replace("\\","").Replace("/","").Replace(" ", "").Replace(":","");
			SaveLoad.cambiaFileName("Partida" + fecha + ".hur");
			int tempLong = temp.texturaBase.width * temp.texturaBase.height;
			float[] data = new float[tempLong];
			Color[] pixels = temp.texturaBase.GetPixels();
			for (int i = 0; i < tempLong; i++) {
				data[i] = pixels[i].r;
			}			
			SaveLoad.Save(data,temp.texturaBase.width, temp.texturaBase.height);
			//Recuperar estado normal
			Time.timeScale = 1.0f;
			script = transform.parent.GetComponent<Control_Raton>();
			script.setInteraccion(true);
			estado = T_estados.principal;
		}
		for (int i = 0; i < numSaves; i++) {
			if (GUI.Button(new Rect(cuantoW, (i + 1) * cuantoH * 4, cuantoW * 18, cuantoH * 4), new GUIContent(nombresSaves[i], "Sobreescribir partida num. " + i))) {
				ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
				SaveLoad.cambiaFileName(nombresSaves[i]);		
				SaveLoad.Save(temp.texturaBase);
				//Recuperar estado normal
				Time.timeScale = 1.0f;
				script = transform.parent.GetComponent<Control_Raton>();
				script.setInteraccion(true);
				estado = T_estados.principal;
			}
		}
		GUI.EndScrollView();
		if (GUI.Button(new Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), new GUIContent("Volver", "Volver a la partida"), "boton_atras")) {
			//Recuperar estado normal
			Time.timeScale = 1.0f;
			escalaTiempo = 1.0f;
			script = transform.parent.GetComponent<Control_Raton>();
			script.setInteraccion(true);
			estado = T_estados.principal;
		}
		
	}
	
	private void menuReparaciones() {
		if (GUI.Button(new Rect(cuantoW, cuantoH * 20, cuantoW * 2, cuantoH), new GUIContent("Volver", "boton_atras"))) {
			camaraPrincipal.GetComponent<Camera>().enabled = true;
			camaraReparaciones.GetComponent<Camera>().enabled = false;
			Control_Raton script = transform.parent.GetComponent<Control_Raton>();
			script.setInteraccion(true);
			estado = T_estados.principal;
		}
	}
	
	private void botonHexCompuestoAltera(Rect pos) {
		
		GUI.Box(new Rect(pos.x,pos.y,pos.width - (pos.width * 3.9f / 12.0f), pos.height), new GUIContent("", "Opciones para alterar el planeta"), "BarraHexGran");
		if (menuAltera)
			GUI.Box(new Rect(pos.x + (pos.width * 7.0f / 12.0f),pos.y + (pos.height * 0.75f / 5.0f),pos.width - (pos.width * 8.1f / 12.0f), pos.height), new GUIContent("", "Opciones (P) para alterar el planeta"), "BarraHexPeq");
		if (GUI.Button(new Rect(pos.x + (pos.width * 3.5f / 12.0f), pos.y + (pos.height * 0.6f / 5.0f), pos.width - (pos.width * 8.3f / 12.0f), pos.height - (pos.height * 0.9f / 5.0f)),"", "BotonGrandeAlterar")) {
			menuAltera = true;
			menuCamara = false;
			menuOpcion = false;
		}
		if (menuAltera) {
			botonPequeSubir = GUI.Toggle(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 0.95f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), botonPequeSubir, new GUIContent("", "Eleva el terreno pulsado"), "BotonPequeSubir");
			if (GUI.changed && botonPequeSubir) {
				botonPequeBajar = false;
				botonPequeAllanar = false;
			}
			botonPequeBajar = GUI.Toggle(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 1.85f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), botonPequeBajar, new GUIContent("", "Hunde el terreno pulsado"), "BotonPequeBajar");
			if (GUI.changed && botonPequeBajar) {
				botonPequeSubir = false;
				botonPequeAllanar = false;
			}
			botonPequeAllanar = GUI.Toggle(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 2.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), botonPequeAllanar, new GUIContent("", "Allana el terreno pulsado"), "BotonPequeAllanar");
			if (GUI.changed && botonPequeAllanar) {
				botonPequeSubir = false;
				botonPequeBajar = false;
				Debug.Log("Pulsado boton allanar. Sin funcionalidad aun.");
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 3.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("", "Selecciona el pincel para el terreno"), "BotonPequePinceles")) {
				botonPequePincel = true;
			}
			if (botonPequePincel) {
				seleccionPincel = cajaSeleccionPincel(seleccionPincel);
				if (GUI.changed) {
					botonPequePincel = false;
				}
			}
		}
	}
	
	private void botonHexCompuestoCamara(Rect pos) {
		Control_Raton script;
		GUI.Box(new Rect(pos.x,pos.y,pos.width - (pos.width * 3.9f / 12.0f), pos.height), new GUIContent("", "Opciones para cambiar la camara"), "BarraHexGran");
		if (menuCamara)
			GUI.Box(new Rect(pos.x + (pos.width * 7.0f / 12.0f),pos.y + (pos.height * 0.75f / 5.0f),pos.width - (pos.width * 8.1f / 12.0f), pos.height), new GUIContent("", "Opciones (P) para cambiar la camara"), "BarraHexPeq");
		if (GUI.Button(new Rect(pos.x + (pos.width * 3.5f / 12.0f), pos.y + (pos.height * 0.6f / 5.0f), pos.width - (pos.width * 8.3f / 12.0f), pos.height - (pos.height * 0.9f / 5.0f)), "", "BotonGrandeCamara")) {
			menuAltera = false;
			menuCamara = true;
			menuOpcion = false;
			botonPequeSubir = false;
			botonPequeBajar = false;
			botonPequeAllanar = false;
		}
		if (menuCamara) {
			if (GUI.Button(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 0.95f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("", "Vista del planeta"), "BotonPequePlaneta")) {
				Debug.Log("Pulsado peque camara 1-4");
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 1.85f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("", "Vista de la nave"), "BotonPequeNave")) {
				camaraPrincipal.GetComponent<Camera>().enabled = false;
				camaraReparaciones.GetComponent<Camera>().enabled = true;
				script = transform.parent.GetComponent<Control_Raton>();
				script.setInteraccion(false);
				estado = T_estados.reparaciones;
			}
			infoEspecies = GUI.Toggle(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 2.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), infoEspecies, new GUIContent("", "Vista de las especies"), "BotonPequeEspecies");
			if (GUI.changed && infoEspecies) {
				//Activar información de las especies de la casilla
				infoElems = false;
			}
			infoElems = GUI.Toggle(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 3.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), infoElems, new GUIContent("", "Vista de los elementos"), "BotonPequeCristales");
			if (GUI.changed && infoElems) {	
				//Activar información de los elementos de la casilla
				infoEspecies = false;
			}
		}
	}
	
	private void botonHexCompuestoOpciones(Rect pos) {
		Control_Raton script;
		GUI.Box(new Rect(pos.x,pos.y,pos.width - (pos.width * 3.9f / 12.0f), pos.height), new GUIContent("", "Opciones generales"), "BarraHexGran");
		if (menuOpcion)
			GUI.Box(new Rect(pos.x + (pos.width * 7.0f / 12.0f),pos.y + (pos.height * 0.75f / 5.0f),pos.width - (pos.width * 8.1f / 12.0f), pos.height), new GUIContent("", "Opciones (P) generales"), "BarraHexPeq");
		if (GUI.Button(new Rect(pos.x + (pos.width * 3.5f / 12.0f), pos.y + (pos.height * 0.6f / 5.0f), pos.width - (pos.width * 8.3f / 12.0f), pos.height - (pos.height * 0.9f / 5.0f)), "Opciones", "BotonGrandeOpciones")) {
			menuAltera = false;
			menuCamara = false;
			menuOpcion = true;
			botonPequeSubir = false;
			botonPequeBajar = false;
			botonPequeAllanar = false;
		}
		if (menuOpcion) {
			if (GUI.Button(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 0.95f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("Peq1", "Opciones (P)(B) generales"), "BotonVacio")) {
				Debug.Log("Pulsado peque opciones 1-4");
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 1.85f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("Guardar", "Guardar la partida actual"), "BotonVacio")) {
				Time.timeScale = 1.0f;
				nombresSaves = SaveLoad.getFileNames();
				estado = T_estados.guardar;
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 2.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("Volver", "Volver al juego con normalidad."), "BotonVacio")) {
				Debug.Log("Pulsado peque opciones 3-4");
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 3.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("Salir", "Salir al menu del juego."), "BotonVacio")) {
				Time.timeScale = 1.0f;
				script = transform.parent.GetComponent<Control_Raton>();
				script.setInteraccion(true);
				estado = T_estados.salir;
			}
		}
	}
	
	private float sliderTiempoCompuesto(Rect pos, float valor) {
		float valorOut;
		GUI.Box(pos, new GUIContent("", "En que tiempo nos encontramos"), "BarraSlider");
		valorOut = GUI.HorizontalSlider(new Rect(pos.x + (pos.width * 3.3f / 12.0f), pos.y + (pos.height * 1.7f / 5.0f), pos.width - (pos.width * 4.3f / 12.0f), pos.height - (pos.height * 2.0f / 5.0f)), valor, 0.2f, 99.9f);
		GUI.Label(new Rect(pos.x + (pos.width * 4.3f / 12.0f), pos.y + (pos.height * 2.9f / 5.0f), pos.width - (pos.width * 4.3f / 12.0f), pos.height - (pos.height * 2.0f / 5.0f)), "Escala temporal: " + Time.timeScale.ToString("0#.0"));
		return valorOut;
	}
	
	private void barraInformacion(Rect pos) {
		GUI.Box(pos, new GUIContent("", "Tiempo en el que te encuentras"), "BarraTiempo");
		string temp = Time.time.ToString("#.0");
		GUI.Label(new Rect(pos.x + (pos.width * 7.0f / 12.0f), pos.y + (pos.height * 2.0f / 5.0f), (pos.width * 3.0f / 12.0f), (pos.height * 2.0f / 5.0f)), temp);
	}
	
	private int cajaSeleccionPincel(int entrada) {
		GUI.Box(new Rect(cuantoW * 18, cuantoH * 25, cuantoW * 12.5f, cuantoH * 5), "", "CajaPinceles");
		if (GUI.Button(new Rect(cuantoW * 19.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelCrater")) {
			GUI.changed = true;
			return 0;
		}
		if (GUI.Button(new Rect(cuantoW * 21.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelVolcan")){
			GUI.changed = true;
			return 1;
		}
		if (GUI.Button(new Rect(cuantoW * 23.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelDuro")){
			GUI.changed = true;
			return 2;
		}
		if (GUI.Button(new Rect(cuantoW * 25.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelSuave")){
			GUI.changed = true;
			return 3;
		}
		if (GUI.Button(new Rect(cuantoW * 27.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelIrregular")){
			GUI.changed = true;
			return 4;
		}
		return entrada;
	}
	
	private void infoCasillaEspecie() {
		GUI.Box(new Rect(cuantoW * 40, cuantoH * 20, cuantoW * 8, cuantoH * 4), new GUIContent("", "Informacion de la casilla"));
		GUI.Label(new Rect(cuantoW * 40, cuantoH * 20.2f, cuantoW * 8, cuantoH * 1.6f), infoEspecies_Hab);
		GUI.Label(new Rect(cuantoW * 40, cuantoH * 22.0f, cuantoW * 8, cuantoH * 1.6f), infoEspecies_Esp);
	}
	
	private void infoCasillaElems() {
		GUI.Box(new Rect(cuantoW * 40, cuantoH * 20, cuantoW * 8, cuantoH * 4), new GUIContent("", "Informacion de la casilla"));
		GUI.Label(new Rect(cuantoW * 40, cuantoH * 20.2f, cuantoW * 8, cuantoH * 1.6f), infoElems_Elem);
	}
	
	private void ventanaInfo() {
		GUI.Box(new Rect(cuantoW * 16, cuantoH * 10, cuantoW * 16, cuantoH * 10), "");
		GUI.Label(new Rect(cuantoW * 17, cuantoH * 11, cuantoW * 5, cuantoH * 5), new GUIContent("Imagen", "Imagen del Edificio/Animal/Planta"));
		GUI.Label(new Rect(cuantoW * 17, cuantoH * 16, cuantoW * 14, cuantoH * 4), infoSeleccionado);
		if (GUI.Button(new Rect(cuantoW * 31, cuantoH * 10, cuantoW * 1, cuantoH * 1), "X")) {
			objetivoAlcanzado = false;
		}
	}
}
