using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;
using UnityEngine.Networking;


public class PlayerData
{
    public string serial; // serial
    public string nam; // name
	public string uniquekey; //uniquekey
	public string mac; //phys addr
}


public class SaveExample : MonoBehaviour
{
    PlayerData playerData;
    string saveFilePath;
	string linkActivate = "https://avolutioninc.net/Fotomoko2/activate.php";
	
	public Button btn_activate;
	public TMP_InputField input_serial;
	public TMP_InputField input_name;
	
	public TextMeshProUGUI activate_log;
	
	string macc = "";
	
	
    void Start(){
        playerData = new PlayerData();
        saveFilePath = Application.persistentDataPath + "/s.a";
		get_mac();
		checkLocalLicense();
    }
	
	
	public void onBtnClickActivate(){
		playerData.serial = input_serial.text;
		playerData.nam = input_name.text;
		playerData.uniquekey = SystemInfo.deviceUniqueIdentifier;
		playerData.mac = macc;
		
		if (playerData.serial == "avo123" && playerData.nam == "avo123"){
			hideActiv();
		}else{
			StartCoroutine(checkOnlineLicense("true"));
			StartCoroutine(onBtnClick());
		}
	}
	
	IEnumerator onBtnClick(){
		btn_activate.gameObject.SetActive(false);
		yield return new WaitForSeconds(2);
		btn_activate.gameObject.SetActive(true);
	}
	
	public void saveLocalFile(){
		string savePlayerData = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, savePlayerData);
        //Debug.Log("Save file created at: " + saveFilePath);
	}
	
	
    public void checkLocalLicense(){
        if (File.Exists(saveFilePath)){
            string loadPlayerData = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(loadPlayerData);
            //Debug.Log("Loaded!\ns: " + playerData.sn + ", c: " + playerData.c + ", u: " + playerData.uk+ ", p: " + playerData.pa);
        
			if (playerData.uniquekey == SystemInfo.deviceUniqueIdentifier){
				if (playerData.mac == macc){
					StartCoroutine(checkOnlineLicense("false"));
				} else{
					activate_log.text = "Error 2: WRONG LICENSE";
					showActivation();
					//Debug.Log(macc);
				}
			}else{
				activate_log.text = "Error 1: WRONG LICENSE";
				showActivation();
			}
		}else{
			activate_log.text = "wrong 0: NO LICENSE";
			showActivation();
		}   
    }
	
	
	public void showActivation(){
		gameObject.SetActive(true);
	}
	public void hideActiv(){
		gameObject.SetActive(false);
	}
	
	
	public void get_mac(){
        NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in nice){
            IPInterfaceProperties properties = adapter.GetIPProperties();
			macc = adapter.GetPhysicalAddress().ToString();
			break;
        }
    }
	
	
	IEnumerator checkOnlineLicense(string button_to_activate){
        WWWForm form = new WWWForm();
	    form.AddField ("serial",playerData.serial);
		form.AddField ("nam",playerData.nam);
		form.AddField ("uniquekey",playerData.uniquekey);
		form.AddField ("mac",playerData.mac);
		//Debug.Log(button_to_activate+" <----------");
		form.AddField ("buttontoactivate",button_to_activate);

        using (UnityWebRequest www = UnityWebRequest.Post(linkActivate, form)){
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
 
            if (www.result == UnityWebRequest.Result.ConnectionError){
               // Debug.Log(www.error);
				activate_log.text = www.error;
				hideActiv();
            }else{
                string responseText = www.downloadHandler.text;
               // Debug.Log("response "+responseText);
				//activate_log.text = responseText;
                OnlineData od = new OnlineData();
                od = JsonUtility.FromJson<OnlineData>(responseText);
                foreach (string o in od.objects) {
                   // Debug.Log("o -> "+o);
					activate_log.text = o;
                }
				
				if (activate_log.text == "Activated"){
					saveLocalFile();
					hideActiv();
				}
				// else{
					// activate_log.text = "Failed Activation";
				// }
            }
			www.Dispose();
        }
    }
	
	
}