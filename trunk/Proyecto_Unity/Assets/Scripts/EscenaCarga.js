#pragma strict

var estiloGUI : GUISkin;				//Los estilos a usar para la escena de carga y menús
private var estado : int = 0;			//0 para menu, 1 para comenzar, 2 para opciones, 3 para creditos, 4 para salir

//function Start() {
//	var async : AsyncOperation = Application.LoadLevelAsync ("Generador_Planeta");
//    yield async;
//    Debug.Log ("Loading complete");
//}

function OnGUI() {
	GUI.skin = estiloGUI;
	GUI.Label(Rect(Screen.width / 2 - 150, 15, 400, 50), "Juego Espacial!");
	switch (estado) {
		case 0: 	//Menu principal
			menuPrincipal();
			break;
		case 1:		//Comenzar
			Application.LoadLevel("Generador_Planeta");
			break;
		case 2:		//Opciones
			//TODO Mostrar menu de opciones de manera similar al menu principal
			//menuOpciones();
			break;
		case 3:		//Creditos
			//TODO Mostrar los creditos junto con un boton "Saltar"
			//creditos();
			break;
		case 4:		//Salir
			Application.Quit();
			break;
	
	}
}

function menuPrincipal() {
	GUILayout.BeginArea(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
	GUILayout.BeginVertical();
	if (GUILayout.Button("Comenzar juego")) {
		estado = 1;
	}
	if (GUILayout.Button("Opciones")) {
		estado = 2;
	}
	if (GUILayout.Button("Créditos")) {
		estado = 3;
	}
	if (GUILayout.Button("Salir")) {
		estado = 4;
	}
	GUILayout.EndVertical();
	GUILayout.EndArea();
}