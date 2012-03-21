using UnityEngine;
using System.Collections;

public class EscenaCarga : MonoBehaviour {

	//Variables del script
	private int estado 						= 0;				//0 para menu, 1 para comenzar, 2 para opciones, 3 para creditos, 4 para salir
	
	//Variables de la creacion
	public GameObject contenedorTexturas;						//Aqui se guardan las texturas que luego se usarán en el planeta
	public Texture2D texturaBase;								//La textura visible que vamos a inicializar durante la creacion de un planeta nuevo
	public Texture2D texturaNorm;								//La textura normal con el mapa de altura
	private Texture2D texturaPantalla 		= null;				//La textura a enseñar durante la creacion
	private Color[] pixels;										//Los pixeles sobre los que realizar operaciones
//	private float media 					= 0.0f;				//La media de altura de la textura
	
	private int faseCreacion 				= 0;				//Fases de la creacion del planeta
	private bool trabajando 				= false;			//Para saber si está haciendo algo por debajo el script
	private bool paso1Completado 			= false;			//Si se han completado los pasos suficientes para pasar a la siguiente fase
	private bool paso2Completado 			= false;			//Si se han completado los pasos suficientes para pasar a la siguiente fase
	private bool paso3Completado 			= false;			//Si se han completado los pasos suficientes para pasar a la siguiente fase
	
	private float gananciaInit 				= 0.35f;			//La ganancia a pasar al script de creación del ruido
	private float escalaInit 				= 0.004f;			//La escala a pasar al script de creación del ruido
	private float lacunaridadInit			= 3.5f;				//La lacunaridad a pasar al script de creacion del ruido
	private float octavasFloat				= 12.0f;			//Las octavas a pasar al script de creacion del ruido
	private float nivelAguaInit 			= 0.5f;				//El punto a partir del cual deja de haber mar en la orografía del planeta
	private float temperaturaInit 			= 0.5f;				//Entre 0.0 y 1.0, la temperatura del planeta, que modificará la paleta.
	
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
		DontDestroyOnLoad(contenedorTexturas);
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
		numSaves = SaveLoad.FileCount();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames();
		
		//Inicializacion de las texturas
//		GameObject planeta = GameObject.FindWithTag("Planeta");
//		MeshRenderer renderer = planeta.GetComponent<MeshRenderer>();
//		texturaBase = renderer.sharedMaterial.mainTexture as Texture2D;
//		texturaNorm = renderer.sharedMaterial.GetTexture("_Normals") as Texture2D;	//Los nombres vienen definidos en el editor, en el material
//		texturaMask = renderer.sharedMaterial.GetTexture("_Mask") as Texture2D;
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
		//GUI.Box(Rect(0,0,Screen.width,Screen.height), "", "fondo_inicio_1");
		GUI.Box(new Rect(cuantoW * 16, cuantoH, cuantoW * 16, cuantoH * 5), "", "header_titulo"); //Header es 500x100px
		switch (estado) {
			case 0: 	//Menu principal
				menuPrincipal();
				break;
			case 1:		//Comenzar
//				if (paso3Completado) {
					ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
					temp.texturaBase = texturaBase;
					temp.texturaNorm = texturaNorm;
//					temp.texturaMask = texturaMask;
					temp.texturaBase.Apply();
					temp.texturaNorm.Apply();
//					temp.texturaMask.Apply();
					Application.LoadLevel("Escena_Principal");
//				}
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
			GUI.Box(new Rect(cuantoW * 22, cuantoH * 13, cuantoW * 4, cuantoH * 4), "Generando\nEspere...");
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
	
	private void creacionParte1() {
//		yield return new WaitForSeconds(0.1f);
		pixels = FuncTablero.ruidoTextura();										//Se crea el ruido para la textura base y normales...
		pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);		//Se suaviza el borde lateral...
		pixels = FuncTablero.suavizaPoloTex(pixels, texturaBase.height / 20);		//Se suavizan los polos...
		texturaPantalla = new Texture2D(texturaBase.width, texturaBase.height);
		texturaPantalla.SetPixels(pixels);
		texturaPantalla.Apply();
		trabajando = false;
	}
	
	private void creacionParte2() {
//		yield return new WaitForSeconds(0.1f);
//		media = FuncTablero.calcularMedia(pixels);
//		pixels = FuncTablero.realzarRelieve(pixels, media);
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();
//		texturaPantalla.SetPixels(pixels);
//		texturaPantalla.Apply();
		trabajando = false;
	}
	
	private void creacionParte3() {
//		yield return new WaitForSeconds(0.1f);
//		Color[] pixelsAgua = FuncTablero.mascaraBumpAgua(pixels, nivelAguaInit);		//se ignora el mar para el relieve
		texturaNorm.SetPixels(pixels);													//Se aplican los pixeles a la textura normal para duplicarlos
		texturaNorm.SetPixels32(FuncTablero.creaNormalMap(texturaNorm));				//se transforma a NormalMap
		texturaNorm.Apply();
//		texturaMask.SetPixels(pixelsAgua);
//		texturaMask.Apply();
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
		texturaNorm.SetPixels(pixels);													//Se aplican los pixeles a la textura normal para duplicarlos
		texturaNorm.SetPixels32(FuncTablero.creaNormalMap(texturaNorm));				//se transforma a NormalMap
		texturaNorm.Apply();
		estado = 1;
	}	
	
	//Menus personalizados --------------------------------------------------------------------------------------------------------------------
	
	private void menuPrincipal() {
		GUILayout.BeginArea(new Rect((float)cuantoW * 20.5f, cuantoH * 25, cuantoW * 7, cuantoH * 5));
		GUILayout.BeginVertical();
		if (GUILayout.Button(new GUIContent("Comenzar juego", "Comenzar un juego nuevo"), "boton_menu_1")) {
			pixels = new Color[texturaBase.width * texturaBase.height];
			FuncTablero.inicializa(texturaBase);
			texturaPantalla = null;
			faseCreacion = 0;
			paso1Completado = false;
			paso2Completado = false;
			paso3Completado = false;
			estado = 5;
		}
		if (GUILayout.Button(new GUIContent("Cargar", "Cargar un juego guardado"), "boton_menu_2")) {
			estado = 6;
		}
		if (GUILayout.Button(new GUIContent("Opciones", "Acceder a las opciones"), "boton_menu_3")) {
			estado = 2;
		}
		if (GUILayout.Button(new GUIContent("Cr\u00e9ditos", "Visualiza los créditos"), "boton_menu_3")) { //U+00E9 es el caracter unicode 'é'
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
			GUI.Label(new Rect(cuantoW * 20, cuantoH * 14, cuantoW * 8, cuantoH * 2), "No hay ninguna partida guardada");
		}
		for (int i = 0; i < numSaves; i++) {
			if (GUI.Button(new Rect(cuantoW, i * cuantoH * 4, cuantoW * 18, cuantoH * 4), new GUIContent(nombresSaves[i], "Cargar partida num. " + i))) {
				SaveLoad.cambiaFileName(nombresSaves[i]);
				saveGame = SaveLoad.Load();
				estado = 7;
			}
		}
		GUI.EndScrollView();
		if (GUI.Button(new Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), new GUIContent("Volver", "Volver al menú"), "boton_atras")) {
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
		if (GUI.Button(new Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), new GUIContent("Volver", "Volver al menú"), "boton_atras")) {
			PlayerPrefs.Save();
			estado = 0;
		}
	}
	
	private void creditos() {
		GUI.TextArea(new Rect(cuantoW * 16, cuantoH * 8, cuantoW * 16, cuantoH * 14), cadenaCreditos);
		if (GUI.Button(new Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), new GUIContent("Volver", "Volver al menú"), "boton_atras")) {
			estado = 0;
		}
	}
	
	private void creacionParte1Interfaz() {
		
		if (texturaPantalla == null) {
			GUI.Box(new Rect(cuantoW * 12, cuantoH * 7, cuantoW * 35, cuantoH * 19), "\n\n\n Debe generar una vista del planeta para poder avanzar.");
		}
		else {
			GUI.Box(new Rect(cuantoW * 12, cuantoH * 7, cuantoW * 35, cuantoH * 19), texturaPantalla);
		}
		GUILayout.BeginArea(new Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 15));
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
			trabajando = true;
			GUI.Box(new Rect(cuantoW * 22, cuantoH * 13, cuantoW * 4, cuantoH * 4), "Generando\nEspere...");
			FuncTablero.reiniciaPerlin();
			creacionParte1();
			paso1Completado = true;
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Volver", "Volver al menú principal"))) {
			faseCreacion = 0;
			estado = 0;	
		}
		GUILayout.Space(cuantoW * 28);
		if (paso1Completado) {
			if (GUILayout.Button(new GUIContent("Siguiente", "Pasar a la segunda fase"))) {
				faseCreacion = 1;	
			}
		}
		else {
			if (GUILayout.Button(new GUIContent("Siguiente", "Generar un planeta primero"))) {
				//Sonido de error, el boton con estilo diferente para estar en gris, etc.
			}
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void creacionParte2Interfaz() {
		GUI.Box(new Rect(cuantoW * 12, cuantoH * 7, cuantoW * 35, cuantoH * 19), texturaPantalla);
		GUILayout.BeginArea(new Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 15));
		GUILayout.BeginVertical();
		//Controles para alterar el tipo de terreno ya creado: tipo de planeta a escoger con la "rampa" adecuada, altura de las montañas, 
		//cantidad de agua, etc.
		//Despues de este paso se colorea el mapa creado.
		
		GUILayout.Space(cuantoH * 2);
		
		GUILayout.Label("Nivel del agua", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		nivelAguaInit = GUILayout.HorizontalSlider(nivelAguaInit, 0.0f, 1.0f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		
		GUILayout.Space(cuantoH * 2);
		
		GUILayout.Label("Temperatura del planeta", "label_centrada");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min");
		temperaturaInit = GUILayout.HorizontalSlider(temperaturaInit, 0.0f, 1.0f);
		GUILayout.Label("Max");
		GUILayout.EndHorizontal();
		
		GUILayout.Space(cuantoH * 2);
		
		if (GUILayout.Button(new GUIContent("Generar", "Genera un nuevo planeta"))) {	
			FuncTablero.setNivelAgua(nivelAguaInit);
			FuncTablero.setTemperatura(temperaturaInit);
			trabajando = true;
			GUI.Box(new Rect(cuantoW * 22, cuantoH * 13, cuantoW * 4, cuantoH * 4), "Generando\nEspere...");
			creacionParte2();
			paso2Completado = true;
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Volver", "Volver a la primera fase"))) {
			faseCreacion = 0;
		}
		GUILayout.Space(cuantoW * 28);
		if (paso2Completado) {
			if (GUILayout.Button(new GUIContent("Siguiente", "Pasar a la tercera fase"))) {
				faseCreacion = 2;
			}
		}
		else {
			if (GUILayout.Button(new GUIContent("Siguiente", "Generar la orografía primero"))) {
				//Sonido de error, el boton con estilo diferente para estar en gris, etc.
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void creacionParte3Interfaz() {
		GUI.Box(new Rect(cuantoW * 12, cuantoH * 7, cuantoW * 35, cuantoH * 19), texturaPantalla);
		GUILayout.BeginArea(new Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 15));
		GUILayout.BeginVertical();
		//Controles para alterar el tipo de terreno ya creado: ultimos retoques como por ejemplo los rios, montañas, cráteres, etc.
		//Despues de este paso se acepta todo lo anterior y se pasa al juego.
		
		GUILayout.Space(cuantoH * 4);
		
		if (GUILayout.Button(new GUIContent("Aceptar", "Aceptar el actual planeta"))) {
			trabajando = true;
			GUI.Box(new Rect(cuantoW * 22, cuantoH * 13, cuantoW * 4, cuantoH * 4), "Generando\nEspere...");
			creacionParte3();
			paso3Completado = true;
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent("Volver", "Volver a la segunda fase"))) {
			faseCreacion = 1;
		}
		GUILayout.Space(cuantoW * 28);
		if (paso3Completado) {
			if (GUILayout.Button(new GUIContent("Comenzar", "Empezar el juego"))) {
				faseCreacion = 0;
				estado = 1;	
			}
		}
		else {
			if (GUILayout.Button(new GUIContent("Comenzar", "Completar el paso primero"))) {
				//Sonido de error, el boton con estilo diferente para estar en gris, etc.
			}
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

}
