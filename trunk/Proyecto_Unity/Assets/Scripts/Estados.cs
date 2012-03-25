using UnityEngine;
using System.Collections;

//using ;

public class Estados : MonoBehaviour {

	//Variables ---------------------------------------------------------------------------------------------------------------------------

	//GUI
	public GUISkin estiloGUI;											//Los estilos diferentes para la GUI, configurables desde el editor
	public GameObject camaraReparaciones;								//Para mostrar las opciones de las reparaciones de la nave
	public GameObject camaraPrincipal;									//Para mostrar el mundo completo (menos escenas especiales)
	private int menuOpcionesInt					= 0;					//Variable de control sobre el menu lateral derecho
	private int cuantoW							= Screen.width / 48;	//Minima unidad de medida de la interfaz a lo ancho (formato 16/10)
	private int cuantoH							= Screen.height / 30;	//Minima unidad de medida de la interfaz a lo alto (formato 16/10)
	
	//Privadas del script
	private T_estados estado 					= T_estados.principal;	//Los estados por los que pasa el juego
	private Casilla[,] tablero;											//Tablero lógico del algoritmo
	
	private GameObject contenedorTexturas;								//El contenedor de las texturas de la primera escena
	
	//Opciones
	public GameObject contenedorSonido;									//El objeto que va a contener la fuente del audio
	private AudioSource sonido;											//La fuente del audio
	
	private bool musicaOn 						= true;					//Está la música activada?
	private float musicaVol 					= 0.5f;					//A que volumen?
	private bool sfxOn 							= true;					//Estan los efectos de sonido activados?
	private float sfxVol 						= 0.5f; 				//A que volumen?
	
	//Tooltips
	private Vector3 posicionMouse 				= Vector3.zero;			//Guarda la ultima posicion del mouse		
	private bool activarTooltip 				= false;				//Controla si se muestra o no el tooltip	
	private float ultimoMov 					= 0.0f;					//Ultima vez que se movio el mouse		
	public float tiempoTooltip 					= 0.75f;				//Tiempo que tarda en aparecer el tooltip	
	
	//Menus para guardar
	private Vector2 posicionScroll 				= Vector2.zero;			//La posicion en la que se encuentra la ventana con scroll
	private int numSaves 						= 0;					//El numero de saves diferentes que hay en el directorio respectivo
	private int numSavesExtra 					= 0;					//Numero de saves que hay que no se ven al primer vistazo en la scrollview
	private string[] nombresSaves;										//Los nombres de los ficheros de savegames guardados

	//Tipos especiales ----------------------------------------------------------------------------------------------------------------------
	
	//Añadir los que hagan falta mas tarde
	enum T_estados {inicial, principal, laboratorio, reparaciones, filtros, guardar, opciones, salir, regenerar};
	
	//Funciones auxiliares -----------------------------------------------------------------------------------------------------------------------
	private IEnumerator terremoto() {
//		public static void fisura(Texture2D tex, int posX, int posY, float longitud, float magnitud, Vector2 dir) {
//		Vector2 dir = UnityEngine.Random.insideUnitCircle;
//		Vector2 dirTemp = dir * longitud;
//		while ((posY + dirTemp.y) >= tex.height || (posY + dirTemp.y) < 0) {
//			dir = UnityEngine.Random.insideUnitCircle;
//			dirTemp = dir * longitud;
//		}
		Vector2 dir = UnityEngine.Random.insideUnitCircle;
		Vector3 cross1 = new Vector3(dir.x, dir.y, 0);
		Vector3 cross2 = new Vector3(0, 0, 1);
		Vector3 res = Vector3.Cross(cross1, cross2);
		Vector2 dirPerp = new Vector2(res.x, res.y);
		//Hacer un raycast al punto seleccionado o elegir aleatoriamente un punto de la textura para que ocurra ahi el terremoto
		ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
		Vector2 coords = new Vector2(Random.Range(0, temp.texturaBase.width), Random.Range(0, temp.texturaBase.height));
		//TODO Completar esta parte para lanzar las llamadas a FuncTablero.fisura(...)
		//A lo largo de la perpendicular, crear fisuras de forma creciente desde el eje de simetría
		//También puede hacerse paulatinamente respecto al tiempo
		
		FuncTablero.fisura(temp.texturaBase, (int)coords.x, (int)coords.y, 10.0f, -0.6f, dir); 
		yield return new WaitForSeconds(0.1f);
		FuncTablero.fisura(temp.texturaBase, (int)coords.x + (int)dirPerp.normalized.x, (int)coords.y + (int)dirPerp.normalized.y, 6.0f, -0.4f, dir); 
		FuncTablero.fisura(temp.texturaBase, (int)coords.x - (int)dirPerp.normalized.x, (int)coords.y - (int)dirPerp.normalized.y, 6.0f, -0.4f, dir);
	}
	
	private IEnumerator corutinaTerremoto() {
		yield return StartCoroutine("terremoto");
	}
	
	//Funciones principales ----------------------------------------------------------------------------------------------------------------------
	private void creacionInicial() {
		
		//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
		GameObject planeta = GameObject.FindWithTag("Planeta");
		MeshRenderer renderer = planeta.GetComponent<MeshRenderer>();
		Texture2D texturaBase = renderer.sharedMaterial.mainTexture as Texture2D;
		Texture2D texturaNorm = renderer.sharedMaterial.GetTexture("_Normals") as Texture2D;	//Los nombres vienen definidos en el editor, en el material
		
		Color[] pixels = new Color[texturaBase.width * texturaBase.height];
		FuncTablero.inicializa(texturaBase);
		
		pixels = FuncTablero.ruidoTextura();											//Se crea el ruido para la textura base y normales...
		pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);			//Se suaviza el borde lateral...
		pixels = FuncTablero.suavizaPoloTex(pixels, texturaBase.height / 20);			//Se suavizan los polos...
		
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();
	
		texturaNorm.SetPixels(pixels);													//Se aplican los pixeles a la textura normal para duplicarlos
		texturaNorm.SetPixels32(FuncTablero.creaNormalMap(texturaNorm));				//se transforma a NormalMap
		texturaNorm.Apply();
		
		estado = T_estados.principal;
	}
	
	//Update y transiciones de estados -------------------------------------------------------------------------------------------------------
	
	void Awake() {
		contenedorTexturas = GameObject.FindGameObjectWithTag("Carga");
		if (contenedorTexturas == null) {
			creacionInicial();
		}
		else {
			//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
			GameObject planeta = GameObject.FindWithTag("Planeta");
			MeshRenderer renderer = planeta.GetComponent<MeshRenderer>();
			Texture2D texturaBase = renderer.sharedMaterial.mainTexture as Texture2D;
			Texture2D texturaNorm = renderer.sharedMaterial.GetTexture("_Normals") as Texture2D;	//Los nombres vienen definidos en el editor, en el material
			ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
			texturaBase = temp.texturaBase;
			texturaNorm = temp.texturaNorm;
			texturaBase.Apply();
			texturaNorm.Apply();
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
		sonido = contenedorSonido.GetComponent<AudioSource>();
		sonido.mute = !musicaOn;
		sonido.volume = musicaVol;
		//esto para que no salgan warnings molestos
		if (sfxOn) {
			float a = sfxVol;
			sfxVol = a;
		}
		else {
		
		}
		numSaves = SaveLoad.FileCount();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames();
		numSavesExtra = numSaves - 3;
		if (numSavesExtra < 0)
			numSavesExtra = 0;
	}
	
	void Update () {
	
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
	
	void OnGUI() {
		GUI.skin = estiloGUI;
		switch (estado) {
			case T_estados.inicial:
				GUI.Box(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 30, 200, 60), "Re-Generando!");
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
	
	private void grupoIzquierda() {
		GUI.BeginGroup(new Rect(5, Screen.height / 2 - 110, 125, 230));
		if (GUI.Button(new Rect(0, 0, 126, 79), new GUIContent("", "Generar otro planeta") , "d_planeta")) {
			estado = T_estados.regenerar;
		}
		if (GUI.Button(new Rect(0, 79, 126, 70), new GUIContent("", "Opciones de c\u00e1mara"), "d_cam")) {
			menuOpcionesInt = 1;
		}
		if (GUI.Button(new Rect(0, 149, 126, 79), new GUIContent("", "Opciones generales"), "d_func")) {
			menuOpcionesInt = 2;
		}
		GUI.EndGroup();
	}
	
	private void grupoDerecha() {
		//Dependiendo de que opción este pulsada, poner un menú u otro!
		Control_Raton script;
		Transform objetivo;
		if (menuOpcionesInt == 1) {
			GUI.BeginGroup(new Rect(Screen.width - 130, Screen.height / 2 - 110, 125, 230));
			if (GUI.Button(new Rect(0, 0, 127, 79), new GUIContent("", "Click izq. para centrar"), "i_c_fija")) {
				script = transform.parent.GetComponent<Control_Raton>();
				objetivo = GameObject.Find("Planeta").GetComponent<Transform>();
				script.cambiarTarget(objetivo);
				script.cambiarEstado(1);
			}
			if (GUI.Button(new Rect(0, 79, 127, 70), new GUIContent("", "Rotar con click der."), "i_c_rot")) {
				script = transform.parent.GetComponent<Control_Raton>();
				objetivo = GameObject.Find("Planeta").GetComponent<Transform>();
				script.cambiarTarget(objetivo);
				script.cambiarEstado(0);
			}
			if (GUI.Button(new Rect(0, 149, 127, 79), new GUIContent("", "Centrar en la luna"), "i_c_3")) {
				script = transform.parent.GetComponent<Control_Raton>();
				objetivo = GameObject.Find("Moon").GetComponent<Transform>();
				script.cambiarTarget(objetivo);
				script.cambiarEstado(0);
			}
			GUI.EndGroup();
		}
		if (menuOpcionesInt == 2) {
			GUI.BeginGroup(new Rect(Screen.width - 130, Screen.height / 2 - 110, 125, 230));
			if (GUI.Button(new Rect(0, 0, 126, 79), new GUIContent("", "Laboratorio gen\u00e9tico"), "i_lab")) {
	
			}
			if (GUI.Button(new Rect(0, 79, 126, 70), new GUIContent("", "Visi\u00f3n de la nave"), "i_nav")) {
				camaraPrincipal.GetComponent<Camera>().enabled = false;
				camaraReparaciones.GetComponent<Camera>().enabled = true;
				script = transform.parent.GetComponent<Control_Raton>();
				script.setInteraccion(false);
				estado = T_estados.reparaciones;
			}
			if (GUI.Button(new Rect(0, 149, 126, 79), new GUIContent("", "Opciones del juego"), "i_fil")) {
				script = transform.parent.GetComponent<Control_Raton>();
				script.setInteraccion(false);
				estado = T_estados.opciones;
			}
			GUI.EndGroup();
		}
	}
	
	private void menuOpciones() {
		Control_Raton script;
		GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
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
		GUI.Box(new Rect(Screen.width / 2 - 126, Screen.height / 2 - 151, 252, 302), "");
		posicionScroll = GUI.BeginScrollView(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 150, 250, 300), posicionScroll, new Rect(0, 0, 250, 75 * numSavesExtra));
		if (GUI.Button(new Rect(5, 0, 240, 75), new GUIContent("Nueva partida salvada", "Guardar una nueva partida"))) {
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
			if (GUI.Button(new Rect(5, (i + 1) * 75, 240, 75), new GUIContent(nombresSaves[i], "Sobreescribir partida num. " + i))) {
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
		if (GUI.Button(new Rect(Screen.width / 2 -30, Screen.height / 2 + 160, 60, 20), new GUIContent("Volver", "Volver a la partida"), "boton_atras")) {
			//Recuperar estado normal
			Time.timeScale = 1.0f;
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
}
