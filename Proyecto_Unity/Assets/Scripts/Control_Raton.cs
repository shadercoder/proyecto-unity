using UnityEngine;
using System.Collections;

public class Control_Raton : MonoBehaviour {

	public Transform target;
	public float distance = 10.0f;
	
	public bool disable = false;
	// ZoomCameraMouse
	public int MouseWheelSensitivity = 5;
	public int MouseZoomMin = 1;
	public int MouseZoomMax = 7;
	
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	
	public int yMinLimit = -20;
	public int yMaxLimit = 80;
	
	private float x = 0.0f;
	private float y = 0.0f;
	
	public float smoothTime = 0.3f;
	
	private float xSmooth = 0.0f;
	private float ySmooth = 0.0f; 
	private float xVelocity = 0.0f;
	private float yVelocity = 0.0f;
		
	//Raycast para la parte de pulsar y centrar
	private RaycastHit hit;
	private Quaternion rotacionObjetivo = Quaternion.identity;
	
	//Estados de la camara
	private int estado = 0;				//0 para orbita normal, 1 para pulsar&centrar
	private bool interaccion = true;	//Si el ratón puede interactuar con el mundo o no
	private bool llamaCorutinaPincel = false;
	
	
	void Start () {	
	    Vector3 angles = transform.eulerAngles;
	    x = angles.y;
	    y = angles.x;
	}
	
	void LateUpdate () {
		
		Vector3 position;
		Quaternion rotation;
		
	//Esta parte es de control y debe estar al principio --------------------------------------------------
		
		//Si el estado es 2, no permitir ningún movimiento
		if (!interaccion) {
			return;
		}
		
		if (disable) {
			Vector3 dirTemp = target.position - Camera.main.transform.position;
			transform.rotation = Quaternion.LookRotation(dirTemp);
			return;
		}
	//Hasta aqui la parte de control inicial --------------------------------------------------------------		
		
		//clic y centrar camara a la distancia actual en direccion al origen de la esfera pasando por el punto señalado:
//		if(Input.GetMouseButtonUp(0) && estado == 1){
//			if (temporizador < Time.time) {
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//				if (Physics.Raycast(ray, out hit, Mathf.Infinity) ) {
//					Vector3 direccion = target.position - hit.point;
//					rotacionClick = Quaternion.LookRotation(direccion);
//					rotacionObjetivo = rotacionClick;
//					temporizador = Time.time + 0.5f;
//				}
//			}
//		}    
	   
	//Esta parte es para el click & drag con el boton derecho --------------------------------------------
		
	    //Al pinchar con el boton derecho, resetear las posiciones para que no haya saltos bruscos
	    if (target && Input.GetMouseButtonDown(1) && estado == 0) {
	    	y = transform.rotation.eulerAngles.x;
	    	x = transform.rotation.eulerAngles.y;
	    	xSmooth = x;
	    	ySmooth = y;
	    }
		
		//mouseorbit activado, desplazamiento onDrag
	    if (target && Input.GetMouseButton(1) && estado == 0) {
		    x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
		    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
		    xSmooth = Mathf.SmoothDamp(xSmooth, x, ref xVelocity, smoothTime);
		    ySmooth = Mathf.SmoothDamp(ySmooth, y, ref yVelocity, smoothTime);
		    ySmooth = ClampAngle(ySmooth, yMinLimit, yMaxLimit);
	        rotation = Quaternion.Euler(ySmooth, xSmooth, 0);
	        transform.rotation = rotation;
	        transform.position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
	        rotacionObjetivo = Quaternion.Euler(y, x, 0);
	    }
	    
	    //Controlar la distancia al objeto con la rueda del raton
		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
		    if (distance >= MouseZoomMin && distance <= MouseZoomMax){
				distance -= Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity;		
				if (distance < MouseZoomMin){distance = MouseZoomMin;}
			    if (distance > MouseZoomMax){distance = MouseZoomMax;}
		   	}
	   	}	
	   	
		rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 3);
		position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
	
		transform.rotation = rotation;
		transform.position = position;
	//Hasta aqui el click & drag con el boton derecho -------------------------------------------------------
	   	
	}
	
	public static float ClampAngle(float angle, float min, float max) {
	    if (angle < -360)
	        angle += 360;
	    if (angle > 360)
	        angle -= 360;
	    return Mathf.Clamp(angle, min, max);
	}
	
	public void cambiarTarget (Transform objetivo) {
		this.target = objetivo;
		this.distance = 20.0f;
	}
	
	//Estados en los que puede encontrarse el script de control: 0 para arrastre y 1 para click & go
//	public void cambiarEstado(int est) {
//		if (est >= 0 && est <= 1)
//			this.estado = est;
//	}
	
	public void setInteraccion(bool bol) {
		interaccion = bol;
	}
}
