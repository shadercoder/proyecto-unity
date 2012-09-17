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
	
	//Descripciones
	private List<string> descripciones;
	private string descripcionSeta				= "Del reino de los hongos, las setas son organismos simples que crecen en llanuras y colinas. No producen mucha comida, pero su reproduccion por esporas es muy efectiva y les permite extenderse a gran distancia.";
	private string descripcionFlor				= "Las flores son plantas peque\u00f1as que crecen en las llanuras, cubriendolas de color. Producen mas comida que las setas, aunque se reproducen menos y de forma mas local.";
	private string descripcionCana				= "Alargadas y finas, las ca\u00f1as crecen en llanuras, costas y desiertos, adaptandose muy bien al entorno en el que se encuentren. No producen mucho alimento, pero pueden expandirse a zonas lejanas.";
	private string descripcionArbusto			= "Los arbustos son plantas muy resistentes, que crecen en llanuras y colinas. Lo mas destacable de ellas es su adaptabilidad, pues una vez que se afianzan en un lugar se adaptan muy bien a su entorno.";
	private string descripcionEstromatolito		= "A medio camino entre las algas y el coral, los estromatolitos forman sus estructuras en las costas. Son organismos muy locales y de reproduccion lenta, pero muy estables y resistentes.";
	private string descripcionCactus			= "Los cactus se extienden por los desiertos y los terrenos volcanicos. Son una fuente muy rica en alimentos para las especies que se alimentan de ellos y pueden reproducirse a grandes distancias.";
	private string descripcionPalmera			= "El habitat natural de la palmera es la costa, aunque tambien pueden encontrarse en llanuras. Son plantas muy resistentes, con una alta produccion de alimento y una gran capacidad de adaptacion al medio.";
	private string descripcionPino				= "Los pinos son arboles de gran tama\u00f1o que crecen en llanuras, colinas y en menor medida en monta\u00f1as. Ademas de su gran resistencia, se adaptan muy bien al entorno en el que crecen.";
	private string descripcionCipres			= "Pocos habitats existen donde un cipres no pueda nacer. Estos arboles crecen muy rapido y tienen un factor de adaptacion extraordinariamente alto, lo que les permite sobrevivir en todo tipo de lugares.";
	private string descripcionPinoAlto			= "El pino alto se encuentra con frecuencia en colinas y monta\u00f1as, aunque puede vivir en mas lugares. Es un arbol enorme y con muchas hojas que representa lo mas alto en la cadena de los vegetales.";
	private string descripcionHerb1				= "Lento pero seguro, el caracol consume poco y gasta poco, lo que le confiere una durabilidad bastante elevada. Su reproducción hermafrodita es rapida y sencilla, y se traduce en abundancia de caracoles. Presa fácil!";
	private string descripcionHerb2				= "El conejo es un animal de reproduccion rapida y numerosa. Combinado con un consumo elevado de comira, el conejo puede convertirse tanto en una plaga como en un suministro abundante para los carnivoros";
	private string descripcionHerb3				= "La familia de los vacunos se caracteriza por 2 cosas: comer mucho y moverse poco. Afortunadamente tampoco son veloces reproduciendose, o no quedaria verde sobre la faz del planeta donde habitara esta especie.";
	private string descripcionHerb4				= "Los grandes herbivoros de la sabana, entre los que se encuentran la jirafa o la cebra, han adaptado su metabolismo para la escasez de comida, consumiendo muy poca. Tambien a la huida de depredadores, claro...";
	private string descripcionHerb5				= "La tortuga gigante es un animal enorme que puede vivir muchos a\u00f1os. A pesar de tener un apetito insaciable, el hecho de tener una reproduccion muy lenta permite cierto control sobre sus poblaciones.";
	private string descripcionCarn1				= "El zorro es un animal oportunista, que se alimenta de otros animales cuando puede aprovechando sus puntos debiles. Puede aguantar bastante sin ingerir alimento, pues no es un animal muy voraz.";
	private string descripcionCarn2				= "El lobo es un animal voraz que se alimenta de animales mas peque\u00f1os que el. Caza normalmente en grupos, acorralando a sus presas, aunque también hay casos de especimenes solitarios. Adoran los conejos.";
	private string descripcionCarn3				= "El tigre es uno de los felinos mas grandes conocidos. Es capaz de alimentarse de animales mas grandes que el a las que despedaza con un solo golpe de sus poderosas zarpas. Afortunadamente, no les gusta cazar mucho.";
	private string descripcionCarn4				= "Los osos son animales sencillos que no se meten con nadie. Menos cuando tienen hambre, amenazan su territorio, son mas peque\u00f1os que ellos o estan demasiado cerca... Comenzamos. Los osos son animales sencillos.";
	private string descripcionCarn5				= "Los tiranosaurios representan la cima de la cadena alimenticia desde que existe cadena alimenticia. Son enormes, cualquier animal es su presa y pocos logran escapar de sus fauces. Se recomienda un control de poblacion estricto.";
	private string descripcionFabBas			= "En este edificio se producen componentes de construccion basicos, necesarios para casi todas las construcciones que puede erigir la nave y para las reparaciones.";
	private string descripcionEnergia1			= "A traves de un reactor de fusion y otro de fision, este edificio produce una alta cantidad de energia. Este combinado de reacciones es practicamente inagotable y limpio.";
	private string descripcionGranja			= "Recolectando seres vivos de los alrededores, la granja es capaz de refinar compuestos de material biologico esenciales para el fin de la mision encomendada a la nave.";
	private string descripcionFabAdv			= "La fabrica de componentes avanzados recolecta ciertos metales raros para producir materiales complejos y muy valiosos, permitiendo construir e investigar en nuevos frentes.";
	private string descripcionEnergia2			= "Creando un campo de contencion magnetica muy poderoso, este edificio capta y utiliza materia oscura para producir una ingente cantidad de energia.";
	
	//Costes
	private List<List<int>> costesSeres;
	private List<int> costeSeta;
	private List<int> costeFlor;
	private List<int> costeCana;
	private List<int> costeArbusto;
	private List<int> costeEstromatolito;
	private List<int> costeCactus;
	private List<int> costePalmera;
	private List<int> costePino;
	private List<int> costeCipres;
	private List<int> costePinoAlto;
	private List<int> costeHerb1;
	private List<int> costeHerb2;
	private List<int> costeHerb3;
	private List<int> costeHerb4;
	private List<int> costeHerb5;
	private List<int> costeCarn1;
	private List<int> costeCarn2;
	private List<int> costeCarn3;
	private List<int> costeCarn4;
	private List<int> costeCarn5;
	
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
		
		descripciones = new List<string>();
		costesSeres = new List<List<int>>();
		
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
		fabricaComBas = new TipoEdificio("Fábrica de comp. básicos",habsEdificios,180,20,0,0,T_elementos.comunes,10,0,0,0,0,20,0,0,modelosEdificios.fabCompBas);
		energia = new TipoEdificio("Central de energía",habsEdificios,150,30,0,0,T_elementos.comunes,0,0,0,0,40,0,0,0,modelosEdificios.centralEnergia);
		granja = new TipoEdificio("Granja",habsEdificios,800,280,0,0,T_elementos.nada,75,0,0,0,0,0,0,0,modelosEdificios.granja);
		fabricaComAdv = new TipoEdificio("Fábrica de comp. avanzados",habsEdificiosAdv,1000,350,0,10,T_elementos.raros,100,0,0,0,0,0,40,0,modelosEdificios.fabCompAdv);
		energiaAdv = new TipoEdificio("Central de energía avanzada",habsEdificiosAdv,2500,250,50,20,T_elementos.raros,0,0,0,0,1000,0,0,0,modelosEdificios.centralEnergiaAdv);
		
		/* Vegetales */
		/*vegetal = new EspecieVegetal(nombre, numMaxSeresEspecie, numMaxVegetales, numIniVegetales, capacidadMigracionLocal, capacidadMigracionGlobal, radioMigracion, 
									   turnosEvolucionInicial, evolucion, habitabilidadInicial, idTextura, modelos
		nombre						=>	nombre de la especie
		numMaxSeresEspecie			=>	número máximo de seres de esa especie
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
		
		//VEGETALES ------------------------------------------------------------
		
		//Seta: habitats -> llanura y colina
		//Tier 1. Barata y normal en llanura y colina. Poca produccion de comida pero alta reproductibilidad y rango de migracion
		List<float> habSeta = new List<float>();
		habSeta.Add(-0.1f);//montana
		habSeta.Add( 0.4f);//llanura
		habSeta.Add( 0.5f);//colina
		habSeta.Add(-1.0f);//desierto
		habSeta.Add(-1.0f);//volcanico
		habSeta.Add(-1.0f);//mar
		habSeta.Add(-0.6f);//costa
		habSeta.Add(-1.0f);//tundra
		habSeta.Add(-1.0f);//inhabitable		
		seta = new EspecieVegetal("Seta",50,1000,100,0.05f,0.02f,7,20,0.01f,habSeta,1,10,modelosVegetales.setas);
		costeSeta = new List<int>();
		costeSeta.Add(200);//Coste energia
		costeSeta.Add(110);//Coste comp bas
		costeSeta.Add(0);//Coste comp adv
		costeSeta.Add(0);//Coste mat bio
		
		//Flor: habitats -> llanura
		//Tier 1. Barata y decente en llanuras. Mediocre produccion de alimento, reproductibilidad buena pero rango de migracion normal.
		List<float> habFlor = new List<float>();
		habFlor.Add(-0.4f);//montana
		habFlor.Add( 0.5f);//llanura
		habFlor.Add(-0.3f);//colina
		habFlor.Add(-1.0f);//desierto
		habFlor.Add(-1.0f);//volcanico
		habFlor.Add(-1.0f);//mar
		habFlor.Add(-1.0f);//costa
		habFlor.Add(-1.0f);//tundra
		habFlor.Add(-1.0f);//inhabitable		
		flor = new EspecieVegetal("Flor",50,1200,120,0.05f,0.02f,4,10,0.01f,habFlor,2,10,modelosVegetales.flores);//0.3f,0.2f,4,10,0.01f,habFlor,2,modelosVegetales.flores);
		costeFlor = new List<int>();
		costeFlor.Add(230);//Coste energia
		costeFlor.Add(100);//Coste comp bas
		costeFlor.Add(0);//Coste comp adv
		costeFlor.Add(0);//Coste mat bio
		
		//Palo (Caña): habitats -> llanura, costa y desierto
		//Tier 2. Normal en llanura, desierto y costa, produccion de alimento normal, reproductibilidad normal y rango alto. Alto ratio de evolucion.
		List<float> habCana = new List<float>();
		habCana.Add(-1.0f);//montana
		habCana.Add( 0.5f);//llanura
		habCana.Add(-0.5f);//colina
		habCana.Add( 0.3f);//desierto
		habCana.Add(-1.0f);//volcanico
		habCana.Add(-1.0f);//mar
		habCana.Add( 0.5f);//costa
		habCana.Add(-1.0f);//tundra
		habCana.Add(-1.0f);//inhabitable		
		palo = new EspecieVegetal("Caña",50,1500,250,0.05f,0.01f,6,8,0.04f,habCana,3,10,modelosVegetales.canas);
		costeCana = new List<int>();
		costeCana.Add(450);//Coste energia
		costeCana.Add(150);//Coste comp bas
		costeCana.Add(0);//Coste comp adv
		costeCana.Add(0);//Coste mat bio
		
		//Arbusto: habitats -> llanura, colina, montaña y desierto
		//Tier 2. Decente en llanura y colina. Produccion normal y reproductibilidad normal. Rango bajo y evolucion lenta pero alta.
		List<float> habArbusto = new List<float>();
		habArbusto.Add(-0.3f);//montana
		habArbusto.Add( 0.5f);//llanura
		habArbusto.Add( 0.6f);//colina
		habArbusto.Add(-0.2f);//desierto
		habArbusto.Add(-1.0f);//volcanico
		habArbusto.Add(-1.0f);//mar
		habArbusto.Add(-1.0f);//costa
		habArbusto.Add(-1.0f);//tundra
		habArbusto.Add(-1.0f);//inhabitable		
		arbusto = new EspecieVegetal("Arbusto",50,1800,220,0.04f,0.01f,3,20,0.1f,habArbusto,2,5,modelosVegetales.arbustos);
		costeArbusto = new List<int>();
		costeArbusto.Add(420);//Coste energia
		costeArbusto.Add(170);//Coste comp bas
		costeArbusto.Add(0);//Coste comp adv
		costeArbusto.Add(0);//Coste mat bio
		
		//Estrom (Estromatolito): habitats -> costa, desierto y volcanico
		//Tier 3. Muy buena en costas. Alta produccion de alimento y reproductibilidad un poco baja. Poca migracion y poco radio. Evolucion decente.
		List<float> habEstrom = new List<float>();
		habEstrom.Add(-1.0f);//montana
		habEstrom.Add( 0.5f);//llanura
		habEstrom.Add(-1.0f);//colina
		habEstrom.Add(-0.8f);//desierto
		habEstrom.Add(-0.1f);//volcanico
		habEstrom.Add(-1.0f);//mar
		habEstrom.Add( 0.8f);//costa
		habEstrom.Add(-0.4f);//tundra
		habEstrom.Add(-1.0f);//inhabitable		
		estrom = new EspecieVegetal("Estromatolito",50,2400,350,0.03f,0.01f,3,25,0.03f,habEstrom,4,5,modelosVegetales.estromatolitos);
		costeEstromatolito = new List<int>();
		costeEstromatolito.Add(620);//Coste energia
		costeEstromatolito.Add(190);//Coste comp bas
		costeEstromatolito.Add(0);//Coste comp adv
		costeEstromatolito.Add(5);//Coste mat bio
		
		//Cactus: habitats -> desierto
		//Tier 3. Muy buena en desierto y buena en volcanico. Produccion de alimento decente, reproductibilidad normal, alto ratio de migracion y alta evolucion.
		List<float> habCactus = new List<float>();
		habCactus.Add(-0.8f);//montana
		habCactus.Add( 0.7f);//llanura
		habCactus.Add(-1.0f);//colina
		habCactus.Add( 0.6f);//desierto
		habCactus.Add( 0.2f);//volcanico
		habCactus.Add(-1.0f);//mar
		habCactus.Add(-1.0f);//costa
		habCactus.Add(-1.0f);//tundra
		habCactus.Add(-1.0f);//inhabitable		
		cactus = new EspecieVegetal("Cactus",50,2200,320,0.03f,0.03f,8,12,0.07f,habCactus,0,3,modelosVegetales.cactus);
		costeCactus = new List<int>();
		costeCactus.Add(600);//Coste energia
		costeCactus.Add(180);//Coste comp bas
		costeCactus.Add(0);//Coste comp adv
		costeCactus.Add(5);//Coste mat bio
		
		//Palmera: habitats -> costa
		//Tier 4. Buenisima en costas y mala en llanuras. Alta produccion, reproductibilidad buena, alta migracion y alta evolucion.
		List<float> habPalm = new List<float>();
		habPalm.Add(-1.0f);//montana
		habPalm.Add( 0.5f);//llanura
		habPalm.Add(-1.0f);//colina
		habPalm.Add(-0.2f);//desierto
		habPalm.Add(-1.0f);//volcanico
		habPalm.Add(-1.0f);//mar
		habPalm.Add( 0.9f);//costa
		habPalm.Add(-1.0f);//tundra
		habPalm.Add(-1.0f);//inhabitable		
		palmera = new EspecieVegetal("Palmera",50,3000,450,0.04f,0.03f,5,15,0.1f,habPalm,3,5,modelosVegetales.palmeras);
		costePalmera = new List<int>();
		costePalmera.Add(1500);//Coste energia
		costePalmera.Add(280);//Coste comp bas
		costePalmera.Add(10);//Coste comp adv
		costePalmera.Add(10);//Coste mat bio
		
		//Pino: habitats -> tundra, colina y montaña
		//Tier 4. Muy buena en colinas, buena en llanura y mediocreo en montañas. Produccion buena, reproductibilidad buena, migracion mediocre y evolucion alta.
		List<float> habPino = new List<float>();
		habPino.Add( 0.5f);//montana
		habPino.Add( 0.6f);//llanura
		habPino.Add( 0.7f);//colina
		habPino.Add(-1.0f);//desierto
		habPino.Add(-1.0f);//volcanico
		habPino.Add(-1.0f);//mar
		habPino.Add(-0.1f);//costa
		habPino.Add(-1.0f);//tundra
		habPino.Add(-1.0f);//inhabitable		
		pino = new EspecieVegetal("Pino",50,3500,420,0.05f,0.02f,5,20,0.15f,habPino,4,5,modelosVegetales.pinos);
		costePino = new List<int>();
		costePino.Add(1800);//Coste energia
		costePino.Add(300);//Coste comp bas
		costePino.Add(10);//Coste comp adv
		costePino.Add(10);//Coste mat bio
		
		//Ciprés: habitats -> tundra, colina y montaña
		//Tier 5. Bueno en muchos habitats. Procuddion muy alta, reproduccion buena, migracion normal y evolucion altisima.
		List<float> habCipres = new List<float>();
		habCipres.Add( 0.8f);//montana
		habCipres.Add( 1.0f);//llanura
		habCipres.Add( 0.6f);//colina
		habCipres.Add(-0.8f);//desierto
		habCipres.Add(-0.5f);//volcanico
		habCipres.Add(-1.0f);//mar
		habCipres.Add(-0.1f);//costa
		habCipres.Add( 0.8f);//tundra
		habCipres.Add(-1.0f);//inhabitable		
		cipres = new EspecieVegetal("Ciprés",50,4000,550,0.06f,0.02f,6,10,0.2f,habCipres,4,5,modelosVegetales.cipreses);
		costeCipres = new List<int>();
		costeCipres.Add(2500);//Coste energia
		costeCipres.Add(420);//Coste comp bas
		costeCipres.Add(50);//Coste comp adv
		costeCipres.Add(20);//Coste mat bio
		
		//Pino Alto: habitats -> tundra y montaña
		//Tier 5. Muy bueno en montaña y colina, y decente en mas. Produccion altisima, reproduccion buena, migracion alta y evolucion muy alta.
		List<float> habPinoAlto = new List<float>();
		habPinoAlto.Add( 0.9f);//montana
		habPinoAlto.Add( 0.9f);//llanura
		habPinoAlto.Add( 0.9f);//colina
		habPinoAlto.Add(-1.0f);//desierto
		habPinoAlto.Add(-1.0f);//volcanico
		habPinoAlto.Add(-1.0f);//mar
		habPinoAlto.Add(-0.5f);//costa
		habPinoAlto.Add( 0.3f);//tundra
		habPinoAlto.Add(-1.0f);//inhabitable		
		pinoAlto = new EspecieVegetal("Pino Alto",50,5000,650,0.05f,0.02f,6,12,0.2f,habPinoAlto,1,5,modelosVegetales.pinosAltos);
		costePinoAlto = new List<int>();
		costePinoAlto.Add(3000);//Coste energia
		costePinoAlto.Add(500);//Coste comp bas
		costePinoAlto.Add(50);//Coste comp adv
		costePinoAlto.Add(25);//Coste mat bio
		
		
		//ANIMALES ----------------------------------------------------
		
		List<T_habitats> listaHabs;
		
		
		/* Herbivoros */
		listaHabs = new List<T_habitats>();;
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.costa);
		
		//Caracol: habitats -> 						
		herbivoro1 = new EspecieAnimal("Caracol",15,40,1200,320,3,4,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro1);
		costeHerb1 = new List<int>();
		costeHerb1.Add(1000);//Coste energia
		costeHerb1.Add(250);//Coste comp bas
		costeHerb1.Add(0);//Coste comp adv
		costeHerb1.Add(10);//Coste mat bio
		
		listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.tundra);
		listaHabs.Add(T_habitats.montana);
		//Conejo: habitats -> 
		herbivoro2 = new EspecieAnimal("Conejo",20,100,1700,500,7,3,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro2);
		costeHerb2 = new List<int>();
		costeHerb2.Add(1500);//Coste energia
		costeHerb2.Add(350);//Coste comp bas
		costeHerb2.Add(0);//Coste comp adv
		costeHerb2.Add(15);//Coste mat bio
		
		listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.costa);
		//Vacas: habitats ->
		herbivoro3 = new EspecieAnimal("Vaca",10,200,2400,800,4,4,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro3);
		costeHerb3 = new List<int>();
		costeHerb3.Add(2200);//Coste energia
		costeHerb3.Add(450);//Coste comp bas
		costeHerb3.Add(30);//Coste comp adv
		costeHerb3.Add(25);//Coste mat bio
		
		listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.desierto);
		listaHabs.Add(T_habitats.llanura);
		//Jirafas: habitats -> 
		herbivoro4 = new EspecieAnimal("Jirafa",12,240,3360,960,6,4,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro4);
		costeHerb4 = new List<int>();
		costeHerb4.Add(3200);//Coste energia
		costeHerb4.Add(570);//Coste comp bas
		costeHerb4.Add(100);//Coste comp adv
		costeHerb4.Add(35);//Coste mat bio
		
		listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.volcanico);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.montana);
		//Tortuga: habitats-> 
		herbivoro5 = new EspecieAnimal("Tortuga",6,400,7500,2500,4,5,tipoAlimentacionAnimal.herbivoro,listaHabs,modelosAnimales.herbivoro5);
		costeHerb5 = new List<int>();
		costeHerb5.Add(4000);//Coste energia
		costeHerb5.Add(750);//Coste comp bas
		costeHerb5.Add(200);//Coste comp adv
		costeHerb5.Add(50);//Coste mat bio
		
		/* Carnivoros */
		
		//Rata: habitats ->
		listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.tundra);
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		carnivoro1 = new EspecieAnimal("Zorro",4,80,2400,2400,7,8,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro1);
		costeCarn1 = new List<int>();
		costeCarn1.Add(2000);//Coste energia
		costeCarn1.Add(350);//Coste comp bas
		costeCarn1.Add(20);//Coste comp adv
		costeCarn1.Add(15);//Coste mat bio
		
		//Lobo: habitats -> 
		listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Add(T_habitats.tundra);
		carnivoro2 = new EspecieAnimal("Lobo",3,120,3400,3400,10,10,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro2);
		costeCarn2 = new List<int>();
		costeCarn2.Add(3300);//Coste energia
		costeCarn2.Add(450);//Coste comp bas
		costeCarn2.Add(80);//Coste comp adv
		costeCarn2.Add(25);//Coste mat bio
		
		//Tigre: habitats -> 
		listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.costa);
		listaHabs.Add(T_habitats.montana);
		carnivoro3 = new EspecieAnimal("Tigre",3,192,4800,4800,12,12,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro3);
		costeCarn3 = new List<int>();
		costeCarn3.Add(4500);//Coste energia
		costeCarn3.Add(750);//Coste comp bas
		costeCarn3.Add(250);//Coste comp adv
		costeCarn3.Add(40);//Coste mat bio
		
		//Oso: habitats ->
		listaHabs = new List<T_habitats>();
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.montana);
		listaHabs.Add(T_habitats.tundra);
		carnivoro4 = new EspecieAnimal("Oso",2,224,6720,6720,11,14,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro4);
		costeCarn4 = new List<int>();
		costeCarn4.Add(5600);//Coste energia
		costeCarn4.Add(830);//Coste comp bas
		costeCarn4.Add(320);//Coste comp adv
		costeCarn4.Add(60);//Coste mat bio
		
		//Tiranosaurio: habitats ->
		listaHabs.Add(T_habitats.llanura);
		listaHabs.Add(T_habitats.colina);
		listaHabs.Add(T_habitats.desierto);
		listaHabs.Add(T_habitats.volcanico);
		carnivoro5 = new EspecieAnimal("Tiranosaurio",1,375,15000,7500,15,15,tipoAlimentacionAnimal.carnivoro,listaHabs,modelosAnimales.carnivoro5);
		costeCarn5 = new List<int>();
		costeCarn5.Add(7500);//Coste energia
		costeCarn5.Add(1200);//Coste comp bas
		costeCarn5.Add(430);//Coste comp adv
		costeCarn5.Add(90);//Coste mat bio
		
	}
	
	void Start () {		
		//NO CAMBIAR EL ORDEN CON EL QUE SE AÑADEN
		//Añadir las descripciones
		descripciones.Add(descripcionSeta);
		descripciones.Add(descripcionFlor);
		descripciones.Add(descripcionCana);
		descripciones.Add(descripcionArbusto);
		descripciones.Add(descripcionEstromatolito);
		descripciones.Add(descripcionCactus);
		descripciones.Add(descripcionPalmera);
		descripciones.Add(descripcionPino);
		descripciones.Add(descripcionCipres);
		descripciones.Add(descripcionPinoAlto);
		descripciones.Add(descripcionHerb1);
		descripciones.Add(descripcionHerb2);
		descripciones.Add(descripcionHerb3);
		descripciones.Add(descripcionHerb4);
		descripciones.Add(descripcionHerb5);
		descripciones.Add(descripcionCarn1);
		descripciones.Add(descripcionCarn2);
		descripciones.Add(descripcionCarn3);
		descripciones.Add(descripcionCarn4);
		descripciones.Add(descripcionCarn5);
		descripciones.Add(descripcionFabBas);
		descripciones.Add(descripcionEnergia1);
		descripciones.Add(descripcionGranja);
		descripciones.Add(descripcionFabAdv);
		descripciones.Add(descripcionEnergia2);
		
		//Añadir los costes
		costesSeres.Add(costeSeta);
		costesSeres.Add(costeFlor);
		costesSeres.Add(costeCana);
		costesSeres.Add(costeArbusto);
		costesSeres.Add(costeEstromatolito);
		costesSeres.Add(costeCactus);
		costesSeres.Add(costePalmera);
		costesSeres.Add(costePino);
		costesSeres.Add(costeCipres);
		costesSeres.Add(costePinoAlto);
		costesSeres.Add(costeHerb1);
		costesSeres.Add(costeHerb2);
		costesSeres.Add(costeHerb3);
		costesSeres.Add(costeHerb4);
		costesSeres.Add(costeHerb5);
		costesSeres.Add(costeCarn1);
		costesSeres.Add(costeCarn2);
		costesSeres.Add(costeCarn3);
		costesSeres.Add(costeCarn4);
		costesSeres.Add(costeCarn5);		
		anadeElementosVida();
		principal.completarCarga();
	}
	
	public void anadeElementosVida()
	{
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
	
	public string getDescripcion(int entrada) {
		if (entrada >= 0 && entrada < descripciones.Count)
			return descripciones[entrada];
		return "";
	}
	
	public List<int> getCostes(int entrada) {
		if (entrada >= 0 && entrada < costesSeres.Count)
			return costesSeres[entrada];
		return null;
	}
	
	public int getNumeroSer(EspecieAnimal entrada) {
		return entrada.idEspecie + 10;
	}
	
	public int getNumeroSer(EspecieVegetal entrada) {
		return entrada.idEspecie;
	}
	
	public int getNumeroSer(TipoEdificio entrada) {
		return entrada.idTipoEdificio + 20;
	}	
}
