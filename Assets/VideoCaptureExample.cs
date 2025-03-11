// using UnityEngine;
// using System.Collections;
// using System.Linq;
// using UnityEngine.Windows.WebCam;

// public class VideoCaptureExample : MonoBehaviour
// {
//     static readonly float MaxRecordingTime = 8.0f;

//     VideoCapture m_VideoCapture = null;
//     float m_stopRecordingTimer = float.MaxValue;
// 	bool isStopping = false;

//     // Use this for initialization
//     void Start()
//     {
//         //StartVideoCaptureTest();
// 		 Debug.Log(Application.persistentDataPath);
//     }

//     void Update()
//     {
//         if (m_VideoCapture == null || !m_VideoCapture.IsRecording)
//         {
//             return;
//         }

//         if (Time.time > m_stopRecordingTimer)
//         {
// 			if (isStopping == false ) {
// 				isStopping = true;
// 				m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
// 			}
//         }
//     }

//     public void StartVideoCaptureTest()
//     {
// 		isStopping = false;
//         Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
//         Debug.Log(cameraResolution);

//         float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
//         Debug.Log(cameraFramerate);

//         VideoCapture.CreateAsync(false, delegate(VideoCapture videoCapture)
//         {
//             if (videoCapture != null)
//             {
//                 m_VideoCapture = videoCapture;
//                 Debug.Log("Created VideoCapture Instance!");

//                 CameraParameters cameraParameters = new CameraParameters();
//                 cameraParameters.hologramOpacity = 0.0f;
//                 cameraParameters.frameRate = cameraFramerate;
//                 cameraParameters.cameraResolutionWidth = cameraResolution.width;
//                 cameraParameters.cameraResolutionHeight = cameraResolution.height;
//                 cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

//                 m_VideoCapture.StartVideoModeAsync(cameraParameters,
//                     VideoCapture.AudioState.ApplicationAndMicAudio,
//                     OnStartedVideoCaptureMode);
//             }
//             else
//             {
//                 Debug.LogError("Failed to create VideoCapture Instance!");
//             }
//         });
//     }

//     void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
//     {
//         Debug.Log("Started Video Capture Mode!");
//         string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
//         string filename = string.Format("TestVideo_{0}.mp4", timeStamp);
//         string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);
//         filepath = filepath.Replace("/", @"\");
//         m_VideoCapture.StartRecordingAsync(filepath, OnStartedRecordingVideo);
//     }

//     void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
//     {
//         Debug.Log("Stopped Video Capture Mode!");
// 		if (result.success){
// 			m_VideoCapture.Dispose();
// 			m_VideoCapture = null;
// 		} else {
// 			Debug.LogError("Error stopping123 video mode: " + result.resultType);
// 		}
//     }

//     void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
//     {
//         Debug.Log("Started Recording Video!");
//         m_stopRecordingTimer = Time.time + MaxRecordingTime;
//     }

//     void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
//     {
//         Debug.Log("Stopped Recording Video!");
//         m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
// 		if (result.success){
// 			m_VideoCapture.Dispose();
// 			m_VideoCapture = null;
// 		} else {
// 			Debug.LogError("Error stopping video mode: " + result.resultType);
// 		}
//     }
// }