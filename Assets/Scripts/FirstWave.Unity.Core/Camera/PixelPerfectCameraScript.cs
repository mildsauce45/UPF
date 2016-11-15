using UnityEngine;

namespace FirstWave.Unity.Core.Camera
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class PixelPerfectCameraScript : MonoBehaviour
	{
		public int pixelsPerUnit = 16;

		private int lastHeight;
		private int lastPPU;

		private UnityEngine.Camera attachedCamera;

		public UnityEngine.Camera Camera
		{
			get
			{
				if (attachedCamera == null)
					attachedCamera = GetComponent<UnityEngine.Camera>();

				return attachedCamera;
			}
		}

		void Update()
		{
			if (lastHeight == Screen.height && lastPPU == pixelsPerUnit)
				return;

			lastHeight = Screen.height;
			lastPPU = pixelsPerUnit;

			Camera.orthographicSize = Screen.height / (pixelsPerUnit * 2f);
		}
	}
}
