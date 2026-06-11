using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace JalexInteractive 
{
	public class PlayerTrailsPanel : MonoBehaviour {
		public static int currentlyEditing = 0;
		public static int startOrEndColour = 0;
		private static Text t_head_t;
		private static Text t_lHand_t;
		private static Text t_rHand_t;
		private static Text t_head_o;
		private static Text t_lHand_o;
		private static Text t_rHand_o;
		private static Text t_startColour;
		private static Text t_endColour;
		private static Text t_head_tr;
		private static Text t_lHand_tr;
		private static Text t_rHand_tr;
		public static List<Image> textures;

		private void Awake()
		{
			// Initialisation
				// Default to editing head
			currentlyEditing = 0;
				// Toggles
			t_head_t = transform.Find("Canvas/TogglesPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_head_t, PlayerTrails.cfg_headEnabled.Value);
			t_lHand_t = transform.Find("Canvas/TogglesPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_lHand_t, PlayerTrails.cfg_lHandEnabled.Value);
			t_rHand_t = transform.Find("Canvas/TogglesPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_rHand_t, PlayerTrails.cfg_rHandEnabled.Value);
				// Offsets
			t_head_o = transform.Find("Canvas/OffsetsPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			t_lHand_o = transform.Find("Canvas/OffsetsPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			t_rHand_o = transform.Find("Canvas/OffsetsPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlipSingle(currentlyEditing, t_head_o, t_lHand_o, t_rHand_o);
				// Material
			t_startColour = transform.Find("Canvas/MaterialPage/StartColour").gameObject.GetComponent(typeof(Text)) as Text;
			t_endColour = transform.Find("Canvas/MaterialPage/EndColour").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlipSingle(currentlyEditing, t_startColour, t_endColour);
				// Trail
			t_head_tr = transform.Find("Canvas/TrailPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			t_lHand_tr = transform.Find("Canvas/TrailPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			t_rHand_tr = transform.Find("Canvas/TrailPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlipSingle(currentlyEditing, t_head_tr, t_lHand_tr, t_rHand_tr);
			
				// Material Panel Content
			// Grab images from folder
			string[] filesRaw = Directory.GetFiles(PlayerTrails.basePath);
			List<string> fileList= filesRaw.ToList();
			for (int i = 0; i == fileList.Count(); i++)
			{
				if (Path.GetFileName(fileList[i]).EndsWith(".png") || Path.GetFileName(fileList[i]).EndsWith(".jpg") || Path.GetFileName(fileList[i]).EndsWith(".jpeg"))
				{
					
				} else
				{
					fileList.Remove(fileList[i]);
				}
			}
			PlayerTrails.Logger.LogMessage(fileList);

			// Assign to a new panel
			GameObject contentFrame = transform.Find("Canvas/MaterialPage/Textures/TexturePanel/ContentFrame").gameObject;

			// TODO -- Slider Values | Selected image for material
			// Attached trail setup
				Transform trailAnchor = transform.Find("TrailAnchor");
				if (PlayerTrails.body)
			{
				Instantiate(PlayerTrails.body, trailAnchor);
				var bodyTrailClone = trailAnchor.GetChild(0);
				bodyTrailClone.localPosition = new Vector3(0, 0, 0);
				bodyTrailClone.gameObject.SetActive(true);	
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
		public void SwitchEditing(int switchTo)
		{
			currentlyEditing = switchTo;
			ColourFlipSingle(switchTo, t_head_o, t_lHand_o, t_rHand_o);
			ColourFlipSingle(switchTo, t_head_tr, t_lHand_tr, t_rHand_tr);
		}
		public void SwitchStartOrEndColour(int switchTo)
		{
			startOrEndColour = switchTo;
			ColourFlipSingle(switchTo, t_startColour, t_endColour);
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