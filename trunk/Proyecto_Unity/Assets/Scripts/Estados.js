#pragma strict

//Variables ---------------------------------------------------------------------------------------------------------------------------

//Para el tablero
var anchoTablero				: int 		= 128;					//El ancho del tablero lógico (debe ser potencia de 2 para cuadrar con la textura)
var altoTablero					: int 		= 128;					//El alto del tablero lógico (debe ser potencia de 2 tambien)
var margen 						: int 		= 2;					//El numero de filas que habrá en los polos intransitables

//Para el ruido
var octavas						: float		= 6;					//Octavas para la funcion de ruido fBm
var lacunaridad					: float		= 2.5;					//La lacunaridad (cuanto se desplazan las coordenadas en sucesivas "octavas")
var tamanoPlaya 				: float		= 0.01;
var ganancia					: float		= 0.8;					//El peso que se le da a cada nueva octava
//var offset 					: float		= 0.75;					//Valor para determinados algoritmos de ruido
var escala						: float		= 0.005;					//Lo mismo
//var agua						: float		= 0.4;					//Entre 0 y 1, en que punto aparece agua

//Para la funcion GUI durante la creación del planeta
private var menuGridInt			: int		= 0;
private var menuGridStrings		: String[] 	= ["Re-generar", "Opciones", "Principal"];
private var menuOpcionesInt		: int		= 0;
private var menuOpcionesStrings	: String[] 	= ["Opción 1", "Opción 2", "Opción 3"];
private var menuDerecha			: boolean[]	= [false, false, false];

//Privadas del script
private var estado 				: T_estados = T_estados.inicial;	//Los estados por los que pasa el juego
private var anchoTextura 		: int;
private var altoTextura 		: int;
private var relTexTabAncho		: float; 							//Que relación hay entre el ancho de la textura y el ancho del tablero lógico
private var relTexTabAlto		: float;							//Lo mismo pero para el alto
private var perlin				: Perlin;							//Semilla
private var nuevoTerreno		: boolean	= false;				//Si se quiere re-generar el terreno, se hace poniendo esto a true

private var tablero				: Casilla[,];						//Tablero lógico del algoritmo

//Tipos especiales ----------------------------------------------------------------------------------------------------------------------

enum T_estados {inicial, principal, laboratorio, filtros, camaras, opciones, salir, regenerar};	//Añadir los que hagan falta mas tarde
enum T_habitats {mountain, plain, hill, sand, volcanic, sea, coast};								//Tipos de orografía
enum T_elementos {hidrogeno, helio, oxigeno, carbono, boro, nitrogeno, litio, silicio, magnesio, argon, potasio};	//Se pueden añadir mas mas adelante

class Especie {
	//A rellenar
}

class Casilla {
	public var altura		: float;
	public var habitat 		: T_habitats;
	public var elementos	: T_elementos[];
	public var especies		: Especie[];
	public var coordsTex	: Vector2;
	
	function Casilla(alt : float, hab : T_habitats, elems : T_elementos[], esp : Especie[], coord : Vector2) {
		habitat = hab;
		altura = alt;
		elementos = elems;
		especies = esp;
		coordsTex = coord;
	}
}
//Funciones auxiliares --------------------------------------------------------------------------------------------------------------------
function ruido_Turbulence(coords : Vector2, nOctavas : int, lacunarity : float, gain : float) {
	var ruidoTotal : float = 0.0;
	var amplitud : float = gain;
	var sumaAmplitud : float = 0;
	for (var i : int = 0; i < nOctavas; i++) {
		ruidoTotal += amplitud * Mathf.Abs(perlin.Noise(coords.x, coords.y));
		amplitud *= gain;
		coords *= lacunarity;
	}
	return ruidoTotal;
}

function esperaSegundos(segs : float) {
	var tiempo : float = Time.time;
	while (Time.time);
	***;
}

//Funciones sobre el terreno y el tablero ---------------------------------------------------------------------------------------------------
function ruidoTextura(tex : Texture2D, tex2 : Texture2D) {
	var pixels : Color[] = new Color[tex.width*tex.height];
	var pixelsBump : Color[] = new Color[tex.width*tex.height];
	for (var i : int = 0; i < tex.height; i++) {
		for (var j : int = 0; j < tex.width; j++) {
			var valor : float = ruido_Turbulence(Vector2(j, i)*escala, octavas, lacunaridad, ganancia);
			pixels[j + i*tex.width] = Color(0.25+0.5*valor,0.2+0.4*valor,valor);
			pixelsBump[j + i*tex.width] = Color(valor, valor, valor);
		}
	}
	tex2.SetPixels(pixelsBump);
	tex2.Apply();
	return pixels;
}

function calcularMedia(pix : Color[]) {
	var med : float = 0;
	for (var i : int = 0; i < pix.Length; i++) {
		med += pix[i].r;
	}
	med /= pix.Length;
	return med;
}

function ponPlayas(pix : Color[], media : float) {
	var pixels : Color[] = pix;
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
	return pixels;
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
	
	//Comprobacion de errores
	if (relTexTabAncho < 1 || relTexTabAlto < 1) {
		Debug.LogError("La textura no tiene tamaño suficiente para ese tablero lógico. Deben estar como minimo en relacion 1:1!");
	}
	if (texturaNorm == null || texturaBump == null || texturaMask == null) {
		Debug.LogError("Una de las texturas con las que debemos trabajar (superficie, bump o mascara) es nula!");
	}
	if (texturaBump.width != texturaNorm.width || texturaBump.height != texturaNorm.height) {
		Debug.LogError("Las texturas del plaeta y el mapa de relieve deben tener la misma superficie!");
	}
	
	//Inicializacion del tablero
	tablero = new Casilla[altoTablero,anchoTablero];
	
	//Crear una seed inicial para que no todos los mapas generados sean iguales!
	if (perlin == null || nuevoTerreno) {
		perlin = new Perlin();
		nuevoTerreno = false;
	}
		
	var media : float = 0;
	var pixels : Color[] = new Color[texturaNorm.width*texturaNorm.height];
	pixels = ruidoTextura(texturaNorm, texturaBump);
	media = calcularMedia(texturaBump.GetPixels());

	//Poner agua en el mundo a partir de la media de "altura"
	pixels = ponPlayas(pixels, media);
	
	//Hay que eliminar la fila superior e inferior de la textura para evitar problemas con el toroide
	//también hacer algo al respecto donde se juntan los bordes derecho e izquierdo
	
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
	
	estado = T_estados.principal;
}

//Update y transiciones de estados -------------------------------------------------------------------------------------------------------
function Update () {

	switch (estado) {
		case T_estados.inicial:
			creacionInicial();
			break;
		case T_estados.principal:
			//Algoritmo!
			break;
		case T_estados.regenerar:
			
			if (esperaSegundos(1))
				estado = T_estados.inicial;
			break;
		case T_estados.filtros:
			break;
		case T_estados.laboratorio:
			break;
		case T_estados.opciones:
			break;
		case T_estados.camaras:
			break;
		case T_estados.salir:
			break;
		default:
			//Error!
			break;
	}

}

//Funciones OnGUI---------------------------------------------------------------------------------------------------------------------------
function OnGUI() {
	
	switch (estado) {
		case T_estados.inicial:
			GUI.Label (Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Re-Generating!");
			break;
		case T_estados.regenerar:
			GUI.Label (Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Re-Generating!");
			break;	
		default:
			menuGridInt = GUI.SelectionGrid(Rect (5, Screen.height / 2 - 100, 70, 200), menuGridInt, menuGridStrings, 1);
			if (menuDerecha[0])
				menuOpcionesInt = GUI.SelectionGrid(Rect (Screen.width - 75, Screen.height / 2 - 100, 70, 200), menuOpcionesInt, menuOpcionesStrings, 1);
			if (menuDerecha[1])
				;
			
			if (GUI.changed)
			{		
				if (menuGridInt == 0) //Opcion re-generar
				{
					GUI.Label (Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Re-Generating!");
					nuevoTerreno = true;
					estado = T_estados.regenerar;
				}
				if (menuGridInt == 1) //Opcion opciones
				{
					menuDerecha = [false, false, false];
					menuDerecha[0] = true;
					estado = T_estados.opciones;
				}
				if (menuGridInt == 2) //Opcion principal
				{
					menuDerecha = [false, false, false];
					estado = T_estados.principal;
				}
			}			
			break;
	}
	
}