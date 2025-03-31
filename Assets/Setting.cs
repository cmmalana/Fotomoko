using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class Setting : MonoBehaviour
{
    bool isOPen = false;
	
	public GameObject UiSettings; // UiSettings
	public GameObject SettingsPasswordTx; // UiSettings
	public Toggle LocalIPAddToggle;
	
	void Start(){
		UiSettings.gameObject.SetActive(false);
		SettingsPasswordTx.gameObject.SetActive(false);
		LocalIPAddToggle.gameObject.SetActive(false);
	}
	
	public void onSettings(){
		//Debug.Log("click");
		if (isOPen == true){
			SettingsPasswordTx.gameObject.SetActive(false);
			UiSettings.gameObject.SetActive(false);
			LocalIPAddToggle.gameObject.SetActive(true);
			isOPen = false;
		}else{
			SettingsPasswordTx.gameObject.SetActive(true);
			UiSettings.gameObject.SetActive(true);
			LocalIPAddToggle.gameObject.SetActive(false);
			isOPen = true;
		}
	}
	
	public void onSettingsPasswordOk(){
		//Debug.Log("click");
		if (pw == "7"){
			SettingsPasswordTx.gameObject.SetActive(false);
			LocalIPAddToggle.gameObject.SetActive(true);
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
