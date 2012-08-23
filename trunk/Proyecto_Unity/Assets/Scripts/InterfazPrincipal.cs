using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InterfazPrincipal : MonoBehaviour
{

	// Variables ---------------------------------------------------------------------------------------------------------------------------
	public GUISkin estilo;
	private float cuantoW;										//Minima unidad de medida de la interfaz a lo ancho
	private float cuantoH;										//Minima unidad de medida de la interfaz a lo alto
	private float aspectRatioNumerico;							//Aspect ratio númerico de la ventana
	private Principal principal;								//Acceso a los datos principales	
	private bool mostrarBloqueIzquierdo 		= true;			//Visibilidad del bloque de opciones izquierdo
	private bool mostrarInfoCasilla 			= true;			//Controla si se muestra la info básica de la casilla a la que estamos apuntando
	private string infoCasilla 					= "";			//Información básica de la casilla mostrada en la barra de información inferior
	private float tiempoUltimaInfoCasilla 		= 0.0f;			//Tiempo de la última comprobación de la info básica de una casilla
	private float tiempoInfoCasilla 			= 0.25f;		//Cantidad mínima de tiempo entre comprobaciones de la info básica de una casilla
	private Vector3 posicionMouseInfoCasilla 	= Vector3.zero;	//Guarda la ultima posicion del mouse para calcular los tooltips	
	private float escalaTiempoAntesMenu;						//Guarda la escala de tiempo que esta seleccionada al entrar al menu para restablecerla después
	private MejorasNave mejoras;								//El script de mejoras de la nave
	private Materiales materiales;								//el script Materiales de los filtros
	private float tiempoUltimoModeloInsercion 	= 0.0f;			//Cantidad de tiempo entre comprobaciones de inserción de un ser
	private float tiempoModeloInsercion 		= 0.1f;			//Cantidad de tiempo entre comprobaciones de inserción de un ser
	private GameObject modeloInsercion;							//Modelo usado temporalmente para mostrar donde se insertaría un ser
	private bool[] togglesFiltros;								//Toggles en los filtros: 0-3 recursos, 4-13 plantas, 14-23 animales
	private bool[] togglesFiltrosOld;							//Toggles en los filtros antes de cambiarlos
		//Seleccion de seres
	private List<string> infoSeleccion;							//Contiene la informacion que se mostrará en el bloque derecho concerniente a la seleccion
	private float[] habitabilidadSeleccion;						//La habitabilidad del ser o edificio seleccionado
	private int tipoSeleccion					= -1;			//Que se ha seleccionado? 0->Planta, 1->Herbivoro, 2->Carnivoro, 3->Edificio
	
	//Mejoras posibles
	private bool mejoraInfoCasilla 				= true;			//La barra inferior de informacion se muestra?
	private bool mostrarInfoHabitat				= true;			//Se muestra informacion de habitats en ella?
	private bool mostrarInfoMetalesRaros 		= true;			//Se muestran los metales raros?
	private bool mostrarInfoSeres 				= true;			//Se muestran los animales y plantas?
	
	//Tooltips
	private Vector3 posicionMouseTooltip 		= Vector3.zero;	//Guarda la ultima posicion del mouse para calcular los tooltips	
	private bool activarTooltip 				= false;		//Controla si se muestra o no el tooltip	
	private float ultimoMov 					= 0.0f;			//Ultima vez que se movio el mouse		
	private float tiempoTooltip 				= 0.75f;		//Tiempo que tarda en aparecer el tooltip	
	private bool forzarTooltip 					= false;		//Fuerza que se muestre un tooltip en una situación especial
	private string mensajeForzarTooltip 		= "";			//Mensaje del tooltip forzado.
	
	//Enumerados
	private enum taspectRatio
	{
		//Aspecto ratio con el que se pintará la ventana. Si no es ninguno de ellos se aproximará al más cercano
		aspectRatio16_9,
		aspectRatio16_10,
		aspectRatio4_3
	}
	private taspectRatio aspectRatio;

	private enum taccion
	{
		//Acción que se esta realizando en el momento actual
		ninguna,
		seleccionarVegetal,
		seleccionarAnimal,
		seleccionarEdificio,
		seleccionarMejora,
		seleccionarHabilidad,
		insertar,
		mostrarInfoDetallada,
		mostrarMenu
	}
	private taccion accion = taccion.ninguna;
	private taccion accionAnterior = taccion.ninguna;

	private enum taccionMenu
	{
		//Acción que se esta realizando en el menu
		mostrarMenu,
		mostrarGuardar,
		mostrarOpcionesAudio,
		mostrarSalirMenuPrincipal,
		mostrarSalirJuego
	}
	private taccionMenu accionMenu = taccionMenu.mostrarMenu;

	/*private enum tcategoriaSeleccion						//Desactivado indica que no hay seleccion en curso, otro valor indica la categoria de la seleccion
		{desactivada,animal,vegetal,edificio,mejoras,habilidades}
	private tcategoriaSeleccion categoriaSeleccion = tcategoriaSeleccion.desactivada;*/

	private enum telementoInsercion
	{
		//Tipo de elemento seleccionado en un momento dado
		ninguno,
		fabricaCompBas,
		centralEnergia,
		granja,
		fabricaCompAdv,
		centralEnergiaAdv,
		seta,
		flor,
		cana,
		arbusto,
		estromatolito,
		cactus,
		palmera,
		pino,
		cipres,
		pinoAlto,
		herbivoro1,
		herbivoro2,
		herbivoro3,
		herbivoro4,
		herbivoro5,
		carnivoro1,
		carnivoro2,
		carnivoro3,
		carnivoro4,
		carnivoro5
	}
	private telementoInsercion elementoInsercion = telementoInsercion.ninguno;
	private telementoInsercion elementoInsercionDerecho = telementoInsercion.ninguno;

	private enum tMenuDerecho
	{
		ninguno,
		filtroAnimales,
		filtroRecursos,
		filtroVegetales,
		insercion,
		seleccion
	}
	private tMenuDerecho tipoMenuDerecho = tMenuDerecho.ninguno;

	//Menus para guardar
	private Vector2 posicionScroll 		= Vector2.zero;		//La posicion en la que se encuentra la ventana con scroll
	private int numSaves 				= 0;				//El numero de saves diferentes que hay en el directorio respectivo
	private int numSavesExtra 			= 0;				//Numero de saves que hay que no se ven al primer vistazo en la scrollview
	private string[] nombresSaves;							//Los nombres de los ficheros de savegames guardados
	//Sonido
	private float musicaVol 			= 0.0f;				//A que volumen la musica?
	private float sfxVol 				= 0.0f;				//A que volumen los efectos?
	public GameObject sonidoAmbiente;						//El objeto que va a contener la fuente del audio de ambiente
	public GameObject sonidoFX;								//El objeto que va a contener la fuente de efectos de audio
	void Start ()
	{
		principal = gameObject.GetComponent<Principal> ();
		mejoras = GameObject.FindGameObjectWithTag ("Mejoras").GetComponent<MejorasNave> ();
		materiales = GameObject.FindGameObjectWithTag("Materiales").GetComponent<Materiales> ();
		togglesFiltros = new bool[24];
		togglesFiltrosOld = new bool[24];
		habitabilidadSeleccion = new float[9];
		infoSeleccion = new List<string>();
		//Cargar la información del numero de saves que hay
		SaveLoad.compruebaRuta ();
		numSaves = SaveLoad.FileCount ();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames ();
		numSavesExtra = numSaves - 3;
		if (numSavesExtra < 0)
			numSavesExtra = 0;
		//Cargar la informacion del sonido que hay en PlayerPrefs
		
		if (PlayerPrefs.HasKey ("MusicaVol"))
			musicaVol = PlayerPrefs.GetFloat ("MusicaVol");
		if (PlayerPrefs.HasKey ("SfxVol"))
			sfxVol = PlayerPrefs.GetFloat ("SfxVol");
		if (PlayerPrefs.HasKey ("MusicaOn") && (PlayerPrefs.GetInt ("MusicaOn") == 0))
			musicaVol = 0.0f;
		if (PlayerPrefs.HasKey ("SfxOn") && (PlayerPrefs.GetInt ("SfxOn") == 0))
			sfxVol = 0.0f;
		Audio_Ambiente ambiente = sonidoAmbiente.GetComponent<Audio_Ambiente> ();
		if (PlayerPrefs.HasKey ("MusicaOn") && (PlayerPrefs.GetInt ("MusicaOn") == 0))
			ambiente.activado = false;
		else
			ambiente.activado = true;
		ambiente.volumen = musicaVol;
		//Volumen del audio de efectos
		Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX> ();
		if (PlayerPrefs.HasKey ("SfxOn") && (PlayerPrefs.GetInt ("SfxOn") == 0))
			efectos.activado = false;
		else
			efectos.activado = true;
		efectos.volumen = sfxVol;
	}

	// Update is called once per frame
	void Update ()
	{
		controlTooltip ();
		calculaInfoCasilla ();
	}

	void OnGUI ()
	{
		if (accionAnterior != accion) {
			forzarTooltip = false;
			actualizarEstilosBotones ();
			if (accionAnterior == taccion.insertar && modeloInsercion != null)
				Destroy (modeloInsercion);
			accionAnterior = accion;
		}
		GUI.skin = estilo;
		aspectRatioNumerico = (float)Screen.width / (float)Screen.height;
		//16:9
		if (aspectRatioNumerico >= 1.69) {
			aspectRatio = taspectRatio.aspectRatio16_9;
			cuantoW = (float)Screen.width / 80;
			cuantoH = (float)Screen.height / 45;
		//16:10
		} else if (aspectRatioNumerico >= 1.47) {
			aspectRatio = taspectRatio.aspectRatio16_10;
			cuantoW = (float)Screen.width / 80;
			cuantoH = (float)Screen.height / 50;
		//4:3
		} else {
			aspectRatio = taspectRatio.aspectRatio4_3;
			cuantoW = (float)Screen.width / 80;
			cuantoH = (float)Screen.height / 60;
		}
		if (accion == InterfazPrincipal.taccion.mostrarMenu)
			bloqueMenu ();
		bloqueSuperior ();
		bloqueIzquierdo ();
		bloqueSeleccion ();
		bloqueInformacion ();
		bloqueDerecho ();
		
		if (posicionFueraDeInterfaz (Input.mousePosition)) {
			mostrarInfoCasilla = true;
			if (accion == taccion.insertar)
				insertarElemento ();
			else if (Input.GetMouseButtonDown(0)) {
				//Se ha hecho click en el tablero sin insertar nada
				if (seleccionarObjetoTablero())
					tipoMenuDerecho = InterfazPrincipal.tMenuDerecho.seleccion;
//				else
//					tipoMenuDerecho = InterfazPrincipal.tMenuDerecho.ninguno;
			}
		} else
			mostrarInfoCasilla = false;
		
		if (forzarTooltip)
			GUI.Box (new Rect (Input.mousePosition.x - cuantoW, Screen.height - Input.mousePosition.y - cuantoH, cuantoW * 2, cuantoH * 2), new GUIContent ("", mensajeForzarTooltip), "");
		if (activarTooltip)
			mostrarToolTip ();
		
	}

	//Dibuja el bloque superior de la ventana que contiene: tiempo, control velocidad, conteo de recursos y menu principal
	private void bloqueSuperior ()
	{
		float ajusteRecursos = 0;
		GUI.BeginGroup (new Rect (cuantoW * 0, cuantoH * 0, cuantoW * 80, cuantoH * 4));
		GUI.Box (new Rect (cuantoW * 0, cuantoH * 0, cuantoW * 73, cuantoH * 4), "", "BloqueSuperior");
		//Tiempo
		GUI.Label (new Rect (cuantoW * 2, cuantoH * 0, cuantoW * 6, cuantoH * 2), principal.numPasos.ToString (), "EtiquetaTiempo");
		/* [Aris]
		 * He creado el metodo para formatear la fecha y que no salga solo un numero cutre, pero ahora mismo
		 * no hay espacio en la cabecera. Lo dejo aqui comentado para que comentes una y descomentes la otra
		 * si te animas ;)
		 */		
//		GUI.Label(new Rect(cuantoW*2,cuantoH*0,cuantoW*6,cuantoH*2),FuncTablero.formateaFechaPasos(principal.numPasos),"EtiquetaTiempo");
		//Velocidad
		if (GUI.Button (new Rect (cuantoW * 3, cuantoH * 2, cuantoW * 1, cuantoH * 1), new GUIContent ("", "Pausa el juego"), "BotonPausa"))
			principal.setEscalaTiempo (0.0f);
		if (GUI.Button (new Rect (cuantoW * 4, cuantoH * 2, cuantoW * 1, cuantoH * 1), new GUIContent ("", "Velocidad normal"), "BotonVelocidad1"))
			principal.setEscalaTiempo (1.0f);
		if (GUI.Button (new Rect (cuantoW * 5, cuantoH * 2, cuantoW * 1, cuantoH * 1), new GUIContent ("", "Velocidad 2x"), "BotonVelocidad2"))
			principal.setEscalaTiempo (2.0f);
		if (GUI.Button (new Rect (cuantoW * 6, cuantoH * 2, cuantoW * 1, cuantoH * 1), new GUIContent ("", "Velocidad 5x"), "BotonVelocidad5"))
			principal.setEscalaTiempo (20.0f);
		//Energia
		GUI.Box (new Rect (cuantoW * 12, cuantoH * 0, cuantoW * 2, cuantoH * 2), new GUIContent ("", "Energía"), "IconoEnergia");
		GUI.Box (new Rect (cuantoW * 14, cuantoH * 0, cuantoW * 7, cuantoH * 2), new GUIContent ("", principal.energia.ToString () + "/" + principal.energiaMax.ToString ()), "BoxEnergia");
		ajusteRecursos = (System.Math.Abs (principal.energiaDif) < 100) ? 0 : 0.5f;
		GUI.Label (new Rect (cuantoW * 14, cuantoH * 0, cuantoW * (5 - ajusteRecursos), cuantoH * 2), principal.energia.ToString (), "LabelEnergia");
		if (principal.energiaDif >= 0)
			GUI.Label (new Rect (cuantoW * (19 - ajusteRecursos), cuantoH * 0, cuantoW * (2 + ajusteRecursos), cuantoH * 2), "+" + principal.energiaDif.ToString (), "LabelRecursosDifVerde");
		else
			GUI.Label (new Rect (cuantoW * (19 - ajusteRecursos), cuantoH * 0, cuantoW * (2 + ajusteRecursos), cuantoH * 2), "" + principal.energiaDif.ToString (), "LabelRecursosDifRojo");
		//Componentes basicos
		GUI.Box (new Rect (cuantoW * 22, cuantoH * 0, cuantoW * 2, cuantoH * 2), new GUIContent ("", "Componentes básicos"), "IconoCompBas");
		GUI.Box (new Rect (cuantoW * 24, cuantoH * 0, cuantoW * 7, cuantoH * 2), new GUIContent ("", principal.componentesBasicos.ToString () + "/" + principal.componentesBasicosMax.ToString ()), "BoxCompBas");
		ajusteRecursos = (System.Math.Abs (principal.componentesBasicosDif) < 100) ? 0 : 0.5f;
		GUI.Label (new Rect (cuantoW * 24, cuantoH * 0, cuantoW * (5 - ajusteRecursos), cuantoH * 2), principal.componentesBasicos.ToString (), "LabelCompBas");
		if (principal.componentesBasicosDif >= 0)
			GUI.Label (new Rect (cuantoW * (29 - ajusteRecursos), cuantoH * 0, cuantoW * (2 + ajusteRecursos), cuantoH * 2), "+" + principal.componentesBasicosDif.ToString (), "LabelRecursosDifVerde");
		else
			GUI.Label (new Rect (cuantoW * (29 - ajusteRecursos), cuantoH * 0, cuantoW * (2 + ajusteRecursos), cuantoH * 2), "" + principal.componentesBasicosDif.ToString (), "LabelRecursosDifRojo");
		//Componentes avanzados
		GUI.Box (new Rect (cuantoW * 32, cuantoH * 0, cuantoW * 2, cuantoH * 2), new GUIContent ("", "Componentes avanzados"), "IconoCompAdv");
		GUI.Box (new Rect (cuantoW * 34, cuantoH * 0, cuantoW * 7, cuantoH * 2), new GUIContent ("", principal.componentesAvanzados.ToString () + "/" + principal.componentesAvanzadosMax.ToString ()), "BoxCompAdv");
		ajusteRecursos = (System.Math.Abs (principal.componentesAvanzadosDif) < 100) ? 0 : 0.5f;
		GUI.Label (new Rect (cuantoW * 34, cuantoH * 0, cuantoW * (5 - ajusteRecursos), cuantoH * 2), principal.componentesAvanzados.ToString (), "LabelCompAdv");
		if (principal.componentesAvanzadosDif >= 0)
			GUI.Label (new Rect (cuantoW * (39 - ajusteRecursos), cuantoH * 0, cuantoW * (2 + ajusteRecursos), cuantoH * 2), "+" + principal.componentesAvanzadosDif.ToString (), "LabelRecursosDifVerde");
		else
			GUI.Label (new Rect (cuantoW * (39 - ajusteRecursos), cuantoH * 0, cuantoW * (2 + ajusteRecursos), cuantoH * 2), "" + principal.componentesAvanzadosDif.ToString (), "LabelRecursosDifRojo");
		//Material biologico
		GUI.Box (new Rect (cuantoW * 42, cuantoH * 0, cuantoW * 2, cuantoH * 2), new GUIContent ("", "Material biológico"), "IconoMatBio");
		GUI.Box (new Rect (cuantoW * 44, cuantoH * 0, cuantoW * 7, cuantoH * 2), new GUIContent ("", principal.materialBiologico.ToString () + "/" + principal.materialBiologicoMax.ToString ()), "BoxMatBio");
		ajusteRecursos = (System.Math.Abs (principal.materialBiologicoDif) < 100) ? 0 : 0.5f;
		GUI.Label (new Rect (cuantoW * 44, cuantoH * 0, cuantoW * (5 - ajusteRecursos), cuantoH * 2), principal.materialBiologico.ToString (), "LabelMatBio");
		if (principal.materialBiologicoDif >= 0)
			GUI.Label (new Rect (cuantoW * (49 - ajusteRecursos), cuantoH * 0, cuantoW * (2 + ajusteRecursos), cuantoH * 2), "+" + principal.materialBiologicoDif.ToString (), "LabelRecursosDifVerde");
		else
			GUI.Label (new Rect (cuantoW * (49 - ajusteRecursos), cuantoH * 0, cuantoW * (2 + ajusteRecursos), cuantoH * 2), "" + principal.materialBiologicoDif.ToString (), "LabelRecursosDifRojo");
		//Menu
		if (GUI.Button (new Rect (cuantoW * 73, cuantoH * 0, cuantoW * 7, cuantoH * 4), new GUIContent ("", "Accede al menu del juego"), "BotonMenu")) {
			escalaTiempoAntesMenu = principal.escalaTiempo;
			principal.setEscalaTiempo (0);
			accion = InterfazPrincipal.taccion.mostrarMenu;
			accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
		}
		GUI.EndGroup ();
	}

	//Dibuja el bloque izquierdo de la ventana que contiene: insertar vegetales o animales, insertar edificios, mejoras de la nave, habilidades, info/seleccionar
	private void bloqueIzquierdo ()
	{
		int posicionBloque = 0;
		switch (aspectRatio) {
		case taspectRatio.aspectRatio16_9:
			posicionBloque = 17;
			break;
		case taspectRatio.aspectRatio16_10:
			posicionBloque = 20;
			break;
		case taspectRatio.aspectRatio4_3:
			posicionBloque = 25;
			break;
		default:
			break;
		}
		
		GUILayout.BeginArea (new Rect (cuantoW * 0, cuantoH * posicionBloque, cuantoW * 3, cuantoH * 10), new GUIContent (), "BloqueIzquierdo");
		GUILayout.BeginHorizontal ();
		if (mostrarBloqueIzquierdo) {
			GUILayout.BeginVertical (GUILayout.Height (cuantoH * 10), GUILayout.Width (cuantoH * 2));
			if (GUILayout.Button (new GUIContent ("", "Accede al menu de insertar vegetales"), "BotonInsertarVegetales")) {
				if (accion != taccion.seleccionarVegetal)
					accion = taccion.seleccionarVegetal;
				else
					accion = taccion.ninguna;
			}
			if (GUILayout.Button (new GUIContent ("", "Accede al menu de insertar animales"), "BotonInsertarAnimales")) {
				if (accion != taccion.seleccionarAnimal)
					accion = taccion.seleccionarAnimal;
				else
					accion = taccion.ninguna;
			}
			if (GUILayout.Button (new GUIContent ("", "Accede al menu de construir edificios"), "BotonInsertarEdificios")) {
				if (accion != taccion.seleccionarEdificio)
					accion = taccion.seleccionarEdificio;
				else
					accion = taccion.ninguna;
			}
			if (GUILayout.Button (new GUIContent ("", "Accede al menu de mejoras de la nave"), "BotonAccederMejoras")) {
				if (accion != taccion.seleccionarMejora)
					accion = taccion.seleccionarMejora;
				else
					accion = taccion.ninguna;
			}
			if (GUILayout.Button (new GUIContent ("", "Accede al menu de habilidades"), "BotonAccederHabilidades")) {
				if (accion != taccion.seleccionarHabilidad)
					accion = taccion.seleccionarHabilidad;
				else
					accion = taccion.ninguna;
			}
			GUILayout.EndVertical ();
			if (GUILayout.Button (new GUIContent ("", "Pulsa para ocultar este menu"), "BotonOcultarBloqueIzquierdo", GUILayout.Height (cuantoH * 10), GUILayout.Width (cuantoH * 1))) {
				mostrarBloqueIzquierdo = false;
				accion = taccion.ninguna;
			}
		} else if (GUILayout.Button (new GUIContent ("", "Pulsa para mostrar el menu de acciones"), "BotonOcultarBloqueIzquierdoActivado", GUILayout.Height (cuantoH * 10), GUILayout.Width (cuantoH * 1)))
			mostrarBloqueIzquierdo = true;
		
		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();
	}

	//Dibuja el bloque seleccion de la ventana que contiene los diferentes edificios, animales o vegetales seleccionables según que botón se haya pulsado en el bloque izquierdo
	private void bloqueSeleccion ()
	{
		int posicionBloque = 0;
		int posicionBloqueMejoras = 0;
		switch (aspectRatio) {
		case taspectRatio.aspectRatio16_9:
			posicionBloque = 40;
			posicionBloqueMejoras = 37;
			break;
		case taspectRatio.aspectRatio16_10:
			posicionBloque = 45;
			posicionBloqueMejoras = 42;
			break;
		case taspectRatio.aspectRatio4_3:
			posicionBloque = 55;
			posicionBloqueMejoras = 52;
			break;
		default:
			break;
		}
		
		switch (accion) {
		case taccion.ninguna:
		case taccion.insertar:
		case taccion.mostrarInfoDetallada:
		case taccion.mostrarMenu:
			break;
		case taccion.seleccionarVegetal:
			tipoMenuDerecho = InterfazPrincipal.tMenuDerecho.insercion;
			Rect areaTemp = new Rect (cuantoW * 22, cuantoH * posicionBloque, cuantoW * 36, cuantoH * 4);
			GUILayout.BeginArea (areaTemp, new GUIContent (), "BloqueSeleccionVegetales");
			elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.ninguno;
			GUILayout.BeginVertical ();
			GUILayout.Space (cuantoH);
			GUILayout.BeginHorizontal (GUILayout.Height (cuantoH * 2));
			GUILayout.Space (cuantoW * 2.5f);
			if (GUILayout.Button (new GUIContent ("", "Seta"), "BotonInsertarSeta")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.seta;
				modeloInsercion = principal.vida.especiesVegetales[0].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[0].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.seta;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Flor"), "BotonInsertarFlor")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.flor;
				modeloInsercion = principal.vida.especiesVegetales[1].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[1].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.flor;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Caña"), "BotonInsertarCana")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.cana;
				modeloInsercion = principal.vida.especiesVegetales[2].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[2].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.cana;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Arbusto"), "BotonInsertarArbusto")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.arbusto;
				modeloInsercion = principal.vida.especiesVegetales[3].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[3].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.arbusto;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Estromatolito"), "BotonInsertarEstromatolito")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.estromatolito;
				modeloInsercion = principal.vida.especiesVegetales[4].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[4].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.estromatolito;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Cactus"), "BotonInsertarCactus")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.cactus;
				modeloInsercion = principal.vida.especiesVegetales[5].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[5].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.cactus;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Palmera"), "BotonInsertarPalmera")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.palmera;
				modeloInsercion = principal.vida.especiesVegetales[6].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[6].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.palmera;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Pino"), "BotonInsertarPino")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.pino;
				modeloInsercion = principal.vida.especiesVegetales[7].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[7].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.pino;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Ciprés"), "BotonInsertarCipres")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.cipres;
				modeloInsercion = principal.vida.especiesVegetales[8].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[8].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.cipres;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Pino Alto"), "BotonInsertarPinoAlto")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.pinoAlto;
				modeloInsercion = principal.vida.especiesVegetales[9].modelos[UnityEngine.Random.Range (0, principal.vida.especiesVegetales[9].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.pinoAlto;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW * 2.5f);
			GUILayout.EndHorizontal ();
			GUILayout.Space (cuantoH);
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			break;
		case taccion.seleccionarAnimal:
			GUILayout.BeginArea (new Rect (cuantoW * 22, cuantoH * posicionBloque, cuantoW * 36, cuantoH * 4), new GUIContent (), "BloqueSeleccionAnimales");
			GUILayout.BeginVertical ();
			elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.ninguno;
			GUILayout.Space (cuantoH);
			GUILayout.BeginHorizontal (GUILayout.Height (cuantoH * 2));
			GUILayout.Space (cuantoW * 1.5f);
			if (GUILayout.Button (new GUIContent ("", "Herbivoro1"), "BotonInsertarHerbivoro1")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.herbivoro1;
				modeloInsercion = principal.vida.especiesAnimales[0].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[0].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.herbivoro1;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Herbivoro2"), "BotonInsertarHerbivoro2")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.herbivoro2;
				modeloInsercion = principal.vida.especiesAnimales[1].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[1].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.herbivoro2;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Herbivoro3"), "BotonInsertarHerbivoro3")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.herbivoro3;
				modeloInsercion = principal.vida.especiesAnimales[2].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[2].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.herbivoro3;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Herbivoro4"), "BotonInsertarHerbivoro4")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.herbivoro4;
				modeloInsercion = principal.vida.especiesAnimales[3].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[3].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.herbivoro4;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Herbivoro5"), "BotonInsertarHerbivoro5")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.herbivoro5;
				modeloInsercion = principal.vida.especiesAnimales[4].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[4].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.herbivoro5;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW * 3);
			if (GUILayout.Button (new GUIContent ("", "Carnivoro1"), "BotonInsertarCarnivoro1")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.carnivoro1;
				modeloInsercion = principal.vida.especiesAnimales[5].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[5].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.carnivoro1;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Carnivoro2"), "BotonInsertarCarnivoro2")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.carnivoro2;
				modeloInsercion = principal.vida.especiesAnimales[6].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[6].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.carnivoro2;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Carnivoro3"), "BotonInsertarCarnivoro3")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.carnivoro3;
				modeloInsercion = principal.vida.especiesAnimales[7].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[7].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.carnivoro3;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Carnivoro4"), "BotonInsertarCarnivoro4")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.carnivoro4;
				modeloInsercion = principal.vida.especiesAnimales[8].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[8].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.carnivoro4;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Carnivoro5"), "BotonInsertarCarnivoro5")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.carnivoro5;
				modeloInsercion = principal.vida.especiesAnimales[9].modelos[UnityEngine.Random.Range (0, principal.vida.especiesAnimales[9].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.carnivoro5;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW * 1.5f);
			GUILayout.EndHorizontal ();
			GUILayout.Space (cuantoH);
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			break;
		case taccion.seleccionarEdificio:
			GUILayout.BeginArea (new Rect (cuantoW * 32, cuantoH * posicionBloque, cuantoW * 16, cuantoH * 4), new GUIContent (), "BloqueSeleccionEdificios");
			GUILayout.BeginVertical ();
			elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.ninguno;
			GUILayout.Space (cuantoH);
			GUILayout.BeginHorizontal (GUILayout.Height (cuantoH * 2));
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Fábrica de componentes básicos"), "BotonInsertarFabComBas")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.fabricaCompBas;
				modeloInsercion = principal.vida.tiposEdificios[0].modelos[UnityEngine.Random.Range (0, principal.vida.tiposEdificios[0].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
				//principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);	
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.fabricaCompBas;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Central de energía"), "BotonInsertarCenEn")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.centralEnergia;
				modeloInsercion = principal.vida.tiposEdificios[1].modelos[UnityEngine.Random.Range (0, principal.vida.tiposEdificios[1].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
				//principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);	
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.centralEnergia;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Granja"), "BotonInsertarGranja")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.granja;
				modeloInsercion = principal.vida.tiposEdificios[2].modelos[UnityEngine.Random.Range (0, principal.vida.tiposEdificios[2].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
				//principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.granja;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Fábrica de componentes avanzados"), "BotonInsertarFabComAdv")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.fabricaCompAdv;
				modeloInsercion = principal.vida.tiposEdificios[3].modelos[UnityEngine.Random.Range (0, principal.vida.tiposEdificios[3].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
				//principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);	
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.fabricaCompAdv;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			if (GUILayout.Button (new GUIContent ("", "Central de energía avanzada"), "BotonInsertarCenEnAdv")) {
				accion = taccion.insertar;
				elementoInsercion = telementoInsercion.centralEnergiaAdv;
				modeloInsercion = principal.vida.tiposEdificios[4].modelos[UnityEngine.Random.Range (0, principal.vida.tiposEdificios[4].modelos.Count)];
				modeloInsercion = FuncTablero.creaMesh (new Vector3 (0, 0, 0), modeloInsercion);
				//principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);	
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) {
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.centralEnergiaAdv;
				seleccionarObjetoInsercion();
			}
			GUILayout.Space (cuantoW);
			GUILayout.EndHorizontal ();
			GUILayout.Space (cuantoH);
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			break;
		case taccion.seleccionarMejora:
			GUILayout.BeginArea (new Rect (cuantoW * 22, cuantoH * posicionBloqueMejoras, cuantoW * 36, cuantoH * 7), new GUIContent (), "BloqueSeleccionMejoras");
			GUILayout.BeginVertical ();
			GUILayout.Space (cuantoH);
			GUILayout.BeginHorizontal (GUILayout.Height (cuantoH * 2));
			GUILayout.Space (cuantoW * 6.5f);
			//Sensores -----------------------------------------------------
			if (mejoras.mejorasCompradas[0])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Habilita la información de la barra inferior"), "BotonMejoraInformacion")) {
				mejoras.compraMejora0 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (mejoras.mejorasCompradas[1])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Habilita la detección de hábitats"), "BotonMejoraHabitats")) {
				mejoras.compraMejora1 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (mejoras.mejorasCompradas[2])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Habilita la deteccion de metales raros"), "BotonMejoraMetalesRaros")) {
				mejoras.compraMejora2 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[0] || mejoras.mejorasCompradas[3])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Habilita la deteccion de animales y vegetales"), "BotonMejoraVida")) {
				mejoras.compraMejora3 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW * 6);
			//Motores ------------------------------------------------------
			//Se pueden añadir mas condiciones como el coste
			if (mejoras.mejorasCompradas[4] || mejoras.mejorasCompradas[5])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Mejora de motor nivel 1"), "BotonMejoraMotor1")) {
				mejoras.compraMejora4 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[4] || mejoras.mejorasCompradas[5])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Mejora de motor nivel 2"), "BotonMejoraMotor2")) {
				mejoras.compraMejora5 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (mejoras.mejorasCompradas[6])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Aislamiento magnético (viaje por los polos)"), "BotonMejoraMotor3")) {
				mejoras.compraMejora6 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (mejoras.mejorasCompradas[7])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Subir la nave de órbita"), "BotonMejoraMotor4")) {
				mejoras.compraMejora7 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW * 1.5f);
			GUILayout.EndHorizontal ();
			GUILayout.Space (cuantoH);
			GUILayout.BeginHorizontal (GUILayout.Height (cuantoH * 2));
			GUILayout.Space (cuantoW * 6.5f);
			//Energia ------------------------------------------------------
			if (mejoras.mejorasCompradas[8])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Añade un condensador de energía para ampliar la energía máxima"), "BotonMejoraEnergia1")) {
				mejoras.compraMejora8 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (mejoras.mejorasCompradas[9])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Añade un condensador de energía mejorado"), "BotonMejoraEnergia2")) {
				mejoras.compraMejora9 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[9] || mejoras.mejorasCompradas[10])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Desbloquea el uso de habilidades avanzadas"), "BotonMejoraEnergia3")) {
				mejoras.compraMejora10 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[8] || !mejoras.mejorasCompradas[9] || !mejoras.mejorasCompradas[10] || mejoras.mejorasCompradas[11])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Desbloquea el uso de habilidades cuánticas"), "BotonMejoraEnergia4")) {
				mejoras.compraMejora11 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW * 6);
			//Almacenamiento -----------------------------------------------
			if (mejoras.mejorasCompradas[12] || mejoras.mejorasCompradas[13] || !mejoras.mejorasCompradas[14])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Añade un contenedor para almacenar más recursos"), "BotonMejoraAlmacen1")) {
				mejoras.compraMejora12 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (mejoras.mejorasCompradas[13] || !mejoras.mejorasCompradas[12] || !mejoras.mejorasCompradas[15])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Mejora del contenedor para almacenar todavía más recursos"), "BotonMejoraAlmacen2")) {
				mejoras.compraMejora13 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (mejoras.mejorasCompradas[14])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Permite almacenar componentes avanzados"), "BotonMejoraAlmacen3")) {
				mejoras.compraMejora14 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[14] || !mejoras.mejorasCompradas[12] || mejoras.mejorasCompradas[15])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Permite almacenar material biológico"), "BotonMejoraAlmacen4")) {
				mejoras.compraMejora15 ();
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW * 1.5f);
			GUILayout.EndHorizontal ();
			GUILayout.Space (cuantoH);
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			break;
		case taccion.seleccionarHabilidad:
			GUILayout.BeginArea (new Rect (cuantoW * 22, cuantoH * posicionBloque, cuantoW * 36, cuantoH * 4), new GUIContent (), "BloqueSeleccionHabilidades");
			GUILayout.BeginVertical ();
			GUILayout.Space (cuantoH);
			GUILayout.BeginHorizontal (GUILayout.Height (cuantoH * 2));
			GUILayout.Space (cuantoW * 1.5f);
			if (!mejoras.mejorasCompradas[1] && !mejoras.mejorasCompradas[2] && !mejoras.mejorasCompradas[3])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", "Desactiva todos los filtros"), "BotonHabilidadVisionNormal")) {
				materiales.habitats.SetFloat("_FiltroOn", 0.0f);
				
				materiales.recursos.SetFloat("_ComunesOn",0.0f);
				materiales.recursos.SetFloat("_RarosOn",0.0f);
				materiales.recursos.SetFloat("_EdificiosOn",0.0f);
				
				for(int i = 0; i< materiales.plantas.Count; i++){
					materiales.plantas[i].SetFloat("_FiltroOn", 0.0f);
					materiales.plantas[i].SetColor("_Tinte", Color.white);
				}
				for(int i = 0; i< materiales.herbivoros.Count; i++){
					materiales.herbivoros[i].SetFloat("_FiltroOn", 0.0f);
					materiales.herbivoros[i].SetColor("_Tinte", Color.white);
				}
				
				for(int i = 0; i< materiales.carnivoros.Count; i++){
					materiales.carnivoros[i].SetFloat("_FiltroOn", 0.0f);
					materiales.carnivoros[i].SetColor("_Tinte", Color.white);
				}
				
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[2])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidadFiltroRecursos")) {
				tipoMenuDerecho = InterfazPrincipal.tMenuDerecho.filtroRecursos;
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[1])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidadFiltroHabitats")) {
				if (materiales.habitats.GetFloat("_FiltroOn") == 0.0f)
					materiales.habitats.SetFloat("_FiltroOn", 1.0f);
				else {
					materiales.habitats.SetFloat("_FiltroOn", 0.0f);
				}
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[3])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidadFiltroVegetales")) {
				tipoMenuDerecho = InterfazPrincipal.tMenuDerecho.filtroVegetales;
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[3])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidadFiltroAnimales")) {
				tipoMenuDerecho = InterfazPrincipal.tMenuDerecho.filtroAnimales;
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW * 3);
			if (!mejoras.mejorasCompradas[10])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidad6")) {
				//TODO
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[10])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidad7")) {
				//TODO
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[10])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidad8")) {
				//TODO
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[11])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidad9")) {
				//TODO
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW);
			if (!mejoras.mejorasCompradas[11])
				GUI.enabled = false;
			if (GUILayout.Button (new GUIContent ("", ""), "BotonHabilidad10")) {
				//TODO
			}
			GUI.enabled = true;
			GUILayout.Space (cuantoW * 1.5f);
			GUILayout.EndHorizontal ();
			GUILayout.Space (cuantoH);
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			break;
		default:
			break;
		}
		
	}

	//Dibuja el bloque de información básica de la casilla a la que estamos apuntando
	private void bloqueInformacion ()
	{
		int posicionBloque = 0;
		switch (aspectRatio) {
		case taspectRatio.aspectRatio16_9:
			posicionBloque = 44;
			break;
		case taspectRatio.aspectRatio16_10:
			posicionBloque = 49;
			break;
		case taspectRatio.aspectRatio4_3:
			posicionBloque = 59;
			break;
		default:
			break;
		}
		GUI.Box (new Rect (cuantoW * 0, cuantoH * posicionBloque, cuantoW * 100, cuantoH * 1), infoCasilla, "BloqueInformacion");
	}

	//Dibuja el menu de opciones que contiene Guardar, Opciones de audio, Menu Principal, Salir, Volver
	private void bloqueMenu ()
	{
		float posicionBloque = 0;
		float posicionConfirmar = 0;
		float posicionAudio = 0;
		float posicionGuardar = 0;
		switch (aspectRatio) {
		case taspectRatio.aspectRatio16_9:
			posicionBloque = 13.5f;
			posicionConfirmar = 19.5f;
			posicionAudio = 16.5f;
			posicionGuardar = 14.5f;
			break;
		case taspectRatio.aspectRatio16_10:
			posicionBloque = 16;
			posicionConfirmar = 22;
			posicionAudio = 19;
			posicionGuardar = 17;
			break;
		case taspectRatio.aspectRatio4_3:
			posicionBloque = 21;
			posicionConfirmar = 27;
			posicionAudio = 24;
			posicionGuardar = 22;
			break;
		default:
			break;
		}
		
		switch (accionMenu) {
		case taccionMenu.mostrarMenu:
			GUILayout.BeginArea (new Rect (cuantoW * 32.5f, cuantoH * posicionBloque, cuantoW * 15, cuantoH * 18), new GUIContent ());
			GUILayout.BeginVertical ();
			GUILayout.Box (new GUIContent (), "BloqueMenu", GUILayout.Height (cuantoH * 3), GUILayout.Width (cuantoW * 15));
			if (GUILayout.Button (new GUIContent ("Guardar partida", "Lleva al menu de guardar partida"), "BotonGuardarPartida", GUILayout.Height (cuantoH * 3), GUILayout.Width (cuantoW * 15)))
				accionMenu = InterfazPrincipal.taccionMenu.mostrarGuardar;
			if (GUILayout.Button (new GUIContent ("Opciones de audio", "Lleva al menu de opciones de audio"), "BotonOpcionesAudio", GUILayout.Height (cuantoH * 3), GUILayout.Width (cuantoW * 15)))
				accionMenu = InterfazPrincipal.taccionMenu.mostrarOpcionesAudio;
			if (GUILayout.Button (new GUIContent ("Menu principal", "Lleva al menu principal del juego"), "BotonMenuPrincipal", GUILayout.Height (cuantoH * 3), GUILayout.Width (cuantoW * 15)))
				accionMenu = InterfazPrincipal.taccionMenu.mostrarSalirMenuPrincipal;
			if (GUILayout.Button (new GUIContent ("Salir del juego", "Cierra completamente el juego"), "BotonSalirJuego", GUILayout.Height (cuantoH * 3), GUILayout.Width (cuantoW * 15)))
				accionMenu = InterfazPrincipal.taccionMenu.mostrarSalirJuego;
			if (GUILayout.Button (new GUIContent ("Volver", "Vuelve a la partida"), "BotonVolver", GUILayout.Height (cuantoH * 3), GUILayout.Width (cuantoW * 15))) {
				accion = InterfazPrincipal.taccion.ninguna;
				principal.setEscalaTiempo (escalaTiempoAntesMenu);
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			if (GUI.Button (new Rect (0, 0, cuantoW * 80, cuantoH * 60), new GUIContent (), "")) {
				;
				//CLINK -> Control para que no se pulse fuera del menú (Buena idea!)
			}
			break;
		
		case taccionMenu.mostrarGuardar:
			GUI.Box (new Rect (cuantoW * 31, cuantoH * posicionGuardar, cuantoW * 18, cuantoH * 14), new GUIContent (""), "BoxGuardar");
			posicionScroll = GUI.BeginScrollView (new Rect (cuantoW * 31, cuantoH * posicionGuardar, cuantoW * 18, cuantoH * 12), posicionScroll, new Rect (0, 0, cuantoW * 18, cuantoH * 4 * numSavesExtra));
			if (GUI.Button (new Rect (cuantoW * 4, 0, cuantoW * 10, cuantoH * 3), new GUIContent ("Nueva partida salvada", "Guardar una nueva partida"))) {
				GameObject contenedor = GameObject.FindGameObjectWithTag ("Carga");
				ValoresCarga temp = contenedor.GetComponent<ValoresCarga> ();
				principal.rellenaContenedor (ref temp);
				string fecha = System.DateTime.Now.ToString ().Replace ("\\", "").Replace ("/", "").Replace (" ", "").Replace (":", "");
				SaveLoad.cambiaFileName ("Partida" + fecha + ".hur");
				SaveLoad.Save (temp);
				//Recuperar estado normal
				accion = InterfazPrincipal.taccion.ninguna;
				principal.setEscalaTiempo (escalaTiempoAntesMenu);
			}
			for (int i = 0; i < numSaves; i++) {
				if (GUI.Button (new Rect (cuantoW * 5, (i + 1) * cuantoH * 3, cuantoW * 10, cuantoH * 3), new GUIContent (nombresSaves[i], "Sobreescribir partida num. " + i))) {
					GameObject contenedor = GameObject.FindGameObjectWithTag ("Carga");
					ValoresCarga temp = contenedor.GetComponent<ValoresCarga> ();
					principal.rellenaContenedor (ref temp);
					SaveLoad.cambiaFileName (nombresSaves[i]);
					SaveLoad.Save (temp);
					//Recuperar estado normal
					accion = InterfazPrincipal.taccion.ninguna;
					principal.setEscalaTiempo (escalaTiempoAntesMenu);
				}
			}

			GUI.EndScrollView ();
			if (GUI.Button (new Rect (cuantoW * 44, cuantoH * (posicionGuardar + 13), cuantoW * 5, cuantoH * 2), new GUIContent ("Volver", "Pulsa aquí para volver al menu de opciones"))) {
				accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
			}
			break;
		case taccionMenu.mostrarOpcionesAudio:
			GUI.Box (new Rect (cuantoW * 32.5f, cuantoH * posicionAudio, cuantoW * 15, cuantoH * 12), new GUIContent (), "BoxOpcionesAudio");
			/*GUI.Label(new Rect(cuantoW*34f,cuantoH*(posicionAudio),cuantoW*15.5f,cuantoH*2),new GUIContent("Sonido"));*/			
			GUI.Label (new Rect (cuantoW * 34f, cuantoH * (posicionAudio + 2f), cuantoW * 9, cuantoH * 2), new GUIContent ("Volumen"));
			GUI.Label (new Rect (cuantoW * 34f, cuantoH * (posicionAudio + 4f), cuantoW * 6.5f, cuantoH * 2), new GUIContent ("Música"));
			musicaVol = GUI.HorizontalSlider (new Rect (cuantoW * 39f, cuantoH * (posicionAudio + 5f), cuantoW * 7, cuantoH * 2), musicaVol, 0, 1.0f);
			GUI.Label (new Rect (cuantoW * 34f, cuantoH * (posicionAudio + 6f), cuantoW * 5.5f, cuantoH * 2), new GUIContent ("Efectos"));
			sfxVol = GUI.HorizontalSlider (new Rect (cuantoW * 39f, cuantoH * (posicionAudio + 7f), cuantoW * 7, cuantoH * 2), sfxVol, 0, 1.0f);
			if (GUI.Button (new Rect (cuantoW * 41f, cuantoH * (posicionAudio + 9.5f), cuantoW * 5f, cuantoH * 2f), new GUIContent ("Volver", "Pulsa aquí para volver al menu de opciones")))
				accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
			
			if (GUI.changed) {
				//Volumen del audio ambiente
				Audio_Ambiente ambiente = sonidoAmbiente.GetComponent<Audio_Ambiente> ();
				if (musicaVol == 0)
					ambiente.activado = false;
				else
					ambiente.activado = true;
				ambiente.volumen = musicaVol;
				//Volumen del audio de efectos
				Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX> ();
				if (sfxVol == 0)
					efectos.activado = false;
				else
					efectos.activado = true;
				efectos.volumen = sfxVol;
			}
			break;
		case taccionMenu.mostrarSalirMenuPrincipal:
			GUI.Box (new Rect (cuantoW * 36, cuantoH * posicionConfirmar, cuantoW * 8, cuantoH * 4), new GUIContent (), "BoxConfirmacion");
			GUI.Label (new Rect (cuantoW * 37.5f, cuantoH * (posicionConfirmar), cuantoW * 6, cuantoH * 2), new GUIContent ("¿Está seguro?"));
			if (GUI.Button (new Rect (cuantoW * 37, cuantoH * (posicionConfirmar + 2), cuantoW * 2.5f, cuantoH * 1.5f), new GUIContent ("Si", "Pulsa aquí para salir al menu principal"))) {
				accion = InterfazPrincipal.taccion.ninguna;
				principal.setEscalaTiempo (1.0f);
				FuncTablero.quitaPerlin ();
				Application.LoadLevel ("Escena_Inicial");
			}
			if (GUI.Button (new Rect (cuantoW * 40.5f, cuantoH * (posicionConfirmar + 2), cuantoW * 2.5f, cuantoH * 1.5f), new GUIContent ("No", "Pulsa aquí para volver al menu de opciones")))
				accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
			break;
		case taccionMenu.mostrarSalirJuego:
			GUI.Box (new Rect (cuantoW * 36, cuantoH * posicionConfirmar, cuantoW * 8, cuantoH * 4), new GUIContent (), "BoxConfirmacion");
			GUI.Label (new Rect (cuantoW * 37.5f, cuantoH * (posicionConfirmar), cuantoW * 6, cuantoH * 2), new GUIContent ("¿Está seguro?"));
			if (GUI.Button (new Rect (cuantoW * 37, cuantoH * (posicionConfirmar + 2), cuantoW * 2.5f, cuantoH * 1.5f), new GUIContent ("Si", "Pulsa aquí para salir del juego")))
				Application.Quit ();
			if (GUI.Button (new Rect (cuantoW * 40.5f, cuantoH * (posicionConfirmar + 2), cuantoW * 2.5f, cuantoH * 1.5f), new GUIContent ("No", "Pulsa aquí para volver al menu de opciones")))
				accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
			break;
		default:
			break;
		}
		
	}

	private void insertarElemento ()
	{
		if (accion == taccion.insertar) {
			if (Time.realtimeSinceStartup > tiempoUltimoModeloInsercion + tiempoModeloInsercion) {
				tiempoUltimoModeloInsercion = Time.realtimeSinceStartup;
				int posX = 0;
				int posY = 0;
				RaycastHit hit;
				int tipo = (int)elementoInsercion - 1;
				if (principal.raycastRoca (Input.mousePosition, ref posX, ref posY,out hit)) {
					/*
					float x = (principal.vida.tablero[posY, posX].coordsVert.x + principal.vida.tablero[posY - 1, posX].coordsVert.x) / 2;
					float y = (principal.vida.tablero[posY, posX].coordsVert.y + principal.vida.tablero[posY - 1, posX].coordsVert.y) / 2;
					float z = (principal.vida.tablero[posY, posX].coordsVert.z + principal.vida.tablero[posY - 1, posX].coordsVert.z) / 2;
					Vector3 coordsVert = new Vector3 (x, y, z);
					*/
					Vector3 coordsVert = principal.vida.tablero[posY, posX].coordsVert;
					//Vector3 coordsVert = new Vector3(principal.vida.tablero[posY, posX].coordsVert.x,principal.vida.tablero[posY, posX].coordsVert.y,hit.normal.z);
					//Mover la malla
					modeloInsercion.transform.position = coordsVert;
					//modeloInsercion.transform.position = hit.point;
					Vector3 normal = modeloInsercion.transform.position - modeloInsercion.transform.parent.position;
					modeloInsercion.transform.position = principal.vida.objetoRoca.transform.TransformPoint (modeloInsercion.transform.position);
					modeloInsercion.transform.rotation = Quaternion.LookRotation (normal);
					modeloInsercion.renderer.material.SetFloat ("_FiltroOn", 1.0f);
					modeloInsercion.renderer.material.SetColor ("_Tinte", Color.red);
					forzarTooltip = false;
					//Edificio
					if (tipo >= 0 && tipo < 5) {
						TipoEdificio tedif = principal.vida.tiposEdificios[tipo];
						if (principal.recursosSuficientes (tedif.energiaConsumidaAlCrear, tedif.compBasConsumidosAlCrear, tedif.compAvzConsumidosAlCrear, tedif.matBioConsumidoAlCrear)) {
							if (principal.vida.compruebaAnadeEdificio (tedif, posY, posX))
								modeloInsercion.renderer.material.SetColor ("_Tinte", Color.green);
							else {
								forzarTooltip = true;
								mensajeForzarTooltip = "Habitat incompatible o ya ocupado";
							}
						} else {
							forzarTooltip = true;
							mensajeForzarTooltip = "No hay recursos suficientes";
						}
					//Vegetal
					} else if (tipo >= 5 && tipo < 15) {
						tipo -= 5;
						EspecieVegetal especie = (EspecieVegetal)principal.vida.especies[tipo];
						if (principal.vida.compruebaAnadeVegetal (especie, especie.habitabilidadInicial, 0.0f, posY, posX))
							modeloInsercion.renderer.material.SetColor ("_Tinte", Color.green);
						else {
							forzarTooltip = true;
							mensajeForzarTooltip = "Habitat incompatible o ya ocupado";
						}
					//Animal (herbivoro o carnivoro)
					} else if (tipo >= 15 && tipo < 25) {
						tipo -= 5;
						EspecieAnimal especie = (EspecieAnimal)principal.vida.especies[tipo];
						if (principal.vida.compruebaAnadeAnimal (especie, posY, posX))
							modeloInsercion.renderer.material.SetColor ("_Tinte", Color.green);
						else {
							forzarTooltip = true;
							mensajeForzarTooltip = "Habitat incompatible o ya ocupado";
						}
					}
					//Probamos inserción
					if (Input.GetMouseButton (0)) {
						tipo = (int)elementoInsercion - 1;
						//Edificio
						if (tipo >= 0 && tipo < 5) {
							TipoEdificio tedif = principal.vida.tiposEdificios[tipo];
							if (principal.recursosSuficientes (tedif.energiaConsumidaAlCrear, tedif.compBasConsumidosAlCrear, tedif.compAvzConsumidosAlCrear, tedif.matBioConsumidoAlCrear)) 
							{
								if (principal.vida.anadeEdificio (tedif, posY, posX, 0, 0, 0, 0, 10, 10, 10, 10)) 
								{
									principal.consumeRecursos (tedif.energiaConsumidaAlCrear, tedif.compBasConsumidosAlCrear, tedif.compAvzConsumidosAlCrear, tedif.matBioConsumidoAlCrear);
//									principal.modificaRecursosPorTurno(10,10,10,10);
									//[Aris] Los recursos generados se tienen que ajustar o cambiar, pero mejor ir probando
									//con cosas creibles desde ya. De todas formas esto se tiene que cambiar para que sean
									//atributos de cada edificio para poder "apagarlos" y por la situacion de cuando se agote la
									//energia y el dif sea negativo...
									switch (tipo) {
									case 0:
										//Fabrica basica
										principal.modificaRecursosPorTurno (-1, 5, 0, 0);
										break;
									case 1:
										//Central energia 1
										principal.modificaRecursosPorTurno (5, 0, 0, 0);
										break;
									case 2:
										//Granja
										principal.modificaRecursosPorTurno (-5, 0, 0, 1);
										break;
									case 3:
										//Fabrica avanzada
										principal.modificaRecursosPorTurno (-5, 5, 5, 0);
										break;
									case 4:
										//Central energia 2
										principal.modificaRecursosPorTurno (25, 0, 0, 0);
										break;
									}
								} else {
									Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX> ();
									efectos.playNumber (Random.Range (1, 3));
									//Sonidos de error son el 1 y 2
								}
							} else {
								Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX> ();
								efectos.playNumber (Random.Range (1, 3));
								//Sonidos de error son el 1 y 2
							}
							elementoInsercion = telementoInsercion.ninguno;
							accion = taccion.ninguna;
							//principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 0);	
						//Vegetal
						} else if (tipo >= 5 && tipo < 15) {
							tipo -= 5;
							EspecieVegetal especie = (EspecieVegetal)principal.vida.especies[tipo];
							principal.vida.anadeVegetal (especie, especie.habitabilidadInicial, 0.0f, posY, posX);
							elementoInsercion = telementoInsercion.ninguno;
							accion = taccion.ninguna;
						//Animal (herbivoro o carnivoro)
						} else if (tipo >= 15 && tipo < 25) {
							tipo -= 5;
							EspecieAnimal especie = (EspecieAnimal)principal.vida.especies[tipo];
							principal.vida.anadeAnimal (especie, posY, posX);
							elementoInsercion = telementoInsercion.ninguno;
							accion = taccion.ninguna;
						}
						Destroy (modeloInsercion);
					//Desactivamos inserción
					} else if (Input.GetMouseButton (1)) {
						elementoInsercion = telementoInsercion.ninguno;
						accion = taccion.ninguna;
						Destroy (modeloInsercion);
					}
				}
			}
		}
		
		
	}
		/*if(accion == taccion.insertar)
		{	
			//pintar modelo en tiempo real y area de efecto si es necesario
			if(Input.GetMouseButtonDown(0))
			{
				int x = 0;
				int y = 0;
				if(principal.raycastRoca(Input.mousePosition,ref x,ref y))
				{
					int tipo = (int)elementoInsercion - 1;
					if(tipo >= 0 && tipo < 5)				//Edificio
					{
						TipoEdificio tedif = principal.vida.tiposEdificios[tipo];
						if(principal.recursosSuficientes(tedif.energiaConsumidaAlCrear,tedif.compBasConsumidosAlCrear,tedif.compAvzConsumidosAlCrear,tedif.matBioConsumidoAlCrear)) {							
							if(principal.vida.anadeEdificio(tedif,y,x,0,0,0,0,10,10,10,10)) {
								principal.consumeRecursos(tedif.energiaConsumidaAlCrear,tedif.compBasConsumidosAlCrear,tedif.compAvzConsumidosAlCrear,tedif.matBioConsumidoAlCrear);
								switch (tipo) {
								case 0:		//Fabrica basica
									principal.modificaRecursosPorTurno(-1,5,0,0);
									break;
								case 1:		//Central energia 1
									principal.modificaRecursosPorTurno(5,0,0,0);
									break;
								case 2:		//Granja
									principal.modificaRecursosPorTurno(-5,0,0,1);
									break;
								case 3:		//Fabrica avanzada
									principal.modificaRecursosPorTurno(-5,5,5,0);
									break;
								case 4: 	//Central energia 2
									principal.modificaRecursosPorTurno(25,0,0,0);
									break;
								}								
							}
							else {							
								infoCasilla += "Habitat inadecuado o ya ocupado!";//Mostrar por pantalla que no se ha podido insertar por que el habitat no es el adecuado o xq ya existe un edificio ahi
							}
						}
						else {
							infoCasilla += "No hay recursos suficientes!";//Mostrar por pantalla que no se ha podido insertar por falta de recursos
						}						
						elementoInsercion = telementoInsercion.ninguno;
						accion = taccion.ninguna;	
						principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 0);	
					}					
					else if(tipo >= 5 && tipo < 15)			//Vegetal
					{
						tipo -= 5;
						EspecieVegetal especie = (EspecieVegetal)principal.vida.especies[tipo];
						principal.vida.anadeVegetal(especie,y,x);
						elementoInsercion = telementoInsercion.ninguno;
						accion = taccion.ninguna;
					}
					else if(tipo >= 15 && tipo < 25)		//Animal (herbivoro o carnivoro)
					{
						tipo -= 5;	
						EspecieAnimal especie = (EspecieAnimal)principal.vida.especies[tipo];
						principal.vida.anadeAnimal(especie,y,x);
						elementoInsercion = telementoInsercion.ninguno;
						accion = taccion.ninguna;
					}
					else
						return;
				}			
			}			
		}*/		
	//Obtiene la información básica de la casilla a mostrar en la barra de información inferior
	private void calculaInfoCasilla ()
	{
		if (mostrarInfoCasilla && mejoraInfoCasilla) {
			if (Input.mousePosition != posicionMouseInfoCasilla && Time.realtimeSinceStartup > tiempoUltimaInfoCasilla + tiempoInfoCasilla) {
				posicionMouseInfoCasilla = Input.mousePosition;
				tiempoUltimaInfoCasilla = Time.realtimeSinceStartup;
				int x = 0;
				int y = 0;
				RaycastHit hit;
				if (principal.raycastRoca (Input.mousePosition, ref x, ref y,out hit)) {
					T_habitats habitat = principal.vida.tablero[y, x].habitat;
					T_elementos elem = principal.vida.tablero[y, x].elementos;
					Edificio edificio = principal.vida.tablero[y, x].edificio;
					Vegetal vegetal = principal.vida.tablero[y, x].vegetal;
					Animal animal = principal.vida.tablero[y, x].animal;
					infoCasilla = "";
					//[Aris] Desbloqueando mejoras de deteccion
					if (mostrarInfoHabitat) {
						if (habitat == T_habitats.montana)
							infoCasilla = "Hábitat: montaña" + "\t\t";
						else
							infoCasilla = "Hábitat: " + habitat.ToString () + "\t\t";
					}
					
					if (elem == T_elementos.comunes)
						infoCasilla += "Elementos: metales comunes" + "\t\t"; 
					else if (elem == T_elementos.raros && mostrarInfoMetalesRaros)
						//Desbloqueando mejoras
						infoCasilla += "Elementos: metales raros" + "\t\t";
					
					if (edificio != null)
						infoCasilla += "Edificio: " + edificio.tipo.nombre + "\t\t";
					
					if (mostrarInfoSeres) {
						if (vegetal != null)
							infoCasilla += "Vegetal: " + vegetal.especie.nombre + "\t\t";
						if (animal != null)
							infoCasilla += "Animal: " + animal.especie.nombre + "\t\t";
					}
					infoCasilla += "\t\tAlto: "+y+"\t\tAncho :"+x;
				}
			}
		} else
			infoCasilla = "";
	}

	//Controla si se tiene que mostrar el tooltip o no
	private void controlTooltip ()
	{
		if (Input.mousePosition != posicionMouseTooltip) {
			posicionMouseTooltip = Input.mousePosition;
			activarTooltip = false;
			ultimoMov = Time.realtimeSinceStartup;
		} else if (Time.realtimeSinceStartup >= ultimoMov + tiempoTooltip)
			activarTooltip = true;
		
	}

	//Muestra el tooltip si ha sido activado
	private void mostrarToolTip ()
	{
		float longitud = GUI.tooltip.Length;
		if (longitud == 0.0f)
			return;
		else {
			if (longitud < 8)
				longitud *= 10.0f; else if (longitud < 15)
				longitud *= 9.0f;
			else
				longitud *= 8.5f;
		}
		
		float posx = Input.mousePosition.x;
		float posy = Input.mousePosition.y;
		if (posx > (Screen.width / 2))
			posx -= (longitud + 20);
		else
			posx += 15;
		if (posy > (Screen.height / 2))
			posy -= 10;
		else
			posy += 5;
		Rect pos = new Rect (posx, Screen.height - posy, longitud, 25);
		GUI.Box (pos, "");
		GUI.Label (pos, GUI.tooltip);
	}

	//Devuelve true si el raton está fuera de la interfaz y por tanto es válido y false si cae dentro de la interfaz dibujada en ese momento
	public bool posicionFueraDeInterfaz (Vector3 posicionRaton)
	{
		if (accion == taccion.mostrarMenu)
			return false;
		float xini, xfin, yini, yfin;
		yfin = Screen.height - cuantoH * 4;
		//Posición donde termina el bloque superior
		if (mostrarBloqueIzquierdo)
			xini = cuantoW * 3;
		else
			//Posición donde termina el bloque izquierdo
			xini = 0;
		//Tamaño mínimo de la ventana
		//if(mostrarBloqueDerecho)
		//xfin = ?									//Posición donde empieza el bloque derecho
		//else
		xfin = cuantoW * 80;
		//Tamaño máximo de la ventana
		if (accion == taccion.seleccionarVegetal || accion == taccion.seleccionarAnimal || accion == taccion.seleccionarEdificio || accion == taccion.seleccionarMejora || accion == taccion.seleccionarHabilidad) {
			int posicionBloqueSeleccion = 0;
			switch (aspectRatio) {
			case taspectRatio.aspectRatio16_9:
				posicionBloqueSeleccion = 40;
				break;
			case taspectRatio.aspectRatio16_10:
				posicionBloqueSeleccion = 45;
				break;
			case taspectRatio.aspectRatio4_3:
				posicionBloqueSeleccion = 55;
				break;
			default:
				break;
			}
			yini = Screen.height - cuantoH * posicionBloqueSeleccion;
			//Posición donde empieza el bloque de seleccion	
		} else {
			int posicionBloqueInformacion = 0;
			switch (aspectRatio) {
			case taspectRatio.aspectRatio16_9:
				posicionBloqueInformacion = 44;
				break;
			case taspectRatio.aspectRatio16_10:
				posicionBloqueInformacion = 49;
				break;
			case taspectRatio.aspectRatio4_3:
				posicionBloqueInformacion = 59;
				break;
			default:
				break;
			}
			yini = Screen.height - cuantoH * posicionBloqueInformacion;
			//Posición donde empieza el bloque de informacion	
		}
		if (posicionRaton.x > xini && posicionRaton.x < xfin && posicionRaton.y > yini && posicionRaton.y < yfin)
			return true;
		else
			return false;
	}

	public void actualizarEstilosBotones ()
	{
		GUIStyle style;
		switch (accion) {
		case taccion.seleccionarVegetal:
			style = (GUIStyle)estilo.customStyles.GetValue (2);
			style.normal = style.onActive;
			break;
		case taccion.seleccionarAnimal:
			style = (GUIStyle)estilo.customStyles.GetValue (3);
			style.normal = style.onActive;
			break;
		case taccion.seleccionarEdificio:
			style = (GUIStyle)estilo.customStyles.GetValue (4);
			style.normal = style.onActive;
			break;
		case taccion.seleccionarMejora:
			style = (GUIStyle)estilo.customStyles.GetValue (5);
			style.normal = style.onActive;
			break;
		case taccion.seleccionarHabilidad:
			style = (GUIStyle)estilo.customStyles.GetValue (6);
			style.normal = style.onActive;
			break;
		default:
			break;
		}
		switch (accionAnterior) {
		case taccion.seleccionarVegetal:
			style = (GUIStyle)estilo.customStyles.GetValue (2);
			style.normal = style.onNormal;
			break;
		case taccion.seleccionarAnimal:
			style = (GUIStyle)estilo.customStyles.GetValue (3);
			style.normal = style.onNormal;
			break;
		case taccion.seleccionarEdificio:
			style = (GUIStyle)estilo.customStyles.GetValue (4);
			style.normal = style.onNormal;
			break;
		case taccion.seleccionarMejora:
			style = (GUIStyle)estilo.customStyles.GetValue (5);
			style.normal = style.onNormal;
			break;
		case taccion.seleccionarHabilidad:
			style = (GUIStyle)estilo.customStyles.GetValue (6);
			style.normal = style.onNormal;
			break;
		default:
			break;
		}
		
	}

	public void mejoraBarraInferior ()
	{
		mejoraInfoCasilla = true;
	}

	public void mejoraMostrarHabitats ()
	{
		mostrarInfoHabitat = true;
	}

	public void mejoraMostrarMetales ()
	{
		mostrarInfoMetalesRaros = true;
	}

	public void mejoraMostrarSeres ()
	{
		mostrarInfoSeres = true;
	}

	private void bloqueDerecho ()
	{
		float posicionBloqueH = 0;
		switch (tipoMenuDerecho) {
		case tMenuDerecho.ninguno:
			break;
		case tMenuDerecho.filtroAnimales:
			switch (aspectRatio) {
			case taspectRatio.aspectRatio16_9:
				posicionBloqueH = 8.5f;
				//sobre 45
				break;
			case taspectRatio.aspectRatio16_10:
				posicionBloqueH = 11;
				//sobre 50
				break;
			case taspectRatio.aspectRatio4_3:
				posicionBloqueH = 16;
				//sobre 60
				break;
			default:
				break;
			}
			GUI.BeginGroup (new Rect (69 * cuantoW, posicionBloqueH * cuantoH, 11 * cuantoW, 28 * cuantoH));
			GUI.Box (new Rect (0, 0, 11 * cuantoW, 28 * cuantoH), "", "BloqueDerechoFiltroAnimales");
			togglesFiltros[14] = GUI.Toggle (new Rect (cuantoW * 2, cuantoH * 4, cuantoW * 2, cuantoH * 2), togglesFiltros[14], new GUIContent ("", "Filtrar la especie Carnivoro1"), "BotonInsertarCarnivoro1");
			togglesFiltros[15] = GUI.Toggle (new Rect (cuantoW * 2, cuantoH * 7, cuantoW * 2, cuantoH * 2), togglesFiltros[15], new GUIContent ("", "Filtrar la especie Carnivoro2"), "BotonInsertarCarnivoro2");
			togglesFiltros[16] = GUI.Toggle (new Rect (cuantoW * 2, cuantoH * 10, cuantoW * 2, cuantoH * 2), togglesFiltros[16], new GUIContent ("", "Filtrar la especie Carnivoro3"), "BotonInsertarCarnivoro3");
			togglesFiltros[17] = GUI.Toggle (new Rect (cuantoW * 2, cuantoH * 13, cuantoW * 2, cuantoH * 2), togglesFiltros[17], new GUIContent ("", "Filtrar la especie Carnivoro4"), "BotonInsertarCarnivoro4");
			togglesFiltros[18] = GUI.Toggle (new Rect (cuantoW * 2, cuantoH * 16, cuantoW * 2, cuantoH * 2), togglesFiltros[18], new GUIContent ("", "Filtrar la especie Carnivoro5"), "BotonInsertarCarnivoro5");
			togglesFiltros[19] = GUI.Toggle (new Rect (cuantoW * 7, cuantoH * 4, cuantoW * 2, cuantoH * 2), togglesFiltros[19], new GUIContent ("", "Filtrar la especie Herbivoro1"), "BotonInsertarHerbivoro1");
			togglesFiltros[20] = GUI.Toggle (new Rect (cuantoW * 7, cuantoH * 7, cuantoW * 2, cuantoH * 2), togglesFiltros[20], new GUIContent ("", "Filtrar la especie Herbivoro2"), "BotonInsertarHerbivoro2");
			togglesFiltros[21] = GUI.Toggle (new Rect (cuantoW * 7, cuantoH * 10, cuantoW * 2, cuantoH * 2), togglesFiltros[21], new GUIContent ("", "Filtrar la especie Herbivoro3"), "BotonInsertarHerbivoro3");
			togglesFiltros[22] = GUI.Toggle (new Rect (cuantoW * 7, cuantoH * 13, cuantoW * 2, cuantoH * 2), togglesFiltros[22], new GUIContent ("", "Filtrar la especie Herbivoro4"), "BotonInsertarHerbivoro4");
			togglesFiltros[23] = GUI.Toggle (new Rect (cuantoW * 7, cuantoH * 16, cuantoW * 2, cuantoH * 2), togglesFiltros[23], new GUIContent ("", "Filtrar la especie Herbivoro5"), "BotonInsertarHerbivoro5");
			if (GUI.Button (new Rect (cuantoW * 2, cuantoH * 23, cuantoW * 2, cuantoH * 2), new GUIContent ("", "Desactivar filtros de Carnivoros"), "BotonFiltroAnimalesOffCarnivoros")) {
				
			}
			if (GUI.Button (new Rect (cuantoW * 7, cuantoH * 23, cuantoW * 2, cuantoH * 2), new GUIContent ("", "Desactivar filtros de Herbivoros"), "BotonFiltroAnimalesOffHerbivoros")) {
				
			}
			GUI.EndGroup ();
			//TODO Botones de filtros de animales
			if (GUI.Button (new Rect (79 * cuantoW, posicionBloqueH * cuantoH, cuantoW, cuantoH), "", "BotonCerrar")) {
				tipoMenuDerecho = tMenuDerecho.ninguno;
			}
			break;
		case tMenuDerecho.filtroRecursos:
			switch (aspectRatio) {
			case taspectRatio.aspectRatio16_9:
				posicionBloqueH = 16;
				//sobre 45
				break;
			case taspectRatio.aspectRatio16_10:
				posicionBloqueH = 18.5f;
				//sobre 50
				break;
			case taspectRatio.aspectRatio4_3:
				posicionBloqueH = 23.5f;
				//sobre 60
				break;
			default:
				break;
			}

			GUI.BeginGroup (new Rect (69 * cuantoW, posicionBloqueH * cuantoH, 11 * cuantoW, 13 * cuantoH));
			GUI.Box (new Rect (0, 0, 11 * cuantoW, 13 * cuantoH), "", "BloqueDerechoFiltroRecursos");
			togglesFiltros[0] = GUI.Toggle (new Rect (cuantoW * 2, cuantoH * 6, cuantoW * 2, cuantoH * 2), togglesFiltros[0], new GUIContent ("", "Muestra los minerales comunes"), "BotonFiltroRecursosMineralComun");
			togglesFiltros[1] = GUI.Toggle (new Rect (cuantoW * 2, cuantoH * 10, cuantoW * 2, cuantoH * 2), togglesFiltros[1],new GUIContent ("", "Influencia Energetica"), "BotonFiltroRecursosRadioEnergia");
			togglesFiltros[2] = GUI.Toggle (new Rect (cuantoW * 7, cuantoH * 6, cuantoW * 2, cuantoH * 2), togglesFiltros[2],new GUIContent ("", "Muestra los minerales raros"), "BotonFiltroRecursosMineralRaro");
			togglesFiltros[3] = GUI.Toggle (new Rect (cuantoW * 7, cuantoH * 10, cuantoW * 2, cuantoH * 2), togglesFiltros[3],new GUIContent ("", "Terreno de las Granjas"), "BotonFiltroRecursosRadioGranja");
			GUI.EndGroup ();
			if (GUI.Button (new Rect (79 * cuantoW, posicionBloqueH * cuantoH, cuantoW, cuantoH), "", "BotonCerrar")) {
				tipoMenuDerecho = tMenuDerecho.ninguno;
			}
			break;
		case tMenuDerecho.filtroVegetales:
			switch (aspectRatio) {
			case taspectRatio.aspectRatio16_9:
				posicionBloqueH = 12.5f;
				//sobre 45
				break;
			case taspectRatio.aspectRatio16_10:
				posicionBloqueH = 15;
				//sobre 50
				break;
			case taspectRatio.aspectRatio4_3:
				posicionBloqueH = 20;
				//sobre 60
				break;
			default:
				break;
			}

			GUI.BeginGroup (new Rect (69 * cuantoW, posicionBloqueH * cuantoH, 11 * cuantoW, 20 * cuantoH));
			GUI.Box (new Rect (0, 0, 11 * cuantoW, 20 * cuantoH), "", "BloqueDerechoFiltroVegetales");
			togglesFiltros[4] = GUI.Toggle (new Rect (cuantoW * 3, cuantoH * 4, cuantoW * 2, cuantoH * 2), togglesFiltros[4], new GUIContent ("", "Resaltar las Setas"), "BotonInsertarSeta");
			togglesFiltros[5] = GUI.Toggle (new Rect (cuantoW * 3, cuantoH * 7, cuantoW * 2, cuantoH * 2), togglesFiltros[5], new GUIContent ("", "Resaltar las Ca\u00f1as"), "BotonInsertarCana");
			togglesFiltros[6] = GUI.Toggle (new Rect (cuantoW * 3, cuantoH * 10, cuantoW * 2, cuantoH * 2), togglesFiltros[6], new GUIContent ("", "Resaltar los Estromatolitos"), "BotonInsertarEstromatolito");
			togglesFiltros[7] = GUI.Toggle (new Rect (cuantoW * 3, cuantoH * 13, cuantoW * 2, cuantoH * 2), togglesFiltros[7], new GUIContent ("", "Resaltar las Palmeras"), "BotonInsertarPalmera");
			togglesFiltros[8] = GUI.Toggle (new Rect (cuantoW * 3, cuantoH * 16, cuantoW * 2, cuantoH * 2), togglesFiltros[8], new GUIContent ("", "Resaltar los Cipreses"), "BotonInsertarCipres");
			togglesFiltros[9] = GUI.Toggle (new Rect (cuantoW * 6, cuantoH * 4, cuantoW * 2, cuantoH * 2), togglesFiltros[9], new GUIContent ("", "Resaltar las Flores"), "BotonInsertarFlor");
			togglesFiltros[10] = GUI.Toggle (new Rect (cuantoW * 6, cuantoH * 7, cuantoW * 2, cuantoH * 2), togglesFiltros[10], new GUIContent ("", "Resaltar los Arbustos"), "BotonInsertarArbusto");
			togglesFiltros[11] = GUI.Toggle (new Rect (cuantoW * 6, cuantoH * 10, cuantoW * 2, cuantoH * 2), togglesFiltros[11], new GUIContent ("", "Resaltar los Cactus"), "BotonInsertarCactus");
			togglesFiltros[12] = GUI.Toggle (new Rect (cuantoW * 6, cuantoH * 13, cuantoW * 2, cuantoH * 2), togglesFiltros[12], new GUIContent ("", "Resaltar los Pinos"), "BotonInsertarPino");
			togglesFiltros[13] = GUI.Toggle (new Rect (cuantoW * 6, cuantoH * 16, cuantoW * 2, cuantoH * 2), togglesFiltros[13], new GUIContent ("", "Resaltar los Pinos altos"), "BotonInsertarPinoAlto");
			GUI.EndGroup ();
			//TODO Botones del filtro de vegetales
			if (GUI.Button (new Rect (79 * cuantoW, posicionBloqueH * cuantoH, cuantoW, cuantoH), "", "BotonCerrar")) {
				tipoMenuDerecho = tMenuDerecho.ninguno;
			}
			break;
		case tMenuDerecho.insercion:
			if (elementoInsercionDerecho == InterfazPrincipal.telementoInsercion.ninguno) {
				break;
			}
			switch (aspectRatio) {
			case taspectRatio.aspectRatio16_9:
				posicionBloqueH = 8.5f;
				//sobre 45
				break;
			case taspectRatio.aspectRatio16_10:
				posicionBloqueH = 11;
				//sobre 50
				break;
			case taspectRatio.aspectRatio4_3:
				posicionBloqueH = 16;
				//sobre 60
				break;
			default:
				break;
			}
			GUI.BeginGroup (new Rect (69 * cuantoW, posicionBloqueH * cuantoH, 11 * cuantoW, 28 * cuantoH));
			GUI.Box (new Rect (0, 0, 11 * cuantoW, 28 * cuantoH), "", "BloqueDerechoInsercion");
			GUI.Label(new Rect (cuantoW, cuantoH, 9 * cuantoW, cuantoH), infoSeleccion[0], "LabelReducido");	//Este texto es el nombre
			GUI.Box(new Rect(cuantoW, 3 * cuantoH, 9 * cuantoW, 4 * cuantoH), "", "");							//Esta es la imagen
			//Habitabilidad --
			GUI.Label(new Rect(cuantoW * 0.9f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[6].ToString("N1"), "Costa"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 2.3f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[1].ToString("N1"), "LLanura"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 3.6f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[2].ToString("N1"), "Colina"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 4.9f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[0].ToString("N1"), "Monta\u00f1a"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 6.2f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[4].ToString("N1"), "Volcanico"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 7.5f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[7].ToString("N1"), "Tundra"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 8.9f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[3].ToString("N1"), "Desierto"), "LabelHabitabilidad");
			//Habitabilidad --
			GUI.Label(new Rect (cuantoW * 1, cuantoH * 13, 9 * cuantoW, 1 * cuantoH), "DESCRIPCION:", "LabelDescripcionTitulo");				//Titulo de la descripcion
			GUI.Label(new Rect (cuantoW * 1, cuantoH * 14, 9 * cuantoW, 4 * cuantoH), infoSeleccion[1], "LabelDescripcionContenido");			//Este texto es la descripcion
			switch (tipoSeleccion) {
			case 0:		//Planta
				break;
			case 1: 	//Herbivoro
				break;
			case 2: 	//Carnivoro
				break;
			case 3: 	//Edificio
				break;
			}
			GUI.EndGroup ();
			//TODO Botones del filtro de vegetales
			if (GUI.Button (new Rect (79 * cuantoW, posicionBloqueH * cuantoH, cuantoW, cuantoH), "", "BotonCerrar")) {
				tipoMenuDerecho = tMenuDerecho.ninguno;
				elementoInsercionDerecho = InterfazPrincipal.telementoInsercion.ninguno;
			}
			break;
			break;
		case tMenuDerecho.seleccion:
			switch (aspectRatio) {
			case taspectRatio.aspectRatio16_9:
				posicionBloqueH = 8.5f;
				//sobre 45
				break;
			case taspectRatio.aspectRatio16_10:
				posicionBloqueH = 11;
				//sobre 50
				break;
			case taspectRatio.aspectRatio4_3:
				posicionBloqueH = 16;
				//sobre 60
				break;
			default:
				break;
			}
			if (infoSeleccion.Count == 0) {
				tipoMenuDerecho = InterfazPrincipal.tMenuDerecho.ninguno;
				break;
			}
			GUI.BeginGroup (new Rect (69 * cuantoW, posicionBloqueH * cuantoH, 11 * cuantoW, 28 * cuantoH));
			GUI.Box (new Rect (0, 0, 11 * cuantoW, 28 * cuantoH), "", "BloqueDerechoSeleccion");
			GUI.Label(new Rect (cuantoW, cuantoH, 9 * cuantoW, cuantoH), infoSeleccion[0], "LabelReducido");	//Este texto es el nombre
			GUI.Box(new Rect(cuantoW, 3 * cuantoH, 9 * cuantoW, 4 * cuantoH), "", "");							//Esta es la imagen
			//Habitabilidad --
			GUI.Label(new Rect(cuantoW * 0.9f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[6].ToString("N1"), "Costa"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 2.3f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[1].ToString("N1"), "LLanura"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 3.6f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[2].ToString("N1"), "Colina"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 4.9f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[0].ToString("N1"), "Monta\u00f1a"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 6.2f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[4].ToString("N1"), "Volcanico"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 7.5f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[7].ToString("N1"), "Tundra"), "LabelHabitabilidad");
			GUI.Label(new Rect(cuantoW * 8.9f, 11 * cuantoH, 1 * cuantoW, 1 * cuantoH), new GUIContent(habitabilidadSeleccion[3].ToString("N1"), "Desierto"), "LabelHabitabilidad");
			//Habitabilidad --
			GUI.Label(new Rect (cuantoW * 1, cuantoH * 13, 9 * cuantoW, 1 * cuantoH), "DESCRIPCION:", "LabelDescripcionTitulo");				//Titulo de la descripcion
			GUI.Label(new Rect (cuantoW * 1, cuantoH * 14, 9 * cuantoW, 4 * cuantoH), infoSeleccion[1], "LabelDescripcionContenido");			//Este texto es la descripcion
			switch (tipoSeleccion) {
			case 0:		//Planta
				break;
			case 1: 	//Herbivoro
				break;
			case 2: 	//Carnivoro
				break;
			case 3: 	//Edificio
				break;
			}
			GUI.EndGroup ();
			//TODO Botones del filtro de vegetales
			if (GUI.Button (new Rect (79 * cuantoW, posicionBloqueH * cuantoH, cuantoW, cuantoH), "", "BotonCerrar")) {
				tipoMenuDerecho = tMenuDerecho.ninguno;
			}
			break;
		}	//Fin switch
		if (GUI.changed) {
			//TODO [Maf] Faltan los botones para desactivar carnivoros y herbivoros del tiron, que no los he encontrado. 
			//Solo que se desactiven los ya puestos o un toggle ke se activen en rojo los carnivoros y en verde los herbivoros y sino se desactiven? Lo hablamos.
			/*En todo caso seria algo asi desactivarlos
			 * for (int i = 0; i< materiales.herbivoros.Count; i++){
			 * 		materiales.herbivoros[i].SetFloat("_FiltroOn",0.0f);
			 * 		materiales.herviboros[i].SetColor("_Tinte",Color.white);
			 * }
			 * y lo mismo con carnivoros[i].
			 */
			for (int i = 0; i < togglesFiltros.Length; i++) {
				if (togglesFiltros[i] != togglesFiltrosOld[i]) {
					switch (i) {
					case 0: 		//Boton minerales comunes
						if (togglesFiltros[i])
							materiales.recursos.SetFloat("_ComunesOn",1.0f);
						else
							materiales.recursos.SetFloat("_ComunesOn",0.0f);
						break;
					case 1:			//Boton radio Edificios
						if (togglesFiltros[i])
							materiales.recursos.SetFloat("_EdificiosOn",1.0f);
						else
							materiales.recursos.SetFloat("_EdificiosOn",0.0f);
						break;	
					case 2: 		//Boton minerales raros
						if (togglesFiltros[i])
							materiales.recursos.SetFloat("_RarosOn",1.0f);
						else
							materiales.recursos.SetFloat("_RarosOn",0.0f);
						break;
					case 3:			//boton radio Granjas
						if (togglesFiltros[i])
							materiales.recursos.SetFloat("_GranjasOn",1.0f);
						else
							materiales.recursos.SetFloat("_GranjasOn",0.0f);
						break;
					case 4: 		//boton plantas 1
						if (togglesFiltros[i]) {
							materiales.plantas[0].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[0].SetColor("_Tinte", new Color(0.7f,0.7f,0.5f));
						}
						else {
							materiales.plantas[0].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[0].SetColor("_Tinte", Color.white);
						}
						break;
					case 5: 		//boton plantas 3
						if (togglesFiltros[i]) {
							materiales.plantas[2].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[2].SetColor("_Tinte", new Color(1.0f, 0.5f, 0.0f));
						}
						else {
							materiales.plantas[2].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[2].SetColor("_Tinte", Color.white);
						}
						break;
					case 6: 		//boton plantas 5
						if (togglesFiltros[i]) {
							materiales.plantas[4].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[4].SetColor("_Tinte", Color.red);
						}
						else {
							materiales.plantas[4].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[4].SetColor("_Tinte", Color.white);
						}
						break;
					case 7: 		//boton plantas 7
						if (togglesFiltros[i]) {
							materiales.plantas[6].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[6].SetColor("_Tinte", new Color(0.0f, 0.7f, 0.7f));
						}
						else {
							materiales.plantas[6].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[6].SetColor("_Tinte", Color.white);
						}
						break;
					case 8: 		//boton plantas 9
						if (togglesFiltros[i]) {
							materiales.plantas[8].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[8].SetColor("_Tinte", new Color(0.5f, 0.0f, 1.0f));
						}
						else {
							materiales.plantas[8].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[8].SetColor("_Tinte", Color.white);
						}
						break;
					case 9: 		//boton plantas 2
						if (togglesFiltros[i]) {
							materiales.plantas[1].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[1].SetColor("_Tinte", new Color(1.0f, 1.0f, 0.0f));
						}
						else {
							materiales.plantas[1].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[1].SetColor("_Tinte", Color.white);
						}
						break;
					case 10: 		//boton plantas 4
						if (togglesFiltros[i]) {
							materiales.plantas[3].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[3].SetColor("_Tinte", new Color(1.0f, 0.3f, 0.0f));
						}
						else {
							materiales.plantas[3].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[3].SetColor("_Tinte", Color.white);
						}
						break;
					case 11: 		//boton plantas 6
						if (togglesFiltros[i]) {
							materiales.plantas[5].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[5].SetColor("_Tinte", Color.green);
						}
						else {
							materiales.plantas[5].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[5].SetColor("_Tinte", Color.white);
						}
						break;
					case 12: 		//boton plantas 8
						if (togglesFiltros[i]) {
							materiales.plantas[7].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[7].SetColor("_Tinte", new Color(0.0f, 0.5f, 1.0f));
						}
						else {
							materiales.plantas[7].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[7].SetColor("_Tinte", Color.white);
						}
						break;
					case 13: 		//boton plantas 10
						if (togglesFiltros[i]) {
							materiales.plantas[9].SetFloat("_FiltroOn",1.0f);
							materiales.plantas[9].SetColor("_Tinte", new Color(1.0f, 0.5f, 1.0f));
						}
						else {
							materiales.plantas[9].SetFloat("_FiltroOn",0.0f);
							materiales.plantas[9].SetColor("_Tinte", Color.white);
						}
						break;
					case 14: 		//boton herbivoro 1
						if (togglesFiltros[i]) {
							materiales.herbivoros[0].SetFloat("_FiltroOn",1.0f);
							materiales.herbivoros[0].SetColor("_Tinte", Color.green);
						}
						else {
							materiales.herbivoros[0].SetFloat("_FiltroOn",0.0f);
							materiales.herbivoros[0].SetColor("_Tinte", Color.white);
						}
						break;
					case 15: 		//boton herbivoro 2
						if (togglesFiltros[i]) {
							materiales.herbivoros[1].SetFloat("_FiltroOn",1.0f);
							materiales.herbivoros[1].SetColor("_Tinte", new Color(0.0f, 0.7f, 0.7f));
						}
						else {
							materiales.herbivoros[1].SetFloat("_FiltroOn",0.0f);
							materiales.herbivoros[1].SetColor("_Tinte", Color.white);
						}
						break;
					case 16: 		//boton herbivoro 3
						if (togglesFiltros[i]) {
							materiales.herbivoros[2].SetFloat("_FiltroOn",1.0f);
							materiales.herbivoros[2].SetColor("_Tinte", new Color(0.0f, 0.5f, 1.0f));
						}
						else {
							materiales.herbivoros[2].SetFloat("_FiltroOn",0.0f);
							materiales.herbivoros[2].SetColor("_Tinte", Color.white);
						}
						break;
					case 17: 		//boton herbivoro 4
						if (togglesFiltros[i]) {
							materiales.herbivoros[3].SetFloat("_FiltroOn",1.0f);
							materiales.herbivoros[3].SetColor("_Tinte", new Color(0.5f, 0.0f, 1.0f));
						}
						else {
							materiales.herbivoros[3].SetFloat("_FiltroOn",0.0f);
							materiales.herbivoros[3].SetColor("_Tinte", Color.white);
						}
						break;
					case 18: 		//boton herbivoro 5
						if (togglesFiltros[i]) {
							materiales.herbivoros[4].SetFloat("_FiltroOn",1.0f);
							materiales.herbivoros[4].SetColor("_Tinte", new Color(1.0f, 0.5f, 1.0f));
						}
						else {
							materiales.herbivoros[4].SetFloat("_FiltroOn",0.0f);
							materiales.herbivoros[4].SetColor("_Tinte", Color.white);
						}
						break;
					case 19: 		//boton carnivoro 1
						if (togglesFiltros[i]) {
							materiales.carnivoros[0].SetFloat("_FiltroOn",1.0f);
							materiales.carnivoros[0].SetColor("_Tinte", new Color(0.7f,0.7f,0.5f));
						}
						else {
							materiales.carnivoros[0].SetFloat("_FiltroOn",0.0f);
							materiales.carnivoros[0].SetColor("_Tinte", Color.white);
						}
						break;
					case 20: 		//boton carnivoro 2
						if (togglesFiltros[i]) {
							materiales.carnivoros[1].SetFloat("_FiltroOn",1.0f);
							materiales.carnivoros[1].SetColor("_Tinte", new Color(1.0f, 1.0f, 0.0f));
						}
						else {
							materiales.carnivoros[1].SetFloat("_FiltroOn",0.0f);
							materiales.carnivoros[1].SetColor("_Tinte", Color.white);
						}
						break;
					case 21: 		//boton carnivoro 3
						if (togglesFiltros[i]) {
							materiales.carnivoros[2].SetFloat("_FiltroOn",1.0f);
							materiales.carnivoros[2].SetColor("_Tinte", new Color(1.0f, 0.5f, 0.0f));
						}
						else {
							materiales.carnivoros[2].SetFloat("_FiltroOn",0.0f);
							materiales.carnivoros[2].SetColor("_Tinte", Color.white);
						}
						break;
					case 22: 		//boton carnivoro 4
						if (togglesFiltros[i]) {
							materiales.carnivoros[3].SetFloat("_FiltroOn",1.0f);
							materiales.carnivoros[3].SetColor("_Tinte", new Color(1.0f, 0.3f, 0.0f));
						}
						else {
							materiales.carnivoros[3].SetFloat("_FiltroOn",0.0f);
							materiales.carnivoros[3].SetColor("_Tinte", Color.white);
						}
						break;
					case 23: 		//boton carnivoro 5
						if (togglesFiltros[i]) {
							materiales.carnivoros[4].SetFloat("_FiltroOn",1.0f);
							materiales.carnivoros[4].SetColor("_Tinte", Color.red);
						}
						else {
							materiales.carnivoros[4].SetFloat("_FiltroOn",0.0f);
							materiales.carnivoros[4].SetColor("_Tinte", Color.white);
						}
						break;
					}
					togglesFiltrosOld[i] = togglesFiltros[i];
				}
			}
		}
	}
	
	private bool seleccionarObjetoTablero() {
		int x = 0;
		int y = 0;
		RaycastHit hit;
		if (principal.raycastRoca (Input.mousePosition, ref x, ref y,out hit)) {
			Edificio edificio = principal.vida.tablero[y, x].edificio;
			Vegetal vegetal = principal.vida.tablero[y, x].vegetal;
			Animal animal = principal.vida.tablero[y, x].animal;
			infoSeleccion.Clear();
			if (animal != null || vegetal != null || edificio != null) {
				if (animal != null) {
					infoSeleccion.Add(animal.especie.nombre);									//Cadena infoSeleccion[0]
					TiposSeres tiposSeres = GameObject.FindGameObjectWithTag("TiposSeres").GetComponent<TiposSeres>();
					infoSeleccion.Add(tiposSeres.getDescripcion(tiposSeres.getNumeroSer(animal.especie)));		//Cadena infoSeleccion[1]
					habitabilidadSeleccion[0] = (animal.especie.habitats.Contains(T_habitats.montana)) ? 1 : -1;
					habitabilidadSeleccion[1] = animal.especie.habitats.Contains(T_habitats.llanura) ? 1 : -1;
					habitabilidadSeleccion[2] = animal.especie.habitats.Contains(T_habitats.colina) ? 1 : -1;
					habitabilidadSeleccion[3] = animal.especie.habitats.Contains(T_habitats.desierto) ? 1 : -1;
					habitabilidadSeleccion[4] = animal.especie.habitats.Contains(T_habitats.volcanico) ? 1 : -1;
					habitabilidadSeleccion[6] = animal.especie.habitats.Contains(T_habitats.costa) ? 1 : -1;
					habitabilidadSeleccion[7] = animal.especie.habitats.Contains(T_habitats.tundra) ? 1 : -1;
					//Cadena infoSeleccion[2]
					if (animal.especie.tipo == tipoAlimentacionAnimal.carnivoro) {
						infoSeleccion.Add("Carnivoro");
						tipoSeleccion = 2;
					}
					else {
						infoSeleccion.Add("Herbivoro");
						tipoSeleccion = 1;
					}
					//-----------------------
					//Cadena infoSeleccion[3]
					if (animal.reserva < (animal.especie.reservaMaxima / 4)) { 	//Menor al 25%
						infoSeleccion.Add("Hambiento!");
					}
					else if (animal.reserva < (animal.especie.reservaMaxima / 2)) { 	//Menor al 50%
						infoSeleccion.Add("Bastante");
					}
					else if (animal.reserva < (animal.especie.reservaMaxima / 4) * 3) { 	//Menor al 75%
						infoSeleccion.Add("Poca");
					}
					else
						infoSeleccion.Add("LLeno");
					//-----------------------
					
					//Cadena infoSeleccion[4]
					string temp = "";
					switch (animal.estado) {
					case tipoEstadoAnimal.buscarAlimento: 
						temp = "Buscando comida\n";
						break;
					case tipoEstadoAnimal.comer:
						temp = "Comiendo\n";
						break;
					case tipoEstadoAnimal.descansar:
						temp = "Descansando\n";
						break;
					case tipoEstadoAnimal.morir:
						temp = "Muriendo\n";
						break;
					case tipoEstadoAnimal.nacer:
						temp = "Naciendo\n";
						break;
					}
					infoSeleccion.Add(temp);
					//------------------------
					
					return true;
				}
				else if (vegetal != null) {
					infoSeleccion.Add(vegetal.especie.nombre);									//Cadena infoSeleccion[0]
					TiposSeres temp = GameObject.FindGameObjectWithTag("TiposSeres").GetComponent<TiposSeres>();
					infoSeleccion.Add(temp.getDescripcion(temp.getNumeroSer(vegetal.especie)));		//Cadena infoSeleccion[1]
					habitabilidadSeleccion[0] = vegetal.habitabilidad[0];
					habitabilidadSeleccion[1] = vegetal.habitabilidad[1];
					habitabilidadSeleccion[2] = vegetal.habitabilidad[2];
					habitabilidadSeleccion[3] = vegetal.habitabilidad[3];
					habitabilidadSeleccion[4] = vegetal.habitabilidad[4];
					habitabilidadSeleccion[6] = vegetal.habitabilidad[6];
					habitabilidadSeleccion[7] = vegetal.habitabilidad[7];
					infoSeleccion.Add(vegetal.numVegetales.ToString());		//Cadena infoSeleccion[2]					
					tipoSeleccion = 0;
					return true;
				}
				else if (edificio != null) {
					infoSeleccion.Add(edificio.tipo.nombre);									//Cadena infoSeleccion[0]
					TiposSeres temp = GameObject.FindGameObjectWithTag("TiposSeres").GetComponent<TiposSeres>();
					infoSeleccion.Add(temp.getDescripcion(temp.getNumeroSer(edificio.tipo)));		//Cadena infoSeleccion[1]
					habitabilidadSeleccion[0] = edificio.tipo.habitats.Contains(T_habitats.montana) ? 1 : -1;
					habitabilidadSeleccion[1] = edificio.tipo.habitats.Contains(T_habitats.llanura) ? 1 : -1;
					habitabilidadSeleccion[2] = edificio.tipo.habitats.Contains(T_habitats.colina) ? 1 : -1;
					habitabilidadSeleccion[3] = edificio.tipo.habitats.Contains(T_habitats.desierto) ? 1 : -1;
					habitabilidadSeleccion[4] = edificio.tipo.habitats.Contains(T_habitats.volcanico) ? 1 : -1;
					habitabilidadSeleccion[6] = edificio.tipo.habitats.Contains(T_habitats.costa) ? 1 : -1;
					habitabilidadSeleccion[7] = edificio.tipo.habitats.Contains(T_habitats.tundra) ? 1 : -1;
					//TODO Poner aqui la info escrita que necesitemos para los edificios
					
					tipoSeleccion = 3;
					return true;
				}
			}	//Si la casilla esta vacia...
			tipoSeleccion = -1;
			return false;
		}		//Si el raycast falla...
		tipoSeleccion = -1;
		return false;
	}
	
	private bool seleccionarObjetoInsercion() {
		int tipo = (int)elementoInsercionDerecho - 1;
		TipoEdificio edificio = null;
		EspecieVegetal vegetal = null;
		EspecieAnimal animal = null;
		if (tipo >= 0 && tipo < 5) {
			edificio = principal.vida.tiposEdificios[tipo];
		//Vegetal
		} else if (tipo >= 5 && tipo < 15) {
			tipo -= 5;
			vegetal = (EspecieVegetal)principal.vida.especies[tipo];
		//Animal (herbivoro o carnivoro)
		} else if (tipo >= 15 && tipo < 25) {
			tipo -= 5;
			animal = (EspecieAnimal)principal.vida.especies[tipo];
		}
		infoSeleccion.Clear();
		if (animal != null || vegetal != null || edificio != null) {
			if (animal != null) {
				infoSeleccion.Add(animal.nombre);									//Cadena infoSeleccion[0]
				TiposSeres tiposSeres = GameObject.FindGameObjectWithTag("TiposSeres").GetComponent<TiposSeres>();
				infoSeleccion.Add(tiposSeres.getDescripcion(tiposSeres.getNumeroSer(animal)));		//Cadena infoSeleccion[1]
				habitabilidadSeleccion[0] = (animal.habitats.Contains(T_habitats.montana)) ? 1 : -1;
				habitabilidadSeleccion[1] = animal.habitats.Contains(T_habitats.llanura) ? 1 : -1;
				habitabilidadSeleccion[2] = animal.habitats.Contains(T_habitats.colina) ? 1 : -1;
				habitabilidadSeleccion[3] = animal.habitats.Contains(T_habitats.desierto) ? 1 : -1;
				habitabilidadSeleccion[4] = animal.habitats.Contains(T_habitats.volcanico) ? 1 : -1;
				habitabilidadSeleccion[6] = animal.habitats.Contains(T_habitats.costa) ? 1 : -1;
				habitabilidadSeleccion[7] = animal.habitats.Contains(T_habitats.tundra) ? 1 : -1;
				//Cadena infoSeleccion[2]
				if (animal.tipo == tipoAlimentacionAnimal.carnivoro) {
					infoSeleccion.Add("Carnivoro");
					tipoSeleccion = 2;
				}
				else {
					infoSeleccion.Add("Herbivoro");
					tipoSeleccion = 1;
				}
				//-----------------------
				return true;
			}
			else if (vegetal != null) {
				infoSeleccion.Add(vegetal.nombre);									//Cadena infoSeleccion[0]
				TiposSeres temp = GameObject.FindGameObjectWithTag("TiposSeres").GetComponent<TiposSeres>();
				infoSeleccion.Add(temp.getDescripcion(temp.getNumeroSer(vegetal)));		//Cadena infoSeleccion[1]
				habitabilidadSeleccion[0] = vegetal.habitabilidadInicial[0];
				habitabilidadSeleccion[1] = vegetal.habitabilidadInicial[1];
				habitabilidadSeleccion[2] = vegetal.habitabilidadInicial[2];
				habitabilidadSeleccion[3] = vegetal.habitabilidadInicial[3];
				habitabilidadSeleccion[4] = vegetal.habitabilidadInicial[4];
				habitabilidadSeleccion[6] = vegetal.habitabilidadInicial[6];
				habitabilidadSeleccion[7] = vegetal.habitabilidadInicial[7];
				tipoSeleccion = 0;
				return true;
			}
			else if (edificio != null) {
				infoSeleccion.Add(edificio.nombre);									//Cadena infoSeleccion[0]
				TiposSeres temp = GameObject.FindGameObjectWithTag("TiposSeres").GetComponent<TiposSeres>();
				infoSeleccion.Add(temp.getDescripcion(temp.getNumeroSer(edificio)));		//Cadena infoSeleccion[1]
				habitabilidadSeleccion[0] = edificio.habitats.Contains(T_habitats.montana) ? 1 : -1;
				habitabilidadSeleccion[1] = edificio.habitats.Contains(T_habitats.llanura) ? 1 : -1;
				habitabilidadSeleccion[2] = edificio.habitats.Contains(T_habitats.colina) ? 1 : -1;
				habitabilidadSeleccion[3] = edificio.habitats.Contains(T_habitats.desierto) ? 1 : -1;
				habitabilidadSeleccion[4] = edificio.habitats.Contains(T_habitats.volcanico) ? 1 : -1;
				habitabilidadSeleccion[6] = edificio.habitats.Contains(T_habitats.costa) ? 1 : -1;
				habitabilidadSeleccion[7] = edificio.habitats.Contains(T_habitats.tundra) ? 1 : -1;
				//TODO Poner aqui la info escrita que necesitemos para los edificios
				
				tipoSeleccion = 3;
				return true;
			}
		}	//Si la casilla esta vacia...
		tipoSeleccion = -1;
		return false;
	}
}
