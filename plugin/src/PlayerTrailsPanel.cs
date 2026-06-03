using UnityEngine;
using UnityEngine.UI;

namespace JalexInteractive 
{
	public class PlayerTrailsPanel : MonoBehaviour {
		private int currentlyActive_offset;
		private int currentlyActive_material;
		private int currentlyActive_trail;
		private void Awake()
		{
			// Initialisation
				// Toggles
			Text t_head = transform.Find("Canvas/TogglesPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_head, PlayerTrails.cfg_headEnabled.Value);
			Text t_lHand = transform.Find("Canvas/TogglesPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_lHand, PlayerTrails.cfg_lHandEnabled.Value);
			Text t_rHand = transform.Find("Canvas/TogglesPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_rHand, PlayerTrails.cfg_rHandEnabled.Value);
				// Offsets
			t_head = transform.Find("Canvas/OffsetsPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			t_lHand = transform.Find("Canvas/OffsetsPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			t_rHand = transform.Find("Canvas/OffsetsPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			currentlyActive_offset = 0;
			ColourFlipSingle(currentlyActive_offset, t_head, t_lHand, t_rHand);
				// Material
			Text t_startColour = transform.Find("Canvas/MaterialPage/StartColour").gameObject.GetComponent(typeof(Text)) as Text;
			Text t_endColour = transform.Find("Canvas/MaterialPage/EndColour").gameObject.GetComponent(typeof(Text)) as Text;
			currentlyActive_material = 0;
			ColourFlipSingle(currentlyActive_material, t_startColour, t_endColour);
				// Trail
			t_head = transform.Find("Canvas/TrailPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			t_lHand = transform.Find("Canvas/TrailPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			t_rHand = transform.Find("Canvas/TrailPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			currentlyActive_trail = 0;
			ColourFlipSingle(currentlyActive_trail, t_head, t_lHand, t_rHand);
			// TODO -- Slider Values | Selected image for material
			// Attached trail setup
				GameObject trailAnchor = transform.Find("TrailAnchor").gameObject;
				if (PlayerTrails.body)
			{
				trailAnchor = Instantiate(PlayerTrails.body);
			} else
			{
				PlayerTrails.Logger.LogError("Body not found for trail reference");
			}
		}
		public void ColourFlip(Text flipMe, bool onOff)
		{
			if (onOff)
			{
				flipMe.color = new Color(flipMe.color.r, flipMe.color.g, flipMe.color.b, 1f);
			} else
			{
				flipMe.color = new Color(flipMe.color.r, flipMe.color.g, flipMe.color.b, 0.1f);
			}
		}
		public void SwitchSelected(int switchThis, int switchTo)
		{
			switch (switchThis)
			{
				case 0:
					currentlyActive_offset = switchTo;
					break;
				case 1:
					currentlyActive_material = switchTo;
					break;
				case 2:
					currentlyActive_trail = switchTo;
					break;
			}
		}
		public void ColourFlipSingle(int selected, Text x, Text y, Text z = null)
		{
			switch (selected)
			{
				case 0:
					x.color = new Color(x.color.r, x.color.g, x.color.b, 1f);
					y.color = new Color(y.color.r, y.color.g, y.color.b, 0.1f);
					if (z)
					{
					z.color = new Color(z.color.r, z.color.g, z.color.b, 0.1f);
					}
					break;
				case 1:
					x.color = new Color(x.color.r, x.color.g, x.color.b, 0.1f);
					y.color = new Color(y.color.r, y.color.g, y.color.b, 1f);
					if (z)
					{
					z.color = new Color(z.color.r, z.color.g, z.color.b, 0.1f);
					}
					break;
				case 2:
					x.color = new Color(x.color.r, x.color.g, x.color.b, 0.1f);
					y.color = new Color(y.color.r, y.color.g, y.color.b, 0.1f);
					if (z)
					{
					z.color = new Color(z.color.r, z.color.g, z.color.b, 1f);
					}
					break;
			}
		}
		public void ActiveFlip(int flipMe)
		{
			switch (flipMe)
			{
				case 0:
					PlayerTrails.body.SetActive(!PlayerTrails.body);
					break;
				case 1:
					PlayerTrails.lHand.SetActive(!PlayerTrails.lHand);
					break;
				case 2:
					PlayerTrails.rHand.SetActive(!PlayerTrails.rHand);
					break;
			}
		}
		public void AllOff()
		{
			PlayerTrails.body.SetActive(false);
			PlayerTrails.lHand.SetActive(false);
			PlayerTrails.rHand.SetActive(false);
		}
		public void AllOn()
		{
			PlayerTrails.body.SetActive(true);
			PlayerTrails.lHand.SetActive(true);
			PlayerTrails.rHand.SetActive(true);
		}
	}
}