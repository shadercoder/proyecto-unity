using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//public enum tipoHabitat {mar, costa, rio, llanura, montana, desierto};		
//Mejor usar T_habitats, que se ha definido de forma mas general en Utilidades.cs

public class Vida
{
	//Estructuras
	public Casilla[,] tablero;										//Tablero lógico que representa las casillas
	public Dictionary<string, Especie> especies;					//Listado de todas las especies
	public Dictionary<string, EspecieVegetal> especiesVegetales;	//Listado de todas las especies vegetales
	public Dictionary<string, EspecieAnimal> especiesAnimales;		//Listado de todas las especies animales
	public List<Ser> seres;											//Listado de todos los seres
	public List<Vegetal> vegetales;									//Listado de todos los vegetales
	public List<Animal> animales;									//Listado de todos los animales
	
	public int numEspecies;
	public int numEspeciesVegetales;
	public int numEspeciesAnimales;
	
	public int idActualVegetal;
	public int idActualAnimal;
	
	public Vida()
	{
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
	
	public Vida(Casilla[,] tablero)
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
	
	public Vida(Vida vida)
	{
		tablero = vida.tablero;
		especies = vida.especies;
		especiesVegetales = vida.especiesVegetales;
		especiesAnimales = vida.especiesAnimales;
		seres = vida.seres;
		vegetales = vida.vegetales;
		animales = vida.animales;	
		numEspecies = vida.numEspecies;
		numEspeciesVegetales = vida.numEspeciesVegetales;
		numEspeciesAnimales = vida.numEspeciesAnimales;
		idActualVegetal = vida.idActualVegetal;
		idActualAnimal = vida.idActualAnimal;
	}
	
	//Devuelve true si hay un vegetal en la casilla [x,y] y false si no lo hay
	public bool tieneVegetal(int x,int y)
	{		
		try{
		return tablero[x,y].vegetal != null;
		}
		catch(System.Exception)
		{
			Debug.Log("x: "+x+"  y: "+y);			
		}
		return true;		
	}
	
	//Devuelve true si hay un animal en la casilla [x,y] y false si no lo hay
	public bool tieneAnimal(int x,int y)
	{
		try{
		return tablero[x,y].animal != null;
		}
		catch(System.Exception)
		{
			Debug.Log("x: "+x+"  y: "+y);			
		}
		return true;
	}
	
	//Devuelve false si la especie ya existe (no se añade) y true si se añade correctamente
	public bool anadeEspecieVegetal(EspecieVegetal especie)
	{		
		if(especies.ContainsKey(especie.nombre))
			return false;
		especie.idEspecie = numEspeciesVegetales;
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
		especie.idEspecie = numEspeciesAnimales;
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
		if(tieneVegetal(posX,posY) || especie.habitat != tablero[posX,posY].habitat)
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
		if(tieneAnimal(posX,posY) || especie.habitat != tablero[posX,posY].habitat)
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
		FuncTablero.convierteCoordenadas(ref nposX,ref nposY);		
		if(anadeVegetal(especie,nposX,nposY))
			return true;
		return false;
	}
	
	//Devuelve true si consigue desplazar al animal y false si no lo consigue
	public bool desplazaAnimal(Animal animal,int nposX,int nposY)
	{		
		Random.seed = System.DateTime.Now.Millisecond;
		FuncTablero.convierteCoordenadas(ref nposX,ref nposY);
		while(animal.posX != nposX || animal.posY != nposY)		
		{			
			if(!tieneAnimal(nposX,nposY) && animal.especie.habitat == tablero[nposX,nposY].habitat)
			{
				animal.desplazarse(nposX,nposY);
				tablero[animal.posX,animal.posY].animal = null;
				tablero[nposX,nposY].animal = animal;
				return true;
			}	
			if(nposX > animal.posX) nposX--;
			else if(nposX < animal.posX) nposX++;
			if(nposY > animal.posY) nposY--;
			else if(nposY < animal.posY) nposY++;
			FuncTablero.convierteCoordenadas(ref nposX,ref nposY);				
		}
		return false;
	}
	
	//Devuelve true si consigue crear un nuevo animal colindante a la posición de entrada y false si no lo consigue
	public bool reproduceAnimal(EspecieAnimal especie,int posX,int posY)
	{
		List<int> aux = new List<int>();
		aux.Add(0);
		aux.Add(1);
		aux.Add(2);
		aux.Add(3);
		aux.Add(4);
		aux.Add(5);
		aux.Add(6);
		aux.Add(7);
		
		System.Random random = new System.Random();
		int pos;		
		int n;
		for(int i = aux.Count; i > 1; i--)
		{
			pos = random.Next(i);
			n = aux[i-1];
			aux[i-1] = aux[pos];
			aux[pos] = n;
		}	
		
		int x;
		int y;
		for(int i = 0; i < 8; i++)
		{
			switch(aux[i])
			{
				case 0: x = 0;y = -1;break;		//arriba
				case 1: x = 1;y = -1;break;		//arriba derecha				
				case 2: x = 1;y = 0;break;		//derecha
				case 3: x = 1;y = 1;break;		//abajo derecha
				case 4: x = 0;y = 1;break;		//abajo
				case 5: x = -1;y = 1;break;		//abajo izquierda
				case 6: x = -1;y = 0;break;		//izquierda
				case 7: x = -1;y = -1;break;	//arriba izquierda
			default: x = 0; y = 0; break;
			}
			x += posX;
			y += posY;
			FuncTablero.convierteCoordenadas(ref x,ref y);
			if(!tieneAnimal(x,y) && especie.habitat == tablero[x,y].habitat)
			{				
				anadeAnimal(especie,x,y);
				return true;
			}			
		}
		return false;
	}
	
	//Devuelve true si ha comido y false si no
	public bool buscaAlimentoAnimal(Animal animal)
	{
		int vision = animal.especie.vision;
		int velocidad = animal.especie.velocidad;
		if(animal.especie.tipo == tipoAnimal.carnivoro)
		{
			for(int i = 0; i < animales.Count; i++)
			{
				/*
				//Si el animal está en su radio de visión
				if(animales[i].posX >= animal.posX - vision && animales[i].posX <= animal.posX + vision && 
				   animales[i].posY >= animal.posY - vision && animales[i].posY <= animal.posY + vision)
				{
					//Si el animal está en su radio de acción lo consume
					if(animales[i].posX >= animal.posX - velocidad && animales[i].posX <= animal.posX + velocidad && 
				   	   animales[i].posY >= animal.posY - velocidad && animales[i].posY <= animal.posY + velocidad &&
				   	   animales[i].especie.idEspecie != animal.especie.idEspecie)
					{
						animal.ingiereAlimento(animales[i].especie.alimentoQueProporciona);
						int x = animales[i].posX;
						int y = animales[i].posY;						
						eliminaAnimal(animales[i]);
						desplazaAnimal(animal,x,y);
						return true;
					}
					//Sino se acerca a el
					else
					{
						int x;
						int y;
						//if(
					}
					
				}*/
				//Si el animal está en su radio de acción lo consume
				if(animales[i].posX >= animal.posX - velocidad && animales[i].posX <= animal.posX + velocidad && 
			   	   animales[i].posY >= animal.posY - velocidad && animales[i].posY <= animal.posY + velocidad &&
				   animales[i].especie.idEspecie != animal.especie.idEspecie)
				{
					animal.ingiereAlimento(animales[i].especie.alimentoQueProporciona);
					int x = animales[i].posX;
					int y = animales[i].posY;						
					eliminaAnimal(animales[i]);
					desplazaAnimal(animal,x,y);
					return true;
				}
				//Sino movimiento random
				else
				{
					movimientoAleatorio(animal);
					return false;
				}
			}			
		}
		else if(animal.especie.tipo == tipoAnimal.herbivoro)
		{
			for(int i = 0; i < vegetales.Count; i++)
			{				
				//Si el animal está en su radio de acción lo consume
				if(vegetales[i].posX >= animal.posX - velocidad && vegetales[i].posX <= animal.posX + velocidad && 
			   	   vegetales[i].posY >= animal.posY - velocidad && vegetales[i].posY <= animal.posY + velocidad)
				{					
					int vegetalesComidos = vegetales[i].consumeVegetales(animal.especie.reservaMaxima - animal.reserva);
					int x = vegetales[i].posX;
					int y = vegetales[i].posY;											
					if(vegetales[i].numVegetales == 0)
						eliminaVegetal(vegetales[i]);	
					animal.ingiereAlimento(vegetalesComidos);
					desplazaAnimal(animal,x,y);
					return true;
				}
				//Sino movimiento random
				else
				{
					movimientoAleatorio(animal);
					return false;
				}
			}
		}
		return false;
	}
	
	public bool movimientoAleatorio(Animal animal)
	{
		int x = Random.Range(-1, 1);            
	    int y = Random.Range(-1, 1);
	    /*      Sacamos la nueva posición en función de la velocidad (numero de posiciones que se puede mover por turno) y la direccion haciendo un random
	     * entre -1, 0 y 1 de x y de y. La dirección viene dada según lo siguiente:
	     * arriba                       => x = 0, y = -1
	     * arriba derecha       => x = 1, y = -1
	     * derecha                      => x = 1, y = 0
	     * abajo derecha        => x = 1, y = 1
	     * abajo                        => x = 0, y = 1
	     * abajo izquierda      => x = -1,y = 1
	     * izquierda            => x = -1,y = 0
	     * arriba izquierda     => x = -1,y = -1
	     */
	    int nposX = animal.posX + animal.especie.velocidad * x;
	    int nposY = animal.posY + animal.especie.velocidad * y;       
		FuncTablero.convierteCoordenadas(ref nposX,ref nposY);
		desplazaAnimal(animal,nposX,nposY);		
		return true;
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
				if(!animal.consumirAlimento())
				{
					eliminaAnimal(animal);
					continue;
				}
				if(animal.reproduccion())
					reproduceAnimal(animal.especie,animal.posX,animal.posY);
				if(animal.reserva < animal.especie.reservaMaxima * 0.75)		//Si está por debajo del 75% de la reserva de comida
					buscaAlimentoAnimal(animal);
				else 															//Movimiento aleatorio				
					movimientoAleatorio(animal);		
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
	}
	
	public bool buscaPosicionVaciaVegetal(T_habitats habitat,ref int x,ref int y)
	{
		System.Random random = new System.Random();
		int pos;
		int aux;
		List<int> listaX = new List<int>();
		for(int i = 0; i < FuncTablero.altoTableroUtil; i++)
			listaX.Add(i);
		for(int i = FuncTablero.altoTableroUtil; i > 1; i--)
		{
			pos = random.Next(i);
			aux = listaX[i-1];
			listaX[i-1] = listaX[pos];
			listaX[pos] = aux;
		}
		
		List<int> listaY = new List<int>();
		for(int i = 0; i < FuncTablero.anchoTablero; i++)
			listaY.Add(i);
		for(int i = FuncTablero.anchoTablero; i > 1; i--)
		{
			pos = random.Next(i);
			aux = listaY[i-1];
			listaY[i-1] = listaY[pos];
			listaY[pos] = aux;
		}
		for(int i = 0; i < FuncTablero.altoTableroUtil;i++)
			for(int j = 0; j < FuncTablero.anchoTablero; j++)
			{
				x = listaX[i];
				y = listaY[j];
				if(!tieneVegetal(x,y) && tablero[x,y].habitat == habitat)			   	
					return true;				
			}
		return false;
	}
			
	public bool buscaPosicionVaciaAnimal(T_habitats habitat,ref int x,ref int y)
	{
		System.Random random = new System.Random();
		int pos;
		int aux;
		List<int> listaX = new List<int>();
		for(int i = 0; i < FuncTablero.altoTableroUtil; i++)
			listaX.Add(i);
		for(int i = FuncTablero.altoTableroUtil; i > 1; i--)
		{
			pos = random.Next(i);
			aux = listaX[i-1];
			listaX[i-1] = listaX[pos];
			listaX[pos] = aux;
		}
		
		List<int> listaY = new List<int>();
		for(int i = 0; i < FuncTablero.anchoTablero; i++)
			listaY.Add(i);
		for(int i = FuncTablero.anchoTablero; i > 1; i--)
		{
			pos = random.Next(i);
			aux = listaY[i-1];
			listaY[i-1] = listaY[pos];
			listaY[pos] = aux;
		}
		
		for(int i = 0; i < FuncTablero.altoTableroUtil;i++)
			for(int j = 0; j < FuncTablero.anchoTablero; j++)
			{
				x = listaX[i];
				y = listaY[j];
				if(!tieneAnimal(x,y) && tablero[x,y].habitat == habitat)			   	
					return true;				
			}
		return false;
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
	public int idTextura;
	//public int capacidadEvolucion;					//Probabilidad de que la especie evolucione dentro de la casilla
	//public int capacidadEvolucionMigracion;			//Probabilidad de que la especie evolucione al migrar (teoricamente mucho más alto que la normal)
	
	public EspecieVegetal(string nombre, int numMaxVegetales, int numIniVegetales, int capacidadReproductiva, int capacidadMigracion, int radioMigracion, T_habitats habitat, int idTextura)
	{
		this.nombre = nombre;
		this.numMaxVegetales = numMaxVegetales;
		this.numIniVegetales = numIniVegetales;
		this.capacidadReproductiva = capacidadReproductiva;
		this.capacidadMigracion = capacidadMigracion;
		this.radioMigracion = radioMigracion;
		this.habitat = habitat;
		this.idTextura = idTextura;
	}	
}

public enum tipoAnimal {herbivoro,carnivoro};	
public class EspecieAnimal : Especie
{
	public int consumo;									//Alimento que consume por turno
	public int reservaMaxima;							//Máximo valor para la reserva de comida, es decir, el alimento almacenado para sobrevivir
	public int alimentoQueProporciona;					//Alimento que recibe un animal al comerse a uno de esta especie
	public int vision;								//Rango de visión del animal para controlar su IA
	public int velocidad;								//Número de casillas que puede desplazarse por turno
	public int reproductibilidad;						//Número de turnos que dura un ciclo completo de reproducción
	public tipoAnimal tipo;								//herbivoro o carnivoro 
		
	public EspecieAnimal(string nombre, int consumo, int reservaMaxima, int alimentoQueProporciona, int vision, int velocidad, int reproductibilidad, tipoAnimal tipo, T_habitats habitat)
	{
		this.nombre = nombre;
		this.consumo = consumo;
		this.reservaMaxima = reservaMaxima;
		this.alimentoQueProporciona = alimentoQueProporciona;
		this.vision = vision;
		this.velocidad = velocidad;
		this.reproductibilidad = reproductibilidad;	
		this.tipo = tipo;
		this.habitat = habitat;
	}		
}

public class Ser
{
	public int idSer;								//Id del ser
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
		this.posX = posX % FuncTablero.altoTableroUtil;
		this.posY = posY % FuncTablero.anchoTablero;
		FuncTablero.convierteCoordenadas(ref posX,ref posY);
		this.numVegetales = especie.numIniVegetales;
	}	
	
	public Vegetal(int idSer, EspecieVegetal especie, int posX, int posY, int numVegetales)
	{
		this.idSer = idSer;
		this.especie = especie;
		this.posX = posX % FuncTablero.altoTableroUtil;
		this.posY = posY % FuncTablero.anchoTablero;
		FuncTablero.convierteCoordenadas(ref posX,ref posY);
		this.numVegetales = numVegetales;
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
	
	public void reproduccion()
	{
		numVegetales = (int)(numVegetales * (1 + especie.capacidadReproductiva/100.0f));
	}
	
	//Devuelve true si se produce una migración y false si no
	public bool migracion()
	{
		Random.seed = System.DateTime.Now.Millisecond;
		int r = Random.Range(0, numVegetales);
		return (r < (especie.capacidadMigracion/100.0f) * numVegetales);
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
		FuncTablero.convierteCoordenadas(ref posX,ref posY);		
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
		FuncTablero.convierteCoordenadas(ref posX,ref posY);		
	}		
}