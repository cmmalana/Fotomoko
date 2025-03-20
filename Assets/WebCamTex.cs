using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using TMPro;
using System.IO;


public class SettingsData
{
	public int cam_pos;
	public string cam;
	public int show_greenscreen;
	public string folder_name;
	public string upload_link;
	public float bright_val;
	public string local_ip;
	public int allownoframe;

	public int qrdropdownvalue;

	public int printbuttonvalue;
}

public class WebCamTex : MonoBehaviour
{
	SettingsData settingsData;
	WebCamTexture backCam;

	// mag input ng number = 0, 1, etc. para mahanap ung Snap Camera
	// karaniwan 0 = webcam, 1 = snapcamera
	int CameraType = 1;
	//public GameObject WebCam_Texture_For_Mid ;
	
	WebCamDevice[] devices;
	
	public GameObject Fotomoko2;
	public GameObject MiniCam; // WebCamTextureLeft
	public GameObject Cam2WithFrame; // WebCamTextureLeft
	public TMP_Dropdown CamDropdown; //UiSettings/CamDropdown
	public TMP_Dropdown CamPosDropdown; //UiSettings/CamPosDropdown
	public TMP_Dropdown BtnGreenScreenDropdown; //UiSettings/
	public TMP_Dropdown BtnAllowNoFrameDropdown;
	//public TMP_Dropdown BtnLocalDropdown; //UiSettings/
	public GameObject BtnGreenScreen; // BotButtons/Buttons/ButtonCam/SelectCamButtons/BtnGreenScreen
	
	public TMP_InputField UploadLinkInput; //UiSettings/UploadLinkName/UploadlInkInput
	public TMP_InputField FolderNameInput; //UiSettings/UploadLinkName/FOlderlInkInput
	public TMP_InputField LocalIpInput; //UiSettings/UploadLinkName/FOlderlInkInput
	public Slider BrightSlider;

	public Dropdown QRDropDown;
	public Button Print;
	public Dropdown PrintButtonStatus;
	string saveFilePath;
	
	void Awake()
	{
		settingsData = new SettingsData();
	}


    void Start()
    {
		settingsData = new SettingsData();
		setupCam();
		CamDropdown.value = CameraType;
		turnCamera();

		string pfat = System.IO.Directory.GetCurrentDirectory();
		string frame_pat = Path.Combine(pfat, "save_images_here");
		string pat = Path.Combine(frame_pat, "webcamsettings.json");
		saveFilePath = pat;
		//Debug.Log(saveFilePath);
		
		loadFile();
	}
	
	void setupCam(){
		if(backCam == null){
			backCam = new WebCamTexture();
		}
		devices = WebCamTexture.devices;	
		
		CamDropdown.ClearOptions();
		List<string> items = new List<string>();
        for (int i = 0; i < devices.Length; i++){
			items.Add(devices[i].name);
		}
		CamDropdown.AddOptions(items);
	}
	
	// public void turnCamera()
    // {
    //     if (devices.Length > 0)
    //     {
    //         backCam.deviceName = devices[CameraType].name;
			
	// 		GetComponent<Renderer>().material.mainTexture = backCam;
	// 		MiniCam.GetComponent<Renderer>().material.mainTexture = backCam;
	// 		Cam2WithFrame.GetComponent<Renderer>().material.mainTexture = backCam;
	// 		if(!backCam.isPlaying){
	// 			backCam.Play();
	// 		}
    //     }
    // }

    public void turnCamera()
    {
        if (backCam != null && backCam.isPlaying)
            backCam.Stop();

        if (devices.Length > 0 && CameraType < devices.Length)
        {
            backCam = new WebCamTexture(devices[CameraType].name);
            GetComponent<Renderer>().material.mainTexture = backCam;
            MiniCam.GetComponent<Renderer>().material.mainTexture = backCam;
            Cam2WithFrame.GetComponent<Renderer>().material.mainTexture = backCam;
            backCam.Play();
        }
    }

	public void SettingsCam(){
		//Debug.Log(CamDropdown.value);
		CameraType = CamDropdown.value;
		turnCamera();
	}
	
	public void SettingsCamPos(){
		//Debug.Log(CamDropdown.value);
		if (CamPosDropdown.value == 0){
			transform.rotation = Quaternion.Euler(0, 180, 90);
			MiniCam.transform.rotation = Quaternion.Euler(0, 180, 90);
			Cam2WithFrame.transform.rotation = Quaternion.Euler(0, 180, 90);
			//transform.localScale = new Vector3(1920,  1080, -10);
			//Cam2WithFrame.transform.localScale = new Vector3(1459.2f,  820.8f, -10);
		}else if(CamPosDropdown.value == 1){
			transform.rotation = Quaternion.Euler(0, 180, 270);
			MiniCam.transform.rotation = Quaternion.Euler(0, 180, 270);
			Cam2WithFrame.transform.rotation = Quaternion.Euler(0, 180, 270);
			//transform.localScale = new Vector3(1920,  1080, -10);
			//Cam2WithFrame.transform.localScale = new Vector3(1459.2f,  820.8f, -10);
		} else {
			transform.rotation = Quaternion.Euler(0, 180, 0);
			//transform.localScale = new Vector3(3456,  1944, -10);
			//MiniCam.transform.rotation = Quaternion.Euler(0, 180, 0);
			Cam2WithFrame.transform.rotation = Quaternion.Euler(0, 180, 0);
			//Cam2WithFrame.transform.localScale = new Vector3(2626.56f,  1477.44f, -10);
		}
		settingsData.cam_pos = CamPosDropdown.value;
		saveFile();
	}
	
	public void onRefresh(){
		SettingsCam();
		setupCam();
	}
   
   public void saveFile(){
		string saveSettingsData = JsonUtility.ToJson(settingsData);
        File.WriteAllText(saveFilePath, saveSettingsData);
		Debug.Log("WEBCAM CONFIG");
        Debug.Log("Save file created at: " + saveFilePath + " Settings Data: "+ saveSettingsData);
	}
	
	 public void loadFile(){
		if (File.Exists(saveFilePath)){
			//Debug.Log("meron");
            string loadSettingsData = File.ReadAllText(saveFilePath);
            settingsData = JsonUtility.FromJson<SettingsData>(loadSettingsData);
			
			CamPosDropdown.value = settingsData.cam_pos;
			SettingsCamPos();
			BtnGreenScreenDropdown.value = settingsData.show_greenscreen;
			setBtnGreenScreen();
			
			BtnAllowNoFrameDropdown.value = settingsData.allownoframe;
			setBtnAllowNoFrame();
			
			BrightSlider.value = settingsData.bright_val;
			GetComponent<Renderer>().material.SetFloat("_Metallic", settingsData.bright_val);
			
			FolderNameInput.text = settingsData.folder_name;
			Fotomoko2.GetComponent<Fotomoko>().onLoadFolderName(settingsData.folder_name);
			
			UploadLinkInput.text = settingsData.upload_link;
			Fotomoko2.GetComponent<Fotomoko>().onLoadLink(settingsData.upload_link);
			
			LocalIpInput.text = settingsData.local_ip;
			Fotomoko2.GetComponent<Fotomoko>().onLoadLocalIp(settingsData.local_ip);
			
			CamPosDropdown.value = settingsData.cam_pos;
			SettingsCamPos();

			QRDropDown.value = settingsData.qrdropdownvalue;
			QRLinkDropdown(QRDropDown.value);

			PrintButtonStatus.value = settingsData.printbuttonvalue;
			// ButtonPrintStat();
			PrintStats();
		}
		else{
			settingsData.folder_name = "folder_name"; // Initial Folder Name
			FolderNameInput.text = settingsData.folder_name;
			saveFile();
		}
	}
	
	
	public void SliderChange(float value){
		GetComponent<Renderer>().material.SetFloat("_Metallic", value);
		settingsData.bright_val = value;
		saveFile();
	}
	
	
	public void setBtnGreenScreen(){
		if (BtnGreenScreenDropdown.value == 0){
			BtnGreenScreen.gameObject.SetActive(true);
		}else{
			BtnGreenScreen.gameObject.SetActive(false);
		}
		settingsData.show_greenscreen = BtnGreenScreenDropdown.value;
		saveFile();
	}
	
	public void setBtnAllowNoFrame(){
		if (BtnAllowNoFrameDropdown.value == 0){
			//BtnGreenScreen.gameObject.SetActive(true);
			Fotomoko2.GetComponent<Fotomoko>().allowEmptyFrame = true;
			
		}else{
			//BtnGreenScreen.gameObject.SetActive(false);
			Fotomoko2.GetComponent<Fotomoko>().allowEmptyFrame = false;
		}
		Debug.Log(Fotomoko2.GetComponent<Fotomoko>().allowEmptyFrame);
		settingsData.allownoframe = BtnAllowNoFrameDropdown.value;
		saveFile();
	}

	public void QRLinkDropdown(int index){
		

		QRDropDown.value = index;
		
		
		settingsData.qrdropdownvalue = index;

		Debug.Log("QR Dropdown Status: " + settingsData.qrdropdownvalue);
		saveFile();
	}

	// Visibility ng print button depending on the config in settings
	public void StatusPrintButton(){
		if (settingsData.printbuttonvalue == 0){
			Print.gameObject.SetActive(true);
		}
		else{
			Print.gameObject.SetActive(false);
		}
	}

	// Print button status
	public void PrintStats(){
		
		settingsData.printbuttonvalue = PrintButtonStatus.value;

		Debug.Log("Print Button Status: " + settingsData.printbuttonvalue);
		saveFile();
		StatusPrintButton();
	}

	// Show ulit yung Print button pag napindot yung return button depending on Print Button Status
	public void onReturnButtonPressed(){
		if (settingsData.printbuttonvalue == 0){
			Print.gameObject.SetActive(true);
		}
		else{
			Print.gameObject.SetActive(false);
		}
	}
	
	public void onSaveFolderName(string folder_txt){
		settingsData.folder_name = folder_txt;
		saveFile();
	}
	
	public void onSaveLink(string link_txt){
		settingsData.upload_link = link_txt;
		saveFile();
	}
	
	public void onSaveLocalIp(string ip_txt){
		settingsData.local_ip = ip_txt;
		saveFile();
	}
}
