#pragma strict


//Variables ---------------------------------------------------------------------------------------------------------------------------

//Para el tablero
//var anchoTablero				: int 		= 128;					//El ancho del tablero lógico (debe ser potencia de 2 para cuadrar con la textura)
//var altoTablero					: int 		= 128;					//El alto del tablero lógico (debe ser potencia de 2 tambien)
//var casillasPolos				: int		= 3;					//El numero de casillas que serán intransitables en los polos
//private var altoTableroUtil		: int;								//El alto del tablero una vez eliminadas las casillas de los polos
//private var margen 				: int 		= 50;					//El numero de pixeles que habrá en los polos intransitables
//var numMaxEspeciesCasilla		: int		= 5;					//Numero maximo de especies que puede haber por casilla a la vez
//var numMaxEspecies				: int		= 20;					//Numero maximo de especies que puede haber en el tablero (juego) a la vez



//GUI
var estiloGUI					: GUISkin;							//Los estilos diferentes para la GUI, configurables desde el editor
var camaraReparaciones			: GameObject;						//Para mostrar las opciones de las reparaciones de la nave
var camaraPrincipal				: GameObject;						//Para mostrar el mundo completo (menos escenas especiales)
private var menuOpcionesInt		: int		= 0;					//Variable de control sobre el menu lateral derecho
private var cuantoW				: int 		= Screen.width / 48;	//Minima unidad de medida de la interfaz a lo ancho (formato 16/10)
private var cuantoH				: int 		= Screen.height / 30;	//Minima unidad de medida de la interfaz a lo alto (formato 16/10)

//Privadas del script
private var estado 				: T_estados = T_estados.principal;	//Los estados por los que pasa el juego
private var nuevoTerreno		: boolean	= false;				//Si se quiere re-generar el terreno, se hace poniendo esto a true
private var tablero				: Casilla[,];						//Tablero lógico del algoritmo

private var contenedorTexturas	: GameObject;						//El contenedor de las texturas de la primera escena

//Opciones
var contenedorSonido			: GameObject;						//El objeto que va a contener la fuente del audio
private var sonido				: AudioSource;						//La fuente del audio

private var musicaOn 			: boolean 	= true;					//Está la música activada?
private var musicaVol 			: float		= 0.5;					//A que volumen?
private var sfxOn 				: boolean 	= true;					//Estan los efectos de sonido activados?
private var sfxVol 				: float 	= 0.5; 					//A que volumen?

//Tooltips
private var posicionMouse 		: Vector3 	= Vector3.zero;			//Guarda la ultima posicion del mouse		
private var activarTooltip 		: boolean 	= false;				//Controla si se muestra o no el tooltip	
private var ultimoMov 			: float 	= 0;					//Ultima vez que se movio el mouse		
var tiempoTooltip 				: float 	= 0.75;					//Tiempo que tarda en aparecer el tooltip	

//Menus para guardar
private var posicionScroll		: Vector2	= Vector2.zero;			//La posicion en la que se encuentra la ventana con scroll
private var numSaves			: int		= 0;					//El numero de saves diferentes que hay en el directorio respectivo
private var numSavesExtra		: int		= 0;					//Numero de saves que hay que no se ven al primer vistazo en la scrollview
private var nombresSaves		: String[];							//Los nombres de los ficheros de savegames guardados

//Tipos especiales ----------------------------------------------------------------------------------------------------------------------

//Añadir los que hagan falta mas tarde
enum T_estados {inicial, principal, laboratorio, reparaciones, filtros, guardar, opciones, salir, regenerar};

//Funciones auxiliares --------------------------------------------------------------------------------------------------------------------

function esperaSegundos(segs : float) {
	yield WaitForSeconds(segs);
}

function pausaJuegoSegs(segs : float) {
	yield StartCoroutine(esperaSegundos(segs));
}

//Funciones principales ----------------------------------------------------------------------------------------------------------------------
function creacionInicial() {
	
	//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
	var planeta : GameObject = GameObject.FindWithTag("Planeta");
	var renderer : MeshRenderer = planeta.GetComponent(MeshRenderer);
	var texturaBase : Texture2D = renderer.sharedMaterial.mainTexture as Texture2D;
	var texturaNorm : Texture2D = renderer.sharedMaterial.GetTexture("_Normals") as Texture2D;	//Los nombres vienen definidos en el editor, en el material
	var texturaMask : Texture2D = renderer.sharedMaterial.GetTexture("_Mask") as Texture2D;
	
	var media : float = 0;
	var pixels : Color[] = new Color[texturaBase.width*texturaBase.height];
	FuncTablero.inicializa(texturaBase);
	
	pixels = FuncTablero.ruidoTextura();										//Se crea el ruido para la textura base y normales...
	pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);		//Se suaviza el borde lateral...
	pixels = FuncTablero.suavizaPoloTex(pixels, texturaBase.height / 20);		//Se suavizan los polos...
	
	media = FuncTablero.calcularMedia(pixels);
	pixels = FuncTablero.realzarRelieve(pixels, media);
	texturaBase.SetPixels(pixels);
	texturaBase.Apply();

	var pixelsAgua : Color[] = FuncTablero.mascaraBumpAgua(pixels, 0.5);			//se ignora el mar para el relieve
	texturaNorm.SetPixels(pixels);													//Se aplican los pixeles a la textura normal para duplicarlos
	texturaNorm.SetPixels32(FuncTablero.creaNormalMap(texturaNorm));				//se transforma a NormalMap
	texturaNorm.Apply();
	texturaMask.SetPixels(pixelsAgua);
	texturaMask.Apply();	
	
	estado = T_estados.principal;
}

//Update y transiciones de estados -------------------------------------------------------------------------------------------------------

function Awake() {
//	creacionInicial();
	contenedorTexturas = GameObject.FindGameObjectWithTag("Carga");
	if (contenedorTexturas == null) {
		creacionInicial();
	}
	else {
		//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
		var planeta : GameObject = GameObject.FindWithTag("Planeta");
		var renderer : MeshRenderer = planeta.GetComponent(MeshRenderer);
		var texturaBase : Texture2D = renderer.sharedMaterial.mainTexture as Texture2D;
		var texturaNorm : Texture2D = renderer.sharedMaterial.GetTexture("_Normals") as Texture2D;	//Los nombres vienen definidos en el editor, en el material
		var texturaMask : Texture2D = renderer.sharedMaterial.GetTexture("_Mask") as Texture2D;
		var temp = contenedorTexturas.GetComponent("ValoresCarga") as ValoresCarga;
		texturaBase = temp.texturaBase;
		texturaNorm = temp.texturaNorm;
		texturaMask = temp.texturaMask;
		texturaBase.Apply();
		texturaNorm.Apply();
		texturaMask.Apply();
	}
	if (PlayerPrefs.GetInt("MusicaOn") == 1)
		musicaOn = true;
	else
		musicaOn = false;
	musicaVol = PlayerPrefs.GetFloat("MusicaVol");
	if (PlayerPrefs.GetInt("SfxOn") == 1)
		sfxOn = true;
	else
		sfxOn = false;
	sfxVol = PlayerPrefs.GetFloat("SfxVol");
	sonido = contenedorSonido.GetComponent(AudioSource);
	sonido.mute = !musicaOn;
	sonido.volume = musicaVol;
	numSaves = SaveLoad.FileCount();
	nombresSaves = new String[numSaves];
	nombresSaves = SaveLoad.getFileNames();
	numSavesExtra = numSaves - 3;
	if (numSavesExtra < 0)
		numSavesExtra = 0;
}

function Update () {

	switch (estado) {
		case T_estados.inicial:
			creacionInicial();
			break;
			
		case T_estados.principal:
			break;
			
		case T_estados.regenerar:
			estado = T_estados.inicial;
			break;
			
		case T_estados.filtros:
			break;
			
		case T_estados.laboratorio:
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
		ultimoMov = Time.time;
	}
	else {
		if (Time.time >= ultimoMov + tiempoTooltip) {
			activarTooltip = true;
		}
	}

}

//Funciones OnGUI---------------------------------------------------------------------------------------------------------------------------

function OnGUI() {
	GUI.skin = estiloGUI;
	switch (estado) {
		case T_estados.inicial:
			GUI.Box(Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Re-Generando!");
			break;
		case T_estados.opciones:
			menuOpciones();
			break;
		case T_estados.principal:
			grupoIzquierda();
			grupoDerecha();
			break;
		case T_estados.guardar:
			menuGuardar();
			break;
		case T_estados.reparaciones:
			menuReparaciones();
			break;
		default:						
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

function grupoIzquierda() {
	GUI.BeginGroup(Rect(5, Screen.height / 2 - 110, 125, 230));
	if (GUI.Button(Rect(0, 0, 126, 79), GUIContent("", "Generar otro planeta") , "d_planeta")) {
		nuevoTerreno = true;
		estado = T_estados.regenerar;
	}
	if (GUI.Button(Rect(0, 79, 126, 70), GUIContent("", "Opciones de cámara"), "d_cam")) {
		menuOpcionesInt = 1;
	}
	if (GUI.Button(Rect(0, 149, 126, 79), GUIContent("", "Opciones generales"), "d_func")) {
		menuOpcionesInt = 2;
	}
	GUI.EndGroup();
}

function grupoDerecha() {
	//TODO Dependiendo de que opción este pulsada, poner un menú u otro!
	if (menuOpcionesInt == 1) {
		GUI.BeginGroup(Rect(Screen.width - 130, Screen.height / 2 - 110, 125, 230));
		if (GUI.Button(Rect(0, 0, 127, 79), GUIContent("", "Click izq. para centrar"), "i_c_fija")) {
			var script : Control_Raton = transform.GetComponent(Control_Raton);
			var objetivo : Transform = GameObject.Find("Planeta").GetComponent(Transform);
			script.cambiarTarget(objetivo);
			script.cambiarEstado(1);
		}
		if (GUI.Button(Rect(0, 79, 127, 70), GUIContent("", "Rotar con click der."), "i_c_rot")) {
			script = transform.GetComponent(Control_Raton);
			objetivo = GameObject.Find("Planeta").GetComponent(Transform);
			script.cambiarTarget(objetivo);
			script.cambiarEstado(0);
		}
		if (GUI.Button(Rect(0, 149, 127, 79), GUIContent("", "Centrar en la luna"), "i_c_3")) {
			script = transform.GetComponent(Control_Raton);
			objetivo = GameObject.Find("Moon").GetComponent(Transform);
			script.cambiarTarget(objetivo);
			script.cambiarEstado(0);
		}
		GUI.EndGroup();
	}
	if (menuOpcionesInt == 2) {
		GUI.BeginGroup(Rect(Screen.width - 130, Screen.height / 2 - 110, 125, 230));
		if (GUI.Button(Rect(0, 0, 126, 79), GUIContent("", "Laboratorio genético"), "i_lab")) {

		}
		if (GUI.Button(Rect(0, 79, 126, 70), GUIContent("", "Visión de la nave"), "i_nav")) {
			camaraPrincipal.GetComponent(Camera).enabled = false;
			camaraReparaciones.GetComponent(Camera).enabled = true;
			estado = T_estados.reparaciones;
		}
		if (GUI.Button(Rect(0, 149, 126, 79), GUIContent("", "Opciones del juego"), "i_fil")) {
			script = transform.GetComponent(Control_Raton);
			script.setInteraccion(false);
			estado = T_estados.opciones;
		}
		GUI.EndGroup();
	}
}

function menuOpciones() {
	GUILayout.BeginArea(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
	GUILayout.BeginVertical();
	if (GUILayout.Button(GUIContent("Salir", "Salir del juego"), "boton_menu_1")) {
		Time.timeScale = 1.0;
		var script : Control_Raton = transform.GetComponent(Control_Raton);
		script.setInteraccion(true);
		estado = T_estados.salir;
	}
	if (GUILayout.Button(GUIContent("Guardar", "Guardar la partida"), "boton_menu_2")) {
		Time.timeScale = 1.0;
		nombresSaves = SaveLoad.getFileNames();
		estado = T_estados.guardar;
	}
	if (GUILayout.Button(GUIContent("Volver", "Volver al juego"), "boton_menu_4")) {
		Time.timeScale = 1.0;
		script = transform.GetComponent(Control_Raton);
		script.setInteraccion(true);
		estado = T_estados.principal;
	}
	GUILayout.EndVertical();
	GUILayout.EndArea();
}

function menuGuardar() {
	GUI.Box(Rect(Screen.width / 2 - 126, Screen.height / 2 - 151, 252, 302), "");
	posicionScroll = GUI.BeginScrollView(Rect(Screen.width / 2 - 125, Screen.height / 2 - 150, 250, 300), posicionScroll, Rect(0, 0, 250, 75 * numSavesExtra));
	if (GUI.Button(Rect(5, 0, 240, 75), GUIContent("Nueva partida salvada", "Guardar una nueva partida"))) {
		var planeta : GameObject = GameObject.FindWithTag("Planeta");
		var renderer : MeshRenderer = planeta.GetComponent(MeshRenderer);
		var texturaBase : Texture2D = renderer.sharedMaterial.mainTexture as Texture2D;
		var fecha : String = System.DateTime.Now.ToString().Replace('\\','').Replace('/','').Replace(' ', '').Replace(':','');
		SaveLoad.cambiaFileName("Partida" + fecha + ".hur");
		SaveLoad.Save(texturaBase);
		//Recuperar estado normal
		Time.timeScale = 1.0;
		var script : Control_Raton = transform.GetComponent(Control_Raton);
		script.setInteraccion(true);
		estado = T_estados.principal;
	}
	for (var i : int = 0; i < numSaves; i++) {
		if (GUI.Button(Rect(5, (i + 1) * 75, 240, 75), GUIContent(nombresSaves[i], "Sobreescribir partida num. " + i))) {
			planeta = GameObject.FindWithTag("Planeta");
			renderer = planeta.GetComponent(MeshRenderer);
			texturaBase = renderer.sharedMaterial.mainTexture as Texture2D;
			SaveLoad.cambiaFileName(nombresSaves[i]);
			SaveLoad.Save(texturaBase);
			//Recuperar estado normal
			Time.timeScale = 1.0;
			script = transform.GetComponent(Control_Raton);
			script.setInteraccion(true);
			estado = T_estados.principal;
		}
	}
	GUI.EndScrollView();
	if (GUI.Button(Rect(Screen.width / 2 -30, Screen.height / 2 + 160, 60, 20), GUIContent("Volver", "Volver a la partida"), "boton_atras")) {
		//Recuperar estado normal
		Time.timeScale = 1.0;
		script = transform.GetComponent(Control_Raton);
		script.setInteraccion(true);
		estado = T_estados.principal;
	}
	
}

function menuReparaciones() {
	if (GUI.Button(Rect(cuantoW, cuantoH * 20, cuantoW * 2, cuantoH), GUIContent("Volver", "boton_atras"))) {
		camaraPrincipal.GetComponent(Camera).enabled = true;
		camaraReparaciones.GetComponent(Camera).enabled = false;
		estado = T_estados.principal;
	}
}

