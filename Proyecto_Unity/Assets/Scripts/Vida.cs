using UnityEngine;
using System.Collections;

public enum tipoHabitat {mar,costa,rio,llanura,montaña,desierto};

public class Vida : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class Vegetal								//Representa una población de vegetales de una especie vegetal
{
	public int idVegetal;							//id de la población
	public EspecieVegetal especie;					//Especie vegetal a la que pertenece
	public int numVegetales;						//Número de vegetales de la población
	public int posX;								//Celda en la que se encuentra el vegetal, coordenada X
	public int posY;								//Celda en la que se encuentra el vegetal, coordenada Y
	public Vegetal(int idIndividuo,EspecieVegetal especie,int posX,int posY)
	{
		this.idIndividuo = idIndividuo;
		this.especie = especie;
		this.posX = posX;
		this.posY = posY;
		numVegetales = especie.numIniVegetales;
	}	
	public Vegetal(int idIndividuo,EspecieVegetal especie,int posX,int posY,int numVegetales)
	{
		this.idIndividuo = idIndividuo;
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
		numVegetales -= vegetalesAConsumir;
		return aux;
	}
	public void reproduccion()
	{
		numVegetales *= especie.capacidadReproductiva;		
	}
	public void migracion()
	{
		TABLERO.migraVegetal(especie,posX,posY);		
	}
	public void algoritmoVida()
	{		
		Random.seed = System.DateTime.Now.Millisecond;
		reproduccion();
		int r = Random.Range(0, numVegetales);
		if(r < especie.capacidadMigracion * numVegetales)	//Se produce una migración
			migracion();
	}	
}

public class EspecieVegetal
{	
	public int idEspecie;							//Identificador de la especie a la que pertenece	
	public string nombre;							//Nombre de la especie
	public int numMaxVegetales;						//Número de vegetales máximos por casilla
	public int numIniVegetales;						//Número inicial de vegetales en la casilla al crearse una nueva poblacion
	public float capacidadReproductiva;				//% de individuos que se incrementan por turno en función de los vegetales actuales	(en tanto por 1)
	public float capacidadMigracion;				//Probabilidad que tiene la especie de migrar a otra casilla en función del número de vegetales que posea (el valor viene indicado para numMaxVegetales y en tanto por 1)
	public int radioMigracion;						//Longitud máxima de migración de la especie
	public tipoHabitat habitat;
	//public int capacidadEvolucion;					//Probabilidad de que la especie evolucione dentro de la casilla
	//public int capacidadEvolucionMigracion;			//Probabilidad de que la especie evolucione al migrar (teoricamente mucho más alto que la normal)
	
	public EspecieVegetal(int idEspecie,string nombre,int numMaxVegetales,int numIniVegetales,int capacidadReproductiva,int capacidadMigracion,int radioMigracion,tipoHabitat habitat)
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

public class Animal
{
	public int idAnimal;							//id del animal
	public EspecieAnimal especie;					//Especie animal a la que pertenece
	public int reserva;								//Reserva de alimento que tiene
	public int turnosParaReproduccion;				//Número de turnos que quedan para que el animal se reproduzca, al llegar a 0 se reproduce y se resetea a reproductibilidad
	public int posX;								//Celda en la que se encuentra el animal, coordenada X
	public int posY;								//Celda en la que se encuentra el animal, coordenada Y
	
	public Animal(int idAnimal,EspecieAnimal especie,int posX,int posY)
	{
		this.idAnimal = idAnimal;
		this.especie = especie;
		reserva = especie.reservaMaxima;
		turnosParaReproduccion = especie.reproductibilidad/2;
		this.posX = posX;
		this.posY = posY;
	}
	public void alimentacion()
	{		
		int idComida;
		if(TABLERO.buscaAlimento(especie.idEspecie,especie.tipo,idComida,posX,posY))
			reserva += TABLERO.consumeAlimento(idComida,ref posX,ref posY);		//Consume el alimento y desplaza la posicion del animal a la de su alimento
		else
			movimientoAleatorio();
	}
	
	public void reproduccion()
	{
		TABLERO.reproduceAnimal(especie,posX,posY);
		turnosParaReproduccion = especie.reproductibilidad;
	}	
	public void movimientoAleatorio()
	{
		Random.seed = System.DateTime.Now.Millisecond;
		int x = Random.Range(-1, 1);		
		int y = Random.Range(-1, 1);
		/*	Sacamos la nueva posición en función de la velocidad (numero de posiciones que se puede mover por turno) y la direccion haciendo un random
		 * entre -1, 0 y 1 de x y de y. La dirección viene dada según lo siguiente:
		 * arriba 			=> x = 0, y = -1
		 * arriba derecha 	=> x = 1, y = -1
		 * derecha			=> x = 1, y = 0
		 * abajo derecha	=> x = 1, y = 1
		 * abajo			=> x = 0, y = 1
		 * abajo izquierda	=> x = -1,y = 1
		 * izquierda		=> x = -1,y = 0
		 * arriba izquierda	=> x = -1,y = -1
		 */
		int nposX = posX + especie.velocidad * x;
		int nposY = posY + especie.velocidad * y;		
		
		TABLERO.movimiento(especie.habitat,nposX,nposY,ref posX,ref posY);	//Mueve al animal a una posición en la que pueda estar, desde donde esta y hasta donde puede ir				
	}
	public bool algoritmoVida()						//Devuelve true si el animal sobrevive y false si muere
	{
		reserva -= especie.consumo;
		if(reserva < 1)
			return false;
		if(turnosParaReproduccion == 0)
		{
			reproduccion();							//Si se reproduce no hace nada más en ese turno
			return true;
		}
		else
			turnosParaReproduccion -= 1;		
		alimentacion();
		return true;
	}	
}

public class EspecieAnimal
{
	public int idEspecie;							//Identificador de la especie a la que pertenece	
	public string nombre;							//Nombre de la especie
	public int consumo;								//Alimento que consume por turno
	public int reservaMaxima;						//Máximo valor para la reserva de comida, es decir, el alimento almacenado para sobrevivir
	public int alimentoQueProporciona;				//Alimento que recibe un animal al comerse a uno de esta especie
	//public int vision;								//Rango de visión del animal para controlar su IA
	public int velocidad;							//Número de casillas que puede desplazarse por turno
	public int reproductibilidad;					//Número de turnos que dura un ciclo completo de reproducción
	public enum tipoAnimal{herbivoro,carnivoro};
	public tipoAnimal tipo;							//herbivoro o carnivoro 
	public tipoHabitat habitat;
		
	public EspecieAnimal(int idEspecie,string nombre,int consumo,int reservaMaxima,int alimentoQueProporciona,int velocidad,int reproductibilidad,tipoAnimal tipo,tipoHabitat habitat)
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