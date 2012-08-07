using UnityEngine;
using System.Collections;

public class ControlesMejorados : MonoBehaviour {
	
	//Variables usadas por velocidad (caché)
	private Transform miTransform;							//Cache de la posicion de la cámara
	
	//Variables públicas (editables desde la vista del editor)
	public Transform objetivo;								//El objetivo sobre el que se mueve la nave
	public Transform nave;									//La nave sobre la que rota la vista
	
	//Posición y rotación objetivos
	private float xObjetivo			= 0.0f;					//Posicion objetivo de la x respecto al objetivo
	private float yObjetivo			= 0.0f;					//Posicion objetivo de la y respecto al objetivo
	
	//Posición y rotacion actuales
	private float distanciaNave		= 5.0f;					//La distancia hasta la nave desde la camara
	private int distanciaMin		= 1;					//La minima distancia a la que estara la camara
	private int distanciaMax		= 30;					//La maxima distancia a la que estara la camara
	
	private Quaternion rotCamara	= Quaternion.identity;	//Sirve para conservar la rotación de la nave correctamente
	
	//Variables privadas y estados
	private bool interaccion		= true;					//Dicta si la interacción está activada o desactivada
	
	
	// Use this for initialization
	void Start () {
		miTransform = transform;
	    Vector3 angulos = miTransform.eulerAngles;
		xObjetivo = angulos.y;
		yObjetivo = angulos.x;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		if (!interaccion)
			return;
		
		//Eje horizontal -----------------------------------------------------------------------------------------------------------------
		float xNaveTemp = 0;
		//Ecuación de la circunferencia
		xNaveTemp += Input.GetAxis("Horizontal");
		if (xNaveTemp != 0) {
			nave.RotateAround(objetivo.position, nave.up, xNaveTemp);
			
		}
		/*
		 * fwd' = fwd cos a + up sen a
		 * up' = fwd sen a + up cos a
		 * */
		
		// ------------------------------------------------------------------------------------------------------------------------------
		
		//Eje vertical -------------------------------------------------------------------------------------------------------------------
		//Con esto conseguimos que cuando soltemos el boton haga el suavizado y pare, no siga
		float yNaveTemp = 0;
		yNaveTemp += Input.GetAxis("Vertical");
		if (yNaveTemp != 0) {
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
	
	public void setInteraccion(bool bol) {
		interaccion = bol;
	}
}
