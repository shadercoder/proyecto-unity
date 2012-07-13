using UnityEngine;

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

using System.Collections;
using System.Collections.Generic;

//Clase contenedora del savegame ------------------------------------------------------------------------------------------------------
[System.Serializable]
public class SaveData {

	//Variables a salvar
	public int width;
	public int height;
	public float[] data;


  	public SaveData () {}

}

//Clase con los métodos a llamar para crear el savegame -------------------------------------------------------------------------------
public class SaveLoad {
	
	public static string currentFileName = "SaveGame.hur";						
	public static string currentFilePath = Application.persistentDataPath + "/Saves/";
	
	public static void Save (Texture2D tex) {							//Sobrecargado
		Color[] colTemp = tex.GetPixels();
		int tempLong = tex.width * tex.height;
		float[] data = new float[tempLong];
		for (int i = 0; i < tempLong; i++) {
			data[i] = colTemp[i].r;
		}
		Save(data, tex.width, tex.height);
	}
	
  	public static void Save (float[] data, int width, int height)  		//Sobrecargado
  	{
    	Save (currentFilePath + currentFileName, data, width, height);
  	}
	
  	public static void Save (string filePath, float[] data, int width, int height)
  	{ 
	    SaveData save = new SaveData ();
		save.data = data;
		save.width = width;
		save.height = height;
	    FileStream stream = new FileStream(filePath, FileMode.Create);
		try {
		    BinaryFormatter bformatter = new BinaryFormatter();
		    bformatter.Serialize(stream, save);
		}
		catch (SerializationException e) {
			Debug.LogError("Excepcion al serializar el savegame. Datos: " + e.Message);
		}
		finally {
	    	stream.Close();
		}
  	}

  	public static SaveData Load ()  {									//Sobrecargado
		return Load(currentFilePath + currentFileName);
	} 
	
  	public static SaveData Load(string filePath) 
  	{
	    SaveData data = new SaveData ();
	    FileStream stream = new FileStream(filePath, FileMode.Open);
		try {
		    BinaryFormatter bformatter = new BinaryFormatter();
		    data = (SaveData)bformatter.Deserialize(stream);
		}
		catch (SerializationException e) {
			Debug.LogError("Excepcion al deserializar el savegame. Datos: " + e.Message);
		}
		finally {
			stream.Close();
		}
		return data;
	}
	
	public static int FileCount () {
		DirectoryInfo d = new DirectoryInfo(currentFilePath);
		FileInfo[] ret = d.GetFiles();
		int num = ret.GetLength(0); 
		return num; 
	}
	
	public static string[] getFileNames() {
		DirectoryInfo d = new DirectoryInfo(currentFilePath);
		FileInfo[] ret = d.GetFiles();
		int num = ret.GetLength(0);
		string[] str = new string[num];
		for (int i = 0; i < num; i++) {
			str[i] = ret[i].Name;
		}
		return str;
	}
	
	public static bool existeFile(string nombre) {
		if (File.Exists(currentFilePath + nombre))
            return true;
		else 
			return false;
	}
	
	public static void compruebaRuta() {
		if (!Directory.Exists(currentFilePath))	{
			Directory.CreateDirectory(currentFilePath);	
		}
	}
	
	public static void cambiaFileName(string nuevo) {
		currentFileName = nuevo;
	}

}

//Clase para tratar la textura y el tablero -----------------------------------------------------------------------------------------
public enum T_habitats {mountain, plain, hill, sand, volcanic, sea, coast};													//Tipos de orografía
public enum T_elementos {hidrogeno, helio, oxigeno, carbono, boro, nitrogeno, litio, silicio, magnesio, argon, potasio};	//Se pueden añadir mas mas adelante

public class FuncTablero {
	
	//Variables ----------------------------------------------------------------------------------------------------------------------
	
	//Privadas para uso del script
	private static int anchoTextura = 0;			//A cero inicialmente para detectar errores
	private static int altoTextura = 0;				//A cero inicialmente para detectar errores
	private static int relTexTabAncho;				//Que relación hay entre el ancho de la textura y el ancho del tablero lógico
	private static int relTexTabAlto;				//Lo mismo pero para el alto
	private static Perlin perlin;					//Semilla
	
	//Ruido
	private static int octavas	= 4;				//Octavas para la funcion de ruido de turbulencias
	private static int octavas2	= 12;				//Octavas para la funcion de ruido de turbulencias
	private static float lacunaridad	= 4.5f;			//La lacunaridad (cuanto se desplazan las coordenadas en sucesivas "octavas")
	private static float ganancia = 0.45f;			//El peso que se le da a cada nueva octava
	private static float escala = 0.004f;			//El nivel de zoom sobre el ruido
	
	//Terreno
	private static float nivelAgua = 0.15f;					//El nivel sobre el que se pondrá agua. La media de altura suele ser 0.4
	private static float tamanoPlaya = nivelAgua/4.0f;		//El tamaño de las playas
//	private static float atenuacionRelieve = 50f;			//Suaviza o acentua el efecto de sombreado
	private static float alturaColinas = 0.35f;				//La altura a partir de la cual se considera colina
	private static float alturaMontana = 0.6f;				//La altura a partir de la cual se considera montaña
	private static float temperatura = 0.0f;				//La temperatura del planeta, que influye en la rampa de color
	
	//Para el tablero
	public static int anchoTablero = 128;			//El ancho del tablero lógico (debe ser potencia de 2 para cuadrar con la textura)
	public static int altoTablero = 128;				//El alto del tablero lógico (debe ser potencia de 2 tambien)
	public static int casillasPolos	= 3;			//El numero de casillas que serán intransitables en los polos
	public static int numMaxEspecies = 20;			//Numero maximo de especies que puede haber en el tablero (juego) a la vez
	public static int margen = 50;					//El numero de pixeles que habrá en los polos intransitables
	public static int altoTableroUtil;				//El alto del tablero una vez eliminadas las casillas de los polos
	
		
	//Funciones --------------------------------------------------------------------------------------------------------------------
	
	public static float ruido_Turbulence(Vector2 coordsIn, int nOctavas, float lacunarity, float gain) {
		float ruidoTotal = 0.0f;
		float amplitud = gain;
		Vector2 coords = coordsIn;
		for (int i = 0; i < nOctavas; i++) {
			ruidoTotal += amplitud * Mathf.Abs(perlin.Noise(coords.x, coords.y));
			amplitud *= gain;
			coords.x *= lacunarity;
			coords.y *= lacunarity;
		}
		return ruidoTotal;
	}
	
	public static float calculaValorRuido(int i, int j){
			float valor,valor1,valor2,valor3;
			//el clasico
			valor1 = ruido_Turbulence(new Vector2(j, i) * escala, octavas, lacunaridad, ganancia);
			//los añadidos para darle mas relieve. seria como si valor 1 fuera la base y valor 2 y 3 calcularan las montañas, y se suman ambos.
			valor2 = ruido_Turbulence(new Vector2(i, j) * escala, octavas, lacunaridad, ganancia);
			valor3 = ruido_Turbulence(new Vector2(j, i) * escala/(valor2 + 1), octavas2, lacunaridad + valor2 * 2, ganancia * valor2 * 6);
			valor = valor1 + valor3;
			return valor;
	}
	
	public static Color[] ruidoTextura() {
		Color[] pixels = new Color[anchoTextura*altoTextura];
		float valor;
		for (int i = 0; i < altoTextura; i++) {
			for (int j = 0; j < anchoTextura; j++) {
				valor = calculaValorRuido(i,j);
				pixels[j + i*anchoTextura] = new Color(valor, valor, valor);
			}
		}
		return pixels;
	}
	
//	public static float calcularMedia(Color[] pix) {
//		float med = 0f;
//		float max = -1.0f;
//		float min = 1.0f;
//		for (int i = 0; i < pix.Length; i++) {
//			med += pix[i].r;
//			if (pix[i].r > max)
//				max = pix[i].r;
//			if (pix[i].r < min)
//				min = pix[i].r;
//		}
//		med /= pix.Length;
//		Debug.Log("Max = " + max.ToString());
//		Debug.Log("Min = " + min.ToString());
//		Debug.Log("Media = " + med.ToString());
//		return med;
//	}
	
	public static Color[] calculaTexAgua(Color[] pix) {
	/*Codigo de colores: 
	 * Negro: no extrusion
	 * tamañoPlaya?
	 * Rojo: agua poco profunda
	 * Punto medio - altura Costa
	 * Verde: oceano
	 * Azul: playa
	 * */
		float mezcla = tamanoPlaya/3.0f;
		Color[] pixAgua = new Color[anchoTextura * altoTextura];
		for (int l = 0; l < pixAgua.Length; l++) {
			float color = pix[l].grayscale;
			if (pix[l].grayscale < nivelAgua-tamanoPlaya-mezcla*2){
				pixAgua[l] = new Color (color, 1, 0, 0);
			} else if ((nivelAgua-tamanoPlaya-mezcla*2 <= color) && (color < nivelAgua-tamanoPlaya-mezcla*0.5 )){
				pixAgua[l] = new Color (0.25f+color, 0.75f-color, 0,  0);
			} else if ((nivelAgua-tamanoPlaya-mezcla*0.5 <= color) && (color < nivelAgua-tamanoPlaya-mezcla )){
				pixAgua[l] = new Color (0.5f+color, 0.5f-color, 0,  0);
			} else if ((nivelAgua-tamanoPlaya-mezcla <= color) && (color < nivelAgua-tamanoPlaya )){
				pixAgua[l] = new Color (0.75f+color, 0.25f-color, 0,  0);
			} else if ((nivelAgua-tamanoPlaya <= color) && (color < nivelAgua )){
				pixAgua[l] = new Color (1, 0, 0,  0);
			} else if ((nivelAgua<= color) && (color <nivelAgua+tamanoPlaya)){
				pixAgua[l] = new Color (0.5f+color, 0, 0.5f-color, 0);
			} else if ((nivelAgua+tamanoPlaya<= color)&& (color <nivelAgua+tamanoPlaya*4+mezcla)){	
				pixAgua[l] = new Color (0, 0, 1, 0);
			} else 
				pixAgua[l] = Color.black;
		}
		return pixAgua;
	}
	
	public static Color[] suavizaBordeTex(Color[] pix, int tam) {
		Color[] pixels = pix;
		int lado = tam;
		for (int i = 0; i < altoTextura; i++) {
			int j = anchoTextura;
			float pesoRuido = 1.0f;
			float pesoTextura = 0.0f;
			float iteraciones = pesoRuido / (float)lado;
			while (pesoTextura < 1.0) {
				pesoTextura += iteraciones;
				pesoRuido -= iteraciones;
				//float valorRuido = ruido_Turbulence(new Vector2(j, i) * escala, octavas, lacunaridad, ganancia);
				float valorRuido = calculaValorRuido(i,j);
				float valorBump = valorRuido * pesoRuido + (pixels[(i - 1) * anchoTextura + j].r) * pesoTextura;
				pixels[(i - 1)*anchoTextura + j] = new Color(valorBump, valorBump, valorBump);
				j++;
			}
		}
		return pixels;
	}
	
	public static Color[] suavizaPoloTex(Color[] pix, int tam) {
		Color[] pixels = pix;
		int lado = tam;
		
		//Se ponen los polos desde el origen hasta el margen (en pixeles) con la orografía deseada
		for (int i = 0; i < margen; i++) {
			for (int j = 0; j < anchoTextura; j++) {			
				pixels[j + anchoTextura*i] = new Color(0, 0, 0); 		//El valor nuevo de los polos
			}
		}
		for (int i = altoTextura - margen; i < altoTextura; i++) {
			for (int j = 0; j < anchoTextura; j++) {			
				pixels[j + anchoTextura*i] = new Color(0, 0, 0); 		//El valor nuevo de los polos 
			}
		}
		
		//Ahora se suaviza desde y hacia el margen
		for (int i = margen; i < margen + lado; i++) {
			for (int j = 0; j < anchoTextura; j++) {
				Color punto = pixels[j + anchoTextura*i];
				float valor = punto.r * (((float)i + 1.0f - (float)margen) / (float)lado);  			
				pixels[j + anchoTextura*i] = new Color(valor, valor, valor);					//El valor nuevo de los polos 
			}
		}
		int comienzo = altoTextura - (margen + lado);
		for (int i = comienzo; i < (altoTextura - margen); i++) {
			for (int j = 0; j < anchoTextura; j++) {	
				Color punto = pixels[j + anchoTextura*i];
				float valor = punto.r * (((float)lado - ((float)i - (float)comienzo)) / (float)lado);  
				pixels[j + anchoTextura*i] = new Color(valor, valor, valor); 						//El valor nuevo de los polos
			}
		}
		return pixels;
	}
	
	public static int safex(int x){
		if (x >= anchoTextura)
			return x - anchoTextura;
		if (x < 0)
			return anchoTextura + x;
		return x;
	}
	
	public static int safey(int y){
		if (y >= altoTextura)
			return y - altoTextura;
		if (y < 0)
			return altoTextura + y;
		return y;
	}
	
//	[Obsolete("No es necesario usar este metodo")]
//	public static Color[] realzarRelieve(Color[] pix, float media) {
//		Color[] pixels = pix;
//		for (int i = 0; i < pixels.Length; i++) {
//			float valor = pixels[i].r;
//			//Los valores por encima de la media * 2 seran maximos (0.85 sobre 1)
//			//y de ahi hacia abajo linealmente descendentes (hasta 0)
//			if (valor <= (media * 2.0f))
//				valor = Mathf.Lerp(0.0f, 0.65f, valor / (media * 2.0f));
//			else
//				valor = Mathf.Lerp(0.65f, 1.0f, valor / (media * 6.5f));
//			pixels[i] = new Color(valor, valor, valor);
//		}
//		return pixels;
//	}
	
//	[Obsolete("No es necesario usar este metodo")]
//	public static Color32[] creaNormalMap(Texture2D tex){
//		Color32[] pixels = tex.GetPixels32();
//		Color32[] pixelsN = new Color32[anchoTextura * altoTextura];
//		Color c3;
//		
//		for (int y = 0; y < altoTextura; y++) {
//	        int offset  = y * anchoTextura;
//	        for (int x = 0; x < anchoTextura; x++)
//	        {
//	
//	            float h0 = pixels[x + offset].r;
//	            float h1 = pixels[x + (anchoTextura * safey(y + 1))].r;
//	            float h2  = pixels[safex(x + 1) + offset].r;
//	
//	            float Nx = h0 - h2;
//	            float Ny = h0 - h1;
//				float Nz = atenuacionRelieve;
//	
//	            Vector3 normal = new Vector3(Nx,Ny,Nz);
//				normal.Normalize();
//	            normal /= 2;
//				
//	            byte cr = (byte)(128 + (255 * normal.x));
//	            byte cg = (byte)(128 + (255 * normal.y));
//	            byte cb = (byte)(128 + (255 * normal.z));
//				c3 = new Color32(cr, cg, cb, 128);
//	            
//				pixelsN[x + offset] = c3;
//	        }
//	    }
//	    return pixelsN;
//	}
	
	private static T_habitats[] calculaHabitats(Texture2D tex) {
		T_habitats[] habitats = new T_habitats[altoTableroUtil*anchoTablero];
		Color[] pixels = tex.GetPixels();
		
		//Se divide el espacio en zonas (en este caso 3 franjas horizontales)
		int limiteHab1 = altoTableroUtil / 3;
		int limiteHab2 = (altoTableroUtil / 3) * 2 + (altoTableroUtil % 3);
		
		/* TODO A la hora de hacer aleatorio un habitat, se me ocurre un problema posible.
		 * Una vez que empiece a haber una zona con un habitat, va a ser dificil que cambie a otro
		 * y si lo hace va a empezar a ser demasiado aleatorio y van a quedar zonas con habitats
		 * caóticos.
		 * 
		 * Para solucionarlo se me ocurre llevar unas variables de conteo que digan cuantos habitats
		 * de un tipo llevamos pintados seguidos. Una vez esta variable alcance cierto valor
		 * o cierta relación se cumpla, disminuir drásticamente las posibilidades de que vuelva a 
		 * aparecer este hábitat en las siguientes casillas a pintar.
		 * De nuevo cuando el valor llegue a otro punto controlado, se pueden invertir las tornas 
		 * o volver a los niveles originales.
		 * */
		
		//TODO Tambien falta meter la influencia de la temperatura del planeta.
		
		//Se calcula la primera franja de izquierda a derecha y de arriba a abajo
		//Esta franja representa la zona cercana al polo norte hasta el primer trópico
		for (int i = 0; i < limiteHab1; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				//Coordenadas de la casilla que estamos mirando
				Vector2 cord = new Vector2(j * relTexTabAncho , (i + casillasPolos) * relTexTabAlto);
				//Se calcula la media de altura de la casilla
				float media = 0;
				for (int x = 0; x < relTexTabAlto; x++) {
					for (int y = 0; y < relTexTabAncho; y++) {
						media += pixels[((int)cord.y + x) * anchoTextura + (int)cord.x + y].r;
					}
				}
				media = media / (relTexTabAncho * relTexTabAlto);
				
				//Se calcula el habitat en el que va a estar la casilla
				T_habitats habitatTemp;
				if (media < (nivelAgua - tamanoPlaya)) {
					habitatTemp = T_habitats.sea; 
				} 
				else if ((nivelAgua - tamanoPlaya <= media) && (media < nivelAgua)) {
					habitatTemp = T_habitats.coast;
				}
				else if ((nivelAgua <= media) && (media < alturaColinas)) {
					//TODO Meter aleatorio entre diferentes habitats a esta altura
					habitatTemp = T_habitats.plain;
				}
				else if ((alturaColinas <= media) && (media < alturaMontana)) {
					//TODO Meter aleatorio entre diferentes habitats a esta altura
					//Área de montaña. Habitats posibles: Mountain y Volcanic
					//Con un 20% es volcanico y con un 80% es montaña
					habitatTemp = T_habitats.hill;
				}
				else /*if (alturaMontana < media)*/ {
					//TODO Esto es una primera aproximación a lo que hay que hacer
					//Área de montaña. Habitats posibles: Mountain y Volcanic
					//Con un 10% es volcanico y con un 90% es montaña
					float posibilidad = 0.1f;
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					if (i > 0 && habitats[((i - 1) * anchoTablero) + j] == T_habitats.volcanic)
						posibilidad += 0.15f;
					if (j > 0) {
						if (i > 0 && habitats[((i - 1) * anchoTablero) + j - 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
						if (i > 0 && habitats[(i * anchoTablero) + j - 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
					}
					else {
						if (i > 0 && habitats[(i * anchoTablero) - 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
						if (i > 0 && habitats[(i * anchoTablero) - 2] == T_habitats.volcanic)
							posibilidad += 0.15f;
					}
					if (numero <= posibilidad)
						habitatTemp = T_habitats.volcanic;
					else
						habitatTemp = T_habitats.mountain;
				}
				habitats[i * anchoTablero + j] = habitatTemp;
			}
		}
		
		//public enum T_habitats {mountain, plain, hill, sand, volcanic, sea, coast};													//Tipos de orografía

		//Franja sur, que equivaldría como la primera a zonas mas cercanas a los polos
		//Se calcula de derecha a izquierda y de abajo a arriba
		for (int i = altoTableroUtil - 1; i >= limiteHab2; i--) {
			for (int j = anchoTablero - 1; j >= 0; j--) {
				//Coordenadas de la casilla que estamos mirando
				Vector2 cord = new Vector2(j * relTexTabAncho , (i + casillasPolos) * relTexTabAlto);
				//Se calcula la media de altura de la casilla
				float media = 0;
				for (int x = 0; x < relTexTabAlto; x++) {
					for (int y = 0; y < relTexTabAncho; y++) {
						media += pixels[((int)cord.y + x) * anchoTextura + (int)cord.x + y].r;
					}
				}
				media = media / (relTexTabAncho * relTexTabAlto);
				
				//Se calcula el habitat en el que va a estar la casilla
				T_habitats habitatTemp;
				if (media < (nivelAgua - tamanoPlaya)) {
					habitatTemp = T_habitats.sea; 
				} 
				else if ((nivelAgua - tamanoPlaya <= media) && (media < nivelAgua)) {
					habitatTemp = T_habitats.coast;
				}
				else if ((nivelAgua <= media) && (media < alturaColinas)) {
					//TODO Meter aleatorio entre diferentes habitats a esta altura
					habitatTemp = T_habitats.plain;
				}
				else if ((alturaColinas <= media) && (media < alturaMontana)) {
					//TODO Meter aleatorio entre diferentes habitats a esta altura
					//Área de montaña baja. Habitats posibles: Mountain y Volcanic
					//Con un 20% es volcanico y con un 80% es montaña
					//{mountain, plain, hill, sand, volcanic, sea, coast};
					habitatTemp = T_habitats.hill;
				}
				else /*if (alturaMontana < media)*/ {
					//TODO Esto es una primera aproximación a lo que hay que hacer
					//Área de montaña. Habitats posibles: Mountain y Volcanic
					//Con un 10% es volcanico y con un 90% es montaña
					float posibilidad = 0.1f;
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					//TODO Hay que mirar el habitat en las direcciones adecuadas
					if (i < (altoTableroUtil - 1) && habitats[((i + 1) * anchoTablero) + j] == T_habitats.volcanic)
						posibilidad += 0.15f;
					if (j < anchoTablero - 1) {
						if (i < (altoTableroUtil - 1) && habitats[((i + 1) * anchoTablero) + j + 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
						if (i < (altoTableroUtil - 1) && habitats[(i * anchoTablero) + j + 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
					}
					else {
						if (i < (altoTableroUtil - 1) && habitats[(i * anchoTablero) + 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
						if (i < (altoTableroUtil - 1) && habitats[(i * anchoTablero) + 2] == T_habitats.volcanic)
							posibilidad += 0.15f;
					}
					if (numero <= posibilidad)
						habitatTemp = T_habitats.volcanic;
					else
						habitatTemp = T_habitats.mountain;
				}
				habitats[i * anchoTablero + j] = habitatTemp;
			}
		}
		
		//Franja central (en esta hay mas posibilidad de areas tropicales o desiertos (mas calor)
		//TODO Sentido del cálculo???
		for (int i = limiteHab1; i < limiteHab2; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				//Coordenadas de la casilla que estamos mirando
				Vector2 cord = new Vector2(j * relTexTabAncho , (i + casillasPolos) * relTexTabAlto);
				//Se calcula la media de altura de la casilla
				float media = 0;
				for (int x = 0; x < relTexTabAlto; x++) {
					for (int y = 0; y < relTexTabAncho; y++) {
						media += pixels[((int)cord.y + x) * anchoTextura + (int)cord.x + y].r;
					}
				}
				media = media / (relTexTabAncho * relTexTabAlto);
				
				//Se calcula el habitat en el que va a estar la casilla
				T_habitats habitatTemp;
				if (media < (nivelAgua - tamanoPlaya)) {
					habitatTemp = T_habitats.sea; 
				} 
				else if ((nivelAgua - tamanoPlaya <= media) && (media < nivelAgua)) {
					habitatTemp = T_habitats.coast;
				}
				else if ((nivelAgua <= media) && (media < alturaColinas)) {
					//TODO Meter aleatorio entre diferentes habitats a esta altura
					habitatTemp = T_habitats.plain;
				}
				else if ((alturaColinas <= media) && (media < alturaMontana)) {
					//TODO Meter aleatorio entre diferentes habitats a esta altura
					//Área de montaña. Habitats posibles: Mountain y Volcanic
					//Con un 20% es volcanico y con un 80% es montaña
					habitatTemp = T_habitats.hill;
				}
				else /*if (alturaMontana < media)*/ {
					//TODO Esto es una primera aproximación a lo que hay que hacer
					//Área de montaña. Habitats posibles: Mountain y Volcanic
					//Con un 10% es volcanico y con un 90% es montaña
					float posibilidad = 0.1f;
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					if (i > 0 && habitats[((i - 1) * anchoTablero) + j] == T_habitats.volcanic)
						posibilidad += 0.15f;
					if (j > 0) {
						if (i > 0 && habitats[((i - 1) * anchoTablero) + j - 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
						if (i > 0 && habitats[(i * anchoTablero) + j - 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
					}
					else {
						if (i > 0 && habitats[(i * anchoTablero) - 1] == T_habitats.volcanic)
							posibilidad += 0.15f;
						if (i > 0 && habitats[(i * anchoTablero) - 2] == T_habitats.volcanic)
							posibilidad += 0.15f;
					}
					if (numero <= posibilidad)
						habitatTemp = T_habitats.volcanic;
					else
						habitatTemp = T_habitats.mountain;
				}
				habitats[i * anchoTablero + j] = habitatTemp;
			}
		}		
		
		return habitats;
	}
	
	public static Casilla[,] iniciaTablero(Texture2D tex, Mesh mesh) {
		Color[] pixels = tex.GetPixels();
		Casilla[,] tablero = new Casilla[altoTableroUtil,anchoTablero];
		Vector3[] vertices = mesh.vertices;
		Vector2[] uvs = mesh.uv;
		T_habitats[] habitat = calculaHabitats(tex);
		for (int i = 0; i < altoTableroUtil; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				//Las coordenadas de la casilla actual en la textura
				Vector2 cord = new Vector2(j * relTexTabAncho , (i + casillasPolos) * relTexTabAlto);
				Vector2 cord2 = new Vector2((j + 1) * relTexTabAncho, (i + 1 + casillasPolos) * relTexTabAlto);
				//Se calcula la coordenada del vértice a partir del UV
				int indice = -1;
				for (int k = 0; k < uvs.Length; k++) {
					Vector2 temp = uvs[k];
					temp.x *= tex.width;
					temp.y *= tex.height;
					if (temp.x >= cord.x && temp.y >= cord.y && temp.x <= cord2.x && temp.y <= cord2.y) {
						indice = k;
						break;
					}
				}
				if (indice == -1)
					Debug.LogError("No se ha encontrado la coordenada por su UV en iniciaTablero(). Casilla: " + i + "," + j + ";");
				
				//Parche hasta que lo introduzcamos
				T_elementos[] elems = new T_elementos[1];
				elems[0] = T_elementos.carbono;
				
				tablero[i,j] = new Casilla(habitat[i * anchoTablero + j], elems, cord, vertices[indice]);
			}
		}
		return tablero;
	}
	
	public static void convierteCoordenadas(ref int x,ref int y)
	{
		x %= altoTableroUtil;
		while(x < 0)
			x += altoTableroUtil;
		y %= anchoTablero;
		while(y < 0)
			y += anchoTablero;		
	}
	
	public static void alteraPixel(Texture2D tex, int w, int h, float valor){
		if (w < 0 || h < 0 || h > tex.height) {
			Debug.LogError("Error en alteraPixel: los limites de la textura se sobrepasan. w = " + w + " h = " + h);
		}
		Color pix = tex.GetPixel(w,h);
		pix.r = pix.r + valor;
		pix.g = pix.g + valor;
		pix.b = pix.b + valor;
		tex.SetPixel(w,h,pix);
	}
	
	public static void alteraPixelColor(Texture2D tex, int w, int h, Color valor, bool positivo){
		if (h < 0 || h > tex.height) {
			Debug.LogError("Error en alteraPixel: los limites de la textura se sobrepasan. w = " + w + " h = " + h);
		}
		if (w < 0)
			w += tex.width;
		w = w % tex.width;
		Color pix = tex.GetPixel(w,h);
		if (positivo)
			pix += valor;
		else
			pix -= valor;
		tex.SetPixel(w,h,pix);
	}
	
//	public static void fisura(Texture2D tex, int posX, int posY, float longitud, float magnitud, Vector2 dir) {
//		float posActual = Math.Abs(longitud);
//		for (int i = 0; i < (int)Math.Abs(longitud) * 2; i++) {
//			Vector2 pixel = new Vector2(posX + (int)(dir.x * posActual), posY + (int)(dir.y * posActual));
//			alteraPixel(tex, (int)pixel.x, (int)pixel.y, magnitud);
//			posActual -= 1.0f;
//		}
//	}
	
	
	
	public static void pintaPlantas(Texture2D objetivo, Vector2 coords, int idColor, bool positivo) {
		Pinceles temp = GameObject.FindGameObjectWithTag("Pinceles").GetComponent<Pinceles>();
		Texture2D pincelTex = temp.pincelPlantas;		
		int w = pincelTex.width;
		int h = pincelTex.height;
		Vector2 pos = coords;
		pos.x -= w/2;
		pos.y -= h/2;
		if (pos.y < 0)
			pos.y = 0;
		if (pos.y >= objetivo.height)
			pos.y = objetivo.height - 1;
		Color colorObjetivo;
		switch (idColor) {
		case 0: 
			colorObjetivo = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			break;
		case 1:
			colorObjetivo = new Color(0.0f, 1.0f, 0.0f, 0.0f);
			break;
		case 2:
			colorObjetivo = new Color(0.0f, 0.0f, 1.0f, 0.0f);
			break;
		case 3:
			colorObjetivo = new Color(0.0f, 0.0f, 0.0f, 1.0f);
			break;
		default:
			colorObjetivo = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			break;
		}
		Color colorTemp = colorObjetivo;
		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				colorTemp = colorObjetivo;
				colorTemp *= pincelTex.GetPixel(i,j);
					alteraPixelColor(objetivo,(int)pos.x + i,(int)pos.y + j, colorTemp, positivo);
			}
		}
	}
	
	public static void pintaPincel(RaycastHit hit, int pincel, bool subir) {
		Pinceles temp = GameObject.FindGameObjectWithTag("Pinceles").GetComponent<Pinceles>();
		Texture2D objetivo = (Texture2D)hit.collider.renderer.sharedMaterial.mainTexture;
		Texture2D pincelTex;
		switch (pincel) {
		case 0: 
			pincelTex = temp.crater;
			break;
		case 1:
			pincelTex = temp.volcan;
			break;
		case 2:
			pincelTex = temp.pincelDuro;
			break;
		case 3: 
			pincelTex = temp.pincelRegular;
			break;
		case 4:			
			pincelTex = temp.pincelIrregular;
			break;
		default: 
			pincelTex = temp.pincelRegular;
			break;			
		}		
		int w = pincelTex.width;
		int h = pincelTex.height;
		Vector2 pos = hit.textureCoord;
		pos.x *= objetivo.width;
		pos.y *= objetivo.height;
		pos.x -= w/2;
		pos.y -= h/2;
		if (pos.y < 0)
			pos.y = 0;
		if (pos.y >= objetivo.height)
			pos.y = objetivo.height - 1;
		int multi = -1;
		if (subir)
			multi = 1;
		float atenuacion = 0.05f;
		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				alteraPixel(objetivo,(int)pos.x + i,(int)pos.y + j, multi * ((pincelTex.GetPixel(i,j).r) * atenuacion));
			}
		}
	}
	
	public static GameObject creaMesh(Vector3 posicion, GameObject mesh) {
		GameObject creacion = GameObject.Instantiate(mesh) as GameObject;
		creacion.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		creacion.transform.parent = GameObject.FindGameObjectWithTag("Planeta").transform;
		creacion.transform.position = posicion;
		Vector3 normal = posicion - creacion.transform.parent.position;
		creacion.transform.rotation = Quaternion.LookRotation(normal);
		//creacion.transform.Rotate(0,0,UnityEngine.Random.Range(0,180),Space.Self);

		return creacion;
	}
	
	public static Mesh extruyeVertices(Mesh mesh, Texture2D tex, float extrusion, Vector3 centro) {
		Vector3[] verts = mesh.vertices;
		Vector3[] vertsRes = new Vector3[verts.Length];
		for (int i = 0; i < verts.Length; i++) {
			Vector2 cord = new Vector2(mesh.uv[i].x, mesh.uv[i].y);
			cord.x *= tex.width;
			cord.y *= tex.height;
			Color col = tex.GetPixel((int)cord.x, (int)cord.y);
			Vector3 normal = verts[i] - centro;
			Vector3 desplazamiento = (extrusion/512 * normal.normalized * col.grayscale);
			vertsRes[i] = verts[i] + desplazamiento;	
		}
		
		Mesh resultado = GameObject.Instantiate(mesh) as Mesh;
		resultado.vertices = vertsRes;
		return resultado;
	}
	
	public static void inicializa(Texture2D tex) {
		anchoTextura = tex.width;
		altoTextura = tex.height;
		altoTableroUtil = altoTablero - casillasPolos * 2;
		relTexTabAncho = anchoTextura / anchoTablero;
		relTexTabAlto = altoTextura / altoTablero;
		margen = relTexTabAlto * casillasPolos;	
		perlin = new Perlin();
	}
	
	public static void reiniciaPerlin() {
		perlin = new Perlin();
	}
	
	//Ordena aleatoriamente una lista
	public static void randomLista<T>(IList<T> lista)
	{
		System.Random random = new System.Random();
		int pos;		
		T aux;
		for(int i = lista.Count; i > 1; i--)
		{
			pos = random.Next(i);
			aux = lista[i-1];
			lista[i-1] = lista[pos];
			lista[pos] = aux;
		}			
	}	
		
	//Getters y setters -------------------------------------
	
	public static void setOctavas(int entrada) {
		if (entrada >= 0)
			octavas = entrada;
	}
	
	public static void setOctavas2(int entrada) {
		if (entrada >= 0)
			octavas2 = entrada;
	}
	
	public static void setLacunaridad(float entrada) {
		if (entrada >= 0)
			lacunaridad = entrada;
	}
	
	public static void setGanancia(float entrada) {
		ganancia = entrada;
	}
	
	public static void setEscala(float entrada) {
		if (entrada >= 0)
			escala = entrada;
	}
	
	public static void setNivelAgua(float entrada) {
		if (entrada >= 0.0f && entrada <= 1.0f)
			nivelAgua = entrada;
	}
	
	public static void setTamanoPlaya(float entrada) {
		if (entrada >= 0)
			tamanoPlaya = entrada;
	}
	
	public static void setAlturaColinas(float entrada) {
		if (entrada >= 0.0f && entrada <= 1.0f)
			alturaColinas = entrada;
	}
	
	public static void setAlturaMontana(float entrada) {
		if (entrada >= 0.0f && entrada <= 1.0f)
			alturaMontana = entrada;
	}
	
//	public static void setAtenuacionRelieve(float entrada) {
//		if (entrada >= 0)
//			atenuacionRelieve = entrada;
//	}
	
	public static void setTemperatura(float entrada) {
		if (entrada >= 0.0f && entrada <= 1.0f)
			temperatura = entrada;
	}
	
	public static int getRelTexTabAncho() {
		return relTexTabAncho;
	}
	
	public static int getRelTexTabAlto() {
		return relTexTabAlto;
	}
	
}