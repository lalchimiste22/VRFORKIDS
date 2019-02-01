using UnityEngine;
using System.Collections;

public class MyFadeInOut : MonoBehaviour {

	public bool isEnabled = true;
	public bool isVisible;
	public float fadeInDuration = 1f;
	public float fadeOutDuration = 1f;

	private Color m_originalColor;
	private Color m_transparentColor;
	private Renderer m_rend;
	private bool isFadingIn, isFadingOut;
	private bool m_fadeOutAfterFadeInOneTime;
	private float lerpVar = 0f;//must start at 0

	void Start () {
		setRenderer ();
		m_originalColor = m_rend.material.color;
		m_transparentColor = new Color (m_originalColor.r, m_originalColor.g, m_originalColor.b, 0f);
		if (isVisible)
			m_rend.material.color = m_originalColor;
		else
			m_rend.material.color = m_transparentColor;
	}
	void OnValidate() {
		setRenderer ();
	}
	private void setRenderer() {
		m_rend = GetComponent<Renderer> ();
	}




	void Update () {
		if (isEnabled) {

			if (isFadingIn && !isFadingOut) {
				lerpVar += Time.deltaTime / fadeInDuration;
				m_rend.material.color = Color.Lerp (m_transparentColor, m_originalColor, lerpVar);
				if (lerpVar >= 1f) {
					isFadingIn = false;
					lerpVar = 0f;
					if (m_fadeOutAfterFadeInOneTime) {
						isFadingOut = true;
						fadeOutDuration *= 3f;
					}
				}
			} else if (isFadingOut && !isFadingIn) {
				lerpVar += Time.deltaTime / fadeOutDuration;
				m_rend.material.color = Color.Lerp (m_originalColor, m_transparentColor, lerpVar);
				if (lerpVar >= 1f) {
					isFadingOut = false;
					lerpVar = 0f;
					if (m_fadeOutAfterFadeInOneTime) {
						fadeOutDuration /= 3f;
						m_fadeOutAfterFadeInOneTime = false;
					}
				}
			}
		}

	}

	public void SetActive(bool b) {
		isEnabled = b;
	}

	public void FadeIn() {
		isFadingIn = true;
		isFadingOut = false;
	}
	public void FadeOut() {
		isFadingOut = true;
		isFadingIn = false;
	}
	public void SetFadeInDuration(float new_duration) {
		this.fadeInDuration = new_duration;
	}
	public void SetFadeOutDuration(float new_duration) {
		this.fadeOutDuration = new_duration;
	}
	public void FadeInAndOut() {
		m_fadeOutAfterFadeInOneTime = true;
		FadeIn ();
	}

}
