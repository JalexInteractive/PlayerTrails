using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JalexInteractive 
{
	public class ButtonColourToggle : MonoBehaviour {
		public enum Type
		{
			Single,
			Multi
		}
		public Type typeOfSet;
		public List<Text> buttons;
		public void AllSwitch(bool allSwitch)
		{
			if (allSwitch)
			{
				foreach (Text button in buttons)
				{
					button.color =  new Color(button.color.r, button.color.g, button.color.b, 1f);
				}
			} else
			{
				foreach (Text button in buttons)
				{
					button.color =  new Color(button.color.r, button.color.g, button.color.b, 0.1f);
				}
			}
		}
		public void DoIt(int selected)
			{
				if (typeOfSet == Type.Single)
				{
					if (buttons[selected].color.a == 0.1f)
					{
						buttons[selected].color =  new Color(buttons[selected].color.r, buttons[selected].color.g, buttons[selected].color.b, 1f);
					} else
					{
						buttons[selected].color =  new Color(buttons[selected].color.r, buttons[selected].color.g, buttons[selected].color.b, 0.1f);
					}
				} else {
					buttons[selected].color =  new Color(buttons[selected].color.r, buttons[selected].color.g, buttons[selected].color.b, 1f);
					for (int i = 0; i <= buttons.Count; i++)
					{
						if (i != selected)
							{
								buttons[selected].color =  new Color(buttons[selected].color.r, buttons[selected].color.g, buttons[selected].color.b, 0.1f);
							}
					}
				}
			}
	}
}