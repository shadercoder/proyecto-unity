#pragma strict

//Variables del script
private var estado 			: int = 0;					//0 para menu, 1 para comenzar, 2 para opciones, 3 para creditos, 4 para salir

//Variables de la creacion
var contenedorTexturas 		: GameObject;				//Aqui se guardan las texturas que luego se usarán en el planeta
var texturaBase				: Texture2D;				//La textura visible que vamos a inicializar durante la creacion de un planeta nuevo
var texturaNorm				: Texture2D;				//La textura normal con el mapa de altura
var texturaMask 			: Texture2D;				//La textura con la mascara de reflejo para el agua
private var texturaPantalla	: Texture2D = null;			//La textura a enseñar durante la creacion
private var pixels			: Color[];					//Los pixeles sobre los que realizar operaciones
private var media			: float = 0;				//La media de altura de la textura
private var faseCreacion 	: int = 0;					//Fases de la creacion del planeta
private var trabajando		: boolean = false;			//Para saber si está haciendo algo por debajo el script
private var paso1Completado	: boolean = false;			//Si se han completado los pasos suficientes para pasar a la siguiente fase
private var paso2Completado	: boolean = false;			//Si se han completado los pasos suficientes para pasar a la siguiente fase
private var paso3Completado	: boolean = false;			//Si se han completado los pasos suficientes para pasar a la siguiente fase
private var gananciaInit	: float = 0.8;				//La ganancia a pasar al script de creación del ruido
private var escalaInit		: float = 0.005;			//La escala a pasar al script de creación del ruido
private var nivelAguaInit	: float = 0.5;				//El punto a partir del cual deja de haber mar en la orografía del planeta
private var temperaturaInit	: float = 0.5;				//Entre 0.0 y 1.0, la temperatura del planeta, que modificará la paleta.

//Opciones
private var musicaOn 		: boolean = true;			//Está la música activada?
private var musicaVol 		: float = 0.5;				//A que volumen?
private var sfxOn 			: boolean = true;			//Estan los efectos de sonido activados?
private var sfxVol 			: float = 0.5; 				//A que volumen?

//Variables de conveniencia
private var miObjeto 		: Transform;				//Guarda la posicion del objeto para ahorrar calculos

private var cadenaCreditos 	: String = "\t Hurricane son: \n Marcos Calleja Fernández\n Aris Goicoechea Lassaletta\n Pablo Pizarro Moleón\n" + 
										"\n\t Música a cargo de:\n Easily Embarrased\n Frost-RAVEN";
														//Cadena con los créditos a mostrar
//Tooltips
private var posicionMouse 	: Vector3 = Vector3.zero;	//Guarda la ultima posicion del mouse		
private var activarTooltip 	: boolean = false;			//Controla si se muestra o no el tooltip	
private var ultimoMov 		: float = 0;				//Ultima vez que se movio el mouse		
var tiempoTooltip 			: float = 0.75;				//Tiempo que tarda en aparecer el tooltip	

//Interfaz
var estiloGUI 				: GUISkin;					//Los estilos a usar para la escena de carga y menús
private var cuantoW			: int = Screen.width / 48;	//Minima unidad de medida de la interfaz a lo ancho (formato 16/10)
private var cuantoH			: int = Screen.height / 30;	//Minima unidad de medida de la interfaz a lo alto (formato 16/10)

//Menus para guardar
private var posicionScroll	: Vector2 = Vector2.zero;	//La posicion en la que se encuentra la ventana con scroll
private var numSaves		: int = 0;					//El numero de saves diferentes que hay en el directorio respectivo
private var nombresSaves	: String[];					//Los nombres de los ficheros de savegames guardados
private var saveGame		: SaveData;					//El contenido de la partida salvada cargada

																		
//Funciones basicas ----------------------------------------------------------------------------------------------------------------------

function Awake() {
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
	nombresSaves = new String[numSaves];
	nombresSaves = SaveLoad.getFileNames();
}

//function Start() {
//	var async : AsyncOperation = Application.LoadLevelAsync ("Generador_Planeta");
//    yield async;
//    Debug.Log ("Loading complete");
//}

function Update() {
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
	//---- Debug ----
//	Debug.Log("Persistent Data path: " + Application.persistentDataPath);
//	Debug.Log("Data path: " + Application.dataPath);
}

function FixedUpdate() {
	var opSonido : AudioSource = miObjeto.GetComponent(AudioSource);
	opSonido.volume = musicaVol;
	if (!musicaOn && opSonido.isPlaying)
		opSonido.Pause();
	else if (musicaOn && !opSonido.isPlaying)
		opSonido.Play();	
}

function OnGUI() {
	GUI.skin = estiloGUI;
	GUI.Box(Rect(0,0,Screen.width,Screen.height), "", "fondo_inicio_1");
	GUI.Box(Rect(cuantoW * 16, cuantoH, cuantoW * 16, cuantoH * 5), "", "header_titulo"); //Header es 500x100px
	switch (estado) {
		case 0: 	//Menu principal
			menuPrincipal();
			break;
		case 1:		//Comenzar
			if (paso3Completado) {
				var temp : ValoresCarga = contenedorTexturas.GetComponent("ValoresCarga") as ValoresCarga;
				temp.texturaBase = texturaBase;
				temp.texturaNorm = texturaNorm;
				temp.texturaMask = texturaMask;
				temp.texturaBase.Apply();
				temp.texturaNorm.Apply();
				temp.texturaMask.Apply();
				Application.LoadLevel("Generador_Planeta");
			}
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
		case 6:		//Cargar
			menuCargar();
			break;
	
	}
	if (trabajando) {
		GUI.Box(Rect(cuantoW * 22, cuantoH * 13, cuantoW * 4, cuantoH * 4), "Generando\nEspere...");
	}
	
	//Tooltip
	mostrarTooltip();
}

//Funciones auxiliares --------------------------------------------------------------------------------------------------------------------

function mostrarTooltip() {
	if (activarTooltip) {
		var longitud : int = GUI.tooltip.Length;
		if (longitud == 0) {
			return;
		}
		else {
			longitud *= 9;
		}
		var posx : float = Input.mousePosition.x;
		var posy : float = Input.mousePosition.y;
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
		var pos : Rect = Rect(posx, Screen.height - posy, longitud, 25);
		GUI.Box(pos, "");
		GUI.Label(pos, GUI.tooltip);
	}
}

function actualizarOpciones() {
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

function creacionParte1() {
	yield WaitForSeconds(0.1);
	pixels = FuncTablero.ruidoTextura();										//Se crea el ruido para la textura base y normales...
	yield WaitForEndOfFrame();
	pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);		//Se suaviza el borde lateral...
	yield WaitForEndOfFrame();
	pixels = FuncTablero.suavizaPoloTex(pixels, texturaBase.height / 20);		//Se suavizan los polos...
	yield WaitForEndOfFrame();
	texturaPantalla = new Texture2D(texturaBase.width, texturaBase.height);
	texturaPantalla.SetPixels(pixels);
	texturaPantalla.Apply();
	yield WaitForEndOfFrame();
	trabajando = false;
}

function creacionParte2() {
	yield WaitForSeconds(0.1);
	media = FuncTablero.calcularMedia(pixels);
	yield WaitForEndOfFrame();
	pixels = FuncTablero.realzarRelieve(pixels, media);
	yield WaitForEndOfFrame();
	texturaBase.SetPixels(pixels);
	texturaBase.Apply();
	yield WaitForEndOfFrame();
	trabajando = false;
}

function creacionParte3() {
	yield WaitForSeconds(0.1);
	var pixelsAgua : Color[] = FuncTablero.mascaraBumpAgua(pixels, nivelAguaInit);	//se ignora el mar para el relieve
	yield WaitForEndOfFrame();
	texturaNorm.SetPixels(pixels);													//Se aplican los pixeles a la textura normal para duplicarlos
	texturaNorm.SetPixels32(FuncTablero.creaNormalMap(texturaNorm));				//se transforma a NormalMap
	yield WaitForEndOfFrame();
	texturaNorm.Apply();
	texturaMask.SetPixels(pixelsAgua);
	texturaMask.Apply();
	yield WaitForEndOfFrame();
	trabajando = false;
}

function creacionRestante() {
	yield creacionParte2();
	paso2Completado = true;
	trabajando = true;
	yield creacionParte3();
	paso3Completado = true;
}


//Menus personalizados --------------------------------------------------------------------------------------------------------------------

function menuPrincipal() {
	GUILayout.BeginArea(Rect(cuantoW * 17, cuantoH * 10, cuantoW * 14, cuantoH * 10));
	GUILayout.BeginVertical();
	if (GUILayout.Button(GUIContent("Comenzar juego", "Comenzar un juego nuevo"), "boton_menu_1")) {
		pixels = new Color[texturaBase.width * texturaBase.height];
		FuncTablero.inicializa(texturaBase);
		texturaPantalla = null;
		faseCreacion = 0;
		paso1Completado = false;
		paso2Completado = false;
		paso3Completado = false;
		estado = 5;
	}
	if (GUILayout.Button(GUIContent("Cargar", "Cargar un juego guardado"), "boton_menu_2")) {
		estado = 6;
	}
	if (GUILayout.Button(GUIContent("Opciones", "Acceder a las opciones"), "boton_menu_3")) {
		estado = 2;
	}
	if (GUILayout.Button(GUIContent("Créditos", "Visualiza los créditos"), "boton_menu_3")) {
		estado = 3;
	}
	if (GUILayout.Button(GUIContent("Salir", "Salir de este juego"), "boton_menu_4")) {
		estado = 4;
	}
	GUILayout.EndVertical();
	GUILayout.EndArea();
}

function menuCargar() {
	var caja : Rect = new Rect(cuantoW * 14, cuantoH * 7, cuantoW * 20, cuantoH * 16);
	GUI.Box(caja, "");
	caja = new Rect(cuantoW * 14, cuantoH * 8, cuantoW * 20, cuantoH * 14);
	posicionScroll = GUI.BeginScrollView(caja, posicionScroll, Rect(0, 0, cuantoW * 20, cuantoH * 4 * numSaves));
	if (numSaves == 0) {
		GUI.Label(Rect(cuantoW * 20, cuantoH * 14, cuantoW * 8, cuantoH * 2), "No hay ninguna partida guardada");
	}
	for (var i : int = 0; i < numSaves; i++) {
		if (GUI.Button(Rect(cuantoW, i * cuantoH * 4, cuantoW * 18, cuantoH * 4), GUIContent(nombresSaves[i], "Cargar partida num. " + i))) {
			SaveLoad.cambiaFileName(nombresSaves[i]);
			saveGame = SaveLoad.Load();
		}
	}
	GUI.EndScrollView();
	if (GUI.Button(Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), GUIContent("Volver", "Volver al menú"), "boton_atras")) {
		estado = 0;
	}
}

function menuOpciones() {
	GUI.Box(Rect(cuantoW * 17, cuantoH * 8, cuantoW * 14, cuantoH * 14),"");
	GUILayout.BeginArea(Rect(cuantoW * 19, cuantoH * 9, cuantoW * 11, cuantoH * 12));
	GUILayout.BeginVertical();
	musicaOn = customToggleLayout(musicaOn, "Musica", "Apagar/Encender música");
	musicaVol = GUILayout.HorizontalSlider(musicaVol, 0.0, 1.0);
	GUILayout.Space(cuantoH * 2); 		//Dejar un espacio de 2 cuantos entre opcion y opcion
	sfxOn = customToggleLayout(sfxOn, "SFX", "Apagar/Encender efectos");
	sfxVol = GUILayout.HorizontalSlider(sfxVol, 0.0, 1.0);
	GUILayout.EndVertical();
	GUILayout.EndArea();
	if (GUI.Button(Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2),GUIContent("Volver", "Volver al menú"), "boton_atras")) {
		PlayerPrefs.Save();
		estado = 0;
	}
}

function creditos() {
	GUI.TextArea(Rect(cuantoW * 16, cuantoH * 8, cuantoW * 16, cuantoH * 14), cadenaCreditos);
	if (GUI.Button(Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2),GUIContent("Volver", "Volver al menú"), "boton_atras")) {
		estado = 0;
	}
}

function creacionParte1Interfaz() {
	
	if (texturaPantalla == null) {
		GUI.Box(Rect(cuantoW * 12, cuantoH * 7, cuantoW * 35, cuantoH * 19), "\n\n\n Debe generar una vista del planeta para poder avanzar.");
	}
	else {
		GUI.Box(Rect(cuantoW * 12, cuantoH * 7, cuantoW * 35, cuantoH * 19), texturaPantalla);
	}
	GUILayout.BeginArea(Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 15));
	GUILayout.BeginVertical();
	//Controles para alterar el tipo de terreno a crear aleatoriamente: cosas que no influyan mucho, nombre, etc. o cosas que 
	//influyan en la creacion del ruido, por ejemplo el numero de octavas a usar podemos llamarlo "factor de erosion" o cosas asi.
	//Despues de este paso se crea el mapa aleatorio con ruido.
	
	GUILayout.Space(cuantoH * 2);
	
	GUILayout.Label("Edad del planeta", "label_centrada");
	GUILayout.BeginHorizontal();
	GUILayout.Label("Plano");
	gananciaInit = GUILayout.HorizontalSlider(gananciaInit, 0.6, 0.85);
	GUILayout.Label("Escarpado");
	GUILayout.EndHorizontal();
	
	GUILayout.Space(cuantoH * 2);
	
	GUILayout.Label("Tamaño de los continentes", "label_centrada");
	GUILayout.BeginHorizontal();
	GUILayout.Label("Pequeños");
	escalaInit = GUILayout.HorizontalSlider(escalaInit, 0.009, 0.001);
	GUILayout.Label("Grandes");
	GUILayout.EndHorizontal();
	
	GUILayout.Space(cuantoH * 2);
	
	if (GUILayout.Button(GUIContent("Generar", "Genera un nuevo planeta"))) {	
		FuncTablero.setEscala(escalaInit);
		FuncTablero.setGanancia(gananciaInit);
		trabajando = true;
		GUI.Box(Rect(cuantoW * 22, cuantoH * 13, cuantoW * 4, cuantoH * 4), "Generando\nEspere...");
		FuncTablero.reiniciaPerlin();
		creacionParte1();
		paso1Completado = true;
	}
	
	GUILayout.EndVertical();
	GUILayout.EndArea();
	GUILayout.BeginArea(Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
	GUILayout.BeginHorizontal();
	if (GUILayout.Button(GUIContent("Volver", "Volver al menú principal"))) {
		faseCreacion = 0;
		estado = 0;	
	}
	GUILayout.Space(cuantoW * 28);
	if (paso1Completado) {
		if (GUILayout.Button(GUIContent("Siguiente", "Pasar a la segunda fase"))) {
//			faseCreacion = 1;	
			faseCreacion = 0;
			creacionRestante();		//Porque dejamos las cosas a medias... Medida temporal
			estado = 1;
		}
	}
	else {
		if (GUILayout.Button(GUIContent("Siguiente", "Generar un planeta primero"))) {
			//Sonido de error, el boton con estilo diferente para estar en gris, etc.
		}
	}
	
	GUILayout.EndHorizontal();
	GUILayout.EndArea();
}

function creacionParte2Interfaz() {
	GUI.Box(Rect(cuantoW * 12, cuantoH * 7, cuantoW * 35, cuantoH * 19), texturaPantalla);
	GUILayout.BeginArea(Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 15));
	GUILayout.BeginVertical();
	//Controles para alterar el tipo de terreno ya creado: tipo de planeta a escoger con la "rampa" adecuada, altura de las montañas, 
	//cantidad de agua, etc.
	//Despues de este paso se colorea el mapa creado.
	
	GUILayout.Space(cuantoH * 2);
	
	GUILayout.Label("Cantidad de líquido", "label_centrada");
	GUILayout.BeginHorizontal();
	GUILayout.Label("Bajo");
	nivelAguaInit = GUILayout.HorizontalSlider(nivelAguaInit, 0.0, 1.0);
	GUILayout.Label("Alto");
	GUILayout.EndHorizontal();
	
	GUILayout.Space(cuantoH * 2);
	
	GUILayout.Label("Temperatura del planeta", "label_centrada");
	GUILayout.BeginHorizontal();
	GUILayout.Label("Frío");
	temperaturaInit = GUILayout.HorizontalSlider(temperaturaInit, 0.0, 1.0);
	GUILayout.Label("Cálido");
	GUILayout.EndHorizontal();
	
	GUILayout.Space(cuantoH * 2);
	
	if (GUILayout.Button(GUIContent("Generar", "Genera un nuevo planeta"))) {	
		FuncTablero.setNivelAgua(nivelAguaInit);
//		FuncTablero.setTemperatura(temperaturaInit);
		trabajando = true;
		GUI.Box(Rect(cuantoW * 22, cuantoH * 13, cuantoW * 4, cuantoH * 4), "Generando\nEspere...");
		creacionParte2();
		paso2Completado = true;
	}
	
	GUILayout.EndVertical();
	GUILayout.EndArea();
	GUILayout.BeginArea(Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
	GUILayout.BeginHorizontal();
	if (GUILayout.Button(GUIContent("Volver", "Volver a la primera fase"))) {
		faseCreacion = 0;
	}
	GUILayout.Space(cuantoW * 28);
	if (paso2Completado) {
		if (GUILayout.Button(GUIContent("Siguiente", "Pasar a la tercera fase"))) {
			faseCreacion = 2;
		}
	}
	else {
		if (GUILayout.Button(GUIContent("Siguiente", "Generar la orografía primero"))) {
			//Sonido de error, el boton con estilo diferente para estar en gris, etc.
		}
	}
	GUILayout.EndHorizontal();
	GUILayout.EndArea();
}

function creacionParte3Interfaz() {
	GUI.Box(Rect(cuantoW * 12, cuantoH * 7, cuantoW * 35, cuantoH * 19), texturaPantalla);
	GUILayout.BeginArea(Rect(cuantoW, cuantoH * 9, cuantoW * 10, cuantoH * 15));
	GUILayout.BeginVertical();
	//Controles para alterar el tipo de terreno ya creado: ultimos retoques como por ejemplo los rios, montañas, cráteres, etc.
	//Despues de este paso se acepta todo lo anterior y se pasa al juego.
	
	
	GUILayout.EndVertical();
	GUILayout.EndArea();
	GUILayout.BeginArea(Rect(cuantoW * 12, cuantoH * 28, cuantoW * 35, cuantoH * 2));
	GUILayout.BeginHorizontal();
	if (GUILayout.Button(GUIContent("Volver", "Volver a la segunda fase"))) {
		faseCreacion = 1;
	}
	GUILayout.Space(cuantoW * 28);
	if (paso3Completado) {
		if (GUILayout.Button(GUIContent("Comenzar", "Empezar el juego"))) {
			faseCreacion = 0;
			estado = 1;	
		}
	}
	else {
		if (GUILayout.Button(GUIContent("Comenzar", "Completar el paso primero"))) {
			//Sonido de error, el boton con estilo diferente para estar en gris, etc.
		}
	}
	GUILayout.EndHorizontal();
	GUILayout.EndArea();
}

//Controles personalizados -----------------------------------------------------------------------------------------------------------------

function customToggleLayout(bool : boolean, str : String, tool : String) {
	var valor : boolean;
	GUILayout.BeginVertical();
	GUILayout.BeginHorizontal();
	GUILayout.Label(GUIContent(str, tool));
	valor = GUILayout.Toggle(bool, GUIContent("", tool));
	GUILayout.EndHorizontal();
	GUILayout.EndVertical();
	return valor;
}

function customSliderLayout(valor : float, izq : float, der : float, str : String, tool : String) {
	var valorOut : float;
	GUILayout.BeginVertical();
	GUILayout.Label(GUIContent(str, tool));
	valorOut = GUILayout.HorizontalSlider(valor, izq, der);
	GUILayout.EndVertical();
	return valorOut;
}
