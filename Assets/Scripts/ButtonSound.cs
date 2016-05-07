using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour {

    public AudioClip clickSound;
    public AudioClip hoverSound;

    private Button button { get { return GetComponent<Button>(); } }
    private AudioSource source { get { return GetComponent<AudioSource>(); } }

	// Use this for initialization
	void Start () {
        gameObject.AddComponent<AudioSource>();
        source.clip = hoverSound;
        source.playOnAwake = false;

        button.onClick.AddListener(() => PlaySound());
	}
	
	// Update is called once per frame
	void PlaySound () {
        source.clip = clickSound;
        source.PlayOneShot(clickSound);
	}

    void OnMouseOver() {
        source.clip = hoverSound;
        source.PlayOneShot(hoverSound);
    }
}
