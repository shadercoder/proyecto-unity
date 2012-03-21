using UnityEngine;    // For Debug.Log, etc.

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;


//Clase contenedora del savegame ------------------------------------------------------------------------------------------------------
[System.Serializable]
public class SaveData {//: ISerializable { 

  // === Values ===
  // Edit these during gameplay
//  	public Texture2D normalMap;
	public int width;
	public int height;
	public float[] data;
//	[SerializeField]
//  	private Color[] normalMapPixels;
  	// === /Values ===

  	// The default constructor. Included for when we call it during Save() and Load()
  	public SaveData () {}

  	// This constructor is called automatically by the parent class, ISerializable
  	// We get to custom-implement the serialization process here
//	[SecurityPermissionAttribute (SecurityAction.Demand, SerializationFormatter = true)]
//  	public SaveData (SerializationInfo info, StreamingContext ctxt)
//  	{
//    	// Get the values from info and assign them to the appropriate properties. Make sure to cast each variable.
//    	// Do this for each var defined in the Values section above
//		try {
////    		normalMap = (Texture2D)info.GetValue("normalMap", typeof(Texture2D));
//    		normalMapPixels = (Color[])info.GetValue("normalMapPixels", typeof(Color[]));
//		}
//		catch (InvalidCastException e) {
//			Debug.LogError("Excepcion de casting al recuperar el valor del savegame. Datos: " + e.Message);
//		}
//  	}
//
//  	// Required by the ISerializable class to be properly serialized. This is called automatically
////	[SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
//	public void GetObjectData (SerializationInfo info, StreamingContext ctxt)
//  	{
//    	// Repeat this for each var defined in the Values section
////    	info.AddValue("normalMap", normalMap, typeof(Texture2D));
//    	info.AddValue("normalMapPixels", normalMapPixels, typeof(Color[]));
//  	}
	
//	public void setNormalMapPixels(Color[] col) {
//		normalMapPixels = col;	
//	}
//	
//	public Color[] getNormalMapPixels() {
//		return normalMapPixels;	
//	}
}

//Clase con los métodos a llamar para crear el savegame -------------------------------------------------------------------------------
public class SaveLoad {
	
	public static string currentFileName = "SaveGame.hur";						// Edit this for different save files
	public static string currentFilePath = Application.persistentDataPath + "/Saves/";

  	// Call this to write data
  	public static void Save (Texture2D norm)  		// Overloaded
  	{
    	Save (currentFilePath + currentFileName, norm);
  	}
  	public static void Save (string filePath, Texture2D norm)
  	{ 
	    SaveData data = new SaveData ();
		int tempLong = norm.width * norm.height;
		data.data = new float[tempLong];
		data.width = norm.width;
		data.height = norm.height;
		Color[] pixels = norm.GetPixels();
		for (int i = 0; i < tempLong; i++) {
			data.data[i] = pixels[i].r;
		}		
//		data.normalMap = new Texture2D(norm.width, norm.height);
//		data.normalMap.SetPixels(norm.GetPixels());
//		data.normalMap.Apply();
//		data.normalMap = norm;
//		data.setNormalMapPixels(norm.GetPixels());
//		data.normalMapPixels = norm.GetPixels();
	    FileStream stream = new FileStream(filePath, FileMode.Create);
		try {
		    BinaryFormatter bformatter = new BinaryFormatter();
	//	    bformatter.Binder = new VersionDeserializationBinder(); 
		    bformatter.Serialize(stream, data);
		}
		catch (SerializationException e) {
			Debug.LogError("Excepcion al serializar el savegame. Datos: " + e.Message);
		}
		finally {
	    	stream.Close();
		}
  	}

  	// Call this to load from a file into "data"
  	public static SaveData Load ()  {				// Overloaded
		return Load(currentFilePath + currentFileName);
	}   
  	public static SaveData Load(string filePath) 
  	{
	    SaveData data = new SaveData ();
	    FileStream stream = new FileStream(filePath, FileMode.Open);
		try {
		    BinaryFormatter bformatter = new BinaryFormatter();
//		    bformatter.Binder = new VersionDeserializationBinder(); 
		    data = (SaveData)bformatter.Deserialize(stream);
		}
		catch (SerializationException e) {
			Debug.LogError("Excepcion al deserializar el savegame. Datos: " + e.Message);
		}
		finally {
			stream.Close();
		}
		Debug.Log("Type of object deserialized: " + data.GetType());
//        Debug.Log("normalMap = " + data.normalMap);
		Debug.Log("Width = " + data.width);
		Debug.Log("Height = " + data.height);
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

// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
// Do not change this
//public sealed class VersionDeserializationBinder : SerializationBinder 
//{ 
//    public override Type BindToType( string assemblyName, string typeName )
//    { 
//        if ( !string.IsNullOrEmpty( assemblyName ) && !string.IsNullOrEmpty( typeName ) ) 
//        { 
//            Type typeToDeserialize = null; 
//
//            assemblyName = Assembly.GetExecutingAssembly().FullName; 
//
//            // The following line of code returns the type. 
//            typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) ); 
//
//            return typeToDeserialize; 
//        } 
//
//        return null; 
//    } 
//}

//Clase para tratar la textura y el tablero -----------------------------------------------------------------------------------------
public enum T_habitats {mountain, plain, hill, sand, volcanic, sea, coast};													//Tipos de orografía
public enum T_elementos {hidrogeno, helio, oxigeno, carbono, boro, nitrogeno, litio, silicio, magnesio, argon, potasio};	//Se pueden añadir mas mas adelante

public class Especie {
	//A rellenar
}

public class Casilla {
	public float altura;
	public T_habitats habitat;
	public T_elementos[] elementos;
	public Especie[] especies;
	public Vector2 coordsTex;
	
	public Casilla(float alt, T_habitats hab, T_elementos[] elems, Especie[] esp, Vector2 coord) {
		habitat = hab;
		altura = alt;
		elementos = elems;
		especies = esp;
		coordsTex = coord;
	}
}

public class FuncTablero {
	
	//Variables ----------------------------------------------------------------------------------------------------------------------
	//Privadas para uso del script
	private static int anchoTextura = 0;			//A cero inicialmente para detectar errores
	private static int altoTextura = 0;				//A cero inicialmente para detectar errores
	private static int relTexTabAncho;				//Que relación hay entre el ancho de la textura y el ancho del tablero lógico
	private static int relTexTabAlto;				//Lo mismo pero para el alto
	private static Perlin perlin;					//Semilla
	
	//Ruido
	public static int octavas	= 4;				//Octavas para la funcion de ruido de turbulencias
	public static int octavas2	= 12;				//Octavas para la funcion de ruido de turbulencias
	public static float lacunaridad	= 4.5f;			//La lacunaridad (cuanto se desplazan las coordenadas en sucesivas "octavas")
	public static float ganancia = 0.45f;			//El peso que se le da a cada nueva octava
	public static float escala = 0.004f;			//El nivel de zoom sobre el ruido
	
	//Terreno
	public static float nivelAgua = 0.1f;			//El nivel sobre el que se pondrá agua. La media de altura suele ser 0.4
	public static float tamanoPlaya = 0.01f;		//El tamaño de las playas
	public static float atenuacionRelieve = 20f;	//Suaviza o acentua el efecto de sombreado
	public static float alturaColinas = 0.15f;		//La altura a partir de la cual se considera colina
	public static float alturaMontana = 0.2f;		//La altura a partir de la cual se considera montaña
	public static float temperatura = 0.0f;			//La temperatura del planeta, que influye en la rampa de color
	
	//Para el tablero
	public static int anchoTablero = 128;			//El ancho del tablero lógico (debe ser potencia de 2 para cuadrar con la textura)
	public static int altoTablero = 128;			//El alto del tablero lógico (debe ser potencia de 2 tambien)
	public static int casillasPolos	= 3;			//El numero de casillas que serán intransitables en los polos
	public static int numMaxEspeciesCasilla	= 5;	//Numero maximo de especies que puede haber por casilla a la vez
	public static int numMaxEspecies = 20;			//Numero maximo de especies que puede haber en el tablero (juego) a la vez
	public static int margen = 50;					//El numero de pixeles que habrá en los polos intransitables

	private static int altoTableroUtil;				//El alto del tablero una vez eliminadas las casillas de los polos
	
	
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
	
	public static float calcularMedia(Color[] pix) {
		float med = 0f;
		float max = -1.0f;
		float min = 1.0f;
		for (int i = 0; i < pix.Length; i++) {
			med += pix[i].r;
			if (pix[i].r > max)
				max = pix[i].r;
			if (pix[i].r < min)
				min = pix[i].r;
		}
		med /= pix.Length;
		Debug.Log("Max = " + max.ToString());
		Debug.Log("Min = " + min.ToString());
		Debug.Log("Media = " + med.ToString());
		return med;
	}
	
	public static Color[] mascaraBumpAgua(Color[] pixBump, float media) {
		Color[] pixAgua = new Color[anchoTextura * altoTextura];
		for (int l = 0; l < pixAgua.Length; l++) {
			if (pixBump[l].r < media){
				//pixBump[l] = new Color(media,media,media);
				pixAgua[l] = new Color(0,0,0);
			}
			else 
				pixAgua[l] = new Color(1,1,1);			
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
	
	public static Color[] realzarRelieve(Color[] pix, float media) {
		Color[] pixels = pix;
		for (int i = 0; i < pixels.Length; i++) {
			float valor = pixels[i].r;
			//Los valores por encima de la media * 2 seran maximos (0.85 sobre 1)
			//y de ahi hacia abajo linealmente descendentes (hasta 0)
			if (valor <= (media * 2.0f))
				valor = Mathf.Lerp(0.0f, 0.65f, valor / (media * 2.0f));
			else
				valor = Mathf.Lerp(0.65f, 1.0f, valor / (media * 6.5f));
			pixels[i] = new Color(valor, valor, valor);
		}
		return pixels;
	}
	
	public static Color32[] creaNormalMap(Texture2D tex){
		Color32[] pixels = tex.GetPixels32();
		Color32[] pixelsN = new Color32[anchoTextura * altoTextura];
		Color c3;
		
		for (int y = 0; y < altoTextura; y++) {
	        int offset  = y * anchoTextura;
	        for (int x = 0; x < anchoTextura; x++)
	        {
	
	            float h0 = pixels[x + offset].r;
	            float h1 = pixels[x + (anchoTextura * safey(y + 1))].r;
	            float h2  = pixels[safex(x + 1) + offset].r;
	
	            float Nx = h0 - h2;
	            float Ny = h0 - h1;
				float Nz = atenuacionRelieve;
	
	            Vector3 normal = new Vector3(Nx,Ny,Nz);
				normal.Normalize();
	            normal /= 2;
				
	            byte cr = (byte)(128 + (255 * normal.x));
	            byte cg = (byte)(128 + (255 * normal.y));
	            byte cb = (byte)(128 + (255 * normal.z));
				c3 = new Color32(cr, cg, cb, 128);
	            
				pixelsN[x + offset] = c3;
	        }
	    }
	    return pixelsN;
	}
	
	public static Casilla[,] iniciaTablero(Texture2D tex) {
		Color[] pixels = tex.GetPixels();
		Casilla[,] tablero = new Casilla[altoTableroUtil,anchoTablero];
		for (int i = 0; i < altoTableroUtil; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				//Las coordenadas de la casilla actual en la textura
				Vector2 cord = new Vector2(j * relTexTabAncho , (i + casillasPolos) * relTexTabAlto);
				
				//Se calcula la media de altura de la casilla
				float media = 0;
				for (int x = 0; x < relTexTabAlto; x++) {
					for (int y = 0; y < relTexTabAncho; y++) {
						media += pixels[((int)cord.y + x) * anchoTextura + (int)cord.x + y].r;
					}
				}
				media = media / (relTexTabAncho * relTexTabAlto);
				
				//Se calcula el habitat en el que va a estar la casilla y los elementos que tendrá
				//TODO Esto es un ejemplo a refinar de los elementos a introducir...
				T_elementos[] elems = new T_elementos[5];
				//Calcular el habitat...
				T_habitats habitat;
				if (media < (nivelAgua - (tamanoPlaya * 1.2))) {
					habitat = T_habitats.sea;
				} 
				else if (((nivelAgua - (tamanoPlaya * 1.2)) <= media) && (media < (nivelAgua + (tamanoPlaya * 1.2)))) {
					habitat = T_habitats.coast;
				}
				else if (((nivelAgua + (tamanoPlaya * 1.2)) <= media) && (media < alturaColinas)) {
					habitat = T_habitats.plain;
				}
				else if ((alturaColinas <= media) && (media < alturaMontana)) {
					habitat = T_habitats.hill;
				}
				else /*if (alturaMontana < media)*/ {
					habitat = T_habitats.mountain;
				}
				
				//TODO Se coge una o varias especies aleatorias de las iniciales
				Especie[] esp = new Especie[numMaxEspeciesCasilla];
				//TODO Calculos para ver la especie/s a meter
				
				tablero[i,j] = new Casilla(media, habitat, elems, esp, cord);
			}
		}
		return tablero;
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
	
	public static void setAtenuacionRelieve(float entrada) {
		if (entrada >= 0)
			atenuacionRelieve = entrada;
	}
	
	public static void setTemperatura(float entrada) {
		if (entrada >= 0.0f && entrada <= 1.0f)
			temperatura = entrada;
	}
	
}