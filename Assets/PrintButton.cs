using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using UnityEngine.UI;

public class PrintButton : MonoBehaviour
{

    public Button ButtonPrint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPrintButton(){
        Process.Start(Environment.CurrentDirectory + @"\print.exe");
        // Debug.Log("Filepath: " + Environment.CurrentDirectory + @"\print.exe");
        ButtonPrint.gameObject.SetActive(false);
    }
}
