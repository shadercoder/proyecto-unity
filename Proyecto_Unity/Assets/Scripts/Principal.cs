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
		//obtener la textura de habitats del array de materiales de roca. Habitats esta en la 2ª posicion.
		Texture2D texElems = objetoRoca.renderer.sharedMaterials[4].mainTexture as Texture2D;
		Texture2D texHabitats = objetoRoca.renderer.sharedMaterials[2].mainTexture as Texture2D;
		Texture2D texHabitatsEstetica = objetoRoca.renderer.sharedMaterials[1].mainTexture as Texture2D;
		Mesh mesh = objetoRoca.GetComponent<MeshFilter>().sharedMesh;
		Casilla[,] tablero = FuncTablero.iniciaTablero(tex, texHabitats, texHabitatsEstetica, texElems, mesh);
		vida = new Vida(tablero, texPlantas, objetoRoca.transform);				
		
		numSaves = SaveLoad.FileCount();
		nombresSaves = new string[numSaves];
		nombresSaves = SaveLoad.getFileNames();
		numSavesExtra = numSaves - 3;
		if (numSavesExtra < 0)
			numSavesExtra = 0;	
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
		if(Input.GetKeyDown(KeyCode.V)) 
            if(algoritmoActivado)
                    algoritmoActivado = false;
            else
                    algoritmoActivado = true;
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
		pixels = FuncTablero.suavizaPoloTex(pixels);									//Se suavizan los polos...
		
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();		
		
		pixels = FuncTablero.calculaTexAgua(pixels);
		texturaAgua.SetPixels(pixels);
		texturaAgua.Apply();
		
		MeshFilter Roca = objetoRoca.GetComponent<MeshFilter>();
		Mesh meshTemp = Roca.mesh;
		meshTemp = FuncTablero.extruyeVertices(meshTemp, texturaBase, 0.5f, objetoRoca.transform.position);
		Roca.mesh = meshTemp;
		//Se añade el collider aqui, para que directamente tenga la mesh adecuada
       	objetoRoca.AddComponent<MeshCollider>();
        objetoRoca.GetComponent<MeshCollider>().sharedMesh = meshTemp;
	}
	
	//Devuelve  true si se ha producido una colision con el planeta y además las coordenadas de la casilla del tablero en la que ha impactado el raycast (en caso de producirse)
	public bool raycastRoca(Vector3 posicion,ref int x,ref int y)
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(posicion);
		if (objetoRoca.collider.Raycast(ray, out hit, Mathf.Infinity)) 
		{
			double xTemp = hit.textureCoord.x;
			double yTemp = hit.textureCoord.y;
			Texture2D tex = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
			xTemp = xTemp * tex.width/ FuncTablero.getRelTexTabAncho();
			yTemp = (yTemp * tex.height/ FuncTablero.getRelTexTabAlto());
			x = (int)xTemp;
			y = (int)yTemp;
			FuncTablero.convierteCoordenadas(ref y, ref x);
			return true;
		}	
		else 
			return false;		
	}
	
	//Estos 3 métodos se usan desde TiposSeres para introducir los edificios, vegetales y animales respectivamente.
	//Así está mas organizado este script.
	public void anadeTipoEdificio(TipoEdificio edif) {
		vida.anadeTipoEdificio(edif);
	}
	
	public void anadeEspecieVegetal(EspecieVegetal vegetal) {
		vida.anadeEspecieVegetal(vegetal);
	}
	
	public void anadeEspecieAnimal(EspecieAnimal animal) {
		vida.anadeEspecieAnimal(animal);
	}
}
