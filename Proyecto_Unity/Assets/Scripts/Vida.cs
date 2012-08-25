using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public class Casilla {
	public T_habitats habitat;
	public T_elementos elementos;
	public Vector2 coordsTex;
	public Vector3 coordsVert;
	public Vegetal vegetal;
	public Animal animal;
	public Edificio edificio;
	public Vector2[] pinceladas;
	
	public Casilla(T_habitats hab, T_elementos elems, Vector2 coord, Vector3 vert) {
		habitat = hab;
		elementos = elems;
		coordsTex = coord;
		vegetal = null;
		animal = null;
		edificio = null;
		coordsVert = vert;
	}
	
	public Casilla() {}
}

[System.Serializable] 
public class Vida //: MonoBehaviour
{
	//Referencia a la textura de las plantas
	public Texture2D texturaPlantas;
	//Transform del objeto roca, para mover los meshes
	public Transform objetoRoca;
	//Estructuras
	public Casilla[,] tablero;										//Tablero lógico que representa las casillas
	public List<Especie> especies;									//Listado de todas las especies
	public List<EspecieVegetal> especiesVegetales;					//Listado de todas las especies vegetales
	public List<EspecieAnimal> especiesAnimales;					//Listado de todas las especies animales
	public List<TipoEdificio> tiposEdificios;						//Listado de todos los tipos de edificios
	public List<Ser> seres;											//Listado de todos los seres
	public List<Vegetal> vegetales;									//Listado de todos los vegetales
	public List<Animal> animales;									//Listado de todos los animales
	public List<Edificio> edificios;								//Listado de todos los edificios	
	//public List<Ser>[] listadoSeresTurnos;							//Listado que contiene los turnos en los que cada ser ejecuta algoritmo
	
	//public int numMaxTurnos;
	//public int turnoActual;
	public int numEspecies;
	public int numEspeciesVegetales;
	public int numEspeciesAnimales;
	public int numTiposEdificios;
	
	public int idActualVegetal;
	public int idActualAnimal;
	public int idActualEdificio;
	
	public int contadorPintarTexturaPlantas = 0;
	public bool texturaPlantasModificado = false;
	
	public List<Tupla<int,int>> posicionesColindantes;
	public int[,] matrizRadio3Circular;
	public int[,] matrizRadio4Circular;
	public int[,] matrizRadio5Circular;
	public int[,] matrizRadio6Circular;
	
	public Vida()
	{
		especies = new List<Especie>();
		especiesVegetales = new List<EspecieVegetal>();
		especiesAnimales = new List<EspecieAnimal>();
		tiposEdificios = new List<TipoEdificio>();
		seres = new List<Ser>();	
		vegetales = new List<Vegetal>();
		animales = new List<Animal>();
		edificios = new List<Edificio>();
		//numMaxTurnos = 0;
		//turnoActual = 0;
		//listadoSeresTurnos = new List<Ser>[numMaxTurnos];
		numEspecies = 0;
		numEspeciesVegetales = 0;
		numEspeciesAnimales = 0;
		numTiposEdificios = 0;
		idActualVegetal = 0;
		idActualAnimal = 0;
		idActualEdificio = 0;
		posicionesColindantes = FuncTablero.calculaPosicionesColindantes();
	}	
	
	public Vida(Casilla[,] tablero, Texture2D texPlantas, Transform objeto)
	{
		this.tablero = tablero;
		especies = new List<Especie>();
		especiesVegetales = new List<EspecieVegetal>();
		especiesAnimales = new List<EspecieAnimal>();
		tiposEdificios = new List<TipoEdificio>();
		seres = new List<Ser>();	
		vegetales = new List<Vegetal>();
		animales = new List<Animal>();
		edificios = new List<Edificio>();		
		//numMaxTurnos = 0;
		//turnoActual = 0;
		//listadoSeresTurnos = new List<Ser>[numMaxTurnos];
		numEspecies = 0;
		numEspeciesVegetales = 0;
		numEspeciesAnimales = 0;
		idActualVegetal = 0;
		idActualAnimal = 0;
		idActualEdificio = 0;
		texturaPlantas = texPlantas;
		objetoRoca = objeto;
		posicionesColindantes = FuncTablero.calculaPosicionesColindantes();
	}
	
	public Vida(Casilla[,] tablero, Texture2D texPlantas)
	{
		this.tablero = tablero;
		especies = new List<Especie>();
		especiesVegetales = new List<EspecieVegetal>();
		especiesAnimales = new List<EspecieAnimal>();
		tiposEdificios = new List<TipoEdificio>();
		seres = new List<Ser>();	
		vegetales = new List<Vegetal>();
		animales = new List<Animal>();
		edificios = new List<Edificio>();
		//numMaxTurnos = 0;
		//turnoActual = 0;
		//listadoSeresTurnos = new List<Ser>[numMaxTurnos];
		numEspecies = 0;
		numEspeciesVegetales = 0;
		numEspeciesAnimales = 0;
		idActualVegetal = 0;
		idActualAnimal = 0;
		idActualEdificio = 0;
		texturaPlantas = texPlantas;
		posicionesColindantes = FuncTablero.calculaPosicionesColindantes();
	}
	
	public Vida(Vida vida)
	{
		objetoRoca = vida.objetoRoca;
		tablero = vida.tablero;
		especies = vida.especies;
		especiesVegetales = vida.especiesVegetales;
		especiesAnimales = vida.especiesAnimales;
		tiposEdificios = vida.tiposEdificios;
		seres = vida.seres;
		vegetales = vida.vegetales;		
		animales = vida.animales;
		edificios = vida.edificios;
		//numMaxTurnos = vida.numMaxTurnos;
		//turnoActual = vida.turnoActual;
		//listadoSeresTurnos = vida.listadoSeresTurnos;
		numEspecies = vida.numEspecies;
		numEspeciesVegetales = vida.numEspeciesVegetales;
		numEspeciesAnimales = vida.numEspeciesAnimales;
		numTiposEdificios = vida.numTiposEdificios;
		idActualVegetal = vida.idActualVegetal;
		idActualAnimal = vida.idActualAnimal;
		idActualEdificio = vida.idActualEdificio;
		texturaPlantas = vida.texturaPlantas;
		posicionesColindantes = FuncTablero.calculaPosicionesColindantes();
	}
	
	/*public void actualizaNumTurnos()
	{
		numMaxTurnos = 0;
		for(int i = 0; i < tiposEdificios.Count;i++)
			if(tiposEdificios[i].siguienteTurno > numMaxTurnos)
				numMaxTurnos = tiposEdificios[i].siguienteTurno;
		
		for(int i = 0; i < especies.Count;i++)
			if(especies[i].siguienteTurno > numMaxTurnos)
				numMaxTurnos = especies[i].siguienteTurno;
		
		numMaxTurnos++;
		turnoActual = 0;
		listadoSeresTurnos = new List<Ser>[numMaxTurnos];
		for(int i = 0; i < listadoSeresTurnos.Length; i++)
		{
			listadoSeresTurnos[i] = new List<Ser>();
		}
	}*/
	
	//Necesario para cuando se crea Vida desde la escena inicial, donde el objeto roca no está creado aun
	public void setObjetoRoca(Transform objeto) {
		objetoRoca = objeto;
	}
	
	private void pintaPlantasTex(int posX,int posY) {
		Vegetal veg = tablero[posX,posY].vegetal;
		texturaPlantasModificado = true;
		if (veg != null && veg.numVegetales > 0) {
			int temp = (int)Mathf.Lerp(0.0f, 4.0f, veg.numVegetales / veg.especie.numMaxVegetales);
			if (tablero[posX,posY].pinceladas != null) {
				if (tablero[posX,posY].pinceladas.Length < temp) {
					Vector2[] arrayPos = new Vector2[temp];
					for (int j = 0; j < tablero[posX,posY].pinceladas.Length; j++) {
						arrayPos[j] = tablero[posX,posY].pinceladas[j];
					}
					for (int i = tablero[posX,posY].pinceladas.Length; i < temp; i++) {
						int tempX = (int)(tablero[posX,posY].coordsTex.x + UnityEngine.Random.Range(0, FuncTablero.getRelTexTabAncho()+1));
						int tempY =  (int)(tablero[posX,posY].coordsTex.y + UnityEngine.Random.Range(0, FuncTablero.getRelTexTabAlto()+1));
						Vector2 posTemp = new Vector2(tempX, tempY);
						arrayPos[i] = posTemp;
						FuncTablero.pintaPlantas(texturaPlantas, posTemp, veg.especie.idTextura, true);
					}
					tablero[posX,posY].pinceladas = arrayPos;
				}
				else if (tablero[posX,posY].pinceladas.Length > temp) {
					Vector2[] arrayPos = new Vector2[temp];
					for (int j = 0; j < temp; j++) {
						arrayPos[j] = tablero[posX,posY].pinceladas[j];
					}
					for (int i = temp; i < tablero[posX,posY].pinceladas.Length; i++) {
						FuncTablero.pintaPlantas(texturaPlantas, tablero[posX,posY].pinceladas[i], veg.especie.idTextura, false);
					}
					tablero[posX,posY].pinceladas = arrayPos;
				}
			}
			else {
				tablero[posX,posY].pinceladas = new Vector2[temp];
				for (int i = 0; i < temp; i++) {
					int tempX = (int)(tablero[posX,posY].coordsTex.x + UnityEngine.Random.Range(0, FuncTablero.getRelTexTabAncho()+1));
					int tempY =  (int)(tablero[posX,posY].coordsTex.y + UnityEngine.Random.Range(0, FuncTablero.getRelTexTabAlto()+1));
					tablero[posX,posY].pinceladas[i] = new Vector2(tempX, tempY);
					FuncTablero.pintaPlantas(texturaPlantas, tablero[posX,posY].pinceladas[i], veg.especie.idTextura, true);
				}
			}
		}
	}
	
	//Devuelve true si hay un vegetal en la casilla [x,y] y false si no lo hay
	public bool tieneVegetal(int x,int y)
	{		
		return tablero[x,y].vegetal != null;				
	}
	
	//Devuelve true si hay un animal en la casilla [x,y] y false si no lo hay
	public bool tieneAnimal(int x,int y)
	{
		return tablero[x,y].animal != null;		
	}
	
	//Devuelve true si hay un edificio en la casilla [x,y] y false si no lo hay
	public bool tieneEdificio(int x,int y)
	{
		return tablero[x,y].edificio != null;		
	}
	
	//Devuelve false si la especie ya existe (no se añade) y true si se añade correctamente
	public bool anadeEspecieVegetal(EspecieVegetal especie)
	{		
		if(especies.Contains(especie))
			return false;
		especie.idEspecie = numEspeciesVegetales;
		especies.Add(especie);
		numEspecies++;
		especiesVegetales.Add(especie);
		numEspeciesVegetales++;
		return true;
	}
	
	//Devuelve false si la especie ya existe (no se añade) y true si se añade correctamente
	public bool anadeEspecieAnimal(EspecieAnimal especie)
	{		
		if(especies.Contains(especie))
			return false;
		especie.idEspecie = numEspeciesAnimales;
		especies.Add(especie);
		numEspecies++;
		especiesAnimales.Add(especie);
		numEspeciesAnimales++;
		return true;
	}
	
	//Devuelve false si la especie ya existe (no se añade) y true si se añade correctamente
	public bool anadeTipoEdificio(TipoEdificio tipoEdificio)
	{			
		if(tiposEdificios.Contains(tipoEdificio))
			return false;
		tipoEdificio.idTipoEdificio = numTiposEdificios;
		tiposEdificios.Add(tipoEdificio);
		numTiposEdificios++;
		return true;
	}
	
	//Devuelve false si la especie no existe (no se elimina) y true si se elimina correctamente
	public bool eliminaEspecieVegetal(EspecieVegetal especie)
	{		
		if(!especies.Contains(especie))
			return false;
		especies.Remove(especie);
		numEspecies--;
		especiesVegetales.Remove(especie);
		numEspeciesVegetales--;
		return true;
	}
	
	//Devuelve false si la especie no existe (no se elimina) y true si se elimina correctamente
	public bool eliminaEspecieAnimal(EspecieAnimal especie)
	{		
		if(!especies.Contains(especie))
			return false;
		especies.Remove(especie);
		numEspecies--;
		especiesAnimales.Remove(especie);
		numEspeciesAnimales--;
		return true;
	}
	
	//Devuelve false si el edificio no existe (no se elimina) y true si se elimina correctamente
	public bool eliminaTipoEdificio(TipoEdificio tipoEdificio)
	{		
		if(!tiposEdificios.Contains(tipoEdificio))
			return false;
		tiposEdificios.Remove(tipoEdificio);
		numTiposEdificios--;
		return true;
	}
	
	public Vector3 posicionAleatoriaVegetal(int posX,int posY)
	{		
		int xIzq = posX;
		int yIzq = posY-1;
		FuncTablero.convierteCoordenadas(ref xIzq,ref yIzq);
		int xDer = posX;
		int yDer = posY+1;
		FuncTablero.convierteCoordenadas(ref xDer,ref yDer);
		int xSup = posX+1;
		int ySup = posY;
		FuncTablero.convierteCoordenadas(ref xSup,ref ySup);
		int xInf = posX-1;
		int yInf = posY;
		FuncTablero.convierteCoordenadas(ref xInf,ref yInf);
		Vector3 centro = tablero[posX,posY].coordsVert;
		Vector3 izquierdo = tablero[xIzq,yIzq].coordsVert;
		Vector3 derecho = tablero[xDer,yDer].coordsVert;
		Vector3 superior = tablero[xSup,ySup].coordsVert;
		Vector3 inferior = tablero[xInf,yInf].coordsVert;
		
		float x,y,z,pos;
		
		if(UnityEngine.Random.Range(0,2) == 0)						//Izquierda
		{
			pos = UnityEngine.Random.Range(0.0f,0.45f);
			x = (1-pos)*centro.x + pos*izquierdo.x;
			y = (1-pos)*centro.y + pos*izquierdo.y;
			z = (1-pos)*centro.z + pos*izquierdo.z;
		}
		else														//Derecha
		{
			pos = UnityEngine.Random.Range(0.0f,0.45f);
			x = (1-pos)*centro.x + pos*derecho.x;
			y = (1-pos)*centro.y + pos*derecho.y;
			z = (1-pos)*centro.z + pos*derecho.z;			
		}
		if(UnityEngine.Random.Range(0,2) == 0)						//Arriba
		{
			pos = UnityEngine.Random.Range(0.0f,0.45f);
			x = (1-pos)*x + pos*superior.x;
			y = (1-pos)*y + pos*superior.y;
			z = (1-pos)*z + pos*superior.z;
		}
		else														//Abajo
		{
			pos = UnityEngine.Random.Range(0.0f,0.45f);
			x = (1-pos)*x + pos*inferior.x;
			y = (1-pos)*y + pos*inferior.y;
			z = (1-pos)*z + pos*inferior.z;			
		}
		return new Vector3 (x, y, z);
	}
		
	//Devuelve si el vegetal se puede insertar en esa posición o no
	public bool compruebaAnadeVegetal(EspecieVegetal especie,List<float> habitabilidad,float habitabilidadMinima,int posX,int posY)
	{
		return(!tieneEdificio(posX,posY) && !tieneVegetal(posX,posY) && habitabilidad[(int)tablero[posX,posY].habitat] > habitabilidadMinima);	
	}	
	
	//Devuelve false si el vegetal ya existe (no se añade) y true si se añade correctamente	
	public bool anadeVegetal(EspecieVegetal especie,List<float> habitabilidad,float habitabilidadMinima,int posX,int posY)
	{
		if(tieneEdificio(posX,posY) || tieneVegetal(posX,posY) || habitabilidad[(int)tablero[posX,posY].habitat] == habitabilidadMinima)
			return false;
		GameObject modelo = especie.modelos[UnityEngine.Random.Range(0,especie.modelos.Count)];
		Vector3 coordsVert = posicionAleatoriaVegetal(posX,posY);
		//Vector3 coordsVert = tablero[posX,posY].coordsVert;
		Vegetal vegetal = new Vegetal(idActualVegetal,especie,posX,posY,habitabilidad,tablero[posX,posY].habitat,FuncTablero.creaMesh(coordsVert, modelo));
		vegetal.modelo.transform.position = objetoRoca.TransformPoint(vegetal.modelo.transform.position);
		seres.Add(vegetal);
		//int turno = (turnoActual + especie.siguienteTurno)%numMaxTurnos;
		//listadoSeresTurnos[turno].Add(vegetal);
		idActualVegetal++;
		vegetales.Add(vegetal);
		tablero[posX,posY].vegetal = vegetal;
		especie.numSeresEspecie++;
		pintaPlantasTex(posX,posY);
		return true;	
	}
	
	//Devuelve si el animal se puede insertar en esa posición o no
	public bool compruebaAnadeAnimal(EspecieAnimal especie,int posX,int posY)
	{
		return(!tieneEdificio(posX,posY) && !tieneAnimal(posX,posY) && especie.tieneHabitat(tablero[posX,posY].habitat));
	}
	
	//Devuelve false si el animal ya existe (no se añade) y true si se añade correctamente	
	public bool anadeAnimal(EspecieAnimal especie,int posX,int posY)
	{
		if(tieneEdificio(posX,posY) || tieneAnimal(posX,posY) || !especie.tieneHabitat(tablero[posX,posY].habitat))
			return false;
		GameObject modelo = especie.modelos[UnityEngine.Random.Range(0,especie.modelos.Count)];
		Vector3 coordsVert = tablero[posX,posY].coordsVert;
		Animal animal = new Animal(idActualAnimal,especie,posX,posY,FuncTablero.creaMesh(coordsVert, modelo));
		animal.modelo.transform.position = objetoRoca.TransformPoint(animal.modelo.transform.position);
		seres.Add(animal);		
		//int turno = (turnoActual + especie.siguienteTurno)%numMaxTurnos;
		//listadoSeresTurnos[turno].Add(animal);
		idActualAnimal++;
		animales.Add(animal);		
		tablero[posX,posY].animal = animal;
		especie.numSeresEspecie++;
		Debug.Log("A\u00F1adido animal");		
		return true;
	}
	
	//Devuelve si el edificio se puede insertar en esa posición o no
	public bool compruebaAnadeEdificio(TipoEdificio tipoEdificio,int posX,int posY)
	{
		return(!tieneEdificio(posX,posY) && tipoEdificio.tieneHabitat(tablero[posX,posY].habitat));
	}
	
	//Devuelve false si el edificio ya existe (no se añade) y true si se añade correctamente	
	public bool anadeEdificio(TipoEdificio tipoEdificio,int posX,int posY,float eficiencia)
	{
		if(tieneEdificio(posX,posY) || !tipoEdificio.tieneHabitat(tablero[posX,posY].habitat))
			return false;
		GameObject modelo = tipoEdificio.modelos[UnityEngine.Random.Range(0,tipoEdificio.modelos.Count)];
		Vector3 coordsVert = tablero[posX,posY].coordsVert;	
		Edificio edificio = new Edificio(idActualEdificio,tipoEdificio,posX,posY,eficiencia,FuncTablero.creaMesh(coordsVert,modelo));
		edificio.modelo.transform.position = objetoRoca.TransformPoint(edificio.modelo.transform.position);
		seres.Add(edificio);
		//int turno = (turnoActual + tipoEdificio.siguienteTurno)%numMaxTurnos;
		//listadoSeresTurnos[turno].Add(edificio);
		idActualEdificio++;		
		edificios.Add(edificio);		
		if(tablero[posX,posY].animal != null)		
			eliminaAnimal(tablero[posX,posY].animal);			
		if(tablero[posX,posY].vegetal != null)		
			eliminaVegetal(tablero[posX,posY].vegetal);			
		
		tablero[posX,posY].edificio = edificio;
		return true;
	}
	
	/*public bool anadeSer(Ser ser)
	{
		if(ser is Vegetal)
		{
			Vegetal vegetal = (Vegetal)ser;
			if(!compruebaAnadeVegetal(vegetal.especie,vegetal.habitabilidad,vegetal.posX,vegetal.posY))
				return false;
			int turno = (turnoActual + vegetal.especie.siguienteTurno)%numMaxTurnos;
			listadoSeresTurnos[turno].Add(vegetal);
			idActualVegetal++;
			vegetales.Add(vegetal);
			tablero[vegetal.posX,vegetal.posY].vegetal = vegetal;
			pintaPlantasTex(vegetal.posX,vegetal.posY);		
		}
		else if(ser is Animal)
		{
			Animal animal = (Animal)ser;
			if(!compruebaAnadeAnimal(animal.especie,animal.posX,animal.posY))
				return false;
			int turno = (turnoActual + animal.especie.siguienteTurno)%numMaxTurnos;
			listadoSeresTurnos[turno].Add(animal);
			idActualAnimal++;
			animales.Add(animal);		
			tablero[animal.posX,animal.posY].animal = animal;		
		}	
		else if(ser is Edificio)
		{
			Edificio edificio = (Edificio)ser;
			if(!compruebaAnadeEdificio(edificio.tipo,edificio.posX,edificio.posY))
				return false;
			int turno = (turnoActual + edificio.tipo.siguienteTurno)%numMaxTurnos;
			listadoSeresTurnos[turno].Add(edificio);
			idActualEdificio++;		
			edificios.Add(edificio);		
			tablero[edificio.posX,edificio.posY].edificio = edificio;				
		}
		seres.Add(ser);
		return true;
	}*/
	
	//Elimina un vegetal, si no existe devuelve false
	public bool eliminaVegetal(Vegetal vegetal)
	{
		if(!vegetales.Contains(vegetal))
			return false;
		UnityEngine.Object.Destroy(vegetal.modelo);
		//listadoSeresTurnos[turnoActual].Remove(vegetal);
		tablero[vegetal.posX,vegetal.posY].vegetal = null;
		seres.Remove(vegetal);
		vegetales.Remove(vegetal);
		vegetal.especie.numSeresEspecie--;
		return true;
	}
	
	//Elimina un animal, si no existe devuelve false
	public bool eliminaAnimal(Animal animal)
	{
		if(!animales.Contains(animal))
			return false;
		UnityEngine.Object.Destroy(animal.modelo);
		//listadoSeresTurnos[turnoActual].Remove(animal);
		tablero[animal.posX,animal.posY].animal = null;
		seres.Remove(animal);
		animales.Remove(animal);
		animal.especie.numSeresEspecie--;
		return true;
	}
	
	//Elimina un edificio, si no existe devuelve false
	public bool eliminaEdificio(Edificio edificio)
	{
		if(!edificios.Contains(edificio))
			return false;
		UnityEngine.Object.Destroy(edificio.modelo);
		//listadoSeresTurnos[turnoActual].Remove(edificio);
		tablero[edificio.posX,edificio.posY].edificio = null;
		seres.Remove(edificio);
		edificios.Remove(edificio);
		return true;
	}
	
	//Devuelve true si consigue migrar una especie a una nueva posicion y false si no
	public bool migraVegetal(EspecieVegetal especie, List<float> habitabilidad, int posX,int posY,int radio)
	{
		int difX = UnityEngine.Random.Range(-radio,radio+1);
		int difY = UnityEngine.Random.Range(-radio,radio+1);		
		int nposX = posX + difX;
		int nposY = posY + difY;				
		FuncTablero.convierteCoordenadas(ref nposX,ref nposY);		
		return anadeVegetal(especie,habitabilidad,-1.0f,nposX,nposY);
	}
	
	//Devuelve true si consigue desplazar al animal y false si no lo consigue
	public bool desplazaAnimal(Animal animal,int nposX,int nposY)
	{		
		FuncTablero.convierteCoordenadas(ref nposX,ref nposY);
		//while(animal.posX != nposX || animal.posY != nposY)		
		//{			
			if(!tieneEdificio(nposX,nposY) && !tieneAnimal(nposX,nposY) && animal.especie.tieneHabitat(tablero[nposX,nposY].habitat))
			{
				tablero[animal.posX,animal.posY].animal = null;
				animal.desplazarse(nposX,nposY);
				tablero[nposX,nposY].animal = animal;
				//Mover la malla
				/*float x = (tablero[nposX,nposY].coordsVert.x + tablero[nposX-1,nposY].coordsVert.x)/2;
				float y = (tablero[nposX,nposY].coordsVert.y + tablero[nposX-1,nposY].coordsVert.y)/2;
				float z = (tablero[nposX,nposY].coordsVert.z + tablero[nposX-1,nposY].coordsVert.z)/2;
				Vector3 coordsVert = new Vector3(x,y,z);
				*/
				Vector3 coordsVert = tablero[nposX,nposY].coordsVert;
				animal.modelo.transform.position = coordsVert;				
				Vector3 normal = animal.modelo.transform.position - animal.modelo.transform.parent.position;
				animal.modelo.transform.position = objetoRoca.TransformPoint(animal.modelo.transform.position);
				animal.modelo.transform.rotation = Quaternion.LookRotation(normal);
				return true;
			}	
			/*if(nposX > animal.posX) nposX--;
			else if(nposX < animal.posX) nposX++;
			if(nposY > animal.posY) nposY--;
			else if(nposY < animal.posY) nposY++;
			FuncTablero.convierteCoordenadas(ref nposX,ref nposY);*/				
		//}
		return false;
	}
	
	//Devuelve true si consigue crear un nuevo animal colindante a la posición de entrada y false si no lo consigue
	public bool reproduceAnimal(EspecieAnimal especie,int posX,int posY)
	{
		int nposX = posX + UnityEngine.Random.Range(-1,2);
		int nposY = posY + UnityEngine.Random.Range(-1,2);
		FuncTablero.convierteCoordenadas(ref nposX,ref nposY);
		return anadeAnimal(especie,nposX,nposY);
	}
	
	//Devuelve true si el animal se ha alimentado y false si no
	public bool buscaAlimentoAnimal(Animal animal)
	{
		int nPosX;
		int nPosY;		
		if(animal.especie.tipo == tipoAlimentacionAnimal.herbivoro)
		{									
			if(tablero[animal.posX,animal.posY].vegetal != null)
			{
				int comida = tablero[animal.posX,animal.posY].vegetal.consumeVegetales(animal.especie.alimentoMaxTurno);
				animal.ingiereAlimento(comida);
				if(tablero[animal.posX,animal.posY].vegetal.numVegetales <= 0)
					eliminaVegetal(tablero[animal.posX,animal.posY].vegetal);
				return true;				
			}
			else
			{
				FuncTablero.randomLista(posicionesColindantes);				
				for(int i = 0; i < posicionesColindantes.Count; i++)
				{
					nPosX = animal.posX + posicionesColindantes[i].e1;
					nPosY = animal.posY + posicionesColindantes[i].e2;					
					FuncTablero.convierteCoordenadas(ref nPosX,ref nPosY);
					if(desplazaAnimal(animal,nPosX,nPosY))
						break;
				}				
			}
		}
		else if(animal.especie.tipo == tipoAlimentacionAnimal.carnivoro)						
		{			
			FuncTablero.randomLista(posicionesColindantes);				
			for(int i = 0; i < posicionesColindantes.Count; i++)
			{
				nPosX = animal.posX + posicionesColindantes[i].e1;
				nPosY = animal.posY + posicionesColindantes[i].e2;					
				FuncTablero.convierteCoordenadas(ref nPosX,ref nPosY);
				if(tablero[nPosX,nPosY].animal != null)
					if(tablero[nPosX,nPosY].animal.especie.tipo == tipoAlimentacionAnimal.herbivoro)					
					{
						int comida = tablero[nPosX,nPosY].animal.reserva % animal.especie.alimentoMaxTurno;
						tablero[nPosX,nPosY].animal.morir();
						tablero[nPosX,nPosY].animal = null;
						animal.ingiereAlimento(comida);
						desplazaAnimal(animal,nPosX,nPosY);
						return true;					
					}
			}			
		}
		return false;
	}
	
	public void algoritmoVida(int numTurno)
	{
		Ser ser;
		Vegetal vegetal;
		Animal animal;
		Edificio edificio;
		
		for(int i = 0; i < seres.Count; i++)
		{
			ser = seres[i];
			if(ser is Vegetal)
			{
				vegetal = (Vegetal)ser;
				//Reproducción y muerte
				if(vegetal.reproduccionMuerte())
					pintaPlantasTex(vegetal.posX, vegetal.posY);
				else
				{					
					eliminaVegetal(vegetal);
					continue;
				}
				//Evolución
				//if(vegetal.evolucion)
				//	;
				//Migración
				if(vegetal.migracionLocal())
					migraVegetal(vegetal.especie,vegetal.habitabilidad,vegetal.posX,vegetal.posY,1);
				if(vegetal.migracionGlobal())
					migraVegetal(vegetal.especie,vegetal.habitabilidad,vegetal.posX,vegetal.posY,vegetal.especie.radioMigracion);				
			}
			else if(ser is Animal)
			{
				animal = (Animal)ser;
				if(!animal.consumirAlimento() && animal.estado != tipoEstadoAnimal.morir)
					animal.estado = tipoEstadoAnimal.morir;
				else if(animal.reserva > animal.especie.reservaMaxima*0.5)
				{
					if(animal.reproduccion() ) 
						reproduceAnimal(animal.especie,animal.posX,animal.posY);											
					animal.estado = tipoEstadoAnimal.descansar;
				}
				else
				{
					switch(animal.estado)
					{
					case tipoEstadoAnimal.buscarAlimento:
						if(animal.aguante == 0)					
							animal.estado = tipoEstadoAnimal.descansar;	
						else
							if(buscaAlimentoAnimal(animal))
								animal.estado = tipoEstadoAnimal.comer;					
						animal.aguante--;					
						break;
					case tipoEstadoAnimal.descansar:
						animal.aguante = animal.especie.aguanteInicial;
						animal.estado = tipoEstadoAnimal.buscarAlimento;					
						break;
					case tipoEstadoAnimal.comer:
						animal.estado = tipoEstadoAnimal.buscarAlimento;
						break;
					case tipoEstadoAnimal.nacer:
						animal.estado = tipoEstadoAnimal.buscarAlimento;
						break;
					case tipoEstadoAnimal.morir:
						eliminaAnimal(animal);
						continue;
					default:break;
					}
				}
			
			}	
			else if(ser is Edificio)
			{
				edificio = (Edificio)ser;				
			}
		}
		
		
		//turnoActual = numTurno % numMaxTurnos;
		//List<Ser> seresTurno = listadoSeresTurnos[turnoActual];	
		//if(turnoActual == 0)
			//FuncTablero.randomLista<Ser>(seresTurno);
		/*for(int i = 0; i < seresTurno.Count; i++)
		{
			ser = seresTurno[i];
			if(ser is Vegetal)
			{
				vegetal = (Vegetal)ser;
				//Reproducción y muerte
				if(vegetal.reproduccionMuerte())
					pintaPlantasTex(vegetal.posX, vegetal.posY);
				else
				{					
					eliminaVegetal(vegetal);
					continue;
				}
				//Evolución
				//if(vegetal.evolucion)
				//	;
				//Migración
				if(vegetal.migracionLocal())
					migraVegetal(vegetal.especie,vegetal.habitabilidad,vegetal.posX,vegetal.posY,1);
				if(vegetal.migracionGlobal())
					migraVegetal(vegetal.especie,vegetal.habitabilidad,vegetal.posX,vegetal.posY,vegetal.especie.radioMigracion);
				int turno = (turnoActual + vegetal.especie.siguienteTurno)%numMaxTurnos;
				listadoSeresTurnos[turno].Add(vegetal);	
				listadoSeresTurnos[turnoActual].Remove(vegetal);
			}
			else if(ser is Animal)
			{
				animal = (Animal)ser;
				if(!animal.consumirAlimento() && animal.estado != tipoEstadoAnimal.morir)
					animal.estado = tipoEstadoAnimal.morir;
				else if(animal.reserva > animal.especie.reservaMaxima*0.5)
				{
					if(animal.reproduccion()) 
						reproduceAnimal(animal.especie,animal.posX,animal.posY);											
					animal.estado = tipoEstadoAnimal.descansar;
				}
				else
				{
					switch(animal.estado)
					{
					case tipoEstadoAnimal.buscarAlimento:
						if(animal.aguante == 0)					
							animal.estado = tipoEstadoAnimal.descansar;	
						else
							if(buscaAlimentoAnimal(animal))
								animal.estado = tipoEstadoAnimal.comer;					
						animal.aguante--;					
						break;
					case tipoEstadoAnimal.descansar:
						animal.aguante = animal.especie.aguanteInicial;
						animal.estado = tipoEstadoAnimal.buscarAlimento;					
						break;
					case tipoEstadoAnimal.comer:
						animal.estado = tipoEstadoAnimal.buscarAlimento;
						break;
					case tipoEstadoAnimal.nacer:
						animal.estado = tipoEstadoAnimal.buscarAlimento;
						break;
					case tipoEstadoAnimal.morir:
						eliminaAnimal(animal);
						continue;
					default:break;
					}
				}
				int turno = (turnoActual + animal.especie.siguienteTurno)%numMaxTurnos;
				listadoSeresTurnos[turno].Add(animal);	
				listadoSeresTurnos[turnoActual].Remove(animal);
			
			}	
			else if(ser is Edificio)
			{
				edificio = (Edificio)ser;
				int turno = (turnoActual + edificio.tipo.siguienteTurno)%numMaxTurnos;
				listadoSeresTurnos[turno].Add(edificio);	
				listadoSeresTurnos[turnoActual].Remove(edificio);
			}
		}*/		
		
		/*for(int i = 0; i < animales.Count; i++)
		{
			animal = animales[i];
			if(!animal.consumirAlimento() && animal.estado != tipoEstadoAnimal.morir)
				animal.estado = tipoEstadoAnimal.morir;
			else if(animal.reserva > animal.especie.reservaMaxima*0.5)
			{
				if(animal.reproduccion()) 
					reproduceAnimal(animal.especie,animal.posX,animal.posY);											
				animal.estado = tipoEstadoAnimal.descansar;
			}
			else
			{
				switch(animal.estado)
				{
				case tipoEstadoAnimal.buscarAlimento:
					if(animal.aguante == 0)					
						animal.estado = tipoEstadoAnimal.descansar;	
					else
						if(buscaAlimentoAnimal(animal))
							animal.estado = tipoEstadoAnimal.comer;					
					animal.aguante--;					
					break;
				case tipoEstadoAnimal.descansar:
					animal.aguante = animal.especie.aguanteInicial;
					animal.estado = tipoEstadoAnimal.buscarAlimento;					
					break;
				case tipoEstadoAnimal.comer:
					animal.estado = tipoEstadoAnimal.buscarAlimento;
					break;
				case tipoEstadoAnimal.nacer:
					animal.estado = tipoEstadoAnimal.buscarAlimento;
					break;
				case tipoEstadoAnimal.morir:
					eliminaAnimal(animal);
					continue;
				default:break;
				}
			}
		}					
		*/
				
		contadorPintarTexturaPlantas++;
		if(texturaPlantasModificado && contadorPintarTexturaPlantas > 5)
		{
			//texturaPlantas.Apply();
			texturaPlantasModificado = false;
			contadorPintarTexturaPlantas = 0;
		}
	}
	
	public void calculaConsumoProduccion(out int energia,out int compBas,out int compAvz,out int matBio)
	{
		energia = compBas = compAvz = matBio = 0;
		for(int i = 0; i < edificios.Count; i++)
		{
			energia-=edificios[i].energiaConsumidaPorTurno;
			energia+=edificios[i].energiaProducidaPorTurno;
			compBas-=edificios[i].compBasConsumidosPorTurno;
			compBas+=edificios[i].compBasProducidosPorTurno;
			compAvz-=edificios[i].compAvzConsumidosPorTurno;
			compAvz+=edificios[i].compAvzProducidosPorTurno;
			matBio-=edificios[i].matBioConsumidoPorTurno;
			matBio+=edificios[i].matBioProducidoPorTurno;			
		}		
	}
}

[System.Serializable]
public class TipoEdificio
{
	public int idTipoEdificio;							//Identificador del tipo de edificio
	public string nombre;								//Nombre del tipo de ser
	public List<T_habitats> habitats;					//Diferentes hábitat en los que puede estar	
	public int energiaConsumidaAlCrear;
	public int compBasConsumidosAlCrear;
	public int compAvzConsumidosAlCrear;
	public int matBioConsumidoAlCrear;
	public T_elementos elemNecesarioAlConstruir;
	public int energiaConsumidaPorTurnoMax;
	public int compBasConsumidosPorTurnoMax;
	public int compAvzConsumidosPorTurnoMax;
	public int matBioConsumidoPorTurnoMax;
	public int energiaProducidaPorTurnoMax;
	public int compBasProducidosPorTurnoMax;
	public int compAvzProducidosPorTurnoMax;
	public int matBioProducidoPorTurnoMax;
	public List<GameObject> modelos;					//Distintos modelos que pueden representar al edificio		
	
	//Devuelve true si ha conseguido introducir el hábitat, false si ya ha sido introducido
	public bool aniadirHabitat(T_habitats habitat)
	{
		if(habitats.Contains(habitat))
			return false;
		habitats.Add(habitat);
		return true;
	}
	
	//Devuelve true si ha conseguido eliminar el hábitat, false si no existe
	public bool eliminarHabitat(T_habitats habitat)
	{
		return habitats.Remove(habitat);
	}
	
	public bool tieneHabitat(T_habitats habitat)
	{
		return habitats.Contains(habitat);
	}
	
	//Devuelve true si ha conseguido introducir el modelo, false si ya ha sido introducido
	public bool aniadirModelo(GameObject modelo)
	{
		if(modelos.Contains(modelo))
			return false;
		modelos.Add(modelo);
		return true;
	}
	
	public TipoEdificio(string nombre, List<T_habitats> habitats,int energiaConsumidaAlCrear,int compBasConsumidosAlCrear,int compAvzConsumidosAlCrear,int matBioConsumidoAlCrear,
	                    T_elementos elemNecesarioAlConstruir,int energiaConsumidaPorTurnoMax,int compBasConsumidosPorTurnoMax,int compAvzConsumidosPorTurnoMax,int matBioConsumidoPorTurnoMax,
	                	int energiaProducidaPorTurnoMax,int compBasProducidosPorTurnoMax,int compAvzProducidosPorTurnoMax,int matBioProducidoPorTurnoMax,GameObject modelo)
	{
		this.nombre = nombre;
		this.habitats = habitats;
		this.energiaConsumidaAlCrear = energiaConsumidaAlCrear;
		this.compBasConsumidosAlCrear = compBasConsumidosAlCrear;
		this.compAvzConsumidosAlCrear = compAvzConsumidosAlCrear;
		this.matBioConsumidoAlCrear = matBioConsumidoAlCrear;
		this.elemNecesarioAlConstruir = elemNecesarioAlConstruir;
		this.energiaConsumidaPorTurnoMax = energiaConsumidaPorTurnoMax;
		this.compBasConsumidosPorTurnoMax = compBasConsumidosPorTurnoMax;
		this.compAvzConsumidosPorTurnoMax = compAvzConsumidosPorTurnoMax;
		this.matBioConsumidoPorTurnoMax = matBioConsumidoPorTurnoMax;
		this.energiaProducidaPorTurnoMax = energiaProducidaPorTurnoMax;
		this.compBasProducidosPorTurnoMax = compBasProducidosPorTurnoMax;
		this.compAvzProducidosPorTurnoMax = compAvzProducidosPorTurnoMax;
		this.matBioProducidoPorTurnoMax = matBioProducidoPorTurnoMax;
		modelos = new List<GameObject>();
		modelos.Add(modelo);
	}
	
	public TipoEdificio(string nombre, List<T_habitats> habitats,int energiaConsumidaAlCrear,int compBasConsumidosAlCrear,int compAvzConsumidosAlCrear,int matBioConsumidoAlCrear,
	                    T_elementos elemNecesarioAlConstruir,int energiaConsumidaPorTurnoMax,int compBasConsumidosPorTurnoMax,int compAvzConsumidosPorTurnoMax,int matBioConsumidoPorTurnoMax,
	                	int energiaProducidaPorTurnoMax,int compBasProducidosPorTurnoMax,int compAvzProducidosPorTurnoMax,int matBioProducidoPorTurnoMax,List<GameObject> modelos)
	{
		this.nombre = nombre;
		this.habitats = habitats;
		this.energiaConsumidaAlCrear = energiaConsumidaAlCrear;
		this.compBasConsumidosAlCrear = compBasConsumidosAlCrear;
		this.compAvzConsumidosAlCrear = compAvzConsumidosAlCrear;
		this.matBioConsumidoAlCrear = matBioConsumidoAlCrear;
		this.elemNecesarioAlConstruir = elemNecesarioAlConstruir;
		this.energiaConsumidaPorTurnoMax = energiaConsumidaPorTurnoMax;
		this.compBasConsumidosPorTurnoMax = compBasConsumidosPorTurnoMax;
		this.compAvzConsumidosPorTurnoMax = compAvzConsumidosPorTurnoMax;
		this.matBioConsumidoPorTurnoMax = matBioConsumidoPorTurnoMax;
		this.energiaProducidaPorTurnoMax = energiaProducidaPorTurnoMax;
		this.compBasProducidosPorTurnoMax = compBasProducidosPorTurnoMax;
		this.compAvzProducidosPorTurnoMax = compAvzProducidosPorTurnoMax;
		this.matBioProducidoPorTurnoMax = matBioProducidoPorTurnoMax;
		this.modelos = modelos;		
	}
}

[System.Serializable]
public class Especie
{
	public int idEspecie;								//Identificador de la especie a la que pertenece	
	public string nombre;								//Nombre de la especie
	public int numMaxSeresEspecie;						//Número máximo de seres de esa especie
	public int numSeresEspecie;							//Número actual de seres de esa especie			
	public List<GameObject> modelos;					//Distintos modelos que pueden representar a la especie		
	
	//Devuelve true si ha conseguido introducir el modelo, false si ya ha sido introducido
	public bool aniadirModelo(GameObject modelo)
	{
		if(modelos.Contains(modelo))
			return false;
		modelos.Add(modelo);
		return true;
	}		
}

[System.Serializable]
public class EspecieVegetal : Especie
{	
	public int numMaxVegetales;							//Número de vegetales máximos por casilla
	public int numIniVegetales;							//Número inicial de vegetales en la casilla al crearse una nueva poblacion
	public float capacidadMigracionLocal;				//Probabilidad que tiene la especie de migrar a otra casilla colindante en función del número de vegetales que posea (el valor viene indicado para numMaxVegetales y en tanto por 1)
	public float capacidadMigracionGlobal;				//Probabilidad que tiene la especie de migrar a otra casilla distanciada como máximo en radioMigración casillas. Se calcula en función del número de vegetales que posea (el valor viene indicado para numMaxVegetales y en tanto por 1)	
	public int radioMigracion;							//Longitud máxima de migración de la especie
	public int turnosEvolucionInicial;					//Turnos para que la especie evolucione y tenga una habitabilidad mejor en el habitat actual
	public float evolucion;								//Valor en el que mejora la habitabilidad en el habitat actual
	public List<float> habitabilidadInicial;			//Habitabilidad inicial para cada hábitat desde -1.0(no puede habitar) hasta 1.0 (habita de forma ideal)
	public int idTextura;
		
	public EspecieVegetal(string nombre, int numMaxSeresEspecie, int numMaxVegetales, int numIniVegetales, float capacidadMigracionLocal, float capacidadMigracionGlobal, 
	                      int radioMigracion, int turnosEvolucionInicial, float evolucion, List<float> habitabilidadInicial, int idTextura, GameObject modelo)
	{
		this.nombre = nombre;
		this.numMaxSeresEspecie = numMaxSeresEspecie;		
		this.numMaxVegetales = numMaxVegetales;
		this.numIniVegetales = numIniVegetales;
		this.capacidadMigracionLocal = capacidadMigracionLocal;
		this.capacidadMigracionGlobal = capacidadMigracionGlobal;
		this.radioMigracion = radioMigracion;
		this.turnosEvolucionInicial = turnosEvolucionInicial;
		this.evolucion = evolucion;
		this.habitabilidadInicial = habitabilidadInicial;
		this.idTextura = idTextura;
		modelos = new List<GameObject>();
		modelos.Add(modelo);
		numSeresEspecie = 0;
	}
	public EspecieVegetal(string nombre, int numMaxSeresEspecie, int numMaxVegetales, int numIniVegetales, float capacidadMigracionLocal,float capacidadMigracionGlobal, 
	                      int radioMigracion, int turnosEvolucionInicial, float evolucion, List<float> habitabilidadInicial, int idTextura, List<GameObject> modelos)
	{
		this.nombre = nombre;
		this.numMaxSeresEspecie = numMaxSeresEspecie;
		this.numMaxVegetales = numMaxVegetales;
		this.numIniVegetales = numIniVegetales;
		this.capacidadMigracionLocal = capacidadMigracionLocal;
		this.capacidadMigracionGlobal = capacidadMigracionGlobal;
		this.radioMigracion = radioMigracion;
		this.turnosEvolucionInicial = turnosEvolucionInicial;
		this.evolucion = evolucion;
		this.habitabilidadInicial = habitabilidadInicial;
		this.idTextura = idTextura;
		this.modelos = modelos;
		numSeresEspecie = 0;
	}	
}

public enum tipoAlimentacionAnimal {herbivoro,carnivoro};
[System.Serializable]
public class EspecieAnimal : Especie
{
	public int consumo;									//Alimento que consume por turno
	public int reservaMaxima;							//Máximo valor para la reserva de comida, es decir, el alimento almacenado para sobrevivir
	public int alimentoMaxTurno;									//Comida máxima que pueden ingerir por turno
	public int aguanteInicial;							//Número de turnos seguidos que puede desplazarse sin agotarse
	public int reproductibilidad;						//Número de turnos que dura un ciclo completo de reproducción
	public tipoAlimentacionAnimal tipo;					//herbivoro o carnivoro 
	public List<T_habitats> habitats;					//Diferentes hábitat en los que puede estar	
	public List<Animation> animaciones;
		
	public EspecieAnimal(string nombre, int numMaxSeresEspecie, int consumo, int reservaMaxima, int alimentoMaxTurno, int aguanteInicial, int reproductibilidad, tipoAlimentacionAnimal tipo, List<T_habitats> habitats, List<GameObject> modelos, List<Animation> animaciones)
	{
		this.nombre = nombre;
		this.numMaxSeresEspecie = numMaxSeresEspecie;
		this.consumo = consumo;
		this.reservaMaxima = reservaMaxima;
		this.alimentoMaxTurno = alimentoMaxTurno;
		this.aguanteInicial = aguanteInicial;
		this.reproductibilidad = reproductibilidad;	
		this.tipo = tipo;
		this.habitats = habitats;
		this.modelos = modelos;
		this.animaciones = animaciones;
		numSeresEspecie = 0;
	}
	//Devuelve true si ha conseguido introducir el hábitat, false si ya ha sido introducido
	public bool aniadirHabitat(T_habitats habitat)
	{
		if(habitats.Contains(habitat))
			return false;
		habitats.Add(habitat);
		return true;
	}
	
	//Devuelve true si ha conseguido eliminar el hábitat, false si no existe
	public bool eliminarHabitat(T_habitats habitat)
	{
		return habitats.Remove(habitat);		
	}
	
	public bool tieneHabitat(T_habitats habitat)
	{
		return habitats.Contains(habitat);
	}	
}

[System.Serializable]
public class Ser
{
	public int idSer;								//Id del ser
	public int posX;
	public int posY;	
	public GameObject modelo;
}

[System.Serializable]
public class Vegetal : Ser 							//Representa una población de vegetales de una especie vegetal
{
	public EspecieVegetal especie;					//Especie vegetal a la que pertenece
	public int numVegetales;						//Número de vegetales de la población
	public List<float> habitabilidad;				//Habitabilidad actual para cada hábitat desde -1.0(no puede habitar) hasta 1.0 (habita de forma ideal)
	public int indiceHabitat;						//Habitat en el que se encuentra actualmente el vegetal
	public int turnosEvolucion;						//Turnos que quedan para que el vegetal evolucione mejorando la habitabilidad del habitat actual
	
	public Vegetal(int idSer, EspecieVegetal especie, int posX, int posY, T_habitats habitatActual, GameObject modelo)
	{
		this.idSer = idSer;
		this.especie = especie;
		FuncTablero.convierteCoordenadas(ref posX,ref posY);	
		this.posX = posX;
		this.posY = posY;
		this.numVegetales = especie.numIniVegetales;		
		this.turnosEvolucion = especie.turnosEvolucionInicial;
		this.habitabilidad = new List<float>();
		for(int i = 0; i < especie.habitabilidadInicial.Count;i++)
			habitabilidad.Add(especie.habitabilidadInicial[i]);
		this.indiceHabitat = (int)habitatActual;
		this.modelo = modelo;
	}
	
	public Vegetal(int idSer, EspecieVegetal especie, int posX, int posY, List<float> habitabilidad, T_habitats habitatActual, GameObject modelo)
	{
		this.idSer = idSer;
		this.especie = especie;
		FuncTablero.convierteCoordenadas(ref posX,ref posY);	
		this.posX = posX;
		this.posY = posY;
		this.numVegetales = especie.numIniVegetales;		
		this.turnosEvolucion = especie.turnosEvolucionInicial;
		this.habitabilidad = habitabilidad;
		this.indiceHabitat = (int)habitatActual;
		this.modelo = modelo;
	}
	
	public Vegetal(int idSer, EspecieVegetal especie, int posX, int posY, T_habitats habitatActual, int numVegetales,GameObject modelo)
	{
		this.idSer = idSer;
		this.especie = especie;
		this.posX = posX % FuncTablero.altoTablero;
		this.posY = posY % FuncTablero.anchoTablero;
		FuncTablero.convierteCoordenadas(ref posX,ref posY);
		this.numVegetales = numVegetales;
		this.turnosEvolucion = especie.turnosEvolucionInicial;
		this.indiceHabitat = (int)habitatActual;
		this.modelo = modelo;
	}	
	
	public void modificaHabitat(T_habitats habitat)
	{
		indiceHabitat = (int)habitat;		
	}
	
	public int consumeVegetales(int vegetalesAConsumir)			//Devuelve el número de vegetales que se han consumido
	{				
		int aux;
		if(numVegetales < vegetalesAConsumir)
		{	
			aux = numVegetales;
			numVegetales = 0;
		}
		else 
		{
			aux = vegetalesAConsumir;		
			numVegetales -= vegetalesAConsumir;
		}
		return aux;
	}
	
	//Devuelve true si la planta sigue viva y false si ha muerto
	public bool reproduccionMuerte()
	{
		if(numVegetales <= 0)			
			return false;			
		numVegetales += (int)(especie.numIniVegetales*habitabilidad[indiceHabitat]);
		if (numVegetales >= especie.numMaxVegetales)
			numVegetales = especie.numMaxVegetales;
		return numVegetales > 0;
	}
	
	//Devuelve true si se produce una migración y false si no
	public bool migracionLocal()
	{
		if(especie.numSeresEspecie >= especie.numMaxSeresEspecie)
			return false;
		int r = UnityEngine.Random.Range(0, especie.numMaxVegetales+1);
		//float migracion = especie.capacidadMigracionLocal * numVegetales * (habitabilidad[indiceHabitat]+1)/2;		//Para permitir migracion con 	-1.0f < hab <= 0.0f
		float migracion = especie.capacidadMigracionLocal * numVegetales * habitabilidad[indiceHabitat];		
		return (r < migracion);
	}	
	
	//Devuelve true si se produce una migración y false si no
	public bool migracionGlobal()
	{
		if(especie.numSeresEspecie >= especie.numMaxSeresEspecie)
			return false;
		int r = UnityEngine.Random.Range(0, especie.numMaxVegetales+1);
		float migracion = especie.capacidadMigracionGlobal * numVegetales * habitabilidad[indiceHabitat];		
		return (r < migracion);
	}
	
	//Devuelve true si se produce una evolución y false si no
	public bool evolucion()
	{
		turnosEvolucion--;
		if(turnosEvolucion <= 0)
		{
			habitabilidad[indiceHabitat] += especie.evolucion;
			if(habitabilidad[indiceHabitat] > 1.0f)
				habitabilidad[indiceHabitat] = 1.0f;
			turnosEvolucion = especie.turnosEvolucionInicial;	
			return true;
		}		
		return false;
	}
	//Devuelve si un hábitat es habitable para un vegetal
	public bool esHabitable(T_habitats habitat,float habitabilidadMinima)
	{
		return (habitabilidad[(int)habitat] > habitabilidadMinima);
	}
}

[System.Serializable]
public enum tipoEstadoAnimal {nacer,descansar,buscarAlimento,comer,morir};
public class Animal : Ser
{
	public EspecieAnimal especie;					//Especie animal a la que pertenece
	public int reserva;								//Reserva de alimento que tiene
	public int turnosParaReproduccion;				//Número de turnos que quedan para que el animal se reproduzca, al llegar a 0 se reproduce y se resetea a reproductibilidad
	public int aguante;								//Número de turnos seguidos (que le quedan) que puede desplazarse sin agotarse
	public tipoEstadoAnimal estado;					//Estado en el que se encuentra el animal
	
	public Animal(int idSer,EspecieAnimal especie,int posX,int posY,GameObject modelo)
	{
		this.idSer = idSer;
		this.especie = especie;
		this.reserva = especie.reservaMaxima/2;
		this.turnosParaReproduccion = especie.reproductibilidad;
		this.aguante = especie.aguanteInicial;
		FuncTablero.convierteCoordenadas(ref posX,ref posY);	
		this.posX = posX;
		this.posY = posY;
		this.modelo = modelo;
		estado = tipoEstadoAnimal.nacer;
	}
	
	public Animal(int idSer,EspecieAnimal especie,int posX,int posY, int res, int turnos, GameObject modelo)
	{
		this.idSer = idSer;
		this.especie = especie;
		this.reserva = res;
		this.turnosParaReproduccion = turnos;		
		this.aguante = especie.aguanteInicial;
		FuncTablero.convierteCoordenadas(ref posX,ref posY);	
		this.posX = posX;
		this.posY = posY;
		this.modelo = modelo;
		estado = tipoEstadoAnimal.nacer;
	}
	
	//Devuelve true si el animal sobrevive y false si muere
	public bool consumirAlimento()
	{		
		reserva -= especie.consumo;
		return reserva > 0;
	}
	
	public void ingiereAlimento(int comida)
	{		
		if(reserva + comida > especie.reservaMaxima)
			reserva = especie.reservaMaxima;
		else 
			reserva += comida;		
	}
	
	//Devuelve true si el animal sobrevive y false si muere
	public bool extraeReservaAnimal(int extraccion)
	{
		reserva -= extraccion;
		return reserva > 0;		
	}
	
	//Devuelve true si el animal se reproducre y false si no	
	public bool reproduccion()
	{
		if(especie.numSeresEspecie >= especie.numMaxSeresEspecie)
			return false;
		turnosParaReproduccion--;
		if(turnosParaReproduccion > 0)		
			return false;		
		turnosParaReproduccion = especie.reproductibilidad;
		return true;
	}
	
	public void morir()
	{
		reserva = 0;
	}
	
	public void desplazarse(int posXin,int posYin)
	{
		FuncTablero.convierteCoordenadas(ref posXin,ref posYin);
		this.posX = posXin;
		this.posY = posYin;
	}		
}

[System.Serializable]
public class Edificio : Ser
{
	public TipoEdificio tipo;
	public int radioAccion;
	public List<Tupla<int,int,bool>> matrizRadioAccion;
	public int energiaConsumidaPorTurno;
	public int compBasConsumidosPorTurno;
	public int compAvzConsumidosPorTurno;
	public int matBioConsumidoPorTurno;
	public int energiaProducidaPorTurno;
	public int compBasProducidosPorTurno;
	public int compAvzProducidosPorTurno;
	public int matBioProducidoPorTurno;
	public float eficiencia;	
	
	public Edificio(int idSer,TipoEdificio tipo,int posX,int posY,float eficiencia,GameObject modelo)
	{
		this.idSer = idSer;
		this.tipo = tipo;
		FuncTablero.convierteCoordenadas(ref posX,ref posY);		
		this.posX = posX;
		this.posY = posY;
		this.eficiencia = eficiencia;
		this.energiaConsumidaPorTurno =  (int)(tipo.energiaConsumidaPorTurnoMax * eficiencia);
		this.compBasConsumidosPorTurno = (int)(tipo.compBasConsumidosPorTurnoMax * eficiencia);
		this.compAvzConsumidosPorTurno = (int)(tipo.compAvzConsumidosPorTurnoMax * eficiencia);
		this.matBioConsumidoPorTurno = (int)(tipo.matBioConsumidoPorTurnoMax * eficiencia);
		this.energiaProducidaPorTurno = (int)(tipo.energiaProducidaPorTurnoMax * eficiencia);
		this.compBasProducidosPorTurno = (int)(tipo.compBasProducidosPorTurnoMax * eficiencia);
		this.compAvzProducidosPorTurno = (int)(tipo.compAvzProducidosPorTurnoMax * eficiencia);
		this.matBioProducidoPorTurno = (int)(tipo.matBioProducidoPorTurnoMax * eficiencia);
		this.modelo = modelo;
		if(eficiencia < 0.25f)
		{
			radioAccion = 0;
			matrizRadioAccion = new List<Tupla<int, int, bool>>();
		}
		else if(eficiencia < 0.5f)
		{
			radioAccion = 3;
			matrizRadioAccion = FuncTablero.calculaMatrizRadio3Circular(posX,posY);
		}
		else if(eficiencia < 0.75f)
		{
			radioAccion = 4;
			matrizRadioAccion = FuncTablero.calculaMatrizRadio4Circular(posX,posY);
		}
		else if(eficiencia < 1.0f)
		{
			radioAccion = 5;
			matrizRadioAccion = FuncTablero.calculaMatrizRadio5Circular(posX,posY);
		}
		else
		{
			radioAccion = 6;
			matrizRadioAccion = FuncTablero.calculaMatrizRadio6Circular(posX,posY);
		}		
	}
	/*public void modificaConsumo(int energiaConsumidaPorTurno,int compBasConsumidosPorTurno,int compAvzConsumidosPorTurno,int matBioConsumidoPorTurno)
	{
		this.energiaConsumidaPorTurno = energiaConsumidaPorTurno;
		this.compBasConsumidosPorTurno = compBasConsumidosPorTurno;
		this.compAvzConsumidosPorTurno = compAvzConsumidosPorTurno;
		this.matBioConsumidoPorTurno = matBioConsumidoPorTurno;
		
	}
	public void modificaProduccion(int energiaProducidaPorTurno,int compBasProducidosPorTurno,int compAvzProducidosPorTurno,int matBioProducidoPorTurno)
	{
		this.energiaProducidaPorTurno = energiaProducidaPorTurno;
		this.compBasProducidosPorTurno = compBasProducidosPorTurno;
		this.compAvzProducidosPorTurno = compAvzProducidosPorTurno;
		this.matBioProducidoPorTurno = matBioProducidoPorTurno;		
	}*/
	public void modificaEficiencia(float eficiencia)
	{
		this.eficiencia = eficiencia;
		this.energiaConsumidaPorTurno = (int)(tipo.energiaConsumidaPorTurnoMax * eficiencia);
		this.compBasConsumidosPorTurno = (int)(tipo.compBasConsumidosPorTurnoMax * eficiencia);
		this.compAvzConsumidosPorTurno = (int)(tipo.compAvzConsumidosPorTurnoMax * eficiencia);
		this.matBioConsumidoPorTurno = (int)(tipo.matBioConsumidoPorTurnoMax * eficiencia);
		this.energiaProducidaPorTurno = (int)(tipo.energiaProducidaPorTurnoMax * eficiencia);
		this.compBasProducidosPorTurno = (int)(tipo.compBasProducidosPorTurnoMax * eficiencia);
		this.compAvzProducidosPorTurno = (int)(tipo.compAvzProducidosPorTurnoMax * eficiencia);
		this.matBioProducidoPorTurno = (int)(tipo.matBioProducidoPorTurnoMax * eficiencia);		
	}
}