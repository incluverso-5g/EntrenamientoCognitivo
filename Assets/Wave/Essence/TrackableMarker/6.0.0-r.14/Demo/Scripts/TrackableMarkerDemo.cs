using System;
using UnityEngine;
using UnityEngine.UI;
using Wave.Native;

namespace Wave.Essence.TrackableMarker.Sample
{
	public class TrackableMarkerDemo : MonoBehaviour
	{
		[SerializeField] private TrackableMarkerController trackableMarkerController;
		[SerializeField] private PassthroughHelper passthroughHelper;
		[SerializeField] private GameObject markerPrefab;
		[SerializeField] private Text observerStateText;
		[SerializeField] private Button btnStartObserver;
		[SerializeField] private Button btnStopObserver;
		[SerializeField] private Button btnClearAllObjects;
		[SerializeField] private Button btnStartDetecting;
		[SerializeField] private Button btnStopDetecting;
		[SerializeField] private Button btnStartTracking;
		[SerializeField] private Button btnStopTracking;
		[SerializeField] private Button btnClearTrackableMarkers;

		private MarkerObserverHelper markerObserverHelper = null;

		[SerializeField] private GameObject leftController = null, rightController = null;

		private const string LOG_TAG = "TrackableMarkerDemo";

		private void OnEnable()
		{
			if (markerObserverHelper == null)
			{
				markerObserverHelper = new MarkerObserverHelper(trackableMarkerController, markerPrefab);
			}

			if (markerObserverHelper != null)
			{
				markerObserverHelper.OnEnable();

				trackableMarkerController.OnGetMarkerObserverState += OnMarkerObserverStateUpdate;
			}

			if (btnStartObserver != null) btnStartObserver.onClick.AddListener(OnStartObserver);
			if (btnStopObserver != null) btnStopObserver.onClick.AddListener(OnStopObserver);
			if (btnClearAllObjects != null) btnClearAllObjects.onClick.AddListener(OnClearAllMarkerObjects);
			if (btnStartDetecting != null) btnStartDetecting.onClick.AddListener(OnStartDetecting);
			if (btnStopDetecting != null) btnStopDetecting.onClick.AddListener(OnStopDetecting);
			if (btnStartTracking != null) btnStartTracking.onClick.AddListener(OnStartTracking);
			if (btnStopTracking != null) btnStopTracking.onClick.AddListener(OnStopTracking);
			if (btnClearTrackableMarkers != null) btnClearTrackableMarkers.onClick.AddListener(OnClickClearTrackableMarkers);


			passthroughHelper.ShowPassthroughUnderlay(true);
		}
		private void OnDisable()
		{
			if (markerObserverHelper != null)
			{
				markerObserverHelper.OnDisable();

				trackableMarkerController.OnGetMarkerObserverState -= OnMarkerObserverStateUpdate;
			}

			if (btnStartObserver != null) btnStartObserver.onClick.RemoveListener(OnStartObserver);
			if (btnStopObserver != null) btnStopObserver.onClick.RemoveListener(OnStopObserver);
			if (btnClearAllObjects != null) btnClearAllObjects.onClick.RemoveListener(OnClearAllMarkerObjects);
			if (btnStartDetecting != null) btnStartDetecting.onClick.RemoveListener(OnStartDetecting);
			if (btnStopDetecting != null) btnStopDetecting.onClick.RemoveListener(OnStopDetecting);
			if (btnStartTracking != null) btnStartTracking.onClick.RemoveListener(OnStartTracking);
			if (btnStopTracking != null) btnStopTracking.onClick.RemoveListener(OnStopTracking);
			if (btnClearTrackableMarkers != null) btnClearTrackableMarkers.onClick.RemoveListener(OnClickClearTrackableMarkers);

			passthroughHelper.ShowPassthroughUnderlay(false);
		}

		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				markerObserverHelper.OnDisable();
			}
			else
			{
				markerObserverHelper.OnEnable();
			}
		}

		private void Update()
		{
			markerObserverHelper.OnUpdate();
			bool isO = markerObserverHelper.isMarkerObserverRunning;
			bool isD = markerObserverHelper.isMarkerDetecting;
			bool isT = markerObserverHelper.isMarkerTracking;

			if (btnClearAllObjects) btnClearAllObjects.interactable = !isO;
			if (btnStartObserver) btnStartObserver.interactable = !isO;
			if (btnStopObserver) btnStopObserver.interactable = isO;
			if (btnStartDetecting) btnStartDetecting.interactable = isO && !isD && !isT;
			if (btnStopDetecting) btnStopDetecting.interactable = isO && isD && !isT;
			if (btnStartTracking) btnStartTracking.interactable = isO && !isD && !isT;
			if (btnStopTracking) btnStopTracking.interactable = isO && !isD && isT;
			if (btnClearTrackableMarkers) btnClearTrackableMarkers.interactable = isO;
		}

		private void OnMarkerObserverStateUpdate(WVR_MarkerObserverTarget observerTarget, WVR_MarkerObserverState observerState, WVR_Result result)
		{
			if (result == WVR_Result.WVR_Success)
			{
				observerStateText.text = observerTarget.ToString() + " : " + observerState.ToString();
			}
		}


		public void OnStartObserver()
		{
			Debug.Log("OnStartObserver");
			markerObserverHelper.OnEnable();
			observerStateText.text = "Start Observing";
		}

		public void OnStopObserver()
		{
			Debug.Log("OnStopObserver");
			markerObserverHelper.OnDisable();
			observerStateText.text = "Stop Observing";
		}

		public void OnClearAllMarkerObjects()
		{
			Debug.Log("OnClearAllMarkerObjects");
			observerStateText.text = "Clear all marker objects";
			markerObserverHelper.HandleDestroyMarkerObjects();
		}

		public void OnStartDetecting()
		{
			Debug.Log("OnStartDetecting");
			observerStateText.text = "OnStartDetecting";
			markerObserverHelper.HandleSwitchDetectionMode(true);
		}

		public void OnStopDetecting()
		{
			Debug.Log("OnStopDetecting");
			observerStateText.text = "OnStopDetecting";
			markerObserverHelper.HandleSwitchDetectionMode(false);
		}

		public void OnStartTracking()
		{
			Debug.Log("OnStartTracking");
			observerStateText.text = "OnStartTracking";
			markerObserverHelper.HandleSwitchTrackingMode(true);
		}

		public void OnStopTracking()
		{
			Debug.Log("OnStopTracking");
			observerStateText.text = "OnStopTracking";
			markerObserverHelper.HandleSwitchTrackingMode(false);
		}

		public void OnClickClearTrackableMarkers()
		{
			Debug.Log("OnClickClearTrackableMarkers");
			observerStateText.text = "OnClickClearTrackableMarkers";
			markerObserverHelper.HandleClearTrackableMarkers();
		}

		private static class ButtonFacade
		{
			public static bool AButtonPressed =>
				WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A);
			public static bool BButtonPressed =>
				WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_B);
			public static bool XButtonPressed =>
				WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_X);
			public static bool YButtonPressed =>
				WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Y);
		}
	}
}
