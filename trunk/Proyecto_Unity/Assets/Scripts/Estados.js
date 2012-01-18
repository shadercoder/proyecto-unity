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
var atenuacionRelieve			: float 	= 90;					//Suaviza o acentua el efecto de sombreado
var alturaColinas				: float		= 0.15;					//La altura a partir de la cual se considera colina
var alturaMontana				: float		= 0.2;					//La altura a partir de la cual se considera montaña

//Para la funcion GUI durante la creación del planeta
var estiloGUI					: GUISkin;							//Los estilos diferentes para la GUI, configurables desde el editor
private var menuOpcionesInt		: int		= 0;					//Variable de control sobre el menu lateral derecho

//Privadas del script
private var estado 				: T_estados = T_estados.principal;	//Los estados por los que pasa el juego
private var anchoTextura 		: int;
private var altoTextura 		: int;
private var relTexTabAncho		: float; 							//Que relación hay entre el ancho de la textura y el ancho del tablero lógico
private var relTexTabAlto		: float;							//Lo mismo pero para el alto
private var perlin				: Perlin;							//Semilla
private var nuevoTerreno		: boolean	= false;				//Si se quiere re-generar el terreno, se hace poniendo esto a true

private var tablero				: Casilla[,];						//Tablero lógico del algoritmo

//Opciones
var contenedorSonido			: GameObject;						//El objeto que va a contener la fuente del audio
private var sonido				: AudioSource;						//La fuente del audio
private var opcionesPerdurables	: Opciones;							//El objeto del que se van a sacar los valores de las opciones

private var musicaOn 			: boolean 	= true;					//Está la música activada?
private var musicaVol 			: float		= 0.5;					//A que volumen?
private var sfxOn 				: boolean 	= true;					//Estan los efectos de sonido activados?
private var sfxVol 				: float 	= 0.5; 					//A que volumen?

//Tooltips
private var posicionMouse : Vector3 = Vector3.zero;		//Guarda la ultima posicion del mouse		
private var activarTooltip : boolean = false;			//Controla si se muestra o no el tooltip	
private var ultimoMov : float = 0;						//Ultima vez que se movio el mouse		
var tiempoTooltip : float = 0.75;						//Tiempo que tarda en aparecer el tooltip	

//Tipos especiales ----------------------------------------------------------------------------------------------------------------------

enum T_estados {inicial, principal, laboratorio, filtros, guardar, opciones, salir, regenerar};						//Añadir los que hagan falta mas tarde
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

function ruidoTextura() {
	var pixels : Color[] = new Color[anchoTextura*altoTextura];
	for (var i : int = 0; i < altoTextura; i++) {
		for (var j : int = 0; j < anchoTextura; j++) {
			var valor : float = ruido_Turbulence(Vector2(j, i)*escala, octavas, lacunaridad, ganancia);
			pixels[j + i*anchoTextura] = Color(valor, valor, valor);
		}
	}
	return pixels;
}

function calcularMedia(pix : Color[]) {
	var med : float = 0;
	var max : float = -1.0;
	var min : float = 1.0;
	for (var i : int = 0; i < pix.Length; i++) {
		med += pix[i].r;
		if (pix[i].r > max)
			max = pix[i].r;
		if (pix[i].r < min)
			min = pix[i].r;
	}
	med /= pix.Length;
//	Debug.Log("Max = " + max);
//	Debug.Log("Min = " + min);
//	Debug.Log("Media = " + med);
	return med;
}

function mascaraBumpAgua(pixBump : Color[], media : float) {
	var pixAgua : Color[] = new Color[anchoTextura * altoTextura];
	for (var l : int = 0; l < pixAgua.Length; l++) {
		if (pixBump[l].r < media){
			pixBump[l] = Color(media,media,media);
			pixAgua[l] = Color(0,0,0);
		}
		else 
			pixAgua[l] = Color(1,1,1);			
	}
	return pixAgua;
}

function suavizaBordeTex(pix : Color[], tam : int) {
	var pixels : Color[] = pix;
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
			var valorBump : float = valorRuido * pesoRuido + (pixels[(i - 1) * anchoTextura + j].r) * pesoTextura;
			pixels[(i - 1)*anchoTextura + j] = Color(valorBump, valorBump, valorBump);
			j++;
		}
	}
	return pixels;
}

function suavizaPoloTex(pix : Color[], tam : int) {
	var pixels : Color[] = pix;
	var lado : int = tam;
	//Se ponen los polos desde el origen hasta el margen (en pixeles) con la orografía deseada
	for (var i : int = 0; i < margen; i++) {
		for (var j : int = 0; j < anchoTextura; j++) {			
			pixels[j + anchoTextura*i] = Color(0, 0, 0); 		//El valor nuevo de los polos
		}
	}
	for (i = altoTextura - margen; i < altoTextura; i++) {
		for (j = 0; j < anchoTextura; j++) {			
			pixels[j + anchoTextura*i] = Color(0, 0, 0); 		//El valor nuevo de los polos 
		}
	}
	
	//Ahora se suaviza desde y hacia el margen
	for (i = margen; i < margen + tam; i++) {
		for (j = 0; j < anchoTextura; j++) {
			var punto : Color = pixels[j + anchoTextura*i];
			var valor : float = punto.r * ((i + 1.0 - margen) / tam);  			
			pixels[j + anchoTextura*i] = Color(valor, valor, valor);					//El valor nuevo de los polos 
		}
	}
	var comienzo : float = altoTextura - (margen + tam);
	for (i = comienzo; i < altoTextura - margen; i++) {
		for (j = 0; j < anchoTextura; j++) {	
			punto = pixels[j + anchoTextura*i];
			valor = punto.r * ((tam - (i - comienzo)) / tam);  
			pixels[j + anchoTextura*i] = Color(valor, valor, valor); 						//El valor nuevo de los polos
		}
	}
	return pixels;
}

function safex(x :int){
	if (x >= anchoTextura)
		return x - anchoTextura;
	if (x < 0)
		return anchoTextura + x;
	return x;
}

function safey(y : int){
	if (y >= altoTextura)
		return y - altoTextura;
	if (y < 0)
		return altoTextura + y;
	return y;
}

function realzarRelieve(pix : Color[], media : float) {
	var pixels : Color[] = pix;
	for (var i : int = 0; i < pixels.Length; i++) {
		var valor : float = pixels[i].r;
		//Los valores por encima de la media * 2 seran maximos (0.85 sobre 1)
		//y de ahi hacia abajo linealmente descendentes (hasta 0)
		valor = Mathf.Lerp(0, 0.85, valor / (media * 2));	
		pixels[i] = Color(valor, valor, valor);
	}
	return pixels;
}

function creaNormalMap(tex : Texture2D){
	var pixels : Color32[] = tex.GetPixels32();
	var pixelsN : Color32[] = new Color32[anchoTextura * altoTextura];
	var c3 : Color;
	
	for (var y : int = 0; y < altoTextura; y++) {
        var offset : int  = y * anchoTextura;
        for (var x : int = 0; x < anchoTextura; x++)
        {

            var h0 : float = pixels[x + offset].r;
            var h1 : float = pixels[x + (anchoTextura * safey(y + 1))].r;
            var h2 : float = pixels[safex(x + 1) + offset].r;

            var Nx : float = h0 - h2;
            var Ny : float = h0 - h1;
			var Nz : float = atenuacionRelieve;

            var normal : Vector3 = new Vector3(Nx,Ny,Nz);
			normal.Normalize();
            normal /= 2;
			
            var cr : byte = (128 + (255 * normal.x));
            var cg : byte = (128 + (255 * normal.y));
            var cb : byte = (128 + (255 * normal.z));
			c3 = new Color32(cr, cg, cb, 128);
            
			pixelsN[x + offset] = c3;
        }
    }
    return pixelsN;
//	tex.SetPixels32(pixelsN);
//	tex.Apply();
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
	
	pixels = ruidoTextura();										//Se crea el ruido para la textura base y normales...
	pixels = suavizaBordeTex(pixels, texturaBase.width / 20);		//Se suaviza el borde lateral...
	pixels = suavizaPoloTex(pixels, texturaBase.height / 20);		//Se suavizan los polos...
	media = calcularMedia(pixels);									//Se calcula la media de altura...	
	pixels = realzarRelieve(pixels, media);							//Se realza el relieve
//	media = calcularMedia(pixels);									//Se calcula la media de altura con el nuevo relieve
		
	texturaBase.SetPixels(pixels);	
	texturaBase.Apply();
	
	
	
	//Transforma los pixeles y crea la mascara de reflejo del mar: 
	//las zonas menores de la altura media (donde hay agua) reflejan luz azul.
	//Devuelve tambien una mascara con los pixeles por encima del nivel del mar
	//para normalizarlos (el relieve submarino no se ve desde el espacio)
	var pixelsAgua : Color[] = mascaraBumpAgua(pixels, 0.5);		//se ignora el mar para el relieve
	texturaNorm.SetPixels(pixels);									//Se aplican los pixeles a la textura normal para duplicarlos
	texturaNorm.SetPixels32(creaNormalMap(texturaNorm));			//se transforma a NormalMap
	texturaNorm.Apply();
	texturaMask.SetPixels(pixelsAgua);
	texturaMask.Apply();
	
	estado = T_estados.principal;
}

function iniciaTablero(tex : Texture2D) {
	var pixels : Color[] = tex.GetPixels();
	tablero = new Casilla[altoTableroUtil,anchoTablero];
	for (var i : int = 0; i < altoTableroUtil; i++) {
		for (var j : int = 0; j < anchoTablero; j++) {
			//Las coordenadas de la casilla actual en la textura
			var cord : Vector2 = Vector2(j * relTexTabAncho , (i + casillasPolos) * relTexTabAlto);
			
			//Se calcula la media de altura de la casilla
			var media : float = 0;
			for (var x : int = 0; x < relTexTabAlto; x++) {
				for (var y : int = 0; y < relTexTabAncho; y++) {
					media += pixels[(cord.y + x)*anchoTextura + cord.x + y].r;
				}
			}
			media = media / (relTexTabAncho * relTexTabAlto);
			
			//Se calcula el habitat en el que va a estar la casilla y los elementos que tendrá
			//TODO Esto es un ejemplo a refinar...
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

function Awake() {
	creacionInicial();
	var opcTemp : GameObject = GameObject.FindWithTag("Opciones");
	if (opcTemp != null) {
		opcionesPerdurables = opcTemp.GetComponent(Opciones);
		musicaOn = opcionesPerdurables.getMusicaOn();
		musicaVol = opcionesPerdurables.getMusicaVol();
		sfxOn = opcionesPerdurables.getSfxOn();
		sfxVol = opcionesPerdurables.getSfxVol();
	}
	sonido = contenedorSonido.GetComponent(AudioSource);
	sonido.mute = !musicaOn;
	sonido.volume = musicaVol;
}

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
			Time.timeScale = 0;
			break;
		case T_estados.guardar:
			Time.timeScale = 0;
			break;
		case T_estados.salir:
			Application.LoadLevel("Escena_Inicial");
			break;
		default:
			//Error!
			Debug.LogError("Estado del juego desconocido! La variable contiene: " + estado);
			break;
	}
	
	//Control del tooltip
	if (Input.mousePosition != posicionMouse) {
		posicionMouse = Input.mousePosition;
		activarTooltip = false;
		ultimoMov = Time.time;
	}
	else {
		if (Time.time >= ultimoMov + tiempoTooltip) {
			activarTooltip = true;
		}
	}

}

//Funciones OnGUI---------------------------------------------------------------------------------------------------------------------------

function OnGUI() {
	GUI.skin = estiloGUI;
	switch (estado) {
		case T_estados.inicial:
			GUI.Box(Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Re-Generando!");
			break;
		case T_estados.opciones:
			menuOpciones();
			break;
		case T_estados.principal:
			grupoIzquierda();
			grupoDerecha();
			break;
		default:						
			break;
	}
	
	//Tooltip
	if (activarTooltip) {
		var longitud : int = GUI.tooltip.Length;
		if (longitud == 0) {
			return;
		}
		else {
			longitud *= 9;
		}
		var posx : float = Input.mousePosition.x;
		var posy : float = Input.mousePosition.y;
		if (posx > (Screen.width / 2)) {
			posx -= 215;
		}
		else {
			posx += 15;
		}
		if (posy > (Screen.height / 2)) {
			posy += 20;
		}		
		var pos : Rect = Rect(posx, Screen.height - posy, longitud, 25);
		GUI.Box(pos, "");
		GUI.Label(pos, GUI.tooltip);
	}
	
}

function grupoIzquierda() {
	GUI.BeginGroup(Rect(5, Screen.height / 2 - 110, 125, 230));
	if (GUI.Button(Rect(0, 0, 126, 79), GUIContent("", "Generar otro planeta") , "d_planeta")) {
		nuevoTerreno = true;
		estado = T_estados.regenerar;
	}
	if (GUI.Button(Rect(0, 79, 126, 70), GUIContent("", "Opciones de cámara"), "d_cam")) {
		menuOpcionesInt = 1;
	}
	if (GUI.Button(Rect(0, 149, 126, 79), GUIContent("", "Opciones generales"), "d_func")) {
		menuOpcionesInt = 2;
	}
	GUI.EndGroup();
}

function grupoDerecha() {
	//TODO Dependiendo de que opción este pulsada, poner un menú u otro!
	if (menuOpcionesInt == 1) {
		GUI.BeginGroup(Rect(Screen.width - 130, Screen.height / 2 - 110, 125, 230));
		if (GUI.Button(Rect(0, 0, 127, 79), GUIContent("", "Click izq. para centrar"), "i_c_fija")) {
			var script : Control_Raton = transform.GetComponent(Control_Raton);
			var objetivo : Transform = GameObject.Find("Planeta").GetComponent(Transform);
			script.cambiarTarget(objetivo);
			script.cambiarEstado(1);
		}
		if (GUI.Button(Rect(0, 79, 127, 70), GUIContent("", "Rotar con click der."), "i_c_rot")) {
			script = transform.GetComponent(Control_Raton);
			objetivo = GameObject.Find("Planeta").GetComponent(Transform);
			script.cambiarTarget(objetivo);
			script.cambiarEstado(0);
		}
		if (GUI.Button(Rect(0, 149, 127, 79), GUIContent("", "Centrar en la luna"), "i_c_3")) {
			script = transform.GetComponent(Control_Raton);
			objetivo = GameObject.Find("Moon").GetComponent(Transform);
			script.cambiarTarget(objetivo);
			script.cambiarEstado(0);
		}
		GUI.EndGroup();
	}
	if (menuOpcionesInt == 2) {
		GUI.BeginGroup(Rect(Screen.width - 130, Screen.height / 2 - 110, 125, 230));
		if (GUI.Button(Rect(0, 0, 126, 79), GUIContent("", "Laboratorio genético"), "i_lab")) {

		}
		if (GUI.Button(Rect(0, 79, 126, 70), GUIContent("", "Visión de la nave"), "i_nav")) {
;
		}
		if (GUI.Button(Rect(0, 149, 126, 79), GUIContent("", "Opciones del juego"), "i_fil")) {
			script = transform.GetComponent(Control_Raton);
			script.setInteraccion(false);
			estado = T_estados.opciones;
		}
		GUI.EndGroup();
	}
}

function menuOpciones() {
	GUILayout.BeginArea(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
	GUILayout.BeginVertical();
	if (GUILayout.Button(GUIContent("Salir", "Salir del juego"), "boton_menu_1")) {
		Time.timeScale = 1.0;
		var script : Control_Raton = transform.GetComponent(Control_Raton);
		script.setInteraccion(true);
		estado = T_estados.salir;
	}
	if (GUILayout.Button(GUIContent("Guardar", "Guardar la partida"), "boton_menu_2")) {
		Time.timeScale = 1.0;
		estado = T_estados.guardar;
	}
	if (GUILayout.Button(GUIContent("Volver", "Volver al juego"), "boton_menu_4")) {
		Time.timeScale = 1.0;
		script = transform.GetComponent(Control_Raton);
		script.setInteraccion(true);
		estado = T_estados.principal;
	}
	GUILayout.EndVertical();
	GUILayout.EndArea();
}

