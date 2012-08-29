using UnityEngine;
using System.Collections;

public class cambiarAnimacion : MonoBehaviour {

	public AnimationClip animacion;
	
	void Play ()
	{
	    transform.animation.Play(animacion.name);
	}
}
