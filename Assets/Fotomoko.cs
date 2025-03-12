using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.ComponentModel;
using UnityEngine.EventSystems;

using UnityEngine.Recorder;
//using UnityEngine.Recorder.Input;
using UnityEngine.Video;
// QR
using ZXing;
using ZXing.QrCode;
using System.Data;
// Video
// using System.Linq;
// using UnityEngine.Windows.WebCam;

public class DataSettings{
	public int capturetimervalue;
	public int cammirrorvalue;
	public int collagevalue;
}


public class Fotomoko : MonoBehaviour
{

	DataSettings datasettings;
	public TextMeshProUGUI TimerText;
	public GameObject TimerWhite;
	// public Image UiCanvas;
	public Image UiCanvasCapturedImage;
	public GameObject UiCanvasCapturedVideo;
	public RawImage RawImageForVid;
	public RawImage FramedPic;
	public RawImage LogoImage;
	public RawImage BgImage;
	public GameObject UiButtons;
	public TextMeshProUGUI TextQR;
	public TextMeshProUGUI TextDebug;
	//public Canvas UiWaterMark;
	public Button BtnGenQr;
	public RawImage SupportQRCodeGenerator;
	public Image QrLoad; // UIQr/SupportQr/QrLoading
	public TMP_InputField FolderNameInput;
	public TMP_InputField LinkInput;
	public TMP_InputField LocalIpInput;
	public Button ButtonCam;
	public Button ButtonMirrorCam;
	public Image ButtonMirrorCamCheck;
	public VideoPlayer ChromaVid;
	public TextMeshProUGUI BtnFrameText;
	public GameObject UiNoFrame;
	public GameObject WebCam_Texture;
	public GameObject SelectCamButtons;
	public GameObject QRLink;
	public Dropdown QRDropDown;
	public Dropdown PrintButtonStatus;
	//public Button ButtonSelectFrame;
	// public Button ButtonSelectFrameRight;
	// public Button ButtonSelectFrameLeft;
	//public TextMeshProUGUI TimerTx;
	
	public Camera MainCam;
	
	// public bool isVideoOn = true;
	public VideoClip FileCaptureMov;
	public int VideoSeconds = 5;
	public TextMeshProUGUI BtnVidText;
	// public int Video1080 = 1080;
	// public int Video1920 = 1920;
	public Sprite SpriteCam;
	public Sprite SpriteVid;
	
	bool isMirror;
	bool isCam = true;
	// bool isVid = false;
	// bool isFrame = true;
	Texture2D StoreEncodedTexture;
	Texture2D Imahe;
	Animator CanvasAnim;
	Animator ButtonCamAnim;
	Animator TextAnim;
	Animator CameraAnim;
	//Animator ButtonSelectFrameAnim;
	int FrameCountMax = 0;
	int FrameCount = 0;
	string finaldate;
	string outside_file_path;
	string folder_name_default = "folder_name";
	string folder_name = "";
	//string link = "https://avolutioninc.hopto.org/Fotomoko2/";
	string link_default = "https://avolutioninc.net/Fotomoko2/";
	string link = "";
	string local_ip_default = "";
	string local_ip = "";
	
	VideoPlayer videoPlayer;
	
	UiChroma UICHROMA;
	// VideoCaptureExample VIDEOCAPTURE;
	
	// -- video
	//RecorderController m_RecorderController;
	public bool m_RecordAudio = true;
	//internal MovieRecorderSettings m_Settings = null;
	
	
	// ------ edit values here --------
	public bool IsGreenScreen = false;
	
	public int chroma_number = 0;
	public bool allowEmptyFrame = false;
	
	ArrayList Picture_Paths = new ArrayList();

	public bool printbutton;

	public int printbuttonvalue;
	public Dropdown CaptureTimer;
	string saveFilePath;
	public Dropdown CamMirror;
	public GameObject WebCamDuplicate;
	public Dropdown Collage;
	private Animator webcamduplicate;
	private string webcameduplicateanimation;
	public GameObject CollageLayout;
	public Canvas Collage4x4;
	private Image[] CollageImages;
	string collagepath;
	private Animator CollageLayoutAnimation;
	private string CollageLayouts;
	private int count = 0;
	public GameObject CamCaptureAnim;
	private Animator CamCaptureAnimator;

	void Awake()
	{
		datasettings = new DataSettings();
	}

	
	void Start(){
		// print(System.IO.Directory.GetCurrentDirectory()+ " ----- asd");
		Picture_Paths.Add("frame0");
		TextDebug.text = "test1";
		UICHROMA = GameObject.FindGameObjectWithTag("TagChroma").GetComponent<UiChroma>();
		// VIDEOCAPTURE = GameObject.FindGameObjectWithTag("TagVideoCapture").GetComponent<VideoCaptureExample>();
		ChromaVid.gameObject.SetActive(false);
		CanvasAnim = BgImage.GetComponent<Animator>();
		CameraAnim = GetComponent<Animator>();
		ButtonCamAnim = ButtonCam.GetComponent<Animator>();
		TextAnim = TimerWhite.gameObject.GetComponent<Animator>();
		//ButtonSelectFrameAnim = ButtonSelectFrame.GetComponent<Animator>();
		FramedPic.gameObject.SetActive(false);
		createLogo();
		createBg();
		videoPlayer = UiCanvasCapturedVideo.GetComponent<VideoPlayer>();
		CamCaptureAnimator = CamCaptureAnim.GetComponent<Animator>();
		// frame
		
		// -----
		//Screen.SetResolution(1080, 1920, true);
		BtnVidText.text = VideoSeconds + "s Video";
		
		StoreEncodedTexture = new Texture2D(256,256);

		// QR Link DropDown
		QRDropDown = QRLink.GetComponentInChildren<Dropdown>();

		// Para ma link yung value ng drop down
		if (QRDropDown != null){
			QRDropDown.onValueChanged.AddListener(QRLinkDropdown);
		}

		datasettings = new DataSettings();
		
		string pfat = System.IO.Directory.GetCurrentDirectory();
		string frame_pat = Path.Combine(pfat, "save_images_here");
		string pat = Path.Combine(frame_pat, "fotomokosettings.json");
		saveFilePath = pat;

		CollageImages = Collage4x4.GetComponentsInChildren<Image>();
		collagepath = Path.Combine(Directory.GetCurrentDirectory(), "CollageImages");

		LoadConfig();


		createFrame();
		LoadImages();
		
		resetSettings();

		onSaveLink();
	}

	public void LoadConfig(){
		if (File.Exists(saveFilePath)){
			Directory.CreateDirectory(collagepath);
			string loadSettingsData = File.ReadAllText(saveFilePath);
			datasettings = JsonUtility.FromJson<DataSettings>(loadSettingsData);

			CaptureTimer.value = datasettings.capturetimervalue;
			CamMirror.value = datasettings.cammirrorvalue;
			Collage.value = datasettings.collagevalue;
		}
		else{
			SaveConfig();
		}
	}

	public void SaveConfig(){
		string savedatasettings = JsonUtility.ToJson(datasettings);
        File.WriteAllText(saveFilePath, savedatasettings);
		
		Debug.Log("FOTOMOKO SETTINGS CONFIG");
        Debug.Log("Save file created at: " + saveFilePath + " Settings Data: "+ savedatasettings);

		Directory.CreateDirectory(collagepath);
	}
	
	void resetSettings(){
		TimerText.text = "";
		TextDebug.text = "test";
		UiButtons.gameObject.SetActive(true);
		//UiNoFrame.gameObject.SetActive(false);
		//animate
		CanvasAnim.SetBool("isUiOpen",false);
		
		//MainCam.GetComponent<Camera>().orthographicSize = 960;
		MainCam.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
	}
	
	
	// check/create logo
	void createLogo(){
		// string pfat = Application.dataPath;
		string pfat = System.IO.Directory.GetCurrentDirectory();
		string frame_pat = Path.Combine(pfat, "save_images_here");
		DirectoryInfo dir2 = Directory.CreateDirectory(frame_pat);
		string frame1 = Path.Combine(frame_pat, "logo.png");
		if (File.Exists(frame1)){
			Texture2D thisTexture = new Texture2D(1000, 1000, TextureFormat.RGB24, true);
			Image m_Image = LogoImage.GetComponent<Image>();
			byte[] bytes = File.ReadAllBytes(frame1);
			thisTexture.LoadImage(bytes);
			LogoImage.GetComponent<RawImage>().texture = thisTexture;
		}
	}
	
	// check/create frame
	void createFrame(){
		// string pfat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Fotomoko2");
		// string pfat = Application.dataPath;
		string pfat = System.IO.Directory.GetCurrentDirectory();
		string frame_pat = Path.Combine(pfat, "save_images_here");
		DirectoryInfo dir2 = Directory.CreateDirectory(frame_pat);
		string frame1 = Path.Combine(frame_pat, "frame001.png");
		if (File.Exists(frame1)){
			Texture2D thisTexture = new Texture2D(1080, 1920, TextureFormat.RGB24, true);
			Image m_Image = FramedPic.GetComponent<Image>();
			byte[] bytes = File.ReadAllBytes(frame1);
			thisTexture.LoadImage(bytes);
			FramedPic.GetComponent<RawImage>().texture = thisTexture;
			
			// if frame001 is there, auto show frame001
			FramedPic.gameObject.SetActive(true);
			FrameCount = 1;
			UiNoFrame.gameObject.SetActive(false);
		} else {
			UiNoFrame.gameObject.SetActive(true);
		}
	}
	
	void createBg(){
		// string pfat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Fotomoko2");
		// string pfat = Application.dataPath;
		string pfat = System.IO.Directory.GetCurrentDirectory();
		string frame_pat = Path.Combine(pfat, "save_images_here");
		DirectoryInfo dir2 = Directory.CreateDirectory(frame_pat);
		string frame1 = Path.Combine(frame_pat, "bg.png");
		if (File.Exists(frame1)){
			Texture2D thisTexture = new Texture2D(1080, 1920, TextureFormat.RGB24, true);
			Image m_Image = BgImage.GetComponent<Image>();
			byte[] bytes = File.ReadAllBytes(frame1);
			thisTexture.LoadImage(bytes);
			BgImage.GetComponent<RawImage>().texture = thisTexture;
		}
	}
	
	
	void selectFrame(int selectedFrame){
		string fullFilename = Picture_Paths[selectedFrame].ToString();
		if (File.Exists(fullFilename)){
			FramedPic.gameObject.SetActive(true);
			Texture2D thisTexture = new Texture2D(1080, 1920, TextureFormat.RGB24, true);
			Image m_Image = FramedPic.GetComponent<Image>();
			byte[] bytes = File.ReadAllBytes(fullFilename);
			thisTexture.LoadImage(bytes);
			FramedPic.GetComponent<RawImage>().texture = thisTexture;
			UiNoFrame.gameObject.SetActive(false);
		}else{
			FramedPic.GetComponent<RawImage>().texture = null;
			FramedPic.gameObject.SetActive(false);
			UiNoFrame.gameObject.SetActive(true);
		}
	}
	
	
	// --------------
	
	public void LoadImages()
	{
		// string pfat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Fotomoko2");
		// string pfat = Application.dataPath;
		string pfat = System.IO.Directory.GetCurrentDirectory();
		string frame_pat = Path.Combine(pfat, "save_images_here");
		string filename = @"frame";
		string fileSuffix = @".png";
		
		for (int i=0; i < 30; i++){
			string indexSuffix = "";
			float logIdx = Mathf.Log10(i+1);
			if (logIdx < 1.0){
				indexSuffix += "00";
			} 
			else if (logIdx < 2.0){
				indexSuffix += "0";
			}
			indexSuffix += (i+1);

			string filename2 = filename + indexSuffix + fileSuffix;
			string fullFilename = Path.Combine(frame_pat,filename2);
		    if (File.Exists(fullFilename)){
				FrameCountMax += 1;
				Picture_Paths.Add(fullFilename);
		    }
		}
	}
	
	// ======================== selection: photo or vid ========================
	
	// button camera photo
	public void onSelectCameraButton(){ 
		// ButtonCam.GetComponent<Image>().sprite = SpriteCam;
		IsGreenScreen = false;
		Vector2 posi = SelectCamButtons.GetComponent<RectTransform>().anchoredPosition;
		posi.x = 0f;
		SelectCamButtons.GetComponent<RectTransform>().anchoredPosition = posi;
		// GameObject child3 = originalGameObject.transform.GetChild(2).gameObject;
		// TextMeshProUGUI thisButton = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		// thisButton.color = Color.yellow;
		// LastButtonSelected = EventSystem.current.currentSelectedGameObject;
		// Debug.Log(LastButtonSelected);
	}
	
	// button 5s video
	// public void onSelectVideoButton(){
		// // isVid = true;
		// ButtonCam.GetComponent<Image>().sprite = SpriteVid;
	// }
	
	public void onSelectGreenScreenButton(){ 
		IsGreenScreen = true;
		// ButtonCam.GetComponent<Image>().sprite = SpriteCam;
		Vector2 posi = SelectCamButtons.GetComponent<RectTransform>().anchoredPosition;
		posi.x = -220f;
		SelectCamButtons.GetComponent<RectTransform>().anchoredPosition = posi;
	}
	
	// public void onSelectAllowNoFrameButton(){ 
		// allowEmptyFrame = true;
		// // ButtonCam.GetComponent<Image>().sprite = SpriteCam;
	// }
	
	
	// ======================== on click ========================
	public void onCamClick(){
		onButton();
	}
	
	void onButton(){
		ButtonCamAnim.SetTrigger("onCamClickedFinished");
		UiButtons.gameObject.SetActive(false);
		TextAnim.SetTrigger("TimerReady");
		TextAnim.SetTrigger("TimerStart");
		// CamCaptureAnimator.SetTrigger("CamReady");
		StartCoroutine(onTime());
	}
	
	// button green screen
	// green screen - wala pa to pero gumagana basta i-call lang
	public void onCameraButtonClick(){
		UiButtons.gameObject.SetActive(false);
		UICHROMA.onPlayVidStart(chroma_number); // calling to UiChroma.cs
		TimerText.text  = "READY";
	}
	
	
	// start timer after mag click ng buttons
	// call on UIChroma.cs
	public void onTimerStart(){
		TextAnim.SetTrigger("TimerStart");
		//StartCoroutine(onTime());
	}
	
	IEnumerator onTime(){
		if (IsGreenScreen == true){
			UICHROMA.onPlayVidStart(chroma_number); // calling to UiChroma
		}

		datasettings.capturetimervalue = CaptureTimer.value;

		if (datasettings.capturetimervalue == 0){
			TimerText.text = "5";
			yield return new WaitForSeconds(1);
			TimerText.text = "4";
			yield return new WaitForSeconds(1);
			TimerText.text = "3";
			yield return new WaitForSeconds(1);
			TimerText.text = "2";
			yield return new WaitForSeconds(1);
			TimerText.text = "1";
			yield return new WaitForSeconds(1);
			TimerText.text = "";
		}
		else{
			TimerText.text = "3";
			yield return new WaitForSeconds(1);
			TimerText.text = "2";
			yield return new WaitForSeconds(1);
			TimerText.text = "1";
			yield return new WaitForSeconds(1);
			TimerText.text = "";
		}

		// if (IsGreenScreen == true){
			// UICHROMA.onPlayVidPause(); // calling to UiChroma
		// }
		// if (isVid == true){
			// VIDEOCAPTURE.StartVideoCaptureTest();
		// }
		// onCamCapture1();
		CaptureFunction();
		
		
	}

	public void CaptureFunction(){
		datasettings.collagevalue = Collage.value;

		// 1x1
		if (datasettings.collagevalue == 0){
			onCamCapture1();
		} //4x4
		else if (datasettings.collagevalue == 1){
			// BgImage.gameObject.SetActive(false);
			StartCoroutine(CaptureRoutine());
			count = 4;
			// WebCamDuplicateCapture("1");
			// TextAnim.SetTrigger("CamCapture");

			// for (int i = 0; i < count; i++){
			// 	TextAnim.SetTrigger("CamCapture");
			// }
			
			// string num = count.ToString();
			// WebCamDuplicateCapture(num);
			
			// onCamCapture1();
		}
	}

	private IEnumerator CaptureRoutine()
	{
		count = 0; // Reset count if needed
		WebCamDuplicateCapture("1");
		// CamCaptureAnimator.SetTrigger("CamReady");
		yield return StartCoroutine(onTimeMultiple()); // Wait for all captures
		// CollageLayout4x4(); // Build collage after all captures
	}

	IEnumerator onTimeMultiple()
	{
		int numCaptures = 3;
		for (int i = 0; i < numCaptures; i++)
		{
			// Play capture animation and wait
			// CanvasAnim.Play("CollageCapture");
			// TextAnim.SetTrigger("TimerReady");
			// CamCaptureAnimator.SetTrigger("CamCapture");
			TextAnim.SetTrigger("CamCapture");
			yield return new WaitForSeconds(1f); // Adjust based on animation length

			// Determine timer duration from settings
			int timerDuration = (datasettings.capturetimervalue == 0) ? 5 : 3;

			// Timer countdown
			for (int j = timerDuration; j > 0; j--)
			{
				TimerText.text = j.ToString();
				yield return new WaitForSeconds(1f);
			}
			TimerText.text = ""; // Clear timer

			// Capture image
			string num = (i + 2).ToString();
			WebCamDuplicateCapture(num);

			// yield return new WaitForSeconds(0.5f); // Brief pause between captures
		}
	}


	public void CaptureTimerValue(){
		datasettings.capturetimervalue = CaptureTimer.value;
		Debug.Log("Saved Capture Timer Value: " + datasettings.capturetimervalue);
		SaveConfig();
	}
	
	// tinawag after ng onTimerStart, nag cacall sa animation
	public void onCamCapture1(){
		if (isCam == true) {
			StartCoroutine(onCamCapture());
		}
		// else if (isVid == true){
			// //StartCoroutine(onVidCapture());
		// }
		// if (IsGreenScreen == true){
			// UICHROMA.onPlayVidResume(); // calling to UiChroma
			// StartCoroutine(onChromaEnd());
		// }
	}
	
	IEnumerator onChromaEnd(){
		yield return new WaitForSeconds(1);
		UICHROMA.onPlayVidStop(); // calling to UiChroma
	}
	
	// ======================== photo ========================
	
	IEnumerator onCamCapture()
    {
		yield return frameEnd;
		
		// ------------------ file path -------------------
		// Save inside the path
		var dattim = System.DateTime.Now;
		string dat = dattim.ToString("yyyyMMdd");
		string tim2 = dattim.ToString("HHmmss");
		finaldate = dattim.ToString(dat + "_" + tim2);
		
		string pic_name = finaldate + ".png";
		string pfat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Fotomoko2");
		string folder_pat = Path.Combine(pfat, folder_name);
		string desktop_path = Path.Combine(folder_pat, pic_name);
		
		DirectoryInfo dir = Directory.CreateDirectory(folder_pat);

		MirrorCamDropDown();
		onMirrorCam(); // Mirror upon last second timer
		yield return new WaitForEndOfFrame();
		
		ScreenCapture.CaptureScreenshot(desktop_path);

		TextDebug.text = desktop_path;
		
		outside_file_path = desktop_path;
		
		// ------------------------------------------------
		
		// showcaptured
		StartCoroutine(screenShot(Screen.width,Screen.height,finaldate));
		
		yield return frameEnd;
	
		onUiCanvas1();
    }


	public void onUiCanvas1(){
		//animate
		CanvasAnim.SetTrigger("OnCapture");
		CanvasAnim.SetBool("isUiOpen",true);

		RawImageForVid.gameObject.SetActive(false);
		//videoPlayer.Stop();

		// Kuha bagong screenshot and gawing print.png
		string desktop_path2 = Path.Combine(Environment.CurrentDirectory, "print.png");
		ScreenCapture.CaptureScreenshot(desktop_path2);
		// Print.gameObject.SetActive(printbutton); // Visibility ng print button depending on the config in settings
		// Debug.Log("Fotomoko.cs Printbutton stat: " + printbutton);
	}

	public void MirrorCamDropDown(){
		datasettings.cammirrorvalue = CamMirror.value;
		
		if (datasettings.cammirrorvalue == 0){ // Hindi dapat mag mirror
			isMirror = true;
		}
		else{
			isMirror = false; // Mirror
		}

		// SaveConfig();
	}

	public void MirrorCamDropDownSave(){
		SaveConfig();
	}
	
	// public void onMirrorCam(){
	// 	if (isMirror == false) {
	// 		isMirror = true;
	// 		// ButtonMirrorCamCheck.gameObject.SetActive(true);
	// 		// WebCam_Texture.localScale.y = 1080;
	// 		var sc = WebCam_Texture.transform.localScale;
	// 		var rot = WebCam_Texture.transform.localRotation;
	// 		if (rot.z == 0){
	// 			sc.x = -1920;
	// 			sc.y = 1080;
	// 		}else{
	// 			sc.x = 1920;
	// 			sc.y = -1080;
	// 		}
			
	// 		WebCam_Texture.transform.localScale = sc;
	// 	}else{
	// 		isMirror = false;
	// 		// ButtonMirrorCamCheck.gameObject.SetActive(false);
	// 		//WebCam_Texture.localScale.y = -1080;
	// 		var sc = WebCam_Texture.transform.localScale;
	// 		sc.x = 1920;
	// 		sc.y = 1080;
	// 		WebCam_Texture.transform.localScale = sc;
	// 	}
	// }

	public void onMirrorCam(){

		if (isMirror == false) {
			isMirror = true;
			// Get the transform and its current scale.
			Transform camTransform = WebCam_Texture.transform;
			Transform camTransform1 = WebCamDuplicate.transform;
			Vector3 sc = camTransform.localScale;
			Vector3 sc1 = camTransform1.localScale;
			
			// Use localEulerAngles to reliably check the rotation around the z-axis.
			float zAngle = camTransform.localEulerAngles.z;
			
			// If the z rotation is approximately 0, flip the x axis.
			if (Mathf.Approximately(zAngle, 0f)) {
				sc.x = (sc.x > 0) ? -Mathf.Abs(sc.x) : Mathf.Abs(sc.x);
				sc1.x = (sc1.x > 0) ? -Mathf.Abs(sc.x) : Mathf.Abs(sc1.x);
				// Ensure y remains positive so it doesn't flip vertically.
				sc.y = Mathf.Abs(sc.y);
				sc1.y = Mathf.Abs(sc1.y);
			} 
			// Otherwise, flip the y axis.
			else {
				sc.y = (sc.y > 0) ? -Mathf.Abs(sc.y) : Mathf.Abs(sc.y);
				sc1.y = (sc1.y > 0) ? -Mathf.Abs(sc1.y) : Mathf.Abs(sc1.y);
				// Ensure x remains positive so it doesn't flip horizontally.
				sc.x = Mathf.Abs(sc.x);
				sc1.x = Mathf.Abs(sc1.x);
			}
			
			// Apply the modified scale back to the transform.
			camTransform.localScale = sc;
			camTransform1.localScale = sc1;
		}
		else{
			
			MirrorCamReset();
		}
		
	}


	public void MirrorCamReset(){
		var sc = WebCam_Texture.transform.localScale;
		var sc1 = WebCamDuplicate.transform.localScale;
		sc.x = 1920;
		sc.y = 1080;
		sc1.x = 1920;
		sc1.y = 1080;
		WebCam_Texture.transform.localScale = sc;
		WebCamDuplicate.transform.localScale = sc1;
	}

	public void CollageFunction(){
		webcamduplicate = WebCamDuplicate.GetComponent<Animator>();
		CollageLayoutAnimation = CollageLayout.GetComponent<Animator>();
		datasettings.collagevalue = Collage.value;

		WebCam_Texture.SetActive(true);
		WebCamDuplicate.SetActive(true);

		// 1x1
		if (datasettings.collagevalue == 0){
			webcameduplicateanimation = "CollageDuplicateDisableAnimation";
			CollageLayouts = "CollageLayout";
			CollageLayoutAnimation.Play(CollageLayouts);

		}
		// 4x4
		else if (datasettings.collagevalue == 1){
			webcameduplicateanimation = "CollageDuplicateAnimation";
			CollageLayouts = "Collage4x4";
			//CollageLayout4x4();
		}
		else{
			webcameduplicateanimation = "CollageDuplicateInitialAnimation";
		}
		// webcamduplicate.Play(webcameduplicateanimation);
		SaveConfig();
	}

	public void CollageLayout4x4(){
		CollageImages[0].sprite = LoadSpriteFromPath(Path.Combine(collagepath, "Picture1.png"));
		CollageImages[1].sprite = LoadSpriteFromPath(Path.Combine(collagepath, "Picture2.png"));
		CollageImages[2].sprite = LoadSpriteFromPath(Path.Combine(collagepath, "Picture3.png"));
		CollageImages[3].sprite = LoadSpriteFromPath(Path.Combine(collagepath, "Picture4.png"));

		// CollageLayoutAnimation.Play(CollageLayouts);
		// WebCam_Texture.SetActive(false); // Disable Main Cam
		// WebCamDuplicate.SetActive(false); // Disable Duplicate Cam
	}

	Sprite LoadSpriteFromPath(string path)
    {
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError($"File at {path} not found.");
            return null;
        }
    }

	public void WebCamDuplicateCapture(string num){
        if (WebCamDuplicate == null)
        {
            Debug.LogError("WebCamDuplicate GameObject is not assigned.");
            return;
        }

        // Try to get the RawImage or Renderer component from WebCamDuplicate
        RawImage duplicateRawImage = WebCamDuplicate.GetComponent<RawImage>();
        Renderer duplicateRenderer = WebCamDuplicate.GetComponent<Renderer>();

        Texture textureToCapture = null;

        if (duplicateRawImage != null)
        {
            textureToCapture = duplicateRawImage.texture; // Use RawImage texture
        }
        else if (duplicateRenderer != null)
        {
            textureToCapture = duplicateRenderer.material.mainTexture; // Use Renderer texture
        }

        if (textureToCapture is WebCamTexture webcamTexture)
        {
			string filename = "Picture" + num + ".png";
			StartCoroutine(TakeScreenshot(webcamTexture, filename));

        }
        else
        {
            Debug.LogError("No WebCamTexture found on WebCamDuplicate.");
        }
	}
	
	private IEnumerator TakeScreenshot(WebCamTexture webcamTexture, string filename)
	{
		yield return new WaitForEndOfFrame(); // Ensure the frame is fully rendered

		// Create a RenderTexture
		RenderTexture renderTexture = new RenderTexture(webcamTexture.width, webcamTexture.height, 24);
		Graphics.Blit(webcamTexture, renderTexture);

		// Read pixels from RenderTexture
		Texture2D screenshot = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
		RenderTexture.active = renderTexture;
		screenshot.ReadPixels(new Rect(0, 0, webcamTexture.width, webcamTexture.height), 0, 0);
		screenshot.Apply();

		// Rotate the image by 90 degrees
    	Texture2D rotatedScreenshot = RotateTexture(screenshot, true); // true = rotate clockwise

		// Define file path
		string filePath = Path.Combine(collagepath, filename);
		
		// Save the screenshot
		byte[] bytes = rotatedScreenshot.EncodeToPNG();
		File.WriteAllBytes(filePath, bytes);
		Debug.Log("Screenshot saved at: " + filePath);

		// Cleanup
		RenderTexture.active = null;
		renderTexture.Release();
		Destroy(renderTexture);
		Destroy(screenshot);
	}

	private Texture2D RotateTexture(Texture2D originalTexture, bool counterClockwise)
	{
		int width = originalTexture.width;
		int height = originalTexture.height;
		Texture2D rotatedTexture = new Texture2D(height, width); // Swap width & height

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (counterClockwise) // Rotate Counterclockwise 90°
				{
					rotatedTexture.SetPixel(height - 1 - y, x, originalTexture.GetPixel(x, y));
				}
				else // Rotate Clockwise 90°
				{
					rotatedTexture.SetPixel(y, width - 1 - x, originalTexture.GetPixel(x, y));
				}
			}
		}

		rotatedTexture.Apply();
		return rotatedTexture;
	}



	// ======================== video ========================
	
	// IEnumerator onVidCapture()
        // {
			// if (isFrame == true){
				// //FramedPic.gameObject.SetActive(true);
				// ChromaVid.gameObject.SetActive(true);
				// ChromaVid.Play();
			// }else{
				// UiNoFrame.gameObject.SetActive(true);
			// }

			// yield return frameEnd;
			// // ------------------ file path -------------------
			// // Save inside the path
			// var dattim = System.DateTime.Now;
			// string dat = dattim.ToString("yyyyMMdd");
			// string tim2 = dattim.ToString("HHmmss");
			// finaldate = dattim.ToString(dat + "_" + tim2);
			
			// string pic_name = finaldate;
			// string pfat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Fotomoko2");
			// string folder_pat = Path.Combine(pfat, folder_name);
			// string folder_pat2 = Path.Combine(folder_pat, "Videos");
			// string desktop_path = Path.Combine(folder_pat2, pic_name);
			
			// Directory.CreateDirectory(folder_pat);
			// Directory.CreateDirectory(folder_pat2);

			// outside_file_path = desktop_path;
			// // ------------------------------------------------
			
            // var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            // m_RecorderController = new RecorderController(controllerSettings);

            // // Video
            // m_Settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            // m_Settings.Enabled = true;
			
            // m_Settings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
            // m_Settings.VideoBitRateMode = VideoBitrateMode.High;

            // m_Settings.ImageInputSettings = new GameViewInputSettings
            // {
                // OutputWidth = 720 , // 1080 default720
                // OutputHeight = 1280  // 1920 default1280
            // };
            // m_Settings.AudioInputSettings.PreserveAudio = m_RecordAudio;
            // m_Settings.OutputFile = desktop_path;
			
            // controllerSettings.AddRecorderSettings(m_Settings);
            // controllerSettings.SetRecordModeToManual();
            // controllerSettings.FrameRate = 30.0f;

            // RecorderOptions.VerboseMode = false;
			// yield return frameEnd;
            // m_RecorderController.PrepareRecording();
            // m_RecorderController.StartRecording();

			// // ------------------------------------------------
			
			
			// // ------------------------------------------------
			// StartCoroutine(VidCoroutine(desktop_path+".mp4"));
        // }
		
		 // IEnumerator VidCoroutine(string desktop_path)
		// {
			// yield return new WaitForSeconds(VideoSeconds); // 5sec default
			// m_RecorderController.StopRecording();
			// ChromaVid.gameObject.SetActive(false);
			// onUiCanvas1();
			// RawImageForVid.gameObject.SetActive(true);
			
			// yield return new WaitForSeconds(1);
			// FramedPic.gameObject.SetActive(false);
			// UiNoFrame.gameObject.SetActive(false);
			
			
			// videoPlayer.url = desktop_path;
			// yield return new WaitForSeconds(3);
			// videoPlayer.Play();
		// }
	
	// ======================== ========================
	
	
	WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
	IEnumerator screenShot(int width, int height, string finaldate)
	{
		yield return frameEnd;

		Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, true);
		texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		texture.LoadRawTextureData(texture.GetRawTextureData());
		texture.Apply();
		
		Sprite tempSprite = Sprite.Create(texture,new Rect(0,0,width, height),new Vector2(0.5f,0.5f));
		UiCanvasCapturedImage.gameObject.GetComponent<Image>().sprite = tempSprite;
	}
	
	private Color32 [] Encode(string textForEncoding, int width, int height)
	{
		BarcodeWriter writer = new BarcodeWriter{
			Format = BarcodeFormat.QR_CODE, 
			Options = new QrCodeEncodingOptions{
				Height = height,
				Width = width
			}
		};
		return writer.Write(textForEncoding);
	}
	
	private void onQr(string lin){
		Color32[] _convertPixelTotexture = Encode(lin, StoreEncodedTexture.width, StoreEncodedTexture.height);
		StoreEncodedTexture.SetPixels32(_convertPixelTotexture);
		StoreEncodedTexture.Apply();
		
		SupportQRCodeGenerator.texture = StoreEncodedTexture;
	}
	
	
	// ======================== ========================
	
	public void _on_generateQR(){
		//upload to server
		if (isCam == true){
			StartCoroutine(Upload2(finaldate + ".png"));
		}else{
			StartCoroutine(Upload3(finaldate + ".mp4"));
		}
		
		// isCam = false;
		// isVid = false;
	}
	
	// photo
	IEnumerator Upload2(string nam){
		WWWForm form = new WWWForm();
		form.AddBinaryData ("myimage", File.ReadAllBytes(outside_file_path), nam, "image/png");
		form.AddField ("foldername",folder_name);
		BtnGenQr.gameObject.SetActive(false);
		using (UnityWebRequest w = UnityWebRequest.Post(link,form))
		{
			//yield return w.SendWebRequest();
			yield return StartCoroutine(SendWebRequestWithTimeout(w, 30f));
			if (w.result != UnityWebRequest.Result.Success){
				//Debug.Log("Error -> "+w.error);
				TextQR.text = w.error;
				if (TextQR.text == "") {
					TextQR.text = "Slow Internet, Retry?";
				}
				BtnGenQr.gameObject.SetActive(true);
			}else{
				if (local_ip != ""){
					onQr(local_ip + "Images/"+folder_name+"/"+finaldate + ".png");
				}else{

					// // Old Link
					// onQr(link + "Images/"+folder_name+"/"+finaldate + ".png");

					// // New Link
					// onQr(link + "webfotomoko.html?image=Images/" + folder_name + "/" + finaldate + ".png");

					QRLinkDropdown(QRDropDown.value);
				}
				QrLoad.gameObject.SetActive(false);
			}
			w.Dispose();
		}
    }

	// QR Link Drop Down
	private void QRLinkDropdown(int index){
		if (index == 0){
			onQr(link + "webfotomoko.html?image=Images/" + folder_name + "/" + finaldate + ".png");
		}
		else{
			onQr(link + "Images/"+folder_name+"/"+finaldate + ".png");
		}
	}
	
	// video
	IEnumerator Upload3(string nam){
		WWWForm form = new WWWForm();
		form.AddBinaryData ("myimage", File.ReadAllBytes(outside_file_path+".mp4"), nam);
		form.AddField ("foldername",folder_name);
		BtnGenQr.gameObject.SetActive(false);
		using (UnityWebRequest w = UnityWebRequest.Post(link,form))
		{
			//yield return StartCoroutine(SendWebRequestWithTimeout(w, 5f));
			yield return w.SendWebRequest();
			if (w.result != UnityWebRequest.Result.Success){
				//Debug.Log("Error -> "+w.error);
				TextQR.text = w.error;
				if (TextQR.text == "") {
					TextQR.text = "Slow Internet, Retry?";
				}
				BtnGenQr.gameObject.SetActive(true);
			}else{
				if (local_ip != ""){
					onQr(local_ip + "Images/"+folder_name+"/"+finaldate + ".mp4");
				}else{
					onQr(link + "Images/"+folder_name+"/"+finaldate + ".mp4");
				}
				QrLoad.gameObject.SetActive(false);
			}
			w.Dispose();
		}
    }
	
	IEnumerator SendWebRequestWithTimeout(UnityWebRequest request, float timeout)
    {
        float timer = 0f;
        bool requestCompleted = false;

        request.SendWebRequest();
		Debug.Log("Sending web request...");
		
        while (!requestCompleted && timer < timeout)
        {
            if (request.isDone)
            {
                requestCompleted = true;
            }
            else
            {
                timer += Time.deltaTime;
				Debug.Log("Timer: " + timer);
                yield return null;
            }
			
        }
		
        if (requestCompleted)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Request completed successfully: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Request failed: " + request.error);
            }
        }
        else
        {
            Debug.LogError("Request timed out.");
        }
    }
	
	// exit preview
	public void onNextClick(){
		TextQR.text = "Scan Here!";
		QrLoad.gameObject.SetActive(true);
		BtnGenQr.gameObject.SetActive(true);
		
		// isCam = false;
		// isVid = false;
		
		Destroy(Imahe);
		MirrorCamReset(); // Reset Camera para di pa rin nakamirror pag binalik 
		resetSettings();
	}
	
	
	
	// ======================== Button Select Filter ========================
	
	public void onSelectFilterButton(){
		CameraAnim.SetBool("FilterOn",true);
	}
	public void onSelectFilterButtonOff(){
		CameraAnim.SetBool("FilterOn",false);
	}

	// ======================== frame click - frame or no frame ========================
	
	// public void onFrameClick(){
		// ButtonSelectFrameAnim.SetBool("SelectFrameB",true);
	// }
	
	public void onFrameClickR(){
		FrameCount += 1;
		if (FrameCount > FrameCountMax) {
			FrameCount = 0;
			if (allowEmptyFrame == false) {
				if (FrameCountMax > 0) {
					FrameCount = 1;
				}
			}
		}
		Debug.Log(allowEmptyFrame);
		Debug.Log(FrameCount);
		selectFrame(FrameCount);
		selectFrame(FrameCount);
		//ButtonSelectFrameAnim.SetBool("SelectFrameB",true);
	}
	public void onFrameClickL(){
		FrameCount -= 1;
		//Debug.Log(FrameCount);
		if (FrameCount < 0) {
			FrameCount = FrameCountMax;
		} else if (FrameCount == 0){
			if (allowEmptyFrame == false) {
				if (FrameCountMax > 0) {
					FrameCount = FrameCountMax;
				}
			}
		}
		Debug.Log(allowEmptyFrame);
		Debug.Log(FrameCount);
		selectFrame(FrameCount);
	}
	// ======================== click frame or no frame ========================
	
	// button camera photo
	// public void onWithFrameButton(){ 
		// //isFrame = true;
		// onButton1();
		// BtnFrameText.text = "w/ Frame";
	// }
	
	// // button 5s video
	// public void onNoFrameButton(){
		// //isFrame = false;
		// onButton1();
		// BtnFrameText.text = "No Frame";
	// }
	
	// void onButton1(){
		// ButtonSelectFrameAnim.SetBool("SelectFrameB",false);
	// }


	// ======================== ========================
	// settings - Save Folder Name
	public void onSaveFolderName(){
		if (FolderNameInput.text == "") {
			FolderNameInput.text = folder_name_default;
		}
		folder_name = FolderNameInput.text;
		Debug.Log(folder_name);
		WebCam_Texture.GetComponent<WebCamTex>().onSaveFolderName(folder_name);
	}
	public void onLoadFolderName(string fnam){ // from WbCamTex.cs
		if (fnam == "") {
			fnam = folder_name_default;
			FolderNameInput.text = folder_name_default;
		}
		folder_name = fnam; 
	}
	
	public void onSaveLink(){
		if (LinkInput.text == "") {
			LinkInput.text = link_default;
		}
		link = LinkInput.text;
		Debug.Log("Link: " + link);
		WebCam_Texture.GetComponent<WebCamTex>().onSaveLink(link);
	}
	public void onLoadLink(string li){ // from WbCamTex.cs
		if (li == "") {
			li = link_default;
			LinkInput.text = link_default;
		}
		link = li; 
	}
	
	public void onSaveLocalIp(){
		local_ip = LocalIpInput.text;
		Debug.Log(local_ip);
		WebCam_Texture.GetComponent<WebCamTex>().onSaveLocalIp(local_ip);
	}
	public void onLoadLocalIp(string lip){ // from WbCamTex.cs
		local_ip = lip; 
		LocalIpInput.text = lip;
	}
	
	// ======================== ========================
	
	
}
