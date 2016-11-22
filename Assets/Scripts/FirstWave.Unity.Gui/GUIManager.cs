using FirstWave.Unity.Core.Utilities;
using FirstWave.Unity.Gui.Controls;
using UnityEngine;

namespace FirstWave.Unity.Gui
{
	public class GUIManager : SafeSingleton<GUIManager>
	{
		protected override string managerName
		{
			get { return "GuiManager"; }
		}

		public BorderTextures borderTextures;
		public FontProperties fontProperties;

		public GUIStyle GetMessageBoxStyle(FontProperties fontProperties)
		{
			var style = new GUIStyle();

			if (fontProperties == null)
				style.normal.textColor = Color.white;
			else
			{
				style.normal.textColor = fontProperties.fontColor;
				style.font = fontProperties.font;
				style.fontSize = fontProperties.fontSize;
				style.wordWrap = true;
			}

			return style;
		}

		public GUIStyle GetButtonStyle(ButtonStyle bStyle)
		{
			GUIStyle style;
			if (bStyle == null || bStyle.Background == null)
				style = GUI.skin.button;
			else
			{
				style = new GUIStyle();

				style.normal.background = bStyle.Background;
				style.hover.background = bStyle.HoverBackground;
				style.active.background = bStyle.PressedBackground;

				var font = bStyle.Font ?? fontProperties;

				if (fontProperties != null)
				{
					style.font = fontProperties.font;
					style.fontSize = fontProperties.fontSize;					
					style.normal.textColor = fontProperties.fontColor;
				}

				style.alignment = TextAnchor.MiddleCenter;
			}

			return style;
		}
	}
}
