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
				ApplyFont(style, fontProperties);

			return style;
		}

		public GUIStyle GetButtonStyle(ButtonStyle bStyle)
		{
			GUIStyle style;
			if (bStyle == null || bStyle.Background == null)
			{
				style = GUI.skin.button;
				ApplyFont(style, fontProperties);
			}
			else
			{
				style = new GUIStyle();

				style.normal.background = bStyle.Background;
				style.hover.background = bStyle.HoverBackground;
				style.active.background = bStyle.PressedBackground;				

				var fp = bStyle.Font ?? fontProperties;

				if (fp != null)
					ApplyFont(style, fp);

				style.alignment = TextAnchor.MiddleCenter;
			}

			return style;
		}

        public GUIStyle GetTextBoxStyle(FontProperties font)
        {
            var style = GUI.skin.textField;

            var fp = font ?? fontProperties;

            if (fp != null)
                ApplyFont(style, fp);

            return style; 
        }

        private void ApplyFont(GUIStyle style, FontProperties fp)
		{
			style.normal.textColor = fp.fontColor;
			style.font = fp.font;
			style.fontSize = fp.fontSize;
			style.wordWrap = true;
		}
	}
}
