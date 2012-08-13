using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TiposSeres : MonoBehaviour {
	
	//Tipos de edificios
	private TipoEdificio fabricaComBas;
	private TipoEdificio energia;
	private TipoEdificio granja;
	private TipoEdificio fabricaComAdv;
	private TipoEdificio energiaAdv;
	
	//Tipos de animales
	
	//Herbivoros
	private EspecieAnimal herbivoro1;
	private EspecieAnimal herbivoro2;
	private EspecieAnimal herbivoro3;
	private EspecieAnimal herbivoro4;
	private EspecieAnimal herbivoro5;
	//Carnivoros
	private EspecieAnimal carnivoro1;
	private EspecieAnimal carnivoro2;
	private EspecieAnimal carnivoro3;
	private EspecieAnimal carnivoro4;
	private EspecieAnimal carnivoro5;
	
	//Tipos de plantas
	private EspecieVegetal seta;
	private EspecieVegetal flor;
	private EspecieVegetal palo;
	private EspecieVegetal arbusto;
	private EspecieVegetal estrom;
	private EspecieVegetal cactus;
	private EspecieVegetal palmera;
	private EspecieVegetal pino;
	private EspecieVegetal cipres;
	private EspecieVegetal pinoAlto;
	
	//Variables del script
	private ModelosEdificios modelosEdificios;
	private ModelosVegetales modelosVegetales;
	private ModelosAnimales modelosAnimales;
	private Principal principal;
	
	void Awake() {
		modelosEdificios = GameObject.FindGameObjectWithTag("ModelosEdificios").GetComponent<ModelosEdificios>();		
		modelosVegetales = GameObject.FindGameObjectWithTag("ModelosVegetales").GetComponent<ModelosVegetales>();		
		modelosAnimales = GameObject.FindGameObjectWithTag("ModelosAnimales").GetComponent<ModelosAnimales>();
		principal = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Principal>();
		
		List<T_habitats> habsEdificios = new List<T_habitats>();
		habsEdificios.Add(T_habitats.llanura);
		habsEdificios.Add(T_habitats.colina);
		
		List<T_habitats> habsEdificiosAdv = new List<T_habitats>();
		habsEdificiosAdv.Add(T_habitats.costa);
		habsEdificiosAdv.Add(T_habitats.llanura);
		habsEdificiosAdv.Add(T_habitats.colina);
		habsEdificiosAdv.Add(T_habitats.tundra);
		habsEdificiosAdv.Add(T_habitats.desierto);
		
		/*fabricaComBas = new TipoEdificio("Fábrica de componentes básicos",habsEdificios,100,25,0,0,T_elementos.comunes,modelosEdificios.fabCompBas);
		energia = new TipoEdificio("Central de energía",habsEdificios,150,15,0,0,T_elementos.comunes,modelosEdificios.centralEnergia);
		granja = new TipoEdificio("Granja",habsEdificios,700,200,50,0,T_elementos.nada,modelosEdificios.granja);
		fabricaComAdv = new TipoEdificio("Fábrica de componentes avanzados",habsEdificiosAdv,850,300,0,0,T_elementos.raros,modelosEdificios.fabCompAdv);
		energiaAdv = new TipoEdificio("Central de energía avanzada",habsEdificiosAdv,1000,500,250,0,T_elementos.raros,modelosEdificios.centralEnergiaAdv);
		*/
		fabricaComBas = new TipoEdificio("Fábrica de componentes básicos",10,habsEdificios,0,0,0,0,T_elementos.comunes,modelosEdificios.fabCompBas);
		energia = new TipoEdificio("Central de energía",10,habsEdificios,0,0,0,0,T_elementos.comunes,modelosEdificios.centralEnergia);
		granja = new TipoEdificio("Granja",10,habsEdificios,0,0,0,0,T_elementos.nada,modelosEdificios.granja);
		fabricaComAdv = new TipoEdificio("Fábrica de componentes avanzados",10,habsEdificiosAdv,0,0,0,0,T_elementos.raros,modelosEdificios.fabCompAdv);
		energiaAdv = new TipoEdificio("Central de energía avanzada",10,habsEdificiosAdv,0,0,0,0,T_elementos.raros,modelosEdificios.centralEnergiaAdv);
		
		/* Vegetales */
		/*vegetal = new EspecieVegetal(nombre, siguienteTurno, numMaxVegetales, numIniVegetales, capacidadMigracionLocal, capacidadMigracionGlobal, radioMigracion, 
									   turnosEvolucionInicial, evolucion, habitabilidadInicial, idTextura, modelos
		nombre						=>	nombre de la especie
		siguienteTurno				=>	cada cuantos turnos ejecuta la especie su algoritmo
		numMaxVegetales				=>	número máximo de vegetales en una misma casilla. Valores desde 1000 para la primera especie y aumentando en sucesivas especies.
		numIniVegetales				=>	número inicial de vegetales al crearse. Valores desde 100 para la primera especie y aumentando en sucesivas especies.
		capacidadMigracionLocal		=>	probabilidad en tanto por 1 de que una especie migre a una casilla colindante. La migración también depende de la habitabilidad y del numVegetales. 
										Así que si por ejemplo tenemos 0.5f en migracionLocal y 0.5 en la habitabilidad de ese hábitat hay un 25% de que migre localmente en el caso de que la
										planta este al 100% (numVegetales = numMaxVegetales). Si por ejemplo estuviera a 100/1000 (caso inicial) pues tendría un 2.5% de probabilidad de migrar.
										Cuando la planta este al 100% (en 10 turnos) pues migrara con un 25% de probabilidad
		capacidadMigracionGlobal	=>	igual que la local sólo que prueba a migrar a una casilla que puede estar entre 1 y radioMigración de distancia en cualquier direccion
		radioMigracion				=>	distancia a la que puede migrar globalmente un vegetal
		turnosEvolucionInicial		=>	turnos que tienen que pasar para que un vegetal mejore la habitabilidad del hábitat en el que está
		evolucion					=>	mejora que se produce cuando turnosEvolucion llega a 0
		habitabilidadInicial		=>	lista de floats desde -1.0f (inhabitable) a 0.0 (decreciente) y hasta 1.0f (creciente). La lista tiene que ser del mismo tamaño que el total de los habitats
		idTextura					=>	id de la textura que se pinta debajo del modelo
		modelos						=>	lista de los diferentes modelos que tiene la especie		
		*/
		//List<T_habitats> habSeta = new List<T_habitats>();
		//habSeta.Add(T_habitats.montana);		
		//Seta: habitats -> montaña
		List<float> habSeta = new List<float>();
		habSeta.Add( 0.5f);//montana
		habSeta.Add(-1.0f);//llanura
		habSeta.Add(-1.0f);//colina
		habSeta.Add(-1.0f);//desierto
		habSeta.Add(-1.0f);//volcanico
		habSeta.Add(-1.0f);//mar
		habSeta.Add(-1.0f);//costa
		habSeta.Add(-1.0f);//tundra
		habSeta.Add(-1.0f);//inhabitable		
		seta = new EspecieVegetal("Seta",8,1000,100,0.5f,0.2f,20,10,0.01f,habSeta,1,modelosVegetales.setas);
		
		//List<T_habitats> habFlor = new List<T_habitats>();
		//habFlor.Add(T_habitats.llanura);
		//Flor: habitats -> llanura
		List<float> habFlor = new List<float>();
		habFlor.Add(-1.0f);//montana
		habFlor.Add( 0.5f);//llanura
		habFlor.Add(-1.0f);//colina
		habFlor.Add(-1.0f);//desierto
		habFlor.Add(-1.0f);//volcanico
		habFlor.Add(-1.0f);//mar
		habFlor.Add(-1.0f);//costa
		habFlor.Add(-1.0f);//tundra
		habFlor.Add(-1.0f);//inhabitable		
		flor = new EspecieVegetal("Flor",8,1000,100,0.5f,0.2f,10,10,0.01f,habFlor,2,modelosVegetales.flores);
		
		//List<T_habitats> habCana = new List<T_habitats>();
		//habCana.Add(T_habitats.costa);
		//habCana.Add(T_habitats.desierto);
		//Palo (Caña): habitats -> llanura, costa y desierto
		List<float> habCana = new List<float>();
		habCana.Add(-1.0f);//montana
		habCana.Add( 0.5f);//llanura
		habCana.Add(-1.0f);//colina
		habCana.Add( 0.5f);//desierto
		habCana.Add(-1.0f);//volcanico
		habCana.Add(-1.0f);//mar
		habCana.Add( 0.5f);//costa
		habCana.Add(-1.0f);//tundra
		habCana.Add(-1.0f);//inhabitable		
		palo = new EspecieVegetal("Caña",8,1000,100,0.5f,0.2f,10,10,0.01f,habCana,3,modelosVegetales.canas);
		
		/*List<T_habitats> habArbusto = new List<T_habitats>();
		habArbusto.Add(T_habitats.llanura);
		habArbusto.Add(T_habitats.colina);
		habArbusto.Add(T_habitats.montana);
		habArbusto.Add(T_habitats.tundra);*/
		//Arbusto: habitats -> llanura, colina, montaña y tundra
		List<float> habArbusto = new List<float>();
		habArbusto.Add(-1.0f);//montana
		habArbusto.Add( 0.5f);//llanura
		habArbusto.Add(-1.0f);//colina
		habArbusto.Add( 0.5f);//desierto
		habArbusto.Add(-1.0f);//volcanico
		habArbusto.Add(-1.0f);//mar
		habArbusto.Add( 0.5f);//costa
		habArbusto.Add(-1.0f);//tundra
		habArbusto.Add(-1.0f);//inhabitable		
		arbusto = new EspecieVegetal("Arbusto",8,1000,100,0.5f,0.2f,10,10,0.01f,habArbusto,2,modelosVegetales.arbustos);
		
		/*List<T_habitats> habEstrom = new List<T_habitats>();
		habEstrom.Add(T_habitats.costa);
		habEstrom.Add(T_habitats.montana);
		habEstrom.Add(T_habitats.volcanico);*/
		//Estrom (Estromatolito): habitats -> costa, desierto y volcanico
		List<float> habEstrom = new List<float>();
		habEstrom.Add(-1.0f);//montana
		habEstrom.Add( 0.5f);//llanura
		habEstrom.Add(-1.0f);//colina
		habEstrom.Add( 0.5f);//desierto
		habEstrom.Add(-1.0f);//volcanico
		habEstrom.Add(-1.0f);//mar
		habEstrom.Add( 0.5f);//costa
		habEstrom.Add(-1.0f);//tundra
		habEstrom.Add(-1.0f);//inhabitable		
		estrom = new EspecieVegetal("Estromatolito",8,1000,100,0.5f,0.2f,10,10,0.01f,habEstrom,4,modelosVegetales.estromatolitos);
		
		//List<T_habitats> habCactus = new List<T_habitats>();
		//habCactus.Add(T_habitats.desierto);
		//Cactus: habitats -> desierto
		List<float> habCactus = new List<float>();
		habCactus.Add(-1.0f);//montana
		habCactus.Add( 0.5f);//llanura
		habCactus.Add(-1.0f);//colina
		habCactus.Add( 0.5f);//desierto
		habCactus.Add(-1.0f);//volcanico
		habCactus.Add(-1.0f);//mar
		habCactus.Add( 0.5f);//costa
		habCactus.Add(-1.0f);//tundra
		habCactus.Add(-1.0f);//inhabitable		
		cactus = new EspecieVegetal("Cactus",8,1000,100,0.5f,0.2f,10,10,0.01f,habCactus,0,modelosVegetales.cactus);
		
		//List<T_habitats> habPalm = new List<T_habitats>();
		//habPalm.Add(T_habitats.costa);
		//Palmera: habitats -> costa
		List<float> habPalm = new List<float>();
		habPalm.Add(-1.0f);//montana
		habPalm.Add(-0.8f);//llanura
		habPalm.Add(-1.0f);//colina
		habPalm.Add(-0.8f);//desierto
		habPalm.Add(-1.0f);//volcanico
		habPalm.Add(-1.0f);//mar
		habPalm.Add( 0.6f);//costa
		habPalm.Add(-1.0f);//tundra
		habPalm.Add(-1.0f);//inhabitable		
		palmera = new EspecieVegetal("Palmera",8,1000,100,0.5f,0.2f,10,10,0.01f,habPalm,3,modelosVegetales.palmeras);
		
		/*List<T_habitats> habPino = new List<T_habitats>();
		habPino.Add(T_habitats.montana);
		habPino.Add(T_habitats.colina);
		habPino.Add(T_habitats.tundra);*/
		//Pino: habitats -> tundra, colina y montaña
		List<float> habPino = new List<float>();
		habPino.Add(-1.0f);//montana
		habPino.Add( 0.5f);//llanura
		habPino.Add(-1.0f);//colina
		habPino.Add( 0.5f);//desierto
		habPino.Add(-1.0f);//volcanico
		habPino.Add(-1.0f);//mar
		habPino.Add( 0.5f);//costa
		habPino.Add(-1.0f);//tundra
		habPino.Add(-1.0f);//inhabitable		
		pino = new EspecieVegetal("Pino",8,1000,100,0.5f,0.2f,10,10,0.01f,habPino,4,modelosVegetales.pinos);
		
		/*List<T_habitats> habCipres = new List<T_habitats>();
		habCipres.Add(T_habitats.montana);
		habCipres.Add(T_habitats.colina);
		habCipres.Add(T_habitats.tundra);*/
		//Ciprés: habitats -> tundra, colina y montaña
		List<float> habCipres = new List<float>();
		habCipres.Add(-1.0f);//montana
		habCipres.Add( 0.5f);//llanura
		habCipres.Add(-1.0f);//colina
		habCipres.Add( 0.5f);//desierto
		habCipres.Add(-1.0f);//volcanico
		habCipres.Add(-1.0f);//mar
		habCipres.Add( 0.5f);//costa
		habCipres.Add(-1.0f);//tundra
		habCipres.Add(-1.0f);//inhabitable		
		cipres = new EspecieVegetal("Ciprés",8,1000,100,0.5f,0.2f,10,10,0.01f,habCipres,4,modelosVegetales.cipreses);
		
		/*List<T_habitats> habPinoAlto = new List<T_habitats>();
		habPinoAlto.Add(T_habitats.montana);
		habPinoAlto.Add(T_habitats.tundra);*/
		//Pino Alto: habitats -> tundra y montaña
		List<float> habPinoAlto = new List<float>();
		habPinoAlto.Add(-1.0f);//montana
		habPinoAlto.Add( 0.5f);//llanura
		habPinoAlto.Add(-1.0f);//colina
		habPinoAlto.Add( 0.5f);//desierto
		habPinoAlto.Add(-1.0f);//volcanico
		habPinoAlto.Add(-1.0f);//mar
		habPinoAlto.Add( 0.5f);//costa
		habPinoAlto.Add(-1.0f);//tundra
		habPinoAlto.Add(-1.0f);//inhabitable		
		pinoAlto = new EspecieVegetal("Pino Alto",8,1000,100,0.5f,0.2f,10,10,0.01f,habPinoAlto,1,modelosVegetales.pinosAltos);
		
		
		List<T_habitats> listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.costa);
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.tundra);
		listaHabs.Add(T_habitats.desierto);
		
		/* Herbivoros */
		listaHabs.Clear();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.costa);
		//Conejo: habitats -> costa, llanura y colina
		herbivoro1 = new EspecieAnimal("Conejo",2,10,100,100,5,1,100,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro1);
		
		listaHabs.Add(T_habitats.desierto);
		//Cabra: habitats -> llanura, colina y desierto
		herbivoro2 = new EspecieAnimal("Camello",2,10,100,100,5,1,100,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro2);
		
		listaHabs.Clear();
		listaHabs.Add(T_habitats.costa);
		//Tortuga: habitats -> costa
		herbivoro3 = new EspecieAnimal("Tortuga",8,10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro3);
		
		listaHabs.Clear();
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Add(T_habitats.tundra);
		//Ciervo: habitats -> colina, tundra y montaña
		herbivoro4 = new EspecieAnimal("Ciervo",8,10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro4);
		
		listaHabs.Remove(T_habitats.tundra);
		listaHabs.Add(T_habitats.volcanico);
		//Salamandra: habitats-> colina, montaña y volcanico
		herbivoro5 = new EspecieAnimal("Salamandra",8,10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro5);
		
		/* Carnivoros */
		listaHabs.Clear();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.costa);
		//Zorro: habitats -> costa, llanura y colina
		carnivoro1 = new EspecieAnimal("Zorro",8,10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro1);
		
		listaHabs.Add(T_habitats.tundra);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Remove(T_habitats.costa);
		//Lobo: habitats -> llanura, colina, tundra y montaña
		carnivoro2 = new EspecieAnimal("Lobo",8,10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro2);
		
		listaHabs.Add(T_habitats.desierto);
		listaHabs.Remove(T_habitats.montana);
		listaHabs.Remove(T_habitats.tundra);
		//Serpiente: habitats -> llanura, desierto y colina
		carnivoro3 = new EspecieAnimal("Serpiente",8,10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro3);
		
		listaHabs.Add(T_habitats.montana);
		listaHabs.Remove(T_habitats.desierto);
		//Tigre: habitats -> llanura, colina, montaña
		carnivoro4 = new EspecieAnimal("Tigre",8,10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro4);
		
		listaHabs.Add(T_habitats.tundra);
		listaHabs.Add(T_habitats.volcanico);
		listaHabs.Remove(T_habitats.montana);
		//Velocitaptor: habitats -> llanura, colina, tundra y volcanico
		carnivoro5 = new EspecieAnimal("Velociraptor",8,10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro5);
		
	}
	
	void Start () {
		
		//NO CAMBIAR EL ORDEN CON EL QUE SE AÑADEN
		//Añadir a Vida los edificios
		principal.anadeTipoEdificio(fabricaComBas);
		principal.anadeTipoEdificio(energia);
		principal.anadeTipoEdificio(granja);
		principal.anadeTipoEdificio(fabricaComAdv);
		principal.anadeTipoEdificio(energiaAdv);
		
		//Añadir a Vida los vegetales
		principal.anadeEspecieVegetal(seta);
		principal.anadeEspecieVegetal(flor);
		principal.anadeEspecieVegetal(palo);
		principal.anadeEspecieVegetal(arbusto);
		principal.anadeEspecieVegetal(estrom);
		principal.anadeEspecieVegetal(cactus);
		principal.anadeEspecieVegetal(palmera);
		principal.anadeEspecieVegetal(pino);
		principal.anadeEspecieVegetal(cipres);
		principal.anadeEspecieVegetal(pinoAlto);
		
		//Añadir a Vida los herbivoros
		principal.anadeEspecieAnimal(herbivoro1);
		principal.anadeEspecieAnimal(herbivoro2);
		principal.anadeEspecieAnimal(herbivoro3);
		principal.anadeEspecieAnimal(herbivoro4);
		principal.anadeEspecieAnimal(herbivoro5);
		
		//Añadir a Vida los carnivoros
		principal.anadeEspecieAnimal(carnivoro1);
		principal.anadeEspecieAnimal(carnivoro2);
		principal.anadeEspecieAnimal(carnivoro3);
		principal.anadeEspecieAnimal(carnivoro4);
		principal.anadeEspecieAnimal(carnivoro5);
		
		principal.vida.actualizaNumTurnos();
	}
}
