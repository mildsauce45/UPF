using UnityEngine;

namespace FirstWave.Unity.Core.Utilities
{
	public class TextDisplayTimer : Timer
	{
		public string FullText { get; private set; }

		public string Text
		{
			get { return FullText.Substring(0, GetNumCharactersToDisplay()); }
		}

		public bool IsFullTextDisplayed
		{
			get { return GetNumCharactersToDisplay() == FullText.Length; }
		}

		public override bool IsComplete
		{
			get { return IsFullTextDisplayed; }
		}

		private float characterDisplayPassedTime;

		public TextDisplayTimer(float characterDelay)
			: this(characterDelay, null)
		{
		}

		public TextDisplayTimer(float characterDelay, string text)
			: base(characterDelay)
		{
			FullText = text;

			characterDisplayPassedTime = 0f;
		}

		public override void Update()
		{
			if (!Enabled)
				return;

			characterDisplayPassedTime += Time.deltaTime;

			base.Update();
		}

		private int GetNumCharactersToDisplay()
		{
			int numCharacters = (int)(characterDisplayPassedTime / TimeSpan);

			return Mathf.Clamp(numCharacters, 0, FullText.Length);
		}
	}
}
