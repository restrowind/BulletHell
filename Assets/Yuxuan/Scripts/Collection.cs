using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collection : MonoBehaviour
{
   public static int blue;
   public static int green; 
   public static int red;
   
   public Text blueText;
   public Text greenText;
   public Text redText;

   void Update()
   {
      blueText.text = blue.ToString();
      greenText.text = green.ToString();
      redText.text = red.ToString();
   }
}
