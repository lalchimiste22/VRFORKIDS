using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambiarDeEscena : MonoBehaviour {

	public string nombreDeEscena;		//nombre de la escena a la que quiero pasar
	public float delay = 0f;


	// metodo que va a aparecer en el listado de acciones al configurar mi Boton.
	public void CambiarEscena() {
		float delay = 0f;
		StartCoroutine (_CambiarEscena(delay));
	}
	private IEnumerator _CambiarEscena(float delay) {
		yield return new WaitForSecondsRealtime(delay);

		UnityEngine.SceneManagement.SceneManager.LoadScene (nombreDeEscena); // esta cambiando de escena
	}
}
