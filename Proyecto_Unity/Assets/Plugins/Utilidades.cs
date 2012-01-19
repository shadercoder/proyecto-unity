using UnityEngine;    // For Debug.Log, etc.

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

//Clase en la que guardar las preferencias --------------------------------------------------------------------------------------------
public class Opciones {
	private bool musicaOn = true;		//Está la música activada?
	private float musicaVol = 0.5f;		//A que volumen?
	private bool sfxOn = true;			//Estan los efectos de sonido activados?
	private float sfxVol = 0.5f; 		//A que volumen?

	//Getters y setters para las variables
	public void setMusicaOn(bool sel) {
		musicaOn = sel;
	}
	
	public void setMusicaVol(float sel) {
		musicaVol = Mathf.Clamp01(sel);
	}
	
	public void setSfxOn(bool sel) {
		sfxOn = sel;
	}
	
	public void setSfxVol(float sel) {
		sfxVol = Mathf.Clamp01(sel);
	}
	
	public bool getMusicaOn() {
		return musicaOn;
	}
	
	public float getMusicaVol() {
		return musicaVol;
	}
	
	public bool getSfxOn() {
		return sfxOn;
	}
	
	public float getSfxVol() {
		return sfxVol;
	}
}


//Clase contenedora del savegame ------------------------------------------------------------------------------------------------------
[Serializable ()]
public class SaveData : ISerializable { 

  // === Values ===
  // Edit these during gameplay
  public Texture2D normalMap;
  public Opciones opcionesPerdurables;
  // === /Values ===

  // The default constructor. Included for when we call it during Save() and Load()
  public SaveData () {}

  // This constructor is called automatically by the parent class, ISerializable
  // We get to custom-implement the serialization process here
  public SaveData (SerializationInfo info, StreamingContext ctxt)
  {
    // Get the values from info and assign them to the appropriate properties. Make sure to cast each variable.
    // Do this for each var defined in the Values section above
    normalMap = (Texture2D)info.GetValue("normalMap", typeof(Texture2D));
    opcionesPerdurables = (Opciones)info.GetValue("opcionesPerdurables", typeof(Opciones));
  }

  // Required by the ISerializable class to be properly serialized. This is called automatically
  public void GetObjectData (SerializationInfo info, StreamingContext ctxt)
  {
    // Repeat this for each var defined in the Values section
    info.AddValue("normalMap", normalMap);
    info.AddValue("opcionesPerdurables", opcionesPerdurables);
  }
}

//Clase con los métodos a llamar para crear el savegame -------------------------------------------------------------------------------
public class SaveLoad {
	
	public static string currentFileName = "SaveGame.hur";						// Edit this for different save files
	public static string currentFilePath = Application.dataPath + "/Saves/";
  	public static string completeFilePath =  currentFilePath + currentFileName;    

  	// Call this to write data
  	public static void Save (Texture2D norm, Opciones opc)  // Overloaded
  	{
    	Save (completeFilePath, norm, opc);
  	}
  	public static void Save (string filePath, Texture2D norm, Opciones opc)
  	{
	    SaveData data = new SaveData ();
		data.normalMap = norm;
		data.opcionesPerdurables = opc;
	
	    Stream stream = File.Open(filePath, FileMode.Create);
	    BinaryFormatter bformatter = new BinaryFormatter();
	    bformatter.Binder = new VersionDeserializationBinder(); 
	    bformatter.Serialize(stream, data);
	    stream.Close();
  	}

  	// Call this to load from a file into "data"
  	public static void Load ()  {			// Overloaded
		Load(completeFilePath);
	}   
  	public static void Load (string filePath) 
  	{
	    SaveData data = new SaveData ();
	    Stream stream = File.Open(filePath, FileMode.Open);
	    BinaryFormatter bformatter = new BinaryFormatter();
	    bformatter.Binder = new VersionDeserializationBinder(); 
	    data = (SaveData)bformatter.Deserialize(stream);
	    stream.Close();

    // Now use "data" to access your Values
	}
	
	public static int FileCount () {
		DirectoryInfo d = new DirectoryInfo(currentFilePath);
		FileInfo[] ret = d.GetFiles();
		int num = ret.GetLength(0); 
		return num; 
	}
	
	public static string[] getFileNames() {
		DirectoryInfo d = new DirectoryInfo(currentFilePath);
		FileInfo[] ret = d.GetFiles();
		int num = ret.GetLength(0);
		string[] str = new string[num];
		for (int i = 0; i < num; i++) {
			str[i] = ret[i].Name;
		}
		return str;
	}
	
	public static void cambiaFileName(string nuevo) {
		currentFileName = nuevo;
	}

}

// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
// Do not change this
public sealed class VersionDeserializationBinder : SerializationBinder 
{ 
    public override Type BindToType( string assemblyName, string typeName )
    { 
        if ( !string.IsNullOrEmpty( assemblyName ) && !string.IsNullOrEmpty( typeName ) ) 
        { 
            Type typeToDeserialize = null; 

            assemblyName = Assembly.GetExecutingAssembly().FullName; 

            // The following line of code returns the type. 
            typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) ); 

            return typeToDeserialize; 
        } 

        return null; 
    } 
}