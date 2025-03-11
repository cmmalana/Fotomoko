using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Setting : MonoBehaviour
{
    bool isOPen = false;
	
	public GameObject UiSettings; // UiSettings
	public GameObject SettingsPasswordTx; // UiSettings
	
	void Start(){
		UiSettings.gameObject.SetActive(false);
		SettingsPasswordTx.gameObject.SetActive(false);
	}
	
	public void onSettings(){
		//Debug.Log("click");
		if (isOPen == true){
			SettingsPasswordTx.gameObject.SetActive(false);
			UiSettings.gameObject.SetActive(false);
			isOPen = false;
		}else{
			SettingsPasswordTx.gameObject.SetActive(true);
			UiSettings.gameObject.SetActive(true);
			isOPen = true;
		}
	}
	
	public void onSettingsPasswordOk(){
		//Debug.Log("click");
		if (pw == "7"){
			SettingsPasswordTx.gameObject.SetActive(false);
		}
		pw = "";
	}
	
	string pw = "";
	public void onNumber(int num){
		if (pw.Length < 4){
			pw = pw+num;
		}
	}
}
