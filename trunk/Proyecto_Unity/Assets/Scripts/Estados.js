#pragma strict

var anchoTablero			: int 		= 128;					//El ancho del tablero lógico (debe ser potencia de 2 para cuadrar con la textura)
var altoTablero				: int 		= 128;					//El alto del tablero lógico (debe ser potencia de 2 tambien)
var margen 					: int 		= 2;					//El numero de filas que habrá en los polos intransitables
var octavas					: float		= 6;					//Octavas para la funcion de ruido fBm
var lacunaridad				: float		= 2.5;					//La lacunaridad (cuanto se desplazan las coordenadas en sucesivas "octavas")
var tamanoPlaya 			: float		= 0.01;
var ganancia				: float		= 0.8;					//El peso que se le da a cada nueva octava
var offset 					: float		= 0.75;					//Valor para determinados algoritmos de ruido
var escala					: float		= 0.005;					//Lo mismo
var agua					: float		= 0.4;					//Entre 0 y 1, en que punto aparece agua
private var estado 			: T_estados = T_estados.inicial;	//Los estados por los que pasa el juego
private var anchoTextura 	: int;
private var altoTextura 	: int;
private var relTexTabAncho	: float; 							//Que relación hay entre el ancho de la textura y el ancho del tablero lógico
private var relTexTabAlto	: float;							//Lo mismo pero para el alto
private var perlin			: Perlin;							//Semilla


enum T_estados {inicial, principal, laboratorio, filtros, camaras, opciones, salir};	//Añadir los que hagan falta mas tarde
enum T_habitats {mountain, plain, hill, sand, volcanic};								//Tipos de orografía
 
 
 
function ruido_fBm(coords : Vector2, nOctavas : int, lacunarity : float, gain : float) {
	var ruidoTotal : float = 0.0;
	var amplitud : float = gain;
	var sumaAmplitud : float = 0;
	for (var i : int = 0; i < nOctavas; i++) {
		ruidoTotal += amplitud * perlin.Noise(coords.x, coords.y);
		sumaAmplitud += amplitud;
		amplitud *= gain;
		coords *= lacunarity;
	}
	ruidoTotal /= sumaAmplitud;
	return ruidoTotal;
}

function ruido_Turbulence(coords : Vector2, nOctavas : int, lacunarity : float, gain : float) {
	var ruidoTotal : float = 0.0;
	var amplitud : float = gain;
	var sumaAmplitud : float = 0;
	for (var i : int = 0; i < nOctavas; i++) {
		ruidoTotal += amplitud * Mathf.Abs(perlin.Noise(coords.x, coords.y));
		sumaAmplitud += amplitud;
		amplitud *= gain;
		coords *= lacunarity;
	}
//	ruidoTotal /= sumaAmplitud;
	return ruidoTotal;
}

function creacionInicial() {
	//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
	var planeta : GameObject = GameObject.FindWithTag("Planeta");
	var renderer : MeshRenderer = planeta.GetComponent(MeshRenderer);
	var texturaNorm : Texture2D = renderer.sharedMaterial.mainTexture as Texture2D;
	var texturaBump : Texture2D = renderer.sharedMaterial.GetTexture("_Normals") as Texture2D;	//Los nombres vienen definidos en el editor, en el material
	var texturaMask : Texture2D = renderer.sharedMaterial.GetTexture("_Mask") as Texture2D;
	//Crear el tablero lógico a la vez!
	anchoTextura = texturaNorm.width;
	altoTextura = texturaNorm.height;
	relTexTabAncho = anchoTextura / anchoTablero;
	relTexTabAlto = altoTextura / altoTablero;
	if (relTexTabAncho < 1 || relTexTabAlto < 1) {
		Debug.LogError("La textura no tiene tamaño suficiente para ese tablero lógico. Deben estar como minimo en relacion 1:1!");
	}
	//Crear una seed inicial para que no todos los mapas generados sean iguales!
	if (perlin == null)
		perlin = new Perlin();
	var fractal : FractalNoise = new FractalNoise(ganancia, lacunaridad, octavas, perlin);
	var media : float = 0;
	var pixels : Color[] = new Color[texturaNorm.width*texturaNorm.height];
	//Hay que eliminar la fila superior e inferior de la textura para evitar problemas con el toroide
	//también hacer algo al respecto donde se juntan los bordes derecho e izquierdo
	for (var i : int = margen; i < altoTablero - margen; i++) {
		for (var j : int = 0; j < anchoTablero; j++) {
			//Aqui usamos casillas, pero cada casilla logica equivale a varios pixeles
			for (var a : int = 0; a < relTexTabAlto; a++) {
				for (var b : int = 0; b < relTexTabAncho; b++) {
					//Esto ya son píxeles dentro de cada casilla!
//					var valor : float = ruido_fBm(Vector2(j*relTexTabAncho + b, i*relTexTabAlto + a)*escala, octavas, lacunaridad, ganancia);
					var valor : float = ruido_Turbulence(Vector2(j*relTexTabAncho + b, i*relTexTabAlto + a)*escala, octavas, lacunaridad, ganancia);
//					var valor : float = fractal.HybridMultifractal((j*relTexTabAncho + b)*escala, (i*relTexTabAlto + a)*escala, offset);
//					var valor : float = fractal.RidgedMultifractal((j*relTexTabAncho + b)*escala, (i*relTexTabAlto + a)*escala, offset, ganancia);
//					var valor : float = fractal.BrownianMotion((j*relTexTabAncho + b)*escala, (i*relTexTabAlto + a)*escala);
//					var valor : float = perlin.Noise((j*relTexTabAncho + b)*escala, (i*relTexTabAlto + a)*escala); 
					media += valor;
					pixels[(j*relTexTabAncho + b) + (i*relTexTabAlto + a)*texturaNorm.width] = Color(0.25+0.5*valor,0.2+0.4*valor,valor);
				}
			}
		}
	}
	media /= pixels.Length;
	Debug.Log(media);
	//Poner agua en el mundo a partir de la media de "altura"
	for (var k : int = 0; k < pixels.Length; k++) {
		//esto es una ñapa, pero queria probar a ver como quedaba :D
		//hace playas
		if ((pixels[k].b > media) &&  (pixels[k].b < media+tamanoPlaya))
			pixels[k] = Color(pixels[k].r*1.2,pixels[k].g*1.2,pixels[k].b+0.075);
		if ((pixels[k].b < media) &&  (pixels[k].b > media-tamanoPlaya/2))
			pixels[k] = Color(pixels[k].r*0.2,pixels[k].g+0.15,pixels[k].b+0.5);
		if ((pixels[k].b < media-tamanoPlaya/2) &&  (pixels[k].b > media-tamanoPlaya))
			pixels[k] = Color(pixels[k].r*0.2,pixels[k].g+0.025,pixels[k].b+0.3);
		//hace el mar
		else if (pixels[k].b < media)
			pixels[k] = Color(0,pixels[k].g*0.8,pixels[k].b+0.25);

	}
	texturaNorm.SetPixels(pixels);
	texturaNorm.Apply();
	
	
	//crea la mascara de reflejo del mar: las zonas menores de la altura media (donde hay agua) reflejan luz azul
	for (var l : int = 0; l < pixels.Length; l++) {
		if (pixels[l].r > media)
			pixels[l] = Color(0,0,0);
		else
			pixels[l] = Color(1,1,1);
			
	}
	texturaMask.SetPixels(pixels);
	texturaMask.Apply();
	
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