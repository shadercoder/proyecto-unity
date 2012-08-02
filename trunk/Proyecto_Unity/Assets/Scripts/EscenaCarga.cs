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
	private float gananciaInit 				= 0.35f;			//La ganancia a pasar al script de creación del ruido
	private float escalaInit 				= 0.004f;			//La escala a pasar al script de creación del ruido
	private float lacunaridadInit			= 3.5f;				//La lacunaridad a pasar al script de creacion del ruido
	private float octavasFloat				= 12.0f;			//Las octavas a pasar al script de creacion del ruido
	
		//Segunda fase
	public GameObject objetoOceano;
	private Vector3 escalaBase 				= new Vector3(45.0f,45.0f,45.0f);	//La escala básica del océano
	public Mesh meshEsfera;														//La esfera sobre la que se harán los cambios
	private Mesh aguaMesh;														//El objeto con el Mesh extruido del agua
	private Mesh rocaMesh;														//El objeto con el Mesh extruido de la roca
	private float tamanoPlayasInit			= 0.1f;								//El tamaño de las playas
	private float nivelAguaInit 			= 0.6f;								//El punto a partir del cual deja de haber mar en la orografía del planeta
	private float temperaturaInit 			= 0.5f;								//Entre 0.0 y 1.0, la temperatura del planeta, que modificará la paleta.
	
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
	
																			
	//Funciones basicas ----------------------------------------------------------------------------------------------------------------------
	
	void Awake() {
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
		objetoOceano.transform.localScale = escalaBase + new Vector3(nivelAguaInit, nivelAguaInit, nivelAguaInit);
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
					ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
					temp.texturaBase = texturaBase;
					temp.texturaBase.Apply();
					temp.roca = rocaMesh;
					temp.agua = aguaMesh;
					temp.nivelAgua = nivelAguaInit;
					temp.tamanoPlaya = tamanoPlayasInit;
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
				cargarJuego();
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
			int longitud = GUI.tooltip.Length;
			if (longitud == 0) {
				return;
			}
			else {
				longitud *= 9;
			}
			float posx = Input.mousePosition.x;
			float posy = Input.mousePosition.y;
			if (posx > (Screen.width / 2)) {
				posx -= 215;
			}
			else {
				posx += 20;
			}
			if (posy > (Screen.height / 2)) {
				posy -= 25;
			}
			else {
				posy += 30;
			}	
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
		GUI.enabled = false;
		progreso = 0.1f;
		yield return null;
		pixels = FuncTablero.ruidoTextura();										//Se crea el ruido para la textura base y normales...
		progreso = 0.5f;
		yield return null;
		pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);		//Se suaviza el borde lateral...
		progreso = 0.7f;
		yield return null;
		pixels = FuncTablero.suavizaPoloTex(pixels);								//Se suavizan los polos...
		progreso = 0.8f;
		yield return null;
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();
		progreso = 1.0f;
		yield return null;
		progreso = 0.0f;
		trabajando = false;
		GUI.enabled = true;
		paso1Completado = true;
	}
	
	private IEnumerator creacionParte2() {
		trabajando = true;
		GUI.enabled = false;
		progreso = 0.1f;
		yield return null;
		Mesh meshTemp = GameObject.Instantiate(meshEsfera) as Mesh;
		progreso = 0.2f;
		yield return null;
		meshTemp = FuncTablero.extruyeVertices(meshTemp, texturaBase, 0.45f, new Vector3(0.0f, 0.0f, 0.0f));
		progreso = 0.5f;
		yield return null;
		rocaMesh = meshTemp;
		Texture2D texturaAgua = FuncTablero.calculaTexAgua(texturaBase);
		progreso = 0.7f;
		yield return null;
		Mesh meshAgua = GameObject.Instantiate(meshEsfera) as Mesh;
		progreso = 0.8f;
		yield return null;
		meshAgua = FuncTablero.extruyeVertices(meshAgua, texturaAgua, 0.45f, new Vector3(0.0f, 0.0f, 0.0f));
		aguaMesh = meshAgua;
		progreso = 1.0f;
		yield return null;
		progreso = 0.0f;
		trabajando = false;
		GUI.enabled = true;
		faseCreacion = 2;
	}
	
	private IEnumerator creacionParte3() {
		trabajando = true;
		yield return null;
		trabajando = false;
	}
	
	private void cargarJuego() {		
		int w = saveGame.width;
		int h = saveGame.height;
		Color[] pixels = new Color[w * h];
		for (int i = 0; i < w * h; i++) {
			float temp = saveGame.data[i];
			pixels[i] = new Color(temp, temp, temp);
		}	
		if (texturaBase.width != w || texturaBase.height != h) {
			Debug.LogError("Las dimensiones de las texturas no coinciden!");
		}
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();
		estado = 1;
	}	
	
	//Menus personalizados --------------------------------------------------------------------------------------------------------------------
	
	private void menuPrincipal() {
		GUILayout.BeginArea(new Rect((float)cuantoW * 20.5f, cuantoH * 25, cuantoW * 7, cuantoH * 5));
		GUILayout.BeginVertical();
		if (GUILayout.Button(new GUIContent("Comenzar juego", "Comenzar un juego nuevo"), "boton_menu_1")) {
			pixels = new Color[texturaBase.width * texturaBase.height];
			FuncTablero.inicializa(texturaBase);
			faseCreacion = 0;
			paso1Completado = false;
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
		GUILayout.BeginArea(new Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 19));
		GUILayout.BeginVertical();
		//Controles para alterar el tipo de terreno a crear aleatoriamente: cosas que no influyan mucho, nombre, etc. o cosas que 
		//influyan en la creacion del ruido, por ejemplo el numero de octavas a usar podemos llamarlo "factor de erosion" o cosas asi.
		//Despues de este paso se crea el mapa aleatorio con ruido.
		
		GUILayout.Label("Ganancia", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		gananciaInit = GUILayout.HorizontalSlider(gananciaInit, 0.1f, 0.6f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();			
		
		GUILayout.Space(cuantoH);
		
		GUILayout.Label("Escala", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Max");
		escalaInit = GUILayout.HorizontalSlider(escalaInit, 0.007f, 0.001f);
		GUILayout.Label("Min");
		GUILayout.EndHorizontal();
		
		GUILayout.Space(cuantoH);
		
		GUILayout.Label("Octavas", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		octavasFloat = GUILayout.HorizontalSlider(octavasFloat, 4.0f, 16.0f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		
		GUILayout.Space(cuantoH);
		
		GUILayout.Label("Lacunaridad", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		lacunaridadInit = GUILayout.HorizontalSlider(lacunaridadInit, 2.0f, 6.5f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		
		GUILayout.Space(cuantoH);
		
		if (GUILayout.Button(new GUIContent("Generar", "Genera un nuevo planeta"))) {	
			FuncTablero.setEscala(escalaInit);
			FuncTablero.setGanancia(gananciaInit);
			FuncTablero.setLacunaridad(lacunaridadInit);
			FuncTablero.setOctavas2((int)octavasFloat);
			FuncTablero.reiniciaPerlin();
			creacionParte1();
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Volver", "Volver al men\u00fa principal"))) {
			faseCreacion = 0;
			estado = 0;	
			Camera.main.animation.Play("AlejarseHolograma");
		}
		GUILayout.Space(cuantoW * 28);
		if (paso1Completado) {
			if (GUILayout.Button(new GUIContent("Siguiente", "Pasar a la segunda fase"))) {
				faseCreacion = 1;	
			}
		}
		else {
			if (GUILayout.Button(new GUIContent("Siguiente", "Generar un planeta primero"))) {
				//TODO Sonido de error, el boton con estilo diferente para estar en gris, etc.
			}
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void creacionParte2Interfaz() {
		GUILayout.BeginArea(new Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 15));
		GUILayout.BeginVertical();
		//Controles para alterar el tipo de terreno ya creado: tipo de planeta a escoger con la "rampa" adecuada, altura de las montañas, 
		//cantidad de agua, etc.
		//Despues de este paso se colorea el mapa creado.
		
		GUILayout.Space(cuantoH * 2);
		
		GUILayout.Label("Nivel del agua", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		nivelAguaInit = GUILayout.HorizontalSlider(nivelAguaInit, 0.2f, 1.0f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		
		if (GUI.changed) {			
			objetoOceano.transform.localScale = escalaBase + new Vector3(nivelAguaInit, nivelAguaInit, nivelAguaInit);
		}
		
		GUILayout.Space(cuantoH * 2);
		
		GUILayout.Label("Temperatura del planeta", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		temperaturaInit = GUILayout.HorizontalSlider(temperaturaInit, 0.0f, 1.0f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		
		GUILayout.Space(cuantoH * 2);
		
		GUILayout.Label("Longitud de las playas", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		tamanoPlayasInit = GUILayout.HorizontalSlider(tamanoPlayasInit, 0.01f, 0.3f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		
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
			creacionParte2();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void creacionParte3Interfaz() {
		GUILayout.BeginArea(new Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Volver", "Volver a la segunda fase"))) {
			faseCreacion = 1;
		}
		GUILayout.Space(cuantoW * 28);
		//Mejor si solo pulsando el boton de comenzar empiezas directamente
		if (GUILayout.Button(new GUIContent("Comenzar", "Empezar el juego"))) {
			if (!trabajando)
				creacionParte3();
			faseCreacion = 0;
			estado = 1;	
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
            GUI.Box(new Rect(0,0, cuantoW * 10, cuantoH), "", "progressBarLLeno");
        GUI.EndGroup();
	}

}
