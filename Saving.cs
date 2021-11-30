using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Saving : MonoBehaviour
{
    void createText()
    {
        string path = Application.dataPath + "/dddeee.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "wassup");
        }
        string content = "sssss" + System.DateTime.Now;
        File.AppendAllText(path, content);
    }
}

  
