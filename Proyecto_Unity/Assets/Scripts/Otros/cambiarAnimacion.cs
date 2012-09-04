using UnityEngine;
using System.Collections;

public class cambiarAnimacion : MonoBehaviour {

	public AnimationClip animacion;
	
	public GameObject particulasDormir;
	public GameObject particulasComer;
	public GameObject particulasMorir;
	
	public void Play ()
	{
		if (animacion != null)
	    	transform.animation.Play(animacion.name);
	}
	
	public void playParticulasDormir() {
		if (particulasDormir != null)
			Instantiate(particulasDormir, this.gameObject.transform.position, this.gameObject.transform.rotation);
	}
	
	public void playParticulasComer() {
		if (particulasComer != null)
			Instantiate(particulasComer, this.gameObject.transform.position, this.gameObject.transform.rotation);
	}
	
	public void playParticulasMorir() {
		if (particulasMorir != null)
			Instantiate(particulasMorir, this.gameObject.transform.position, this.gameObject.transform.rotation);
	}
}
