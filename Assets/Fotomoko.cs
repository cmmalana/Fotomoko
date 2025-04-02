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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SocialPlatforms;
// Video
// using System.Linq;
// using UnityEngine.Windows.WebCam;

public class DataSettings{
	public int capturetimervalue;
	public int cammirrorvalue;
	public int collagevalue;
	public bool isLocalIP;
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
	public Dropdown Collage;
	Animator frameanimation;
	bool isCollage = false;
	int collagecount;
	string collagepath;
	string collagepath1;
	public RawImage CollageLayout;
	Animator CollageLayoutAnim;
	Animator WebCamTextureAnim;
	private Image [] CollageImages;
	public Canvas Collage2PicsLayout;
	public Canvas Collage3PicsLayout;
	public Canvas Collage4PicsLayout;
	Animator fotomokologo;
	Image TimerTextImage;
	String CollageFramePath;
	public Image InstructionsPage;
	public Toggle LocalIPToggle;
	string PublicIPAdd;

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
		frameanimation = FramedPic.GetComponent<Animator>();
		CollageLayoutAnim = CollageLayout.GetComponent<Animator>();
		WebCamTextureAnim = WebCam_Texture.GetComponent<Animator>();
		// Collage4PicsLayout = CollageLayout.GetComponentInChildren<Canvas>();
		fotomokologo = UiNoFrame.GetComponent<Animator>();
		TimerTextImage = TimerWhite.GetComponent<Image>();
		InstructionsPage.gameObject.SetActive(false);
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
		
		string pfat = Directory.GetCurrentDirectory();
		string frame_pat = Path.Combine(pfat, "FotomokoConfigs");
		string pat = Path.Combine(frame_pat, "fotomokosettings.json");
		saveFilePath = pat;

		collagepath = Path.Combine(pfat, "CollagePictures");

		string configDirectory = Path.Combine(Directory.GetCurrentDirectory(), "FotomokoConfigs");

		if (!Directory.Exists(configDirectory))
		{
			Directory.CreateDirectory(configDirectory);
		}

		LoadConfig();

		CollageDropdownFunction();


		createFrame();
		LoadImages();
		
		resetSettings();

		onSaveLink();
	}

	public void LoadConfig(){

		CollageFramePath = Path.Combine(Directory.GetCurrentDirectory(), "CollageFrames");
		Directory.CreateDirectory(collagepath);
		Directory.CreateDirectory(CollageFramePath);
		StartCoroutine(CollageFolderFrame());

		if (File.Exists(saveFilePath)){
			string loadSettingsData = File.ReadAllText(saveFilePath);
			datasettings = JsonUtility.FromJson<DataSettings>(loadSettingsData);

			CaptureTimer.value = datasettings.capturetimervalue;
			CamMirror.value = datasettings.cammirrorvalue;
			Collage.value = datasettings.collagevalue;
			LocalIPToggle.isOn = datasettings.isLocalIP;

		}
		else{
			onLoadFolderName("folder_name"); // Initial Folder Name
			SaveConfig();
		}
	}

	IEnumerator CollageFolderFrame(){

		for (int i = 2; i <= 4; i++){
			collagepath1 = Path.Combine(CollageFramePath, "Collage" + i);
			Directory.CreateDirectory(collagepath1);
			yield return new WaitForEndOfFrame();
		}
	}

	public void SaveConfig(){
		string savedatasettings = JsonUtility.ToJson(datasettings);
        File.WriteAllText(saveFilePath, savedatasettings);
		Debug.Log("FOTOMOKO SETTINGS CONFIG");
        Debug.Log("Save file created at: " + saveFilePath + " Settings Data: "+ savedatasettings);
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
		// string frame_pat = Path.Combine(pfat, "save_images_here");

		string frame_pat;

		if (isCollage){
			// frame_pat = Path.Combine(pfat, "CollageFrames");
			frame_pat = Path.Combine(CollageFramePath, "Collage" + collagecount);
			Debug.Log("FRAME PATH: " + frame_pat);
		}
		else{
			frame_pat = Path.Combine(pfat, "save_images_here");
		}

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

	public void ResetCollagePos(){
		if (isCollage){
			RectTransform rect4 = Collage4PicsLayout.GetComponent<RectTransform>(); // for 4 pics
			rect4.offsetMin = new Vector2(rect4.offsetMin.x, 0); // bottom
			rect4.offsetMax = new Vector2(rect4.offsetMax.x, 0); // top
			rect4.localScale = new Vector3(1,1,1); // reset scale to 1
			// rect4.localScale = Vector3.one;
			// CollageLayoutAnim.Play("CollageAnimation4x4Pause");
			// CollageLayoutAnim.SetTrigger("CollageLayoutBack");
		}
	}

	public void InitialCollagePos(){

		// 4 pictures initial layout
		RectTransform rect4 = Collage4PicsLayout.GetComponent<RectTransform>(); // for 4 pics
		rect4.offsetMin = new Vector2(rect4.offsetMin.x, -90); // bottom
		rect4.offsetMax = new Vector2(rect4.offsetMax.x, -90); // top
		rect4.localScale = new Vector3(0.78f,0.78f,0.78f); // reset scale to 1
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
			InitialCollagePos();
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
			ResetCollagePos();
		}
	}
	
	
	// --------------
	
	public void LoadImages()
	{

		Picture_Paths.Clear();
		Picture_Paths.Add("frame0");
    	FrameCountMax = 0;

		// string pfat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Fotomoko2");
		// string pfat = Application.dataPath;
		string pfat = System.IO.Directory.GetCurrentDirectory();
		// string frame_pat = Path.Combine(pfat, "save_images_here");
		string frame_pat;

		if (isCollage){
			// frame_pat = Path.Combine(pfat, "CollageFrames");
			frame_pat = Path.Combine(CollageFramePath, "Collage" + collagecount);
		}
		else{
			frame_pat = Path.Combine(pfat, "save_images_here");
		}

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
		StartCoroutine(onTime());
	}
	
	// button green screen
	// green screen - wala pa to pero gumagana basta i-call lang
	public void onCameraButtonClick(){
		UiButtons.gameObject.SetActive(false);
		UICHROMA.onPlayVidStart(chroma_number); // calling to UiChroma.cs
		TimerText.text  = "READY";
	}
	
	
	// start timer after mab click ng buttons
	// call on UIChroma.cs
	public void onTimerStart(){
		TextAnim.SetTrigger("TimerStart");
		//StartCoroutine(onTime());
	}
	
	IEnumerator onTime(){

		CollageLayoutAnim.Play("CollageAnimationInitial");

		if (IsGreenScreen == true){
			UICHROMA.onPlayVidStart(chroma_number); // calling to UiChroma
		}

		// If for Collage
		if (isCollage){
			// CameraAnim.Play("CameraCollageAnim");
			// CameraAnim.SetTrigger("CollageStart");
			frameanimation.SetTrigger("FrameAnim");
			fotomokologo.SetTrigger("FotomokoLogoHide");
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
		// Not Collage Mode
		if (isCollage){
			// for (int i = 0; i < (collagecount-1); i++){
			// 	TextAnim.SetTrigger("TimerReady");
			// 	TextAnim.SetTrigger("TimerStart");
			// 	StartCoroutine(onTime1());
			// 	Debug.Log(i);
			// }
			StartCoroutine(onTime1());
			// isCollage = false;
		} // Collage Mode
		else {
			onCamCapture1();
		}

	}

	IEnumerator onTime1(){

		// Enable Mirror Function Even on Collage
		MirrorCamDropDown();
		onMirrorCam(); // Mirror upon last second timer
		yield return new WaitForEndOfFrame();

		string pic_name = "Picture" + 1 + ".png";
		string collageimage = Path.Combine(collagepath, pic_name);
		yield return new WaitForEndOfFrame();
		ScreenCapture.CaptureScreenshot(collageimage);
		Debug.Log(pic_name + " saved at " + collagepath);

		CollageLayoutAnim.SetTrigger("CameraCapture");
		// CollageLayoutAnim.Play("CameraCaptureAnimStart");
		yield return new WaitForEndOfFrame();

		MirrorCamReset(); // Reset 

		for (int i = 0; i < (collagecount-1); i++){

			TextAnim.Play("TimerTextOff");
			TextAnim.SetTrigger("TimerReady");
			TextAnim.SetTrigger("TimerStart");

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

			// Enable Mirror Function
			MirrorCamDropDown();
			onMirrorCam(); // Mirror upon last second timer
			yield return new WaitForEndOfFrame();

			pic_name = "Picture" + (i+2) + ".png";
			collageimage = Path.Combine(collagepath, pic_name);
			yield return new WaitForEndOfFrame();
			ScreenCapture.CaptureScreenshot(collageimage);

			Debug.Log(pic_name + " saved at " + collagepath);

			CollageLayoutAnim.SetTrigger("CameraCapture");
			yield return new WaitForEndOfFrame();

			MirrorCamReset(); // Reset 
		}

		fotomokologo.SetTrigger("FotomokoLogoReveal");

		// For Capturing of the collage
		StartCoroutine(onTime2());
	}

	IEnumerator onTime2(){
		frameanimation.SetTrigger("FrameBack");
		WebCamTextureAnim.SetTrigger("WebCamCollageAnim");		
		// CollageLayoutAnim.SetTrigger("Collage4x4");
		yield return new WaitForEndOfFrame();
		CollageLayoutPictures();
		// TextAnim.Play("TimerTextReady");
		TextAnim.Play("TimerTextOff");
		TextAnim.SetTrigger("TimerReady");
		TextAnim.SetTrigger("TimerStart");

		TimerText.text = "3";
		yield return new WaitForSeconds(1);
		TimerText.text = "2";
		yield return new WaitForSeconds(1);
		TimerText.text = "1";
		yield return new WaitForSeconds(1);
		TimerText.text = "";

		onCamCapture1();
	}

	IEnumerator LoadSpriteAsync(string path, Image targetImage) {
		string filePath = "file://" + path;
		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath)) {
			yield return uwr.SendWebRequest();
			if (uwr.result == UnityWebRequest.Result.Success) {
				Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
				// Resize texture if necessary
				targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
			}
		}
	}

	// Update CollageLayout4Pictures to use coroutine
	public void CollageLayoutPictures(){
		
		if (collagecount == 2){
			CollageImages = Collage2PicsLayout.GetComponentsInChildren<Image>();
			StartCoroutine(LoadCollageImagesAsync2());
		}
		else if (collagecount == 3){
			CollageImages = Collage3PicsLayout.GetComponentsInChildren<Image>();
			StartCoroutine(LoadCollageImagesAsync3());
		}
		else if (collagecount == 4){
			CollageImages = Collage4PicsLayout.GetComponentsInChildren<Image>();
			StartCoroutine(LoadCollageImagesAsync4());
		}
	}

	IEnumerator LoadCollageImagesAsync2(){
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture1.png"), CollageImages[0]));
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture2.png"), CollageImages[1]));
	}

	IEnumerator LoadCollageImagesAsync3(){
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture1.png"), CollageImages[0]));
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture2.png"), CollageImages[1]));
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture3.png"), CollageImages[2]));
	}

	IEnumerator LoadCollageImagesAsync4(){
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture1.png"), CollageImages[0]));
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture2.png"), CollageImages[1]));
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture3.png"), CollageImages[2]));
		yield return StartCoroutine(LoadSpriteAsync(Path.Combine(collagepath, "Picture4.png"), CollageImages[3]));
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
			Vector3 sc = camTransform.localScale;
			
			// Use localEulerAngles to reliably check the rotation around the z-axis.
			float zAngle = camTransform.localEulerAngles.z;
			
			// If the z rotation is approximately 0, flip the x axis.
			if (Mathf.Approximately(zAngle, 0f)) {
				sc.x = (sc.x > 0) ? -Mathf.Abs(sc.x) : Mathf.Abs(sc.x);
				// Ensure y remains positive so it doesn't flip vertically.
				sc.y = Mathf.Abs(sc.y);
			} 
			// Otherwise, flip the y axis.
			else {
				sc.y = (sc.y > 0) ? -Mathf.Abs(sc.y) : Mathf.Abs(sc.y);
				// Ensure x remains positive so it doesn't flip horizontally.
				sc.x = Mathf.Abs(sc.x);
			}
			
			// Apply the modified scale back to the transform.
			camTransform.localScale = sc;
		}
		else{
			
			MirrorCamReset();
		}
		
	}


	public void MirrorCamReset(){
		var sc = WebCam_Texture.transform.localScale;
		sc.x = 1920;
		sc.y = 1080;
		WebCam_Texture.transform.localScale = sc;
	}

	public void CollageDropdownFunction(){

		datasettings.collagevalue = Collage.value;

		if (datasettings.collagevalue == 0){
			isCollage = false;
			Collage2PicsLayout.gameObject.SetActive(false);
			Collage3PicsLayout.gameObject.SetActive(false);
			Collage4PicsLayout.gameObject.SetActive(false);
			InitialCollagePos();
		}
		else if (datasettings.collagevalue == 1){
			isCollage = true;
			Collage2PicsLayout.gameObject.SetActive(true);
			Collage3PicsLayout.gameObject.SetActive(false);
			Collage4PicsLayout.gameObject.SetActive(false);
			collagecount = 2;
			InitialCollagePos();
		}
		else if (datasettings.collagevalue == 2){
			isCollage = true;	
			Collage2PicsLayout.gameObject.SetActive(false);
			Collage3PicsLayout.gameObject.SetActive(true);
			Collage4PicsLayout.gameObject.SetActive(false);
			collagecount = 3;
			InitialCollagePos();
		}
		else if (datasettings.collagevalue == 3){
			isCollage = true;
			Collage2PicsLayout.gameObject.SetActive(false);
			Collage3PicsLayout.gameObject.SetActive(false);
			Collage4PicsLayout.gameObject.SetActive(true);
			collagecount = 4;
			InitialCollagePos();
		}
		else{
			isCollage = true;
			// collagecount = 4;
		}

		Debug.Log("COLLAGE DROPDOWN VALUE: " + (datasettings.collagevalue+1));

		createFrame();
		LoadImages();

		SaveConfig();

	}

	public void onShowInstructionsButton(){
		InstructionsPage.gameObject.SetActive(true);
	}

	public void onBackInstructionsButton(){
		InstructionsPage.gameObject.SetActive(false);
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

		if (datasettings.isLocalIP){
			link = "http://localhost/fotomoko/";
		}
		else{
			link = link_default;
		}

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
				// if (local_ip != ""){
				// 	print(local_ip + "Images/"+folder_name+"/"+finaldate + ".png");
				// 	onQr(local_ip + "Images/"+folder_name+"/"+finaldate + ".png");
				// }else{

				// 	// // Old Link
				// 	// onQr(link + "Images/"+folder_name+"/"+finaldate + ".png");

				// 	// // New Link
				// 	// onQr(link + "webfotomoko.html?image=Images/" + folder_name + "/" + finaldate + ".png");

				// 	QRLinkDropdown(QRDropDown.value);
				// }


				QRLinkDropdown(QRDropDown.value);

				QrLoad.gameObject.SetActive(false);
			}
			w.Dispose();
		}
    }

	public void localIPToggle(){
		if (LocalIPToggle.isOn){
			GetLocalIPAddress();
			print("IP ADDRESS: " + "http://" + PublicIPAdd + "/fotomoko/");
		}
		else{
			print("SERVER ADDRESS USE");
		}
		datasettings.isLocalIP = LocalIPToggle.isOn;
		print("LOCAL IP TOGGLE: " + datasettings.isLocalIP);
		SaveConfig();
	}

	string GetLocalIPAddress()
    {
        PublicIPAdd = "Not Found";
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                PublicIPAdd = ip.ToString();
                break;
            }
        }
        return PublicIPAdd;
    }

	// QR Link Drop Down
	private void QRLinkDropdown(int index){
		if (datasettings.isLocalIP){
			link = "http://" + PublicIPAdd + "/fotomoko/";
		}
		else{
			link = link_default;
		}

		if (index == 0){
			onQr(link + "webfotomoko.html?image=Images/" + folder_name + "/" + finaldate + ".png");
			print(link + "webfotomoko.html?image=Images/" + folder_name + "/" + finaldate + ".png");
		}
		else{
			onQr(link + "Images/"+ folder_name +"/"+ finaldate + ".png");
			print(link + "Images/"+ folder_name +"/"+ finaldate + ".png");
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
		
		if (isCollage){
			WebCamTextureAnim.Play("WebCamTextureBackAnim");
			fotomokologo.SetTrigger("FotomokoLogoInital");
			CollageLayoutAnim.Play("CollageAnimationInitial");
		}

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