using UnityEngine;
using System.Collections;

public class ConversorVertices : MonoBehaviour
{

	public class ComparadorUVy : IComparer
	{
		public int Compare (System.Object x, System.Object y)
		{
			Vector2 objX = (Vector2)x;
			Vector2 objY = (Vector2)y;
			//Devuelve negativo si el primer elemento es menor, 0 si son iguales y positivo si es mayor
			return Mathf.RoundToInt ((objX.y - objY.y) * 5000);
		}
	}

	public class ComparadorUVx : IComparer
	{
		public int Compare (System.Object x, System.Object y)
		{
			Vector2 objX = (Vector2)x;
			Vector2 objY = (Vector2)y;
			//Devuelve negativo si el primer elemento es menor, 0 si son iguales y positivo si es mayor
			return Mathf.RoundToInt ((objX.x - objY.x) * 5000);
		}
	}

	public class ComparadorClases : IComparer
	{
		public int Compare (System.Object x, System.Object y)
		{
			ArrayList objX = (ArrayList)x;
			ArrayList objY = (ArrayList)y;
			Vector2 temp1 = (Vector2)objX[0];
			Vector2 temp2 = (Vector2)objY[0];
			//Devuelve negativo si el primer elemento es menor, 0 si son iguales y positivo si es mayor
			return Mathf.RoundToInt ((temp1.y - temp2.y) * 5000);
		}
	}

	public Mesh malla;

	private Vector3[] finalVertices;
	private Vector2[] finalUVs;
	private int[] finalTriangulos;

	private ArrayList verts;
	private ArrayList uvs;

	void Start ()
	{
		Debug.Log (formateaTiempo () + ": Comenzando script...");
		
		finalVertices = new Vector3[malla.vertices.Length];
		finalUVs = new Vector2[malla.uv.Length];
		finalTriangulos = new int[malla.triangles.Length];
		verts = new ArrayList ((int)(malla.vertices.Length * 1.5f));
		uvs = new ArrayList ((int)(malla.uv.Length * 1.5f));
		saveMesh ();
	}

	void saveMesh ()
	{
		
		Debug.Log (formateaTiempo () + ": Creando ArrayLists...");
		
		//Agrega a los arrayList verts y uvs los elementos que les corresponden
		for (int i = 0; i < malla.vertices.Length; i++) {
			verts.Add (malla.vertices[i]);
		}
		for (int i = 0; i < malla.uv.Length; i++) {
			uvs.Add (malla.uv[i]);
		}
		
		Debug.Log (formateaTiempo () + ": ArrayLists creados.");
		Debug.Log (formateaTiempo () + ": Ordenando UVs...");
		
		IComparer comparador = new ComparadorUVy ();
		uvs.Sort (comparador);
		
		Debug.Log (formateaTiempo () + ": UVs ordenados.");
		Debug.Log (formateaTiempo () + ": Comenzando separacion en clases de los uvs...");
		ArrayList arrayClases = new ArrayList ();
		ArrayList clase = null;
		float fclase = -1.0f;
		int numObj = uvs.Count;
		int numProcesados = 0;
		int numClases = 0;
		for (int i = 0; i < numObj; i++) {
			Vector2 temp = (Vector2)uvs[i];
			if (temp.y != fclase) {
				fclase = temp.y;
				if (clase != null)
					arrayClases.Add (clase);
				clase = null;
				// Este codigo es por si los uvs no estuvieran ordenados! En otro caso es inutil y es muy lento.
				for (int j = 0; j < arrayClases.Count; j++) {
					ArrayList claseTemp = (ArrayList)arrayClases[j];
					Vector2 claseTempUV = (Vector2)claseTemp[0];
					if (claseTempUV.y == fclase) {
						clase = (ArrayList)arrayClases[j];
						arrayClases.RemoveAt (j);
						break;
					}
				}
				if (clase == null)
					clase = new ArrayList ();
				numClases++;
			}
			clase.Add (temp);
			numProcesados++;
		}
		arrayClases.Add (clase);
		
		Debug.Log ("Procesados el " + System.String.Format ("{0:000}", (numProcesados / numObj) * 100) + "% de los objetos");
		Debug.Log ("separados en " + numClases + " clases diferentes.");
		Debug.Log (formateaTiempo () + ": Separacion en clases completada.");
		Debug.Log (formateaTiempo () + ": Ordenando arrays de clases por valor X...");
		
		//Ordena cada elemento de arrayClases por sus valores en X (U) 
		//de forma ascendente.
		IComparer comparador2 = new ComparadorUVx ();
		for (int j = 0; j < arrayClases.Count; j++) {
			((ArrayList)arrayClases[j]).Sort (comparador2);
		}
		
		Debug.Log (formateaTiempo () + ": Ordenacion por X completada.");
		
		//Ordena arrayClases poniendo primero los ArrayList con los valores de Y (V) 
		//en su primer elemento mas bajos.
		IComparer comparador3 = new ComparadorClases ();
		arrayClases.Sort (comparador3);
		
		Debug.Log (formateaTiempo () + ": Ordenando vertices y triangulos acorde a los uvs...");
		
		//Vamos rellenando los arrays finales (arrays built-in) siguiendo el orden de los ArrayList,
		//primero los V mas bajos, y dentro de cada clase de Vs los Us mas bajos primero.
		int indiceVerts = 0;
		for (int i = 0; i < arrayClases.Count; i++) {
			ArrayList temp = (ArrayList)arrayClases[i];
			for (int j = 0; j < temp.Count; j++) {
				Vector2 temp2 = (Vector2)temp[j];
				int numPosUV = -1;
				for (int k = 0; k < malla.uv.Length; k++) {
					if ((malla.uv[k].x == temp2.x) && (malla.uv[k].y == temp2.y)) {
						numPosUV = k;
						break;
					}
				}
				if (numPosUV == -1) {
					Debug.LogError("Error al coincidir los uvs originales con los de los arrayLists!");
					return;
				}
				finalVertices[indiceVerts] = malla.vertices[numPosUV];
				finalUVs[indiceVerts] = malla.uv[numPosUV];
				finalTriangulos[indiceVerts*3] = malla.triangles[numPosUV*3];
				finalTriangulos[indiceVerts*3 + 1] = malla.triangles[numPosUV*3 + 1];
				finalTriangulos[indiceVerts*3 + 2] = malla.triangles[numPosUV*3 + 2];
				indiceVerts++;
			}
		}
		
		Debug.Log (formateaTiempo () + ": Guardando objeto en el archivo correspondiente...");
		
		
	}

	string formateaTiempo ()
	{
		string result = "";
		int tiempo = (int)Time.realtimeSinceStartup;
		int minutes = tiempo / 60;
		int seconds = tiempo % 60;
		int fraction = (tiempo * 100) % 100;
		result = System.String.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
		return result;
	}
}
