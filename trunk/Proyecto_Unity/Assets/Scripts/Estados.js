#pragma strict

//Variables ---------------------------------------------------------------------------------------------------------------------------

//Para el tablero
var anchoTablero				: int 		= 128;					//El ancho del tablero lógico (debe ser potencia de 2 para cuadrar con la textura)
var altoTablero					: int 		= 128;					//El alto del tablero lógico (debe ser potencia de 2 tambien)
var casillasPolos				: int		= 3;					//El numero de casillas que serán intransitables en los polos
private var altoTableroUtil		: int;								//El alto del tablero una vez eliminadas las casillas de los polos
private var margen 				: int 		= 50;					//El numero de pixeles que habrá en los polos intransitables
var numMaxEspeciesCasilla		: int		= 5;					//Numero maximo de especies que puede haber por casilla a la vez
var numMaxEspecies				: int		= 20;					//Numero maximo de especies que puede haber en el tablero (juego) a la vez

//Para el ruido
var octavas						: float		= 6;					//Octavas para la funcion de ruido de turbulencias
var lacunaridad					: float		= 2.5;					//La lacunaridad (cuanto se desplazan las coordenadas en sucesivas "octavas")
var ganancia					: float		= 0.8;					//El peso que se le da a cada nueva octava
var escala						: float		= 0.005;				//El nivel de zoom sobre el ruido
var nivelAgua					: float		= 0.1;					//El nivel sobre el que se pondrá agua. La media de altura suele ser 0.1
var tamanoPlaya 				: float		= 0.01;					//El tamaño de las playas
var alturaColinas				: float		= 0.15;					//La altura a partir de la cual se considera colina
var alturaMontana				: float		= 0.2;					//La altura a partir de la cual se considera montaña

//Para la funcion GUI durante la creación del planeta
var estiloGUI					: GUISkin;							//Los estilos diferentes para la GUI, configurables desde el editor
private var menuOpcionesInt		: int		= 0;					//Variable de control sobre el menu lateral derecho

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

enum T_estados {inicial, principal, laboratorio, filtros, camaras, opciones, salir, regenerar};						//Añadir los que hagan falta mas tarde
enum T_habitats {mountain, plain, hill, sand, volcanic, sea, coast};												//Tipos de orografía
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
	for (var i : int = 0; i < nOctavas; i++) {
		ruidoTotal += amplitud * Mathf.Abs(perlin.Noise(coords.x, coords.y));
		amplitud *= gain;
		coords *= lacunarity;
	}
	return ruidoTotal;
}

function esperaSegundos(segs : float) {
	yield WaitForSeconds(segs);
}

function pausaJuegoSegs(segs : float) {
	yield StartCoroutine(esperaSegundos(segs));
}

//Funciones sobre el terreno y el tablero ---------------------------------------------------------------------------------------------------

function ruidoTextura(tex : Texture2D) {
	var pixels : Color[] = new Color[anchoTextura*altoTextura];
	var pixelsBump : Color[] = new Color[anchoTextura*altoTextura];
	for (var i : int = 0; i < altoTextura; i++) {
		for (var j : int = 0; j < anchoTextura; j++) {
			var valor : float = ruido_Turbulence(Vector2(j, i)*escala, octavas, lacunaridad, ganancia);
			pixels[j + i*anchoTextura] = Color(0.25+0.5*valor,0.2+0.4*valor,valor);
			pixelsBump[j + i*anchoTextura] = Color(valor, valor, valor);
		}
	}
	tex.SetPixels(pixelsBump);
	tex.Apply();
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
		//TODO Mejorar la visualización de playas, montañas y mares para que se note mas el relieve!
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

function mascaraReflejoAgua(pix : Color[], media : float) {
	var pixels : Color[] = pix;
	for (var l : int = 0; l < pixels.Length; l++) {
		if (pixels[l].r > media)
			pixels[l] = Color(1,1,1);
		else
			pixels[l] = Color(0,0,0);			
	}
	return pixels;
}

function suavizaBordeTex(pix : Color[], tex : Texture2D, tam : int) {
	var pixels : Color[] = pix;
	var pixelsBump : Color[] = tex.GetPixels();
	var lado : int = tam;
	for (var i : int = 0; i < altoTextura; i++) {
		var j : int = anchoTextura;
		var pesoRuido : float = 1.0;
		var pesoTextura : float = 0.0;
		var iteraciones : float = pesoRuido / lado;
		while (pesoTextura < 1.0) {
			pesoTextura += iteraciones;
			pesoRuido -= iteraciones;
			var valorRuido : float = ruido_Turbulence(Vector2(j, i)*escala, octavas, lacunaridad, ganancia);
			var valorBump : float = valorRuido*pesoRuido + (pixelsBump[(i - 1)*anchoTextura + j].r)*pesoTextura;
			pixelsBump[(i - 1)*anchoTextura + j] = Color(valorBump, valorBump, valorBump);
			pixels[(i - 1)*anchoTextura + j] = Color(0.25+0.5*valorBump,0.2+0.4*valorBump,valorBump);
			j++;
		}
	}
	tex.SetPixels(pixelsBump);
	tex.Apply();
	return pixels;
}

function suavizaPoloTex(pix : Color[], tex : Texture2D, tam : int) {
	var pixels : Color[] = pix;
	var pixelsBump : Color[] = tex.GetPixels();
	var lado : int = tam;
	//Se ponen los polos desde el origen hasta el margen (en pixeles) con la orografía deseada
	for (var i : int = 0; i < margen; i++) {
		for (var j : int = 0; j < anchoTextura; j++) {			
			pixelsBump[j + anchoTextura*i] = Color(0, 0, 0); 		//El valor nuevo de los polos
			pixels[j + anchoTextura*i] = Color(0.25, 0.2, 0); 		//El valor nuevo de los polos con pintura
		}
	}
	for (i = altoTextura - margen; i < altoTextura; i++) {
		for (j = 0; j < anchoTextura; j++) {			
			pixelsBump[j + anchoTextura*i] = Color(0, 0, 0); 		//El valor nuevo de los polos
			pixels[j + anchoTextura*i] = Color(0.25, 0.2, 0); 		//El valor nuevo de los polos con pintura
		}
	}
	
	//Ahora se suaviza desde y hacia el margen
	for (i = margen; i < margen + tam; i++) {
		for (j = 0; j < anchoTextura; j++) {
			var punto : Color = pixelsBump[j + anchoTextura*i];
			var valor : float = punto.r * ((i + 1.0 - margen) / tam);  
			
			pixelsBump[j + anchoTextura*i] = Color(valor, valor, valor); 				//El valor nuevo de los polos
			pixels[j + anchoTextura*i] = Color(0.25+0.5*valor,0.2+0.4*valor,valor);	//El valor nuevo de los polos con pintura
		}
	}
	var comienzo : float = altoTextura - (margen + tam);
	for (i = comienzo; i < altoTextura - margen; i++) {
		for (j = 0; j < anchoTextura; j++) {	
			punto = pixelsBump[j + anchoTextura*i];
			valor = punto.r * ((tam - (i - comienzo)) / tam);  
					
			pixelsBump[j + anchoTextura*i] = Color(valor, valor, valor); 					//El valor nuevo de los polos
			pixels[j + anchoTextura*i] = Color(0.25+0.5*valor,0.2+0.4*valor,valor); 		//El valor nuevo de los polos con pintura
		}
	}
	
	tex.SetPixels(pixelsBump);
	tex.Apply();
	return pixels;
}

function realzarRelieve(tex : Texture2D) {
	var pixels : Color[] = tex.GetPixels();
	for (var i : int = 0; i < pixels.Length; i++) {
		var valor : float = pixels[i].r;
		valor += Mathf.Lerp(-0.2, 0.6, valor/0.3);			//Los valores bajos bajarán y los valores altos subirán		
		valor = Mathf.Clamp01(valor);
		pixels[i] = Color(valor, valor, valor);
	}
	tex.SetPixels(pixels);
	tex.Apply();
}

//Funciones principales ----------------------------------------------------------------------------------------------------------------------
function creacionInicial() {
	
	//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
	var planeta : GameObject = GameObject.FindWithTag("Planeta");
	var renderer : MeshRenderer = planeta.GetComponent(MeshRenderer);
	var texturaBase : Texture2D = renderer.sharedMaterial.mainTexture as Texture2D;
	var texturaNorm : Texture2D = renderer.sharedMaterial.GetTexture("_Normals") as Texture2D;	//Los nombres vienen definidos en el editor, en el material
	var texturaMask : Texture2D = renderer.sharedMaterial.GetTexture("_Mask") as Texture2D;

	anchoTextura = texturaBase.width;
	altoTextura = texturaBase.height;
	altoTableroUtil = altoTablero - casillasPolos*2;
	relTexTabAncho = anchoTextura / anchoTablero;
	relTexTabAlto = altoTextura / altoTablero;
	margen = relTexTabAlto * casillasPolos;
	
	//Comprobacion de errores
	if (relTexTabAncho < 1 || relTexTabAlto < 1) {
		Debug.LogError("La textura no tiene tamaño suficiente para ese tablero lógico. Deben estar como minimo en relacion 1:1!");
	}
	if (texturaBase == null || texturaNorm == null || texturaMask == null) {
		Debug.LogError("Una de las texturas con las que debemos trabajar (superficie, bump o mascara) es nula!");
	}
	if (texturaNorm.width != texturaBase.width || texturaNorm.height != texturaBase.height) {
		Debug.LogError("Las texturas del plaeta y el mapa de relieve deben tener la misma superficie!");
	}
	
	//TODO Inicializacion del tablero
	iniciaTablero(texturaNorm);
	
	//Crear una seed inicial para que no todos los mapas generados sean iguales!
	if (perlin == null || nuevoTerreno) {
		perlin = new Perlin();
		nuevoTerreno = false;
	}
		
	var media : float = 0;
	var pixels : Color[] = new Color[texturaBase.width*texturaBase.height];
	
	pixels = ruidoTextura(texturaNorm);											//Se crea el ruido para la textura base y normales...
	pixels = suavizaBordeTex(pixels, texturaNorm, texturaBase.width / 20);		//Se suaviza el borde lateral...
	pixels = suavizaPoloTex(pixels, texturaNorm, texturaBase.height / 20);		//Se suavizan los polos...
	media = calcularMedia(texturaNorm.GetPixels());								//Se calcula la media de altura...	
	realzarRelieve(texturaNorm);												//Se realza el relieve
	pixels = ponPlayas(pixels, nivelAgua);										//Poner agua en el mundo a partir de la media de "altura"
		
	texturaBase.SetPixels(pixels);
	texturaBase.Apply();
	
	//TODO: Hay que caracterizar a texturaNorm (el bumpMap) como tal! (una vez creada, establecer como tipo Normal Map)
	//texturaABumpMap(texturaNorm);
	
	//crea la mascara de reflejo del mar: las zonas menores de la altura media (donde hay agua) reflejan luz azul
	pixels = mascaraReflejoAgua(pixels, nivelAgua);
	
	texturaMask.SetPixels(pixels);
	texturaMask.Apply();
	
	estado = T_estados.principal;
}

function iniciaTablero(tex : Texture2D) {
	var pixels : Color[] = tex.GetPixels();
	tablero = new Casilla[altoTableroUtil,anchoTablero];
	for (var i : int = 0; i < altoTableroUtil; i++) {
		for (var j : int = 0; j < anchoTablero; j++) {
			//Las coordenadas de la casilla actual en la textura
			var cord : Vector2 = Vector2(j , (i + casillasPolos)*relTexTabAlto);
			
			//Se calcula la media de altura de la casilla
			var media : float = 0;
			for (var x : int = 0; x < relTexTabAlto; x++) {
				for (var y : int = 0; y < relTexTabAncho; y++) {
					media += pixels[(i + x)*anchoTextura + (j*relTexTabAncho) + y].r;
				}
			}
			media = media / (relTexTabAncho * relTexTabAlto);
			
			//Se calcula el habitat en el que va a estar la casilla y los elementos que tendrá
			var elems : T_elementos[] = new T_elementos[5];
			var habitat : T_habitats;
			if (media < (nivelAgua - (tamanoPlaya * 1.2))) {
				habitat = T_habitats.sea;
			} 
			else if (((nivelAgua - (tamanoPlaya * 1.2)) < media) && (media < (nivelAgua + (tamanoPlaya * 1.2)))) {
				habitat = T_habitats.coast;
			}
			else if (((nivelAgua + (tamanoPlaya * 1.2)) < media) && (media < alturaColinas)) {
				habitat = T_habitats.plain;
			}
			else if ((alturaColinas < media) && (media < alturaMontana)) {
				habitat = T_habitats.hill;
			}
			else if (alturaMontana < media) {
				habitat = T_habitats.mountain;
			}
			
			//TODO Se coge una o varias especies aleatorias de las iniciales
			var esp : Especie[] = new Especie[numMaxEspeciesCasilla];
			//TODO Calculos para ver la especie/s a meter
			
			tablero[i,j] = new Casilla(media, habitat, elems, esp, cord);
		}
	}
}

//Update y transiciones de estados -------------------------------------------------------------------------------------------------------

function Update () {

	switch (estado) {
		case T_estados.inicial:
			creacionInicial();
			break;
		case T_estados.principal:
			break;
		case T_estados.regenerar:
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
			Debug.LogError("Estado del juego desconocido! La variable contiene: " + estado);
			break;
	}

}

//Funciones OnGUI---------------------------------------------------------------------------------------------------------------------------

function OnGUI() {
	GUI.skin = estiloGUI;
	grupoIzquierda();
	grupoDerecha();
	switch (estado) {
		case T_estados.inicial:
			GUI.Box(Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Re-Generando!");
			break;
		default:						
			break;
	}
	
}

function grupoIzquierda() {
	GUI.BeginGroup(Rect(5, Screen.height / 2 - 140, 300, 300));
	if (GUI.Button(Rect(0, 0, 79, 96), "", "botonPlaneta")) {
		nuevoTerreno = true;
		estado = T_estados.regenerar;
	}
	if (GUI.Button(Rect(0, 100, 79, 96), "", "botonOpciones")) {
		menuOpcionesInt = 1;
		estado = T_estados.opciones;
	}
	if (GUI.Button(Rect(0, 200, 79, 96), "", "botonIz")) {
		menuOpcionesInt = 0;
		estado = T_estados.principal;
	}
	GUI.EndGroup();
}

function grupoDerecha() {
	//TODO Dependiendo de que opción este pulsada, poner un menú u otro!
	if (menuOpcionesInt == 1) {
		GUI.BeginGroup(Rect(Screen.width - 100, Screen.height / 2 - 140, 300, 300));
		if (GUI.Button(Rect(0, 0, 79, 96), "", "botonCamRot")) {
			
		}
		if (GUI.Button(Rect(0, 100, 79, 96), "Opcion 2", "botonDer")) {
			
		}
		if (GUI.Button(Rect(0, 200, 79, 96), "Opcion 3", "botonDer")) {
			
		}
		GUI.EndGroup();
	}
}

