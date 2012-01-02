#pragma strict

var target : Transform;
var velocidad : float;
var eje : Vector3;
	
// Use this for initialization
function Start() {
	target = transform;
}
	
// Update is called once per frame
function Update() {
	// rotar en el eje a velocidad
	target.Rotate(eje * velocidad * Time.deltaTime);
}
