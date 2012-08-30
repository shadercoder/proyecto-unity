#pragma strict

var target : Transform;
var speed : float;
var orbitDir : Vector3;
var direction : Vector3;

private var orbitingBody : Transform;

function Awake(){
	orbitingBody = transform;
} 

// Use this for initialization
function Start() {
	var line : Vector3 = target.position - transform.position;
	line.Normalize();
	Vector3.OrthoNormalize(line, direction, orbitDir); 
}

function FixedUpdate () {
	transform.RotateAround (target.position, orbitDir, speed * Time.deltaTime);
}
