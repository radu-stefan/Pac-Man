using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class img_rotate : MonoBehaviour {

    public Sprite sp1;
    public Sprite sp2;
    public Sprite sp3;
    private int i;

    private Image[] images;
    // Use this for initialization
    void Start () {
        images = gameObject.GetComponentsInChildren<Image>();
        {
            for (i = 0; i < 3; i++)
                StartCoroutine(loseInvurnelability(1, i));
        }

    }
	
	// Update is called once per frame
	void Update () {
        
    }

    IEnumerator loseInvurnelability(float time, int img)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        foreach (Image image in images)
        {
            if (i == 0)
                image.sprite = sp1;
            if (i == 1)
                image.sprite = sp2;
            if (i == 2)
                image.sprite = sp3;
        }
    }
}
