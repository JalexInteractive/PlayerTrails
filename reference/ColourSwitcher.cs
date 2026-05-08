using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualMission {
	public class ColourSwitcher : MonoBehaviour {

		public List<MeshRenderer> switchRenderers;

		public List<MeshRenderer> particleMeshRenderers;

		public List<ParticleSystemRenderer> particleRenderers;

		public static byte transValue = 255;

		private Color32 selectedColour = new Color32(0, 192, 255, transValue);

		public void ColourSwitch(int colour) {
			// Overdraw Check
			if (DevPanel.overdrawOn) {
				transValue = 150;
			} else {
				transValue = 255;
			}
			// Set Colour
			switch (colour)
			{
				case 0:
				{
					selectedColour = new Color32(0, 192, 255, transValue); // Default Blue
					break;
				}
				case 1:
				{
					selectedColour = new Color32(0, 255, 0, transValue); // Jalex Interactive Green
					break;
				}
				case 2:
				{
					selectedColour = new Color32(255, 0, 0, transValue); // Red
					break;
				}
				case 3:
				{
					selectedColour = new Color32(255, 111, 255, transValue); // Pink
					break;
				}
				case 4:
				{
					selectedColour = new Color32(255, 143, 0, transValue); // New Vegas Amber
					break;
				}
				case 5:
				{
					selectedColour = new Color32(255, 235, 4, transValue); // Yellow
					break;
				}
				case 6:
				{
					selectedColour = new Color32(0, 50, 111, transValue); // Dark Blue
					break;
				}
				case 7:
				{
					selectedColour = new Color32(0, 101, 0, transValue); // Dark Green
					break;
				}
				case 8:
				{
					selectedColour = new Color32(101, 0, 0, transValue); // Dark Red
					break;
				}
				case 9:
				{
					selectedColour = new Color32(169, 0, 169, transValue); // Ourple
					break;
				}
				case 10:
				{
					selectedColour = new Color32(101, 60, 0, transValue); // Dark Amber
					break;
				}
				case 11:
				{
					selectedColour = new Color32(150, 150, 0, transValue); // Dark Yellow
					break;
				}

			}
			// Switch Colour
			foreach (MeshRenderer mat in switchRenderers)
			{
				mat.sharedMaterial.SetColor("_Color", selectedColour);
				mat.sharedMaterial.SetColor("_EmissionColor", selectedColour);
				mat.sharedMaterial.SetColor("_RimColor", selectedColour);
			}

			foreach (MeshRenderer parmat in particleMeshRenderers)
			{
				parmat.sharedMaterial.SetColor("_TintColor", selectedColour);
			}

			foreach (ParticleSystemRenderer par in particleRenderers)
			{
				par.sharedMaterial.SetColor("_TintColor", selectedColour);
			}	
		}
	}
}