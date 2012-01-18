using UnityEngine;    // For Debug.Log, etc.

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

// === This is the info container class ===
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

// === This is the class that will be accessed from scripts ===
public class SaveLoad {
	
	public static string currentFileName = "SaveGame.hur";						// Edit this for different save files
  	public static string currentFilePath = Application.dataPath + "/Saves/" + currentFileName;    

  // Call this to write data
  public static void Save (Texture2D norm, Opciones opc)  // Overloaded
  {
    Save (currentFilePath, norm, opc);
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
  public static void Load ()  { Load(currentFilePath);  }   // Overloaded
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
	
//	public static int FileCount () {
//		DirectoryInfo d = new DirectoryInfo(Application.dataPath);
//		int ret = d.GetFiles.GetLength(0) + 1; 
//		return ret; 
//	}

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