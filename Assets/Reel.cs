using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reel : MonoBehaviour
{
    public List<Symbol> possibleSymbols;
    
    public List<ReelSymbol> reelSymbols;

    public void SpinReel()
    {
        foreach (var reelSymbol in reelSymbols)
        {
            reelSymbol.SetSymbolColor(Color.white);
            
            int i = Random.Range(0, possibleSymbols.Count);
            reelSymbol.symbol = possibleSymbols[i];
            reelSymbol.UpdateSymbolVisual();
        }
    }

    public void RefillReel()
    {
        foreach (var reelSymbol in reelSymbols)
        {
            if (reelSymbol.symbol == null)
            {
                int i = Random.Range(0, possibleSymbols.Count);
                reelSymbol.symbol = possibleSymbols[i];
                reelSymbol.UpdateSymbolVisual();
            }
        }
    }
}
