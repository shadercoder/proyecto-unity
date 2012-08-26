using UnityEngine;

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

using System.Collections;
using System.Collections.Generic;

//Clase para tratar la textura y el tablero -----------------------------------------------------------------------------------------
public enum T_habitats {montana, llanura, colina, desierto, volcanico, mar, costa, tundra, inhabitable};	//Tipos de orografía
public enum T_elementos {comunes, raros, nada};																//Se pueden añadir mas mas adelante

public struct Tupla<T1, T2>
{
	public T1 e1;
	public T2 e2;
	public Tupla(T1 i1,T2 i2)
	{
		e1 = i1;
		e2 = i2;
	}
}

public struct Tupla<T1, T2, T3>
{
	public T1 e1;
	public T2 e2;
	public T3 e3;
	public Tupla(T1 i1,T2 i2,T3 i3)
	{
		e1 = i1;
		e2 = i2;
		e3 = i3;
	}
	/*public void setE1(T1 i1){this = new Tupla<T1, T2, T3>(i1,e2,e3);}
	public void setE3(T2 i2){this = new Tupla<T1, T2, T3>(e1,i2,e3);}
	public void setE3(T3 i3){this = new Tupla<T1, T2, T3>(e1,e2,i3);}*/
	
}
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
	public static int altoTablero = 64;			//El alto del tablero lógico (debe ser potencia de 2 tambien)
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
					//Con un 2% es desierto, con un 5% es tundra y con un 93% es llanura
					float posDesierto = 0.02f;
					float posTundra = 0.05f;;
					
					//TODO Esto es un planteamiento posible de como usar la temperatura
					posTundra -= tempeLineal * 0.1f;		//La temperatura hace que haya menos tundras
					posDesierto += tempeLineal * 0.05f;		//La temperatura hace que haya mas desiertos
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posDesierto += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.15f;
					posDesierto += numCasillasCercanasHabitat(T_habitats.desierto, habitats, i, j) * 0.2f;
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
					//Con un 2% es desierto, con un 5% es tundra y con un 93% es llanura
					float posDesierto = 0.02f;
					float posTundra = 0.05f;;
					
					//TODO Esto es un planteamiento posible de como usar la temperatura
					posTundra -= tempeLineal * 0.1f;		//La temperatura hace que haya menos tundras
					posDesierto += tempeLineal * 0.05f;		//La temperatura hace que haya mas desiertos
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posDesierto += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.15f;
					posDesierto += numCasillasCercanasHabitat(T_habitats.desierto, habitats, i, j) * 0.2f;
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
					float posDesierto = 0.05f;
					
					//TODO Posible uso de temperatura
					posDesierto += tempeLineal * 0.05f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posDesierto += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.15f;
					posDesierto += numCasillasCercanasHabitat(T_habitats.desierto, habitats, i, j) * 0.2f;
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
					//Con un 1% es volcanico, con un 5% es desierto y con un 94% es colina
					float posVolcanico = 0.01f;
					float posDesierto = 0.05f;
					
					//TODO Posible uso de temperatura
					posDesierto += tempeLineal * 0.05f;
					posVolcanico += tempeLineal * 0.01f;
					
					float numero = UnityEngine.Random.Range(0.0f, 1.0f);
					posVolcanico += numCasillasCercanasHabitat(T_habitats.volcanico, habitats, i, j) * 0.05f;
					posDesierto += numCasillasCercanasHabitat(T_habitats.desierto, habitats, i, j) * 0.1f;
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
					//Con un 5% es volcanico y con un 95% es montaña
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
	
	public static Casilla[,] iniciaTablero(Texture2D texHeightmap, Texture2D texHabitats, Texture2D texHabitatsEstetica, Texture2D texElems, Mesh mesh, Vector3 posicionPlaneta) {
		Casilla[,] tablero = new Casilla[altoTablero,anchoTablero];
		Vector3[] vertices = mesh.vertices;
		T_elementos[] elems = new T_elementos[altoTablero*anchoTablero];
		T_habitats[] habitat = calculaHabitats(texHeightmap, texHabitats, texHabitatsEstetica, texElems, out elems);
		
		//Generacion de indices ----------------------------------------------
		
		//Comentar para generar indices nuevos------------
		//int[] indices = SaveLoad.LoadIndices().indices;
		
		//Descomentar para generar indices nuevos---------
		Vector2[] uvs = mesh.uv;
		int[] indices = calculaIndicesVertices(texHeightmap.width, texHeightmap.height, uvs);
		//int[] indices = calculaVerticesCasilla(texHeightmap.width, texHeightmap.height, mesh); //calculo de vertices con raycast.
		SaveLoad.SaveIndices(indices);
		
		//Generacion de indices ----------------------------------------------
		
		int k = 0;
		for (int i = 0; i < altoTablero; i++) {
			for (int j = 0; j < anchoTablero; j++) {
				Vector2 cord = new Vector2(j * relTexTabAncho , i * relTexTabAlto);
				
				tablero[i,j] = new Casilla(habitat[i * anchoTablero + j], elems[i * anchoTablero + j], cord, vertices[indices[k]]);
				k++;
			}
		}
		//Comento esta linea para hacer una prueba
		//tablero = mueveVertices(tablero);
		//tablero = mueveVertices(tablero, texHeightmap, posicionPlaneta);
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
	
	private static int[] calculaVerticesCasilla(int width, int height, Mesh mesh){
		int[] resultado = new int[altoTablero * anchoTablero];
		Vector3 origen;
		RaycastHit hit;
		Vector2 pixel;
		Vector3 direccion;
		
		for (int i = 0; i< mesh.normals.Length; i++){
			//por cada vertice lanzo un raycast en direccion la normal
			origen = mesh.normals[i] * 10;
			direccion = mesh.normals[i] - origen;
			Physics.Raycast(origen,direccion,out hit);
			//Debug.DrawLine(origen, direccion, Color.red,500.0f);
			//en cada normal (una por vertice) ha dado en un pixel de la textura, sus coordenadas normalizadas. Las denormalizamos con las dimensiones de la textura
			pixel = new Vector2(hit.textureCoord.x*width, hit.textureCoord.y*height);
			//calculamos a que casilla corresponde con una regla de 3
			int posX = (int)pixel.x / relTexTabAncho;
			int posY = (int)pixel.y / relTexTabAlto;
			int indice = posX + posY*anchoTablero;
			resultado[indice] = i;
		}
		return resultado;
	}                                         
	                                            
	private static Casilla[,] mueveVertices(Casilla[,] tableroIn) {
		Casilla[,] tableroOut;
		tableroOut = tableroIn;
		float x,y,z;
		Vector3 coordsVert;
		for (int i = altoTablero-1; i > 0; i--) 
		{
			for (int j = anchoTablero-1; j >= 0; j--) 
			{
				x = (tableroIn[i-1,j].coordsVert.x + tableroIn[i,j].coordsVert.x)/2;
				y = (tableroIn[i-1,j].coordsVert.y + tableroIn[i,j].coordsVert.y)/2;
				z = (tableroIn[i-1,j].coordsVert.z + tableroIn[i,j].coordsVert.z)/2;
				coordsVert = new Vector3(x,y,z);				
				tableroOut[i,j].coordsVert = coordsVert;
			}			
		}
		return tableroOut;		
	}
	private static Casilla[,] mueveVertices(Casilla[,] tableroIn, Texture2D texIn, Vector3 posicionPlaneta) {
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
		//Dejo esto comentado para probar la siguiente forma tambien
//		float x,y,z;
//		Vector3 coordsVert;
//		for (int i = altoTablero-1; i > 0; i--) 
//		{
//			for (int j = anchoTablero-1; j >= 0; j--) 
//			{
//				x = (tableroIn[i-1,j].coordsVert.x + tableroIn[i,j].coordsVert.x)/2;
//				y = (tableroIn[i-1,j].coordsVert.y + tableroIn[i,j].coordsVert.y)/2;
//				z = (tableroIn[i-1,j].coordsVert.z + tableroIn[i,j].coordsVert.z)/2;
//				coordsVert = new Vector3(x,y,z);				
//				tableroOut[i,j].coordsVert = coordsVert;
//			}			
//		}
		/* [Aris] Con esta parte "teoricamente" seria a la altura perfecta, pero actualmente el fallo es que se 
		 * suma la extrusion dos veces, y por tanto sale el punto flotando por encima del planeta.
		 * Habria que restar la extrusion a los vertices utilizados para las coordenadas x y z y luego dejar
		 * el resto como esta.
		 * */
		float x,y,z;
		Vector3 coordsVert;
		Vector3 pos;
		Color[] col = texIn.GetPixels();
		for (int i = altoTablero-1; i > 0; i--) 
		{
			for (int j = anchoTablero-1; j >= 0; j--) 
			{
				int iTab = (i * relTexTabAlto) + (relTexTabAlto / 2);
				int jTab = (j * relTexTabAncho) + ( relTexTabAncho / 2);
				float altura = col[iTab * texIn.width + jTab].r;
				x = (tableroIn[i-1,j].coordsVert.x + tableroIn[i,j].coordsVert.x)/2;
				y = (tableroIn[i-1,j].coordsVert.y + tableroIn[i,j].coordsVert.y)/2;
				z = (tableroIn[i-1,j].coordsVert.z + tableroIn[i,j].coordsVert.z)/2;
				coordsVert = new Vector3(x,y,z);
				pos = coordsVert - posicionPlaneta;
				//0.45f es la extrusion
				pos = (0.45f/512) * pos.normalized * altura;
				coordsVert = coordsVert + pos;				
				tableroOut[i,j].coordsVert = coordsVert;
			}
		}
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
		GameObject contenedor = new GameObject("contenedor" + creacion.name);
		creacion.transform.position = Vector3.zero;
		creacion.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		contenedor.transform.parent = GameObject.FindGameObjectWithTag("Planeta").transform;
		creacion.transform.parent = contenedor.transform;
		contenedor.transform.position = posicion;
		Vector3 normal = posicion - contenedor.transform.parent.position;
		creacion.transform.localRotation = Quaternion.identity;
		contenedor.transform.rotation = Quaternion.LookRotation(normal);
		
		//creacion.transform.Rotate(0,0,UnityEngine.Random.Range(0,180),Space.Self);

		return contenedor;
	}
	
	public static Mesh extruyeVerticesTex(Mesh mesh, Texture2D tex, float extrusion, Vector3 centro) {
		Vector3[] verts = mesh.vertices;
		Vector3[] vertsRes = new Vector3[verts.Length];
		Color[] colores = tex.GetPixels();
		for (int i = 0; i < verts.Length; i++) {
			Vector2 cord = new Vector2(mesh.uv[i].x, mesh.uv[i].y);
			cord.x *= tex.width;
			cord.y *= tex.height;
//			Color col = tex.GetPixel((int)cord.x, (int)cord.y);
			Color col = colores[((int)cord.x) + (((int)cord.y) * tex.width)];
			Vector3 normal = verts[i] - centro;
			Vector3 desplazamiento = (extrusion/512 * normal.normalized * col.grayscale);
			vertsRes[i] = verts[i] + desplazamiento;	
		}
		
		Mesh resultado = GameObject.Instantiate(mesh) as Mesh;
		resultado.vertices = vertsRes;
		return resultado;
	}
	
	public static Mesh extruyeVerticesValor(Mesh mesh, float valor, float extrusion, Vector3 centro) {
		Vector3[] verts = mesh.vertices;
		Vector3[] vertsRes = new Vector3[verts.Length];
		for (int i = 0; i < verts.Length; i++) {
			Vector3 normal = verts[i] - centro;
			Vector3 desplazamiento = (extrusion/512 * normal.normalized * valor);
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
	
	public static void quitaPerlin() {
		perlin = null;
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
		int fraction = ((int)(Time.realtimeSinceStartup * 100f)) % 100;
		result = System.String.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
		return result;
	}
	
	public static string formateaFechaPasos (int num)
	{
		string result = "";
		//Fecha de partida--- Representada con un año de 150 dias
		int annoBase = 2325;
		int diaBase = 11;
		int diasPorCiclo = 150;
		//-------------------
		int diaMes = (num + diaBase) % diasPorCiclo;
		int anno = (num + diaBase) / diasPorCiclo;
		annoBase += anno;		
		result = System.String.Format ("Dia: {0,3} A/u00f1o: {1,4}", diaMes, annoBase);
		return result;
	}
	
	public static List<Tupla<int,int>> calculaPosicionesColindantes()
	{		
		List<Tupla<int, int>> posicionesColindantes = new List<Tupla<int, int>>();
		posicionesColindantes.Add(new Tupla<int,int>(-1,-1));
		posicionesColindantes.Add(new Tupla<int,int>(0,-1));
		posicionesColindantes.Add(new Tupla<int,int>(1,-1));
		posicionesColindantes.Add(new Tupla<int,int>(1,0));
		posicionesColindantes.Add(new Tupla<int,int>(1,1));
		posicionesColindantes.Add(new Tupla<int,int>(0,1));
		posicionesColindantes.Add(new Tupla<int,int>(-1,1));
		posicionesColindantes.Add(new Tupla<int,int>(-1,0));
		return posicionesColindantes;
	}
	
	public static List<Tupla<int,int,bool>> calculaMatrizRadio2Circular(int x, int y)
	{
		int auxX,auxY;
		List<Tupla<int,int,bool>> matriz = new List<Tupla<int,int,bool>>();
		for(int i = -2; i <= 2; i++)
			for(int j = -2; j <= 2; j++)
			{
				auxX = x + i;
				auxY = y + j;
				convierteCoordenadas(ref auxX, ref auxY);
				matriz.Add(new Tupla<int,int,bool>(auxX,auxY,true));
			}	
		matriz[0] = new Tupla<int, int, bool>(matriz[0].e1,matriz[0].e2,false);
		matriz[4] = new Tupla<int, int, bool>(matriz[4].e1,matriz[4].e2,false);
		matriz[20] = new Tupla<int, int, bool>(matriz[20].e1,matriz[20].e2,false);
		matriz[24] = new Tupla<int, int, bool>(matriz[24].e1,matriz[24].e2,false);		
		return matriz;
	}
	
	public static List<Tupla<int,int,bool>> calculaMatrizRadio3Circular(int x, int y)
	{
		int auxX,auxY;
		List<Tupla<int,int,bool>> matriz = new List<Tupla<int,int,bool>>();
		for(int i = -3; i <= 3; i++)
			for(int j = -3; j <= 3; j++)
			{
				auxX = x + i;
				auxY = y + j;
				convierteCoordenadas(ref auxY, ref auxX);
				matriz.Add(new Tupla<int,int,bool>(auxY,auxX,true));
			}	
		matriz[0] = new Tupla<int, int, bool>(matriz[0].e1,matriz[0].e2,false);
		matriz[1] = new Tupla<int, int, bool>(matriz[1].e1,matriz[1].e2,false);
		matriz[5] = new Tupla<int, int, bool>(matriz[5].e1,matriz[5].e2,false);
		matriz[6] = new Tupla<int, int, bool>(matriz[6].e1,matriz[6].e2,false);
		matriz[7] = new Tupla<int, int, bool>(matriz[7].e1,matriz[7].e2,false);
		matriz[13] = new Tupla<int, int, bool>(matriz[13].e1,matriz[13].e2,false);
		matriz[35] = new Tupla<int, int, bool>(matriz[35].e1,matriz[35].e2,false);
		matriz[41] = new Tupla<int, int, bool>(matriz[41].e1,matriz[41].e2,false);
		matriz[42] = new Tupla<int, int, bool>(matriz[42].e1,matriz[42].e2,false);
		matriz[43] = new Tupla<int, int, bool>(matriz[43].e1,matriz[43].e2,false);
		matriz[47] = new Tupla<int, int, bool>(matriz[47].e1,matriz[47].e2,false);
		matriz[48] = new Tupla<int, int, bool>(matriz[48].e1,matriz[48].e2,false);		
		return matriz;
	}
		
	public static List<Tupla<int,int,bool>> calculaMatrizRadio4Circular(int x, int y)
	{
		int auxX,auxY;
		List<Tupla<int,int,bool>> matriz = new List<Tupla<int,int,bool>>();
		for(int i = -4; i <= 4; i++)
			for(int j = -4; j <= 4; j++)
			{
				auxX = x + i;
				auxY = y + j;
				convierteCoordenadas(ref auxX, ref auxY);
				matriz.Add(new Tupla<int,int,bool>(auxX,auxY,true));
			}	
		matriz[0] = new Tupla<int, int, bool>(matriz[0].e1,matriz[0].e2,false);
		matriz[1] = new Tupla<int, int, bool>(matriz[1].e1,matriz[1].e2,false);
		matriz[2] = new Tupla<int, int, bool>(matriz[2].e1,matriz[2].e2,false);
		matriz[6] = new Tupla<int, int, bool>(matriz[6].e1,matriz[6].e2,false);
		matriz[7] = new Tupla<int, int, bool>(matriz[7].e1,matriz[7].e2,false);
		matriz[8] = new Tupla<int, int, bool>(matriz[8].e1,matriz[8].e2,false);
		matriz[9] = new Tupla<int, int, bool>(matriz[9].e1,matriz[9].e2,false);
		matriz[17] = new Tupla<int, int, bool>(matriz[17].e1,matriz[17].e2,false);
		matriz[18] = new Tupla<int, int, bool>(matriz[18].e1,matriz[18].e2,false);
		matriz[26] = new Tupla<int, int, bool>(matriz[26].e1,matriz[26].e2,false);
		matriz[54] = new Tupla<int, int, bool>(matriz[54].e1,matriz[54].e2,false);
		matriz[62] = new Tupla<int, int, bool>(matriz[62].e1,matriz[62].e2,false);
		matriz[63] = new Tupla<int, int, bool>(matriz[63].e1,matriz[63].e2,false);
		matriz[71] = new Tupla<int, int, bool>(matriz[71].e1,matriz[71].e2,false);
		matriz[72] = new Tupla<int, int, bool>(matriz[72].e1,matriz[72].e2,false);
		matriz[73] = new Tupla<int, int, bool>(matriz[73].e1,matriz[73].e2,false);
		matriz[74] = new Tupla<int, int, bool>(matriz[74].e1,matriz[74].e2,false);
		matriz[78] = new Tupla<int, int, bool>(matriz[78].e1,matriz[78].e2,false);
		matriz[79] = new Tupla<int, int, bool>(matriz[79].e1,matriz[79].e2,false);
		matriz[80] = new Tupla<int, int, bool>(matriz[80].e1,matriz[80].e2,false);
		return matriz;
	}
	public static List<Tupla<int,int,bool>> calculaMatrizRadio5Circular(int x, int y)
	{
		int auxX,auxY;
		List<Tupla<int,int,bool>> matriz = new List<Tupla<int,int,bool>>();
		for(int i = -5; i <= 5; i++)
			for(int j = -5; j <= 5; j++)
			{
				auxX = x + i;
				auxY = y + j;
				convierteCoordenadas(ref auxX, ref auxY);
				matriz.Add(new Tupla<int,int,bool>(auxX,auxY,true));
			}	
		matriz[0] = new Tupla<int, int, bool>(matriz[0].e1,matriz[0].e2,false);
		matriz[1] = new Tupla<int, int, bool>(matriz[1].e1,matriz[1].e2,false);
		matriz[2] = new Tupla<int, int, bool>(matriz[2].e1,matriz[2].e2,false);
		matriz[3] = new Tupla<int, int, bool>(matriz[3].e1,matriz[3].e2,false);
		matriz[7] = new Tupla<int, int, bool>(matriz[7].e1,matriz[7].e2,false);
		matriz[8] = new Tupla<int, int, bool>(matriz[8].e1,matriz[8].e2,false);
		matriz[9] = new Tupla<int, int, bool>(matriz[9].e1,matriz[9].e2,false);
		matriz[10] = new Tupla<int, int, bool>(matriz[10].e1,matriz[10].e2,false);			
		matriz[11] = new Tupla<int, int, bool>(matriz[11].e1,matriz[11].e2,false);			
		matriz[12] = new Tupla<int, int, bool>(matriz[12].e1,matriz[12].e2,false);			
		matriz[20] = new Tupla<int, int, bool>(matriz[20].e1,matriz[20].e2,false);
		matriz[21] = new Tupla<int, int, bool>(matriz[21].e1,matriz[21].e2,false);			
		matriz[22] = new Tupla<int, int, bool>(matriz[22].e1,matriz[22].e2,false);			
		matriz[32] = new Tupla<int, int, bool>(matriz[32].e1,matriz[32].e2,false);			
		matriz[33] = new Tupla<int, int, bool>(matriz[33].e1,matriz[33].e2,false);			
		matriz[43] = new Tupla<int, int, bool>(matriz[43].e1,matriz[43].e2,false);			
		matriz[77] = new Tupla<int, int, bool>(matriz[77].e1,matriz[77].e2,false);			
		matriz[87] = new Tupla<int, int, bool>(matriz[87].e1,matriz[87].e2,false);			
		matriz[88] = new Tupla<int, int, bool>(matriz[88].e1,matriz[88].e2,false);			
		matriz[98] = new Tupla<int, int, bool>(matriz[98].e1,matriz[98].e2,false);			
		matriz[99] = new Tupla<int, int, bool>(matriz[99].e1,matriz[99].e2,false);			
		matriz[100] = new Tupla<int, int, bool>(matriz[100].e1,matriz[100].e2,false);			
		matriz[111] = new Tupla<int, int, bool>(matriz[111].e1,matriz[111].e2,false);			
		matriz[112] = new Tupla<int, int, bool>(matriz[112].e1,matriz[112].e2,false);			
		matriz[113] = new Tupla<int, int, bool>(matriz[113].e1,matriz[113].e2,false);			
		matriz[117] = new Tupla<int, int, bool>(matriz[117].e1,matriz[117].e2,false);			
		matriz[118] = new Tupla<int, int, bool>(matriz[118].e1,matriz[118].e2,false);			
		matriz[119] = new Tupla<int, int, bool>(matriz[119].e1,matriz[119].e2,false);			
		matriz[120] = new Tupla<int, int, bool>(matriz[120].e1,matriz[120].e2,false);			
		return matriz;
	}
	public static List<Tupla<int,int,bool>> calculaMatrizRadio6Circular(int x, int y)
	{
		int auxX,auxY;
		List<Tupla<int,int,bool>> matriz = new List<Tupla<int,int,bool>>();
		for(int i = -6; i <= 6; i++)
			for(int j = -6; j <= 6; j++)
			{
				auxX = x + i;
				auxY = y + j;
				convierteCoordenadas(ref auxX, ref auxY);
				matriz.Add(new Tupla<int,int,bool>(auxX,auxY,true));
			}	
		matriz[0] = new Tupla<int, int, bool>(matriz[0].e1,matriz[0].e2,false);
		matriz[1] = new Tupla<int, int, bool>(matriz[1].e1,matriz[1].e2,false);
		matriz[2] = new Tupla<int, int, bool>(matriz[2].e1,matriz[2].e2,false);
		matriz[3] = new Tupla<int, int, bool>(matriz[3].e1,matriz[3].e2,false);
		matriz[9] = new Tupla<int, int, bool>(matriz[9].e1,matriz[9].e2,false);
		matriz[10] = new Tupla<int, int, bool>(matriz[10].e1,matriz[10].e2,false);			
		matriz[11] = new Tupla<int, int, bool>(matriz[11].e1,matriz[11].e2,false);			
		matriz[12] = new Tupla<int, int, bool>(matriz[12].e1,matriz[12].e2,false);			
		matriz[13] = new Tupla<int, int, bool>(matriz[13].e1,matriz[13].e2,false);			
		matriz[14] = new Tupla<int, int, bool>(matriz[14].e1,matriz[14].e2,false);			
		matriz[15] = new Tupla<int, int, bool>(matriz[15].e1,matriz[15].e2,false);			
		matriz[23] = new Tupla<int, int, bool>(matriz[23].e1,matriz[23].e2,false);			
		matriz[24] = new Tupla<int, int, bool>(matriz[24].e1,matriz[24].e2,false);			
		matriz[25] = new Tupla<int, int, bool>(matriz[25].e1,matriz[25].e2,false);			
		matriz[26] = new Tupla<int, int, bool>(matriz[26].e1,matriz[26].e2,false);			
		matriz[27] = new Tupla<int, int, bool>(matriz[27].e1,matriz[27].e2,false);			
		matriz[37] = new Tupla<int, int, bool>(matriz[37].e1,matriz[37].e2,false);			
		matriz[38] = new Tupla<int, int, bool>(matriz[38].e1,matriz[38].e2,false);			
		matriz[39] = new Tupla<int, int, bool>(matriz[39].e1,matriz[39].e2,false);			
		matriz[51] = new Tupla<int, int, bool>(matriz[51].e1,matriz[51].e2,false);			
		matriz[117] = new Tupla<int, int, bool>(matriz[117].e1,matriz[117].e2,false);			
		matriz[129] = new Tupla<int, int, bool>(matriz[129].e1,matriz[129].e2,false);			
		matriz[130] = new Tupla<int, int, bool>(matriz[130].e1,matriz[130].e2,false);			
		matriz[131] = new Tupla<int, int, bool>(matriz[131].e1,matriz[131].e2,false);			
		matriz[141] = new Tupla<int, int, bool>(matriz[141].e1,matriz[141].e2,false);			
		matriz[142] = new Tupla<int, int, bool>(matriz[142].e1,matriz[142].e2,false);			
		matriz[143] = new Tupla<int, int, bool>(matriz[143].e1,matriz[143].e2,false);			
		matriz[144] = new Tupla<int, int, bool>(matriz[144].e1,matriz[144].e2,false);			
		matriz[145] = new Tupla<int, int, bool>(matriz[145].e1,matriz[145].e2,false);			
		matriz[153] = new Tupla<int, int, bool>(matriz[153].e1,matriz[153].e2,false);			
		matriz[154] = new Tupla<int, int, bool>(matriz[154].e1,matriz[154].e2,false);			
		matriz[155] = new Tupla<int, int, bool>(matriz[155].e1,matriz[155].e2,false);			
		matriz[156] = new Tupla<int, int, bool>(matriz[156].e1,matriz[156].e2,false);			
		matriz[157] = new Tupla<int, int, bool>(matriz[157].e1,matriz[157].e2,false);			
		matriz[158] = new Tupla<int, int, bool>(matriz[158].e1,matriz[158].e2,false);			
		matriz[159] = new Tupla<int, int, bool>(matriz[159].e1,matriz[159].e2,false);			
		matriz[165] = new Tupla<int, int, bool>(matriz[165].e1,matriz[165].e2,false);			
		matriz[166] = new Tupla<int, int, bool>(matriz[166].e1,matriz[166].e2,false);			
		matriz[167] = new Tupla<int, int, bool>(matriz[167].e1,matriz[167].e2,false);			
		matriz[168] = new Tupla<int, int, bool>(matriz[168].e1,matriz[168].e2,false);			
		return matriz;
	}
}