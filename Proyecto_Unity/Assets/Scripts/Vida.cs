using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//public enum tipoHabitat {mar, costa, rio, llanura, montana, desierto};		
//Mejor usar T_habitats, que se ha definido de forma mas general en Utilidades.cs

public class Vida{// : MonoBehaviour {	
	//Estructuras
	public Casilla[,] tablero;										//Tablero lógico que representa las casillas
	public Dictionary<string, Especie> especies;					//Listado de todas las especies
	public Dictionary<string, EspecieVegetal> especiesVegetales;	//Listado de todas las especies vegetales
	public Dictionary<string, EspecieAnimal> especiesAnimales;		//Listado de todas las especies animales
	public List<Ser> seres;										//Listado de todos los seres
	public List<Vegetal> vegetales;									//Listado de todos los vegetales
	public List<Animal> animales;									//Listado de todos los animales
	
	public int numEspecies;
	public int numEspeciesVegetales;
	public int numEspeciesAnimales;
	
	public int idActualVegetal;
	public int idActualAnimal;
			
	Vida(Casilla[,] tablero)
	{
		this.tablero = tablero;
		especies = new Dictionary<string, Especie>();
		especiesVegetales = new Dictionary<string, EspecieVegetal>();
		especiesAnimales = new Dictionary<string, EspecieAnimal>();
		seres = new List<Ser>();	
		vegetales = new List<Vegetal>();
		animales = new List<Animal>();
		numEspecies = 0;
		numEspeciesVegetales = 0;
		numEspeciesAnimales = 0;
		idActualVegetal = 0;
		idActualAnimal = 0;
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
	
	//Devuelve false si la especie ya existe (no se añade) y true si se añade correctamente
	public bool anadeEspecieVegetal(EspecieVegetal especie)
	{		
		if(especies.ContainsKey(especie.nombre))
			return false;
		especies.Add(especie.nombre,especie);
		numEspecies++;
		especiesVegetales.Add(especie.nombre,especie);
		numEspeciesVegetales++;
		return true;
	}
	
	//Devuelve false si la especie ya existe (no se añade) y true si se añade correctamente
	public bool anadeEspecieAnimal(EspecieAnimal especie)
	{		
		if(especies.ContainsKey((string)especie.nombre))
			return false;
		especies.Add((string)especie.nombre,especie);
		numEspecies++;
		especiesAnimales.Add(especie.nombre,especie);
		numEspeciesAnimales++;
		return true;
	}
	
	//Devuelve false si la especie no existe (no se elimina) y true si se elimina correctamente
	public bool eliminaEspecieVegetal(EspecieVegetal especie)
	{		
		if(!especies.ContainsKey(especie.nombre))
			return false;
		especies.Remove(especie.nombre);
		numEspecies--;
		especiesVegetales.Remove(especie.nombre);
		numEspeciesVegetales--;
		return true;
	}
	
	//Devuelve false si la especie no existe (no se elimina) y true si se elimina correctamente
	public bool eliminaEspecieAnimal(EspecieAnimal especie)
	{		
		if(!especies.ContainsKey(especie.nombre))
			return false;
		especies.Remove(especie.nombre);
		numEspecies--;
		especiesAnimales.Remove(especie.nombre);
		numEspeciesAnimales--;
		return true;
	}
	
	//Devuelve la especie identificada por nombre
	public Especie dameEspecie(string nombre)
	{
		Especie especie;
		especies.TryGetValue(nombre,out especie);
		return especie;			
	}	
	
	//Devuelve false si el vegetal ya existe (no se añade) y true si se añade correctamente	
	public bool anadeVegetal(EspecieVegetal especie,int posX,int posY)
	{
		if(tieneVegetal(posX,posY))
			return false;
		Vegetal vegetal = new Vegetal(idActualVegetal,especie,posX,posY);
		idActualVegetal++;
		seres.Add(vegetal);
		vegetales.Add(vegetal);
		tablero[posX,posY].vegetal = vegetal;
		return true;
	}	
	
	//Devuelve false si el vegetal ya existe (no se añade) y true si se añade correctamente	
	public bool anadeAnimal(EspecieAnimal especie,int posX,int posY)
	{
		if(tieneAnimal(posX,posY))
			return false;
		Animal animal = new Animal(idActualAnimal,especie,posX,posY);
		idActualAnimal++;
		seres.Add(animal);
		animales.Add(animal);		
		tablero[posX,posY].animal = animal;
		return true;
	}
	
	//Devuelve false si la especie no existe (no se elimina) y true si se elimina correctamente
	public bool eliminaVegetal(Vegetal vegetal)
	{
		if(!vegetales.Contains(vegetal))
			return false;
		vegetales.Remove(vegetal);
		return true;
	}
	
	//Devuelve false si la especie no existe (no se elimina) y true si se elimina correctamente
	public bool eliminaAnimal(Animal animal)
	{
		if(!animales.Contains(animal))
			return false;
		animales.Remove(animal);
		return true;
	}
	
	//Devuelve true si consigue migrar una especie a una nueva posicion y false si no
	public bool migraVegetal(EspecieVegetal especie,int posX,int posY)
	{
		Random.seed = System.DateTime.Now.Millisecond;
		int nposX = posX + Random.Range(-especie.radioMigracion,especie.radioMigracion);
		int nposY = posY + Random.Range(-especie.radioMigracion,especie.radioMigracion);		
		if(anadeVegetal(especie,nposX,nposY))
			return true;
		return false;
	}
	
	public void algoritmoVida()
	{
		randomLista();
		Ser ser;
		Vegetal vegetal;
		Animal animal;
		for(int i = 0; i < seres.Count; i++)
		{
			ser = seres[i];
			if(ser is Vegetal)
			{
				vegetal = (Vegetal)ser;
				vegetal.reproduccion();
				if(vegetal.migracion())
					migraVegetal(vegetal.especie,vegetal.posX,vegetal.posY);
			}
			else if(ser is Animal)
			{
				animal = (Animal)ser;	
				animal.alimentacion();
				if(animal.reproduccion())
					;//alimentaAnimal();
				//buscaAlimento();
			}			
		}
	}
	
	public void randomLista()
	{
		System.Random random = new System.Random();
		int pos;
		
		Ser ser;
		for(int i = seres.Count; i > 1; i--)
		{
			pos = random.Next(i);
			ser = seres[i-1];
			seres[i-1] = seres[pos];
			seres[pos] = ser;
		}
		/*
		Vegetal vegetal;
		Animal animal;
		for(int i = vegetales.Count; i > 1; i--)
		{
			pos = random.Next(i);
			vegetal = vegetales[i-1];
			vegetales[i-1] = vegetales[pos];
			vegetales[pos] = ser;
		}
		for(int i = seres.Count; i > 1; i--)
		{
			pos = random.Next(i);
			ser = seres[i-1];
			seres[i-1] = seres[pos];
			seres[pos] = ser;
		}*/
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class Especie
{
	public int idEspecie;								//Identificador de la especie a la que pertenece	
	public string nombre;								//Nombre de la especie
	public T_habitats habitat;							//Diferentes hábitat en los que puede estar la especie
}

public class EspecieVegetal : Especie
{	
	public int numMaxVegetales;							//Número de vegetales máximos por casilla
	public int numIniVegetales;							//Número inicial de vegetales en la casilla al crearse una nueva poblacion
	public float capacidadReproductiva;					//% de individuos que se incrementan por turno en función de los vegetales actuales	(en tanto por 1)
	public float capacidadMigracion;					//Probabilidad que tiene la especie de migrar a otra casilla en función del número de vegetales que posea (el valor viene indicado para numMaxVegetales y en tanto por 1)
	public int radioMigracion;							//Longitud máxima de migración de la especie
	//public int capacidadEvolucion;					//Probabilidad de que la especie evolucione dentro de la casilla
	//public int capacidadEvolucionMigracion;			//Probabilidad de que la especie evolucione al migrar (teoricamente mucho más alto que la normal)
	
	public EspecieVegetal(int idEspecie, string nombre, int numMaxVegetales, int numIniVegetales, int capacidadReproductiva, int capacidadMigracion, int radioMigracion, T_habitats habitat)
	{
		this.idEspecie = idEspecie;
		this.nombre = nombre;
		this.numMaxVegetales = numMaxVegetales;
		this.numIniVegetales = numIniVegetales;
		this.capacidadReproductiva = capacidadReproductiva;
		this.capacidadMigracion = capacidadMigracion;
		this.radioMigracion = radioMigracion;
		this.habitat = habitat;
	}	
}

public class EspecieAnimal : Especie
{
	public int consumo;									//Alimento que consume por turno
	public int reservaMaxima;							//Máximo valor para la reserva de comida, es decir, el alimento almacenado para sobrevivir
	public int alimentoQueProporciona;					//Alimento que recibe un animal al comerse a uno de esta especie
	//public int vision;								//Rango de visión del animal para controlar su IA
	public int velocidad;								//Número de casillas que puede desplazarse por turno
	public int reproductibilidad;						//Número de turnos que dura un ciclo completo de reproducción
	public enum tipoAnimal {herbivoro,carnivoro};
	public tipoAnimal tipo;								//herbivoro o carnivoro 
		
	public EspecieAnimal(int idEspecie, string nombre, int consumo, int reservaMaxima, int alimentoQueProporciona, int velocidad, int reproductibilidad, tipoAnimal tipo)//, T_habitats habitat)
	{
		this.idEspecie = idEspecie;
		this.nombre = nombre;
		this.consumo = consumo;
		this.reservaMaxima = reservaMaxima;
		this.alimentoQueProporciona = alimentoQueProporciona;
		this.velocidad = velocidad;
		this.reproductibilidad = reproductibilidad;	
		this.tipo = tipo;
		this.habitat = habitat;
	}		
}

public class Ser
{
	public int idSer;								//Id del ser
	//public Especie especie;							//Especie a la que pertenece
	public int posX;
	public int posY;
}

public class Vegetal : Ser 							//Representa una población de vegetales de una especie vegetal
{
	public EspecieVegetal especie;				//Especie vegetal a la que pertenece
	public int numVegetales;						//Número de vegetales de la población
	//public int posX;								//Celda en la que se encuentra el vegetal, coordenada X
	//public int posY;								//Celda en la que se encuentra el vegetal, coordenada Y
	
	public Vegetal(int idSer, EspecieVegetal especie, int posX, int posY)
	{
		this.idSer = idSer;
		this.especie = especie;
		this.posX = posX;
		this.posY = posY;
		this.numVegetales = especie.numIniVegetales;
	}	
	
	public Vegetal(int idSer, EspecieVegetal especie, int posX, int posY, int numVegetales)
	{
		this.idSer = idSer;
		this.especie = especie;
		this.posX = posX;
		this.posY = posY;
		this.numVegetales = numVegetales;
	}	
	
	public int consumeVegetales(int vegetalesAConsumir)			//Devuelve el número de vegetales que se han consumido
	{				
		int aux;
		if(numVegetales < vegetalesAConsumir)
			aux = numVegetales;
		else 
			aux = vegetalesAConsumir;		
		this.numVegetales -= vegetalesAConsumir;
		return aux;
	}
	
	public void reproduccion()
	{
		numVegetales *= (int)especie.capacidadReproductiva;			
	}
	
	//Devuelve true si se produce una migración y false si no
	public bool migracion()
	{
		Random.seed = System.DateTime.Now.Millisecond;
		int r = Random.Range(0, numVegetales);
		return (r < especie.capacidadMigracion * numVegetales);
	}	
}

public class Animal : Ser
{
	public EspecieAnimal especie;					//Especie animal a la que pertenece
	public int reserva;								//Reserva de alimento que tiene
	public int turnosParaReproduccion;				//Número de turnos que quedan para que el animal se reproduzca, al llegar a 0 se reproduce y se resetea a reproductibilidad
	//public int posX;								//Celda en la que se encuentra el animal, coordenada X
	//public int posY;								//Celda en la que se encuentra el animal, coordenada Y
	
	public Animal(int idSer,EspecieAnimal especie,int posX,int posY)
	{
		this.idSer = idSer;
		this.especie = especie;
		this.reserva = especie.reservaMaxima/2;
		this.turnosParaReproduccion = especie.reproductibilidad;
		this.posX = posX;
		this.posY = posY;
	}
	
	//Devuelve true si el animal sobrevive y false si muere
	public bool alimentacion()
	{		
		reserva -= especie.consumo;
		return reserva > 0;
		
		/*
		int idComida;
		if(TABLERO.buscaAlimento(especie.idEspecie,especie.tipo,idComida,posX,posY))
			reserva += TABLERO.consumeAlimento(idComida,ref posX,ref posY);		//Consume el alimento y desplaza la posicion del animal a la de su alimento
		else
			movimientoAleatorio();
			*/
	}
	
	//Devuelve true si el animal se reproducre y false si no	
	public bool reproduccion()
	{
		turnosParaReproduccion--;
		if(turnosParaReproduccion > 0)		
			return false;		
		turnosParaReproduccion = especie.reproductibilidad;
		return true;
	}
	
	public void desplazarse(int posX,int posY)
	{
		this.posX = posX;
		this.posY = posY;		
	}		
}