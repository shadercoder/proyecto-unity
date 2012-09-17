using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using ;

public class Principal : MonoBehaviour {

	//Variables ---------------------------------------------------------------------------------------------------------------------------
	private InterfazPrincipal interfaz;
	private MejorasNave mejoras;
	private float escalaTiempoAntesMenu;
	//Trucos
	public bool developerMode					= true;					//Mientras este a true, maximo de recursos en todo momento
	
	//Camaras, texturas y sonido
	public GameObject camaraPrincipal;									//Para mostrar el mundo completo (menos escenas especiales)
	public GameObject objetoOceano;										//El objeto que representa la esfera del oceano
	public GameObject objetoRoca;										//El objeto que representa la esfera de la roca
	private GameObject contenedor;										//El contenedor de las texturas de la primera escena
		
	//Recursos
	public int energiaProducidaNave = 5;								//Energia que produce la nave mediante sus paneles solares (inicialmente 5)
	public int energia = 1000;											//Cantidad de energia almacenada en la nave
	public int energiaDif = 10;											//Incremento o decremento por turno de energia
	public int componentesBasicos = 25;									//Cantidad de componentes basicos alojados en la nave
	public int componentesBasicosDif = 0;								//Incremento o decremento por turno de componentes basicos
	public int componentesAvanzados = 0;								//Cantidad de componentes avanzados alojados en la nave
	public int componentesAvanzadosDif = 0;								//Incremento o decremento por turno de componentes avanzados
	public int materialBiologico = 0;									//Cantidad de material biologico alojado en la nave
	public int materialBiologicoDif = 0;								//Incremento o decremento por turno de material biologico
	
	public int energiaMax = 2000;										//Energía máxima que se puede almacenar
	public int componentesBasicosMax = 250;								//Componentes básicos máximos que se pueden almacenar
	public int componentesAvanzadosMax = 100;							//Componentes avanzados máximos que se pueden almacenar
	public int materialBiologicoMax = 50;								//Material biológico máximo que se puede almacenar
	
	//Algoritmo vida
	public Vida vida;													//Tablero lógico del algoritmo		
	private float tiempoPaso					= 0.0f;					//El tiempo que lleva el paso actual del algoritmo
	public int numPasos							= 0;					//Numero de pasos del algoritmo ejecutados
	private bool algoritmoActivado				= true;					//Se encuentra activado el algoritmo de la vida?
	private bool algoritmoPasoAPaso 			= false;
	private const float tiempoTurno				= 3.0f;					//El tiempo que dura un turno del algoritmo
		
	//Escala de tiempo
	public float escalaTiempo					= 1.0f;					//La escala temporal a la que se updateará todo
	
	//Guardar y cargar
	private bool hechaCarga						= false;				//Indica si se ha llevado a cabo una carga
	
	//Update y transiciones de estados -------------------------------------------------------------------------------------------------------
	
	void Awake() {		
		Random.seed = System.DateTime.Now.Millisecond;
		UnityEngine.Random.seed = System.DateTime.Now.Millisecond;
		Debug.Log (FuncTablero.formateaTiempo() + ": Iniciando el script Principal...");
		//Se busca el objeto con los valores guardados en la escena inicial, si lo hubiera
		contenedor = GameObject.FindGameObjectWithTag("Carga");
		if (contenedor == null) {		//Si el objeto no existe, crear el planeta de cero
			Debug.Log (FuncTablero.formateaTiempo() + ": No encontrado contenedor, iniciando creacion inicial...");
			creacionInicial();
			contenedor = new GameObject("Contenedor");
			contenedor.tag = "Carga";
			contenedor.AddComponent<ValoresCarga>();
		}
		else {							//Si el objeto existe, cargar los valores necesarios
			Debug.Log (FuncTablero.formateaTiempo() + ": Encontrado contenedor, cargando...");
			ValoresCarga cont = contenedor.GetComponent<ValoresCarga>();
			creacionCarga(cont);
			hechaCarga = true;
		}
		Debug.Log (FuncTablero.formateaTiempo() + ": Completada la creacion del planeta.");
		mejoras = GameObject.FindGameObjectWithTag("Mejoras").GetComponent<MejorasNave>();
		interfaz = gameObject.GetComponent<InterfazPrincipal>();
	}
	
	void Start()
	{
		
	}
	
	void FixedUpdate() {
		//Algoritmo de vida		
		tiempoPaso += Time.deltaTime;		
		if(algoritmoActivado && tiempoPaso > tiempoTurno)
		{		
			actualizaRecursos();			
			int energiaEdificios = 0,compBasEdificios = 0,compAvzEdificios = 0,matBioEdificios = 0;
			vida.algoritmoVida(numPasos,ref energiaEdificios,ref compBasEdificios,ref compAvzEdificios,ref matBioEdificios);
			calculaDifRecursosSigTurno(energiaEdificios,compBasEdificios,compAvzEdificios,matBioEdificios);
			numPasos++;
			tiempoPaso = 0.0f;
			if(algoritmoPasoAPaso)
				algoritmoActivado = false;
		}
		if (developerMode) {
			setDeveloperMode();
		}
	}
	
	void Update () {
		Time.timeScale = escalaTiempo;
		if(Input.GetKeyDown(KeyCode.V)) 
			if(algoritmoActivado)
 				algoritmoActivado = false;
            else
				algoritmoActivado = true;
		if(Input.GetKeyDown(KeyCode.B)) 
			if(algoritmoPasoAPaso)
 				algoritmoPasoAPaso = false;
            else
				algoritmoPasoAPaso = true;	
		if(Input.GetKeyDown(KeyCode.Alpha1)) 
			setEscalaTiempo(0.0f);	
		if(Input.GetKeyDown(KeyCode.Alpha2)) 
			setEscalaTiempo(1.0f);	
		if(Input.GetKeyDown(KeyCode.Alpha3)) 
			setEscalaTiempo(2.0f);	
		if(Input.GetKeyDown(KeyCode.Alpha4)) 
			setEscalaTiempo(5.0f);	
		if(Input.GetKeyDown(KeyCode.Escape)) 
		{
			if(interfaz.accion == InterfazPrincipal.taccion.mostrarMenu)
			{
				if(interfaz.accionMenu == InterfazPrincipal.taccionMenu.mostrarMenu)
				{
					interfaz.accion = InterfazPrincipal.taccion.ninguna;
					escalaTiempo = escalaTiempoAntesMenu;
				}		
				else
					interfaz.accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
			}
			else
			{
				interfaz.accion = InterfazPrincipal.taccion.mostrarMenu;
				interfaz.accionMenu = InterfazPrincipal.taccionMenu.mostrarMenu;
				escalaTiempoAntesMenu = escalaTiempo;
				escalaTiempo = 0;
			}			
		}
		
		//Velocidad extra rapida para debug
		if(Input.GetKeyDown(KeyCode.Alpha5) && developerMode) 
			setEscalaTiempo(50.0f);
		
		//Activar/desactivar developer mode (maximos recursos)
		if (Input.GetKeyDown(KeyCode.G)) {
			if (developerMode)
				developerMode = false;
			else
				developerMode = true;
		}
	}
	
	public void setEscalaTiempo(float nuevaEscala)
	{
		escalaTiempo = nuevaEscala;
	}

	private void creacionInicial() {
		Debug.Log(FuncTablero.formateaTiempo() + ": Iniciando la creacion desde cero...");
		//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
		Texture2D texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		
		Color[] pixels = new Color[texturaBase.width * texturaBase.height];
		FuncTablero.inicializa(texturaBase);
		Debug.Log (FuncTablero.formateaTiempo() + ": Creando ruido...");
		pixels = FuncTablero.ruidoTextura();											//Se crea el ruido para la textura base y normales...
		Debug.Log(FuncTablero.formateaTiempo() + ": Completado. Suavizando polos y bordes...");
		pixels = FuncTablero.suavizaBordeTex(pixels, texturaBase.width / 20);			//Se suaviza el borde lateral...
		pixels = FuncTablero.suavizaPoloTex(pixels);									//Se suavizan los polos...
		Debug.Log(FuncTablero.formateaTiempo() + ": Completado. Aplicando cambios...");
		texturaBase.SetPixels(pixels);
		texturaBase.Apply();
		Debug.Log(FuncTablero.formateaTiempo() + ": Completado. Extruyendo vertices de la roca...");
		float extrusion = 0.45f;
		MeshFilter Roca = objetoRoca.GetComponent<MeshFilter>();
		Mesh meshTemp = Roca.mesh;
		meshTemp = FuncTablero.extruyeVerticesTex(meshTemp, texturaBase, extrusion, objetoRoca.transform.position);
		Roca.mesh = meshTemp;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completado. Construyendo collider...");
		//Se añade el collider aqui, para que directamente tenga la mesh adecuada
       	objetoRoca.AddComponent<MeshCollider>();
        objetoRoca.GetComponent<MeshCollider>().sharedMesh = meshTemp;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completado. Calculando y extruyendo vertices del oceano...");
		MeshFilter Agua = objetoOceano.GetComponent<MeshFilter>();
		Mesh meshAgua = Agua.mesh;
		meshAgua = FuncTablero.extruyeVerticesValor(meshAgua, FuncTablero.getNivelAgua(), extrusion, objetoOceano.transform.position);
		Agua.mesh = meshAgua;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completado. Rellenando detalles...");
		//se ajusta la propiedad de nivel de agua del shader
		objetoOceano.renderer.sharedMaterial.SetFloat("_nivelMar", FuncTablero.getNivelAgua());
		objetoOceano.renderer.sharedMaterial.SetFloat("_tamPlaya", FuncTablero.getTamanoPlaya());
		
		Debug.Log (FuncTablero.formateaTiempo() + ": Terminado. Cargando texturas de habitats...");
		//obtener la textura de habitats del array de materiales de roca. Habitats esta en la 1ª posicion.
		Texture2D texElems = objetoRoca.renderer.sharedMaterials[3].mainTexture as Texture2D;
		Texture2D texPlantas = objetoRoca.renderer.sharedMaterials[2].mainTexture as Texture2D;
		Texture2D texHabitatsEstetica = objetoRoca.renderer.sharedMaterials[1].mainTexture as Texture2D;
		Texture2D texHabitats = objetoRoca.renderer.sharedMaterials[1].GetTexture("_FiltroTex") as Texture2D;
		Debug.Log (FuncTablero.formateaTiempo() + ": Terminado. Creando el tablero...");
		Casilla[,] tablero = FuncTablero.iniciaTablero(texturaBase, texHabitats, texHabitatsEstetica, texElems, Roca.mesh, objetoRoca.transform.position);
		Debug.Log (FuncTablero.formateaTiempo() + ": Terminado. Creando Vida...");
		vida = new Vida(tablero, texPlantas, objetoRoca.transform);				
		Debug.Log (FuncTablero.formateaTiempo() + ": Completada la creacion del planeta.");
	}
	
	private void creacionCarga(ValoresCarga contenedor) {
		Debug.Log (FuncTablero.formateaTiempo() + ": Iniciando el script de carga de valores...");
		Texture2D texBase = contenedor.texturaBase;
		FuncTablero.inicializa(texBase);
		Mesh rocaMesh = contenedor.roca;
		Mesh aguaMesh = contenedor.agua;
		float nivelAgua = contenedor.nivelAgua;
		float tamanoPlaya = contenedor.tamanoPlaya;
		//Trabajar con la textura Textura_Planeta y crear el mapa lógico a la vez
		Debug.Log (FuncTablero.formateaTiempo() + ": Aplicando textura de ruido...");
		Texture2D texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		texturaBase = texBase;
		texturaBase.Apply();		
		Debug.Log (FuncTablero.formateaTiempo() + ": Asignando Mesh a la roca...");
		MeshFilter Roca = objetoRoca.GetComponent<MeshFilter>();
		Roca.mesh = rocaMesh;
		//Se añade el collider aqui, para que directamente tenga la mesh adecuada
		Debug.Log (FuncTablero.formateaTiempo() + ": Creando collider de la roca...");
       	objetoRoca.AddComponent<MeshCollider>();
        objetoRoca.GetComponent<MeshCollider>().sharedMesh = rocaMesh;
		Debug.Log (FuncTablero.formateaTiempo() + ": Asignando Mesh al oceano...");
		MeshFilter Agua = objetoOceano.GetComponent<MeshFilter>();
		Agua.mesh = aguaMesh;
		Debug.Log (FuncTablero.formateaTiempo() + ": Completando detalles...");
		//se ajusta la propiedad de nivel de agua del shader
		objetoOceano.renderer.sharedMaterial.SetFloat("_nivelMar", nivelAgua);
		objetoOceano.renderer.sharedMaterial.SetFloat("_tamPlaya", tamanoPlaya);
		
		Debug.Log (FuncTablero.formateaTiempo() + ": Cargando texturas de habitats...");
		//obtener la textura de habitats del array de materiales de roca. Habitats esta en la 1ª posicion.
		objetoRoca.renderer.sharedMaterials[3].mainTexture = contenedor.texturaElementos;
		objetoRoca.renderer.sharedMaterials[2].mainTexture = contenedor.texturaPlantas;
		objetoRoca.renderer.sharedMaterials[1].mainTexture = contenedor.texturaHabsEstetica;
		Texture2D texHabitats = objetoRoca.renderer.sharedMaterials[1].GetTexture("_FiltroTex") as Texture2D;
		texHabitats = contenedor.texturaHabitats;
		objetoRoca.renderer.sharedMaterials[1].SetTexture("_FiltroTex", texHabitats);
		Debug.Log (FuncTablero.formateaTiempo() + ": Cargando la vida...");
		vida = new Vida(contenedor.vida);
		vida.setObjetoRoca(objetoRoca.transform);
		Debug.Log (FuncTablero.formateaTiempo() + ": Carga completada.");
	}
	
	//Devuelve  true si se ha producido una colision con el planeta y además las coordenadas de la casilla del tablero en la que ha impactado el raycast (en caso de producirse)
	public bool raycastRoca(Vector3 posicion,ref int x,ref int y,out RaycastHit hit)
	{
		//RaycastHit hit;
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
	
	//Devuelve true si es posible consumir los recursos pedidos y false si no hay suficiente de alguno de ellos
	public bool recursosSuficientes(int energiaAconsumir,int componentesBasAconsumir,int componentesAvzAconsumir,int materialAconsumir)
	{
		if(energia >= energiaAconsumir && componentesBasicos >= componentesBasAconsumir && componentesAvanzados >= componentesAvzAconsumir && 
		   materialBiologico >= materialAconsumir)		
			return true;		
		else
			return false;		
	}
	
	//Consume la cantidad de recursos que hay en los parametros. Se debe utilizar siempre antes el metodo recursosSuficientes para comprobar si los hay, y este cuando la inserción se haya completado
	public void consumeRecursos(int energiaAconsumir,int componentesBasAconsumir,int componentesAvzAconsumir,int materialAconsumir)
	{
		energia -= energiaAconsumir;
		componentesBasicos -= componentesBasAconsumir;
		componentesAvanzados -= componentesAvzAconsumir;
		materialBiologico -= materialAconsumir;
	}
	
	//Devuelve true si es posible consumir la energía pedida y false si no hay suficiente
	public bool consumeEnergia(int energiaAconsumir)
	{
		if(energia >= energiaAconsumir)
		{
			energia -= energiaAconsumir;
			return true;
		}
		else
			return false;
	}
	
	//Devuelve true si es posible consumir los componentes básicos pedidos y false si no hay suficientes
	public bool consumeComponentesBasicos(int componentesAconsumir)
	{
		if(componentesBasicos >= componentesAconsumir)
		{
			componentesBasicos -= componentesAconsumir;
			return true;
		}
		else
			return false;
	}

	//Devuelve true si es posible consumir los componentes avanzados pedidos y false si no hay suficientes
	public bool consumeComponentesAvanzados(int componentesAconsumir)
	{
		if(componentesAvanzados >= componentesAconsumir)
		{
			componentesAvanzados -= componentesAconsumir;
			return true;
		}
		else
			return false;
	}
	
	//Devuelve true si es posible consumir los componentes básicos pedidos y false si no hay suficientes	
	public bool consumeMaterialBiologico(int materialAconsumir)
	{
		if(materialBiologico >= materialAconsumir)
		{
			materialBiologico -= materialAconsumir;
			return true;
		}
		else
			return false;
	}
	
	/*//Modifica la cantidad de cada recurso que se consume por turno
	public void modificaRecursosPorTurno(int energiaPorTurno,int componentesBasPorTurno,int componentesAvzPorTurno,int materialPorTurno)
	{
		energiaDif += energiaPorTurno;
		componentesBasicosDif += componentesBasPorTurno;
		componentesAvanzadosDif += componentesAvzPorTurno;
		materialBiologicoDif += materialPorTurno;
	}
	
	//Modifica la cantidad de energia que se consume por turno
	public void modificaEnergiaPorTurno(int energiaPorTurno)
	{
		energiaDif += energiaPorTurno;
	}
	
	//Modifica la cantidad de componentes básicos que se consumen por turno
	public void modificaComponentesBasicosPorTurno(int componentesPorTurno)
	{
	}
	//Modifica la cantidad de componentes avanzados que se consumen por turno
	public void modificaComponentesAvanzadosPorTurno(int componentesPorTurno)
	{
		componentesAvanzadosDif += componentesPorTurno;
	}
	
	//Modifica la cantidad de material biológico que se consume por turno
	public void modificaMaterialBiologicoPorTurno(int materialPorTurno)
	{
		materialBiologicoDif += materialPorTurno;
	}
	*/	
		
	//Actualiza los recursos sumando o restando
	public void calculaDifRecursosSigTurno(int energiaEdificios,int compBasEdificios,int compAvzEdificios,int matBioEdificios)
	{		
		/*calculo de costes y producciones de habilidades y mejoras*/
		int energiaCosteHabilidades = 0;
		int compBasCosteHabilidades = 0;
		int compAvzCosteHabilidades = 0;
		int matBioCosteHabilidades = 0;
		
		//Filtro recursos
		for(int i = 0; i < 2; i++)
		{
			if(interfaz.togglesFiltros[i])
			{
				energiaCosteHabilidades += mejoras.costeHab0[0];
				compBasCosteHabilidades += mejoras.costeHab0[1];
				compAvzCosteHabilidades += mejoras.costeHab0[2];
				matBioCosteHabilidades += mejoras.costeHab0[3];
			}
		}
		
		//Filtro habitats
		if (interfaz.filtroHabitats)
		{
			energiaCosteHabilidades += mejoras.costeHab1[0];
			compBasCosteHabilidades += mejoras.costeHab1[1];
			compAvzCosteHabilidades += mejoras.costeHab1[2];
			matBioCosteHabilidades += mejoras.costeHab1[3];			
		}
		
		//Filtro Vegetales
		for(int i = 4; i < 14; i++)
		{
			if(interfaz.togglesFiltros[i])
			{
				energiaCosteHabilidades += mejoras.costeHab2[0];
				compBasCosteHabilidades += mejoras.costeHab2[1];
				compAvzCosteHabilidades += mejoras.costeHab2[2];
				matBioCosteHabilidades += mejoras.costeHab2[3];
			}
		}
		
		//Filtro Animales		
		for(int i = 14; i < 24; i++)
		{
			if(interfaz.togglesFiltros[i])
			{
				energiaCosteHabilidades += mejoras.costeHab3[0];
				compBasCosteHabilidades += mejoras.costeHab3[1];
				compAvzCosteHabilidades += mejoras.costeHab3[2];
				matBioCosteHabilidades += mejoras.costeHab3[3];
			}
		}
		
		//Habilidad Foco
		if (interfaz.filtroHabitats)
		{
			energiaCosteHabilidades += mejoras.costeHab4[0];
			compBasCosteHabilidades += mejoras.costeHab4[1];
			compAvzCosteHabilidades += mejoras.costeHab4[2];
			matBioCosteHabilidades += mejoras.costeHab4[3];			
		}		
		
		energiaDif = energiaEdificios + energiaProducidaNave - energiaCosteHabilidades;
		componentesBasicosDif = compBasEdificios - compBasCosteHabilidades;
		componentesAvanzadosDif = compAvzEdificios - compAvzCosteHabilidades;
		materialBiologicoDif = matBioEdificios - matBioCosteHabilidades;
	}
	
	//Actualiza los recursos sumando o restando
	public void actualizaRecursos()
	{
		energia += energiaDif;
		if(energia > energiaMax)
		{
			energia = energiaMax;
			//Avisar en el bloque de mensajes que la energía producida es superior a la que se puede almacenar
		}
		else if(energia < 0)
		{
			energia = 0;
			//Desactivar cosas hasta que la energía sea >= 0 y avisarlo por el bloque de mensajes			
		}
		
		componentesBasicos += componentesBasicosDif;
		if(componentesBasicos > componentesBasicosMax)
		{
			componentesBasicos = componentesBasicosMax;
			//Avisar en el bloque de mensajes que el número de componentes básicos producido es superior al que se puede almacenar
		}
		else if(componentesBasicos < 0)
		{
			componentesBasicos = 0;
			//Desactivar cosas hasta que el número de componentes básicos sea >= 0 y avisarlo por el bloque de mensajes			
		}
		
		componentesAvanzados += componentesAvanzadosDif;
		if(componentesAvanzados > componentesAvanzadosMax)
		{
			componentesAvanzados = componentesAvanzadosMax;
			//Avisar en el bloque de mensajes que el número de componentes avanzados producido es superior al que se puede almacenar
		}
		else if(componentesAvanzados < 0)
		{
			componentesAvanzados = 0;
			//Desactivar cosas hasta que el número de componentes avanzados sea >= 0 y avisarlo por el bloque de mensajes			
		}
		materialBiologico += materialBiologicoDif;
		if(materialBiologico > materialBiologicoMax)
		{
			materialBiologico = materialBiologicoMax;
			//Avisar en el bloque de mensajes que el número de material biológico producido es superior al que se puede almacenar
		}
		else if(materialBiologico < 0)
		{
			materialBiologico = 0;
			//Desactivar cosas hasta que el número de material biológico sea >= 0 y avisarlo por el bloque de mensajes			
		}		
	}
	
	/*//Actualiza la diferencia de recursos por turno comprobando los edificios
	public void actualizaConsumoProduccion()
	{
		int energia,compBas,compAvz,matBio;
		vida.calculaConsumoProduccion(out energia,out compBas,out compAvz,out matBio);
		energiaDif = energia;
		componentesBasicosDif = compBas;
		componentesAvanzadosDif = compAvz;
		materialBiologicoDif = matBio;
	}*/
	
	public void setEnergiaMax(int nuevo) {
		if (nuevo >= 0)
			energiaMax = nuevo;
		if (energia > energiaMax)
			energia = energiaMax;
	}
	
	public void setCompBasMax(int nuevo) {
		if (nuevo >= 0)
			componentesBasicosMax = nuevo;
		if (componentesBasicos > componentesBasicosMax)
			componentesBasicos = componentesBasicosMax;
	}
	
	public void setCompAdvMax(int nuevo) {
		if (nuevo >= 0)
			componentesAvanzadosMax = nuevo;
		if (componentesAvanzados > componentesAvanzadosMax)
			componentesAvanzados = componentesAvanzadosMax;
	}
	
	public void setMatBioMax(int nuevo) {
		if (nuevo >= 0)
			materialBiologicoMax = nuevo;
		if (materialBiologico > materialBiologicoMax)
			materialBiologico = materialBiologicoMax;
	}
	
	public void rellenaContenedor(ref ValoresCarga contenedor) {
		contenedor.texturaBase = objetoRoca.renderer.sharedMaterial.mainTexture as Texture2D;
		contenedor.texturaElementos = objetoRoca.renderer.sharedMaterials[3].mainTexture as Texture2D;
		contenedor.texturaHabitats = objetoRoca.renderer.sharedMaterials[1].GetTexture("_FiltroTex") as Texture2D;
		contenedor.texturaHabsEstetica = objetoRoca.renderer.sharedMaterials[1].mainTexture as Texture2D;
		contenedor.texturaPlantas = objetoRoca.renderer.sharedMaterials[2].mainTexture as Texture2D;
		contenedor.vida = vida;
		contenedor.roca = objetoRoca.GetComponent<MeshFilter>().mesh;
		contenedor.agua = objetoOceano.GetComponent<MeshFilter>().mesh;
		contenedor.nivelAgua = FuncTablero.getNivelAgua();
		contenedor.tamanoPlaya = FuncTablero.getTamanoPlaya();
		
		contenedor.energia = energia;
		contenedor.compBas = componentesBasicos;
		contenedor.compAdv = componentesAvanzados;
		contenedor.matBio = materialBiologico;
		
		contenedor.mejorasCompradas = mejoras.mejorasCompradas;
		contenedor.etapaJuego = (int)interfaz.etapaJuego;
	}
	
	public void mejoraEnergia1() {
		energiaProducidaNave = 15;
	}
	
	public void mejoraEnergia2() {
		energiaProducidaNave = 30;
	}
	
	private void setDeveloperMode() {
		energia = 20000;
		componentesBasicos = 8000;
		componentesAvanzados = 5000;
		materialBiologico = 500;
		
	}
	
	public void completarCarga() {
		if (hechaCarga) {
			ValoresCarga temp = contenedor.GetComponent<ValoresCarga>();
			SaveData save = temp.save;
			SaveLoad.colocarSeresTablero(ref vida, save.vidaData);
			energia = temp.energia;
			componentesBasicos = temp.compBas;
			componentesAvanzados = temp.compAdv;
			materialBiologico = temp.matBio;
			interfaz.etapaJuego = (InterfazPrincipal.tEtapaJuego)temp.etapaJuego;			
			for (int i = 0; i < temp.mejorasCompradas.Length; i++) {
				if (temp.mejorasCompradas[i]) {
					switch (i) {
					case 0: 
						mejoras.compraMejora0();
						break;
					case 1: 
						mejoras.compraMejora1();
						break;
					case 2: 
						mejoras.compraMejora2();
						break;
					case 3: 
						mejoras.compraMejora3();
						break;
					case 4: 
						mejoras.compraMejora4();
						break;
					case 5: 
						mejoras.compraMejora5();
						break;
					case 6: 
						mejoras.compraMejora6();
						break;
					case 7: 
						mejoras.compraMejora7();
						break;
					case 8: 
						mejoras.compraMejora8();
						break;
					case 9: 
						mejoras.compraMejora9();
						break;
					case 10: 
						mejoras.compraMejora10();
						break;
					case 11: 
						mejoras.compraMejora11();
						break;
					case 12: 
						mejoras.compraMejora12();
						break;
					case 13: 
						mejoras.compraMejora13();
						break;
					case 14: 
						mejoras.compraMejora14();
						break;
					case 15: 
						mejoras.compraMejora15();
						break;
					}
				}
			}
		}
	}
}
