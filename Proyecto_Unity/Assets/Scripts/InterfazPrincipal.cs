using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InterfazPrincipal : MonoBehaviour {

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
	
	//Tooltips
	private Vector3 posicionMouseTooltip 		= Vector3.zero;	//Guarda la ultima posicion del mouse para calcular los tooltips	
	private bool activarTooltip 				= false;		//Controla si se muestra o no el tooltip	
	private float ultimoMov 					= 0.0f;			//Ultima vez que se movio el mouse		
	public float tiempoTooltip 					= 0.75f;		//Tiempo que tarda en aparecer el tooltip	
	
	//Enumerados
	private enum taspectRatio								//Aspecto ratio con el que se pintará la ventana. Si no es ninguno de ellos se aproximará al más cercano
		{aspectRatio16_9, aspectRatio16_10, aspectRatio4_3};
	private taspectRatio aspectRatio;
	
	private enum taccion									//Acción que se esta realizando en el momento actual
		{ninguna,desplegableInsercionV_A,seleccionarInsercion,insertar,mostrarInfoDetallada,mostrarMejoras,mostrarHabilidades,mostrarMenu}
	private taccion accion = taccion.ninguna;
	
	private enum taccionMenu								//Acción que se esta realizando en el menu
		{mostrarMenu,mostrarGuardar,mostrarOpcionesAudio,mostrarSalirMenuPrincipal,mostrarSalirJuego};
	private taccionMenu accionMenu = taccionMenu.mostrarMenu;
	
	private enum tcategoriaInsercion						//Desactivado indica que no hay insercion en curso, otro valor indica la categoria de la insercion
		{desactivada,animal,vegetal,edificio}
	private tcategoriaInsercion categoriaInsercion = tcategoriaInsercion.desactivada;
	
	private enum telementoInsercion							//Tipo de elemento seleccionado en un momento dado
		{ninguno,fabricaCompBas,centralEnergia,granja,fabricaCompAdv,centralEnergiaAdv,seta,flor,cana,arbusto,estromatolito,cactus,palmera,pino,cipres,pinoAlto,
		herbivoro1,herbivoro2,herbivoro3,herbivoro4,herbivoro5,carnivoro1,carnivoro2,carnivoro3,carnivoro4,carnivoro5}	
	private telementoInsercion elementoInsercion = telementoInsercion.ninguno;
	
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
	
	void Start() {
		principal = gameObject.GetComponent<Principal>();
		//Cargar la información del numero de saves que hay
		SaveLoad.compruebaRuta();
		numSaves = SaveLoad.FileCount();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames();
		numSavesExtra = numSaves - 3;
		if (numSavesExtra < 0)
			numSavesExtra = 0;
		//Cargar la informacion del sonido que hay en PlayerPrefs
		
		if (PlayerPrefs.HasKey("MusicaVol"))
			musicaVol = PlayerPrefs.GetFloat("MusicaVol");
		if (PlayerPrefs.HasKey("SfxVol"))
			sfxVol = PlayerPrefs.GetFloat("SfxVol");
		if (PlayerPrefs.HasKey("MusicaOn") && (PlayerPrefs.GetInt("MusicaOn") == 0))
			musicaVol = 0.0f;
		if (PlayerPrefs.HasKey("SfxOn") && (PlayerPrefs.GetInt("SfxOn") == 0))
			sfxVol = 0.0f;
		Audio_Ambiente ambiente = sonidoAmbiente.GetComponent<Audio_Ambiente>();
		if (PlayerPrefs.HasKey("MusicaOn") && (PlayerPrefs.GetInt("MusicaOn") == 0))
			ambiente.activado = false;
		else
			ambiente.activado = true;
		ambiente.volumen = musicaVol;
		//Volumen del audio de efectos
		Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX>();
		if (PlayerPrefs.HasKey("SfxOn") && (PlayerPrefs.GetInt("SfxOn") == 0))
			efectos.activado = false;
		else
			efectos.activado = true;
		efectos.volumen = sfxVol;
	}
	
	// Update is called once per frame
	void Update () 
	{	
		controlTooltip();
		calculaInfoCasilla();		
	}
	
	void OnGUI()
	{		
		GUI.skin = estilo;
		/*[Aris]
		 * Muevo esta linea al método Start porque no es necesario hacerlo cada poco tiempo (no se modifica
		 * en ninguna parte y es una variable privada)
		 * 
		 * [Marcos]
		 * Yo supose que sería un puntero y que tendría coste 0, pero vamos ni lo pense al ponerlo, 
		 * así que lo movemos a start y si vemos que no falla nada pues guay.
		 * */
		aspectRatioNumerico = (float)Screen.width/(float)Screen.height;		
		
		/*[Aris]
		 * El calculo del aspect ratio tambien lo moveria al método Start, porque no necesitas que se ejecute
		 * mas de una vez. Si acaso, poner en el update una condicion de "si el tamaño de la pantalla se modifica..."
		 * Pero lo dejo aqui de momento, para no molestarte :)
		 * 
		 * [Marcos] 
		 * Hacer esa condición con un if y tal puede que llegue incluso a ser más costoso en tiempo. 
		 * Y como la mejora (de haberla) sería totalmente ridicula dejamos eso ahí y ya esta.
		 * */
		if(aspectRatioNumerico >= 1.69)			//16:9
		{
			aspectRatio = taspectRatio.aspectRatio16_9;
			cuantoW = (float)Screen.width / 80;
			cuantoH = (float)Screen.height / 45;		
		}
		else if	(aspectRatioNumerico >= 1.47)	//16:10
		{
			aspectRatio = taspectRatio.aspectRatio16_10;
			cuantoW = (float)Screen.width / 80;
			cuantoH = (float)Screen.height / 50;	
		}
		else 									//4:3
		{
			aspectRatio = taspectRatio.aspectRatio4_3;			
			cuantoW = (float)Screen.width / 80;
			cuantoH = (float)Screen.height / 60;	
		}
		if(accion == InterfazPrincipal.taccion.mostrarMenu)			
			bloqueMenu();
		bloqueSuperior();
		bloqueIzquierdo();
		bloqueSeleccion();
		bloqueInformacion();
		
		if(posicionFueraDeInterfaz(Input.mousePosition))
		{
			mostrarInfoCasilla = true;
			if(accion == taccion.insertar)
				insertarElemento();					
		}
		else
			mostrarInfoCasilla = false;	
		
		if(activarTooltip)			
			mostrarToolTip();		
	}	

	//Dibuja el bloque superior de la ventana que contiene: tiempo, control velocidad, conteo de recursos y menu principal
	private void bloqueSuperior()
	{
		float ajusteRecursos = 0;
		GUI.BeginGroup(new Rect(cuantoW*0,cuantoH*0,cuantoW*80,cuantoH*4));
		GUI.Box(new Rect(cuantoW*0,cuantoH*0,cuantoW*73,cuantoH*4),"","BloqueSuperior");		
		//Tiempo
		GUI.Label(new Rect(cuantoW*2,cuantoH*0,cuantoW*6,cuantoH*2),principal.numPasos.ToString(),"EtiquetaTiempo");
		//Velocidad
		if(GUI.Button(new Rect(cuantoW*3,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Pausa el juego"),"BotonPausa"))
			principal.setEscalaTiempo(0.0f);
		if(GUI.Button(new Rect(cuantoW*4,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad normal"),"BotonVelocidad1"))
			principal.setEscalaTiempo(1.0f);
		if(GUI.Button(new Rect(cuantoW*5,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 2x"),"BotonVelocidad2"))
			principal.setEscalaTiempo(2.0f);
		if(GUI.Button(new Rect(cuantoW*6,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 5x"),"BotonVelocidad5"))
			principal.setEscalaTiempo(5.0f);		
		//Energia
		GUI.Box(new Rect(cuantoW*12,cuantoH*0,cuantoW*2,cuantoH*2),new GUIContent("","Energía"),"IconoEnergia");
		GUI.Box(new Rect(cuantoW*14,cuantoH*0,cuantoW*7,cuantoH*2),new GUIContent("",principal.energia.ToString()+"/"+principal.energiaMax.ToString()),"BoxEnergia");		
		ajusteRecursos = (System.Math.Abs(principal.energiaDif) < 100)? 0: 0.5f;		
		GUI.Label(new Rect(cuantoW*14,cuantoH*0,cuantoW*(5-ajusteRecursos),cuantoH*2),principal.energia.ToString(),"LabelEnergia");
		if(principal.energiaDif >= 0)
		   GUI.Label(new Rect(cuantoW*(19-ajusteRecursos),cuantoH*0,cuantoW*(2+ajusteRecursos),cuantoH*2),"+" + principal.energiaDif.ToString(),"LabelRecursosDifVerde");
		else
			GUI.Label(new Rect(cuantoW*(19-ajusteRecursos),cuantoH*0,cuantoW*(2+ajusteRecursos),cuantoH*2),"" + principal.energiaDif.ToString(),"LabelRecursosDifRojo");
		//Componentes basicos
		GUI.Box(new Rect(cuantoW*22,cuantoH*0,cuantoW*2,cuantoH*2),new GUIContent("","Componentes básicos"),"IconoCompBas");
		GUI.Box(new Rect(cuantoW*24,cuantoH*0,cuantoW*7,cuantoH*2),new GUIContent("",principal.componentesBasicos.ToString()+"/"+principal.componentesBasicosMax.ToString()),"BoxCompBas");
		ajusteRecursos = (System.Math.Abs(principal.componentesBasicosDif) < 100)? 0: 0.5f;		
		GUI.Label(new Rect(cuantoW*24,cuantoH*0,cuantoW*(5-ajusteRecursos),cuantoH*2),principal.componentesBasicos.ToString(),"LabelCompBas");
		if(principal.componentesBasicosDif >= 0)
		   GUI.Label(new Rect(cuantoW*(29-ajusteRecursos),cuantoH*0,cuantoW*(2+ajusteRecursos),cuantoH*2),"+" + principal.componentesBasicosDif.ToString(),"LabelRecursosDifVerde");
		else
			GUI.Label(new Rect(cuantoW*(29-ajusteRecursos),cuantoH*0,cuantoW*(2+ajusteRecursos),cuantoH*2),"" + principal.componentesBasicosDif.ToString(),"LabelRecursosDifRojo");
		//Componentes avanzados
		GUI.Box(new Rect(cuantoW*32,cuantoH*0,cuantoW*2,cuantoH*2),new GUIContent("","Componentes avanzados"),"IconoCompAdv");
		GUI.Box(new Rect(cuantoW*34,cuantoH*0,cuantoW*7,cuantoH*2),new GUIContent("",principal.componentesAvanzados.ToString()+"/"+principal.componentesAvanzadosMax.ToString()),"BoxCompAdv");
		ajusteRecursos = (System.Math.Abs(principal.componentesAvanzadosDif) < 100)? 0: 0.5f;		
		GUI.Label(new Rect(cuantoW*34,cuantoH*0,cuantoW*(5-ajusteRecursos),cuantoH*2),principal.componentesAvanzados.ToString(),"LabelCompAdv");
		if(principal.componentesAvanzadosDif >= 0)
		   GUI.Label(new Rect(cuantoW*(39-ajusteRecursos),cuantoH*0,cuantoW*(2+ajusteRecursos),cuantoH*2),"+" + principal.componentesAvanzadosDif.ToString(),"LabelRecursosDifVerde");
		else
			GUI.Label(new Rect(cuantoW*(39-ajusteRecursos),cuantoH*0,cuantoW*(2+ajusteRecursos),cuantoH*2),"" + principal.componentesAvanzadosDif.ToString(),"LabelRecursosDifRojo");
		//Material biologico
		GUI.Box(new Rect(cuantoW*42,cuantoH*0,cuantoW*2,cuantoH*2),new GUIContent("","Material biológico"),"IconoMatBio");
		GUI.Box(new Rect(cuantoW*44,cuantoH*0,cuantoW*7,cuantoH*2),new GUIContent("",principal.materialBiologico.ToString()+"/"+principal.materialBiologicoMax.ToString()),"BoxMatBio");
		ajusteRecursos = (System.Math.Abs(principal.materialBiologicoDif) < 100)? 0: 0.5f;		
		GUI.Label(new Rect(cuantoW*44,cuantoH*0,cuantoW*(5-ajusteRecursos),cuantoH*2),principal.materialBiologico.ToString(),"LabelMatBio");
		if(principal.materialBiologicoDif >= 0)
		   GUI.Label(new Rect(cuantoW*(49-ajusteRecursos),cuantoH*0,cuantoW*(2+ajusteRecursos),cuantoH*2),"+" + principal.materialBiologicoDif.ToString(),"LabelRecursosDifVerde");
		else
			GUI.Label(new Rect(cuantoW*(49-ajusteRecursos),cuantoH*0,cuantoW*(2+ajusteRecursos),cuantoH*2),"" + principal.materialBiologicoDif.ToString(),"LabelRecursosDifRojo");		
		//Menu
		if(GUI.Button(new Rect(cuantoW*73,cuantoH*0,cuantoW*7,cuantoH*4),new GUIContent("","Accede al menu del juego"),"BotonMenu"))
		{			
			escalaTiempoAntesMenu = principal.escalaTiempo;
			principal.setEscalaTiempo(0);
			accion = InterfazPrincipal.taccion.mostrarMenu;		
			accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
		}
		GUI.EndGroup();
	}
	
	//Dibuja el bloque izquierdo de la ventana que contiene: insertar vegetales o animales, insertar edificios, mejoras de la nave, habilidades, info/seleccionar
	private void bloqueIzquierdo()
	{
		int posicionBloque = 0;
		switch (aspectRatio)
		{
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
		
		if(accion == taccion.desplegableInsercionV_A)
		{
			if(GUI.Button(new Rect(cuantoW*3,cuantoH*posicionBloque,cuantoW*3,cuantoH*1),new GUIContent("Vegetal","Insertar un vegetal"),"BotonesDesplegableV_A"))				
			{
				accion = taccion.seleccionarInsercion;
				categoriaInsercion = tcategoriaInsercion.vegetal;
			}
			if(GUI.Button(new Rect(cuantoW*3,cuantoH*(posicionBloque+1),cuantoW*3,cuantoH*1),new GUIContent("Animal","Insertar un animal"),"BotonesDesplegableV_A"))				
			{
				accion = taccion.seleccionarInsercion;
				categoriaInsercion = tcategoriaInsercion.animal;
			}
		}
		
		GUILayout.BeginArea(new Rect(cuantoW*0,cuantoH*posicionBloque,cuantoW*3,cuantoH*10),new GUIContent(),"BloqueIzquierdo");		
		GUILayout.BeginHorizontal();
		if(mostrarBloqueIzquierdo)
		{			
			GUILayout.BeginVertical(GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*2));			
			if(GUILayout.Button(new GUIContent("","Accede al menu de insertar animales o vegetales"),"BotonInsertarVida"))				
				accion = taccion.desplegableInsercionV_A;			
			if(GUILayout.Button(new GUIContent("","Accede al menu de construir edificios"),"BotonInsertarEdificios"))
			{	
				accion = taccion.seleccionarInsercion;
				categoriaInsercion = tcategoriaInsercion.edificio;	
			}
			if(GUILayout.Button(new GUIContent("","Accede al menu de mejoras de la nave"),"BotonAccederMejoras"))
			{	
				accion = taccion.mostrarMejoras;	
				categoriaInsercion = tcategoriaInsercion.desactivada;
			}				
			if(GUILayout.Button(new GUIContent("","Accede al menu de habilidades"),"BotonAccederHabilidades"))
			{
				accion = taccion.mostrarHabilidades;
				categoriaInsercion = tcategoriaInsercion.desactivada;
			}				
			if(GUILayout.Button(new GUIContent("","Cambia entre info y seleccionar"),"BotonInfoSelec"))
			{
				mostrarInfoCasilla = !mostrarInfoCasilla;
				categoriaInsercion = tcategoriaInsercion.desactivada;
			}				
			GUILayout.EndVertical();
			if(GUILayout.Button(new GUIContent("","Pulsa para ocultar este menu"),"BotonOcultarBloqueIzquierdo",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
			{			
				mostrarBloqueIzquierdo = false;
				if(accion == taccion.desplegableInsercionV_A)
					accion = taccion.ninguna;
			}
		}
		else
			if(GUILayout.Button(new GUIContent("","Pulsa para mostrar el menu de acciones"),"BotonOcultarBloqueIzquierdoActivado",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
				mostrarBloqueIzquierdo = true;	
			
		GUILayout.EndHorizontal();		
		GUILayout.EndArea();
	}
	
	//Dibuja el bloque seleccion de la ventana que contiene los diferentes edificios, animales o vegetales seleccionables según que botón se haya pulsado en el bloque izquierdo
	private void bloqueSeleccion()
	{
		if(accion != taccion.seleccionarInsercion)
			return;
		int posicionBloque = 0;
		
		switch (aspectRatio)
		{
			case taspectRatio.aspectRatio16_9:
				posicionBloque = 40;
				break;
			case taspectRatio.aspectRatio16_10:
				posicionBloque = 45;
				break;
			case taspectRatio.aspectRatio4_3:
				posicionBloque = 55;
				break;
			default:
				break;
		}
		
		switch (categoriaInsercion)
		{
			case tcategoriaInsercion.edificio:
				GUILayout.BeginArea(new Rect(cuantoW*32,cuantoH*posicionBloque,cuantoW*16,cuantoH*4),new GUIContent(),"BloqueSeleccionEdificios");
				GUILayout.BeginVertical();
				GUILayout.Space(cuantoH);				
				GUILayout.BeginHorizontal(GUILayout.Height(cuantoH*2));			
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Fábrica de componentes básicos"),"BotonInsertarFabComBas"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.fabricaCompBas;			
					principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);	
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Central de energía"),"BotonInsertarCenEn"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.centralEnergia;
					principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);	
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Granja"),"BotonInsertarGranja"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.granja;
				/* [Aris]
				 * Yo quitaría esta linea de activar filtro de aqui, porque las granjas no
				 * necesitan recursos para funcionar, no? 
				 * 
				 * [Marcos] 
				 * La puse en su momento para activar los recursos (xq no estaban activados)
				 * para ver donde insertar los edificios. En realidad habria que hacer que si no es
				 * de tipo granja se active. En cualquier caso falta tb ponerle lo del radio de acción.
				 * */
					principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Fábrica de componentes avanzados"),"BotonInsertarFabComAdv"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.fabricaCompBas;
					principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);	
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Central de energía avanzada"),"BotonInsertarCenEnAdv"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.centralEnergiaAdv;
					principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 1);	
				}		
				GUILayout.Space(cuantoW);
				GUILayout.EndHorizontal();
				GUILayout.Space(cuantoH);				
				GUILayout.EndVertical();
				GUILayout.EndArea();
				break;
			case tcategoriaInsercion.animal:
				GUILayout.BeginArea(new Rect(cuantoW*22,cuantoH*posicionBloque,cuantoW*36,cuantoH*4),new GUIContent(),"BloqueSeleccionAnimales");
				GUILayout.BeginVertical();
				GUILayout.Space(cuantoH);				
				GUILayout.BeginHorizontal(GUILayout.Height(cuantoH*2));			
				GUILayout.Space(cuantoW*1.5f);
				if(GUILayout.Button(new GUIContent("","Herbivoro1"),"BotonInsertarHerbivoro1"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.herbivoro1;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Herbivoro2"),"BotonInsertarHerbivoro2"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.herbivoro2;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Herbivoro3"),"BotonInsertarHerbivoro3"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.herbivoro3;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Herbivoro4"),"BotonInsertarHerbivoro4"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.herbivoro4;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Herbivoro5"),"BotonInsertarHerbivoro5"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.herbivoro5;
				}	
				GUILayout.Space(cuantoW);
				GUILayout.Box("",GUILayout.Width(cuantoW*1),GUILayout.Height(cuantoH*2));
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Carnivoro1"),"BotonInsertarCarnivoro1"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.carnivoro1;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Carnivoro2"),"BotonInsertarCarnivoro2"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.carnivoro2;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Carnivoro3"),"BotonInsertarCarnivoro3"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.carnivoro3;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Carnivoro4"),"BotonInsertarCarnivoro4"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.carnivoro4;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Carnivoro5"),"BotonInsertarCarnivoro5"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.carnivoro5;
				}		
				GUILayout.Space(cuantoW*1.5f);
				GUILayout.EndHorizontal();
				GUILayout.Space(cuantoH);				
				GUILayout.EndVertical();
				GUILayout.EndArea();
				break;
			case tcategoriaInsercion.vegetal:
				GUILayout.BeginArea(new Rect(cuantoW*22,cuantoH*posicionBloque,cuantoW*36,cuantoH*4),new GUIContent(),"BloqueSeleccionVegetales");
				GUILayout.BeginVertical();
				GUILayout.Space(cuantoH);				
				GUILayout.BeginHorizontal(GUILayout.Height(cuantoH*2));			
				GUILayout.Space(cuantoW*2.5f);
				if(GUILayout.Button(new GUIContent("","Seta"),"BotonInsertarSeta"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.seta;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Flor"),"BotonInsertarFlor"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.flor;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Caña"),"BotonInsertarCana"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.cana;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Arbusto"),"BotonInsertarArbusto"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.arbusto;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Estromatolito"),"BotonInsertarEstromatolito"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.estromatolito;
				}		
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Cactus"),"BotonInsertarCactus"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.cactus;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Palmera"),"BotonInsertarPalmera"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.palmera;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Pino"),"BotonInsertarPino"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.pino;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Ciprés"),"BotonInsertarCipres"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.cipres;
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Pino Alto"),"BotonInsertarPinoAlto"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.pinoAlto;
				}		
				GUILayout.Space(cuantoW*2.5f);
				GUILayout.EndHorizontal();
				GUILayout.Space(cuantoH);				
				GUILayout.EndVertical();
				GUILayout.EndArea();
				break;
			default:
				break;
		}
		
	}
	
	//Dibuja el bloque de información básica de la casilla a la que estamos apuntando
	private void bloqueInformacion()
	{
		/* [Aris]
		 * En algunos sitios he visto que haces esto, hacer un switch para conseguir
		 * la posicion que es un cuantoH justo por encima del final de la pantalla.
		 * Creo que es más facil conseguirla con esta formula:
		 	* Screen.Height - CuantoH
	 	 * Es básicamente lo mismo que lo que haces con el switch y luego poner
	 	 * cuantoH * posicionBloque.
	 	 * Es una sugerencia nada mas ;)
	 	 * 
	 	 * [Marcos] 
	 	 * Sólo lo has visto aquí y es así porque el bloque de información tiene
	 	 * un cuanto de alto y está abajo de todo. Si hacemos lo que tu dices tendríamos 
	 	 * mayor coste porque habria que hacer:
	 	 * 		Screen.Height /numCuantos (que depende de cada aspect ratio) - CuantoH
	 	 * Y en caso de querer poner el bloque en otra parte (por lo que fuera) habría que
	 	 * cambiar todo.
		 * */
		int posicionBloque = 0;
		switch (aspectRatio)
		{
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
		GUI.Box(new Rect(cuantoW*0,cuantoH*posicionBloque,cuantoW*100,cuantoH*1),infoCasilla,"BloqueInformacion");		
	}
	
	//Dibuja el menu de opciones que contiene Guardar, Opciones de audio, Menu Principal, Salir, Volver
	private void bloqueMenu()
	{
		/* [Aris]
		 * Aqui tenias tres bloques switch sobre la misma variable, asi que los he combinado en uno solo
		 * mas que nada porque es una perdida de lineas de codigo para hacer lo mismo...
		 * Si tienes otras razones, volvemos para atras y fuera. 
		 * 
		 * [Marcos]
		 * Está puesto en 3 switch porque lo hice deprisa y corriendo antes de irme al pueblo. Evidentemente
		 * con un sólo switch está mucho mejor y te agradezco enormemente la observación.
		 * */
		float posicionBloque = 0;
		float posicionConfirmar = 0;
		float posicionAudio = 0;
		float posicionGuardar = 0;
		switch (aspectRatio)
		{
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
		
		switch(accionMenu)
		{
			case taccionMenu.mostrarMenu:				
				GUILayout.BeginArea(new Rect(cuantoW*32.5f,cuantoH*posicionBloque,cuantoW*15,cuantoH*18),new GUIContent());
				GUILayout.BeginVertical();
				GUILayout.Box(new GUIContent(),"BloqueMenu",GUILayout.Height(cuantoH*3),GUILayout.Width(cuantoW*15));
				if(GUILayout.Button(new GUIContent("Guardar partida","Lleva al menu de guardar partida"),"BotonGuardarPartida",GUILayout.Height(cuantoH*3),GUILayout.Width(cuantoW*15)))									
					accionMenu = InterfazPrincipal.taccionMenu.mostrarGuardar;				
				if(GUILayout.Button(new GUIContent("Opciones de audio","Lleva al menu de opciones de audio"),"BotonOpcionesAudio",GUILayout.Height(cuantoH*3),GUILayout.Width(cuantoW*15)))
					accionMenu = InterfazPrincipal.taccionMenu.mostrarOpcionesAudio;
				if(GUILayout.Button(new GUIContent("Menu principal","Lleva al menu principal del juego"),"BotonMenuPrincipal",GUILayout.Height(cuantoH*3),GUILayout.Width(cuantoW*15)))
					accionMenu = InterfazPrincipal.taccionMenu.mostrarSalirMenuPrincipal;
				if(GUILayout.Button(new GUIContent("Salir del juego","Cierra completamente el juego"),"BotonSalirJuego",GUILayout.Height(cuantoH*3),GUILayout.Width(cuantoW*15)))
					accionMenu = InterfazPrincipal.taccionMenu.mostrarSalirJuego;
				if(GUILayout.Button(new GUIContent("Volver","Vuelve a la partida"),"BotonVolver",GUILayout.Height(cuantoH*3),GUILayout.Width(cuantoW*15)))
				{
					accion = InterfazPrincipal.taccion.ninguna;
					principal.setEscalaTiempo(escalaTiempoAntesMenu);
				}	
			    GUILayout.EndVertical();
				GUILayout.EndArea();
				if(GUI.Button(new Rect(0,0,cuantoW*80,cuantoH*60),new GUIContent(),""))
				{
					;//CLINK -> Control para que no se pulse fuera del menú (Buena idea!)
				}
				break;
			
			case taccionMenu.mostrarGuardar:
			/* [Aris]
			 * Re-escrito. Deberia funcionar ahora, al menos esta parte esta bien escrita.
			 * Otra cosa será que no funcione la parte de guardar... Pero esto es solo interfaz
			 * con poca lógica asociada.
			 * */
		        GUI.Box(new Rect(cuantoW*30,cuantoH*posicionGuardar,cuantoW*20,cuantoH*16),new GUIContent(""),"BoxGuardar");
		        posicionScroll = GUI.BeginScrollView(new Rect(cuantoW*30,cuantoH*posicionGuardar,cuantoW*20,cuantoH*14), posicionScroll, new Rect(0, 0, cuantoW * 20, cuantoH * 4 * numSavesExtra));
		        if (GUI.Button(new Rect(cuantoW, 0, cuantoW * 18, cuantoH * 4), new GUIContent("Nueva partida salvada", "Guardar una nueva partida"))) 
				{
					GameObject contenedor = GameObject.FindGameObjectWithTag("Carga");
		            ValoresCarga temp = contenedor.GetComponent<ValoresCarga>();
					principal.rellenaContenedor(ref temp);
		            string fecha = System.DateTime.Now.ToString().Replace("\\","").Replace("/","").Replace(" ", "").Replace(":","");
		            SaveLoad.cambiaFileName("Partida" + fecha + ".hur"); 
		            SaveLoad.Save(temp);
		            //Recuperar estado normal
		           	accion = InterfazPrincipal.taccion.ninguna;
					principal.setEscalaTiempo(escalaTiempoAntesMenu);
		        }
		        for (int i = 0; i < numSaves; i++) {
	                if (GUI.Button(new Rect(cuantoW, (i + 1) * cuantoH * 4, cuantoW * 18, cuantoH * 4), new GUIContent(nombresSaves[i], "Sobreescribir partida num. " + i))) {
                        GameObject contenedor = GameObject.FindGameObjectWithTag("Carga");
		            	ValoresCarga temp = contenedor.GetComponent<ValoresCarga>();
						principal.rellenaContenedor(ref temp);
                        SaveLoad.cambiaFileName(nombresSaves[i]);               
                        SaveLoad.Save(temp);
                        //Recuperar estado normal
                        accion = InterfazPrincipal.taccion.ninguna;
						principal.setEscalaTiempo(escalaTiempoAntesMenu);
	                }
		        }
		        GUI.EndScrollView();
		        if (GUI.Button(new Rect(cuantoW*44,cuantoH*(posicionGuardar+13),cuantoW*5,cuantoH*2), new GUIContent("Volver", "Pulsa aquí para volver al menu de opciones"))) {
	                accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
		        }		
				break;
			case taccionMenu.mostrarOpcionesAudio:
				GUI.Box(new Rect(cuantoW*32.5f,cuantoH*posicionAudio,cuantoW*15,cuantoH*12),new GUIContent(),"BoxOpcionesAudio");
				GUI.Label(new Rect(cuantoW*33f,cuantoH*(posicionAudio+1f),cuantoW*14.5f,cuantoH*2),new GUIContent("Sonido"));
				GUI.Label(new Rect(cuantoW*33f,cuantoH*(posicionAudio+3f),cuantoW*8,cuantoH*2),new GUIContent("Volumen"));
				GUI.Label(new Rect(cuantoW*33f,cuantoH*(posicionAudio+5f),cuantoW*5.5f,cuantoH*2),new GUIContent("Música"));
				musicaVol = GUI.HorizontalSlider(new Rect(cuantoW*39f,cuantoH*(posicionAudio+5f),cuantoW*8,cuantoH*2),musicaVol,0,1.0f);	
				GUI.Label(new Rect(cuantoW*33f,cuantoH*(posicionAudio+7f),cuantoW*5.5f,cuantoH*2),new GUIContent("Efectos"));
				sfxVol = GUI.HorizontalSlider(new Rect(cuantoW*39f,cuantoH*(posicionAudio+7f),cuantoW*8,cuantoH*2),sfxVol,0,1.0f);
				if(GUI.Button(new Rect(cuantoW*42f,cuantoH*(posicionAudio+9.5f),cuantoW*5f,cuantoH*2f),new GUIContent("Volver","Pulsa aquí para volver al menu de opciones")))
					accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;			
				
				if(GUI.changed)
				{
					//Volumen del audio ambiente
					Audio_Ambiente ambiente = sonidoAmbiente.GetComponent<Audio_Ambiente>();
					if (musicaVol == 0)
						ambiente.activado = false;
					else
						ambiente.activado = true;
					ambiente.volumen = musicaVol;
					//Volumen del audio de efectos
					Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX>();
					if (sfxVol == 0)
						efectos.activado = false;
					else
						efectos.activado = true;
					efectos.volumen = sfxVol;
				}				
				break;
			case taccionMenu.mostrarSalirMenuPrincipal:
				GUI.Box(new Rect(cuantoW*36,cuantoH*posicionConfirmar,cuantoW*8,cuantoH*4),new GUIContent(),"BoxConfirmacion");
				GUI.Label(new Rect(cuantoW*37,cuantoH*(posicionConfirmar),cuantoW*6,cuantoH*2),new GUIContent("¿Está seguro?"));
				if(GUI.Button(new Rect(cuantoW*37,cuantoH*(posicionConfirmar+2),cuantoW*2.5f,cuantoH*1.5f),new GUIContent("Si","Pulsa aquí para salir al menu principal")))
					Application.LoadLevel("Escena_Inicial");
				if(GUI.Button(new Rect(cuantoW*40.5f,cuantoH*(posicionConfirmar+2),cuantoW*2.5f,cuantoH*1.5f),new GUIContent("No","Pulsa aquí para volver al menu de opciones")))
					accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
				break;
			case taccionMenu.mostrarSalirJuego:			
				GUI.Box(new Rect(cuantoW*36,cuantoH*posicionConfirmar,cuantoW*8,cuantoH*4),new GUIContent(),"BoxConfirmacion");
				GUI.Label(new Rect(cuantoW*37,cuantoH*(posicionConfirmar),cuantoW*6,cuantoH*2),new GUIContent("¿Está seguro?"));
				if(GUI.Button(new Rect(cuantoW*37,cuantoH*(posicionConfirmar+2),cuantoW*2.5f,cuantoH*1.5f),new GUIContent("Si","Pulsa aquí para salir del juego")))
					Application.Quit();
				if(GUI.Button(new Rect(cuantoW*40.5f,cuantoH*(posicionConfirmar+2),cuantoW*2.5f,cuantoH*1.5f),new GUIContent("No","Pulsa aquí para volver al menu de opciones")))
					accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;				
				break;
			default:
				break;			
		}
		
	}
	
	private void insertarElemento()	
	{
		if(accion == taccion.insertar)
		{	
			//pintar modelo en tiempo real y area de efecto si es necesario
			/* [Aris]
			 * Recomendación: En vez de sacar el GetMouseButton, creo que es mejor seguir el 
			 * GetMouseButtonDown(0), de forma que solo tenga en cuenta el click en si mismo, para 
			 * evitar que la condicion se pueda producir mas de una vez (GetMouseButton se cumple mientras
			 * siga pulsado, lo que puede ser 0.3 segundos perfectamente y eso en un método que se refresca
			 * unas 100 veces por segundo o mas... puede dar problemas no? Porque al menos un par pueden
			 * pasar...
			 * 
			 * [Marcos]
			 * Efectivamente quería poner sólo el de Down y por descuido puse ese. En realidad funciona porque 
			 * ya hay una lógica que se encarga de hacer que sólo se inserte una vez. Vamos desde que esto está
			 * funcionando, habremos insertado cientos de cosas y siempre lo ha hecho bien así que creo que 
			 * funciona perfectamente.
			 * */
			if(Input.GetMouseButtonDown(0))
			{
				int x = 0;
				int y = 0;
				if(principal.raycastRoca(Input.mousePosition,ref x,ref y))
				{
					int tipo = (int)elementoInsercion - 1;
					if(tipo >= 0 && tipo < 5)				//Edificio
					{
						TipoEdificio[] tipos = new TipoEdificio[principal.vida.tiposEdificios.Count];
						principal.vida.tiposEdificios.Values.CopyTo(tipos,0);
						TipoEdificio tedif = tipos[tipo];
						if(principal.recursosSuficientes(tedif.energiaConsumidaAlCrear,tedif.compBasConsumidosAlCrear,tedif.compAvzConsumidosAlCrear,tedif.matBioConsumidoAlCrear)) {							
							if(principal.vida.anadeEdificio(tedif,y,x,0,0,0,0,10,10,10,10)) {
								principal.consumeRecursos(tedif.energiaConsumidaAlCrear,tedif.compBasConsumidosAlCrear,tedif.compAvzConsumidosAlCrear,tedif.matBioConsumidoAlCrear);
								principal.modificaRecursosPorTurno(10,10,10,10);								
							}
							else {								
								;//Mostrar por pantalla que no se ha podido insertar por que el habitat no es el adecuado o xq ya existe un edificio ahi
							}
						}
						else {
							;//Mostrar por pantalla que no se ha podido insertar por falta de recursos
						}						
						elementoInsercion = telementoInsercion.ninguno;
						accion = taccion.ninguna;	
						principal.objetoRoca.renderer.sharedMaterials[3].SetFloat("_FiltroOn", 0);	
					}					
					else if(tipo >= 5 && tipo < 15)			//Vegetal
					{
						tipo -= 5;
						Especie[] especies = new Especie[principal.vida.especies.Count];
						principal.vida.especies.Values.CopyTo(especies,0);						
						EspecieVegetal especie = (EspecieVegetal)especies[tipo];
						principal.vida.anadeVegetal(especie,y,x);
						elementoInsercion = telementoInsercion.ninguno;
						accion = taccion.ninguna;
					}
					else if(tipo >= 15 && tipo < 25)		//Animal (herbivoro o carnivoro)
					{
						tipo -= 5;	
						Especie[] especies = new Especie[principal.vida.especies.Count];
						principal.vida.especies.Values.CopyTo(especies,0);
						EspecieAnimal especie = (EspecieAnimal)especies[tipo];
						principal.vida.anadeAnimal(especie,y,x);
						elementoInsercion = telementoInsercion.ninguno;
						accion = taccion.ninguna;
					}
					else
						return;
				}			
			}			
		}
	}	
	//Obtiene la información básica de la casilla a mostrar en la barra de información inferior
	private void calculaInfoCasilla()
	{
		if(mostrarInfoCasilla)
		{
			if(Input.mousePosition != posicionMouseInfoCasilla && Time.realtimeSinceStartup > tiempoUltimaInfoCasilla + tiempoInfoCasilla)
			{
				posicionMouseInfoCasilla = Input.mousePosition;
				tiempoUltimaInfoCasilla = Time.realtimeSinceStartup;
				int x = 0;
				int y = 0;
				if(principal.raycastRoca(Input.mousePosition,ref x,ref y))
				{
					T_habitats habitat = principal.vida.tablero[y,x].habitat;
					T_elementos elem = principal.vida.tablero[y,x].elementos;					
					Edificio edificio = principal.vida.tablero[y,x].edificio;
					Vegetal vegetal = principal.vida.tablero[y,x].vegetal;
					Animal animal = principal.vida.tablero[y,x].animal;
										
					if(habitat == T_habitats.montana)
						infoCasilla = "Hábitat: montaña" + "\t\t";
					else
						infoCasilla = "Hábitat: " + habitat.ToString() + "\t\t";
					if(elem == T_elementos.comunes)
						infoCasilla += "Elementos: metales comunes" + "\t\t";
					else if(elem == T_elementos.raros)
						infoCasilla += "Elementos: metales raros" + "\t\t";					
					
					if(edificio != null)
						infoCasilla += "Edificio: " + edificio.tipo.nombre + "\t\t";
					if(vegetal != null)
						infoCasilla += "Vegetal: " + vegetal.especie.nombre + "\t\t";
					if(animal != null)
						infoCasilla += "Animal: " + animal.especie.nombre + "\t\t";					
				}			
			}						
		}	
		else
			infoCasilla = "";
	}
	
	//Controla si se tiene que mostrar el tooltip o no
	private void controlTooltip()
	{
		if (Input.mousePosition != posicionMouseTooltip) 
		{
			posicionMouseTooltip = Input.mousePosition;
			activarTooltip = false;
			ultimoMov = Time.realtimeSinceStartup;
		}
		else 
			if (Time.realtimeSinceStartup >= ultimoMov + tiempoTooltip)
				activarTooltip = true;		
	}	
	
	//Muestra el tooltip si ha sido activado
	private void mostrarToolTip()
	{
		float longitud = GUI.tooltip.Length;
		if (longitud == 0.0f) 
			return;			
		else {
			/* [Aris] 
			 * Hago aqui un pequeño añadido para mejorar la longitud del tooltip, que antes
			 * salia un poco mal, sobretodo en las muy pequeñas como por ejemplo el tooltip 
			 * de los recursos del bloque superior.
			 * */
			if (longitud < 8)
				longitud *= 10.0f;
			else if (longitud < 15)
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
			posy += 20;					
		Rect pos = new Rect(posx, Screen.height - posy, longitud, 25);
		GUI.Box(pos, "");
		GUI.Label(pos, GUI.tooltip);					
	}	
	
	//Devuelve true si el raton está fuera de la interfaz y por tanto es válido y false si cae dentro de la interfaz dibujada en ese momento
	public bool posicionFueraDeInterfaz(Vector3 posicionRaton)		
	{
		/* [Aris] 
		 * Esto es solo una sugerencia, pero... sería mucho mas preciso y mas fácil,
		 * al menos en mi cabeza, que lo hicieras comprobando por zonas de Rect().
		 * Es la forma que usan en todos los sitios que he mirado y es muy sencilla 
		 * de hacer porque esos Rect ya tienen que estar creados por ahi... Es tan facil
		 * como crear unos rectangulos Rect que representen los bloques que hay activos
		 * de la interfaz, y llevar una variable booleana que nos diga si el cursor
		 * se encuentra dentro de alguno de ellos. Para comprobarlo, basta con lanzar la 
		 * funcion Rect.Contains(Input.mousePosition), o en este caso
		 * Rect.Contains(posicionRaton). Eso devuelve true si se encuentra dentro.
		 * 
		 * Te pongo un ejemplo de código por si te animas a hacerlo:
		 
		 	bool dentro = false;
		 	Rect temp = new Rect(0,0,cuantoW*80, cuantoH*4);						//Bloque superior
		 	if (temp.Contains(posicionRaton))
		 		dentro = true;
		 	temp = new Rect(0, Screen.height - cuantoH, cuantoW * 80, cuantoH);		//Barra informacion inferior
		 	if (temp.Contains(posicionRaton))
		 		dentro = true;
		 
		 * Y así sucesivamente...
		 * 
		 * [Marcos]
		 * Lo más exacto sería eso, tener guardadas todas las rect que he usado y comprobar si está dentro o no.
		 * Pero como no hay tiempo de sobra pues... así funciona y es lo que importa xDD. Ya lo cambiaré si hay tiempo.
		 * De todas formas con el ejemplo que tu propones al final es lo mismo. Una cosa es hacerse una lista de todos
		 * los rect que hay y ver que el puntero NO está contenido en ellos y otra cosaes mirar simplemente si esta 
		 * contenido dentro de un rect permitido. Lo que hago es lo segundo y es lo mismo comparar con 4 posiciones que 
		 * con 1 rect, porque por dentro será lo que haga. Vamos que de hacerlo como dices a raiz de lo que hay, sería 
		 * simplemente hacer un Rect al final con las posiciones que se han hallado antes y usar el contains y es algo 
		 * que te va a costar más xq estas duplicando los datos para luego hacer la misma comprobación. Porque casi 
		 * 100% seguro que el contains te hace lo mismo que hago yo xDD.
		 * */
		if(accion == taccion.mostrarMenu)
			return false;
		float xini,xfin,yini,yfin;		
		yfin = Screen.height - cuantoH*4;				//Posición donde termina el bloque superior
		if(mostrarBloqueIzquierdo)
			xini = cuantoW*3;							//Posición donde termina el bloque izquierdo
		else
			xini = 0;									//Tamaño mínimo de la ventana
		
		//if(mostrarBloqueDerecho)
			//xfin = ?									//Posición donde empieza el bloque derecho
		//else
		xfin = cuantoW*80;								//Tamaño máximo de la ventana
		
		if(categoriaInsercion != tcategoriaInsercion.desactivada)
		{
			int posicionBloqueSeleccion = 0;
			switch (aspectRatio)
			{
				case taspectRatio.aspectRatio16_9:
					posicionBloqueSeleccion = 40;
					break;
				case taspectRatio.aspectRatio16_10:
					posicionBloqueSeleccion = 45;
					break;
				case taspectRatio.aspectRatio4_3:
					posicionBloqueSeleccion = 55;
					break;
				default:break;
			}		
			yini = Screen.height - cuantoH*posicionBloqueSeleccion;		//Posición donde empieza el bloque de seleccion	
		}
		else
		{
			int posicionBloqueInformacion = 0;
			switch (aspectRatio)
			{
				case taspectRatio.aspectRatio16_9:
					posicionBloqueInformacion = 44;
					break;
				case taspectRatio.aspectRatio16_10:
					posicionBloqueInformacion = 49;
					break;
				case taspectRatio.aspectRatio4_3:
					posicionBloqueInformacion = 59;
					break;
				default:break;
			}	
			yini = Screen.height - cuantoH*posicionBloqueInformacion;	//Posición donde empieza el bloque de informacion	
		}
		if(posicionRaton.x > xini && posicionRaton.x < xfin && posicionRaton.y > yini && posicionRaton.y < yfin)
			return true;
		else 
			return false;
	}
}
