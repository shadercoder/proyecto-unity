using UnityEngine;
using System.Collections;

public class InterfazIngame : MonoBehaviour {

	// Variables ---------------------------------------------------------------------------------------------------------------------------
	public GUISkin estilo;
	private float cuantoW;				//Minima unidad de medida de la interfaz a lo ancho
	private float cuantoH;				//Minima unidad de medida de la interfaz a lo alto
	private float aspectRatio;			//Aspect ratio de la ventana
	private bool mostrarBloqueIzquierdo = true;
		
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUI.skin = estilo;		
		aspectRatio = (float)Screen.width/(float)Screen.height;
		if(aspectRatio >= 1.69)
			interfaz16_9();
		else if	(aspectRatio >= 1.47)
			interfaz16_10();
		else
			interfaz4_3();
	}
	
	// Interfaz para aspect ratio 16:10
	void interfaz16_10()
	{
		int temp = 0;		
		cuantoW	= (float)Screen.width / 80;
		cuantoH	= (float)Screen.height / 50;
		
		Estados estados = gameObject.GetComponent<Estados>();		
		//bloque superior
		GUI.BeginGroup(new Rect(cuantoW*0,cuantoH*0,cuantoW*80,cuantoH*3));
		GUI.Box(new Rect(cuantoW*0,cuantoH*0,cuantoW*73,cuantoH*3),"","BloqueSuperior");		
		GUI.Label(new Rect(cuantoW*2,cuantoH*0,cuantoW*6,cuantoH*2),estados.numPasos.ToString(),"EtiquetaTiempo");
		if(GUI.Button(new Rect(cuantoW*4,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Pausa el juego"),"BotonPausa"))
			estados.escalaTiempo = 0;	
		if(GUI.Button(new Rect(cuantoW*5,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad normal"),"BotonVelocidad1"))
			estados.escalaTiempo = 1;	
		if(GUI.Button(new Rect(cuantoW*6,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 2x"),"BotonVelocidad2"))
			estados.escalaTiempo = 2;	
		if(GUI.Button(new Rect(cuantoW*7,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 5x"),"BotonVelocidad5"))
			estados.escalaTiempo = 5;	
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
			estados.escalaTiempo = 0;	
		if(GUI.Button(new Rect(cuantoW*5,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad normal"),"BotonVelocidad1"))
			estados.escalaTiempo = 1;	
		if(GUI.Button(new Rect(cuantoW*6,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 2x"),"BotonVelocidad2"))
			estados.escalaTiempo = 2;	
		if(GUI.Button(new Rect(cuantoW*7,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 5x"),"BotonVelocidad5"))
			estados.escalaTiempo = 5;	
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
			estados.escalaTiempo = 0;	
		if(GUI.Button(new Rect(cuantoW*5,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad normal"),"BotonVelocidad1"))
			estados.escalaTiempo = 1;	
		if(GUI.Button(new Rect(cuantoW*6,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 2x"),"BotonVelocidad2"))
			estados.escalaTiempo = 2;	
		if(GUI.Button(new Rect(cuantoW*7,cuantoH*2,cuantoW*1,cuantoH*1),new GUIContent("","Velocidad 5x"),"BotonVelocidad5"))
			estados.escalaTiempo = 5;	
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
	}
	
}
