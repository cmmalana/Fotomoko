using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class UiChroma : MonoBehaviour
{
	
	// public VideoPlayer[] Vid_Start;
	// public VideoPlayer[] Vid_End;
	
	public VideoPlayer[] Vid;
	
	int Int_Video;

	// call on Fotomoko.cs
	public void onPlayVidStart(int i){
		Int_Video = i;
		
		Vid[i].gameObject.SetActive(true);
		Vid[i].Play();
	}
	
	public void onPlayVidPause(){
		Vid[Int_Video].Pause();
	}
	public void onPlayVidResume(){
		Vid[Int_Video].Play();
	}

	public void onPlayVidStop(){
		Vid[Int_Video].Stop();
		Vid[Int_Video].gameObject.SetActive(false);
	}
}
