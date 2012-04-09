using UnityEngine;
using System.Collections;

public class Audio_Ambience : MonoBehaviour {

	public bool activado;
	public float volumen;
	public AudioClip[] canciones;
	private int ultimaCancion;
	private AudioSource source;
	
	
	void Awake () {
		source = this.transform.GetComponent<AudioSource>();
	}
	
	void Start() {
		ultimaCancion = Random.Range(0, canciones.Length);
		source.clip = canciones[ultimaCancion];
		source.loop = false;
		if (activado)
			source.Play();
	}
	
	void LateUpdate() {
		if (!source.isPlaying && activado) {
			int temp = Random.Range(0, canciones.Length);
			while (temp == ultimaCancion) {
				temp = Random.Range(0, canciones.Length);
			}
			source.clip = canciones[temp];
			ultimaCancion = temp;
			source.Play();
		}
	}
	
}
