#pragma strict

var canciones : AudioClip[];
private var orden : int[];

//Generar una lista de canciones aleatoria
function Awake() {
	orden = new int[canciones.length];
	randomizarLista();
}

function Start() {
	
}

function randomizarLista() {
	for (var i : int = 0; i < canciones.length; i++) {
		orden[i] = -1;
	}
	//TODO Añadir un orden aleatorio al array orden
}
