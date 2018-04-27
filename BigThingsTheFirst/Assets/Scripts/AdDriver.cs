using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;//https://developers.google.com/admob/unity/start

public class AdDriver : MonoBehaviour {

	public static AdDriver Instance{ set; get;}

	//Android app ids
	public string appIdAndroid;
	public string bannerIdAndroid;
	public string interstitialIdAndroid;
	//IOs app ids
	public string appIdIOs;
	public string bannerIdIOs;
	public string interstitialIdIOs;

	private BannerView bannerView;//From GoogleMobileAds.Api

	void Awake () {
		Instance = this;
		DontDestroyOnLoad (gameObject);
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
}
