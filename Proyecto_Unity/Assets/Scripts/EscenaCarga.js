#pragma strict

var estiloGUI : GUISkin;				//Los estilos a usar para la escena de carga y menús
var texturaFondo : Texture;
private var estado : int = 0;			//0 para menu, 1 para comenzar, 2 para opciones, 3 para creditos, 4 para salir
private var musicaOn : boolean = true;	//Está la música activada?
private var musicaVol : float = 0.5;	//A que volumen?
private var sfxOn : boolean = true;		//Estan los efectos de sonido activados?
private var sfxVol : float = 0.5; 		//A que volumen?
private var miObjeto : Transform;

private var cadenaCreditos : String = "\t Hurricane son: \n Marcos Calleja Fernández\n Aris Goicoechea Lassaletta\n Pablo Pizarro Moleón\n" + 
										"\n\t Música a cargo de:\n Easily Embarrased";

function Awake() {
	miObjeto = this.transform;
}

//function Start() {
//	var async : AsyncOperation = Application.LoadLevelAsync ("Generador_Planeta");
//    yield async;
//    Debug.Log ("Loading complete");
//}

function FixedUpdate() {
	var opSonido : AudioSource = miObjeto.GetComponent(AudioSource);
	opSonido.volume = musicaVol;
	if (!musicaOn && opSonido.isPlaying)
		opSonido.Stop();
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
			Application.LoadLevel("Generador_Planeta");
			break;
		case 2:		//Opciones
			menuOpciones();
			break;
		case 3:		//Creditos
			creditos();
			break;
		case 4:		//Salir
			Application.Quit();
			break;
	
	}
}

function menuPrincipal() {
	GUILayout.BeginArea(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
	GUILayout.BeginVertical();
	if (GUILayout.Button("Comenzar juego", "boton_menu_1")) {
		estado = 1;
	}
	if (GUILayout.Button("Opciones", "boton_menu_2")) {
		estado = 2;
	}
	if (GUILayout.Button("Créditos", "boton_menu_4")) {
		estado = 3;
	}
	if (GUILayout.Button("Salir", "boton_menu_5")) {
		estado = 4;
	}
	GUILayout.EndVertical();
	GUILayout.EndArea();
}

function creditos() {
	GUI.TextArea(Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300), cadenaCreditos);
	if (GUI.Button(Rect(Screen.width - 100, Screen.height - 50, 80, 30), "Volver", "boton_atras")) {
		estado = 0;
	}
}

function menuOpciones() {
	GUILayout.BeginArea(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
	GUILayout.BeginVertical();
	musicaOn = GUILayout.Toggle(musicaOn, "Activar música?");
	musicaVol = GUILayout.HorizontalSlider(musicaVol, 0.0, 1.0);
	sfxOn = GUILayout.Toggle(sfxOn, "Activar efectos?");
	sfxVol = GUILayout.HorizontalSlider(sfxVol, 0.0, 1.0);
	GUILayout.EndVertical();
	GUILayout.EndArea();
	if (GUI.Button(Rect(Screen.width - 100, Screen.height - 50, 80, 30), "Volver")) {
		estado = 0;
	}
}

