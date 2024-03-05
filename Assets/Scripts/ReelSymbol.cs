using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ReelSymbol : MonoBehaviour
{
    public Symbol symbol;

    private Image _symbolImage;

    private void Start()
    {
        _symbolImage = GetComponent<Image>();
    }
    
    public void UpdateSymbolVisual()
    {
        if (symbol == null)
        {
            _symbolImage.sprite = null;
        }

        else
        {
            _symbolImage.sprite = symbol.symbolSprite;
        }
    }

    public void SetSymbolColor(Color color)
    {
        _symbolImage.color = color;
    }

    public Color GetSymbolColor()
    {
        return _symbolImage.color;
    }
}
