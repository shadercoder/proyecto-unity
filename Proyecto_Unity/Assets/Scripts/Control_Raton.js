var target : Transform;
var distance = 10.0;

var disable : boolean = false;
// ZoomCameraMouse
var MouseWheelSensitivity = 5;
var MouseZoomMin = 1;
var MouseZoomMax = 7;

var xSpeed = 250.0;
var ySpeed = 120.0;

var yMinLimit = -20;
var yMaxLimit = 80;

private var x = 0.0;
private var y = 0.0;

var smoothTime = 0.3;

private var xSmooth = 0.0;
private var ySmooth = 0.0; 
private var xVelocity = 0.0;
private var yVelocity = 0.0;

private var posVelocity = Vector3.zero;

//Raycast para la parte de pulsar y centrar
private var hit : RaycastHit;
private var temporizador : float = 0.0;
private var rotacionClick : Quaternion = Quaternion.identity;
private var rotacionObjetivo : Quaternion = Quaternion.identity;

//Estados de la camara
private var estado : int = 0;				//0 para orbita normal, 1 para pulsar&centrar
private var interaccion : boolean = true;	//Si el ratón puede interactuar con el mundo o no

@script AddComponentMenu("Camera-Control/Mouse Orbit smoothed")

function Start () {
   
//    Screen.showCursor = false;

    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;

    // Make the rigid body not change rotation
//    if (rigidbody)
//        rigidbody.freezeRotation = true;
}

function LateUpdate () {
	
	var position : Vector3;
	//Si el estado es 2, no permitir ningún movimiento
	if (!interaccion) {
		return;
	}
	
	if (disable) {
		var dirTemp : Vector3 = target.position - Camera.main.transform.position;
		transform.rotation = Quaternion.LookRotation(dirTemp);
		return;
	}
	
	//clic y centrar camara a la distancia actual en direccion al origen de la esfera pasando por el punto señalado:
	if(Input.GetMouseButtonUp(0) && estado == 1){
		if (temporizador < Time.time) {
			var ray : Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray, hit, Mathf.Infinity) ) {
				var direccion : Vector3 = target.position - hit.point;
				rotacionClick = Quaternion.LookRotation(direccion);
				rotacionObjetivo = rotacionClick;
				temporizador = Time.time + 0.5;
			}
		}
	}    
   
    //Al pinchar con el boton derecho, resetear las posiciones para que no haya saltos bruscos
    if (target && Input.GetMouseButtonDown(1) && estado == 0) {
    	y = transform.rotation.eulerAngles.x;
    	x = transform.rotation.eulerAngles.y;
    	xSmooth = x;
    	ySmooth = y;
    }
	//mouseorbit activado, desplazamiento onDrag
    if (target && Input.GetMouseButton(1) && estado == 0) {
	    x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
	    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
	    xSmooth = Mathf.SmoothDamp(xSmooth, x, xVelocity, smoothTime);
	    ySmooth = Mathf.SmoothDamp(ySmooth, y, yVelocity, smoothTime);
	    ySmooth = ClampAngle(ySmooth, yMinLimit, yMaxLimit);
        var rotation = Quaternion.Euler(ySmooth, xSmooth, 0);
        transform.rotation = rotation;
        transform.position = rotation * Vector3(0.0, 0.0, -distance) + target.position;
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
	position = rotation * Vector3(0.0, 0.0, -distance) + target.position;

	transform.rotation = rotation;
	transform.position = position;
   	
}

static function ClampAngle (angle : float, min : float, max : float) {
    if (angle < -360)
        angle += 360;
    if (angle > 360)
        angle -= 360;
    return Mathf.Clamp (angle, min, max);
}

function cambiarTarget (objetivo : Transform) {
	this.target = objetivo;
	this.distance = 20.0;
}

//Estados en los que puede encontrarse el script de control: 0 para arrastre y 1 para click & go
function cambiarEstado(est : int) {
	if (est >= 0 && est <= 1)
		this.estado = est;
}

function setInteraccion(bol : boolean) {
	interaccion = bol;
}