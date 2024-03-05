using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private List<Symbol> allSymbols;
    [SerializeField] private Reel[] reels;

    private bool _readyToSpin = true;

    private void Start()
    {
        InitializeReels();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _readyToSpin)
        {
            SpinSlot();
        }
    }

    private void InitializeReels()
    {
        foreach (var reel in reels)
        {
            reel.possibleSymbols = allSymbols;
        }
    }
    
    private void SpinSlot()
    {
        _readyToSpin = false;
        
        foreach (var reel in reels)
        {
           reel.SpinReel(); 
        }
        
        FindConnections();
    }

    private void FindConnections()
    {
        List<Symbol> relevantSymbols = new List<Symbol>();

        List<List<string>> connectionLists = new List<List<string>>();
        List<string> reelThreeConnections = new List<string>();
        List<string> reelFourConnections = new List<string>();
        List<string> reelFiveConnections = new List<string>();
        
        connectionLists.Add(reelThreeConnections);
        connectionLists.Add(reelFourConnections);
        connectionLists.Add(reelFiveConnections);

        // Add all symbols from first reel to relevant symbol list
        
        // If first reel has a wild: all symbols still relevant
        bool wildInFirstReel = false;
        
        
        foreach (var reelSymbol in reels[0].reelSymbols)
        {
            if (reelSymbol.symbol.name == "Wild")
            {
                wildInFirstReel = true;
            }
        }

        if (wildInFirstReel)
        {
            foreach (var symbol in allSymbols)
            {
                relevantSymbols.Add(symbol);
            }
        }
        else
        {
            foreach (var reelSymbol in reels[0].reelSymbols)
            {
                relevantSymbols.Add(reelSymbol.symbol);
            }
        }

        // Iterate over every reel starting from the second one
        for (int i = 1; i < reels.Length; i++)
        {
            // Check if each relevant symbol is in the next reel
            for (int j = relevantSymbols.Count - 1; j > -1; j--)
            {
                bool symbolStillRelevant = false;
                
                foreach (var reelSymbol in reels[i].reelSymbols)
                {
                    if (reelSymbol.symbol.name == relevantSymbols[j].name || reelSymbol.symbol.name == "Wild")
                    {
                        symbolStillRelevant = true;
                    }
                }

                if (!symbolStillRelevant)
                {
                    relevantSymbols.RemoveAt(j);
                }
            }

            if (i >= 2)
            {
                switch (i)
                {
                    // Reel 3
                    case 2:
                        foreach (var symbol in relevantSymbols)
                        {
                            connectionLists[0].Add(symbol.name);
                        }
                        break;
                    
                    // Reel 4
                    case 3:
                        foreach (var symbol in relevantSymbols)
                        {
                            connectionLists[0].Remove(symbol.name);
                            
                            connectionLists[1].Add(symbol.name);
                        }
                        break;
                    
                    // Reel 5
                    case 4:
                        foreach (var symbol in relevantSymbols)
                        {
                            connectionLists[1].Remove(symbol.name);
                            
                            connectionLists[2].Add(symbol.name);
                        }
                        break;
                }
            }
        }

        if (AnyConnectionFound(connectionLists))
        {
            // Connections have been searched and stored... handle them:
            StartCoroutine(DelayedConnectionHandling(connectionLists));
        }
        else
        {
            _readyToSpin = true;
        }
    }
    
    IEnumerator DelayedConnectionHandling(List<List<string>> foundConnections)
    {
        yield return new WaitForSeconds(.5f);
        HandleConnections(foundConnections);
    }

    private void HandleConnections(List<List<string>> foundConnections)
    {
        // Initially set all symbols to gray color (suggesting no connections)
        foreach (var reel in reels)
        {
            foreach (var reelSymbol in reel.reelSymbols)
            {
                reelSymbol.SetSymbolColor(Color.gray);
            }
        }
        
        // Three reel connections
        foreach (var connectionName in foundConnections[0])
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (var reelSymbol in reels[i].reelSymbols)
                {
                    if (reelSymbol.symbol.name == connectionName || reelSymbol.symbol.name == "Wild")
                    {
                        reelSymbol.SetSymbolColor(Color.white);
                    } 
                }
            }
        }
        
        // Four reel connections
        foreach (var connectionName in foundConnections[1])
        {
            for (int i = 0; i < 4; i++)
            {
                foreach (var reelSymbol in reels[i].reelSymbols)
                {
                    if (reelSymbol.symbol.name == connectionName || reelSymbol.symbol.name == "Wild")
                    {
                        reelSymbol.SetSymbolColor(Color.white);
                    } 
                }
            }
        }
        
        // Five reel connections
        foreach (var connectionName in foundConnections[2])
        {
            foreach (var reel in reels)
            {
                foreach (var reelSymbol in reel.reelSymbols)
                {
                    if (reelSymbol.symbol.name == connectionName || reelSymbol.symbol.name == "Wild")
                    {
                        reelSymbol.SetSymbolColor(Color.white);
                    }
                }
            }
        }

        StartCoroutine(DelayedConnectionClearing());
    }

    IEnumerator DelayedConnectionClearing()
    {
        yield return new WaitForSeconds(1);
        ClearConnections();
    }
    
    private void ClearConnections()
    {
        foreach (var reel in reels)
        {
            foreach (var reelSymbol in reel.reelSymbols)
            {
                if (reelSymbol.GetSymbolColor() == Color.white)
                {
                    reelSymbol.symbol = null;
                    reelSymbol.UpdateSymbolVisual();
                }
            }
        }

        StartCoroutine(DelayedSymbolGrounding());
    }

    IEnumerator DelayedSymbolGrounding()
    {
        yield return new WaitForSeconds(.33f);
        foreach (var reel in reels)
        {
            foreach (var reelSymbol in reel.reelSymbols)
            {
                reelSymbol.SetSymbolColor(Color.white);
            }
        }
        
        GroundSymbols();
        yield return new WaitForSeconds(.05f);
        GroundSymbols();
        yield return new WaitForSeconds(.05f);
        GroundSymbols();
        yield return new WaitForSeconds(.05f);
        GroundSymbols();

        StartCoroutine(DelayedReelRefilling());
    }
    
    private void GroundSymbols()
    {
        foreach (var reel in reels)
        {
            for (int i = 1; i < reel.reelSymbols.Count; i++)
            {
                // If symbol slot below current one is empty && current one is not empty...
                if (reel.reelSymbols[i - 1].symbol == null && reel.reelSymbols[i] != null)
                {
                    // Drop current symbol down
                    reel.reelSymbols[i - 1].symbol = reel.reelSymbols[i].symbol;
                    reel.reelSymbols[i].symbol = null;
                }
            }
        }

        foreach (var reel in reels)
        {
            foreach (var reelSymbol in reel.reelSymbols)
            {
                reelSymbol.UpdateSymbolVisual();
            }
        }
    }

    IEnumerator DelayedReelRefilling()
    {
        yield return new WaitForSeconds(1f);
        
        RefillReels();
    }
    
    private void RefillReels()
    {
        foreach (var reel in reels)
        {
            reel.RefillReel();
        }
        
        FindConnections();
    }

    private bool AnyConnectionFound(List<List<string>> foundConnections)
    {
        bool connectionFound = false;

        foreach (var connectionList in foundConnections)
        {
            if (connectionList.Count > 0)
            {
                connectionFound = true;
            }
        }

        return connectionFound;
    }
}
