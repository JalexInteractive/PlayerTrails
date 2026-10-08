using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// TODO
// Add default page that disappears on switching to a new one
// Fix the scroll bar, worst comes to the worst maybe you can fudge it with a slider

namespace JalexInteractive 
{
	public class PlayerTrailsPanel : MonoBehaviour {
		public static int currentlyEditing = 0;
		private static TrailRenderer trailClone;
		public static Image rawImage;
		public static GameObject rawImageGO;
		public static Vector3 rawImageBasePos;
		public static GameObject newPanel = null;
		public static List<string> fileList;
		public static List<Image> imageList = [];
		// Type _ name _ page
		// Toggles
		private static Text t_head_t;
		private static Text t_lHand_t;
		private static Text t_rHand_t;
		// Offsets
		private static Text t_head_o;
		private static Text t_lHand_o;
		private static Text t_rHand_o;
		private static Slider s_x_o;
		private static Slider s_y_o;
		private static Slider s_z_o;
		private static Text t_xv_o;
		private static Text t_yv_o;
		private static Text t_zv_o;
		// Material
		public static int startOrEndColour = 0;
		private static Text t_startColour;
		private static Text t_endColour;
		private static Image i_colourSwatch;
		private static Color32 currentColor;
		private static Slider s_r_m;
		private static Slider s_g_m;
		private static Slider s_b_m;
		private static Slider s_a_m;
		private static Text t_rv_m;
		private static Text t_gv_m;
		private static Text t_bv_m;
		private static Text t_av_m;
		// Trail
		private static Text t_head_tr;
		private static Text t_lHand_tr;
		private static Text t_rHand_tr;
		private static Slider s_t_tr;
		private static Slider s_s_tr;
		private static Slider s_e_tr;
		private static Text t_tv_tr;
		private static Text t_sv_tr;
		private static Text t_ev_tr;

		private void Awake()
		{
			// Initialisation
				// Default to editing head
			currentlyEditing = 0;
				// Toggles
			t_head_t = transform.Find("Canvas/TogglesPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_head_t, PlayerTrails.cfg_bodyEnabled.Value);
			t_lHand_t = transform.Find("Canvas/TogglesPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_lHand_t, PlayerTrails.cfg_lHandEnabled.Value);
			t_rHand_t = transform.Find("Canvas/TogglesPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlip(t_rHand_t, PlayerTrails.cfg_rHandEnabled.Value);
				// Offsets
			t_head_o = transform.Find("Canvas/OffsetsPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			t_lHand_o = transform.Find("Canvas/OffsetsPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			t_rHand_o = transform.Find("Canvas/OffsetsPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlipSingle(currentlyEditing, t_head_o, t_lHand_o, t_rHand_o);
			s_x_o = transform.Find("Canvas/OffsetsPage/Sliders/X").gameObject.GetComponent(typeof(Slider)) as Slider;
			s_y_o = transform.Find("Canvas/OffsetsPage/Sliders/Y").gameObject.GetComponent(typeof(Slider)) as Slider;
			s_z_o = transform.Find("Canvas/OffsetsPage/Sliders/Z").gameObject.GetComponent(typeof(Slider)) as Slider;
			t_xv_o =  transform.Find("Canvas/OffsetsPage/Sliders/X/Value").gameObject.GetComponent(typeof(Text)) as Text;
			t_yv_o =  transform.Find("Canvas/OffsetsPage/Sliders/Y/Value").gameObject.GetComponent(typeof(Text)) as Text;
			t_zv_o =  transform.Find("Canvas/OffsetsPage/Sliders/Z/Value").gameObject.GetComponent(typeof(Text)) as Text;
				// Material
			t_startColour = transform.Find("Canvas/MaterialPage/StartColour").gameObject.GetComponent(typeof(Text)) as Text;
			t_endColour = transform.Find("Canvas/MaterialPage/EndColour").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlipSingle(currentlyEditing, t_startColour, t_endColour);
			i_colourSwatch = transform.Find("Canvas/MaterialPage/ColourSwatch").gameObject.GetComponent(typeof(Image)) as Image;
			s_r_m = transform.Find("Canvas/MaterialPage/Sliders/R").gameObject.GetComponent(typeof(Slider)) as Slider;
			s_g_m = transform.Find("Canvas/MaterialPage/Sliders/G").gameObject.GetComponent(typeof(Slider)) as Slider;
			s_b_m = transform.Find("Canvas/MaterialPage/Sliders/B").gameObject.GetComponent(typeof(Slider)) as Slider;
			s_a_m = transform.Find("Canvas/MaterialPage/Sliders/A").gameObject.GetComponent(typeof(Slider)) as Slider;
			t_rv_m =  transform.Find("Canvas/MaterialPage/Sliders/R/Value").gameObject.GetComponent(typeof(Text)) as Text;
			t_gv_m =  transform.Find("Canvas/MaterialPage/Sliders/G/Value").gameObject.GetComponent(typeof(Text)) as Text;
			t_bv_m =  transform.Find("Canvas/MaterialPage/Sliders/B/Value").gameObject.GetComponent(typeof(Text)) as Text;
			t_av_m =  transform.Find("Canvas/MaterialPage/Sliders/A/Value").gameObject.GetComponent(typeof(Text)) as Text;
			currentColor = PlayerTrails.startColour;
				// Trail
			t_head_tr = transform.Find("Canvas/TrailPage/Body").gameObject.GetComponent(typeof(Text)) as Text;
			t_lHand_tr = transform.Find("Canvas/TrailPage/LHand").gameObject.GetComponent(typeof(Text)) as Text;
			t_rHand_tr = transform.Find("Canvas/TrailPage/RHand").gameObject.GetComponent(typeof(Text)) as Text;
			ColourFlipSingle(currentlyEditing, t_head_tr, t_lHand_tr, t_rHand_tr);
			s_t_tr = transform.Find("Canvas/TrailPage/Sliders/Time").gameObject.GetComponent(typeof(Slider)) as Slider;
			s_s_tr = transform.Find("Canvas/TrailPage/Sliders/StartWidth").gameObject.GetComponent(typeof(Slider)) as Slider;
			s_e_tr = transform.Find("Canvas/TrailPage/Sliders/EndWidth").gameObject.GetComponent(typeof(Slider)) as Slider;
			t_tv_tr =  transform.Find("Canvas/TrailPage/Sliders/Time/Value").gameObject.GetComponent(typeof(Text)) as Text;
			t_sv_tr =  transform.Find("Canvas/TrailPage/Sliders/StartWidth/Value").gameObject.GetComponent(typeof(Text)) as Text;
			t_ev_tr =  transform.Find("Canvas/TrailPage/Sliders/EndWidth/Value").gameObject.GetComponent(typeof(Text)) as Text;
				// Final Setup
			SwitchEditing(0);
			SwitchStartOrEndColour(0);
			
			// Material Panel Content
				// Grab images from folder
			CreateFileList();
				// Grab blank panel for textures panel.
			rawImage = transform.Find("Canvas/MaterialPage/Textures/TexturesPanel/Viewport/ContentFrame/RawImage").gameObject.GetComponent(typeof(Image)) as Image;
			rawImageGO = rawImage.gameObject;
			rawImageBasePos = rawImageGO.transform.localPosition;
				//Swatch
			MaterialActiveSwitched();
				// Attached trail setup
			Transform trailAnchor = transform.Find("TrailAnchor");
			if (!trailAnchor)
			{
				PlayerTrails.Logger.LogError("Trail anchor found for trail reference");
			}
			if (PlayerTrails.body != null)
			{
				Instantiate(PlayerTrails.body, trailAnchor);
				Transform bodyTrailClone = trailAnchor.GetChild(0);
				bodyTrailClone.localPosition = new Vector3(0, 0, 0);
				bodyTrailClone.gameObject.SetActive(true);
				trailClone = bodyTrailClone.gameObject.GetComponent(typeof(TrailRenderer)) as TrailRenderer;
			} else
			{
				PlayerTrails.Logger.LogError("Body not found for trail reference");
			}
		}
		public static void CreateFileList()
		{
			string[] filesRaw = Directory.GetFiles(PlayerTrails.basePath);
			fileList = [.. filesRaw];
			for (int i = 0; i < fileList.Count; i++)
			{
				string extension = Path.GetExtension(fileList[i].ToLower());
				if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
				{
					fileList.Remove(fileList[i]);
				}
			}
			PlayerTrails.Logger.LogMessage("~~ Discovered " + fileList.Count.ToString() + " valid textures ~~");
			foreach (string file in fileList)
			{	
				PlayerTrails.Logger.LogMessage(file);
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
			OffsetActiveSwitched();
			PropertiesActiveSwitched();
			switch (switchTo)
			{
				case 0:
				trailClone = PlayerTrails.bodyTrail;
				break;
				case 1:
				trailClone = PlayerTrails.lHandTrail;
				break;
				case 2:
				trailClone = PlayerTrails.rHandTrail;
				break;
			}
		}
		public void SwitchStartOrEndColour(int switchTo)
		{
			startOrEndColour = switchTo;
			ColourFlipSingle(switchTo, t_startColour, t_endColour);
			MaterialActiveSwitched();
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
			z = null; // TODO - See if this fixes the trail page being a poophead
		}
		public void ActiveFlip(int flipMe)
		{
			switch (flipMe)
			{
				case 0:
					PlayerTrails.body.SetActive(!PlayerTrails.body.activeSelf);
					PlayerTrails.cfg_bodyEnabled.Value = !PlayerTrails.cfg_bodyEnabled.Value;
					break;
				case 1:
					PlayerTrails.lHand.SetActive(!PlayerTrails.lHand.activeSelf);
					PlayerTrails.cfg_lHandEnabled.Value = !PlayerTrails.cfg_lHandEnabled.Value;
					break;
				case 2:
					PlayerTrails.rHand.SetActive(!PlayerTrails.rHand.activeSelf);
					PlayerTrails.cfg_rHandEnabled.Value = !PlayerTrails.cfg_rHandEnabled.Value;
					break;
			}
		}
		public void OffsetUpdate(int axis) // 0-X | 1-Y | 2-Z
		{
			switch (currentlyEditing)
			{
				case 0:
					switch(axis)
					{
						case 0:
							PlayerTrails.bodyOffset.x = s_x_o.value;
						break;
						case 1:
							PlayerTrails.bodyOffset.y = s_y_o.value;
						break;
						case 2:
							PlayerTrails.bodyOffset.z = s_z_o.value;
						break;
					}
					PlayerTrails.body.transform.localPosition = PlayerTrails.bodyOffset;
                    PlayerTrails.cfg_bodyOffset.Value = PlayerTrails.bodyOffset;
				break;
				case 1:
				switch(axis)
					{
						case 0:
							PlayerTrails.lHandOffset.x = s_x_o.value;
						break;
						case 1:
							PlayerTrails.lHandOffset.y = s_y_o.value;
						break;
						case 2:
							PlayerTrails.lHandOffset.z = s_z_o.value;
						break;
					}
					PlayerTrails.lHand.transform.localPosition = PlayerTrails.lHandOffset;
					PlayerTrails.cfg_lHandOffset.Value = PlayerTrails.lHandOffset;
				break;
				case 2:
					switch(axis)
					{
						case 0:
							PlayerTrails.rHandOffset.x = s_x_o.value;
						break;
						case 1:
							PlayerTrails.rHandOffset.y = s_y_o.value;
						break;
						case 2:
							PlayerTrails.rHandOffset.z = s_z_o.value;
						break;
					}
					PlayerTrails.rHand.transform.localPosition = PlayerTrails.rHandOffset;
					PlayerTrails.cfg_rHandOffset.Value = PlayerTrails.rHandOffset;
				break;
			}
			t_xv_o.text = s_x_o.value.ToString();
			t_yv_o.text = s_y_o.value.ToString();
			t_zv_o.text = s_z_o.value.ToString();
		}
		private void OffsetActiveSwitched()
		{
			switch(currentlyEditing)
			{
				case 0:
					s_x_o.value = PlayerTrails.bodyOffset.x;
					s_y_o.value = PlayerTrails.bodyOffset.y;
					s_z_o.value = PlayerTrails.bodyOffset.z;
				break;
				case 1:
					s_x_o.value = PlayerTrails.lHandOffset.x;
					s_y_o.value = PlayerTrails.lHandOffset.y;
					s_z_o.value = PlayerTrails.lHandOffset.z;
				break;
				case 2:
					s_x_o.value = PlayerTrails.rHandOffset.x;
					s_y_o.value = PlayerTrails.rHandOffset.y;
					s_z_o.value = PlayerTrails.rHandOffset.z;
				break;
			}
			t_xv_o.text = s_x_o.value.ToString();
			t_yv_o.text = s_y_o.value.ToString();
			t_zv_o.text = s_z_o.value.ToString();
		}
		public void MaterialUpdate(int rgba) // 0-R | 1-G | 2-B | 3-A
		{
			switch(startOrEndColour)
			{
				case 0:
					switch (rgba)
					{
						case 0:
							PlayerTrails.startColour.r = (byte)s_r_m.value;
						break;
						case 1:
							PlayerTrails.startColour.g = (byte)s_g_m.value;
						break;
						case 2:
							PlayerTrails.startColour.b = (byte)s_b_m.value;
						break;
						case 3:
							PlayerTrails.startColour.a = (byte)s_a_m.value;
						break;
					}
					PlayerTrails.bodyTrail.startColor = PlayerTrails.startColour;
					PlayerTrails.lHandTrail.startColor = PlayerTrails.startColour;
					PlayerTrails.rHandTrail.startColor = PlayerTrails.startColour;
					i_colourSwatch.color = PlayerTrails.startColour;
					PlayerTrails.cfg_startColour.Value = PlayerTrails.Color32ToV4(PlayerTrails.startColour);
					PlayerTrails.cfg_startColour.ConfigFile.Save(); // Unknown if nessecary
					trailClone.startColor = PlayerTrails.startColour;
				break;
				case 1:
				switch (rgba)
					{
						case 0:
							PlayerTrails.endColour.r = (byte)s_r_m.value;
						break;
						case 1:
							PlayerTrails.endColour.g = (byte)s_g_m.value;
						break;
						case 2:
							PlayerTrails.endColour.b = (byte)s_b_m.value;
						break;
						case 3:
							PlayerTrails.endColour.a = (byte)s_a_m.value;
						break;
					}
					PlayerTrails.bodyTrail.endColor = PlayerTrails.endColour;
					PlayerTrails.lHandTrail.endColor = PlayerTrails.endColour;
					PlayerTrails.rHandTrail.endColor = PlayerTrails.endColour;
					i_colourSwatch.color = PlayerTrails.endColour;
					PlayerTrails.cfg_endColour.Value = PlayerTrails.Color32ToV4(PlayerTrails.endColour);
					PlayerTrails.cfg_endColour.ConfigFile.Save(); // Unknown if nessecary
					trailClone.endColor = PlayerTrails.endColour;
				break;
			}
			t_rv_m.text = s_r_m.value.ToString();
			t_gv_m.text = s_g_m.value.ToString();
			t_bv_m.text = s_b_m.value.ToString();
			t_av_m.text = s_a_m.value.ToString();
		}
		private void MaterialActiveSwitched()
		{
			switch (startOrEndColour)
			{
				case 0:
					// Sliders
					currentColor = PlayerTrails.startColour;
					// Swatch
					i_colourSwatch.color = currentColor;
				break;
				case 1:
					currentColor = PlayerTrails.endColour;
					i_colourSwatch.color = currentColor;
				break;	
			}
			s_r_m.value = currentColor.r;
			s_g_m.value = currentColor.g;
			s_b_m.value = currentColor.b;
			s_a_m.value = currentColor.a;
			t_rv_m.text = s_r_m.value.ToString();
			t_gv_m.text = s_g_m.value.ToString();
			t_bv_m.text = s_b_m.value.ToString();
			t_av_m.text = s_a_m.value.ToString();
		}
		public static void TextureSwitch (string newmat)
		{
			PlayerTrails.trailTex.LoadImage(PlayerTrails.TextureGrab(newmat));
			string[] split = newmat.Split('\\');
			string cfgUpdate = split.Last();
			PlayerTrails.cfg_trailTex.Value = cfgUpdate;
		}
		public void StartLoadPanelImages()
		{
			((MonoBehaviour)this).StartCoroutine(LoadPanelImages());
		}
		private IEnumerator LoadPanelImages()
		{
			CreateFileList();
			// Check if run previously, if so delete old panels
			bool isNullOrEmpty = imageList?.Any() != true;
			if (!isNullOrEmpty)
			{
				foreach (Image img in imageList)
				{
					Destroy(img.gameObject);
				}
				imageList.Clear();
				rawImageGO.transform.localPosition = rawImageBasePos;
			}
			for (int k = 0; k < fileList.Count; k++)
			{
				// Instantiate new panels and move template down
				newPanel = Instantiate(rawImageGO, rawImageGO.transform.parent.transform);
				// Add new image to image list
				Image newPanelImage = newPanel.GetComponent(typeof(Image)) as Image;
                imageList.Add(newPanelImage);
				// Create sprites and Texture2Ds for image
				Texture2D panelTex = new(128, 128, TextureFormat.DXT5, false){name = "panelTex" + k.ToString()};
				panelTex.LoadImage(PlayerTrails.TextureGrab(fileList[k]));
                Sprite panelSprite = Sprite.Create(panelTex, new Rect(0f, 0f, (float)((Texture)panelTex).width, (float)((Texture)panelTex).height), rawImage.transform.localPosition);
				panelSprite.name = "panelSprite" + k.ToString();
				// Apply
                imageList[k].sprite = panelSprite;
				Button buttonClick = newPanel.GetComponent(typeof(Button)) as Button;
				string sendURL = fileList[k];
				buttonClick.onClick.AddListener(()=>TextureSwitch(sendURL));
				newPanel.SetActive(true);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(rawImageGO.transform.parent.transform as RectTransform);
			yield return null;
		}
		public void PropertiesUpdate(int property) // 0-Time | 1-Start width | 2-End Width
		{
			switch (currentlyEditing)
			{
				case 0:
					switch(property)
					{
						case 0:
							PlayerTrails.bodyTrail.time = s_t_tr.value;
                    		PlayerTrails.cfg_bodyTime.Value = PlayerTrails.bodyTrail.time;
						break;
						case 1:
							PlayerTrails.bodyTrail.startWidth = s_s_tr.value / 2;
                    		PlayerTrails.cfg_bodyStartWidth.Value = PlayerTrails.bodyTrail.startWidth;
						break;
						case 2:
							PlayerTrails.bodyTrail.endWidth = s_e_tr.value / 2;
							PlayerTrails.cfg_bodyEndWidth.Value = PlayerTrails.bodyTrail.endWidth;
						break;
					}
				break;
				case 1:
				switch(property)
					{
						case 0:
							PlayerTrails.lHandTrail.time = s_t_tr.value;
                    		PlayerTrails.cfg_lHandTime.Value = PlayerTrails.lHandTrail.time;
						break;
						case 1:
							PlayerTrails.lHandTrail.startWidth = s_s_tr.value / 2;
                    		PlayerTrails.cfg_lHandStartWidth.Value = PlayerTrails.lHandTrail.startWidth;
						break;
						case 2:
							PlayerTrails.lHandTrail.endWidth = s_e_tr.value / 2;
							PlayerTrails.cfg_lHandEndWidth.Value = PlayerTrails.lHandTrail.endWidth;
						break;
					}
				break;
				case 2:
					switch(property)
					{
						case 0:
							PlayerTrails.rHandTrail.time = s_t_tr.value;
                    		PlayerTrails.cfg_rHandTime.Value = PlayerTrails.rHandTrail.time;
						break;
						case 1:
							PlayerTrails.rHandTrail.startWidth = s_s_tr.value / 2;
                    		PlayerTrails.cfg_rHandStartWidth.Value = PlayerTrails.rHandTrail.startWidth;
						break;
						case 2:
							PlayerTrails.rHandTrail.endWidth = s_e_tr.value / 2;
							PlayerTrails.cfg_rHandEndWidth.Value = PlayerTrails.rHandTrail.endWidth;
						break;
					}
				break;
			}
			t_tv_tr.text = s_t_tr.value.ToString();
			t_sv_tr.text = s_s_tr.value.ToString();
			t_ev_tr.text = s_e_tr.value.ToString();
		}
		private void PropertiesActiveSwitched()
		{
			switch(currentlyEditing)
			{
				case 0:
					s_t_tr.value = PlayerTrails.bodyTrail.time;
					s_s_tr.value = PlayerTrails.bodyTrail.startWidth * 2;
					s_e_tr.value = PlayerTrails.bodyTrail.endWidth * 2;
				break;
				case 1:
					s_t_tr.value = PlayerTrails.lHandTrail.time;
					s_s_tr.value = PlayerTrails.lHandTrail.startWidth * 2;
					s_e_tr.value = PlayerTrails.lHandTrail.endWidth * 2;
				break;
				case 2:
					s_t_tr.value = PlayerTrails.rHandTrail.time;
					s_s_tr.value = PlayerTrails.rHandTrail.startWidth * 2;
					s_e_tr.value = PlayerTrails.rHandTrail.endWidth * 2;
				break;
			}
			t_tv_tr.text = s_t_tr.value.ToString();
			t_sv_tr.text = s_s_tr.value.ToString();
			t_ev_tr.text = s_e_tr.value.ToString();
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