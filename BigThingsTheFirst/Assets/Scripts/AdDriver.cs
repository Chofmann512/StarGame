using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;//https://developers.google.com/admob/unity/start

public class AdDriver : MonoBehaviour {

	public static AdDriver Instance{ set; get;}

	public string appId;//Android app id, need another one for IOs
	public string bannerId;
	public string interstitialId;

	private BannerView bannerView;//From GoogleMobileAds.Api

	void Awake () {
		Instance = this;
		DontDestroyOnLoad (gameObject);

		//#if UNITY_EDITOR
		//Debug.Log("Failed to initialize MobileAds. Can not use Ads in editor.");
		//#elif UNITY_ANDROID
		MobileAds.Initialize(appId);


		//request a banner
		this.RequestBannerAd();
	}
	
	private void RequestBannerAd(){
		//Create a BannerView
		bannerView = new BannerView (bannerId, AdSize.Banner, AdPosition.Bottom);

		//Request an ad
		AdRequest req = new AdRequest.Builder().Build();

		//Load the banner with the request
		bannerView.LoadAd(req);
	}
}
