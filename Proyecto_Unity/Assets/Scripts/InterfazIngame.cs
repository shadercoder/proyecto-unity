using UnityEngine;
using System.Collections;

public class InterfazIngame : MonoBehaviour {

	// Variables ---------------------------------------------------------------------------------------------------------------------------
	public GUISkin estilo;
	private float cuantoW;							//Minima unidad de medida de la interfaz a lo ancho
	private float cuantoH;							//Minima unidad de medida de la interfaz a lo alto
	private float aspectRatioNumerico;				//Aspect ratio númerico de la ventana
	private Estados estados;						//Acceso a los datos principales	
	private bool mostrarBloqueIzquierdo = true;		//Visibilidad del bloque de opciones izquierdo
	private enum taspectRatio						//Aspecto ratio con el que se pintará la ventana. Si no es ninguno de ellos se aproximará al más cercano
		{aspectRatio16_9,aspectRatio16_10,aspectRatio4_3};
	private taspectRatio aspectRatio;
	private enum tinsercion							//Desactivado indica que no hay insercion en curso, otro valor indica la categoria de la insercion
		{desactivada,animal,vegetal,edificio}
	private tinsercion insercion = tinsercion.desactivada;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUI.skin = estilo;				
		estados = gameObject.GetComponent<Estados>();		
		aspectRatioNumerico = (float)Screen.width/(float)Screen.height;		
		
		if(aspectRatioNumerico >= 1.69)			//16:9
		{
			aspectRatio = taspectRatio.aspectRatio16_9;
			cuantoW	= (float)Screen.width / 80;
			cuantoH	= (float)Screen.height / 45;		
		}
		else if	(aspectRatioNumerico >= 1.47)	//16:10
		{
			aspectRatio = taspectRatio.aspectRatio16_10;
			cuantoW	= (float)Screen.width / 80;
			cuantoH	= (float)Screen.height / 50;	
		}
		else 							//4:3
		{
			aspectRatio = taspectRatio.aspectRatio4_3;			
			cuantoW	= (float)Screen.width / 80;
			cuantoH	= (float)Screen.height / 60;	
		}	
		bloqueSuperior();
		bloqueIzquierdo();
		bloqueSeleccion();
		bloqueInformacion();	
	}
	
	//Dibuja el bloque superior de la ventana que contiene: tiempo, control velocidad, conteo de recursos y menu principal
	void bloqueSuperior()
	{
		GUI.BeginGroup(new Rect(cuantoW*0,cuantoH*0,cuantoW*80,cuantoH*3));
		GUI.Box(new Rect(cuantoW*0,cuantoH*0,cuantoW*73,cuantoH*3),"","BloqueSuperior");		
		GUI.Label(new Rect(cuantoW*2,cuantoH*0,cuantoW*6,cuantoH*2),estados.numPasos.ToString(),"EtiquetaTiempo");
		if(GUI.Button(new Rect(cuantoW*4,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Pausa el juego"),"BotonPausa"))
			Time.timeScale = 0;	
		if(GUI.Button(new Rect(cuantoW*5,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad normal"),"BotonVelocidad1"))
			Time.timeScale = 1;	
		if(GUI.Button(new Rect(cuantoW*6,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 2x"),"BotonVelocidad2"))
			Time.timeScale = 2;	
		if(GUI.Button(new Rect(cuantoW*7,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 5x"),"BotonVelocidad5"))
			Time.timeScale = 5;	
		if(GUI.Button(new Rect(cuantoW*73,cuantoH*0,cuantoW*7,cuantoH*3),new GUIContent("","Accede al menu del juego"),"BotonMenu"))
			cuantoH = cuantoH;		
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
		
		GUILayout.BeginArea(new Rect(cuantoW*0,cuantoH*posicionBloque,cuantoW*3,cuantoH*10),new GUIContent(),"BloqueIzquierdo");		
		GUILayout.BeginHorizontal();
		if(mostrarBloqueIzquierdo)
		{
			GUILayout.BeginVertical(GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*2));			
			if(GUILayout.Button(new GUIContent("","Accede al menu de insertar animales o vegetales"),"BotonInsertarVida"))
				insercion = tinsercion.animal;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de construir edificios"),"BotonInsertarEdificios"))
				insercion = tinsercion.edificio;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de mejoras de la nave"),"BotonAccederMejoras"))
				cuantoH = cuantoH;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de habilidades"),"BotonAccederHabilidades"))
				cuantoH = cuantoH;		
			if(GUILayout.Button(new GUIContent("","Cambia entre info y seleccionar"),"BotonInfoSelec"))
				cuantoH = cuantoH;		
			GUILayout.EndVertical();
			if(GUILayout.Button(new GUIContent("","Pulsa para ocultar este menu"),"BotonOcultarBloqueIzquierdo",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
				mostrarBloqueIzquierdo = false;
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
		if(insercion == tinsercion.desactivada)
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
		switch (insercion)
		{
			case tinsercion.edificio:
				GUILayout.BeginArea(new Rect(cuantoW*32,cuantoH*posicionBloque,cuantoW*16,cuantoH*4),new GUIContent(),"BloqueSeleccionEdificios");
				GUILayout.BeginVertical();
				GUILayout.Space(cuantoH*1);				
				GUILayout.BeginHorizontal(GUILayout.Height(cuantoH*2));			
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Fabrica de componentes básicos"),"BotonInsertarFabComBas"))
					insercion = insercion;		
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Central de energía"),"BotonInsertarCenEn"))
					insercion = insercion;		
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Granja"),"BotonInsertarGranja"))
					insercion = insercion;		
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Fábrica de componentes avanzados"),"BotonInsertarFabComAdv"))
					insercion = insercion;		
				GUILayout.Space(cuantoW);
				if(GUILayout.Button(new GUIContent("","Central de energía avanzada"),"BotonInsertarCenEnAdv"))
					insercion = insercion;		
				GUILayout.Space(cuantoW);
				GUILayout.EndHorizontal();
				GUILayout.Space(cuantoH*1);				
				GUILayout.EndVertical();
				GUILayout.EndArea();
				break;
			case tinsercion.animal:
				break;
			case tinsercion.vegetal:
				break;
			default:break;
		}
		
	}
	
	//Dibuja el bloque información de la ventana que proporciona información
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
		GUI.Box(new Rect(cuantoW*0,cuantoH*posicionBloque,cuantoW*100,cuantoH*1),"","BloqueInformacion");
	}
	/*
	// Interfaz para aspect ratio 16:10
	void interfaz16_10()
	{
		int temp = 0;		
		cuantoW	= (float)Screen.width / 80;
		cuantoH	= (float)Screen.height / 50;
		
		//bloque superior
		GUI.BeginGroup(new Rect(cuantoW*0,cuantoH*0,cuantoW*80,cuantoH*3));
		GUI.Box(new Rect(cuantoW*0,cuantoH*0,cuantoW*73,cuantoH*3),"","BloqueSuperior");		
		GUI.Label(new Rect(cuantoW*2,cuantoH*0,cuantoW*6,cuantoH*2),estados.numPasos.ToString(),"EtiquetaTiempo");
		if(GUI.Button(new Rect(cuantoW*4,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Pausa el juego"),"BotonPausa"))
			Time.timeScale = 0;//estados.escalaTiempo = 0;	
		if(GUI.Button(new Rect(cuantoW*5,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad normal"),"BotonVelocidad1"))
			Time.timeScale = 1;//estados.escalaTiempo = 1;	
		if(GUI.Button(new Rect(cuantoW*6,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 2x"),"BotonVelocidad2"))
			Time.timeScale = 2;//estados.escalaTiempo = 2;	
		if(GUI.Button(new Rect(cuantoW*7,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 5x"),"BotonVelocidad5"))
			Time.timeScale = 5;//estados.escalaTiempo = 5;	
		if(GUI.Button(new Rect(cuantoW*73,cuantoH*0,cuantoW*7,cuantoH*3),new GUIContent("","Accede al menu del juego"),"BotonMenu"))
			temp++;		
		GUI.EndGroup();
		
		//bloque izquierdo
		GUILayout.BeginArea(new Rect(cuantoW*0,cuantoH*20,cuantoW*3,cuantoH*10),new GUIContent(),"BloqueIzquierdo");		
		GUILayout.BeginHorizontal();
		if(mostrarBloqueIzquierdo)
		{
			GUILayout.BeginVertical(GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*2));			
			if(GUILayout.Button(new GUIContent("","Accede al menu de insertar animales o vegetales"),"BotonInsertarVida"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de construir edificios"),"BotonInsertarEdificios"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de mejoras de la nave"),"BotonAccederMejoras"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de habilidades"),"BotonAccederHabilidades"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Cambia entre info y seleccionar"),"BotonInfoSelec"))
				temp++;					
			GUILayout.EndVertical();
			if(GUILayout.Button(new GUIContent("","Pulsa para ocultar este menu"),"BotonOcultarBloqueIzquierdo",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
				mostrarBloqueIzquierdo = false;
		}
		else
			if(GUILayout.Button(new GUIContent("","Pulsa para mostrar el menu de acciones"),"BotonOcultarBloqueIzquierdoActivado",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
				mostrarBloqueIzquierdo = true;	
			
		GUILayout.EndHorizontal();		
		GUILayout.EndArea();
		
		//Barra construccion
		GUI.BeginGroup(new Rect(cuantoW*22,cuantoH*45,cuantoW*36,cuantoH*4),"","BloqueConstruccion");
		GUI.EndGroup();
		
		//Barra construccion
		GUI.Box(new Rect(cuantoW*0,cuantoH*49,cuantoW*100,cuantoH*1),"","BloqueInferior");	
	}
	
	// Interfaz para aspect ratio 16:9
	void interfaz16_9()
	{
		int temp = 0;		
		cuantoW	= (float)Screen.width / 80;
		cuantoH	= (float)Screen.height / 45;
		
		Estados estados = gameObject.GetComponent<Estados>();		
		//bloque superior
		GUI.BeginGroup(new Rect(cuantoW*0,cuantoH*0,cuantoW*80,cuantoH*3));
		GUI.Box(new Rect(cuantoW*0,cuantoH*0,cuantoW*73,cuantoH*3),"","BloqueSuperior");		
		GUI.Label(new Rect(cuantoW*2,cuantoH*0,cuantoW*6,cuantoH*2),estados.numPasos.ToString(),"EtiquetaTiempo");
		if(GUI.Button(new Rect(cuantoW*4,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Pausa el juego"),"BotonPausa"))
			Time.timeScale = 0;//estados.escalaTiempo = 0;	
		if(GUI.Button(new Rect(cuantoW*5,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad normal"),"BotonVelocidad1"))
			Time.timeScale = 1;//estados.escalaTiempo = 1;	
		if(GUI.Button(new Rect(cuantoW*6,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 2x"),"BotonVelocidad2"))
			Time.timeScale = 2;//estados.escalaTiempo = 2;	
		if(GUI.Button(new Rect(cuantoW*7,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 5x"),"BotonVelocidad5"))
			Time.timeScale = 5;//estados.escalaTiempo = 5;	
		if(GUI.Button(new Rect(cuantoW*73,cuantoH*0,cuantoW*7,cuantoH*3),new GUIContent("","Accede al menu del juego"),"BotonMenu"))
			temp++;		
		GUI.EndGroup();
		
		//bloque izquierdo
		GUILayout.BeginArea(new Rect(cuantoW*0,cuantoH*17,cuantoW*3,cuantoH*10),new GUIContent(),"BloqueIzquierdo");		
		GUILayout.BeginHorizontal();
		if(mostrarBloqueIzquierdo)
		{
			GUILayout.BeginVertical(GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*2));			
			if(GUILayout.Button(new GUIContent("","Accede al menu de insertar animales o vegetales"),"BotonInsertarVida"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de construir edificios"),"BotonInsertarEdificios"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de mejoras de la nave"),"BotonAccederMejoras"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de habilidades"),"BotonAccederHabilidades"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Cambia entre info y seleccionar"),"BotonInfoSelec"))
				temp++;					
			GUILayout.EndVertical();
			if(GUILayout.Button(new GUIContent("","Pulsa para ocultar este menu"),"BotonOcultarBloqueIzquierdo",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
				mostrarBloqueIzquierdo = false;
		}
		else
			if(GUILayout.Button(new GUIContent("","Pulsa para mostrar el menu de acciones"),"BotonOcultarBloqueIzquierdoActivado",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
				mostrarBloqueIzquierdo = true;	
			
		GUILayout.EndHorizontal();		
		GUILayout.EndArea();
		
		//Barra construccion
		GUI.BeginGroup(new Rect(cuantoW*22,cuantoH*40,cuantoW*36,cuantoH*4),"","BloqueConstruccion");
		GUI.EndGroup();
		
		//Barra construccion
		GUI.Box(new Rect(cuantoW*0,cuantoH*44,cuantoW*100,cuantoH*1),"","BloqueInferior");		
	}
	
	// Interfaz para aspect ratio 4:3
	void interfaz4_3()
	{
		int temp = 0;		
		cuantoW	= (float)Screen.width / 80;
		cuantoH	= (float)Screen.height / 60;
		
		Estados estados = gameObject.GetComponent<Estados>();		
		//bloque superior
		GUI.BeginGroup(new Rect(cuantoW*0,cuantoH*0,cuantoW*80,cuantoH*3));
		GUI.Box(new Rect(cuantoW*0,cuantoH*0,cuantoW*73,cuantoH*3),"","BloqueSuperior");		
		GUI.Label(new Rect(cuantoW*2,cuantoH*0,cuantoW*6,cuantoH*2),estados.numPasos.ToString(),"EtiquetaTiempo");
		if(GUI.Button(new Rect(cuantoW*4,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Pausa el juego"),"BotonPausa"))
			Time.timeScale = 0;//estados.escalaTiempo = 0;	
		if(GUI.Button(new Rect(cuantoW*5,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad normal"),"BotonVelocidad1"))
			Time.timeScale = 1;//estados.escalaTiempo = 1;	
		if(GUI.Button(new Rect(cuantoW*6,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 2x"),"BotonVelocidad2"))
			Time.timeScale = 2;//estados.escalaTiempo = 2;	
		if(GUI.Button(new Rect(cuantoW*7,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 5x"),"BotonVelocidad5"))
			Time.timeScale = 5;//estados.escalaTiempo = 5;	
		if(GUI.Button(new Rect(cuantoW*73,cuantoH*0,cuantoW*7,cuantoH*3),new GUIContent("","Accede al menu del juego"),"BotonMenu"))
			temp++;		
		GUI.EndGroup();
		
		//bloque izquierdo
		GUILayout.BeginArea(new Rect(cuantoW*0,cuantoH*25,cuantoW*3,cuantoH*10),new GUIContent(),"BloqueIzquierdo");		
		GUILayout.BeginHorizontal();
		if(mostrarBloqueIzquierdo)
		{
			GUILayout.BeginVertical(GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*2));			
			if(GUILayout.Button(new GUIContent("","Accede al menu de insertar animales o vegetales"),"BotonInsertarVida"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de construir edificios"),"BotonInsertarEdificios"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de mejoras de la nave"),"BotonAccederMejoras"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Accede al menu de habilidades"),"BotonAccederHabilidades"))
				temp++;		
			if(GUILayout.Button(new GUIContent("","Cambia entre info y seleccionar"),"BotonInfoSelec"))
				temp++;					
			GUILayout.EndVertical();
			if(GUILayout.Button(new GUIContent("","Pulsa para ocultar este menu"),"BotonOcultarBloqueIzquierdo",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
				mostrarBloqueIzquierdo = false;
		}
		else
			if(GUILayout.Button(new GUIContent("","Pulsa para mostrar el menu de acciones"),"BotonOcultarBloqueIzquierdoActivado",GUILayout.Height(cuantoH*10),GUILayout.Width(cuantoH*1)))
				mostrarBloqueIzquierdo = true;	
			
		GUILayout.EndHorizontal();		
		GUILayout.EndArea();
		
		//Barra construccion
		GUI.BeginGroup(new Rect(cuantoW*22,cuantoH*55,cuantoW*36,cuantoH*4),"","BloqueConstruccion");
		GUI.EndGroup();
		
		//Barra construccion
		GUI.Box(new Rect(cuantoW*0,cuantoH*59,cuantoW*100,cuantoH*1),"","BloqueInferior");
	}*/
	
}
