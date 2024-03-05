using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Slot/Symbol", fileName = "New Symbol")]
public class Symbol : ScriptableObject
{
    public string symbolName;
    
    public Sprite symbolSprite;
}
