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
//	private Texture2D texPlantas;										//La textura donde se pintan las plantas 
	private GameObject contenedor;										//El contenedor de las texturas de la primera escena
	
	//Recursos
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
	private bool algoritmoActivado				= true;				//Se encuentra activado el algoritmo de la vida?
	private bool algoritmoPasoAPaso = false;
	//Escala de tiempo
	public float escalaTiempo					= 1.0f;					//La escala temporal a la que se updateará todo
	
	//Tipos especiales ----------------------------------------------------------------------------------------------------------------------
	
	
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
			vida.setObjetoRoca(objetoRoca.transform);
		}
		Debug.Log (FuncTablero.formateaTiempo() + ": Completada la creacion del planeta.");		
	}
	
	
	void FixedUpdate() {
		//Algoritmo de vida		
		tiempoPaso += Time.deltaTime;		
		if(algoritmoActivado && tiempoPaso > 3.0f) 		//El 1.0f significa que se ejecuta un paso cada 1.0 segundos, cuando la escala temporal esta a 1.0
		{		
			actualizaRecursos();
			vida.algoritmoVida(numPasos);
			numPasos++;
			tiempoPaso = 0.0f;
			if(algoritmoPasoAPaso)
				algoritmoActivado = false;
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
//		Casilla[,] tablero = FuncTablero.iniciaTablero(texturaBase, texHabitats, texHabitatsEstetica, texElems, Roca.sharedMesh, objetoRoca.transform.position);
		Debug.Log (FuncTablero.formateaTiempo() + ": Terminado. Creando Vida...");
		vida = new Vida(tablero, texPlantas, objetoRoca.transform);				
		Debug.Log (FuncTablero.formateaTiempo() + ": Completada la creacion del planeta.");
	}
	
	private void creacionCarga(ValoresCarga contenedor) {
		Debug.Log (FuncTablero.formateaTiempo() + ": Iniciando el script de carga de valores...");
		Texture2D texBase = contenedor.texturaBase;
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
	
	//Modifica la cantidad de cada recurso que se consume por turno
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
		
	//Actualiza los recursos sumando o restando los consumidos por turno
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
	
	//Actualiza la diferencia de recursos por turno comprobando los edificios
	public void actualizaConsumoProduccion()
	{
		int energia,compBas,compAvz,matBio;
		vida.calculaConsumoProduccion(out energia,out compBas,out compAvz,out matBio);
		energiaDif = energia;
		componentesBasicosDif = compBas;
		componentesAvanzadosDif = compAvz;
		materialBiologicoDif = matBio;
	}
	
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
	}
	
	public void mejoraEnergia1() {
		modificaEnergiaPorTurno(3);
	}
	
	public void mejoraEnergia2() {
		modificaEnergiaPorTurno(6);
	}	
}
