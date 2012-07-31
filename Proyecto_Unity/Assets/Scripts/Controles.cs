using UnityEngine;
using System.Collections;

public class Controles : MonoBehaviour {
	
	//Variables usadas por velocidad (caché)
	private Transform miTransform;				//Cache de la posicion de la cámara
	
	//Variables públicas (editables desde la vista del editor)
	public Transform objetivo;								//El objetivo sobre el que se mueve la nave
	public Transform nave;									//La nave sobre la que rota la vista
	public float tiempoSuavizado 	= 0.4f;					//El tiempo que tarda el suavizado en llevarse a cabo
	
	//Posición y rotación objetivos
	private float xNave				= 0.0f;					//Posicion objetivo de la x respecto a la nave
	private float yNave				= 0.0f;					//Posicion objetivo de la y respecto a la nave
	private float xObjetivo			= 0.0f;					//Posicion objetivo de la x respecto al objetivo
	private float yObjetivo			= 0.0f;					//Posicion objetivo de la y respecto al objetivo
	
	//Posición y rotacion actuales
	private float distanciaNave		= 5.0f;					//La distancia hasta la nave desde la camara
	private float distanciaObjetivo = 7.0f;					//La distancia entre la nave y el objetivo
	private int distanciaMin		= 1;					//La minima distancia a la que estara la camara
	private int distanciaMax		= 30;					//La maxima distancia a la que estara la camara
	
	private Quaternion rotCamara	= Quaternion.identity;	//Sirve para conservar la rotación de la nave correctamente
	
	//Suavizado del movimiento
	private float velocidadX 		= 0.0f;					//Es cambiada por el método SmoothDamp de forma dinámica
	private float velocidadY 		= 0.0f;					//Es cambiada por el método SmoothDamp de forma dinámica
	private float xSuave 			= 0.0f;					//Dicta el movimiento con una trayectoria suavizada
	private float ySuave 			= 0.0f;					//Dicta el movimiento con una trayectoria suavizada
	public float velocidadMax		= 25.0f;				//La velocidad máxima a la que puede moverse la nave
	
	//Variables privadas y estados
	private bool interaccion		= true;					//Dicta si la interacción está activada o desactivada
	
	
	// Use this for initialization
	void Start () {
		miTransform = transform;
	    Vector3 angulos = miTransform.eulerAngles;
	    xNave = angulos.y;
	    yNave = angulos.x;
		xObjetivo = angulos.y;
		yObjetivo = angulos.x;
		xSuave = xNave;
		ySuave = yNave;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
//		Quaternion rotacionObjeto = miTransform.rotation;
		Quaternion rotacionNave;
		
		if (!interaccion)
			return;
		
		//Eje horizontal -----------------------------------------------------------------------------------------------------------------
		//Con esto conseguimos que cuando soltemos el boton haga el suavizado y pare, no siga
		float xNaveTemp = xNave; 					
		xNaveTemp += Input.GetAxis("Horizontal");
		if (Mathf.Abs(xNaveTemp - xSuave) < (velocidadMax * tiempoSuavizado * 1.2f))
			xNave = xNaveTemp;
		
		//Seguridad para que no siga avanzando cuando soltemos
		if ((Input.GetAxis("Horizontal") == 0.0f) && (Mathf.Abs(xNaveTemp - xSuave) > (velocidadMax * tiempoSuavizado * 1.2f))) {
			if (xNaveTemp > xSuave)
				xNave = xSuave + velocidadMax * tiempoSuavizado * 1.2f;
			else
				xNave = xSuave - velocidadMax * tiempoSuavizado * 1.2f;
		}// ------------------------------------------------------------------------------------------------------------------------------
		
		//Eje vertical -------------------------------------------------------------------------------------------------------------------
		//Con esto conseguimos que cuando soltemos el boton haga el suavizado y pare, no siga
		float yNaveTemp = yNave;
		yNaveTemp += Input.GetAxis("Vertical");
		if (Mathf.Abs(yNaveTemp - ySuave) < (velocidadMax * tiempoSuavizado * 1.2f))
			yNave = yNaveTemp;
		
		//Seguridad para que no siga avanzando cuando soltemos
		if ((Input.GetAxis("Vertical") == 0.0f) && (Mathf.Abs(yNaveTemp - ySuave) > (velocidadMax * tiempoSuavizado * 1.2f))) {
			if (yNaveTemp > ySuave)
				yNave = ySuave + velocidadMax * tiempoSuavizado * 1.2f;
			else
				yNave = ySuave - velocidadMax * tiempoSuavizado * 1.2f;
		}// ------------------------------------------------------------------------------------------------------------------------------
		
		//Y con esto suavizamos el movimiento con respecto a un máximo.
		xSuave = Mathf.SmoothDamp(xSuave, xNave, ref velocidadX, tiempoSuavizado, velocidadMax);
		ySuave = Mathf.SmoothDamp(ySuave, yNave, ref velocidadY, tiempoSuavizado, velocidadMax);
		ySuave = anguloSeguro(ySuave);
		xSuave = anguloSeguro(xSuave);
		
		//Obtenemos la rotación...
		rotacionNave = Quaternion.Euler(ySuave, xSuave, 0);
		//Y calculamos la posición a raiz de esta
		nave.position = rotacionNave * new Vector3(0.0f, 0.0f, -distanciaObjetivo) + objetivo.position;
		//Rotar la nave para que vaya moviéndose en la dirección
		//en la que avanza.
		nave.LookAt(objetivo.position);
		//Si se pulsa el botón derecho del ratón, se rota en torno a la nave
		if (Input.GetMouseButton(1)) {
			xObjetivo += Input.GetAxis("Mouse X");
		    yObjetivo += Input.GetAxis("Mouse Y");
			rotCamara = Quaternion.Euler(yObjetivo, xObjetivo, 0);
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
		    if (distanciaNave >= distanciaMin && distanciaNave <= distanciaMax){
				distanciaNave -= Input.GetAxis("Mouse ScrollWheel");		
				if (distanciaNave < distanciaMin) {
					distanciaNave = distanciaMin;
				}
			    if (distanciaNave > distanciaMax) {
					distanciaNave = distanciaMax;
				}
		   	}
	   	}
		
		//Se aplica la rotación y la posición de la cámara respecto a la nave.
		miTransform.rotation = rotCamara;
		miTransform.position = rotCamara * new Vector3(0.0f, 0.0f, -distanciaNave) + nave.position;
	}
	
	public static float anguloSeguro(float angulo) {
	    float ang = angulo;
		while (ang < -360) {
	        ang += 360;
		}
	    while (ang >= 360) {
	        ang -= 360;
		}
		return ang;
	}
	
	public void setInteraccion(bool bol) {
		interaccion = bol;
	}
}
