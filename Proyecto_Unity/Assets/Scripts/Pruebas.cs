using UnityEngine;
using System.Collections;

public class Pruebas : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		if (GUI.Button(new Rect(50, 30, 70, 20), "Mejora1")) {
			MejorasNave temp = GameObject.FindGameObjectWithTag("Mejoras").GetComponent<MejorasNave>();
			temp.compraMejora1();
		}
		if (GUI.Button(new Rect(150, 30, 70, 20), "Mejora2")) {
			MejorasNave temp = GameObject.FindGameObjectWithTag("Mejoras").GetComponent<MejorasNave>();
			temp.compraMejora2();
		}
	}
}
