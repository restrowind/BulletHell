using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCollection : MonoBehaviour
{
   public static int aqua;
   public static int vitality; 
   public static int lumen;

    public Text aquaText;
   public Text vitalityText;
   public Text lumenText;

   void Update()
   {
      aquaText.text = aqua.ToString();
      vitalityText.text = vitality.ToString();
      lumenText.text = lumen.ToString();
   }
}
