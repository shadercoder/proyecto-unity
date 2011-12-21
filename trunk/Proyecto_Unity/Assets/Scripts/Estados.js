#pragma strict

enum T_estados {inicial, principal, laboratorio, filtros, camaras, opciones, salir};				//Añadir los que hagan falta mas tarde


private var estado : T_estados = T_estados.inicial;
 

function creacionInicial() {
	//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
	var planeta : GameObject = GameObject.FindWithTag("Planeta");
	var renderer : MeshRenderer = planeta.GetComponent(MeshRenderer);
	var textura : Texture2D = renderer.sharedMaterial.mainTexture as Texture2D;
	//Por aqui voy!!
	
}

function Update () {

	switch (estado) {
		case T_estados.inicial:
			creacionInicial();
			estado = T_estados.principal;
			break;
		case T_estados.principal:
			//Algoritmo!
			break;
		default:
			//Error!
			break;
	}

}