using UnityEngine;
using System.Collections.Generic;

public class Panino : MonoBehaviour
{
    [Header("Visuali salse (opzionali)")]
    public GameObject ketchupVisual;
    public GameObject maioneseVisual;

    private HashSet<SauceType> sauces = new HashSet<SauceType>();

    public void AddSauce(SauceType sauce)
    {
        if (sauces.Add(sauce))
        {
            ShowSauceVisual(sauce);
            Debug.Log($"Panino: aggiunta {sauce}. Totale salse: {sauces.Count}");
        }
    }

    private void ShowSauceVisual(SauceType sauce)
    {
        switch (sauce)
        {
            case SauceType.Ketchup:
                if (ketchupVisual != null) ketchupVisual.SetActive(true);
                break;
            case SauceType.Maionese:
                if (maioneseVisual != null) maioneseVisual.SetActive(true);
                break;
        }
    }

    public bool HasSauce(SauceType sauce) => sauces.Contains(sauce);
    public int SauceCount => sauces.Count;
}