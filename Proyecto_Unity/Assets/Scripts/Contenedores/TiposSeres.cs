using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TiposSeres : MonoBehaviour {
	
	//Tipos de edificios
	private TipoEdificio fabrica1;
	private TipoEdificio fabrica2;
	private TipoEdificio energia1;
	private TipoEdificio energia2;
	private TipoEdificio granja;
	
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
		
		List<T_habitats> listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.tundra);
		listaHabs.Add(T_habitats.desierto);
		listaHabs.Add(T_habitats.volcanico);
		
		fabrica1 = new TipoEdificio("Fábrica componentes básicos",listaHabs,modelosEdificios.fabCompBas);
		energia1 = new TipoEdificio("Central de energía",listaHabs,modelosEdificios.centralEnergia);
		granja = new TipoEdificio("Granja",listaHabs,modelosEdificios.granja);
		fabrica2 = new TipoEdificio("Fábrica de componentes avanzados",listaHabs,modelosEdificios.fabCompAdv);
		energia2 = new TipoEdificio("Central de energía avanzada",listaHabs,modelosEdificios.centralEnergiaAdv);
		
		listaHabs.Clear();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Add(T_habitats.colina);
		/* Vegetales */		
		//Seta: habitats -> llanura, montaña y colina
		seta = new EspecieVegetal("Seta",1000,50,50,50,0.1f,8,listaHabs,0,modelosVegetales.setas);
		
		listaHabs.Remove(T_habitats.costa);
		listaHabs.Remove(T_habitats.montana);
		listaHabs.Add(T_habitats.tundra);
		//Flor: habitats -> llanura, tundra y colina
		flor = new EspecieVegetal("Flor",1000,50,50,20,0.1f,15,listaHabs,1,modelosVegetales.flores);
		
		listaHabs.Add(T_habitats.costa);
		listaHabs.Add(T_habitats.desierto);
		listaHabs.Remove(T_habitats.colina);
		//Palo (Caña): habitats -> llanura, costa y desierto
		palo = new EspecieVegetal("Caña",1000,50,50,20,0.1f,12,listaHabs,2,modelosVegetales.canas);
		
		listaHabs.Remove(T_habitats.costa);
		listaHabs.Remove(T_habitats.desierto);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.volcanico);
		listaHabs.Add(T_habitats.tundra);
		//Arbusto: habitats -> llanura, montaña, colina, tundra y volcanico
		arbusto = new EspecieVegetal("Arbusto",1000,50,50,20,0.1f,12,listaHabs,3,modelosVegetales.arbustos);
		
		listaHabs.Add(T_habitats.desierto);
		listaHabs.Remove(T_habitats.llanura);
		listaHabs.Remove(T_habitats.colina);
		//Estrom (Estromatolito): habitats -> montaña, desierto y volcanico
		estrom = new EspecieVegetal("Estromatolito",1000,50,50,20,0.1f,12,listaHabs,3,modelosVegetales.estromatolitos);
		
		listaHabs.Remove(T_habitats.montana);
		listaHabs.Remove(T_habitats.volcanico);
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		//Cactus: habitats -> llanura, colina y desierto
		cactus = new EspecieVegetal("Cactus",1000,50,50,20,0.1f,12,listaHabs,3,modelosVegetales.cactus);
		
		listaHabs.Add(T_habitats.costa);
		listaHabs.Remove(T_habitats.desierto);
		//Palmera: habitats -> costa, llanura y colina
		palmera = new EspecieVegetal("Palmera",1000,50,50,20,0.1f,12,listaHabs,3,modelosVegetales.palmeras);
		
		listaHabs.Remove(T_habitats.costa);
		listaHabs.Add(T_habitats.montana);
		//Pino: habitats -> llanura, colina y montaña
		pino = new EspecieVegetal("Pino",1000,50,50,20,0.1f,12,listaHabs,3,modelosVegetales.pinos);
		
		listaHabs.Remove(T_habitats.montana);
		listaHabs.Add(T_habitats.tundra);
		//Ciprés: habitats -> tundra, llanura y colina
		cipres = new EspecieVegetal("Ciprés",1000,50,50,20,0.1f,12,listaHabs,3,modelosVegetales.cipreses);
		
		listaHabs.Remove(T_habitats.llanura);
		listaHabs.Remove(T_habitats.costa);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Add(T_habitats.tundra);
		//Pino Alto: habitats -> colina, tundra y montaña
		pinoAlto = new EspecieVegetal("Pino Alto",1000,50,50,20,0.1f,12,listaHabs,3,modelosVegetales.pinosAltos);
		
		
		/* Herbivoros */
		listaHabs.Clear();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.costa);
		//Conejo: habitats -> costa, llanura y colina
		herbivoro1 = new EspecieAnimal("Conejo",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro1);
		
		listaHabs.Add(T_habitats.desierto);
		//Cabra: habitats -> llanura, colina y desierto
		herbivoro2 = new EspecieAnimal("Camello",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro2);
		
		listaHabs.Clear();
		listaHabs.Add(T_habitats.costa);
		//Tortuga: habitats -> costa
		herbivoro3 = new EspecieAnimal("Tortuga",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro3);
		
		listaHabs.Clear();
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Add(T_habitats.tundra);
		//Ciervo: habitats -> colina, tundra y montaña
		herbivoro4 = new EspecieAnimal("Ciervo",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro4);
		
		listaHabs.Remove(T_habitats.tundra);
		listaHabs.Add(T_habitats.volcanico);
		//Salamandra: habitats-> colina, montaña y volcanico
		herbivoro5 = new EspecieAnimal("Salamandra",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro5);
		
		/* Carnivoros */
		listaHabs.Clear();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.costa);
		//Zorro: habitats -> costa, llanura y colina
		carnivoro1 = new EspecieAnimal("Zorro",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro1);
		
		listaHabs.Add(T_habitats.tundra);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Remove(T_habitats.costa);
		//Lobo: habitats -> llanura, colina, tundra y montaña
		carnivoro2 = new EspecieAnimal("Lobo",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro2);
		
		listaHabs.Add(T_habitats.desierto);
		listaHabs.Remove(T_habitats.montana);
		listaHabs.Remove(T_habitats.tundra);
		//Serpiente: habitats -> llanura, desierto y colina
		carnivoro3 = new EspecieAnimal("Serpiente",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro3);
		
		listaHabs.Add(T_habitats.montana);
		listaHabs.Remove(T_habitats.desierto);
		//Tigre: habitats -> llanura, colina, montaña
		carnivoro4 = new EspecieAnimal("Tigre",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro4);
		
		listaHabs.Add(T_habitats.tundra);
		listaHabs.Add(T_habitats.volcanico);
		listaHabs.Remove(T_habitats.montana);
		//Velocitaptor: habitats -> llanura, colina, tundra y volcanico
		carnivoro5 = new EspecieAnimal("Velociraptor",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro5);
		
	}
	
	void Start () {
		
		//Añadir a Vida los edificios
		principal.anadeTipoEdificio(fabrica1);
		principal.anadeTipoEdificio(fabrica2);
		principal.anadeTipoEdificio(energia1);
		principal.anadeTipoEdificio(energia2);
		principal.anadeTipoEdificio(granja);
		
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
	}
}
