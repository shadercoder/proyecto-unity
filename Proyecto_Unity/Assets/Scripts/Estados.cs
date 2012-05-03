using UnityEngine;
using System.Collections;

//using ;

public class Estados : MonoBehaviour {

	//Variables ---------------------------------------------------------------------------------------------------------------------------
	
	//Públicas
	public GameObject camaraReparaciones;								//Para mostrar las opciones de las reparaciones de la nave
	public GameObject camaraPrincipal;									//Para mostrar el mundo completo (menos escenas especiales)
	public GUISkin estiloGUI;											//Los estilos diferentes para la GUI, configurables desde el editor
	public GUISkin estiloGUI_Nuevo;										//Estilos para la GUI, nueva versión
	public GameObject objetoOceano;										//El objeto que representa la esfera del oceano
	public GameObject objetoRoca;										//El objeto que representa la esfera de la roca
	public GameObject objetoPlanta;										//El objeto que representa la esfera de las plantas
	public Texture2D texPlantas;										//La textura donde se pintan las plantas 
	public float tiempoPincel					= 0.001f;				//Incremento de tiempo para aplicar el pincel
	public float tiempoTooltip 					= 0.75f;				//Tiempo que tarda en aparecer el tooltip	
	public GameObject sonidoAmbiente;									//El objeto que va a contener la fuente del audio de ambiente
	public GameObject sonidoFX;											//El objeto que va a contener la fuente de efectos de audio
	
	//GUI	
	private int cuantoW							= Screen.width / 48;	//Minima unidad de medida de la interfaz a lo ancho (formato 16/10)
	private int cuantoH							= Screen.height / 30;	//Minima unidad de medida de la interfaz a lo alto (formato 16/10)
		//Botones grandes
	private bool menuAltera						= false;				//Variables de control de los botones grandes
	private bool menuCamara						= false;				//de la interfaz del menu izquierdo
	private bool menuOpcion						= false;
		//Botones pequeños
	private bool botonPequePincel				= false;				//Variables de control de los botones pequeños
	private bool botonPequeSubir				= false;				//de la interfaz del menu izquierdo
	private bool botonPequeBajar				= false;
	private bool botonPequeAllanar				= false;
	
	//Privadas del script
	private T_estados estado 					= T_estados.principal;	//Los estados por los que pasa el juego
	private Vida vida;													//Tablero lógico del algoritmo		
	private GameObject contenedorTexturas;								//El contenedor de las texturas de la primera escena
	private float escalaTiempo					= 1.0f;					//La escala temporal a la que se updateará todo
		//Pinceles
	private bool activarPinceles				= false;				//Variable de control para pintar sobre la textura	
	private int seleccionPincel 				= 0;					//la selección del pincel a utilizar
	private float ultimoPincel					= 0.0f;					//Ultimo pincel aplicado
		//Filtros -----------------
	private float tiempoCasilla					= 0.5f;					//Cuanto tiempo tiene que pasar entre casilla y casilla
	private float ultimaCasilla					= 0.0f;					//Momento en el que se lanzo el ultimo rayo
		//Filtro de especies 
	private bool infoEspecies					= false;				//Indica si se muestra la info de las especies de la casilla
	private string infoEspecies_Hab;									//La primera linea a mostrar en la casilla de info de especies
	private string infoEspecies_Esp;									//La segunda linea de la casilla info especies
		//Filtro de elementos
	private bool infoElems						= false;				//Indica si se muestra la info de los elementos de la casilla
	private string infoElems_Elem;										//Primera linea a mostrar de la info de elementos
		//Algoritmo vida
	private float tiempoPaso					= 0.0f;					//El tiempo que lleva el paso actual del algoritmo
	private int numPasos						= 0;					//Numero de pasos del algoritmo ejecutados
	private bool algoritmoActivado				= false;				//Se encuentra activado el algoritmo de la vida?
		
	//Tooltips
	private Vector3 posicionMouse 				= Vector3.zero;			//Guarda la ultima posicion del mouse		
	private bool activarTooltip 				= false;				//Controla si se muestra o no el tooltip	
	private float ultimoMov 					= 0.0f;					//Ultima vez que se movio el mouse		
	
	//Menus para guardar
	private Vector2 posicionScroll 				= Vector2.zero;			//La posicion en la que se encuentra la ventana con scroll
	private int numSaves 						= 0;					//El numero de saves diferentes que hay en el directorio respectivo
	private int numSavesExtra 					= 0;					//Numero de saves que hay que no se ven al primer vistazo en la scrollview
	private string[] nombresSaves;										//Los nombres de los ficheros de savegames guardados

	//Tipos especiales ----------------------------------------------------------------------------------------------------------------------
	 
	//Añadir los que hagan falta mas tarde
	enum T_estados {inicial, principal, reparaciones, filtros, guardar, opciones, salir};
	
	//Funciones auxiliares -----------------------------------------------------------------------------------------------------------------------
	
	private IEnumerator corutinaPincel() {
		//Interacción con los pinceles
		if (Time.realtimeSinceStartup >= ultimoPincel + tiempoPincel) {
			ultimoPincel = Time.realtimeSinceStartup;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				bool temp = false;
				if (botonPequeSubir)
					temp = true;
				FuncTablero.pintaPincel(hit, seleccionPincel, temp);
				Texture2D texTemp = hit.transform.renderer.sharedMaterial.mainTexture as Texture2D;
				texTemp.Apply();
			}
		}
		yield return new WaitForEndOfFrame();
	}
	
	private void actualizaInfoEspecies() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) {
			Vector2 coordTemp = hit.textureCoord;
			Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			coordTemp.x = (int)((int)(coordTemp.x * tex.width) / FuncTablero.getRelTexTabAncho());
			coordTemp.y = (int)((int)(coordTemp.y * tex.height) / FuncTablero.getRelTexTabAlto());
			Vegetal veg = vida.tablero[(int)coordTemp.x, (int)coordTemp.y].vegetal;
			Animal anim = vida.tablero[(int)coordTemp.x, (int)coordTemp.y].animal;
			T_habitats hab = vida.tablero[(int)coordTemp.x, (int)coordTemp.y].habitat;
			infoEspecies_Hab = "Habitat: " + hab.ToString() + "";
			infoEspecies_Esp = "";
			if (anim != null)
				infoEspecies_Esp += "Animal: " + anim.especie.nombre + "\n";
			if (veg != null)
				infoEspecies_Esp += "Planta: " + veg.especie.nombre;
		}
		else {
			infoEspecies_Esp = "";
			infoEspecies_Hab = "";
		}
	}
	
	private void actualizaInfoElems() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) {
			Vector2 coordTemp = hit.textureCoord;
			Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			coordTemp.x = (int)((int)(coordTemp.x * tex.width) / FuncTablero.getRelTexTabAncho());
			coordTemp.y = (int)((int)(coordTemp.y * tex.height) / FuncTablero.getRelTexTabAlto());
			T_elementos[] elem = vida.tablero[(int)coordTemp.x, (int)coordTemp.y].elementos;
			if (elem.Length > 0) {
				infoElems_Elem = "Encontrado " + elem[0].ToString();
				for (int i = 1; i < elem.Length; i++) {
					infoElems_Elem += ", " + elem[i].ToString() + "\n";	
				}
			}
			else 
				infoElems_Elem = "";
		}
		else {
			infoElems_Elem = "";
		}
	}
	
	//Funciones principales ----------------------------------------------------------------------------------------------------------------------
	private void creacionInicial() {
		Debug.Log("Creando planeta de cero en creacionInicial");
		//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
		Texture2D texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		Texture2D texturaAgua = objetoOceano.renderer.sharedMaterial.mainTexture as Texture2D;
		
		Color[] pixels = new Color[texturaBase.width * texturaBase.height];
		FuncTablero.inicializa(texturaBase);
		
		pixels = FuncTablero.ruidoTextura();											//Se crea el ruido para la textura base y normales...
		pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);			//Se suaviza el borde lateral...
		pixels = FuncTablero.suavizaPoloTex(pixels, texturaBase.height / 20);			//Se suavizan los polos...
		
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();
		
		
		pixels = FuncTablero.calculaTexAgua(pixels);
		texturaAgua.SetPixels(pixels);
		texturaAgua.Apply();
		
		MeshFilter filter = objetoRoca.GetComponent<MeshFilter>();
		MeshFilter filter2 = objetoPlanta.GetComponent<MeshFilter>();
		Mesh meshTemp = filter.mesh;
		meshTemp = FuncTablero.extruyeVertices(meshTemp, texturaBase, 0.5f, objetoRoca.transform.position);
		filter.mesh = meshTemp;
		filter2.mesh = meshTemp;
		estado = T_estados.principal;
	}
	
	//Update y transiciones de estados -------------------------------------------------------------------------------------------------------
	
	void Awake() {		
		Random.seed = System.DateTime.Now.Millisecond;
		contenedorTexturas = GameObject.FindGameObjectWithTag("Carga");
		if (contenedorTexturas == null) {
			creacionInicial();
		}
		else {
			//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez			
			Texture2D texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
			texturaBase = temp.texturaBase;
			texturaBase.Apply();
			objetoOceano.transform.localScale = temp.escalaOceano;
		}
		Audio_Ambience ambiente = sonidoAmbiente.GetComponent<Audio_Ambience>();
		Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX>();
		if (PlayerPrefs.GetInt("MusicaOn") == 1)
			ambiente.activado = true;
		else
			ambiente.activado = false;
		ambiente.volumen = PlayerPrefs.GetFloat("MusicaVol");
		if (PlayerPrefs.GetInt("SfxOn") == 1)
			efectos.activado = true;
		else
			efectos.activado = false;
		efectos.volumen = PlayerPrefs.GetFloat("SfxVol");
		
		Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		Mesh mesh = objetoRoca.GetComponent<MeshFilter>().sharedMesh;
		Casilla[,] tablero = FuncTablero.iniciaTablero(tex, mesh);
		vida = new Vida(tablero, texPlantas, objetoRoca.transform);				
		
		int x = 0;
		int y = 0;
		//ESPECIE VEGETAL => nombre numMaxVegetales numIniVegetales capacidadReproductiva capacidadMigracionLocal capacidadMigracionGlobal int radioMigracion T_habitats habitat canalTextura
		
		EspecieVegetal especie = new EspecieVegetal("musgo",1000,50,50,50,0.1f,8,T_habitats.plain,0,GameObject.FindGameObjectWithTag("arbolesSimples"));
		vida.anadeEspecieVegetal(especie);
		vida.buscaPosicionVaciaVegetal(T_habitats.plain,ref x,ref y);
		vida.anadeVegetal(especie,x,y);	
		Debug.Log("Introducido vegetal "+ especie.nombre +" en la posicion:   x: "+x+"   y: "+y);
		
		EspecieVegetal especie2 = new EspecieVegetal("musgo2",1000,50,50,20,0.1f,15,T_habitats.mountain,1,GameObject.FindGameObjectWithTag("arbolesSimples"));
		vida.anadeEspecieVegetal(especie2);
		vida.buscaPosicionVaciaVegetal(T_habitats.mountain,ref x,ref y);
		vida.anadeVegetal(especie2,x,y);	
		Debug.Log("Introducido vegetal "+ especie2.nombre +" en la posicion:   x: "+x+"   y: "+y);
		
		EspecieVegetal especie3 = new EspecieVegetal("musgo3",1000,50,50,20,0.1f,12,T_habitats.hill,2,GameObject.FindGameObjectWithTag("arbolesSimples"));
		vida.anadeEspecieVegetal(especie3);
		vida.buscaPosicionVaciaVegetal(T_habitats.hill,ref x,ref y);
		vida.anadeVegetal(especie3,x,y);	
		Debug.Log("Introducido vegetal "+ especie3.nombre +" en la posicion:   x: "+x+"   y: "+y);
		
		EspecieVegetal especie4 = new EspecieVegetal("musgo4",1000,50,50,20,0.1f,12,T_habitats.coast,3,GameObject.FindGameObjectWithTag("arbolesSimples"));
		vida.anadeEspecieVegetal(especie4);
		vida.buscaPosicionVaciaVegetal(T_habitats.coast,ref x,ref y);
		vida.anadeVegetal(especie4,x,y);	
		Debug.Log("Introducido vegetal "+ especie4.nombre +" en la posicion:   x: "+x+"   y: "+y);
			
		numSaves = SaveLoad.FileCount();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames();
		numSavesExtra = numSaves - 3;
		if (numSavesExtra < 0)
			numSavesExtra = 0;
	}
	
	void FixedUpdate() {
		//Algoritmo de vida		
		tiempoPaso += Time.deltaTime;		
		if(algoritmoActivado && tiempoPaso > 1.0f) {		//El 1.0f significa que se ejecuta un paso cada 1.0 segundos, cuando la escala temporal esta a 1.0
			vida.algoritmoVida();
			numPasos ++;
			tiempoPaso = 0.0f;
		}
	}
	
	void Update () {
		
		switch (estado) {
			case T_estados.inicial:
				creacionInicial();
				break;
				
			case T_estados.principal:
				break;
				
			case T_estados.filtros:
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
			ultimoMov = Time.realtimeSinceStartup;
		}
		else {
			if (Time.realtimeSinceStartup >= ultimoMov + tiempoTooltip) {
				activarTooltip = true;
			}
		}
		if (GUI.changed) {
			
			//Control del timescale
			Time.timeScale = escalaTiempo;
			Time.fixedDeltaTime = 0.02f * escalaTiempo;
		}
		
		//Estado de los pinceles
		if (botonPequeSubir || botonPequeBajar || botonPequeAllanar) 
			activarPinceles = true;
		else
			activarPinceles = false;
		
		//Info de la casilla
		if ((infoEspecies || infoElems) && (Time.realtimeSinceStartup > ultimaCasilla + tiempoCasilla)) {
			//Especies
			if (infoEspecies) {
				actualizaInfoEspecies();
			}
			//Elementos
			else {
				actualizaInfoElems();
			}
		}
	}
	
	//Funciones OnGUI---------------------------------------------------------------------------------------------------------------------------
	
	void OnGUI() {
		GUI.skin = estiloGUI;
		switch (estado) {
			case T_estados.inicial:
				GUI.skin = estiloGUI;
				GUI.Box(new Rect(cuantoW * 21, cuantoH * 14, cuantoW * 6, cuantoH * 2), "Re-Generando!");
				break;
			case T_estados.opciones:
				GUI.skin = estiloGUI;
				menuOpciones();
				break;
			case T_estados.principal:
				GUI.skin = estiloGUI_Nuevo;
				menuIzquierdaHex();
				break;
			case T_estados.guardar:
				GUI.skin = estiloGUI;
				menuGuardar();
				break;
			case T_estados.reparaciones:
				GUI.skin = estiloGUI;
				menuReparaciones();
				break;
			default:						
				break;
		}
		
		//Control pincel en textura
		//TODO alguna forma de evitar el raycast cuando colisione con la GUI
		if (activarPinceles && Input.GetMouseButton(0) && ((Event.current.type == EventType.MouseDown) || (Event.current.type == EventType.MouseDrag))) {
			StartCoroutine(corutinaPincel());
		}
		
		//Debug del algoritmo
		if(GUI.Button(new Rect(cuantoW * 40, cuantoH * 24,cuantoW * 8,cuantoH * 1), "Activar/Desactivar")) 
			if(algoritmoActivado)
				algoritmoActivado = false;
			else
				algoritmoActivado = true;
		
		//Informacion del debug del algoritmo
		if(GUI.Button(new Rect(cuantoW * 40, cuantoH * 23,cuantoW * 8,cuantoH * 1), "Introducir animal")) 			
		{
			int x=0,y=0;
			vida.buscaPosicionVaciaAnimal(T_habitats.plain,ref x,ref y);
			EspecieAnimal especie = new EspecieAnimal("comemusgo"+vida.numEspeciesAnimales,10,1000,0,5,5,5,tipoAlimentacionAnimal.herbivoro,T_habitats.plain,GameObject.CreatePrimitive(PrimitiveType.Cube));			
			vida.anadeEspecieAnimal(especie);						
			vida.anadeAnimal(especie,x,y);	
			Debug.Log("Introducido animal "+especie.nombre+" en la posicion:   "+"x: "+x+"   y: "+y);		
		}
		GUI.Box(new Rect(cuantoW * 40, cuantoH * 25,cuantoW * 8,cuantoH * 6),new GUIContent("Algoritmo Especies","debug"));
		GUI.Label(new Rect(cuantoW * 41, cuantoH * 26,cuantoW * 7,cuantoH * 2),"Num vegetales: "+vida.vegetales.Count);
		GUI.Label(new Rect(cuantoW * 41, cuantoH * 27,cuantoW * 7,cuantoH * 2),"Num animales: "+vida.animales.Count);
		GUI.Label(new Rect(cuantoW * 41, cuantoH * 28,cuantoW * 7,cuantoH * 2),"Num pasos: "+numPasos);
			
		
		//Info de la casilla
		//Especies
		if (infoEspecies) {
			infoCasillaEspecie();
		}
		//Elementos
		else if (infoElems) {
			infoCasillaElems();
		}		
		
		//Tooltip
		if (activarTooltip) {
			float longitud = GUI.tooltip.Length;
			if (longitud == 0.0f) {
				return;
			}
			else {
				longitud *= 8.5f;
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
	
	private void menuIzquierdaHex() {
		barraInformacion(new Rect(cuantoW,cuantoH,cuantoW * 8, cuantoH * 3));
		escalaTiempo = sliderTiempoCompuesto(new Rect(cuantoW,cuantoH * 4, cuantoW * 8, cuantoH * 3), escalaTiempo);
		botonHexCompuestoAltera(new Rect(cuantoW, cuantoH * 7, cuantoW * 8, cuantoH * 3));
		botonHexCompuestoCamara(new Rect(cuantoW, cuantoH * 10, cuantoW * 8, cuantoH * 3));
		botonHexCompuestoOpciones(new Rect(cuantoW, cuantoH * 13, cuantoW * 8, cuantoH * 3));
		GUI.Box(new Rect(cuantoW, cuantoH * 16, cuantoW * 8, cuantoH * 3), "", "BarraAbajo");
	}
	
	private void menuOpciones() {
		Control_Raton script;
		GUILayout.BeginArea(new Rect(cuantoW * 20, cuantoH * 12, cuantoW * 8, cuantoH * 6));
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
		GUI.Box(new Rect(cuantoW * 14, cuantoH * 7, cuantoW * 20, cuantoH * 16), "");
		posicionScroll = GUI.BeginScrollView(new Rect(cuantoW * 14, cuantoH * 8, cuantoW * 20, cuantoH * 14), posicionScroll, new Rect(0, 0, cuantoW * 20, cuantoH * 4 * numSavesExtra));
		if (GUI.Button(new Rect(cuantoW, 0, cuantoW * 18, cuantoH * 4), new GUIContent("Nueva partida salvada", "Guardar una nueva partida"))) {
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
			if (GUI.Button(new Rect(cuantoW, (i + 1) * cuantoH * 4, cuantoW * 18, cuantoH * 4), new GUIContent(nombresSaves[i], "Sobreescribir partida num. " + i))) {
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
		if (GUI.Button(new Rect(cuantoW * 42, cuantoH * 26, cuantoW * 4, cuantoH * 2), new GUIContent("Volver", "Volver a la partida"), "boton_atras")) {
			//Recuperar estado normal
			Time.timeScale = 1.0f;
			escalaTiempo = 1.0f;
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
	
	private void botonHexCompuestoAltera(Rect pos) {
		
		GUI.Box(new Rect(pos.x,pos.y,pos.width - (pos.width * 3.9f / 12.0f), pos.height), new GUIContent("", "Opciones para alterar el planeta"), "BarraHexGran");
		if (menuAltera)
			GUI.Box(new Rect(pos.x + (pos.width * 7.0f / 12.0f),pos.y + (pos.height * 0.75f / 5.0f),pos.width - (pos.width * 8.1f / 12.0f), pos.height), new GUIContent("", "Opciones (P) para alterar el planeta"), "BarraHexPeq");
		if (GUI.Button(new Rect(pos.x + (pos.width * 3.5f / 12.0f), pos.y + (pos.height * 0.6f / 5.0f), pos.width - (pos.width * 8.3f / 12.0f), pos.height - (pos.height * 0.9f / 5.0f)),"", "BotonGrandeAlterar")) {
			menuAltera = true;
			menuCamara = false;
			menuOpcion = false;
		}
		if (menuAltera) {
			botonPequeSubir = GUI.Toggle(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 0.95f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), botonPequeSubir, new GUIContent("", "Eleva el terreno pulsado"), "BotonPequeSubir");
			if (GUI.changed && botonPequeSubir) {
				botonPequeBajar = false;
				botonPequeAllanar = false;
			}
			botonPequeBajar = GUI.Toggle(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 1.85f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), botonPequeBajar, new GUIContent("", "Hunde el terreno pulsado"), "BotonPequeBajar");
			if (GUI.changed && botonPequeBajar) {
				botonPequeSubir = false;
				botonPequeAllanar = false;
			}
			botonPequeAllanar = GUI.Toggle(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 2.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), botonPequeAllanar, new GUIContent("", "Allana el terreno pulsado"), "BotonPequeAllanar");
			if (GUI.changed && botonPequeAllanar) {
				botonPequeSubir = false;
				botonPequeBajar = false;
				Debug.Log("Pulsado boton allanar. Sin funcionalidad aun.");
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 3.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("", "Selecciona el pincel para el terreno"), "BotonPequePinceles")) {
				botonPequePincel = true;
			}
			if (botonPequePincel) {
				seleccionPincel = cajaSeleccionPincel(seleccionPincel);
				if (GUI.changed) {
					botonPequePincel = false;
				}
			}
		}
	}
	
	private void botonHexCompuestoCamara(Rect pos) {
		Control_Raton script;
		GUI.Box(new Rect(pos.x,pos.y,pos.width - (pos.width * 3.9f / 12.0f), pos.height), new GUIContent("", "Opciones para cambiar la camara"), "BarraHexGran");
		if (menuCamara)
			GUI.Box(new Rect(pos.x + (pos.width * 7.0f / 12.0f),pos.y + (pos.height * 0.75f / 5.0f),pos.width - (pos.width * 8.1f / 12.0f), pos.height), new GUIContent("", "Opciones (P) para cambiar la camara"), "BarraHexPeq");
		if (GUI.Button(new Rect(pos.x + (pos.width * 3.5f / 12.0f), pos.y + (pos.height * 0.6f / 5.0f), pos.width - (pos.width * 8.3f / 12.0f), pos.height - (pos.height * 0.9f / 5.0f)), "", "BotonGrandeCamara")) {
			menuAltera = false;
			menuCamara = true;
			menuOpcion = false;
			botonPequeSubir = false;
			botonPequeBajar = false;
			botonPequeAllanar = false;
		}
		if (menuCamara) {
			if (GUI.Button(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 0.95f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("", "Vista del planeta"), "BotonPequePlaneta")) {
				Debug.Log("Pulsado peque camara 1-4");
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 1.85f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("", "Vista de la nave"), "BotonPequeNave")) {
				camaraPrincipal.GetComponent<Camera>().enabled = false;
				camaraReparaciones.GetComponent<Camera>().enabled = true;
				script = transform.parent.GetComponent<Control_Raton>();
				script.setInteraccion(false);
				estado = T_estados.reparaciones;
			}
			infoEspecies = GUI.Toggle(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 2.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), infoEspecies, new GUIContent("", "Vista de las especies"), "BotonPequeEspecies");
			if (GUI.changed && infoEspecies) {
				//Activar información de las especies de la casilla
				infoElems = false;
			}
			infoElems = GUI.Toggle(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 3.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), infoElems, new GUIContent("", "Vista de los elementos"), "BotonPequeCristales");
			if (GUI.changed && infoElems) {	
				//Activar información de los elementos de la casilla
				infoEspecies = false;
			}
		}
	}
	
	private void botonHexCompuestoOpciones(Rect pos) {
		Control_Raton script;
		GUI.Box(new Rect(pos.x,pos.y,pos.width - (pos.width * 3.9f / 12.0f), pos.height), new GUIContent("", "Opciones generales"), "BarraHexGran");
		if (menuOpcion)
			GUI.Box(new Rect(pos.x + (pos.width * 7.0f / 12.0f),pos.y + (pos.height * 0.75f / 5.0f),pos.width - (pos.width * 8.1f / 12.0f), pos.height), new GUIContent("", "Opciones (P) generales"), "BarraHexPeq");
		if (GUI.Button(new Rect(pos.x + (pos.width * 3.5f / 12.0f), pos.y + (pos.height * 0.6f / 5.0f), pos.width - (pos.width * 8.3f / 12.0f), pos.height - (pos.height * 0.9f / 5.0f)), "Opciones", "BotonGrandeOpciones")) {
			menuAltera = false;
			menuCamara = false;
			menuOpcion = true;
			botonPequeSubir = false;
			botonPequeBajar = false;
			botonPequeAllanar = false;
		}
		if (menuOpcion) {
			if (GUI.Button(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 0.95f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("Peq1", "Opciones (P)(B) generales"), "BotonVacio")) {
				Debug.Log("Pulsado peque opciones 1-4");
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 1.85f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("Guardar", "Guardar la partida actual"), "BotonVacio")) {
				Time.timeScale = 1.0f;
				nombresSaves = SaveLoad.getFileNames();
				estado = T_estados.guardar;
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 7.7f / 12.0f), pos.y + (pos.height * 2.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("Volver", "Volver al juego con normalidad."), "BotonVacio")) {
				Debug.Log("Pulsado peque opciones 3-4");
			}
			if (GUI.Button(new Rect(pos.x + (pos.width * 9.1f / 12.0f), pos.y + (pos.height * 3.75f / 5.0f), (pos.width * 1.4f / 12.0f), (pos.height * 1.7f / 5.0f)), new GUIContent("Salir", "Salir al menu del juego."), "BotonVacio")) {
				Time.timeScale = 1.0f;
				script = transform.parent.GetComponent<Control_Raton>();
				script.setInteraccion(true);
				estado = T_estados.salir;
			}
		}
	}
	
	private float sliderTiempoCompuesto(Rect pos, float valor) {
		float valorOut;
		GUI.Box(pos, new GUIContent("", "En que tiempo nos encontramos"), "BarraSlider");
		valorOut = GUI.HorizontalSlider(new Rect(pos.x + (pos.width * 3.3f / 12.0f), pos.y + (pos.height * 1.7f / 5.0f), pos.width - (pos.width * 4.3f / 12.0f), pos.height - (pos.height * 2.0f / 5.0f)), valor, 0.2f, 99.9f);
		GUI.Label(new Rect(pos.x + (pos.width * 4.3f / 12.0f), pos.y + (pos.height * 2.9f / 5.0f), pos.width - (pos.width * 4.3f / 12.0f), pos.height - (pos.height * 2.0f / 5.0f)), "Escala temporal: " + Time.timeScale.ToString("0#.0"));
		return valorOut;
	}
	
	private void barraInformacion(Rect pos) {
		GUI.Box(pos, new GUIContent("", "Tiempo en el que te encuentras"), "BarraTiempo");
		string temp = Time.time.ToString("#.0");
		GUI.Label(new Rect(pos.x + (pos.width * 7.0f / 12.0f), pos.y + (pos.height * 2.0f / 5.0f), (pos.width * 3.0f / 12.0f), (pos.height * 2.0f / 5.0f)), temp);
	}
	
	private int cajaSeleccionPincel(int entrada) {
		GUI.Box(new Rect(cuantoW * 18, cuantoH * 25, cuantoW * 12.5f, cuantoH * 5), "", "CajaPinceles");
		if (GUI.Button(new Rect(cuantoW * 19.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelCrater")) {
			GUI.changed = true;
			return 0;
		}
		if (GUI.Button(new Rect(cuantoW * 21.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelVolcan")){
			GUI.changed = true;
			return 1;
		}
		if (GUI.Button(new Rect(cuantoW * 23.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelDuro")){
			GUI.changed = true;
			return 2;
		}
		if (GUI.Button(new Rect(cuantoW * 25.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelSuave")){
			GUI.changed = true;
			return 3;
		}
		if (GUI.Button(new Rect(cuantoW * 27.5f, cuantoH * 27.0f, cuantoW * 1.5f, cuantoH * 1.5f), "", "PincelIrregular")){
			GUI.changed = true;
			return 4;
		}
		return entrada;
	}
	
	private void infoCasillaEspecie() {
		GUI.Box(new Rect(cuantoW * 40, cuantoH * 20, cuantoW * 8, cuantoH * 4), new GUIContent("", "Informacion de la casilla"));
		GUI.Label(new Rect(cuantoW * 40, cuantoH * 20.2f, cuantoW * 8, cuantoH * 1.6f), infoEspecies_Hab);
		GUI.Label(new Rect(cuantoW * 40, cuantoH * 22.0f, cuantoW * 8, cuantoH * 1.6f), infoEspecies_Esp);
	}
	
	private void infoCasillaElems() {
		GUI.Box(new Rect(cuantoW * 40, cuantoH * 20, cuantoW * 8, cuantoH * 4), new GUIContent("", "Informacion de la casilla"));
		GUI.Label(new Rect(cuantoW * 40, cuantoH * 20.2f, cuantoW * 8, cuantoH * 1.6f), infoElems_Elem);
	}

}
