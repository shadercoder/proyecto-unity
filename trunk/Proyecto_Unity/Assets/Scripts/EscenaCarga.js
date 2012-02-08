#pragma strict

var estiloGUI 				: GUISkin;				//Los estilos a usar para la escena de carga y menús
var texturaFondo 			: Texture;
//var opcionesPerdurables : GameObject;
private var estado 			: int = 0;				//0 para menu, 1 para comenzar, 2 para opciones, 3 para creditos, 4 para salir
private var faseCreacion 	: int = 0;				//Fases de la creacion del planeta
private var musicaOn 		: boolean = true;		//Está la música activada?
private var musicaVol 		: float = 0.5;			//A que volumen?
private var sfxOn 			: boolean = true;		//Estan los efectos de sonido activados?
private var sfxVol 			: float = 0.5; 			//A que volumen?
private var miObjeto 		: Transform;

private var cadenaCreditos 	: String = "\t Hurricane son: \n Marcos Calleja Fernández\n Aris Goicoechea Lassaletta\n Pablo Pizarro Moleón\n" + 
										"\n\t Música a cargo de:\n Easily Embarrased\n Frost-RAVEN";

//Tooltips
private var posicionMouse 	: Vector3 = Vector3.zero;	//Guarda la ultima posicion del mouse		
private var activarTooltip 	: boolean = false;			//Controla si se muestra o no el tooltip	
private var ultimoMov 		: float = 0;				//Ultima vez que se movio el mouse		
var tiempoTooltip 			: float = 0.75;				//Tiempo que tarda en aparecer el tooltip	

//Interfaz
private var cuantoW			: int = Screen.width / 48;	//Minima unidad de medida de la interfaz a lo ancho (formato 16/10)
private var cuantoH			: int = Screen.height / 30;	//Minima unidad de medida de la interfaz a lo alto (formato 16/10)

									
//Funciones basicas ----------------------------------------------------------------------------------------------------------------------

function Awake() {
	miObjeto = this.transform;
//	DontDestroyOnLoad(opcionesPerdurables);
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
}

//function Start() {
//	var async : AsyncOperation = Application.LoadLevelAsync ("Generador_Planeta");
//    yield async;
//    Debug.Log ("Loading complete");
//}

function Update() {
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
			Application.LoadLevel("Generador_Planeta");
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
				creacionParte1();
			else if (faseCreacion == 1)
				creacionParte2();
			else if (faseCreacion == 2)
				creacionParte3();
			break;
	
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

//Menus personalizados --------------------------------------------------------------------------------------------------------------------

function menuPrincipal() {
	GUILayout.BeginArea(Rect(cuantoW * 17, cuantoH * 10, cuantoW * 14, cuantoH * 10));
	GUILayout.BeginVertical();
	if (GUILayout.Button(GUIContent("Comenzar juego", "Comenzar un juego nuevo"), "boton_menu_1")) {
		estado = 5;
	}
	if (GUILayout.Button(GUIContent("Opciones", "Acceder a las opciones"), "boton_menu_2")) {
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

function creditos() {
	GUI.TextArea(Rect(cuantoW * 16, cuantoH * 8, cuantoW * 16, cuantoH * 14), cadenaCreditos);
	if (GUI.Button(Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2),GUIContent("Volver", "Volver al menú"), "boton_atras")) {
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
	GUI.Box(Rect(cuantoW * 8, cuantoH * 7, cuantoW * 39, cuantoH * 20), "Primera fase de creacion del planeta.");
	GUILayout.BeginArea(Rect(cuantoW, cuantoH * 10, cuantoW * 6, cuantoH * 15));
	GUILayout.BeginVertical();
	//Controles para alterar el tipo de terreno a crear aleatoriamente: cosas que no influyan mucho, nombre, etc. o cosas que 
	//influyan en la creacion del ruido, por ejemplo el numero de octavas a usar podemos llamarlo "factor de erosion" o cosas asi.
	//Despues de este paso se crea el mapa aleatorio con ruido.
	
	//Temporal!
	GUILayout.Space(cuantoH * 11);	//Para dejar espacio para los botones que faltan
	if (GUILayout.Button(GUIContent("Siguiente", "Pasar a la segunda fase"))) {
		faseCreacion = 1;	
	}
	GUILayout.Space(cuantoH);
	if (GUILayout.Button(GUIContent("Volver", "Volver al menú principal"))) {
		faseCreacion = 0;
		estado = 0;	
	}
	GUILayout.EndVertical();
	GUILayout.EndArea();
}

function creacionParte2() {
	GUI.Box(Rect(cuantoW * 8, cuantoH * 7, cuantoW * 39, cuantoH * 20), "Segunda fase de creacion del planeta.");
	GUILayout.BeginArea(Rect(cuantoW, cuantoH * 10, cuantoW * 6, cuantoH * 15));
	GUILayout.BeginVertical();
	//Controles para alterar el tipo de terreno ya creado: tipo de planeta a escoger con la "rampa" adecuada, altura de las montañas, 
	//cantidad de agua, etc.
	//Despues de este paso se colorea el mapa creado.
	
	//Temporal!
	GUILayout.Space(cuantoH * 11);	//Para dejar espacio para los botones que faltan
	if (GUILayout.Button(GUIContent("Siguiente", "Pasar a la tercera fase"))) {
		faseCreacion = 2;	
	}
	GUILayout.Space(cuantoH);
	if (GUILayout.Button(GUIContent("Volver", "Volver a la primera fase"))) {
		faseCreacion = 0;	
	}
	GUILayout.EndVertical();
	GUILayout.EndArea();
}

function creacionParte3() {
	GUI.Box(Rect(cuantoW * 8, cuantoH * 7, cuantoW * 39, cuantoH * 20), "Tercera fase de creacion del planeta.");
	GUILayout.BeginArea(Rect(cuantoW, cuantoH * 10, cuantoW * 6, cuantoH * 15));
	GUILayout.BeginVertical();
	//Controles para alterar el tipo de terreno ya creado: ultimos retoques como por ejemplo los rios, montañas, cráteres, etc.
	//Despues de este paso se acepta todo lo anterior y se pasa al juego.
	
	//Temporal!
	GUILayout.Space(cuantoH * 11);	//Para dejar espacio para los botones que faltan
	if (GUILayout.Button(GUIContent("Comenzar", "Empezar el juego"))) {
		faseCreacion = 0;
		estado = 1;	
	}
	GUILayout.Space(cuantoH);
	if (GUILayout.Button(GUIContent("Volver", "Volver a la segunda fase"))) {
		faseCreacion = 1;	
	}
	GUILayout.EndVertical();
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
