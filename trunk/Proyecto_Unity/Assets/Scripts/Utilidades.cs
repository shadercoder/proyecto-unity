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
	public int heightmapW;
	public int heightmapH;
	public float[] heightmapData;
	public int elementosW;
	public int elementosH;
	public float[] elementosData;
	public int plantasW;
	public int plantasH;
	public float[] plantasData;
	public int habitatsW;
	public int habitatsH;
	public float[] habitatsData;
	public int esteticaW;
	public int esteticaH;
	public float[] esteticaData;
	public Vida vidaData;
	public float[] rocaVertices;
	public float[] rocaNormals;
	public float[] rocaUVs;
	public int[] rocaTriangulos;
	public float[] aguaVertices;
	public float[] aguaNormals;
	public float[] aguaUVs;
	public int[] aguaTriangulos;
	public float nivelAgua;
	public float tamanoPlaya;


  	public SaveData () {}

}

//Clase contenedora del archivo de indices ------------------------------------------------------------------------------------------------------
[System.Serializable]
public class SaveIndices {

	//Variables a salvar
	public int[] indices;

  	public SaveIndices() {}

}

//Clase con los métodos a llamar para crear el savegame y otros archivos ---------------------------------------------------------------
public class SaveLoad {
	
	public static string currentFileName = "SaveGame.hur";						
	public static string currentFilePath = Application.persistentDataPath + "/Saves/";
	
	//Savegame
	public static void Save (ValoresCarga contenedor) {					//Sobrecargado
		Save(currentFilePath + currentFileName, contenedor);
	}
	
  	public static void Save (string filePath, ValoresCarga contenedor)
  	{ 
	    SaveData save = generarValores(contenedor);
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
	
	private static SaveData generarValores(ValoresCarga contenedor) {
		SaveData resultado = new SaveData();
		//Textura Heightmap (Textura_Planeta)
		resultado.heightmapW = contenedor.texturaBase.width;
		resultado.heightmapH = contenedor.texturaBase.height;
		Color[] temp1 = contenedor.texturaBase.GetPixels();
		resultado.heightmapData = new float[temp1.Length];
		for (int i = 0; i < temp1.Length; i++) {
			resultado.heightmapData[i] = temp1[i].r;
		}
		//Textura elementos (Textura_Recursos)
		resultado.elementosW = contenedor.texturaElementos.width;
		resultado.elementosH = contenedor.texturaElementos.height;
		Color[] temp2 = contenedor.texturaElementos.GetPixels();
		resultado.elementosData = new float[temp2.Length * 4];
		for (int i = 0; i < temp2.Length; i++) {
			resultado.elementosData[i * 4] = temp2[i].r;
			resultado.elementosData[i * 4 + 1] = temp2[i].g;
			resultado.elementosData[i * 4 + 2] = temp2[i].b;
			resultado.elementosData[i * 4 + 3] = temp2[i].a;
		}
		//Textura plantas (Textura_planta)
		resultado.plantasW = contenedor.texturaPlantas.width;
		resultado.plantasH = contenedor.texturaPlantas.height;
		Color[] temp3 = contenedor.texturaPlantas.GetPixels();
		resultado.plantasData = new float[temp3.Length * 4];
		for (int i = 0; i < temp3.Length; i++) {
			resultado.plantasData[i * 4] = temp3[i].r;
			resultado.plantasData[i * 4 + 1] = temp3[i].g;
			resultado.plantasData[i * 4 + 2] = temp3[i].b;
			resultado.plantasData[i * 4 + 3] = temp3[i].a;
		}
		//Textura habitats (Textura_Habitats)
		resultado.habitatsW = contenedor.texturaHabitats.width;
		resultado.habitatsH = contenedor.texturaHabitats.height;
		Color[] temp4 = contenedor.texturaHabitats.GetPixels();
		resultado.habitatsData = new float[temp4.Length * 4];
		for (int i = 0; i < temp4.Length; i++) {
			resultado.habitatsData[i * 4] = temp4[i].r;
			resultado.habitatsData[i * 4 + 1] = temp4[i].g;
			resultado.habitatsData[i * 4 + 2] = temp4[i].b;
			resultado.habitatsData[i * 4 + 3] = temp4[i].a;
		}
		//Textura habitats estetica (Textura_Habitats_Estetica)
		resultado.esteticaW = contenedor.texturaHabsEstetica.width;
		resultado.esteticaH = contenedor.texturaHabsEstetica.height;
		Color[] temp5 = contenedor.texturaHabsEstetica.GetPixels();
		resultado.esteticaData = new float[temp5.Length * 4];
		for (int i = 0; i < temp5.Length; i++) {
			resultado.esteticaData[i * 4] = temp5[i].r;
			resultado.esteticaData[i * 4 + 1] = temp5[i].g;
			resultado.esteticaData[i * 4 + 2] = temp5[i].b;
			resultado.esteticaData[i * 4 + 3] = temp5[i].a;
		}
		//Clase Vida
		resultado.vidaData = contenedor.vida;
		//Mesh Roca
		Vector3[] temp6 = contenedor.roca.vertices;
		Vector3[] temp7 = contenedor.roca.normals;
		resultado.rocaVertices = new float[temp6.Length * 3];
		resultado.rocaNormals = new float[temp7.Length * 3];
		for (int i = 0; i < temp6.Length; i++) {
			resultado.rocaVertices[i * 3] = temp6[i].x;
			resultado.rocaVertices[i * 3 + 1] = temp6[i].y;
			resultado.rocaVertices[i * 3 + 2] = temp6[i].z;
			resultado.rocaNormals[i * 3] = temp7[i].x;
			resultado.rocaNormals[i * 3 + 1] = temp7[i].y;
			resultado.rocaNormals[i * 3 + 2] = temp7[i].z;
		}
		Vector2[] temp8 = contenedor.roca.uv;
		resultado.rocaUVs = new float[temp8.Length * 2];
		for (int i = 0; i < temp8.Length; i++) {
			resultado.rocaUVs[i * 2] = temp8[i].x;
			resultado.rocaUVs[i * 2 + 1] = temp8[i].y;
		}
		int[] temp9 = contenedor.roca.triangles;
		resultado.rocaTriangulos = new int[temp9.Length];
		for (int i = 0; i < temp9.Length; i++) {
			resultado.rocaTriangulos[i] = temp9[i];
		}
		//Mesh Agua
		Vector3[] temp10 = contenedor.agua.vertices;
		Vector3[] temp11 = contenedor.agua.normals;
		resultado.aguaVertices = new float[temp10.Length * 3];
		resultado.aguaNormals = new float[temp11.Length * 3];
		for (int i = 0; i < temp10.Length; i++) {
			resultado.aguaVertices[i * 3] = temp10[i].x;
			resultado.aguaVertices[i * 3 + 1] = temp10[i].y;
			resultado.aguaVertices[i * 3 + 2] = temp10[i].z;
			resultado.aguaNormals[i * 3] = temp11[i].x;
			resultado.aguaNormals[i * 3 + 1] = temp11[i].y;
			resultado.aguaNormals[i * 3 + 2] = temp11[i].z;
		}
		Vector2[] temp12 = contenedor.agua.uv;
		resultado.aguaUVs = new float[temp12.Length * 2];
		for (int i = 0; i < temp12.Length; i++) {
			resultado.aguaUVs[i * 2] = temp12[i].x;
			resultado.aguaUVs[i * 2 + 1] = temp12[i].y;
		}
		int[] temp13 = contenedor.agua.triangles;
		resultado.aguaTriangulos = new int[temp13.Length];
		for (int i = 0; i < temp13.Length; i++) {
			resultado.aguaTriangulos[i] = temp13[i];
		}
		//Otras variables
		resultado.nivelAgua = contenedor.nivelAgua;
		resultado.tamanoPlaya = contenedor.tamanoPlaya;
		//Fin
		return resultado;
	}
	
	public static void rehacerScript(SaveData save, ref ValoresCarga contenedor) {
		//Textura Heightmap (Textura_Planeta)
		Texture2D temp1 = new Texture2D(save.heightmapW, save.heightmapH);
		Color[] temp1a = new Color[save.heightmapData.Length];
		for (int i = 0; i < save.heightmapData.Length; i++) {
			temp1a[i].r = save.heightmapData[i];
			temp1a[i].g = save.heightmapData[i];
			temp1a[i].b = save.heightmapData[i];
		}
		temp1.SetPixels(temp1a);
		temp1.Apply();
		contenedor.texturaBase = temp1;
		//Textura elementos (Textura_Recursos)
		Texture2D temp2 = new Texture2D(save.elementosW, save.elementosH);
		Color[] temp2a = new Color[save.elementosData.Length / 4];
		for (int i = 0; i < temp2a.Length; i++) {
			temp2a[i].r = save.elementosData[i * 4];
			temp2a[i].g = save.elementosData[i * 4 + 1];
			temp2a[i].b = save.elementosData[i * 4 + 2];
			temp2a[i].a = save.elementosData[i * 4 + 3];
		}
		temp2.SetPixels(temp2a);
		temp2.Apply();
		contenedor.texturaElementos = temp2;
		//Textura plantas (Textura_planta)
		Texture2D temp3 = new Texture2D(save.plantasW, save.plantasH);
		Color[] temp3a = new Color[save.plantasData.Length / 4];
		for (int i = 0; i < temp3a.Length; i++) {
			temp3a[i].r = save.plantasData[i * 4];
			temp3a[i].g = save.plantasData[i * 4 + 1];
			temp3a[i].b = save.plantasData[i * 4 + 2];
			temp3a[i].a = save.plantasData[i * 4 + 3];
		}
		temp3.SetPixels(temp3a);
		temp3.Apply();
		contenedor.texturaPlantas = temp3;
		//Textura habitats (Textura_Habitats)		
		Texture2D temp4 = new Texture2D(save.habitatsW, save.habitatsH);
		Color[] temp4a = new Color[save.habitatsData.Length / 4];
		for (int i = 0; i < temp4a.Length; i++) {
			temp4a[i].r = save.plantasData[i * 4];
			temp4a[i].g = save.plantasData[i * 4 + 1];
			temp4a[i].b = save.plantasData[i * 4 + 2];
			temp4a[i].a = save.plantasData[i * 4 + 3];
		}
		temp4.SetPixels(temp4a);
		temp4.Apply();
		contenedor.texturaHabitats = temp4;
		//Textura habitats estetica (Textura_Habitats_Estetica)		
		Texture2D temp5 = new Texture2D(save.esteticaW, save.esteticaH);
		Color[] temp5a = new Color[save.esteticaData.Length / 4];
		for (int i = 0; i < temp5a.Length; i++) {
			temp5a[i].r = save.esteticaData[i * 4];
			temp5a[i].g = save.esteticaData[i * 4 + 1];
			temp5a[i].b = save.esteticaData[i * 4 + 2];
			temp5a[i].a = save.esteticaData[i * 4 + 3];
		}
		temp5.SetPixels(temp5a);
		temp5.Apply();
		contenedor.texturaHabsEstetica = temp5;
		//Clase Vida
		contenedor.vida = save.vidaData;
		//Mesh Roca
		Mesh temp6 = new Mesh();
		Vector3[] temp6v = new Vector3[save.rocaVertices.Length / 3];
		Vector3[] temp6n = new Vector3[save.rocaNormals.Length / 3];
		for (int i = 0; i < temp6v.Length; i++) {
			temp6v[i].x = save.rocaVertices[i * 3];
			temp6v[i].y = save.rocaVertices[i * 3 + 1];
			temp6v[i].z = save.rocaVertices[i * 3 + 2];
			temp6n[i].x = save.rocaNormals[i * 3];
			temp6n[i].y = save.rocaNormals[i * 3 + 1];
			temp6n[i].z = save.rocaNormals[i * 3 + 2];
		}
		temp6.vertices = temp6v;
		temp6.normals = temp6n;
		Vector2[] temp6u = new Vector2[save.rocaUVs.Length / 2];
		for (int i = 0; i < temp6u.Length; i++) {
			temp6u[i].x = save.rocaUVs[i * 2];
			temp6u[i].y = save.rocaUVs[i * 2 + 1];
		}
		temp6.uv = temp6u;
		int[] temp6t = new int[save.rocaTriangulos.Length];
		for (int i = 0; i < temp6t.Length; i++) {
			temp6t[i] = save.rocaTriangulos[i];
		}
		temp6.triangles = temp6t;
		contenedor.roca = temp6;
		//Mesh Agua
		Mesh temp7 = new Mesh();
		Vector3[] temp7v = new Vector3[save.aguaVertices.Length / 3];
		Vector3[] temp7n = new Vector3[save.aguaNormals.Length / 3];
		for (int i = 0; i < temp7v.Length; i++) {
			temp7v[i].x = save.aguaVertices[i * 3];
			temp7v[i].y = save.aguaVertices[i * 3 + 1];
			temp7v[i].z = save.aguaVertices[i * 3 + 2];
			temp7n[i].x = save.aguaNormals[i * 3];
			temp7n[i].y = save.aguaNormals[i * 3 + 1];
			temp7n[i].z = save.aguaNormals[i * 3 + 2];
		}
		temp7.vertices = temp7v;
		temp7.normals = temp7n;
		Vector2[] temp7u = new Vector2[save.aguaUVs.Length / 2];
		for (int i = 0; i < temp7u.Length; i++) {
			temp7u[i].x = save.aguaUVs[i * 2];
			temp7u[i].y = save.aguaUVs[i * 2 + 1];
		}
		temp7.uv = temp7u;
		int[] temp7t = new int[save.aguaTriangulos.Length];
		for (int i = 0; i < temp7t.Length; i++) {
			temp7t[i] = save.aguaTriangulos[i];
		}
		temp7.triangles = temp7t;
		contenedor.agua = temp7;
		//Otras variables
		contenedor.nivelAgua = save.nivelAgua;
		contenedor.tamanoPlaya = save.tamanoPlaya;
		//Fin
	}
	
	//Objeto con los indices
	public static void SaveIndices (int[] indices)
  	{ 
	    SaveIndices save = new SaveIndices();
		save.indices = indices;
	    FileStream stream = new FileStream(Application.dataPath + "/Indices.bin", FileMode.Create);
		Debug.Log("Archivo de indices guardado en: " + Application.dataPath);
		try {
		    BinaryFormatter bformatter = new BinaryFormatter();
		    bformatter.Serialize(stream, save);
		}
		catch (SerializationException e) {
			Debug.LogError("Excepcion al serializar el archivo de indices. Datos: " + e.Message);
		}
		finally {
	    	stream.Close();
		}
  	}

  	public static SaveIndices LoadIndices ()  {									//Sobrecargado
		return LoadIndices(Application.dataPath + "/Indices.bin");
	} 
	
  	public static SaveIndices LoadIndices(string filePath) 
  	{
	    SaveIndices data = new SaveIndices();
	    FileStream stream = new FileStream(filePath, FileMode.Open);
		try {
		    BinaryFormatter bformatter = new BinaryFormatter();
		    data = (SaveIndices)bformatter.Deserialize(stream);
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
public enum T_habitats {montana, llanura, colina, desierto, volcanico, mar, costa, tundra, inhabitable};	//Tipos de orografía
public enum T_elementos {comunes, raros, nada};																//Se pueden añadir mas mas adelante
	
public class FuncTablero {
	
	//Variables ----------------------------------------------------------------------------------------------------------------------
	
	//Uso del script
	public static int anchoTextura;					//Ancho de la textura en pixeles
	public static int altoTextura;					//Alto de la textura en pixeles
	private static int relTexTabAncho;				//Que relación hay entre el ancho de la textura y el ancho del tablero lógico
	private static int relTexTabAlto;				//Lo mismo pero para el alto
	private static Perlin perlin;					//Semilla
	
	//Ruido
	private static int octavas	= 4;				//Octavas para la funcion de ruido de turbulencias
	private static int octavas2	= 12;				//Octavas para la funcion de ruido de turbulencias
	private static float lacunaridad	= 4.5f;		//La lacunaridad (cuanto se desplazan las coordenadas en sucesivas "octavas")
	private static float ganancia = 0.45f;			//El peso que se le da a cada nueva octava
	private static float escala = 0.004f;			//El nivel de zoom sobre el ruido
	
	//Terreno
	private static float nivelAgua = 0.27f;								//El nivel sobre el que se pondrá agua. 0.45 ya es pasarse
	private static float tamanoPlaya = 0.05f;							//El tamaño de las playas. de 0.02 a 0.05 razonable
	private static float alturaColinas = nivelAgua+(1-nivelAgua)*0.3f;	//La altura a partir de la cual se considera colina (situada a 33% de lo que resta de tierra al establecer el nivel del agua)
	private static float alturaMontana = nivelAgua+(1-nivelAgua)*0.6f;	//La altura a partir de la cual se considera montaña (situada a 66% de lo que resta de tierra al establecer el nivel del agua)
	private static float temperatura = 0.5f;								//La temperatura del planeta, que influye en la generacion de habitats
	
	//Para el tablero
	public static int anchoTablero = 128;			//El ancho del tablero lógico (debe ser potencia de 2 para cuadrar con la textura)
	public static int altoTablero = 64;				//El alto del tablero lógico (debe ser potencia de 2 tambien)
	public static int casillasPolos	= 3;			//El numero de casillas que serán intransitables en los polos
	public static int numMaxEspecies = 20;			//Numero maximo de especies que puede haber en el tablero (juego) a la vez

		
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
	
	public static Texture2D calculaTexAgua(Texture2D texBase) {
		Color[] pixAgua = new Color[anchoTextura * altoTextura];
		Color[] pixBase = texBase.GetPixels();
		for (int l = 0; l < pixAgua.Length; l++) {
			if (pixBase[l].grayscale < nivelAgua) 
				pixAgua[l] = new Color(nivelAgua, nivelAgua, nivelAgua);
			else 
				pixAgua[l] = new Color(0, 0, 0);
		}
		Texture2D textura = new Texture2D(anchoTextura,altoTextura);
		textura.SetPixels(pixAgua);
		textura.Apply();
		return textura;
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
	
	public static Color[] suavizaPoloTex(Color[] pix) {
		Color[] pixels = pix;
		int lado = 2 * relTexTabAlto;				//Se suavizan dos casillas desde las inhabitables
		int margen = casillasPolos * relTexTabAlto;
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
	
	private static float calculaMediaCasilla(Vector2 cord, Color[] pixels){
		float media = 0;

		//--------------aqui valoramos diferentes metodos para determinar el habitat de la casilla:				
		//--------------Determinar el habitat por el valor min de color en la casilla				

//		float[] esquinas = new float[4];
//		esquinas[0] = pixels[((int)cord.y) * anchoTextura + (int)cord.x].r;
//		esquinas[1] = pixels[((int)cord.y) * anchoTextura + (int)cord.x + relTexTabAncho - 1].r;
//		esquinas[2] = pixels[((int)cord.y + relTexTabAlto - 1) * anchoTextura + (int)cord.x].r;
//		esquinas[3] = pixels[((int)cord.y + relTexTabAlto - 1) * anchoTextura + (int)cord.x + relTexTabAncho - 1].r;
//		media = Mathf.Min(esquinas);
		
		//--------------pixel del vertice(?)


		//--------------Contabilizar solo las esquinas de la casilla para la media
		media += pixels[((int)cord.y) * anchoTextura + (int)cord.x].r;
		media += pixels[((int)cord.y) * anchoTextura + (int)cord.x + relTexTabAncho - 1].r;
		media += pixels[((int)cord.y + relTexTabAlto - 1) * anchoTextura + (int)cord.x].r;
		media += pixels[((int)cord.y + relTexTabAlto - 1) * anchoTextura + (int)cord.x + relTexTabAncho - 1].r;
		media = media / 4;
		
//		//--------------Contabilizar todos los pixeles de la casilla
//		for (int x = 0; x < relTexTabAlto; x++) {
//			for (int y = 0; y < relTexTabAncho; y++) {
//				media += pixels[((int)cord.y + x) * anchoTextura + (int)cord.x + y].r;
//			}
//		}
//		media = media / (relTexTabAncho * relTexTabAlto);	
		
		return media;		
	}
	
	private static T_habitats[] calculaHabitats(Texture2D texHeightmap, Texture2D texHabitats, Texture2D texHabitatsEstetica, Texture2D texElems, out T_elementos[] elemsOut) {
		T_habitats[] habitats = new T_habitats[altoTablero*anchoTablero];
		T_elementos[] elems = new T_elementos[altoTablero*anchoTablero];
		for (int i = 0; i < altoTablero * anchoTablero; i++) {
			habitats[i] = T_habitats.inhabitable;
		}
		//reinicia la textura de habitats estetica
		Color[] pixels = new Color[anchoTextura*altoTextura];
		texHabitatsEstetica.SetPixels(pixels);
		texHabitatsEstetica.Apply();
		
		pixels = texHeightmap.GetPixels();
		Color[] pixelsHab = texHabitats.GetPixels();
		Color[] pixelsElem = texElems.GetPixels();
		int altoTableroUtil = altoTablero - casillasPolos * 2;
		
		float tempeLineal = 0;
		int casillasTempe = (int)Mathf.Lerp(0,altoTableroUtil / 4, temperatura);
		//Se divide el espacio en zonas (en este caso 3 franjas horizontales)
		int limiteHab1 = altoTableroUtil / 3 + casillasPolos - (casillasTempe / 2);
		int limiteHab2 = (altoTableroUtil / 3) * 2 + (altoTableroUtil % 3) + casillasTempe + (casillasTempe % 2);
		
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
				
		//Se ponen las casillas de los polos a habitat inhabitable
		for (int i = 0; i < casillasPolos; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				habitats[i * anchoTablero + j] = T_habitats.inhabitable;
				elems[i * anchoTablero + j] = T_elementos.nada;
				Vector2 cord = new Vector2(j * relTexTabAncho , i * relTexTabAlto);
				Color color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
				pintaPixelsCasilla(pixelsHab, cord, color);
			}
		}
		
		//Se calcula la primera franja de izquierda a derecha y de arriba a abajo
		//Esta franja representa la zona cercana al polo SUR hasta el primer trópico
				
		for (int i = casillasPolos; i < limiteHab1; i++) {
			tempeLineal = Mathf.Lerp(0, temperatura / 2, (i - casillasPolos) / (limiteHab1 - casillasPolos));
			for (int j = 0; j < anchoTablero; j++) {
				
				//Coordenadas de la casilla que estamos mirando
				Vector2 cord = new Vector2(j * relTexTabAncho , i * relTexTabAlto);
				
				//Se calcula la media de altura de la casilla
				float media = calculaMediaCasilla(cord, pixels);
				
				//Se calcula el habitat en el que va a estar la casilla
				T_habitats habitatTemp;
				T_elementos elemTemp;
				if (media < (nivelAgua - tamanoPlaya)) {
					habitatTemp = T_habitats.mar; 
					elemTemp = T_elementos.nada;
				} 
				else if ((nivelAgua - tamanoPlaya <= media) && (media < nivelAgua + tamanoPlaya/4)) {
					habitatTemp = T_habitats.costa;
					elemTemp = T_elementos.nada;
				}
				else if ((nivelAgua + tamanoPlaya/4 <= media) && (media < alturaColinas)) {
					//Area de colina. Habitats posibles: LLanura y tundra
					//Con un 5% es desierto, con un 15% es tundra y con un 80% es llanura
					float posDesierto = 0.05f;
					float posTundra = 0.15f;;
					
					//TODO Esto es un planteamiento posible de como usar la temperatura
					posTundra -= tempeLineal * 0.1f;		//La temperatura hace que haya menos tundras
					posDesierto += tempeLineal * 0.05f;		//La temperatura hace que haya mas desiertos
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posDesierto += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.05f;
					posTundra += numCasillasCercanasHabitat(T_habitats.desierto, habitats, i, j) * 0.15f;
					
					if (numero > posTundra) {
						habitatTemp = T_habitats.llanura;
						if (UnityEngine.Random.Range(0,9) == 0)		//Una posibilidad entre 10, un 10%
							elemTemp = T_elementos.comunes;
						else
							elemTemp = T_elementos.nada;
					}
					else {//(numero <= posTundra)
						if (UnityEngine.Random.Range(0,21) == 0)		//Una posibilidad entre 20, un 5%
							elemTemp = T_elementos.raros;
						else
							elemTemp = T_elementos.comunes;
						
						if (numero > posDesierto)
							habitatTemp = T_habitats.tundra;
						else
							habitatTemp = T_habitats.desierto;
					}
				}
				else if ((alturaColinas <= media) && (media < alturaMontana)) {
					//Área de colina. Habitats posibles: Colina, tundra
					//Con un 25% es tundra y con un 75% es colina
					float posibilidad = 0.25f;
					
					//TODO Posible uso de temperatura
					posibilidad += tempeLineal * 0.1f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posibilidad += numCasillasCercanasHabitat(T_habitats.tundra, habitats, i, j) * 0.15f;
					posibilidad += numCasillasCercanasHabitat(T_habitats.montana, habitats, i, j) * 0.1f;
					
					if (numero <= posibilidad) {
						habitatTemp = T_habitats.tundra;
						int posElementos = UnityEngine.Random.Range(0,11);
						if (posElementos == 0)		
							elemTemp = T_elementos.raros;
						else {
							if (posElementos > 9)
								elemTemp = T_elementos.comunes;
							else {
								elemTemp = T_elementos.nada;
							}
						}
					}
					else {
						habitatTemp = T_habitats.colina;
						if (UnityEngine.Random.Range(0,6) == 0)
							elemTemp = T_elementos.comunes;
						else
							elemTemp = T_elementos.nada;
					}					
				}
				else { //if (alturaMontana < media)
					//Área de montaña. Habitats posibles: Mountain y Volcanic
					//Con un 1% es volcanico y con un 99% es montaña
					float posibilidad = 0.01f;
					
					//TODO Posible uso de temperatura
					posibilidad += tempeLineal * 0.01f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posibilidad += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.01f;
					if (numero <= posibilidad) {
						habitatTemp = T_habitats.volcanico;
						elemTemp = T_elementos.raros;
					}
					else {
						habitatTemp = T_habitats.montana;
						int posElementos = UnityEngine.Random.Range(0,11);
						if (posElementos == 0)
							elemTemp = T_elementos.raros;
						else if (posElementos > 9)
							elemTemp = T_elementos.comunes;
						else
							elemTemp = T_elementos.nada;
					}
				}
				Color colorTemp = getColorHabitat(habitatTemp);
				Color colorTemp2 = getColorElem(elemTemp);
				Color colorTemp3 = getColorHabitatShader(habitatTemp);
				pintaTexPincel(texHabitatsEstetica, cord, UnityEngine.Random.Range(6,14), colorTemp3, true, true);			
				pintaPixelsCasilla(pixelsHab, cord, colorTemp);
				pintaPixelsCasilla(pixelsElem, cord, colorTemp2);
				
				habitats[i * anchoTablero + j] = habitatTemp;
				elems[i * anchoTablero + j] = elemTemp;
			}
		}
		
		//Franja NORTE, que equivaldría como la primera a zonas mas cercanas a los polos
		//Se calcula de derecha a izquierda y de abajo a arriba
		
		for (int i = altoTablero - 1 - casillasPolos; i >= limiteHab2; i--) {
			tempeLineal = Mathf.Lerp(temperatura / 2, 0, (i - limiteHab2) / (altoTablero - casillasPolos - 1 - limiteHab2));
			for (int j = anchoTablero - 1; j >= 0; j--) {
				
				//Coordenadas de la casilla que estamos mirando
				Vector2 cord = new Vector2(j * relTexTabAncho , i * relTexTabAlto);
				
				//Se calcula la media de altura de la casilla
				float media = calculaMediaCasilla(cord, pixels);
				
				//Se calcula el habitat en el que va a estar la casilla
				T_habitats habitatTemp;
				T_elementos elemTemp;
				if (media < (nivelAgua - tamanoPlaya)) {
					habitatTemp = T_habitats.mar; 
					elemTemp = T_elementos.nada;
				} 
				else if ((nivelAgua - tamanoPlaya <= media) && (media < nivelAgua + tamanoPlaya/4)) {
					habitatTemp = T_habitats.costa;
					elemTemp = T_elementos.nada;
				}
				else if ((nivelAgua + tamanoPlaya/4 <= media) && (media < alturaColinas)) {
					//Area de colina. Habitats posibles: LLanura y tundra
					//Con un 5% es desierto, con un 15% es tundra y con un 80% es llanura
					float posDesierto = 0.05f;
					float posTundra = 0.15f;;
					
					//TODO Esto es un planteamiento posible de como usar la temperatura
					posTundra -= tempeLineal * 0.1f;		//La temperatura hace que haya menos tundras
					posDesierto += tempeLineal * 0.05f;		//La temperatura hace que haya mas desiertos
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posDesierto += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.05f;
					posTundra += numCasillasCercanasHabitat(T_habitats.tundra, habitats, i, j) * 0.15f;
					
					if (numero > posTundra) {
						habitatTemp = T_habitats.llanura;
						if (UnityEngine.Random.Range(0,9) == 0)		//Una posibilidad entre 10, un 10%
							elemTemp = T_elementos.comunes;
						else
							elemTemp = T_elementos.nada;
					}
					else {//(numero <= posTundra)
						if (UnityEngine.Random.Range(0,21) == 0)		//Una posibilidad entre 20, un 5%
							elemTemp = T_elementos.raros;
						else
							elemTemp = T_elementos.comunes;
						
						if (numero > posDesierto)
							habitatTemp = T_habitats.tundra;
						else
							habitatTemp = T_habitats.desierto;
					}
				}
				else if ((alturaColinas <= media) && (media < alturaMontana)) {
					//Área de colina. Habitats posibles: Colina, tundra
					//Con un 25% es tundra y con un 75% es colina
					float posibilidad = 0.25f;
					
					//TODO Posible uso de temperatura
					posibilidad += tempeLineal * 0.1f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posibilidad += numCasillasCercanasHabitat(T_habitats.tundra, habitats, i, j) * 0.15f;
					posibilidad += numCasillasCercanasHabitat(T_habitats.montana, habitats, i, j) * 0.1f;
					
					if (numero <= posibilidad) {
						habitatTemp = T_habitats.tundra;
						int posElementos = UnityEngine.Random.Range(0,11);
						if (posElementos == 0)		
							elemTemp = T_elementos.raros;
						else {
							if (posElementos > 9)
								elemTemp = T_elementos.comunes;
							else {
								elemTemp = T_elementos.nada;
							}
						}
					}
					else {
						habitatTemp = T_habitats.colina;
						if (UnityEngine.Random.Range(0,6) == 0)
							elemTemp = T_elementos.comunes;
						else
							elemTemp = T_elementos.nada;
					}					
				}
				else { //if (alturaMontana < media)
					//Área de montaña. Habitats posibles: Mountain y Volcanic
					//Con un 1% es volcanico y con un 99% es montaña
					float posibilidad = 0.01f;
					
					//TODO Posible uso de temperatura
					posibilidad += tempeLineal * 0.01f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posibilidad += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.01f;
					if (numero <= posibilidad) {
						habitatTemp = T_habitats.volcanico;
						elemTemp = T_elementos.raros;
					}
					else {
						habitatTemp = T_habitats.montana;
						int posElementos = UnityEngine.Random.Range(0,11);
						if (posElementos == 0)
							elemTemp = T_elementos.raros;
						else if (posElementos > 9)
							elemTemp = T_elementos.comunes;
						else
							elemTemp = T_elementos.nada;
					}
				}
				Color colorTemp = getColorHabitat(habitatTemp);
				Color colorTemp2 = getColorElem(elemTemp);
				Color colorTemp3 = getColorHabitatShader(habitatTemp);
				pintaTexPincel(texHabitatsEstetica, cord, UnityEngine.Random.Range(6,14), colorTemp3, true, true);			
				pintaPixelsCasilla(pixelsHab, cord, colorTemp);
				pintaPixelsCasilla(pixelsElem, cord, colorTemp2);
				
				habitats[i * anchoTablero + j] = habitatTemp;
				elems[i * anchoTablero + j] = elemTemp;
			}
		}
		
		//Franja central (en esta hay mas posibilidad de areas tropicales o desiertos (mas calor)
		//TODO Sentido del cálculo???
		for (int i = limiteHab1; i < limiteHab2; i++) {
			tempeLineal = Mathf.Lerp(temperatura, temperatura / 2, Mathf.Abs((altoTablero / 2) - i));
			for (int j = 0; j < anchoTablero; j++) {
				
				//Coordenadas de la casilla que estamos mirando
				Vector2 cord = new Vector2(j * relTexTabAncho , i * relTexTabAlto);
				
				//Se calcula la media de altura de la casilla
				float media = calculaMediaCasilla(cord, pixels);
				
				//Se calcula el habitat en el que va a estar la casilla
				T_habitats habitatTemp;
				T_elementos elemTemp;
				if (media < (nivelAgua - tamanoPlaya)) {
					habitatTemp = T_habitats.mar; 
					elemTemp = T_elementos.nada;
				} 
				else if ((nivelAgua - tamanoPlaya <= media) && (media < nivelAgua + tamanoPlaya/4)) {
					habitatTemp = T_habitats.costa;
					elemTemp = T_elementos.nada;
				}
				else if ((nivelAgua + tamanoPlaya/4 <= media) && (media < alturaColinas)) {
					//Area de llanuras. Habitats posibles: LLanura y desierto
					//Con un 5% es desierto y con un 95% es llanura
					float posDesierto = 0.1f;
					
					//TODO Posible uso de temperatura
					posDesierto += tempeLineal * 0.05f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posDesierto += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.05f;
					posDesierto += numCasillasCercanasHabitat(T_habitats.desierto, habitats, i, j) * 0.05f;
					if (numero <= posDesierto) {
						habitatTemp = T_habitats.desierto;
						if (UnityEngine.Random.Range(0,11) == 0)
							elemTemp = T_elementos.raros;
						else
							elemTemp = T_elementos.comunes;
					}
					else {
						habitatTemp = T_habitats.llanura;
						if (UnityEngine.Random.Range(0,9) == 0)
							elemTemp = T_elementos.comunes;
						else
							elemTemp = T_elementos.nada;
					}
				}
				else if ((alturaColinas <= media) && (media < alturaMontana)) {
					//Área de colina. Habitats posibles: Colina, desierto y volcanico
					//Con un 1% es volcanico, con un 10% es desierto y con un 89% es colina
					float posVolcanico = 0.01f;
					float posDesierto = 0.05f;
					
					//TODO Posible uso de temperatura
					posDesierto += tempeLineal * 0.05f;
					posVolcanico += tempeLineal * 0.01f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posVolcanico += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.05f;
					posDesierto += numCasillasCercanasHabitat(T_habitats.desierto, habitats, i, j) * 0.05f;
					if (numero <= posVolcanico) {
						habitatTemp = T_habitats.volcanico;
						elemTemp = T_elementos.raros;
					}
					else if (numero <= posVolcanico + posDesierto) {
						habitatTemp = T_habitats.desierto;
						if (UnityEngine.Random.Range(0,11) == 0)
							elemTemp = T_elementos.raros;
						else
							elemTemp = T_elementos.comunes;
					}
					else {
						habitatTemp = T_habitats.colina;
						if (UnityEngine.Random.Range(0,9) == 0)
							elemTemp = T_elementos.comunes;
						else
							elemTemp = T_elementos.nada;
					}					
				}
				else { //if (alturaMontana < media)
					//Área de montaña. Habitats posibles: montaña y volcanico
					//Con un 10% es volcanico y con un 90% es montaña
					float posibilidad = 0.05f;
					
					//TODO Posible uso de temperatura
					posibilidad += tempeLineal * 0.05f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posibilidad += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.01f;
					if (numero <= posibilidad) {
						habitatTemp = T_habitats.volcanico;
						elemTemp = T_elementos.raros;
					}
					else {
						habitatTemp = T_habitats.montana;
						int posElementos = UnityEngine.Random.Range(0,11);
						if (posElementos == 0)
							elemTemp = T_elementos.raros;
						else if (posElementos > 9)
							elemTemp = T_elementos.comunes;
						else {
							elemTemp = T_elementos.nada;
						}
					}
				}
				
				Color colorTemp = getColorHabitat(habitatTemp);
				Color colorTemp2 = getColorElem(elemTemp);
				Color colorTemp3 = getColorHabitatShader(habitatTemp);
				pintaTexPincel(texHabitatsEstetica, cord, UnityEngine.Random.Range(6,14), colorTemp3, true, true);
				pintaPixelsCasilla(pixelsHab, cord, colorTemp);
				pintaPixelsCasilla(pixelsElem,cord, colorTemp2);

				habitats[i * anchoTablero + j] = habitatTemp;
				elems[i * anchoTablero + j] = elemTemp;
			}
		}
			
		//Después de la ultima franja es inhabitable igual que antes que la primera.
		for (int i = altoTablero - casillasPolos; i < altoTablero; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				habitats[i * anchoTablero + j] = T_habitats.inhabitable;
				Vector2 cord = new Vector2(j * relTexTabAncho , i * relTexTabAlto);
				Color color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
				pintaPixelsCasilla(pixelsHab, cord, color);
			}
		}
		texHabitats.SetPixels(pixelsHab);
		texElems.SetPixels(pixelsElem);
		texHabitats.Apply();
		texHabitatsEstetica.Apply();
		texElems.Apply();
		elemsOut = elems; 
		return habitats;
	}
		
	private static void pintaPixelsCasilla(Color[] pixelsHab, Vector2 cord, Color color){
		for (int x = 0; x < relTexTabAlto; x++)
			for (int y = 0; y < relTexTabAncho; y++)
				pixelsHab[((int)cord.y + x) * anchoTextura + (int)cord.x + y] = color;
	}
	
	private static Color getColorHabitat(T_habitats habitatTemp){
		Color colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
				switch (habitatTemp) {
				case T_habitats.mar:
					colorTemp = new Color(0.0f, 0.25f, 0.5f, 0.0f);
					break;
				case T_habitats.costa:
					colorTemp = new Color(1.0f, 0.9f, 0.7f, 0.0f);
					break;
				case T_habitats.llanura:
					colorTemp = new Color(0.5f, 0.75f, 0.15f, 0.0f);
					break;
				case T_habitats.tundra:
					colorTemp = new Color(0.6f, 0.8f, 1.0f, 0.0f);
					break;
				case T_habitats.desierto:
					colorTemp = new Color(1.0f, 0.9f, 0.3f, 0.0f);
					break;
				case T_habitats.colina:
					colorTemp = new Color(0.4f, 0.65f, 0.05f, 0.0f);
					break;
				case T_habitats.volcanico:
					colorTemp = new Color(1.0f, 0.0f, 0.0f, 0.0f);
					break;
				case T_habitats.montana:
					colorTemp = new Color(0.5f, 0.5f, 0.55f, 0.0f);
					break;
				default:
					colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
					break;
				}	
		return colorTemp;
	}
	
	private static Color getColorHabitatShader(T_habitats habitatTemp){
		Color colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
				switch (habitatTemp) {
				case T_habitats.mar:
					colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
					break;
				case T_habitats.costa:
					colorTemp = new Color(0.0f, 0.0f, 0.0f, 1.0f);
					break;
				case T_habitats.llanura:
					colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
					break;
				case T_habitats.tundra:
					colorTemp = new Color(0.0f, 1.0f, 0.0f, 0.0f);
					break;
				case T_habitats.desierto:
					colorTemp = new Color(0.0f, 0.0f, 1.0f, 0.0f);
					break;
				case T_habitats.colina:
					colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
					break;
				case T_habitats.volcanico:
					colorTemp = new Color(1.0f, 0.0f, 0.0f, 0.0f);
					break;
				case T_habitats.montana:
					colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
					break;
				default:
					colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
					break;
				}	
		return colorTemp;
	}
	
	private static Color getColorElem(T_elementos elemTemp){
		Color colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
				switch (elemTemp) {
				case T_elementos.comunes:
					colorTemp = new Color(1.0f, 0.0f, 0.0f, 0.0f);
					break;
				case T_elementos.raros:
					colorTemp = new Color(0.0f, 0.0f, 1.0f, 0.0f);
					break;
				default:
					colorTemp = new Color(0.0f, 0.0f, 0.0f, 0.0f);
					break;
				}	
		return colorTemp;
	}
	
	private static int numCasillasCercanasHabitat(T_habitats habComprobar, T_habitats[] habitats, int i, int j) {
		int numCasillasIguales = 0;
		for(int x = -1; x <= 1; x++) {
			for(int y = -1; y <= 1; y++) {
				if (i + x < 0 || i + x >= altoTablero)
					continue;
				if (j + y >= anchoTablero) { 			//Hay que mirar la casilla del inicio de la fila
					if (habitats[(i + x) * anchoTablero] == habComprobar)
						numCasillasIguales++;
				}
				else if (j + y < 0) {					//Hay que mirar la casilla del final de la fila
					if (habitats[(i + x + 1) * anchoTablero - 1] == habComprobar)
						numCasillasIguales++;
				}
				else 
					if (habitats[(i + x) * anchoTablero + (j + y)] == habComprobar)
						numCasillasIguales++;
			}
		}
		return numCasillasIguales;
	}
	
	public static Casilla[,] iniciaTablero(Texture2D texHeightmap, Texture2D texHabitats, Texture2D texHabitatsEstetica, Texture2D texElems, Mesh mesh) {
		Casilla[,] tablero = new Casilla[altoTablero,anchoTablero];
		Vector3[] vertices = mesh.vertices;
		T_elementos[] elems = new T_elementos[altoTablero*anchoTablero];
		T_habitats[] habitat = calculaHabitats(texHeightmap, texHabitats, texHabitatsEstetica, texElems, out elems);
		
		//Generacion de indices ----------------------------------------------
		
		//Comentar para generar indices nuevos------------
		int[] indices = SaveLoad.LoadIndices().indices;
		
		//Descomentar para generar indices nuevos---------
		//Vector2[] uvs = mesh.uv;
		//int[] indices = calculaIndicesVertices(texHeightmap.width, texHeightmap.height, uvs);
		//SaveLoad.SaveIndices(indices);
		
		//Generacion de indices ----------------------------------------------
		
		int k = 0;
		for (int i = 0; i < altoTablero; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				Vector2 cord = new Vector2(j * relTexTabAncho , i * relTexTabAlto);
				
				tablero[i,j] = new Casilla(habitat[i * anchoTablero + j], elems[i * anchoTablero + j], cord, vertices[indices[k]]);
				k++;
			}
		}
		tablero = mueveVertices(tablero);
		return tablero;
	}
	
	private static int[] calculaIndicesVertices(int width, int height, Vector2[] uvs) {
		int[] resultado = new int[altoTablero * anchoTablero];
		int z = 0;
		
		for (int i = 0; i < altoTablero; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				//Las coordenadas de la casilla actual en la textura
				Vector2 cord = new Vector2(j * relTexTabAncho , i * relTexTabAlto);
				Vector2 cord2 = new Vector2((j + 1) * relTexTabAncho, (i + 1) * relTexTabAlto);
				//Se calcula la coordenada del vértice a partir del UV
				int indice = -1;
				for (int k = 0; k < uvs.Length; k++) {
					Vector2 temp = uvs[k];
					temp.x *= width;
					temp.y *= height;
					if (temp.x >= cord.x && temp.y >= cord.y && temp.x <= cord2.x && temp.y <= cord2.y) {
						indice = k;
						break;
					}
				}
				if (indice == -1)
					Debug.LogError("No se ha encontrado la coordenada por su UV en iniciaTablero(). Casilla: " + i + "," + j + ";");
				
				
				resultado[z] = indice;
				z++;
			}
		}
		
		return resultado;
	}
	
	private static Casilla[,] mueveVertices(Casilla[,] tableroIn) {
		Casilla[,] tableroOut;
		tableroOut = tableroIn;
//		De momento lo dejo comentado porque es un cambio estetico y de momento lo podemos aparcar.
//		for (int i = 0; i < altoTablero; i++) {
//			for (int j = 0; j < anchoTablero; j++) {
//				Vector2 coordsTex = tableroIn[i,j].coordsTex;
//				Vector3 coordsVert = tableroIn[i,j].coordsVert;
//				
//			}
//		}
		return tableroOut;
	}
	
	public static void convierteCoordenadas(ref int x,ref int y)
	{
		x %= altoTablero;
		while(x < 0)
			x += altoTablero;
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
	
	public static void alteraPixelColor(Texture2D tex, int w, int h, Color valor, bool aditivo, bool positivo){
//		if (h < 0 || h > tex.height) {
//			Debug.LogError("Error en alteraPixel: los limites de la textura se sobrepasan. w = " + w + " h = " + h);
//		}
		if (h >= tex.height)
			h = tex.height - 1;
		else if (h < 0)
			h = 0;
		if (w < 0)
			w += tex.width;
		w = w % tex.width;
		Color pix = tex.GetPixel(w,h);
		if (positivo){
			if (aditivo) 
				pix += valor;
			else
				if (valor.r > 0 || valor.g > 0 || valor.b > 0)
					pix = valor; 
		}
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
	
	
	public static void pintaTexPincel(Texture2D objetivo, Vector2 cords, int pincel, Color colorObjetivo, bool aditivo, bool positivo) {
		Pinceles temp = GameObject.FindGameObjectWithTag("Pinceles").GetComponent<Pinceles>();
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
		case 5:			
			pincelTex = temp.pincelPlantas;
			break;
		case 6:			
			pincelTex = temp.pincelHabitats1;
			break;
		case 7:			
			pincelTex = temp.pincelHabitats2;
			break;
		case 8:			
			pincelTex = temp.pincelHabitats3;
			break;
		case 9:			
			pincelTex = temp.pincelHabitats4;
			break;
		case 10:			
			pincelTex = temp.pincelHabitats5;
			break;
		case 11:			
			pincelTex = temp.pincelHabitats6;
			break;
		case 12:			
			pincelTex = temp.pincelHabitats7;
			break;
		case 13:			
			pincelTex = temp.pincelHabitats8;
			break;
		default: 
			pincelTex = temp.pincelRegular;
			break;			
		}		
		int w = pincelTex.width;
		int h = pincelTex.height;
		Vector2 pos = cords;
		
		pos.x -= w/4;
		pos.y -= h/4;
		if (pos.y < 0)
			pos.y = 0;
		if (pos.y >= objetivo.height)
			pos.y = objetivo.height - 1;
		
		Color colorTemp = colorObjetivo;
		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				colorTemp = colorObjetivo;
				colorTemp *= pincelTex.GetPixel(i,j);
					alteraPixelColor(objetivo,(int)pos.x + i,(int)pos.y + j, colorTemp, aditivo, positivo);
			}
		}
	}	
	
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
			colorObjetivo = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			break;
		}
		Color colorTemp = colorObjetivo;
		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				colorTemp = colorObjetivo;
				colorTemp *= pincelTex.GetPixel(i,j);
					alteraPixelColor(objetivo,(int)pos.x + i,(int)pos.y + j, colorTemp, true, positivo);
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
		relTexTabAncho = anchoTextura / anchoTablero;
		relTexTabAlto = altoTextura / altoTablero;
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
	
	public static float getNivelAgua() {
		return nivelAgua;
	}
	
	public static float getTamanoPlaya() {
		return tamanoPlaya;
	}
	
	public static void setAlturaColinas(float entrada) {
		if (entrada >= 0.0f && entrada <= 1.0f)
			alturaColinas = entrada;
	}
	
	public static void setAlturaMontana(float entrada) {
		if (entrada >= 0.0f && entrada <= 1.0f)
			alturaMontana = entrada;
	}
	
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
	
	public static string formateaTiempo ()
	{
		string result = "";
		int tiempo = (int)Time.realtimeSinceStartup;
		int minutes = tiempo / 60;
		int seconds = tiempo % 60;
		int fraction = (tiempo * 100) % 100;
		result = System.String.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
		return result;
	} 
}