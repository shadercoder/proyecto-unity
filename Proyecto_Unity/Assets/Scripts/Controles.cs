using UnityEngine;
using System.Collections;

public class Controles : MonoBehaviour {
	
	//Variables usadas por velocidad (caché) -------------------------
	private Transform miTransform;							//Cache de la posicion de la cámara
	private Vector3 poloNorteOrbita;						//El punto proyectado a partir del polo norte que choca con la orbita actual 
	private Vector3 poloSurOrbita;							//El punto proyectado a partir del polo sur que choca con la orbita actual 
	
	//Variables públicas (editables desde la vista del editor) --------
	public Transform objetivo;								//El objetivo sobre el que se mueve la nave
	public Transform nave;									//La nave sobre la que rota la vista
		//Para controlar las mejoras
	public float velocidadX			= 0.3f;					//Controla la velocidad con la que se mueve horizontalmente
	public float velocidadY			= 0.4f;					//Controla la velocidad con la que se mueve verticalmente
	public float distMinPolos		= 1.0f;					//La distancia minima hasta los polos
	public float distCamaraMax		= 6.0f;					//La maxima distancia a la que estara la camara de la nave
	
	/*
	 * Despues de experimentar con estos valores un poco, creo que un buen comienzo (el inicio del juego)
	 * seria el siguiente:
	 	* velocidadX = 0.1f;
	 	* velocidadY = 0.2f;
	 	* distMinPolos = 5.0f;
	 	* distCamaraMax = 2.0f;
	 */
	
	//Variables privadas ----------------------------------------------
	//Posición y rotación objetivos
	private float xObjetivo			= 0.0f;					//Posicion objetivo de la x respecto al objetivo
	private float yObjetivo			= 0.0f;					//Posicion objetivo de la y respecto al objetivo
	
	private float distOrbitaTemp;							//Usado solamente para subir de orbita o bajar
	
	//Posición y rotacion actuales
	private float distanciaNave		= 1.5f;					//La distancia hasta la nave desde la camara
	private float distCamaraMin		= 1.0f;					//La minima distancia a la que estara la camara
	
	private Quaternion rotCamara	= Quaternion.identity;	//Sirve para conservar la rotación de la nave correctamente
	
	//Estados
	private bool interaccion		= true;					//Dicta si la interacción está activada o desactivada
	
	
	// Use this for initialization
	void Start () {
		miTransform = transform;
	    Vector3 angulos = miTransform.eulerAngles;
		xObjetivo = angulos.y;
		yObjetivo = angulos.x;
		setOrbita(6.5f);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		if (!interaccion)
			return;
		
		//Eje horizontal -----------------------------------------------------------------------------------------------------------------
		float xNaveTemp = 0;
		//Ecuación de la circunferencia
		xNaveTemp += Input.GetAxis("Horizontal") * velocidadX;
		if (xNaveTemp != 0) {
			nave.RotateAround(objetivo.position, nave.up, xNaveTemp);
		}
		
		// ------------------------------------------------------------------------------------------------------------------------------
		
		//Eje vertical -------------------------------------------------------------------------------------------------------------------
		//Con esto conseguimos que cuando soltemos el boton haga el suavizado y pare, no siga
		float yNaveTemp = 0;
		yNaveTemp += Input.GetAxis("Vertical") * velocidadY;
		if (yNaveTemp != 0) {
			if (Vector3.Distance(nave.position, poloSurOrbita) > distMinPolos && Vector3.Distance(nave.position, poloNorteOrbita) > distMinPolos)
				nave.RotateAround(objetivo.position, nave.right, yNaveTemp);
			else if((Vector3.Distance(nave.position, poloSurOrbita) <= distMinPolos && yNaveTemp > 0) || (Vector3.Distance(nave.position, poloNorteOrbita) <= distMinPolos && yNaveTemp < 0))
				nave.RotateAround(objetivo.position, nave.right, yNaveTemp);
		}
		
		// ------------------------------------------------------------------------------------------------------------------------------
		
		
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
		    if (distanciaNave >= distCamaraMin && distanciaNave <= distCamaraMax){
				distanciaNave -= Input.GetAxis("Mouse ScrollWheel");		
				if (distanciaNave < distCamaraMin) {
					distanciaNave = distCamaraMin;
				}
			    if (distanciaNave > distCamaraMax) {
					distanciaNave = distCamaraMax;
				}
		   	}
	   	}
		
		//Se aplica la rotación y la posición de la cámara respecto a la nave.
		miTransform.rotation = rotCamara;
		miTransform.position = rotCamara * new Vector3(0.0f, 0.0f, -distanciaNave) + nave.position;
	}
	
	public void setOrbita(float distancia) {
		nave.position = nave.rotation * new Vector3(0.0f, 0.0f, -distancia) + objetivo.position;
		float dist = Vector3.Distance(nave.position, objetivo.position);
		poloNorteOrbita = Vector3.up * dist + objetivo.position;
		poloSurOrbita = Vector3.up * (-dist) + objetivo.position;
	}
	
	public void setInteraccion(bool bol) {
		interaccion = bol;
	}
	
	public void mejoraVelocidad1() {
		velocidadX = 0.4f;
		velocidadY = 0.5f;
	}
	
	public void mejoraVelocidad2() {
		velocidadX = 0.8f;
		velocidadY = 1.0f;
	}
	
	public void mejoraSubirOrbita() {
		setOrbita(8.5f);
		distMinPolos = 1.0f;
	}
}
