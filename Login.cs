using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Login :MonoBehaviour
{
    //Setting the variables
    public string thename;
    public GameObject inputField;
    public GameObject textDisplay;
    

    public void Storename()
    {
        thename = inputField.GetComponent<Text>().text;//Gets the input from the input box.
        if (thename.Length <= 8 && 1 <= thename.Length)//Checks the length of the name
        {  
            SceneManager.LoadScene("gameMenuScene");//loads the next Scene
        }
        else if(thename.Length == 0 || thename.Length > 8)//checks if the input is more than 8 characters or there is no input
        
            textDisplay.GetComponent<Text>().text = "Has to be between 1 to 8 characters";//Display text at the bottom of the screen
    }
   
}
