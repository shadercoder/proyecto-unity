using UnityEngine;
using System.Collections;

public class Pruebas : MonoBehaviour {
	
	void OnGUI() {
		if (GUI.Button(new Rect(50, 30, 70, 20), "Speed1")) {
			MejorasNave temp = GameObject.FindGameObjectWithTag("Mejoras").GetComponent<MejorasNave>();
			temp.compraMejora1();
		}
		if (GUI.Button(new Rect(150, 30, 70, 20), "Speed2")) {
			MejorasNave temp = GameObject.FindGameObjectWithTag("Mejoras").GetComponent<MejorasNave>();
			temp.compraMejora2();
		}
		
		if (GUI.Button(new Rect(250, 30, 70, 20), "Orbita")) {
			MejorasNave temp = GameObject.FindGameObjectWithTag("Mejoras").GetComponent<MejorasNave>();
			temp.compraMejora7();
		}
	}
}
