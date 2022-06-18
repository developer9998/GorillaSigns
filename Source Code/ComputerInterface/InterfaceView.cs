using UnityEngine;
using ComputerInterface;
using ComputerInterface.ViewLib;
using Plugin = GorillaSigns.Main.Plugin;

namespace GorillaSigns.ComputerInterface
{
	public class InterfaceView : ComputerView
	{
		const string highlightColor = "977061ff";

		private readonly UISelectionHandler _selectionHandler;

		private bool showSign;
		private string enableDisableText = "Enabled";
		private string imageModeText = "Bilinear";

        private readonly string[] imageModeTexts = { "Point", "Bilinear", "Trilinear" };
        private readonly string[] enableDisableTexts = { "Disabled", "Enabled" };
        private readonly bool[] showedSigns = { false, true };

        public InterfaceView()
		{
			_selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
			_selectionHandler.MaxIdx = 3;
			_selectionHandler.OnSelected += OnEntrySelected;
			_selectionHandler.ConfigureSelectionIndicator($"<color=#{highlightColor}>></color> ", "", "  ", "");
		}

		public override void OnShow(object[] args)
		{
			base.OnShow(args);
			UpdateScreen();
		}

		void UpdateScreen()
		{
			enableDisableText = enableDisableTexts[Plugin.showSign];
			showSign = showedSigns[Plugin.showSign];

			imageModeText = imageModeTexts[Plugin.imageMode];	

			SetText(str =>
			{
				str.BeginCenter().Repeat("=", SCREEN_WIDTH).AppendLine();
				str.AppendClr("Gorilla Signs", highlightColor).AppendLine();
				str.Append("A cosmetic mod by dev9998").AppendLine();
				str.Repeat("=", SCREEN_WIDTH).EndAlign().AppendLines(2);
				str.AppendLine(_selectionHandler.GetIndicatedText(0, $"<color={(showSign ? "#" + highlightColor : "white")}>[{enableDisableText}]</color>")).AppendLine();
				str.AppendLine(_selectionHandler.GetIndicatedText(1, $"Image: {Plugin.pngImageNames[Plugin.current]}"));
				str.AppendLine(_selectionHandler.GetIndicatedText(2, $"Filter Mode: {imageModeText}"));
				str.AppendLine(_selectionHandler.GetIndicatedText(3, $"Resolution: {Plugin.res * 1024}"));
				str.AppendLines(1).BeginColor("ffffff10").AppendLine("  ▲/▼ Select  Enter/◀/▶ Adjust").EndColor();
			});
		}

		private void OnEntryAdjusted(int index, bool increase)
		{
			int offset = increase ? 1 : -1;
			switch (index)
			{
				case 1:
					Plugin.current = Mathf.Clamp(Plugin.current + offset, 0, Plugin.pngImagesPublic.Count - 1);
					Plugin.UpdateImage();
					break;
				case 2:
					Plugin.imageMode = Mathf.Clamp(Plugin.imageMode + offset, 0, 2);
					PlayerPrefs.SetInt("GorillaSignsImageMode", Plugin.imageMode);
					Plugin.UpdateImage();
					break;
				case 3:
					Plugin.res = Mathf.Clamp(Plugin.res + offset, 1, 4);
					PlayerPrefs.SetInt("GorillaSignsImageResu", Plugin.res);
					Plugin.UpdateImage();
					break;
			}
		}

		private void OnEntrySelected(int index)
		{
			switch (index)
			{
				case 0:
					if (showSign)
                    {
						showSign = false;
						Plugin.showSign = 0;
						PlayerPrefs.SetInt("GorillaSignsEnabled", 0);
					}
					else
                    {
						showSign = true;
						Plugin.showSign = 1;
						PlayerPrefs.SetInt("GorillaSignsEnabled", 1);
					}

					if (Plugin.showSign == 1)
                    {
						Plugin.signObject.SetActive(true);
                    }
					else
                    {
						Plugin.signObject.SetActive(false);
					}

					break;
			}
		}

		public override void OnKeyPressed(EKeyboardKey key)
		{
			if (_selectionHandler.HandleKeypress(key))
			{
				UpdateScreen();
				return;
			}

			if (key == EKeyboardKey.Left || key == EKeyboardKey.Right)
			{
				OnEntryAdjusted(_selectionHandler.CurrentSelectionIndex, key == EKeyboardKey.Right);
				UpdateScreen();
			}

			switch (key)
			{
				case EKeyboardKey.Back:
					ReturnToMainMenu();
					break;
			}
		}
	}
}
