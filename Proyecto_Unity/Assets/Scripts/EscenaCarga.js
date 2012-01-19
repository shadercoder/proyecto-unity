#pragma strict

var estiloGUI : GUISkin;				//Los estilos a usar para la escena de carga y menús
var texturaFondo : Texture;
//var opcionesPerdurables : GameObject;
private var estado : int = 0;			//0 para menu, 1 para comenzar, 2 para opciones, 3 para creditos, 4 para salir
private var musicaOn : boolean = true;	//Está la música activada?
private var musicaVol : float = 0.5;	//A que volumen?
private var sfxOn : boolean = true;		//Estan los efectos de sonido activados?
private var sfxVol : float = 0.5; 		//A que volumen?
private var miObjeto : Transform;

private var cadenaCreditos : String = "\t Hurricane son: \n Marcos Calleja Fernández\n Aris Goicoechea Lassaletta\n Pablo Pizarro Moleón\n" + 
										"\n\t Música a cargo de:\n Easily Embarrased";

//Tooltips
private var posicionMouse : Vector3 = Vector3.zero;		//Guarda la ultima posicion del mouse		
private var activarTooltip : boolean = false;			//Controla si se muestra o no el tooltip	
private var ultimoMov : float = 0;						//Ultima vez que se movio el mouse		
var tiempoTooltip : float = 0.75;						//Tiempo que tarda en aparecer el tooltip	

			
									
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
	GUI.Label(Rect(Screen.width / 2 - 100, 15, 200, 50), "Juego Espacial!");
	switch (estado) {
		case 0: 	//Menu principal
			menuPrincipal();
			break;
		case 1:		//Comenzar
			PlayerPrefs.Save();
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
	
	}
	
	//Tooltip
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
			posx += 15;
		}
		if (posy > (Screen.height / 2)) {
			posy += 20;
		}		
		var pos : Rect = Rect(posx, Screen.height - posy, longitud, 25);
		GUI.Box(pos, "");
		GUI.Label(pos, GUI.tooltip);
	}
}

//Menus personalizados --------------------------------------------------------------------------------------------------------------------

function menuPrincipal() {
	GUILayout.BeginArea(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
	GUILayout.BeginVertical();
	if (GUILayout.Button(GUIContent("Comenzar juego", "Comenzar un juego nuevo"), "boton_menu_1")) {
		estado = 1;
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
	GUI.TextArea(Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300), cadenaCreditos);
	if (GUI.Button(Rect(Screen.width - 100, Screen.height - 50, 80, 30),GUIContent("Volver", "Volver al menú"), "boton_atras")) {
		estado = 0;
	}
}

function menuOpciones() {
	GUI.Box(Rect(Screen.width / 2 - 105, Screen.height / 2 - 105, 210, 210),"");
	GUILayout.BeginArea(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
	GUILayout.BeginVertical();
	musicaOn = customToggle(musicaOn, "Musica", "Apagar/Encender música");
	musicaVol = GUILayout.HorizontalSlider(musicaVol, 0.0, 1.0);
	sfxOn = customToggle(sfxOn, "SFX", "Apagar/Encender efectos");
	sfxVol = GUILayout.HorizontalSlider(sfxVol, 0.0, 1.0);
	GUILayout.EndVertical();
	GUILayout.EndArea();
	if (GUI.Button(Rect(Screen.width - 100, Screen.height - 50, 80, 30), GUIContent("Volver", "Volver al menú"), "boton_atras")) {
		estado = 0;
	}
}

function actualizarOpciones() {
//	var opc : Opciones = opcionesPerdurables.GetComponent(Opciones);
	if (musicaOn)
		PlayerPrefs.SetInt("MusicaOn", 1);
	else
		PlayerPrefs.SetInt("MusicaOn", 0); 
//	var opc : Opciones = new Opciones();
//	opc.setMusicaOn(musicaOn);
	PlayerPrefs.SetFloat("MusicaVol", musicaVol);
//	opc.setMusicaVol(musicaVol);
	if (sfxOn)
		PlayerPrefs.SetInt("SfxOn", 1);
	else
		PlayerPrefs.SetInt("SfxOn", 0);
//	opc.setSfxOn(sfxOn);
	PlayerPrefs.SetFloat("SfxVol", sfxVol);
//	opc.setSfxVol(sfxVol);
}

//Controles personalizados -----------------------------------------------------------------------------------------------------------------

function customToggle(bool : boolean, str : String, tool : String) {
	var valor : boolean;
	GUILayout.BeginVertical();
	GUILayout.BeginHorizontal();
	GUILayout.Label(GUIContent(str, tool));
	valor = GUILayout.Toggle(bool, GUIContent("", tool));
	GUILayout.EndHorizontal();
	GUILayout.EndVertical();
	return valor;
}
