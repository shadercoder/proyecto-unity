using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InterfazPrincipal : MonoBehaviour {

	// Variables ---------------------------------------------------------------------------------------------------------------------------
	public GUISkin estilo;
	private float cuantoW;									//Minima unidad de medida de la interfaz a lo ancho
	private float cuantoH;									//Minima unidad de medida de la interfaz a lo alto
	private float aspectRatioNumerico;						//Aspect ratio númerico de la ventana
	private Principal principal;							//Acceso a los datos principales	
	private bool mostrarBloqueIzquierdo = true;				//Visibilidad del bloque de opciones izquierdo
	private bool mostrarInfoCasilla = true;					//Controla si se muestra la info básica de la casilla a la que estamos apuntando
	private string infoCasilla = "";						//Información básica de la casilla mostrada en la barra de información inferior
	private float tiempoUltimaInfoCasilla = 0.0f;			//Tiempo de la última comprobación de la info básica de una casilla
	private float tiempoInfoCasilla = 0.25f;				//Cantidad mínima de tiempo entre comprobaciones de la info básica de una casilla
	private Vector3 posicionMouseInfoCasilla = Vector3.zero;//Guarda la ultima posicion del mouse para calcular los tooltips	
	private bool mostrarMenu = false;						//Controla si se muestra el menu de opciones
	
	//Tooltips
	private Vector3 posicionMouseTooltip = Vector3.zero;	//Guarda la ultima posicion del mouse para calcular los tooltips	
	private bool activarTooltip = false;					//Controla si se muestra o no el tooltip	
	private float ultimoMov = 0.0f;							//Ultima vez que se movio el mouse		
	public float tiempoTooltip = 0.75f;						//Tiempo que tarda en aparecer el tooltip	
	
	//Enumerados
	private enum taspectRatio								//Aspecto ratio con el que se pintará la ventana. Si no es ninguno de ellos se aproximará al más cercano
		{aspectRatio16_9,aspectRatio16_10,aspectRatio4_3};
	private taspectRatio aspectRatio;	
	private enum taccion									//Acción que se esta realizando en el momento actual
		{ninguna,desplegableInsercionV_A,seleccionarInsercion,insertar,mostrarInfoDetallada,mostrarMejoras,mostrarHabilidades}
	private taccion accion = taccion.ninguna;
	private enum tcategoriaInsercion						//Desactivado indica que no hay insercion en curso, otro valor indica la categoria de la insercion
		{desactivada,animal,vegetal,edificio}
	private tcategoriaInsercion categoriaInsercion = tcategoriaInsercion.desactivada;
	private enum telementoInsercion							//Tipo de elemento seleccionado en un momento dado
		{ninguno,fabricaCompBas,centralEnergia,granja,fabricaCompAdv,centralEnergiaAdv,seta,flor,cana,arbusto,estromatolito,cactus,palmera,pino,cipres,pinoAlto,
		herbivoro1,herbivoro2,herbivoro3,herbivoro4,herbivoro5,carnivoro1,carnivoro2,carnivoro3,carnivoro4,carnivoro5}	
	private telementoInsercion elementoInsercion = telementoInsercion.ninguno;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () 
	{
		principal = gameObject.GetComponent<Principal>();		
		controlTooltip();
		calculaInfoCasilla();		
	}
	
	void OnGUI()
	{		
		GUI.skin = estilo;				
		principal = gameObject.GetComponent<Principal>();		
		aspectRatioNumerico = (float)Screen.width/(float)Screen.height;		
		
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
		bloqueSuperior();
		bloqueIzquierdo();
		bloqueSeleccion();
		if(posicionFueraDeInterfaz(Input.mousePosition))
		{
			mostrarInfoCasilla = true;
			if(accion == taccion.insertar)
				insertarElemento();					
		}
		else
			mostrarInfoCasilla = false;
			
		bloqueInformacion();			
		if(activarTooltip)			
			mostrarToolTip();			
	}	

	//Dibuja el bloque superior de la ventana que contiene: tiempo, control velocidad, conteo de recursos y menu principal
	void bloqueSuperior()
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
			mostrarMenu = true;			
		GUI.EndGroup();
	}
	
	
	
	//Dibuja el bloque izquierdo de la ventana que contiene: insertar vegetales o animales, insertar edificios, mejoras de la nave, habilidades, info/seleccionar
	void bloqueIzquierdo()
	{
		int posicionBloque = 0;
		switch (aspectRatio)
		{
			case taspectRatio.aspectRatio16_9:
				posicionBloque = 17;break;
			case taspectRatio.aspectRatio16_10:
				posicionBloque = 20;break;
			case taspectRatio.aspectRatio4_3:
				posicionBloque = 25;break;
			default:break;
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
	void bloqueSeleccion()
	{
		if(categoriaInsercion == tcategoriaInsercion.desactivada)
			return;
		int posicionBloque = 0;
		switch (aspectRatio)
		{
			case taspectRatio.aspectRatio16_9:
				posicionBloque = 40;break;
			case taspectRatio.aspectRatio16_10:
				posicionBloque = 45;break;
			case taspectRatio.aspectRatio4_3:
				posicionBloque = 55;break;
			default:break;
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
					principal.objetoRoca.renderer.sharedMaterials[4].SetFloat("_FiltroOn", 1);	
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Central de energía"),"BotonInsertarCenEn"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.centralEnergia;
					principal.objetoRoca.renderer.sharedMaterials[4].SetFloat("_FiltroOn", 1);	
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Granja"),"BotonInsertarGranja"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.granja;
					principal.objetoRoca.renderer.sharedMaterials[4].SetFloat("_FiltroOn", 1);	
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Fábrica de componentes avanzados"),"BotonInsertarFabComAdv"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.fabricaCompBas;
					principal.objetoRoca.renderer.sharedMaterials[4].SetFloat("_FiltroOn", 1);	
				}
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Central de energía avanzada"),"BotonInsertarCenEnAdv"))
				{	
					accion = taccion.insertar;
					elementoInsercion = telementoInsercion.centralEnergiaAdv;
					principal.objetoRoca.renderer.sharedMaterials[4].SetFloat("_FiltroOn", 1);	
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
			default:break;
		}
		
	}
	
	//Obtiene la información básica de la casilla a mostrar en la barra de información inferior
	void calculaInfoCasilla()
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
	
	//Dibuja el bloque de información básica de la casilla a la que estamos apuntando
	void bloqueInformacion()
	{
		int posicionBloque = 0;
		switch (aspectRatio)
		{
			case taspectRatio.aspectRatio16_9:
				posicionBloque = 44;break;
			case taspectRatio.aspectRatio16_10:
				posicionBloque = 49;break;
			case taspectRatio.aspectRatio4_3:
				posicionBloque = 59;break;
			default:break;
		}
		GUI.Box(new Rect(cuantoW*0,cuantoH*posicionBloque,cuantoW*100,cuantoH*1),infoCasilla,"BloqueInformacion");		
	}
	
	void insertarElemento()	
	{
		if(accion == taccion.insertar)
		{	
			//pintar modelo en tiempo real y area de efecto si es necesario			
			if(Input.GetMouseButton(0))
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
						if(principal.recursosSuficientes(tedif.energiaConsumidaAlCrear,tedif.compBasConsumidosAlCrear,tedif.compAvzConsumidosAlCrear,
						                             	tedif.matBioConsumidoAlCrear))
						{							
							if(principal.vida.anadeEdificio(tedif,y,x,0,0,0,0,10,10,10,10))
							{
								principal.consumeRecursos(tedif.energiaConsumidaAlCrear,tedif.compBasConsumidosAlCrear,tedif.compAvzConsumidosAlCrear,
								                         tedif.matBioConsumidoAlCrear);
								principal.modificaRecursosPorTurno(10,10,10,10);								
							}
							else									
								;//Mostrar por pantalla que no se ha podido insertar por que el habitat no es el adecuado o xq ya existe un edificio ahi
						}
						else
							//Mostrar por pantalla que no se ha podido insertar por falta de recursos
						
						elementoInsercion = telementoInsercion.ninguno;
						accion = taccion.ninguna;	
						principal.objetoRoca.renderer.sharedMaterials[4].SetFloat("_FiltroOn", 0);	
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
	
	//Controla si se tiene que mostrar el tooltip o no
	void controlTooltip()
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
	void mostrarToolTip()
	{
		float longitud = GUI.tooltip.Length;
		if (longitud == 0.0f) 
			return;			
		else 
			longitud *= 8.5f;			
		float posx = Input.mousePosition.x;
		float posy = Input.mousePosition.y;
		if (posx > (Screen.width / 2)) 
			posx -= 215;			
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
					posicionBloqueSeleccion = 40;break;
				case taspectRatio.aspectRatio16_10:
					posicionBloqueSeleccion = 45;break;
				case taspectRatio.aspectRatio4_3:
					posicionBloqueSeleccion = 55;break;
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
					posicionBloqueInformacion = 44;break;
				case taspectRatio.aspectRatio16_10:
					posicionBloqueInformacion = 49;break;
				case taspectRatio.aspectRatio4_3:
					posicionBloqueInformacion = 59;break;
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
