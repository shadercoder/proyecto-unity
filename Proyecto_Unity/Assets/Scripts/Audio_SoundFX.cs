using UnityEngine;
using System.Collections;

public class Audio_SoundFX : MonoBehaviour {
	
	public bool activado;
	public float volumen;
	public AudioClip[] sonidos;
	private int ultimoSonido;
	private AudioSource source;
	
	void Awake () {
		source = this.transform.GetComponent<AudioSource>();
	}
	
	public void playNumber(int entrada) {
		if (entrada < 0 || entrada >= sonidos.Length) {
			Debug.LogError("Clip de soundfx incorrecto!");
			return;
		}
		if (activado)
			source.PlayOneShot(sonidos[entrada], volumen);
		else
			Debug.Log("El reproductor de sonidofx esta desactivado.");
	}
	
}
