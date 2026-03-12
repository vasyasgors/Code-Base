using UnityEngine;

namespace CodeBase
{
	public class GUIFPS : MonoBehaviour
	{
		private float deltaTime;

		void OnGUI()
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			float fps = 1.0f / deltaTime;
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.fontSize = 25;

			GUI.Box(new Rect(0, 0, 120, 30), " ");
			GUI.Label(new Rect(10, 0, 120, 30), "FPS: " + Mathf.Ceil(fps).ToString(), style);
		}

	}
}