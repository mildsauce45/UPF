﻿using FirstWave.Unity.Core.Utilities;
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

		public GUIStyle GetButtonStyle(Texture2D background, Texture2D hoverBackground)
		{
            GUIStyle style;
            if (background == null)
                style = GUI.skin.button;
            else
            {
                style = new GUIStyle();

                style.normal.background = background;
                style.hover.background = hoverBackground ?? background;
                style.alignment = TextAnchor.MiddleCenter;
            }

			return style;
		}
	}
}
