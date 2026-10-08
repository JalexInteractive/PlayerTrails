using FistVR;
using BepInEx;
using Sodalite;
using System.IO;
using UnityEngine;
using Sodalite.Api;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;
using MonoMod.Utils;
using System;

namespace JalexInteractive
{
    [BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    [BepInDependency("nrgill28.Sodalite", "1.4.1")]
    public partial class PlayerTrails : BaseUnityPlugin
    {    
        // Config Globals
            // String, Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double, Decimal, Enum, Color, Vector2, Vector3, Vector4, Quaternion
        // Toggles
        public static ConfigEntry<bool> cfg_bodyEnabled;
        public static ConfigEntry<bool> cfg_lHandEnabled;
        public static ConfigEntry<bool> cfg_rHandEnabled;
        // Offsets
        public static ConfigEntry<Vector3> cfg_bodyOffset;
        public static ConfigEntry<Vector3> cfg_lHandOffset;
        public static ConfigEntry<Vector3> cfg_rHandOffset;
        // Material Properties
        public static ConfigEntry<string> cfg_trailTex;
        public static ConfigEntry<Vector4> cfg_startColour;
        public static ConfigEntry<Vector4> cfg_endColour;
        // Trail Properties
        public static ConfigEntry<float> cfg_bodyTime;
        public static ConfigEntry<float> cfg_lHandTime;
        public static ConfigEntry<float> cfg_rHandTime;
        public static ConfigEntry<float> cfg_bodyStartWidth;
        public static ConfigEntry<float> cfg_lHandStartWidth;
        public static ConfigEntry<float> cfg_rHandStartWidth;
        public static ConfigEntry<float> cfg_bodyEndWidth;
        public static ConfigEntry<float> cfg_lHandEndWidth;
        public static ConfigEntry<float> cfg_rHandEndWidth;

        // TODO
        // Corner Cap vertices maybe
        // End Cap vertices maybe

        // GameObject Globals
        public static GameObject body;
        public static GameObject lHand;
        public static  GameObject rHand;
        private GameObject optionsPanelPrefab;
        public static TrailRenderer bodyTrail;          
        public static TrailRenderer lHandTrail;          
        public static TrailRenderer rHandTrail;
        public static Material trailMat;
        public static Texture2D trailTex;
        public bool parented = false;
        public static Vector3 bodyOffset;
        public static Vector3 lHandOffset;
        public static Vector3 rHandOffset;
        public static Color32 startColour;
        public static Color32 endColour;
        public static string basePath;
        public Scene activeScene;
        public GameObject panel = null;
        private bool spawnedThisScene = false;

        private void Awake()
        {
            Logger = base.Logger;
            // Your plugin's ID, Name, and Version are available here.
            //Logger.LogMessage($"Hello, world! Sent from {Id} {Name} {Version}");
            Logger.LogMessage($"~ {Name} Version {Version} by Jalex Interactive - Initalising ~");

            // Config Setup - These are appearing in R2 bottom to top so consider switching the order round
            Config.SaveOnConfigSet = true;
                // Toggles
            cfg_bodyEnabled = Config.Bind("Toggles", "Head trail On/Off", true, "Turn the head/body trail on or off.");
            cfg_lHandEnabled = Config.Bind("Toggles", "Left hand trail On/Off", true, "Turn the left hand trail on or off.");
            cfg_rHandEnabled = Config.Bind("Toggles", "Right hand On/Off", true, "Turn the right hand trail on or off.");
             // Offsets
            Vector3 defaultBodyOffset = new(0f, -1f, 0f);
            cfg_bodyOffset = Config.Bind("Offsets", "Head Offset", defaultBodyOffset, "Offset for the head/body trail position (x,y,z)");
            Vector3 defaultlHandOffset = new(0f, 0f, -0.21f);
            cfg_lHandOffset = Config.Bind("Offsets", "Left Hand Offset", defaultlHandOffset, "Offset for the left hand trail position (x,y,z)");
            Vector3 defaultrHandOffset = new(0f, 0f, -0.21f);
            cfg_rHandOffset = Config.Bind("Offsets", "Right Hand Offset", defaultrHandOffset, "Offset for the right hand trail position (x,y,z)");
                // Material Properties
            cfg_trailTex = Config.Bind("Material Properties", "Image name", "blank.png", "Image file used for the trail texture. Add your own by adding them to the textures folder in this mods directory (png and jpg only.)");
            Vector4 defaultWhite = new(255, 255, 255, 255);
            cfg_startColour = Config.Bind("Material Properties", "Start Colour", defaultWhite, "Start colour of the trail");
            Vector4 defaultInvisible = new(255, 255, 255, 0);
            cfg_endColour = Config.Bind("Material Properties", "End Colour", defaultInvisible, "End colour of the trail");
                // Trail Properties - Maybe consider grouping hands together for this section
            cfg_bodyTime = Config.Bind("Trail Properties", "Body Time", 3f, "Time before the head/body trail fades (higher values mean longer trails)");
            cfg_lHandTime = Config.Bind("Trail Properties", "Left Hand Time", 1f, "Time before the left hand trail fades (higher values mean longer trails)");
            cfg_rHandTime = Config.Bind("Trail Properties", "Right Hand Time", 1f, "Time before the right hand trail fades (higher values mean longer trails)");
            cfg_bodyStartWidth = Config.Bind("Trail Properties", "Body Start Width", 0.2f, "Width of the body trail at the start of the trail");
            cfg_lHandStartWidth = Config.Bind("Trail Properties", "Left Hand Start Width", 0.05f, "Width of the left hand trail at the start of the trail");
            cfg_rHandStartWidth = Config.Bind("Trail Properties", "Right Hand Start Width", 0.05f, "Width of the right hand trail at the start of the trail");
            cfg_bodyEndWidth = Config.Bind("Trail Properties", "Body End Width", 0f, "Width of the body trail at the end of the trail");
            cfg_lHandEndWidth = Config.Bind("Trail Properties", "Left Hand End Width", 0f, "Width of the left hand trail at the end of the trail");
            cfg_rHandEndWidth = Config.Bind("Trail Properties", "Right Hand End Width", 0f, "Width of the right hand trail at the end of the trail");

            TrailSetup();

            // Turn off after setup if config says so.
            if (!cfg_bodyEnabled.Value)
            {
                body.SetActive(false);
            }
            if (!cfg_lHandEnabled.Value)
            {
                lHand.SetActive(false);
            }
            if (!cfg_rHandEnabled.Value)
            {
                rHand.SetActive(false);
            }

            // Load options panel and add wrist menu option to spawn it
            var panelBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), "playertrailspanel_preload"));
            optionsPanelPrefab = panelBundle.LoadAsset<GameObject>("PlayerTrailsPanel");
            if (optionsPanelPrefab)
            {
                WristMenuAPI.Buttons.Add(new WristMenuButton("Player Trails Panel", SpawnTrailsPanel));
            }
            else
            {
                Logger.LogError("~ Oopsie woopsie I made a fucky wucky ~");
            }
            activeScene = SceneManager.GetActiveScene();
        }
        private void TrailSetup()
        {
            // GameObject Setup
            body = new GameObject("BodyTrail");
            lHand = new GameObject("LHandTrail");
            rHand = new GameObject("RHandTrail");

            // Material Setup Unlit/Transparent
                // Create objects
            trailMat = new Material(Shader.Find("Alloy/Particles/Additive (Soft)")){name = "TrailMaterial"};
            trailTex = new Texture2D(128, 128, TextureFormat.DXT5, false){name = "TrailTexture",};
                // Grab texture from disk
                // Log base path for panel to access
            basePath = Path.GetDirectoryName(Info.Location) + "\\textures\\";
            string url = basePath + cfg_trailTex.Value;
            if (File.Exists(url))
            {
                Logger.LogMessage("~ Grabbing texture from: " + url + " ~");
                trailTex.LoadImage(TextureGrab(url));
            } else
            {
                Logger.LogError("~ Current texture does not exist. Defaulting to blank. ~");
            }
            trailMat.SetTexture("_MainTex", trailTex);
            // Flip texture so it reads correctly in game
            trailMat.SetTextureScale("_MainTex", new Vector2(-1,1));
                // Colour to white with no transparency (will be handled by trail renderer)
            trailMat.SetColor("_Color", new Color32(255, 255, 255, 255));

            // TrailRenderer Setup
                // Create components
            bodyTrail = body.AddComponent(typeof(TrailRenderer)) as TrailRenderer;
            lHandTrail = lHand.AddComponent(typeof(TrailRenderer)) as TrailRenderer;
            rHandTrail = rHand.AddComponent(typeof(TrailRenderer)) as TrailRenderer;
                // Width
            bodyTrail.startWidth = cfg_bodyStartWidth.Value;
            lHandTrail.startWidth = cfg_lHandStartWidth.Value;
            rHandTrail.startWidth = cfg_rHandStartWidth.Value;
            bodyTrail.endWidth = cfg_bodyEndWidth.Value;
            lHandTrail.endWidth = cfg_lHandEndWidth.Value;
            rHandTrail.endWidth = cfg_rHandEndWidth.Value;
                // Time
            bodyTrail.time = cfg_bodyTime.Value;
            lHandTrail.time = cfg_lHandTime.Value;
            rHandTrail.time = cfg_rHandTime.Value;
                // Smoothness
            bodyTrail.numCornerVertices = lHandTrail.numCornerVertices = rHandTrail.numCornerVertices = 5;
                // Texture setup
            bodyTrail.textureMode = lHandTrail.textureMode = rHandTrail.textureMode = LineTextureMode.RepeatPerSegment;
            bodyTrail.material = lHandTrail.material = rHandTrail.material = trailMat;
                // Colour setup
            startColour = V4ToColor32(cfg_startColour.Value);
            bodyTrail.startColor = lHandTrail.startColor = rHandTrail.startColor = startColour;
            endColour = V4ToColor32(cfg_endColour.Value);
            bodyTrail.endColor = lHandTrail.endColor = rHandTrail.endColor = endColour;
                //Offset Setup
            bodyOffset = cfg_bodyOffset.Value;
            lHandOffset = cfg_lHandOffset.Value;
            rHandOffset = cfg_rHandOffset.Value;
        }
        private void LateUpdate()
        {
            if (GM.CurrentPlayerBody && !parented) {
                if (!body)
                {
                    Logger.LogMessage("~ Respawning trails ~");
                    TrailSetup();
                }
            Logger.LogMessage("~ Binding trails to player ~");
            bodyTrail.transform.SetParent(GM.CurrentPlayerBody.Head, false);
            lHandTrail.transform.SetParent(GM.CurrentPlayerBody.LeftHand, false);
            rHandTrail.transform.SetParent(GM.CurrentPlayerBody.RightHand, false);
            bodyTrail.transform.localPosition = bodyOffset;
            lHandTrail.transform.localPosition = lHandOffset;
            rHandTrail.transform.localPosition = rHandOffset;
            parented = true;
            }
            if (!bodyTrail)
            {
                Logger.LogMessage("~ Trail destroyed - unparenting for respawn ~");
                parented = false;
            }
        }       
        public static byte[] TextureGrab(string url)
        {
                var bytes = System.IO.File.ReadAllBytes(url);
                return bytes;
        }
        private void SpawnTrailsPanel(object sender, ButtonClickEventArgs args)
            {
                // If on Main Menu (because otherwise it won't trigger there) or not the active scene
                if (activeScene.name == "MainMenu3" | activeScene != SceneManager.GetActiveScene())
                    {
                        // If it was option B, change spawned to false and reset active scene
                        if (activeScene != SceneManager.GetActiveScene()) {spawnedThisScene = false;}
                        activeScene = SceneManager.GetActiveScene();
                        // If not spawned this scene, spawn it and set spawned to true
                        if (!spawnedThisScene) {panel = Instantiate(optionsPanelPrefab);}
                        spawnedThisScene = true;
                    }
                // Pull panel from the ether
                args.Hand.OtherHand.RetrieveObject(panel.GetComponent<FVRPhysicalObject>());
            }
        public static Vector4 Color32ToV4(Color32 colour)
        {
             return new Vector4(
                colour.r,
                colour.g,
                colour.b,
                colour.a);
        }
        public static Color32 V4ToColor32(Vector4 v4)
        {
            Color tempColour = v4 / 255;
            Color32 sendColour = tempColour;
            return sendColour;
        }
        // The line below allows access to your plugin's logger from anywhere in your code, including outside of this file.
        // Use it with 'YourPlugin.Logger.LogInfo(message)' (or any of the other Log* methods)
        internal new static ManualLogSource Logger { get; private set; }
    }
}
