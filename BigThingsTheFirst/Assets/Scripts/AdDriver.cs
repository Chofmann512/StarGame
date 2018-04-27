using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;//https://developers.google.com/admob/unity/start

public class AdDriver : MonoBehaviour {

	public static AdDriver Instance{ set; get;}

	public int loadCount;
	public int interstitialInterval;//Play an interstitial after every 'x' amount of games
	//Android app ids
	public string appIdAndroid;
	public string bannerIdAndroid;
	public string interstitialIdAndroid;
	//IOs app ids
	public string appIdIOs;
	public string bannerIdIOs;
	public string interstitialIdIOs;

	private BannerView bannerView;//From GoogleMobileAds.Api
	private InterstitialAd interstitial;

	void Awake () {
		Instance = this;
		DontDestroyOnLoad (gameObject);

		loadCount = 2;
		#if UNITY_EDITOR
			return;
		#elif UNITY_ANDROID
			MobileAds.Initialize(appIdAndroid);
		#elif UNITY_IOS
			MobileAds.Initialize(appIdIOs);
		#else
		Debug.Log("Unsupported platform")
		return;
		#endif

		//request a banner
		this.RequestBannerAd();
	}
	
	private void RequestBannerAd(){
		//Create a BannerView
		#if UNITY_ANDROID
			string id = bannerIdAndroid;
		#elif UNITY_IOS
			string id = bannerIdIOs;
		#else
			Debug.Log("Unsupported platform");
			return;
		#endif

		bannerView = new BannerView (id, AdSize.Banner, AdPosition.Bottom);

		//Request an ad
		AdRequest req = new AdRequest.Builder().Build();

		//Load the banner with the request
		bannerView.LoadAd(req);
	}

	public void RequestInterstitialAd(){

		#if UNITY_ANDROID
		string id = interstitialIdAndroid;
		#elif UNITY_IOS
		string id = interstitialIdIOs;
		#else
		Debug.Log("Unsupported platform");
		return;
		#endif

		//Initialize an insterstitial ad
		interstitial = new InterstitialAd (id);
		//Create an empty ad request
		AdRequest req = new AdRequest.Builder ().Build ();
		//Load the interstitial using the request, THIS IS A ONE TIME USE,
		//then a new InterstitialAd object must be created
		interstitial.LoadAd (req);
	}

	//public IEnumerator ShowInterstitial(float delay){
	public void ShowInterstitial(){
		//yield return new WaitForSeconds (delay);
		bannerView.Destroy ();//Destroy the current banner ad
		interstitial.Show ();
		interstitial.OnAdClosed += HandleOnAdClosed;
		//yield return null;
	}

	private void HandleOnAdClosed(object sender, EventArgs args){
		RequestBannerAd ();//Build a new banner ad
		interstitial.Destroy ();
		return;
	}
}
