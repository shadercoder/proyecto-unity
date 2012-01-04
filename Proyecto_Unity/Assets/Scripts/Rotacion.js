#pragma strict

var velocidad : float;
var eje : Vector3;

private var target : Transform;
	
// Use this for initialization
function Start() {
	target = transform;
}
	
// Update is called once per frame
function Update() {
	// rotar en el eje a velocidad
	target.Rotate(eje * velocidad * Time.deltaTime);
}
