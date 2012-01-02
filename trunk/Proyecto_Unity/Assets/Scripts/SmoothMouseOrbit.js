var target : Transform;
var distance = 10.0;

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

//private var xPin = 0.0;
//private var yPin = 0.0;

var smoothTime = 0.3;

private var xSmooth = 0.0;
private var ySmooth = 0.0; 
private var xVelocity = 0.0;
private var yVelocity = 0.0;

//private var xSmoothPin = 0.0;
//private var ySmoothPin = 0.0; 
//private var xVelocityPin = 0.0;
//private var yVelocityPin = 0.0;

private var posSmooth = Vector3.zero;
private var posVelocity = Vector3.zero;

//private var posSmoothPin = Vector3.zero;
//private var posVelocityPin = Vector3.zero;

//private var pinTarget = false;


@script AddComponentMenu("Camera-Control/Mouse Orbit smoothed")

function Start () {
   
//    Screen.showCursor = false;

    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;

    // Make the rigid body not change rotation
    if (rigidbody)
        rigidbody.freezeRotation = true;
}

function LateUpdate () {

    if (target) {
        x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;

        xSmooth = Mathf.SmoothDamp(xSmooth, x, xVelocity, smoothTime);
        ySmooth = Mathf.SmoothDamp(ySmooth, y, yVelocity, smoothTime);

        ySmooth = ClampAngle(ySmooth, yMinLimit, yMaxLimit);

        var rotation = Quaternion.Euler(ySmooth, xSmooth, 0);

       // posSmooth = Vector3.SmoothDamp(posSmooth,target.position,posVelocity,smoothTime);

        posSmooth = target.position; // no follow smoothing
		
//		if (pinTarget) {
//			var rotaScript : Rotacion = target.GetComponent(Rotacion);
//			
//			xPin += rotaScript.eje.x * xSpeed * 0.02;
//	        yPin -= rotaScript.eje.y * ySpeed * 0.02;
//	
//	        xSmoothPin = Mathf.SmoothDamp(xSmoothPin, xPin, xVelocityPin, smoothTime);
//	        ySmoothPin = Mathf.SmoothDamp(ySmoothPin, yPin, yVelocityPin, smoothTime);
//	
//	        ySmoothPin = ClampAngle(ySmoothPin, yMinLimit, yMaxLimit);
//			
//			var rotacionInv : Quaternion = Quaternion.Euler(ySmoothPin, xSmoothPin, 0); 
//			rotacionInv = Quaternion.Inverse(rotacionInv);
//			rotation = rotation * rotacionInv;
//		}
        transform.rotation = rotation;
        transform.position = rotation * Vector3(0.0, 0.0, -distance) + posSmooth;
    }
    
	if (Input.GetAxis("Mouse ScrollWheel") != 0) {

    //Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
    //Debug.Log(distance);
    if (distance >= MouseZoomMin && distance <= MouseZoomMax){


	distance -= Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity;
	
	if (distance < MouseZoomMin){distance = MouseZoomMin;}
    if (distance > MouseZoomMax){distance = MouseZoomMax;}
   }
   }	
   
   rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime * 3);
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

function cambiarTarget (objetivo : Transform, pin : boolean) {
	this.target = objetivo;
	this.distance = 20.0;
//	this.pinTarget = pin;
}
