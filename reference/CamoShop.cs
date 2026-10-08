using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using FistVR;
using HarmonyLib;
using OtherLoader;
using UnityEngine;
using UnityEngine.UI;

[assembly: Debuggable(DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("0.0.0.0")]
[module: UnverifiableCode]
namespace NGA.CamoShop
{
	[BepInPlugin("NGA.CamoShop", "CamoShop", "0.1.0")]
	[BepInProcess("h3vr.exe")]
	[Description("Built with MeatKit")]
	[BepInDependency("h3vr.otherloader", "1.3.0")]
	public class CamoShopPlugin : BaseUnityPlugin
	{
		private static readonly string BasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		internal static ManualLogSource Logger;

		private void Awake()
		{
			Logger = ((BaseUnityPlugin)this).Logger;
			LoadAssets();
		}

		private void LoadAssets()
		{
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "NGA.CamoShop");
			OtherLoader.RegisterDirectLoad(BasePath, "NGA.CamoShop", "", "", "camoshop,shaderholder,camospray,resizecontrol", "");
		}
	}
}
namespace NGA
{
	public class PaintTubMan : MonoBehaviour
	{
		public Text output_text;

		public List<Image> camo_images_ordered;

		public Text packname_text;

		public Text typename_text;

		public Text camo_text_name;

		private string curr_pack_name = "";

		private string curr_type_name = "";

		private string curr_camo_name = "";

		private List<string> curr_camo_names_ordered = new List<string>();

		private List<string> curr_type_names = new List<string>();

		private List<string> pack_names = new List<string>();

		private int curr_pack_ix = 0;

		private int curr_type_ix = 0;

		private bool camo_selected = false;

		private int selected_camo_ix = 0;

		private int starting_page_ix = 0;

		private Dictionary<string, Dictionary<string, List<string>>> all_camo_dirs;

		public Text camochallenge_skinname_text;

		public Text camochallenge_description_text;

		public Text camochallenge_progress_text;

		public Text camochallenge_bind2gun_text;

		private List<Quest> camoavailable_quests = new List<Quest>();

		private int camocurr_quest_ix = 0;

		public Text gunchallenge_gunname_text;

		public Text gunchallenge_description_text;

		public Text gunchallenge_progress_text;

		public Text gunchallenge_camounlock_text;

		public Text gunchallenge_challLimit_text;

		private List<Quest> gunavailable_quests = new List<Quest>();

		private int guncurr_quest_ix = 0;

		public Text cheats_output;

		public FVRQuickBeltSlot slot;

		public FVRPhysicalObject selectedSlotObj = null;

		private bool unlock_all_camos_cheats = false;

		public Text cheatModelNumber;

		private SpecialCamo currSpecCamo = null;

		public Transform AdvancedUI;

		private bool isPreview = false;

		public Text previewButText;

		public Transform CashMoneyUI;

		private int universalPrice = 2;

		private string TutorialText = "Tutorial: (1) Place gun on QB slot (2) Select camo pack and type (3) Select unlocked camo, 'APPLY Camo' (4) Select locked camo (5) Pick any challenge, 'Accept Challenge' on gun.\n\nRules: (1) Progress tracks only on challenge's bound gun (2) Each gun can unlock a max of " + MasteryCamos.maxQuestsPerGun + " camos (3) 'Remove' uncompleted challenges from a gun to replace it with another.";

		private float lastPressTime = 0f;

		private int lastButtonPressed = -1;

		private int pressCount = 0;

		private const float doublePressThreshold = 0.5f;

		private Dictionary<int, string> buttonMapping = new Dictionary<int, string>
		{
			{ 1, "#" },
			{ 2, "ABC" },
			{ 3, "DEF" },
			{ 4, "GHI" },
			{ 5, "JKL" },
			{ 6, "MNO" },
			{ 7, "PQRS" },
			{ 8, "TUV" },
			{ 9, "WXYZ" },
			{ 0, "-" }
		};

		private Dictionary<string, string> cheatsDictionary = new Dictionary<string, string>();

		private void Start()
		{
			all_camo_dirs = MasteryCamos.camoPacks.PackTypeCamos;
			pack_names.AddRange(all_camo_dirs.Keys);
			RefreshUI();
			BTN_SetTutorialText();
			string text = GenerateFreeCamosCode();
			cheatsDictionary.Add(text, "CHEAT ENABLED: All camos are unlocked for all firearms!\n\nTip: Memorize this code, it will always be the same.");
			cheatsDictionary.Add("MW", "Haha, funny sex number.");
			cheatsDictionary.Add("W##", "911 Operator, how may I help you..?");
			cheatsDictionary.Add("-G#A", "Oooh so close, you almost cracked the super secret gamer davinci code! Just 1 number off!");
			cheatsDictionary.Add("G#A", "That's it! You did it!     Oh no, wait, it's just a tired trope.");
			cheatsDictionary.Add("##J", "MAYBE NEXT TIME! Brutus OUT!");
			cheatsDictionary.Add("###", "War. War never changes.");
			cheatsDictionary.Add("#-#", "The Lone Wanderer.");
			cheatsDictionary.Add("#D", "The Vault Dweller.");
			cheatsDictionary.Add("T--T", "Heh!");
			cheatsDictionary.Add("T--TJ", "Heh! heh!");
			cheatsDictionary.Add("BOOB", "8008");
			cheatsDictionary.Add("BOOBS", "80085");
			cheatsDictionary.Add("MMM", "The average price of a McDonald's FRENCH fry in the U.S.A. has far exceeded inflation trends. INVEST!");
			cheatsDictionary.Add("#WTG", "What year is it?");
			cheatsDictionary.Add("TMPJD-W", "Hi, this is Jenny?");
			cheatsDictionary.Add("GA-", "BlAzE IT BrOtHEr");
			cheatsDictionary.Add("GJ#", "A kindling");
			cheatsDictionary.Add("DGD", "So be it. Save his head. Dispose of the rest.");
			cheatsDictionary.Add("##P", "Master Shi Fu!");
			cheatsDictionary.Add("MODMAS", "Like Meatmas, but with blackjack and hookers!");
			cheatsDictionary.Add("JERRYAR", "Is it a bird? Is it a plane? YES. It's a motherducking FIGHTER JET! 'Impossible' my ass");
			cheatsDictionary.Add("GRONCH", "Gronch.\nSTOLE MEATMAS!\nBut so much to think about.\nHustlers must watch.");
			cheatsDictionary.Add("GOD", "I'm a FRENCH fry, so we're not in the best of terms.");
			cheatsDictionary.Add("CITYROBO", "If OpenScripts2 is so good, why isn't there an OpenScripts3, huh?\nEver think about that!?");
			cheatsDictionary.Add("ANDREA", "Thanks for helping me get started in H3VR modding, have had lots of fun!");
			cheatsDictionary.Add("NRGILL", "Thanks for the free SODA suckeerrrrrr!");
			cheatsDictionary.Add("NSFW", "Wow, really, in H3VR? You want lewd hotdogs?\nHow about you watch some Healthy Gamers GG videos about shaking 'certain' habits.");
			cheatsDictionary.Add("MEAT", "You spelled banana wrong!");
			cheatsDictionary.Add("WOLFIE", "Think modular <<>{}<>>");
			cheatsDictionary.Add("SMIDGEON", "If my pistol malfunctions one more F****** time!");
			cheatsDictionary.Add("POTATOES", "Great for soup, though an unreliable source of SUStenance.\nAlso I'm still not sure how many of you there are.");
			cheatsDictionary.Add("NGA", "Yous making me blush, stop it!");
			cheatsDictionary.Add("AGN", "They call me, Professor Chaos! Sorry about the cat prank, unless you found it funny, in which case, still sorry.");
			cheatsDictionary.Add("AZNFLIP", "Thank you for your help testing and giving ideas for the creation of this mod :)");
			cheatsDictionary.Add("HOYT", "Thank you for your help testing & giving feedback on this mod during Alpha and Beta :)");
			cheatsDictionary.Add("GAY", "You are now legally gay.\nNotifying all local, state, and federal agencies.");
			cheatsDictionary.Add("MELON", "They got that GYATT ykwim ong ong fr");
			cheatsDictionary.Add("PACKER", "Handsomely FRENCH fry, go catch their Twitch streams!");
			cheatsDictionary.Add("HINT", "You have to look for the factory serial number under the base of this object, then type it in like an old telephone number.");
			cheatsDictionary.Add("CHEAT", "The camo-unlock cheat code is: " + text);
			cheatsDictionary.Add("VIP", "Never call something impossible as long as there are talented people on earth and at least $1000 dollars.");
			cheatsDictionary.Add("SORA", "Epic skins! Keep being the reason there's anime girls on R2MM every time someone walks into my room.");
			cheatsDictionary.Add("MERCY", "Mommy makes my fav maps, and most importantly, helps others make their own too <3");
			cheatsDictionary.Add("NIKPO", "MORE. CYBER. PUNK. NOW.");
			cheatsDictionary.Add("OKKIM", "I lost half my teeth, but at least I can use grenades now!");
			cheatsDictionary.Add("ANTON", "Anton is short for Antonio Banderas, the majority shareholder of RUST LTD and the main developer of this video game.");
			cheatsDictionary.Add("VR", "Technically cheaper than therapy, but not in the long run.");
			cheatsDictionary.Add("HAND", "Antonio Banderas's little known middle name.");
			cheatsDictionary.Add("SOSIG", "Abstractly, they're the equivalent of a human videogame enemy wrapped in a condom.\nPractically, they're mystery meat, also wrapped in a condom.");
			cheatsDictionary.Add("ILLEMONATI", "Novus Ordo Citrorum.\nThe Faithful Shall Prosper And The Heretic Will Fall Against The Endless Scream Of Our Lord\nAAAAAAAAAAAAAAGGGGGGGHHHHHHHHH!!!!!!!!!!!!!!");
			cheatsDictionary.Add("CLUNK", "The precursors.\nIt is from them that Sosigs derive.");
			cheatsDictionary.Add("TNH", "It's like Supply Raid (SR), but without blackjack and hookers.");
			cheatsDictionary.Add("SR", "It's like TNH, but with blackjack and hookers.");
			cheatsDictionary.Add("HOTDOGS", "-> HORSHOES");
			cheatsDictionary.Add("HOTDOG", "-> HORSHOES");
			cheatsDictionary.Add("HORSHOES", "-> HANDGRENADES");
			cheatsDictionary.Add("HANDGRENADES", "No person shall be found with fewer than thirteen times their own body weight of federally provisioned munitions.");
			cheatsDictionary.Add("WURSTWORLD", "Weird noises coming from under that mountain.");
			cheatsDictionary.Add("ROTWEINERS", "Rotten weiner!? What's that supposed to mean! I shower almost daily!");
			cheatsDictionary.Add("ROTR", "If I didn't have a dopamine deficiency I would totally love playing this slower-paced gamemode.\nI'd probably have a more successful, happy life too, but that's beside the point.");
			cheatsDictionary.Add("WEINER", "Another word for DING DONG!");
			cheatsDictionary.Add("SHRINK", "*SSCZZ SSCZZ* ExpErImEntAL SHrInK RAY FAiL\nBYpAsS COmprEsOR: SET TEmP 32 F");
			cheatsDictionary.Add("GROW", "*SSCZZ SSCZZ* ExpErImEntAL GrOWtH RAY FAiL\nBYpAsS COmprEsOR: SET TEmP 451 F");
			cheatsDictionary.Add("OLEFAR", "The only reason the camos can look as awesome as they do!");
		}

		private void Update()
		{
			if ((Object)(object)selectedSlotObj == (Object)null && (Object)(object)slot.CurObject != (Object)null)
			{
				selectedSlotObj = slot.CurObject;
				RefreshCamoSelectImages();
				BTN_ShowGunQuests();
				GetGunsSpecCamo(((Component)selectedSlotObj).gameObject);
				RefreshAdvancedCamoUI();
			}
			else if ((Object)(object)selectedSlotObj != (Object)null && (Object)(object)slot.CurObject == (Object)null)
			{
				if (isPreview)
				{
					BTN_RemoveCamo(((Component)selectedSlotObj).gameObject);
				}
				selectedSlotObj = null;
				currSpecCamo = null;
				SetIsPreview(value: false);
				CleanGunQuestsSection();
				RefreshCamoSelectImages();
			}
		}

		private SpecialCamo GetGunsSpecCamo(GameObject obj)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected O, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			MasteryCamosGunState mCState = MasteryCamos.GetMCState(obj);
			if ((Object)(object)mCState == (Object)null)
			{
				currSpecCamo = new SpecialCamo();
				return currSpecCamo;
			}
			if (mCState.shader_params_v1 != null)
			{
				SetCamoHeirFromName(mCState.shader_params_v1.ColorTexture);
				currSpecCamo = mCState.shader_params_v1;
				return currSpecCamo;
			}
			if (mCState.camo_id != "")
			{
				SpecialCamo val = new SpecialCamo();
				val.ColorTexture = mCState.camo_id;
				SetCamoHeirFromName(mCState.camo_id);
				currSpecCamo = val;
				return currSpecCamo;
			}
			currSpecCamo = new SpecialCamo();
			return currSpecCamo;
		}

		private void SetCamoHeirFromName(string camoName)
		{
			if (!(camoName == ""))
			{
				string text = default(string);
				string text2 = default(string);
				string text3 = default(string);
				MasteryCamos.ExtractPackAndType(camoName, ref text, ref text2, ref text3);
				camo_text_name.text = text3;
				curr_camo_name = camoName;
				camo_selected = true;
			}
		}

		private void SetIsPreview(bool value)
		{
			isPreview = value;
			if (!((Object)(object)previewButText == (Object)null))
			{
				if (isPreview)
				{
					previewButText.text = "PREVIEW*";
				}
				else
				{
					previewButText.text = "PREVIEW";
				}
			}
		}

		public void RefreshUI()
		{
			RefreshCamoSubsection();
			CleanCamoQuestsSection();
			RefreshGunQuestsSection();
		}

		public void RefreshCashMoneyUI()
		{
			if ((Object)(object)CashMoneyUI == (Object)null)
			{
				Debug.LogError((object)"CashMoneyUI Camo UI no here");
				output_text.text = "Internal error: CashMoneyUI UI no here";
				return;
			}
			Transform val = CashMoneyUI.Find("ButtonsAndUI");
			if ((Object)(object)val != (Object)null)
			{
				Text component = ((Component)val.Find("CashFunds")).GetComponent<Text>();
				if ((Object)(object)component != (Object)null)
				{
					component.text = "Current Funds: " + GM.MMFlags.GB;
				}
				Text component2 = ((Component)val.Find("MarketPrice")).GetComponent<Text>();
				if ((Object)(object)component2 != (Object)null)
				{
					component2.text = "Camo Price: " + universalPrice;
				}
			}
		}

		public void RefreshAdvancedCamoUI()
		{
			if ((Object)(object)AdvancedUI == (Object)null)
			{
				Debug.LogError((object)"Advanced Camo UI no here");
				output_text.text = "Internal error: Advanced UI no here";
				return;
			}
			Transform val = AdvancedUI.Find("CamoProperties");
			if ((Object)(object)val != (Object)null)
			{
				Text component = ((Component)val.Find("CamoStrength")).GetComponent<Text>();
				if ((Object)(object)component != (Object)null)
				{
					component.text = ((currSpecCamo == null) ? "" : currSpecCamo.CamoStrength.ToString());
				}
				Text component2 = ((Component)val.Find("CamoMet")).GetComponent<Text>();
				if ((Object)(object)component2 != (Object)null)
				{
					component2.text = ((currSpecCamo == null) ? "" : currSpecCamo.CamoMetallic.ToString());
				}
				Text component3 = ((Component)val.Find("CamoRough")).GetComponent<Text>();
				if ((Object)(object)component3 != (Object)null)
				{
					component3.text = ((currSpecCamo == null) ? "" : currSpecCamo.CamoRoughness.ToString());
				}
			}
			Transform val2 = AdvancedUI.Find("MaskProperties");
			if ((Object)(object)val2 != (Object)null)
			{
				Text component4 = ((Component)val2.Find("CamoMask")).GetComponent<Text>();
				if ((Object)(object)component4 != (Object)null)
				{
					component4.text = ((currSpecCamo == null) ? "" : currSpecCamo.CamoMaskChannel.ToString());
				}
				Text component5 = ((Component)val2.Find("CamoMaskMin")).GetComponent<Text>();
				if ((Object)(object)component5 != (Object)null)
				{
					component5.text = ((currSpecCamo == null) ? "" : currSpecCamo.CamoMaskMin.ToString());
				}
				Text component6 = ((Component)val2.Find("CamoMaskMax")).GetComponent<Text>();
				if ((Object)(object)component6 != (Object)null)
				{
					component6.text = ((currSpecCamo == null) ? "" : currSpecCamo.CamoMaskMax.ToString());
				}
				Text component7 = ((Component)val2.Find("BTN_CamoInvertMask")).GetComponent<Text>();
				if ((Object)(object)component7 != (Object)null)
				{
					component7.text = "Invert Camo Mask: " + ((currSpecCamo == null) ? "" : currSpecCamo.InvertCamoMask.ToString());
				}
			}
		}

		public void RefreshCamoSubsection()
		{
			camo_text_name.text = "";
			camo_selected = false;
			starting_page_ix = 0;
			if (pack_names.Count > 0)
			{
				curr_pack_name = pack_names[curr_pack_ix % pack_names.Count];
				packname_text.text = curr_pack_name;
				curr_type_names.Clear();
				curr_type_names.AddRange(all_camo_dirs[curr_pack_name].Keys);
				if (curr_type_names.Count > 0)
				{
					string text = curr_type_names[curr_type_ix % curr_type_names.Count];
					if (!(text == curr_type_name))
					{
						curr_type_name = text;
						typename_text.text = curr_type_name;
						curr_camo_names_ordered.Clear();
						curr_camo_names_ordered.AddRange(all_camo_dirs[curr_pack_name][curr_type_name]);
						RefreshCamoSelectImages();
					}
				}
				else
				{
					output_text.text = "Error: No camo types in current camo pack: " + curr_pack_name;
				}
			}
			else
			{
				output_text.text = "Error: There aren't any loaded camo Packs!";
			}
		}

		public void RefreshCamoSelectImages()
		{
			StartRefreshCamoSelectImages();
		}

		public void StartRefreshCamoSelectImages()
		{
			((MonoBehaviour)this).StartCoroutine(RefreshCamoSelectImagesCoroutine());
		}

		private IEnumerator RefreshCamoSelectImagesCoroutine()
		{
			int sizeCamoNames = curr_camo_names_ordered.Count;
			int sizeCamoButtons = camo_images_ordered.Count;
			for (int i = starting_page_ix; i < sizeCamoNames || i < starting_page_ix + sizeCamoButtons; i++)
			{
				int imageCourted = i % sizeCamoButtons;
				if (i < sizeCamoNames && i < starting_page_ix + sizeCamoButtons)
				{
					Texture2D val = (Texture2D)MasteryCamos.GetTexture(curr_camo_names_ordered[i]);
					if ((Object)(object)val != (Object)null)
					{
						Sprite sprite = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f));
						camo_images_ordered[imageCourted].sprite = sprite;
						((Component)((Component)camo_images_ordered[imageCourted]).transform.parent).gameObject.SetActive(true);
						if (IsUnlockedOnCurrObj(curr_camo_names_ordered[i]))
						{
							((Component)((Component)camo_images_ordered[imageCourted]).transform.parent).gameObject.GetComponent<Text>().text = "";
						}
						else
						{
							((Component)((Component)camo_images_ordered[imageCourted]).transform.parent).gameObject.GetComponent<Text>().text = "LOCKED";
						}
					}
					else
					{
						output_text.text = "Error: Couldn't load picture for " + curr_camo_names_ordered[i];
					}
				}
				else
				{
					if (i < sizeCamoNames || i >= starting_page_ix + sizeCamoButtons)
					{
						break;
					}
					((Component)((Component)camo_images_ordered[imageCourted]).transform.parent).gameObject.SetActive(false);
					((Component)((Component)camo_images_ordered[imageCourted]).transform.parent).gameObject.GetComponent<Text>().text = "";
				}
				yield return null;
			}
			yield return null;
		}

		public void CleanCamoQuestsSection()
		{
			camochallenge_skinname_text.text = ((!camo_selected) ? "Skin Name" : curr_camo_name);
			camochallenge_description_text.text = "Description";
			camochallenge_progress_text.text = FormatProgressText(0, 0);
			camochallenge_bind2gun_text.text = FormatBind2GunText("");
			camoavailable_quests.Clear();
			camocurr_quest_ix = 0;
		}

		public void RefreshCamoQuestsSection()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (camo_selected)
			{
				camochallenge_skinname_text.text = curr_camo_name;
				if (ViewableCamoQuestExists())
				{
					Quest val = camoavailable_quests[camocurr_quest_ix];
					camochallenge_description_text.text = QuestsPool.GetQuestDescription(val.description_key);
					camochallenge_progress_text.text = FormatProgressText(val.current_progress, val.target_goal);
					RefreshAllGunNameStrings();
				}
				else
				{
					CleanCamoQuestsSection();
				}
			}
			else
			{
				CleanCamoQuestsSection();
			}
		}

		public void CleanGunQuestsSection()
		{
			RefreshAllGunNameStrings();
			gunchallenge_gunname_text.text = "Gun Name";
			gunchallenge_description_text.text = "Description";
			gunchallenge_progress_text.text = FormatProgressText(0, 0);
			gunchallenge_camounlock_text.text = FormatUnlocksCamoText("");
			gunchallenge_challLimit_text.text = FormatChallMaxText(0, 0);
			gunavailable_quests.Clear();
			guncurr_quest_ix = 0;
		}

		public void RefreshGunQuestsSection()
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			QBSlotResult qbObj = GetQbObj();
			if ((Object)(object)qbObj.itemObject == (Object)null || !qbObj.isFirearm || !ViewableGunQuestExists())
			{
				CleanGunQuestsSection();
				return;
			}
			RefreshAllGunNameStrings();
			Quest val = gunavailable_quests[guncurr_quest_ix];
			gunchallenge_description_text.text = QuestsPool.GetQuestDescription(val.description_key);
			gunchallenge_progress_text.text = FormatProgressText(val.current_progress, val.target_goal);
			gunchallenge_camounlock_text.text = FormatUnlocksCamoText(val.camo_reward);
			gunchallenge_challLimit_text.text = FormatChallMaxText(MasteryCamos.CountChallengesForGun(((Object)qbObj.itemObject).name), MasteryCamos.maxQuestsPerGun);
		}

		public void RefreshAllGunNameStrings()
		{
			QBSlotResult qbObj = GetQbObj();
			if ((Object)(object)qbObj.itemObject == (Object)null || !qbObj.isFirearm)
			{
				camochallenge_bind2gun_text.text = FormatBind2GunText("");
				gunchallenge_gunname_text.text = "Gun Name";
			}
			else
			{
				string name = ((Object)qbObj.itemObject).name;
				camochallenge_bind2gun_text.text = FormatBind2GunText(name);
				gunchallenge_gunname_text.text = name;
			}
		}

		public string FormatProgressText(int progress, int target)
		{
			return "Progress: " + progress + "/" + target;
		}

		public string FormatChallMaxText(int progress, int target)
		{
			return "Gun Challenge Limit: " + progress + "/" + target + " (not including Mastery)";
		}

		public string FormatBoundGunText(string gunname)
		{
			return "Bound-to-gun: " + gunname;
		}

		public string FormatBind2GunText(string gunname)
		{
			return "Bind-to-gun: " + gunname;
		}

		public string FormatUnlocksCamoText(string camoname)
		{
			return "Unlocks camo: " + camoname;
		}

		public void NextSkin()
		{
			GameObject gameObject = ((Component)((Component)cheats_output).transform.parent).gameObject;
			gameObject.SetActive(!gameObject.activeSelf);
			if (gameObject.activeSelf)
			{
				output_text.text = "[CHEATS MENU ACTIVATED]\nCLUE: Keep talking and nobody explodes: 'HINT'.\nTip: Lotta easter eggs: " + cheatsDictionary.Keys.ElementAt(Random.Range(0, cheatsDictionary.Count));
			}
		}

		public bool ViewableCamoQuestExists()
		{
			if (camoavailable_quests != null && camoavailable_quests.Count > 0)
			{
				camocurr_quest_ix %= camoavailable_quests.Count;
				return true;
			}
			return false;
		}

		public bool ViewableGunQuestExists()
		{
			if (gunavailable_quests != null && gunavailable_quests.Count > 0)
			{
				guncurr_quest_ix %= gunavailable_quests.Count;
				return true;
			}
			return false;
		}

		public QBSlotResult GetQbObj()
		{
			QBSlotResult qBSlotResult = new QBSlotResult();
			if ((Object)(object)slot == (Object)null)
			{
				output_text.text = "Error: Unexpected, the Quickbelt slot is null!?";
				return qBSlotResult;
			}
			FVRPhysicalObject curObject = slot.CurObject;
			if ((Object)(object)curObject == (Object)null)
			{
				return qBSlotResult;
			}
			qBSlotResult.itemObject = ((Component)curObject).gameObject;
			FVRFireArm component = ((Component)curObject).gameObject.GetComponent<FVRFireArm>();
			if ((Object)(object)component == (Object)null)
			{
				qBSlotResult.isFirearm = false;
			}
			else
			{
				qBSlotResult.isFirearm = true;
			}
			return qBSlotResult;
		}

		public void BTN_NextCamoPage()
		{
			if (curr_camo_names_ordered.Count > camo_images_ordered.Count && starting_page_ix + camo_images_ordered.Count < curr_camo_names_ordered.Count)
			{
				starting_page_ix += camo_images_ordered.Count;
				RefreshCamoSelectImages();
			}
			else
			{
				output_text.text = "No more pages to see next.";
			}
		}

		public void BTN_PrevCamoPage()
		{
			if (starting_page_ix >= camo_images_ordered.Count)
			{
				starting_page_ix -= camo_images_ordered.Count;
				RefreshCamoSelectImages();
			}
			else
			{
				output_text.text = "No previous pages to see.";
			}
		}

		public void BTN_NextPack()
		{
			curr_pack_ix = (curr_pack_ix + 1) % pack_names.Count;
			output_text.text = "Changed to next camo Pack!";
			RefreshUI();
		}

		public void BTN_PrevPack()
		{
			curr_pack_ix = (curr_pack_ix - 1 + pack_names.Count) % pack_names.Count;
			output_text.text = "Changed to prev camo Pack!";
			RefreshUI();
		}

		public void BTN_NextType()
		{
			curr_type_ix = (curr_type_ix + 1) % curr_type_names.Count;
			output_text.text = "Changed to next type within camo pack: " + curr_pack_name;
			RefreshUI();
		}

		public void BTN_PrevType()
		{
			curr_type_ix = (curr_type_ix - 1 + curr_type_names.Count) % curr_type_names.Count;
			output_text.text = "Changed to prev type within camo pack: " + curr_pack_name;
			RefreshUI();
		}

		public void BTN_SelectCamoImage(int i)
		{
			if (curr_camo_names_ordered.Count > 0 && starting_page_ix + i < curr_camo_names_ordered.Count && i >= 0)
			{
				curr_camo_name = curr_camo_names_ordered[starting_page_ix + i];
				SpecialCamo val = MasteryCamos.LoadSpecialCamoFromFile(MasteryCamos.camoPacks.GetSpecialCamo(curr_camo_name));
				if (val != null)
				{
					currSpecCamo = val;
				}
				else if (currSpecCamo != null)
				{
					currSpecCamo.ColorTexture = curr_camo_name;
					currSpecCamo.CamoStrength = 1f;
				}
				RefreshAdvancedCamoUI();
				string text = default(string);
				string text2 = default(string);
				string text3 = default(string);
				MasteryCamos.ExtractPackAndType(curr_camo_name, ref text, ref text2, ref text3);
				camo_text_name.text = text3;
				camo_selected = true;
				output_text.text = "Selected camo named: " + text3;
				BTN_ShowCamoQuests();
			}
			else
			{
				Debug.LogError((object)("BTN_SelectCamoImage when no camo names or outside bounds " + i));
			}
		}

		private bool IsUnlockedOnCurrObj(string camoname)
		{
			if (MasteryCamos.questsState.unlockedCamos.Contains(camoname))
			{
				return true;
			}
			QBSlotResult qbObj = GetQbObj();
			if ((Object)(object)qbObj.itemObject == (Object)null)
			{
				return false;
			}
			if (!qbObj.isFirearm)
			{
				return true;
			}
			return MasteryCamos.questsState.GetMasteryCamosForGun(((Object)qbObj.itemObject).name).Contains(camoname);
		}

		private bool CheatUnlockCamos()
		{
			return unlock_all_camos_cheats;
		}

		public void BTN_ApplyCamo()
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			if (!camo_selected)
			{
				return;
			}
			QBSlotResult qbObj = GetQbObj();
			if ((Object)(object)qbObj.itemObject != (Object)null)
			{
				if (IsUnlockedOnCurrObj(curr_camo_name) || CheatUnlockCamos())
				{
					MasteryCamosGunState val = new MasteryCamosGunState();
					val.shader_params_v1 = currSpecCamo;
					MasteryCamos.ApplyCamo(qbObj.itemObject, val);
					output_text.text = "Applied camo!";
					SetIsPreview(value: false);
				}
				else
				{
					output_text.text = "NOPE!\nYou have not unlocked this camo! To do so, accept a camo challenge on a gun, complete the challenge, then come back here :)\n\nPSST! Don't listen to that >derogatory<, just enable cheats - The Devil (french)";
				}
			}
			else
			{
				output_text.text = "NOPE!\nNo object found in application area. Cannot apply camo!";
			}
		}

		public void BTN_PreviewCamo()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			if (camo_selected)
			{
				QBSlotResult qbObj = GetQbObj();
				if ((Object)(object)qbObj.itemObject != (Object)null)
				{
					MasteryCamosGunState val = new MasteryCamosGunState();
					val.shader_params_v1 = currSpecCamo;
					MasteryCamos.ApplyCamo(qbObj.itemObject, val);
					output_text.text = "WARNING!! If you remove the weapon before clicking APPLY Camo, current camo will be deleted.";
					SetIsPreview(value: true);
				}
				else
				{
					output_text.text = "NOPE!\nNo object found in application area. Cannot apply camo!";
				}
			}
		}

		public void BTN_RemoveCamo(GameObject optional_remove = null)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			QBSlotResult qbObj = GetQbObj();
			if ((Object)(object)qbObj.itemObject != (Object)null || (Object)(object)optional_remove != (Object)null)
			{
				GameObject val = ((!((Object)(object)optional_remove == (Object)null)) ? optional_remove : qbObj.itemObject);
				MasteryCamosGunState val2 = new MasteryCamosGunState();
				currSpecCamo = new SpecialCamo();
				currSpecCamo.CamoStrength = 0f;
				RefreshAdvancedCamoUI();
				val2.shader_params_v1 = currSpecCamo;
				if ((Object)(object)val == (Object)null)
				{
					Debug.LogError((object)"RemoveCamo obj to clean is null!");
				}
				MasteryCamos.ApplyCamo(val, val2);
				output_text.text = "Cleared camo!";
			}
			else
			{
				output_text.text = "NOPE!\nNo object found in application area. Cannot clear camo!";
			}
		}

		public void BTN_SetTutorialText()
		{
			output_text.text = TutorialText;
		}

		public void BTN_ShowGunQuests()
		{
			QBSlotResult qbObj = GetQbObj();
			if ((Object)(object)qbObj.itemObject != (Object)null)
			{
				if (qbObj.isFirearm)
				{
					string name = ((Object)qbObj.itemObject).name;
					MasteryCamos.BindMasteryQuestsToGun(name);
					output_text.text = "Showing challenges that are active on the current gun: " + name + "\nTip: You can remove any camo-challenge bound to a gun if you'd like to use a different challenge to unlock that camo.";
					gunavailable_quests.Clear();
					gunavailable_quests.AddRange(MasteryCamos.questsState.GetActiveCamosForGun(name));
					camocurr_quest_ix = 0;
					RefreshGunQuestsSection();
				}
				else
				{
					output_text.text = "NOPE!\nThe current object being painted " + ((Object)qbObj.itemObject).name + " is not a firearm. Only firearms have challenges and can unlock camos with them. You can still paint this object though.";
				}
			}
			else
			{
				output_text.text = "NOPE!\nI can't detect any items in the Paint Area behind me.";
			}
		}

		public void BTN_ShowCamoQuests()
		{
			if (camo_selected)
			{
				output_text.text = "Showing challenges that can unlock the current camo: " + curr_camo_name + "\nTip: You can now pick ANY challenge and bind it to the current gun. Completing the challenge will unlock the camo for ALL GUNS and ITEMS.\nTip: Only " + 1 + " camo challenge can be bound and completed per gun. Unlock more camos by using different guns.";
				camoavailable_quests.Clear();
				if (!IsUnlockedOnCurrObj(curr_camo_name))
				{
					camoavailable_quests.AddRange(QuestsPool.GetCamoQuests(curr_camo_name));
					camocurr_quest_ix = 0;
				}
				RefreshCamoQuestsSection();
			}
			else
			{
				output_text.text = "NOPE!\nI can't detect your selected camo, make sure you click on the image of the camo you want to unlock.";
			}
		}

		public void BTN_NextCamoQuest()
		{
			if (ViewableCamoQuestExists())
			{
				camocurr_quest_ix = (camocurr_quest_ix + 1) % camoavailable_quests.Count;
				RefreshCamoQuestsSection();
			}
			else
			{
				output_text.text = "NOPE!\nI don't see any camo challenges, can't go to the next one.";
			}
		}

		public void BTN_PrevCamoQuest()
		{
			if (ViewableCamoQuestExists())
			{
				camocurr_quest_ix = (camocurr_quest_ix - 1 + camoavailable_quests.Count) % camoavailable_quests.Count;
				RefreshCamoQuestsSection();
			}
			else
			{
				output_text.text = "NOPE!\nI don't see any camo challenges, can't go to the previous one.";
			}
		}

		public void BTN_NextGunQuest()
		{
			if (ViewableGunQuestExists())
			{
				guncurr_quest_ix = (guncurr_quest_ix + 1) % gunavailable_quests.Count;
				RefreshGunQuestsSection();
			}
			else
			{
				output_text.text = "NOPE!\nI don't see any gun challenges, can't go to the next one.";
			}
		}

		public void BTN_PrevGunQuest()
		{
			if (ViewableGunQuestExists())
			{
				guncurr_quest_ix = (guncurr_quest_ix - 1 + gunavailable_quests.Count) % gunavailable_quests.Count;
				RefreshGunQuestsSection();
			}
			else
			{
				output_text.text = "NOPE!\nI don't see any gun challenges, can't go to the previous one.";
			}
		}

		public void BTN_AcceptChallOnGun()
		{
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			QBSlotResult qbObj = GetQbObj();
			if ((Object)(object)qbObj.itemObject == (Object)null)
			{
				output_text.text = "NOPE!\nI can't detect any items in the Paint Area behind me that I can bind a challenge to.";
				return;
			}
			if (!qbObj.isFirearm)
			{
				output_text.text = "NOPE!\nThe current object " + ((Object)qbObj.itemObject).name + " is not a firearm. Only firearms can have challenges.\nTip: You can still apply camos to this object.";
				return;
			}
			if (!camo_selected)
			{
				output_text.text = "NOPE!\nI can't detect if you selected a camo, you need to click on one so I can bind it to this challenge.";
				return;
			}
			if (!ViewableCamoQuestExists())
			{
				output_text.text = "NOPE!\nCouldn't find any currently viewable challenges to bind. Make sure you selected 'View this camos Challenges'";
				return;
			}
			Quest val = camoavailable_quests[camocurr_quest_ix];
			if (val.gun_name != "")
			{
				output_text.text = "NOPE!\nThe selected challenge is already bound to a gun. Use the arrow buttons next to 'Challenges' to find one without a bound gun.";
				return;
			}
			if (val.camo_reward != "" && val.camo_reward != curr_camo_name)
			{
				output_text.text = "NOPE!\nThe selected challenge is specific to camo " + val.gun_name + " while the currently selected camo is called " + curr_camo_name + ". Try picking another challenge or this challenge's specific camo.";
				return;
			}
			val.current_progress = 0;
			string text = (val.gun_name = ((Object)qbObj.itemObject).name);
			int num = MasteryCamos.CountChallengesForGun(text);
			int maxQuestsPerGun = MasteryCamos.maxQuestsPerGun;
			if (num >= maxQuestsPerGun)
			{
				output_text.text = "NOPE!\nYou already have " + num + " many challenges bound to gun " + text + " which exceeds the current max at " + maxQuestsPerGun + "\nTip: Use a different gun to unlock this camo! Don't worry, H3VR has plenty!";
			}
			else
			{
				val.camo_reward = curr_camo_name;
				MasteryCamos.questsState.AddQuest(val);
				output_text.text = "Binded a challenge to the gun " + text + " to unlock camo " + curr_camo_name + "\nTip: You NEED to be holding the gun or have it in your QuickBelt while the scoring event happens. For example if you need to finish a TnH hold and immediately drop the weapon on the ground, the hold will not count towards progress.";
				FileStuff.RefreshSaveFile();
				BTN_ShowGunQuests();
			}
		}

		public void BTN_RemChallOnGun()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (ViewableGunQuestExists())
			{
				Quest val = gunavailable_quests[guncurr_quest_ix];
				if (val.gun_name != "")
				{
					if (val.current_progress < val.target_goal || CheatUnlockCamos())
					{
						MasteryCamos.questsState.RemoveQuest(val);
						output_text.text = "Unbound the previously selected challenge to unlock camo " + val.camo_reward + " from firearm " + val.gun_name;
						gunavailable_quests.Clear();
						FileStuff.RefreshSaveFile();
						BTN_ShowGunQuests();
					}
					else
					{
						output_text.text = "NOPE!\nYou can't remove already-completed challenges. This is how we track if you reached your challenge quota per gun.";
					}
				}
				else
				{
					output_text.text = "NOPE!\nThis quest does not have a gun bound to it, so I can't unbind it.";
				}
			}
			else
			{
				output_text.text = "NOPE!\nCouldn't find any currently viewable challenges to unbind. Make sure you selected 'View this XXX Challenges'";
			}
		}

		public void BTN_AdvancedToggle()
		{
			((Component)AdvancedUI).gameObject.SetActive(!((Component)AdvancedUI).gameObject.activeSelf);
		}

		public void BTN_ModStrength(float f)
		{
			if (currSpecCamo != null)
			{
				SpecialCamo obj = currSpecCamo;
				obj.CamoStrength += f;
				currSpecCamo.CamoStrength = Mathf.Round(currSpecCamo.CamoStrength * 10f) / 10f;
			}
			RefreshAdvancedCamoUI();
			BTN_PreviewCamo();
		}

		public void BTN_ModMetallic(float f)
		{
			if (currSpecCamo != null)
			{
				SpecialCamo obj = currSpecCamo;
				obj.CamoMetallic += f;
				currSpecCamo.CamoMetallic = Mathf.Round(currSpecCamo.CamoMetallic * 10f) / 10f;
			}
			RefreshAdvancedCamoUI();
			BTN_PreviewCamo();
		}

		public void BTN_ModRoughness(float f)
		{
			if (currSpecCamo != null)
			{
				SpecialCamo obj = currSpecCamo;
				obj.CamoRoughness += f;
				currSpecCamo.CamoRoughness = Mathf.Round(currSpecCamo.CamoRoughness * 10f) / 10f;
			}
			RefreshAdvancedCamoUI();
			BTN_PreviewCamo();
		}

		public void BTN_ModCamoMask(int number)
		{
			if (currSpecCamo != null)
			{
				string camoMaskChannel = currSpecCamo.CamoMaskChannel;
				string[] array = new string[5] { "", "metal", "rough", "spec", "ao" };
				string text = "";
				int num = Array.IndexOf(array, camoMaskChannel);
				if (num == -1)
				{
					num = 0;
				}
				int num2 = (num + number + array.Length) % array.Length;
				text = array[num2];
				currSpecCamo.CamoMaskChannel = text;
				RefreshAdvancedCamoUI();
				BTN_PreviewCamo();
			}
		}

		public void BTN_ModInvtMask()
		{
			if (currSpecCamo != null)
			{
				currSpecCamo.InvertCamoMask = !currSpecCamo.InvertCamoMask;
			}
			RefreshAdvancedCamoUI();
			BTN_PreviewCamo();
		}

		public void BTN_ModMove(int i)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			if (currSpecCamo != null)
			{
				Vector3 triplanarPosition = currSpecCamo.TriplanarPosition;
				switch (i)
				{
				case 1:
					triplanarPosition.z += 0.1f;
					break;
				case -1:
					triplanarPosition.z -= 0.1f;
					break;
				case 2:
					triplanarPosition.y -= 0.1f;
					break;
				case -2:
					triplanarPosition.y += 0.1f;
					break;
				}
				triplanarPosition.y = Mathf.Round(triplanarPosition.y * 10f) / 10f;
				triplanarPosition.z = Mathf.Round(triplanarPosition.z * 10f) / 10f;
				currSpecCamo.TriplanarPosition = triplanarPosition;
				RefreshAdvancedCamoUI();
				BTN_PreviewCamo();
			}
		}

		public void BTN_Scroll()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			if (currSpecCamo != null)
			{
				Vector3 scroll = currSpecCamo.Scroll;
				if (scroll.x != 0f || scroll.y != 0f || scroll.z != 0f)
				{
					scroll.x = 0f;
					scroll.y = 0f;
					scroll.z = 0f;
				}
				else
				{
					scroll.x = 0.1f;
					scroll.y = 0.1f;
					scroll.z = 0.1f;
				}
				currSpecCamo.Scroll = scroll;
				BTN_PreviewCamo();
			}
		}

		public void BTN_ModScale(int i)
		{
			if (currSpecCamo != null)
			{
				if (i > 0)
				{
					SpecialCamo obj = currSpecCamo;
					obj.Scale += 0.5f;
				}
				else
				{
					SpecialCamo obj2 = currSpecCamo;
					obj2.Scale -= 0.5f;
				}
				currSpecCamo.Scale = Mathf.Round(currSpecCamo.Scale * 2f) / 2f;
				RefreshAdvancedCamoUI();
				BTN_PreviewCamo();
			}
		}

		public void BTN_ModMaskMin(float i)
		{
			if (currSpecCamo != null)
			{
				SpecialCamo obj = currSpecCamo;
				obj.CamoMaskMin += i;
				currSpecCamo.CamoMaskMin = Mathf.Round(currSpecCamo.CamoMaskMin * 10f) / 10f;
				RefreshAdvancedCamoUI();
				BTN_PreviewCamo();
			}
		}

		public void BTN_ModMaskMax(float i)
		{
			if (currSpecCamo != null)
			{
				SpecialCamo obj = currSpecCamo;
				obj.CamoMaskMax += i;
				currSpecCamo.CamoMaskMax = Mathf.Round(currSpecCamo.CamoMaskMax * 10f) / 10f;
				RefreshAdvancedCamoUI();
				BTN_PreviewCamo();
			}
		}

		public void BTN_SetAsSecondary()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			if (currSpecCamo == null)
			{
				output_text.text = "Can't set secondary camo as there's no current camo being worked on.";
				return;
			}
			SpecialCamo val = new SpecialCamo();
			val.CamoStrength = 0f;
			val.SecondaryColorTexture = currSpecCamo.ColorTexture;
			val.SecondaryPBRTexture = currSpecCamo.PBRTexture;
			val.SecondaryCamoStrength = currSpecCamo.CamoStrength;
			val.SecondaryCamoPBRStrength = currSpecCamo.CamoPBRStrength;
			val.SecondaryCamoMetallic = currSpecCamo.CamoMetallic;
			val.SecondaryCamoRoughness = currSpecCamo.CamoRoughness;
			val.SecondaryCamoSpecularity = currSpecCamo.CamoSpecularity;
			val.SecondaryCamoMaskCutoff = currSpecCamo.CamoMaskMin;
			val.SecondaryCamoMaskInvert = currSpecCamo.InvertCamoMask;
			val.CamoMaskChannel = currSpecCamo.CamoMaskChannel;
			currSpecCamo = val;
			RefreshAdvancedCamoUI();
			BTN_PreviewCamo();
		}

		public void BTN_RemoveSecondary()
		{
			if (currSpecCamo == null)
			{
				output_text.text = "Can't remove secondary camo as there's no current camo being worked on.";
				return;
			}
			currSpecCamo.SecondaryColorTexture = "";
			currSpecCamo.SecondaryPBRTexture = "";
			RefreshAdvancedCamoUI();
			BTN_PreviewCamo();
		}

		public void BTN_CashMoneyToggle()
		{
			((Component)CashMoneyUI).gameObject.SetActive(!((Component)CashMoneyUI).gameObject.activeSelf);
			RefreshCashMoneyUI();
		}

		public void BTN_AddFunds()
		{
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Invalid comparison between Unknown and I4
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Invalid comparison between Unknown and I4
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Invalid comparison between Unknown and I4
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Invalid comparison between Unknown and I4
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Invalid comparison between Unknown and I4
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Invalid comparison between Unknown and I4
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Invalid comparison between Unknown and I4
			if ((Object)(object)CashMoneyUI == (Object)null)
			{
				Debug.LogError((object)"CashMoneyUI Camo UI no here");
				output_text.text = "Internal error: CashMoneyUI UI no here";
				return;
			}
			Transform val = CashMoneyUI.Find("QBSlotMoney");
			if ((Object)(object)val == (Object)null)
			{
				Debug.LogError((object)"QBSlotMoney in CashMoney no here");
				output_text.text = "Internal error: QBSlotMoney in CashMoney no here";
				return;
			}
			FVRQuickBeltSlot component = ((Component)val).GetComponent<FVRQuickBeltSlot>();
			if ((Object)(object)component == (Object)null)
			{
				Debug.LogError((object)"No FVR QB in CashMoney no here");
				output_text.text = "Internal error: No FVR QB in CashMoney no here";
				return;
			}
			FVRPhysicalObject curObject = component.CurObject;
			if ((Object)(object)curObject == (Object)null)
			{
				output_text.text = "CAN'T BUY! No .69 CashMoney bullets in the quickbelt-slot.";
				return;
			}
			FVRFireArmRound component2 = ((Component)curObject).gameObject.GetComponent<FVRFireArmRound>();
			if ((Object)(object)component2 == (Object)null)
			{
				output_text.text = "CAN'T BUY! The object in the quickbelt slot is not a .69 CashMoney bullet.";
				return;
			}
			if ((int)component2.RoundType != 993)
			{
				output_text.text = "CAN'T BUY! The round in the quickbelt slot is not a .69 CashMoney bullet.";
				return;
			}
			int num = 0;
			FireArmRoundClass roundClass = component2.RoundClass;
			if ((int)roundClass == 220)
			{
				num = 1;
			}
			else if ((int)roundClass == 221)
			{
				num = 5;
			}
			else if ((int)roundClass == 222)
			{
				num = 10;
			}
			else if ((int)roundClass == 223)
			{
				num = 20;
			}
			else if ((int)roundClass == 224)
			{
				num = 100;
			}
			else if ((int)roundClass == 225)
			{
				num = 1000;
			}
			GM.MMFlags.AGB(num);
			GM.MMFlags.SaveToFile();
			((FVRPhysicalObject)component2).ClearQuickbeltState();
			Object.Destroy((Object)(object)((Component)component2).gameObject);
			RefreshCamoSelectImages();
			RefreshCashMoneyUI();
		}

		public void BTN_BuyCamo()
		{
			if (!camo_selected)
			{
				output_text.text = "CAN'T buy camo, none is selected!";
				return;
			}
			string text = curr_camo_name;
			if (text.StartsWith("xx_mastery"))
			{
				output_text.text = "MASTERY camos cannot be purchased, INSTEAD try using the CAMO SPRAYCAN.";
				return;
			}
			if (MasteryCamos.questsState.unlockedCamos.Contains(text))
			{
				output_text.text = "Camo ALREADY UNLOCKED: " + text + ". No need to buy!";
				return;
			}
			int gB = GM.MMFlags.GB;
			int num = universalPrice;
			if (gB - universalPrice < 0)
			{
				output_text.text = "CAN'T AFFORD to buy at price " + num + " with current funds: " + gB;
			}
			else
			{
				MasteryCamos.questsState.UnlockCamo(text);
				GM.MMFlags.SGB(num);
				GM.MMFlags.SaveToFile();
				RefreshCamoSelectImages();
				RefreshCashMoneyUI();
			}
		}

		public uint GetCheatCodeAsInt()
		{
			if (uint.TryParse(cheats_output.text, out var result))
			{
				return result;
			}
			Debug.LogError((object)"Failed to parse cheat code to integer.");
			return 0u;
		}

		public void BTN_CheatCode(int i)
		{
			float time = Time.time;
			if (i == lastButtonPressed && time - lastPressTime < 0.5f)
			{
				pressCount = (pressCount + 1) % buttonMapping[i].Length;
			}
			else
			{
				if (lastButtonPressed != -1)
				{
					Text obj = cheats_output;
					obj.text += buttonMapping[lastButtonPressed][pressCount];
				}
				pressCount = 0;
			}
			lastButtonPressed = i;
			lastPressTime = time;
			cheats_output.text = cheats_output.text.Substring(0, cheats_output.text.Length - ((cheats_output.text.Length != 0 && lastButtonPressed == i) ? 1 : 0)) + buttonMapping[i][pressCount];
		}

		private uint CheatsGenerateHash()
		{
			string pluginPath = Paths.PluginPath;
			using SHA256 sHA = SHA256.Create();
			byte[] value = sHA.ComputeHash(Encoding.UTF8.GetBytes(pluginPath));
			uint num = BitConverter.ToUInt32(value, 0);
			return (uint)Math.Abs(num);
		}

		private string GenerateFreeCamosCode()
		{
			uint hash = CheatsGenerateHash();
			string text = ConvertHashToCheatCode(hash);
			cheatModelNumber.text = text;
			return text;
		}

		private string ConvertHashToCheatCode(uint hash)
		{
			char[] array = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
			StringBuilder stringBuilder = new StringBuilder(4);
			for (int i = 0; i < 4; i++)
			{
				stringBuilder.Append(array[hash % 26]);
				hash /= 26;
			}
			return stringBuilder.ToString();
		}

		public void BTN_CheckCheatCode()
		{
			if (cheatsDictionary.TryGetValue(cheats_output.text, out var value))
			{
				output_text.text = value;
				if (cheats_output.text == cheatModelNumber.text)
				{
					unlock_all_camos_cheats = true;
				}
				if (cheats_output.text == "GROW")
				{
					ApplyGrow(1.1f);
				}
				if (cheats_output.text == "SHRINK")
				{
					ApplyGrow(0.9f);
				}
			}
			else
			{
				output_text.text = "Invalid Cheat Code:" + cheats_output.text + ".";
			}
		}

		public void ApplyGrow(float multiplier)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			QBSlotResult qbObj = GetQbObj();
			if ((Object)(object)qbObj.itemObject != (Object)null)
			{
				Transform transform = qbObj.itemObject.transform;
				transform.localScale *= multiplier;
			}
		}

		public void BTN_BackspaceCheatCode()
		{
			if (cheats_output.text.Length > 0)
			{
				cheats_output.text = cheats_output.text.Substring(0, cheats_output.text.Length - 1);
			}
			if (cheats_output.text.Length == 0)
			{
				lastButtonPressed = -1;
				pressCount = 0;
			}
		}
	}
	public class QBSlotResult
	{
		public GameObject itemObject;

		public bool isFirearm;

		public QBSlotResult(GameObject itemObject = null, bool isFirearm = true)
		{
			this.itemObject = itemObject;
			this.isFirearm = isFirearm;
		}
	}
	public class ReszMan : MonoBehaviour
	{
		public FVRPhysicalObject parentOjb;

		public Transform MenuCanvas;

		public Transform MuzzlePoint;

		public float hitDist = 100f;

		public float waitSeconds = 0.5f;

		private Text uniformScaleTxt;

		private float uniScale = 1f;

		private Text X_ScaleTxt;

		private float X_Scale = 1f;

		private Text Z_ScaleTxt;

		private float Z_Scale = 1f;

		private Text Y_ScaleTxt;

		private float Y_Scale = 1f;

		private Text uniformMultpTxt;

		private float uniMult = 0.1f;

		private Text X_MultpTxt;

		private float X_Mult = 0.1f;

		private Text Z_MultpTxt;

		private float Z_Mult = 0.1f;

		private Text Y_MultpTxt;

		private float Y_Mult = 0.1f;

		private bool usingUniform = false;

		private float nextApplyTime;

		private void Start()
		{
			FindUiVariables();
			nextApplyTime = waitSeconds;
		}

		public void FindUiVariables()
		{
			if ((Object)(object)MenuCanvas == (Object)null)
			{
				Debug.LogError((object)"MenuCanvas UI no here");
				return;
			}
			Transform val = MenuCanvas.Find("ScaleOptions");
			if (!((Object)(object)val != (Object)null))
			{
				return;
			}
			uniformMultpTxt = ((Component)val.Find("UniOrd")).GetComponent<Text>();
			if ((Object)(object)uniformMultpTxt != (Object)null)
			{
				uniformMultpTxt.text = "+" + uniMult;
			}
			X_MultpTxt = ((Component)val.Find("XOrd")).GetComponent<Text>();
			if ((Object)(object)X_MultpTxt != (Object)null)
			{
				X_MultpTxt.text = "+" + X_Mult;
			}
			Y_MultpTxt = ((Component)val.Find("YOrd")).GetComponent<Text>();
			if ((Object)(object)Y_MultpTxt != (Object)null)
			{
				Y_MultpTxt.text = "+" + Y_Mult;
			}
			Z_MultpTxt = ((Component)val.Find("ZOrd")).GetComponent<Text>();
			if ((Object)(object)Z_MultpTxt != (Object)null)
			{
				Z_MultpTxt.text = "+" + Z_Mult;
			}
			uniformScaleTxt = ((Component)val.Find("UniScale_")).GetComponent<Text>();
			if ((Object)(object)uniformScaleTxt != (Object)null)
			{
				if (usingUniform)
				{
					uniformScaleTxt.text = "A: " + uniScale;
				}
				else
				{
					uniformScaleTxt.text = "XZY not same";
				}
			}
			X_ScaleTxt = ((Component)val.Find("XScale_")).GetComponent<Text>();
			if ((Object)(object)X_ScaleTxt != (Object)null)
			{
				X_ScaleTxt.text = "X: " + X_Scale;
			}
			Y_ScaleTxt = ((Component)val.Find("YScale_")).GetComponent<Text>();
			if ((Object)(object)Y_ScaleTxt != (Object)null)
			{
				Y_ScaleTxt.text = "Y: " + Y_Scale;
			}
			Z_ScaleTxt = ((Component)val.Find("ZScale_")).GetComponent<Text>();
			if ((Object)(object)Z_ScaleTxt != (Object)null)
			{
				Z_ScaleTxt.text = "Z: " + Z_Scale;
			}
		}

		public void RefreshUi()
		{
			if ((Object)(object)MenuCanvas == (Object)null)
			{
				Debug.LogError((object)"MenuCanvas UI no here");
				return;
			}
			Transform val = MenuCanvas.Find("ScaleOptions");
			if (!((Object)(object)val != (Object)null))
			{
				return;
			}
			if ((Object)(object)uniformMultpTxt != (Object)null)
			{
				uniformMultpTxt.text = "+" + uniMult;
			}
			if ((Object)(object)X_MultpTxt != (Object)null)
			{
				X_MultpTxt.text = "+" + X_Mult;
			}
			if ((Object)(object)Y_MultpTxt != (Object)null)
			{
				Y_MultpTxt.text = "+" + Y_Mult;
			}
			if ((Object)(object)Z_MultpTxt != (Object)null)
			{
				Z_MultpTxt.text = "+" + Z_Mult;
			}
			if ((Object)(object)uniformScaleTxt != (Object)null)
			{
				if (usingUniform)
				{
					uniformScaleTxt.text = "A: " + uniScale;
				}
				else
				{
					uniformScaleTxt.text = "XZY not same";
				}
			}
			if ((Object)(object)X_ScaleTxt != (Object)null)
			{
				X_ScaleTxt.text = "X: " + X_Scale;
			}
			if ((Object)(object)Y_ScaleTxt != (Object)null)
			{
				Y_ScaleTxt.text = "Y: " + Y_Scale;
			}
			if ((Object)(object)Z_ScaleTxt != (Object)null)
			{
				Z_ScaleTxt.text = "Z: " + Z_Scale;
			}
		}

		public void BTN_ScaleAdd(int axzy)
		{
			ScaleAdjust(axzy, 1f);
		}

		public void BTN_ScaleSub(int axzy)
		{
			ScaleAdjust(axzy, -1f);
		}

		public void ScaleAdjust(int axzy, float addsub)
		{
			switch (axzy)
			{
			case 0:
				uniScale += uniMult * addsub;
				uniScale = Mathf.Round(uniScale * 100f) / 100f;
				X_Scale = uniScale;
				Z_Scale = uniScale;
				Y_Scale = uniScale;
				usingUniform = true;
				break;
			case 1:
				X_Scale += X_Mult * addsub;
				X_Scale = Mathf.Round(X_Scale * 100f) / 100f;
				usingUniform = false;
				break;
			case 2:
				Z_Scale += Z_Mult * addsub;
				Z_Scale = Mathf.Round(Z_Scale * 100f) / 100f;
				usingUniform = false;
				break;
			case 3:
				Y_Scale += Y_Mult * addsub;
				Y_Scale = Mathf.Round(Y_Scale * 100f) / 100f;
				usingUniform = false;
				break;
			default:
				Debug.LogError((object)"ERROR: BTN_ScaleAdjust sent bad axzy int");
				return;
			}
			RefreshUi();
		}

		public void BTN_MultpAdjust(int axzy)
		{
			switch (axzy)
			{
			case 0:
				uniMult = FindNextMult(uniformMultpTxt.text);
				break;
			case 1:
				X_Mult = FindNextMult(X_MultpTxt.text);
				break;
			case 2:
				Z_Mult = FindNextMult(Z_MultpTxt.text);
				break;
			case 3:
				Y_Mult = FindNextMult(Y_MultpTxt.text);
				break;
			default:
				Debug.LogError((object)"ERROR: BTN_MultpAdjust sent bad axzy int");
				return;
			}
			RefreshUi();
		}

		public float FindNextMult(string curr_text)
		{
			float[] array = new float[4] { 1f, 10f, 0.01f, 0.1f };
			string[] array2 = new string[4] { "+1", "+10", "+0.01", "+0.1" };
			int num = Array.IndexOf(array2, curr_text);
			if (num == -1)
			{
				num = 0;
			}
			int num2 = (num + 1 + array.Length) % array.Length;
			return array[num2];
		}

		public void BTN_ReadValues()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			Transform val = FindWhatPointing();
			if ((Object)(object)val == (Object)null)
			{
				Debug.LogWarning((object)"Pointer didn't find anything.");
				return;
			}
			X_Scale = val.localScale.x;
			Y_Scale = val.localScale.y;
			Z_Scale = val.localScale.z;
			usingUniform = false;
			RefreshUi();
		}

		public void BTN_ApplyValues()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			Transform val = FindWhatPointing();
			if ((Object)(object)val == (Object)null)
			{
				Debug.LogWarning((object)"Pointer didn't find anything.");
				return;
			}
			Vector3 localScale = val.localScale;
			localScale.x = X_Scale;
			localScale.y = Y_Scale;
			localScale.z = Z_Scale;
			val.localScale = localScale;
		}

		private Transform FindWhatPointing()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(MuzzlePoint.position, MuzzlePoint.forward, ref val, hitDist))
			{
				GameObject gameObject = ((Component)((RaycastHit)(ref val)).collider).gameObject;
				Transform val2 = gameObject.transform;
				while ((Object)(object)val2.parent != (Object)null)
				{
					val2 = val2.parent;
				}
				return val2;
			}
			return null;
		}

		public void BTN_UndoLast()
		{
		}

		private void Update()
		{
			if ((Object)(object)((FVRInteractiveObject)parentOjb).m_hand != (Object)null && ((FVRInteractiveObject)parentOjb).m_hand.Input.TriggerFloat > 0.8f)
			{
				nextApplyTime -= Time.deltaTime;
				if (nextApplyTime <= 0f)
				{
					BTN_ApplyValues();
					nextApplyTime = waitSeconds;
				}
			}
		}
	}
	public class SprayCamoizer : MonoBehaviour
	{
		public HairsprayCan theCan;

		public float waitSeconds = 0.5f;

		private float nextApplyTime;

		private MasteryCamosGunState theGunState;

		private void Start()
		{
			nextApplyTime = waitSeconds;
			FindInnerCamo();
		}

		private void Update()
		{
			if (theCan.AudSource_Loop.isPlaying)
			{
				nextApplyTime -= Time.deltaTime;
				if (nextApplyTime <= 0f)
				{
					ApplyCamo();
					nextApplyTime = waitSeconds;
				}
			}
		}

		private void ApplyCamo()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected O, but got Unknown
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(theCan.Muzzle.position, theCan.Muzzle.forward, ref val, theCan.SprayDistance))
			{
				GameObject gameObject = ((Component)((RaycastHit)(ref val)).collider).gameObject;
				Transform val2 = gameObject.transform;
				while ((Object)(object)val2.parent != (Object)null)
				{
					val2 = val2.parent;
				}
				FindInnerCamo();
				if ((Object)(object)theGunState != (Object)null)
				{
					MasteryCamosGunState val3 = new MasteryCamosGunState();
					val3.CopyState(theGunState);
					MasteryCamos.ApplyCamo(((Component)val2).gameObject, val3);
				}
			}
		}

		public void FindInnerCamo()
		{
			theGunState = ((Component)this).GetComponent<MasteryCamosGunState>();
		}
	}
}
