using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using ;

public class Principal : MonoBehaviour {

	//Variables ---------------------------------------------------------------------------------------------------------------------------
	
	//Camaras, texturas y sonido
	public GameObject camaraPrincipal;									//Para mostrar el mundo completo (menos escenas especiales)
	public GameObject objetoOceano;										//El objeto que representa la esfera del oceano
	public GameObject objetoRoca;										//El objeto que representa la esfera de la roca
	public GameObject objetoPlanta;										//El objeto que representa la esfera de las plantas
	public Texture2D texPlantas;										//La textura donde se pintan las plantas 
	public GameObject sonidoAmbiente;									//El objeto que va a contener la fuente del audio de ambiente
	public GameObject sonidoFX;											//El objeto que va a contener la fuente de efectos de audio
	private GameObject contenedorTexturas;								//El contenedor de las texturas de la primera escena
	
	//Recursos
	public int energia = 100;											//Cantidad de energia almacenada en la nave
	public int energiaDif = 10;											//Incremento o decremento por turno de energia
	public int componentesBasicos = 25;									//Cantidad de componentes basicos alojados en la nave
	public int componentesBasicosDif = 0;								//Incremento o decremento por turno de componentes basicos
	public int componentesAvanzados = 0;								//Cantidad de componentes avanzados alojados en la nave
	public int componentesAvanzadosDif = 0;								//Incremento o decremento por turno de componentes avanzados
	public int materialBiologico = 0;									//Cantidad de material biologico alojado en la nave
	public int materialBiologicoDif = 0;								//Incremento o decremento por turno de material biologico
	
	
	//Algoritmo vida
	public Vida vida;													//Tablero lógico del algoritmo		
	private float tiempoPaso					= 0.0f;					//El tiempo que lleva el paso actual del algoritmo
	public int numPasos							= 0;					//Numero de pasos del algoritmo ejecutados
	private bool algoritmoActivado				= false;				//Se encuentra activado el algoritmo de la vida?
	
	//Escala de tiempo
	public float escalaTiempo					= 1.0f;					//La escala temporal a la que se updateará todo
	
	//Menus para guardar
	private Vector2 posicionScroll 				= Vector2.zero;			//La posicion en la que se encuentra la ventana con scroll
	private int numSaves 						= 0;					//El numero de saves diferentes que hay en el directorio respectivo
	private int numSavesExtra 					= 0;					//Numero de saves que hay que no se ven al primer vistazo en la scrollview
	private string[] nombresSaves;										//Los nombres de los ficheros de savegames guardados

	//Tipos especiales ----------------------------------------------------------------------------------------------------------------------
	
	
	//Update y transiciones de estados -------------------------------------------------------------------------------------------------------
	
	void Awake() {		
		Random.seed = System.DateTime.Now.Millisecond;
		contenedorTexturas = GameObject.FindGameObjectWithTag("Carga");
		if (contenedorTexturas == null) {
			creacionInicial();
		}
		else {
			//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez			
			Texture2D texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			ValoresCarga temp = contenedorTexturas.GetComponent<ValoresCarga>();
			texturaBase = temp.texturaBase;
			texturaBase.Apply();
			objetoOceano.transform.localScale = temp.escalaOceano;
		}
		Audio_Ambience ambiente = sonidoAmbiente.GetComponent<Audio_Ambience>();
		Audio_SoundFX efectos = sonidoFX.GetComponent<Audio_SoundFX>();
		if (PlayerPrefs.GetInt("MusicaOn") == 1)
			ambiente.activado = true;
		else
			ambiente.activado = false;
		ambiente.volumen = PlayerPrefs.GetFloat("MusicaVol");
		if (PlayerPrefs.GetInt("SfxOn") == 1)
			efectos.activado = true;
		else
			efectos.activado = false;
		efectos.volumen = PlayerPrefs.GetFloat("SfxVol");
		
		Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		Mesh mesh = objetoRoca.GetComponent<MeshFilter>().sharedMesh;
		Casilla[,] tablero = FuncTablero.iniciaTablero(tex, mesh);
		vida = new Vida(tablero, texPlantas, objetoRoca.transform);				
		
		numSaves = SaveLoad.FileCount();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames();
		numSavesExtra = numSaves - 3;
		if (numSavesExtra < 0)
			numSavesExtra = 0;	
	}
	
	void Start()
	{
		creacionEspeciesEdificios();
	}
	
	void FixedUpdate() {
		//Algoritmo de vida		
		tiempoPaso += Time.deltaTime;		
		if(algoritmoActivado && tiempoPaso > 1.0f) 		//El 1.0f significa que se ejecuta un paso cada 1.0 segundos, cuando la escala temporal esta a 1.0
		{		
			vida.algoritmoVida();
			numPasos++;
			tiempoPaso = 0.0f;
		}
	}
	
	void Update () {
		Time.timeScale = escalaTiempo;
	}
	
	public void setEscalaTiempo(float nuevaEscala)
	{
		escalaTiempo = nuevaEscala;
	}

	private void creacionInicial() {
		Debug.Log("Creando planeta de cero en creacionInicial");
		//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
		Texture2D texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		Texture2D texturaAgua = objetoOceano.renderer.sharedMaterial.mainTexture as Texture2D;
		
		Color[] pixels = new Color[texturaBase.width * texturaBase.height];
		FuncTablero.inicializa(texturaBase);
		
		pixels = FuncTablero.ruidoTextura();											//Se crea el ruido para la textura base y normales...
		pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);			//Se suaviza el borde lateral...
		pixels = FuncTablero.suavizaPoloTex(pixels, texturaBase.height / 20);			//Se suavizan los polos...
		
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();		
		
		pixels = FuncTablero.calculaTexAgua(pixels);
		texturaAgua.SetPixels(pixels);
		texturaAgua.Apply();
		
		MeshFilter filter = objetoRoca.GetComponent<MeshFilter>();
		MeshFilter filter2 = objetoPlanta.GetComponent<MeshFilter>();
		Mesh meshTemp = filter.mesh;
		meshTemp = FuncTablero.extruyeVertices(meshTemp, texturaBase, 0.5f, objetoRoca.transform.position);
		filter.mesh = meshTemp;
		filter2.mesh = meshTemp;
		//Se añade el collider aqui, para que directamente tenga la mesh adecuada
       	objetoRoca.AddComponent<MeshCollider>();
        objetoRoca.GetComponent<MeshCollider>().sharedMesh = meshTemp;
	}
		
	void creacionEspeciesEdificios()
	{		
		List<T_habitats> habitats1 = new List<T_habitats>();
		habitats1.Add(T_habitats.plain);
		habitats1.Add(T_habitats.sand);
		habitats1.Add(T_habitats.coast);
		habitats1.Add(T_habitats.mountain);
		habitats1.Add(T_habitats.hill);
		habitats1.Add(T_habitats.sea);
		habitats1.Add(T_habitats.volcanic);

		ModelosEdificios modelosEdificios = GameObject.FindGameObjectWithTag("ModelosEdificios").GetComponent<ModelosEdificios>();		
		ModelosVegetales modelosVegetales = GameObject.FindGameObjectWithTag("ModelosVegetales").GetComponent<ModelosVegetales>();		
		ModelosAnimales modelosAnimales = GameObject.FindGameObjectWithTag("ModelosAnimales").GetComponent<ModelosAnimales>();		
				
		TipoEdificio tipoEdif1 = new TipoEdificio("Fábrica componentes básicos",habitats1,modelosEdificios.fabCompBas);
		vida.anadeTipoEdificio(tipoEdif1);
		TipoEdificio tipoEdif2 = new TipoEdificio("Central de energía",habitats1,modelosEdificios.centralEnergia);
		vida.anadeTipoEdificio(tipoEdif2);
		TipoEdificio tipoEdif3 = new TipoEdificio("Granja",habitats1,modelosEdificios.granja);
		vida.anadeTipoEdificio(tipoEdif3);
		TipoEdificio tipoEdif4 = new TipoEdificio("Fábrica de componentes avanzados",habitats1,modelosEdificios.fabCompAdv);
		vida.anadeTipoEdificio(tipoEdif4);
		TipoEdificio tipoEdif5 = new TipoEdificio("Central de energía avanzada",habitats1,modelosEdificios.centralEnergiaAdv);
		vida.anadeTipoEdificio(tipoEdif5);
		
		/* Vegetales */
		EspecieVegetal especieV1 = new EspecieVegetal("Seta",1000,50,50,50,0.1f,8,habitats1,0,modelosVegetales.setas);
		vida.anadeEspecieVegetal(especieV1);
		EspecieVegetal especieV2 = new EspecieVegetal("Flor",1000,50,50,20,0.1f,15,habitats1,1,modelosVegetales.flores);
		vida.anadeEspecieVegetal(especieV2);
		EspecieVegetal especieV3 = new EspecieVegetal("Caña",1000,50,50,20,0.1f,12,habitats1,2,modelosVegetales.canas);
		vida.anadeEspecieVegetal(especieV3);
		EspecieVegetal especieV4 = new EspecieVegetal("Arbusto",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.arbustos);
		vida.anadeEspecieVegetal(especieV4);
		EspecieVegetal especieV5 = new EspecieVegetal("Estromatolito",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.estromatolitos);
		vida.anadeEspecieVegetal(especieV5);
		EspecieVegetal especieV6 = new EspecieVegetal("Cactus",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.cactus);
		vida.anadeEspecieVegetal(especieV6);
		EspecieVegetal especieV7 = new EspecieVegetal("Palmera",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.palmeras);
		vida.anadeEspecieVegetal(especieV7);
		EspecieVegetal especieV8 = new EspecieVegetal("Pino",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.pinos);
		vida.anadeEspecieVegetal(especieV8);
		EspecieVegetal especieV9 = new EspecieVegetal("Ciprés",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.cipreses);
		vida.anadeEspecieVegetal(especieV9);
		EspecieVegetal especieV10 = new EspecieVegetal("Pino Alto",1000,50,50,20,0.1f,12,habitats1,3,modelosVegetales.pinosAltos);
		vida.anadeEspecieVegetal(especieV10);
		
		/* Herbivoros */
		EspecieAnimal especieH1 = new EspecieAnimal("herbivoro1",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,modelosAnimales.herbivoro1);
		vida.anadeEspecieAnimal(especieH1);
		EspecieAnimal especieH2 = new EspecieAnimal("herbivoro2",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,modelosAnimales.herbivoro2);
		vida.anadeEspecieAnimal(especieH2);
		EspecieAnimal especieH3 = new EspecieAnimal("herbivoro3",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,modelosAnimales.herbivoro3);
		vida.anadeEspecieAnimal(especieH3);
		EspecieAnimal especieH4 = new EspecieAnimal("herbivoro4",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,habitats1,modelosAnimales.herbivoro4);
		vida.anadeEspecieAnimal(especieH4);
		EspecieAnimal especieH5 = new EspecieAnimal("herbivoro5",10,100,100,5,5,1,tipoAlimentacionAnimal.herbivoro,T_habitats.hill,modelosAnimales.herbivoro5);
		vida.anadeEspecieAnimal(especieH5);
		
		/* Carnivoros */
		EspecieAnimal especieC1 = new EspecieAnimal("carnivoro1",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,modelosAnimales.carnivoro1);
		vida.anadeEspecieAnimal(especieC1);
		EspecieAnimal especieC2 = new EspecieAnimal("carnivoro2",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,modelosAnimales.carnivoro2);
		vida.anadeEspecieAnimal(especieC2);
		EspecieAnimal especieC3 = new EspecieAnimal("carnivoro3",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,modelosAnimales.carnivoro3);
		vida.anadeEspecieAnimal(especieC3);
		EspecieAnimal especieC4 = new EspecieAnimal("carnivoro4",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,habitats1,modelosAnimales.carnivoro4);
		vida.anadeEspecieAnimal(especieC4);
		EspecieAnimal especieC5 = new EspecieAnimal("carnivoro5",10,100,100,5,5,1,tipoAlimentacionAnimal.carnivoro,T_habitats.hill,modelosAnimales.carnivoro5);
		vida.anadeEspecieAnimal(especieC5);	
	}	
}
