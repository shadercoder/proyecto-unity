using UnityEngine;
using System.Collections;

public class EscenaCarga : MonoBehaviour {

	//Variables del script
	private int estado 						= 0;				//0 para menu, 1 para comenzar, 2 para opciones, 3 para creditos, 4 para salir
	
	//Variables de la creacion
	private GameObject contenedorTexturas;						//Aqui se guardan las texturas que luego se usarán en el planeta
	public Texture2D texturaBase;								//La textura visible que vamos a inicializar durante la creacion de un planeta nuevo
	private Color[] pixels;										//Los pixeles sobre los que realizar operaciones
	
	private int faseCreacion 				= 0;				//Fases de la creacion del planeta
	private bool trabajando 				= false;			//Para saber si está haciendo algo por debajo el script
	private float progreso					= 0.0f;				//El progreso entre 0 y 1 del trabajo
	private bool paso1Completado 			= false;			//Si se han completado los pasos suficientes para pasar a la siguiente fase
	
		//Primera fase
	private float gananciaInit 				= 0.5f;				//La ganancia a pasar al script de creación del ruido
	private float escalaInit 				= 0.003f;			//La escala a pasar al script de creación del ruido
	private float lacunaridadInit			= 3.51284702f;		//La lacunaridad a pasar al script de creacion del ruido
	private float octavasFloat				= 6.0f;				//Las octavas a pasar al script de creacion del ruido
	
		//Segunda fase
	public GameObject objetoRoca;
	public Mesh meshEsfera;										//La esfera sobre la que se harán los cambios
	private Mesh aguaMesh;										//El objeto con el Mesh extruido del agua
	private Mesh rocaMesh;										//El objeto con el Mesh extruido de la roca
	private float tamanoPlayasInit			= 0.035f;			//Entre 0.02 y 0.05 El tamaño de las playas
	private float nivelAguaInit 			= 0.3f;				//El punto a partir del cual deja de haber mar en la orografía del planeta
	private float temperaturaInit 			= 0.5f;				//Entre 0.0 y 1.0, la temperatura del planeta, que modificará la paleta.
			//Planeta
	private string nombrePlaneta			= "";				//El nombre del planeta al que se viaja
	private string nombreEstrella			= "";				//El nombre de la estrella sobre la que orbita el planeta al que se viaja
	
		//Tercera fase
	public Texture2D texElems;									//Textura donde se representan los elementos del terreno
	public Texture2D texPlantas;								//Textura donde se pintan las plantas del planeta
	public Texture2D texHabitatsEstetica;						//Textura donde se pintan los habitats a mostrar
	public Texture2D texHabitats;								//Textura donde se pintan los habitats para filtros
	private Vida vida;											//El algoritmo de vida
	
	//Opciones
	private bool musicaOn 					= true;				//Está la música activada?
	private float musicaVol 				= 0.5f;				//A que volumen?
	private bool sfxOn 						= true;				//Estan los efectos de sonido activados?
	private float sfxVol 					= 0.5f; 			//A que volumen?
	
	//Variables de conveniencia
	private Transform miObjeto;									//Guarda la posicion del objeto para ahorrar calculos
	
	private string cadenaCreditos = "\t Hurricane son: \n Marcos Calleja Fern\u00e1ndez\n Aris Goicoechea Lassaletta\n Pablo Pizarro Mole\u00f3n\n" + 
											"\n\t M\u00fasica a cargo de:\n Easily Embarrased\n Frost-RAVEN";
																//Cadena con los créditos a mostrar
	
	//Tooltips
	private Vector3 posicionMouse 			= Vector3.zero;		//Guarda la ultima posicion del mouse		
	private bool activarTooltip 			= false;			//Controla si se muestra o no el tooltip	
	private float ultimoMov 				= 0.0f;				//Ultima vez que se movio el mouse		
	public float tiempoTooltip 				= 0.75f;			//Tiempo que tarda en aparecer el tooltip	
	
	//Interfaz
	public GUISkin estiloGUI;										//Los estilos a usar para la escena de carga y menús
	private int cuantoW 					= Screen.width / 48;	//Minima unidad de medida de la interfaz a lo ancho (formato 16/10)
	private int cuantoH 					= Screen.height / 30;	//Minima unidad de medida de la interfaz a lo alto (formato 16/10)
	
	//Menus para guardar
	private Vector2 posicionScroll 			= Vector2.zero;			//La posicion en la que se encuentra la ventana con scroll
	private int numSaves 					= 0;					//El numero de saves diferentes que hay en el directorio respectivo
	private string[] nombresSaves;									//Los nombres de los ficheros de savegames guardados
	private SaveData saveGame;										//El contenido de la partida salvada cargada
	
	//Nave
	public GameObject nave;
	
	
	//Funciones basicas ----------------------------------------------------------------------------------------------------------------------
	
	void Awake() {
		Debug.Log (FuncTablero.formateaTiempo() + ": Iniciando el metodo Awake() de la escena inicial...");
		miObjeto = this.transform;
		GameObject[] cadena = GameObject.FindGameObjectsWithTag("Carga");
		if (cadena.Length > 1) {
			contenedorTexturas = cadena[0];
			DontDestroyOnLoad(cadena[0]);
			for (int i = 1; i < cadena.Length; i++) {
				Destroy(cadena[i]);
			}
		}
		else {
			contenedorTexturas = cadena[0];
			DontDestroyOnLoad(cadena[0]);
		}
		if (PlayerPrefs.HasKey("MusicaOn")) {
			if (PlayerPrefs.GetInt("MusicaOn") == 1)
				musicaOn = true;
			else
				musicaOn = false;
		}
		if (PlayerPrefs.HasKey("MusicaVol")) {
			musicaVol = PlayerPrefs.GetFloat("MusicaVol");
		}
		if (PlayerPrefs.HasKey("SfxOn")) {
			if (PlayerPrefs.GetInt("SfxOn") == 1)
				sfxOn = true;
			else
				sfxOn = false;
		}
		if (PlayerPrefs.HasKey("SfxVol")) {
			sfxVol = PlayerPrefs.GetFloat("SfxVol");
		}
		SaveLoad.compruebaRuta();
		numSaves = SaveLoad.FileCount();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames();
		objetoRoca.renderer.sharedMaterials[1].SetFloat("_nivelMar", nivelAguaInit);
		objetoRoca.renderer.sharedMaterials[1].SetFloat("_tamPlaya", tamanoPlayasInit);
		Debug.Log (FuncTablero.formateaTiempo() + ": Completado el metodo Awake().");
	}
	
	void Update() {
		//Tooltip
		if (Input.mousePosition != posicionMouse) {
			posicionMouse = Input.mousePosition;
			activarTooltip = false;
			ultimoMov = Time.time;
		}
		else {
			if (Time.time >= ultimoMov + tiempoTooltip) {
				activarTooltip = true;
			}
		}
		//animacion idle de la nave
		naveIdle();
	}
	
	void FixedUpdate() {
		AudioSource opSonido = miObjeto.GetComponent<AudioSource>();
		opSonido.volume = musicaVol;
		if (!musicaOn && opSonido.isPlaying)
			opSonido.Pause();
		else if (musicaOn && !opSonido.isPlaying)
			opSonido.Play();	
	}
	
	void OnGUI() {
		GUI.skin = estiloGUI;
		GUI.Box(new Rect(cuantoW * 16, cuantoH, cuantoW * 16, cuantoH * 5), "", "header_titulo"); //Header es 500x100px
		switch (estado) {
			case 0: 	//Menu principal
				menuPrincipal();
				break;
			case 1:		//Comenzar
				Debug.Log (FuncTablero.formateaTiempo() + ": Iniciando la carga de la siguiente escena...");
				ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
				temp.texturaBase = texturaBase;
				temp.texturaBase.Apply();
				temp.roca = rocaMesh;
				temp.agua = aguaMesh;
				temp.nivelAgua = nivelAguaInit;
				temp.tamanoPlaya = tamanoPlayasInit;
				temp.texturaElementos = texElems;
				temp.texturaHabitats = texHabitats;
				temp.texturaHabsEstetica = texHabitatsEstetica;
				temp.texturaPlantas = texPlantas;
				temp.vida = vida;
				Debug.Log (FuncTablero.formateaTiempo() + ": Valores cargados correctamente. Iniciando carga de nivel...");
				Application.LoadLevel("Escena_Principal");
				break;
			case 2:		//Opciones
				menuOpciones();
				if (GUI.changed) {
					actualizarOpciones();
				}
				break;
			case 3:		//Creditos
				creditos();
				break;
			case 4:		//Salir
				PlayerPrefs.Save();
				Application.Quit();
				break;
			case 5:		//Creacion
				if (faseCreacion == 0)
					creacionParte1Interfaz();
				else if (faseCreacion == 1)
					creacionParte2Interfaz();
				else if (faseCreacion == 2)
					creacionParte3Interfaz();
				break;
			case 6:		//Cargar (seleccion)
				menuCargar();
				break;
			case 7:		//Cargar (el juego seleccionado)
//				cargarJuego();
				Application.LoadLevel("Escena_Principal");
				break;		
		}
		if (trabajando) {
			barraProgreso();
		}
		
		//Tooltip
		mostrarTooltip();
	}
	
	//Funciones auxiliares --------------------------------------------------------------------------------------------------------------------
	
	private void mostrarTooltip() {
		if (activarTooltip) {
			//Muestra el tooltip si ha sido activado
			float longitud = GUI.tooltip.Length;
			if (longitud == 0.0f) 
				return;			
			else {
				if (longitud < 8)
					longitud *= 10.0f;
				else if (longitud < 15)
					longitud *= 9.0f;
				else
					longitud *= 8.75f;
			}
						
			float posx = Input.mousePosition.x;
			float posy = Input.mousePosition.y;
			if (posx > (Screen.width / 2)) 
				posx -= (longitud + 20);			
			else 
				posx += 15;				
			if (posy > (Screen.height / 2)) 
				posy -= 10;
			else 
				posy += 5;
			Rect pos = new Rect(posx, Screen.height - posy, longitud, 25);
			GUI.Box(pos, "");
			GUI.Label(pos, GUI.tooltip);					
		}
	}
	
	private void actualizarOpciones() {
		if (musicaOn)
			PlayerPrefs.SetInt("MusicaOn", 1);
		else
			PlayerPrefs.SetInt("MusicaOn", 0); 
		PlayerPrefs.SetFloat("MusicaVol", musicaVol);
		if (sfxOn)
			PlayerPrefs.SetInt("SfxOn", 1);
		else
			PlayerPrefs.SetInt("SfxOn", 0);
		PlayerPrefs.SetFloat("SfxVol", sfxVol);
	}
	
	private IEnumerator creacionParte1() {
		trabajando = true;
		progreso = 0.0f;
		GUI.enabled = false;
		Debug.Log (FuncTablero.formateaTiempo() + ": Iniciando creacionParte1()...");
		yield return new WaitForSeconds(0.1f);
		progreso = 0.1f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Creando ruido...");
		yield return new WaitForSeconds(0.01f);
		pixels = FuncTablero.ruidoTextura();										//Se crea el ruido para la textura base y normales...
		progreso = 0.7f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completado. Suavizando borde...");
		yield return new WaitForSeconds(0.01f);
		pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);		//Se suaviza el borde lateral...
		progreso = 0.8f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completado. Suavizando polos...");
		yield return new WaitForSeconds(0.01f);
		pixels = FuncTablero.suavizaPoloTex(pixels);								//Se suavizan los polos...
		progreso = 0.9f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completado. Aplicando cambios...");
		yield return new WaitForSeconds(0.01f);
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();
		progreso = 1.0f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completada creacionParte1().");
		yield return new WaitForSeconds(0.01f);
		progreso = 0.0f;
		trabajando = false;
		GUI.enabled = true;
		paso1Completado = true;
	}
	
	private IEnumerator creacionParte2() {
		trabajando = true;
		progreso = 0.0f;
		GUI.enabled = false;
		Debug.Log (FuncTablero.formateaTiempo() + ": Iniciando creacionParte2().");
		yield return new WaitForSeconds(0.1f);
		progreso = 0.1f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Instanciando la esfera roca...");
		yield return new WaitForSeconds(0.01f);
		Mesh meshTemp = GameObject.Instantiate(meshEsfera) as Mesh;
		progreso = 0.2f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Extruyendo vertices de roca...");
		yield return new WaitForSeconds(0.01f);
		meshTemp = FuncTablero.extruyeVerticesTex(meshTemp, texturaBase, 0.45f, new Vector3(0.0f, 0.0f, 0.0f));
		progreso = 0.6f;
		yield return new WaitForSeconds(0.01f);
		rocaMesh = meshTemp;
		Debug.Log (FuncTablero.formateaTiempo() + ": Instanciando la esfera agua...");
		yield return new WaitForSeconds(0.01f);
		Mesh meshAgua = GameObject.Instantiate(meshEsfera) as Mesh;
		progreso = 0.7f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Extruyendo vertices de agua...");
		yield return new WaitForSeconds(0.01f);
		meshAgua = FuncTablero.extruyeVerticesValor(meshAgua, nivelAguaInit, 0.45f, new Vector3(0.0f, 0.0f, 0.0f));
		aguaMesh = meshAgua;
		progreso = 1.0f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completada creacionParte2().");
		yield return new WaitForSeconds(0.01f);
		nombrePlaneta = "PL" + Random.Range(1, 50000).ToString();
		nombreEstrella = "SP" + Random.Range(200, 6000).ToString();
		progreso = 0.0f;
		trabajando = false;
		GUI.enabled = true;
		faseCreacion = 2;
	}
	
	private IEnumerator creacionParte3() {
		trabajando = true;
		progreso = 0.0f;
		GUI.enabled = false;
		Debug.Log (FuncTablero.formateaTiempo() + ": Iniciando creacionParte3().");
		Debug.Log (FuncTablero.formateaTiempo() + ": Creando el tablero...");
		yield return new WaitForSeconds(0.1f);
		Casilla[,] tablero = FuncTablero.iniciaTablero(texturaBase, texHabitats, texHabitatsEstetica, texElems, rocaMesh, Vector3.zero);
		progreso = 0.7f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Creando la vida...");
		yield return new WaitForSeconds(0.01f);
		vida = new Vida(tablero, texPlantas);
		progreso = 1.0f;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completado creacionParte3().");
		yield return new WaitForSeconds(0.01f);
		progreso = 0.0f;
		GUI.enabled = true;
		trabajando = false;
		faseCreacion = 0;
		estado = 1;	
	}
	
//	private void cargarJuego() {		
//		int w = saveGame.heightmapW;
//		int h = saveGame.heightmapH;
//		Color[] pixels = new Color[w * h];
//		for (int i = 0; i < w * h; i++) {
//			float temp = saveGame.heightmapData[i];
//			pixels[i] = new Color(temp, temp, temp);
//		}	
//		if (texturaBase.width != w || texturaBase.height != h) {
//			Debug.LogError("Las dimensiones de las texturas no coinciden!");
//		}
//		texturaBase.SetPixels(pixels);
//		texturaBase.Apply();
//		estado = 1;
//	}	
	
	//Menus personalizados --------------------------------------------------------------------------------------------------------------------
	
	private void menuPrincipal() {
		GUILayout.BeginArea(new Rect((float)cuantoW * 20.5f, cuantoH * 20, cuantoW * 7, cuantoH * 5));
		GUILayout.BeginVertical();
		if (GUILayout.Button(new GUIContent("Comenzar juego", "Comenzar un juego nuevo"), "boton_menu_1")) {
			pixels = new Color[texturaBase.width * texturaBase.height];
			FuncTablero.inicializa(texturaBase);
			faseCreacion = 0;
			paso1Completado = false;
//			objetoRoca.renderer.enabled = true;
			objetoRoca.animation.Play("HologramaAparecer");
			Camera.main.animation.Play("AcercarseHolograma");
			estado = 5;
		}
		if (GUILayout.Button(new GUIContent("Cargar", "Cargar un juego guardado"), "boton_menu_2")) {
			estado = 6;
		}
		if (GUILayout.Button(new GUIContent("Opciones", "Acceder a las opciones"), "boton_menu_3")) {
			Camera.main.animation.Play("AcercarsePantalla");
			estado = 2;
		}
		if (GUILayout.Button(new GUIContent("Cr\u00e9ditos", "Visualiza los cr\u00e9ditos"), "boton_menu_3")) { //U+00E9 es el caracter unicode 'é'
			estado = 3;
		}
		if (GUILayout.Button(new GUIContent("Salir", "Salir de este juego"), "boton_menu_4")) {
			estado = 4;
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	private void menuCargar() {
		Rect caja = new Rect(cuantoW * 14, cuantoH * 7, cuantoW * 20, cuantoH * 16);
		GUI.Box(caja, "");
		caja = new Rect(cuantoW * 14, cuantoH * 8, cuantoW * 20, cuantoH * 14);
		posicionScroll = GUI.BeginScrollView(caja, posicionScroll, new Rect(0, 0, cuantoW * 20, cuantoH * 4 * numSaves));
		if (numSaves == 0) {
			GUI.Label(new Rect(cuantoW, cuantoH * 4, cuantoW * 18, cuantoH * 4), "No hay ninguna partida guardada");
		}
		for (int i = 0; i < numSaves; i++) {
			if (GUI.Button(new Rect(cuantoW, i * cuantoH * 4, cuantoW * 18, cuantoH * 4), new GUIContent(nombresSaves[i], "Cargar partida num. " + i))) {
				SaveLoad.cambiaFileName(nombresSaves[i]);
				saveGame = SaveLoad.Load();
				GameObject temp = GameObject.FindGameObjectWithTag("Carga");
				ValoresCarga contenedor = temp.GetComponent<ValoresCarga>();
				SaveLoad.rehacerScript(saveGame, ref contenedor);
				estado = 7;
			}
		}
		GUI.EndScrollView();
		if (GUI.Button(new Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), new GUIContent("Volver", "Volver al men\u00fa"), "boton_atras")) {	//Aqui \u00fa es el caracter unicode para "ú"
			estado = 0;
		}
	}
	
	private void menuOpciones() {
		GUI.Box(new Rect(cuantoW * 17, cuantoH * 8, cuantoW * 14, cuantoH * 14),"");
		GUILayout.BeginArea(new Rect(cuantoW * 19, cuantoH * 9, cuantoW * 11, cuantoH * 12));
		GUILayout.BeginVertical();
		musicaOn = customToggleLayout(musicaOn, "M\u00fasica", "Apagar/Encender m\u00fasica");
		musicaVol = GUILayout.HorizontalSlider(musicaVol, 0.0f, 1.0f);
		GUILayout.Space(cuantoH * 2); 		//Dejar un espacio de 2 cuantos entre opcion y opcion
		sfxOn = customToggleLayout(sfxOn, "SFX", "Apagar/Encender efectos");
		sfxVol = GUILayout.HorizontalSlider(sfxVol, 0.0f, 1.0f);
		GUILayout.EndVertical();
		GUILayout.EndArea();
		if (GUI.Button(new Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), new GUIContent("Volver", "Volver al men\u00fa"), "boton_atras")) {
			PlayerPrefs.Save();
			Camera.main.animation.Play("AlejarsePantalla");
			estado = 0;
		}
	}
	
	private void creditos() {
		GUI.TextArea(new Rect(cuantoW * 16, cuantoH * 8, cuantoW * 16, cuantoH * 14), cadenaCreditos);
		if (GUI.Button(new Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), new GUIContent("Volver", "Volver al men\u00fa"), "boton_atras")) {
			estado = 0;
		}
	}
	
	private void creacionParte1Interfaz() {
		GUI.Label(new Rect(cuantoW * 2, cuantoH * 7, cuantoW * 15, cuantoH * 1), "Introduzca los parametros de busqueda para el proximo planeta a colonizar.", "label_centrada");
		
		GUILayout.BeginArea(new Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 19));
		GUILayout.BeginVertical();
		//Controles para alterar el tipo de terreno a crear aleatoriamente: cosas que no influyan mucho, nombre, etc. o cosas que 
		//influyan en la creacion del ruido, por ejemplo el numero de octavas a usar podemos llamarlo "factor de erosion" o cosas asi.
		//Despues de este paso se crea el mapa aleatorio con ruido.
		
		GUILayout.Label("Altura del terreno", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Bajo");
		gananciaInit = GUILayout.HorizontalSlider(gananciaInit, 0.45f, 0.55f);
		GUILayout.Label("Alto");
		GUILayout.EndHorizontal();
		if (gananciaInit <= 0.475f) {
			GUILayout.Label("Bajo");
		}
		else if (gananciaInit <= 0.5f) {
			GUILayout.Label("Normal");
		}
		else if (gananciaInit <= 0.525f) {
			GUILayout.Label("Alto");
		}
		else {
			GUILayout.Label("Muy alto");
		}		
				
		GUILayout.Label("Numero de continentes", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Bajo");
		escalaInit = GUILayout.HorizontalSlider(escalaInit, 0.0055f, 0.001f);
		GUILayout.Label("Alto");
		GUILayout.EndHorizontal();
		if (escalaInit >= 0.0045f) {
			GUILayout.Label("Islas peque\u00f1as");
		}
		else if (escalaInit >= 0.0033f) {
			GUILayout.Label("Islas grandes");
		}
		else if (escalaInit >= 0.0021f) {
			GUILayout.Label("Continentes");
		}
		else {
			GUILayout.Label("Grandes continentes");
		}	
				
		GUILayout.Label("Irregularidad del relieve", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Baja");
		octavasFloat = GUILayout.HorizontalSlider(octavasFloat, 2.0f, 10.0f);
		GUILayout.Label("Alta");
		GUILayout.EndHorizontal();
		if (octavasFloat <= 4.0f) {
			GUILayout.Label("Terreno liso");
		}
		else if (octavasFloat <= 6.0f) {
			GUILayout.Label("Con relieve");
		}
		else if (octavasFloat <= 8.0f) {
			GUILayout.Label("Escarpado");
		}
		else {
			GUILayout.Label("Muy escarpado");
		}	
				
		GUILayout.Label("Variacion", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Baja");
		lacunaridadInit = GUILayout.HorizontalSlider(lacunaridadInit, 1.4f, 3.1f);
		GUILayout.Label("Alta");
		GUILayout.EndHorizontal();
		float lacunaridadInitTemp = (lacunaridadInit - 1.4f) / (3.1f - 1.4f);
		if (lacunaridadInitTemp <= 0.25f) {
			GUILayout.Label("Muy regular");
		}
		else if (lacunaridadInitTemp <= 0.5f) {
			GUILayout.Label("Regular");
		}
		else if (lacunaridadInitTemp <= 0.75f) {
			GUILayout.Label("Irregular");
		}
		else {
			GUILayout.Label("Muy irregular");
		}
				
		if (GUILayout.Button(new GUIContent("Buscar", "Busca un planeta con esos parametros."))) {	
			FuncTablero.setEscala(escalaInit);
			FuncTablero.setGanancia(gananciaInit);
			FuncTablero.setLacunaridad(lacunaridadInit);
			FuncTablero.setOctavas2((int)octavasFloat);
			FuncTablero.reiniciaPerlin();
			StartCoroutine(creacionParte1());
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Volver", "Volver al men\u00fa principal"))) {
			faseCreacion = 0;
			estado = 0;
//			objetoRoca.renderer.enabled = false;
			objetoRoca.animation.Play("HologramaDesaparecer");
			Camera.main.animation.Play("AlejarseHolograma");
		}
		GUILayout.Space(cuantoW * 28);
		string tooltipTemp = "Pasar a la segunda fase";
		if (!paso1Completado) {
			tooltipTemp	= "Generar un planeta primero";
			GUI.enabled = false;
		}
		if (GUILayout.Button(new GUIContent("Siguiente", tooltipTemp))) {
			faseCreacion = 1;	
		}
		GUI.enabled = true;
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void creacionParte2Interfaz() {
		GUI.Label(new Rect(cuantoW * 2, cuantoH * 7, cuantoW * 15, cuantoH * 1), "Especifique los detalles de la orografia del planeta deseado.", "label_centrada");

		GUILayout.BeginArea(new Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 15));
		GUILayout.BeginVertical();
		//Controles para alterar el tipo de terreno ya creado: tipo de planeta a escoger con la "rampa" adecuada, altura de las montañas, 
		//cantidad de agua, etc.
		//Despues de este paso se colorea el mapa creado.
				
		GUILayout.Label("Nivel del agua", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		nivelAguaInit = GUILayout.HorizontalSlider(nivelAguaInit, 0.15f, 0.45f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		GUILayout.Label(nivelAguaInit.ToString());
		
		if (GUI.changed) {			
			objetoRoca.renderer.sharedMaterials[1].SetFloat("_nivelMar", nivelAguaInit);
		}
				
		GUILayout.Label("Temperatura del planeta", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		temperaturaInit = GUILayout.HorizontalSlider(temperaturaInit, 0.0f, 1.0f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		GUILayout.Label(temperaturaInit.ToString());
				
		GUILayout.Label("Longitud de las playas", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		tamanoPlayasInit = GUILayout.HorizontalSlider(tamanoPlayasInit, 0.02f, 0.06f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		GUILayout.Label(tamanoPlayasInit.ToString());
		
		if (GUI.changed) {			
			objetoRoca.renderer.sharedMaterials[1].SetFloat("_tamPlaya", tamanoPlayasInit);
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Volver", "Volver a la primera fase"))) {
			faseCreacion = 0;
		}
		GUILayout.Space(cuantoW * 28);
		if (GUILayout.Button(new GUIContent("Siguiente", "Pasar a la tercera fase"))) {
			FuncTablero.setNivelAgua(nivelAguaInit);
			FuncTablero.setTemperatura(temperaturaInit);
			FuncTablero.setTamanoPlaya(tamanoPlayasInit);
			StartCoroutine(creacionParte2());
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void creacionParte3Interfaz() {
		GUI.Label(new Rect(cuantoW * 5, cuantoH * 12, cuantoW * 15, cuantoH * 1), "Hemos encontrado una coincidencia con los parametros introducidos en el sistema.", "label_centrada");
		GUI.Label(new Rect(cuantoW * 5, cuantoH * 13, cuantoW * 15, cuantoH * 1), "El planeta encontrado se llama " + nombrePlaneta + " y se encuentra orbitando la estrella " + nombreEstrella + ".", "label_centrada");
		GUI.Label(new Rect(cuantoW * 5, cuantoH * 14, cuantoW * 15, cuantoH * 1), "Rumbo fijado. Buena suerte en su mision.", "label_centrada");
		GUILayout.BeginArea(new Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Volver", "Volver a la segunda fase"))) {
			faseCreacion = 1;
		}
		GUILayout.Space(cuantoW * 28);
		//Mejor si solo pulsando el boton de comenzar empiezas directamente
		if (GUILayout.Button(new GUIContent("Comenzar", "Empezar el juego"))) {
			StartCoroutine(creacionParte3());
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	//Controles personalizados -----------------------------------------------------------------------------------------------------------------
	
	private bool customToggleLayout(bool boo, string str, string tool) {
		bool valor;
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent(str, tool));
		valor = GUILayout.Toggle(boo, new GUIContent("", tool));
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		return valor;
	}
	
	private float customSliderLayout(float valor, float izq, float der, string str, string tool) {
		float valorOut;
		GUILayout.BeginVertical();
		GUILayout.Label(new GUIContent(str, tool));
		valorOut = GUILayout.HorizontalSlider(valor, izq, der);
		GUILayout.EndVertical();
		return valorOut;
	}
	
	private void barraProgreso() {
		GUI.Label(new Rect(cuantoW * 22, cuantoH * 16, cuantoW * 4, cuantoH), "Cargando...");
		//Debug solo
		GUI.Label(new Rect(cuantoW * 22, cuantoH * 17, cuantoW * 4, cuantoH), "Progreso: " + progreso.ToString());
		//fin debug
        GUI.Box(new Rect(cuantoW * 19, cuantoH * 14, cuantoW * 10, cuantoH), "", "progressBarVacio");
        GUI.BeginGroup(new Rect(cuantoW * 19, cuantoH * 14, (cuantoW * 10) * progreso, cuantoH));
            GUI.Box(new Rect(0,0, cuantoW * 10, cuantoH), "", "progressBarLleno");
        GUI.EndGroup();
	}
	
	//funciones de animacion de la escna inicial
	private void naveIdle(){
		Vector3 posicion = nave.transform.position;
		posicion.y += Mathf.Sin(Time.time)*0.0013f + Mathf.Cos(Time.time)*0.0037f;
		posicion.x += Mathf.Cos(Time.time)*0.0023f + Mathf.Sin(Time.time)*0.0017f;
		float rotacion = Mathf.Sin(Time.time)*0.05f;
		nave.transform.Rotate(Vector3.forward,rotacion);
		
		nave.transform.position = posicion;
	}
	
}
