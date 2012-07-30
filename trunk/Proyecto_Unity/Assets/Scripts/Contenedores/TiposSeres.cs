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
		
		fabricaComBas = new TipoEdificio("Fábrica componentes básicos",habsEdificios,modelosEdificios.fabCompBas);
		energia = new TipoEdificio("Central de energía",habsEdificios,modelosEdificios.centralEnergia);
		granja = new TipoEdificio("Granja",habsEdificios,modelosEdificios.granja);
		fabricaComAdv = new TipoEdificio("Fábrica de componentes avanzados",habsEdificiosAdv,modelosEdificios.fabCompAdv);
		energiaAdv = new TipoEdificio("Central de energía avanzada",habsEdificiosAdv,modelosEdificios.centralEnergiaAdv);
		
		List<T_habitats> habSeta = new List<T_habitats>();
		habSeta.Add(T_habitats.montana);
		/* Vegetales */		
		//Seta: habitats -> montaña
		seta = new EspecieVegetal("Seta",1000,50,50,20,0.1f,8,habSeta,1,modelosVegetales.setas);
		

		List<T_habitats> habFlor = new List<T_habitats>();
		habFlor.Add(T_habitats.llanura);
		habFlor.Add(T_habitats.colina);
		//Flor: habitats -> llanura, colina
		flor = new EspecieVegetal("Flor",1000,50,50,20,0.1f,15,habFlor,2,modelosVegetales.flores);
		
		List<T_habitats> habCana = new List<T_habitats>();
		habCana.Add(T_habitats.costa);
		habCana.Add(T_habitats.desierto);
		//Palo (Caña): habitats -> llanura, costa y desierto
		palo = new EspecieVegetal("Caña",1000,50,50,20,0.1f,12,habCana,3,modelosVegetales.canas);
		
		List<T_habitats> habArbusto = new List<T_habitats>();
		habArbusto.Add(T_habitats.llanura);
		habArbusto.Add(T_habitats.colina);
		habArbusto.Add(T_habitats.montana);
		habArbusto.Add(T_habitats.tundra);
		//Arbusto: habitats -> llanura, colina, montaña y tundra
		arbusto = new EspecieVegetal("Arbusto",1000,50,50,20,0.1f,12,habArbusto,2,modelosVegetales.arbustos);
		
		List<T_habitats> habEstrom = new List<T_habitats>();
		habEstrom.Add(T_habitats.costa);
		habEstrom.Add(T_habitats.montana);
		habEstrom.Add(T_habitats.volcanico);
		//Estrom (Estromatolito): habitats -> costa, desierto y volcanico
		estrom = new EspecieVegetal("Estromatolito",1000,50,50,20,0.1f,12,habEstrom,4,modelosVegetales.estromatolitos);
		
		List<T_habitats> habCactus = new List<T_habitats>();
		habCactus.Add(T_habitats.desierto);
		//Cactus: habitats -> desierto
		cactus = new EspecieVegetal("Cactus",1000,50,50,20,0.1f,12,habCactus,0,modelosVegetales.cactus);
		
		List<T_habitats> habPalm = new List<T_habitats>();
		habPalm.Add(T_habitats.costa);
		//Palmera: habitats -> costa
		palmera = new EspecieVegetal("Palmera",1000,50,50,20,0.1f,12,habPalm,3,modelosVegetales.palmeras);
		
		List<T_habitats> habPino = new List<T_habitats>();
		habPino.Add(T_habitats.montana);
		habPino.Add(T_habitats.colina);
		habPino.Add(T_habitats.tundra);
		//Pino: habitats -> tundra, colina y montaña
		pino = new EspecieVegetal("Pino",1000,50,50,20,0.1f,12,habPino,4,modelosVegetales.pinos);
		
		List<T_habitats> habCipres = new List<T_habitats>();
		habCipres.Add(T_habitats.montana);
		habCipres.Add(T_habitats.colina);
		habCipres.Add(T_habitats.tundra);
		//Ciprés: habitats -> tundra, colina y montaña
		cipres = new EspecieVegetal("Ciprés",1000,50,50,20,0.1f,12,habCipres,4,modelosVegetales.cipreses);
		
		List<T_habitats> habPinoAlto = new List<T_habitats>();
		habPinoAlto.Add(T_habitats.montana);
		habPinoAlto.Add(T_habitats.tundra);
		//Pino Alto: habitats -> tundra y montaña
		pinoAlto = new EspecieVegetal("Pino Alto",1000,50,50,20,0.1f,12,habPinoAlto,1,modelosVegetales.pinosAltos);
		
		
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
	}
}
