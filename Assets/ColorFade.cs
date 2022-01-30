using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorFade : MonoBehaviour
{
    [SerializeField] Image guiRender;
    [SerializeField] SpriteRenderer spriteRender;
    [SerializeField] Color colorIn, colorOut;
    [SerializeField] float fadeSpeed;
    [SerializeField] bool loop;
    [SerializeField] bool startOnAwake;
    private bool fading;
    private Color fadeToColor;
    private void Awake()
    {
        fadeToColor = colorIn;
        if (startOnAwake)
            StartFade(2);
    }

    public void SetLoop(bool value)
    {
        loop = value;
    }

    public void StartFade(int state)
    {
        switch (state)
        {
            case 0:
                fadeToColor = colorOut;
                break;
            case 1:
                fadeToColor = colorIn;
                break;
            case 2:
                if(fadeToColor == colorOut)
                    fadeToColor = colorIn;
                else if (fadeToColor == colorIn)
                    fadeToColor = colorOut;
                break;
        }
        StopAllCoroutines();
        if(guiRender != null)
            StartCoroutine(ImageFadeTo());
        if (spriteRender != null)
            StartCoroutine(SpriteFadeTo());
    }

    IEnumerator ImageFadeTo()
    {
        while (guiRender.color != fadeToColor)
        {
            guiRender.color = Color.Lerp(guiRender.color, fadeToColor, fadeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        if (loop)
            StartFade(2);
        yield return null;
    }

    IEnumerator SpriteFadeTo()
    {
        while (spriteRender.color != fadeToColor)
        {
            spriteRender.color = Color.Lerp(spriteRender.color, fadeToColor, fadeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        fading = false;
        if (loop)
            StartFade(2);
        yield return null;
    }
}
