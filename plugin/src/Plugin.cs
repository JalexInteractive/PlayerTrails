using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FistVR;
using UnityEngine;
using System.Collections;

namespace JalexInteractive
{
    [BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    public partial class PlayerTrails : BaseUnityPlugin
    {
        /* == Quick Start == 
         * Your plugin class is a Unity MonoBehaviour that gets added to a global game object when the game starts.
         * You should use Awake to initialize yourself, read configs, register stuff, etc.
         * If you need to use Update or other Unity event methods those will work too.
         *
         * Some references on how to do various things:
         * Adding config settings to your plugin: https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/4_configuration.html
         * Hooking / Patching game methods: https://harmony.pardeike.net/articles/patching.html
         * Also check out the Unity documentation: https://docs.unity3d.com/560/Documentation/ScriptReference/index.html
         * And the C# documentation: https://learn.microsoft.com/en-us/dotnet/csharp/
         */
        
        // Config Globals
        // Toggles
        private ConfigEntry<bool> cfg_headEnabled;
        private ConfigEntry<bool> cfg_lHandEnabled;
        private ConfigEntry<bool> cfg_rHandEnabled;
        // Offsets
        private ConfigEntry<float> cfg_headOffsetX;
        private ConfigEntry<float> cfg_headOffsetY;
        private ConfigEntry<float> cfg_headOffsetZ;
        private ConfigEntry<float> cfg_lHandOffsetX;
        private ConfigEntry<float> cfg_lHandOffsetY;
        private ConfigEntry<float> cfg_lHandOffsetZ;
        private ConfigEntry<float> cfg_rHandOffsetX;
        private ConfigEntry<float> cfg_rHandOffsetY;
        private ConfigEntry<float> cfg_rHandOffsetZ;
        // Material Properties
        private ConfigEntry<string> cfg_trailTex;

        // TODO
        // R
        // G
        // B
        // A
        // For each body part, start and end colour, ugh

        // GameObject Globals
        public GameObject body;
        public GameObject lHand;
        public GameObject rHand;
        public TrailRenderer bodyTrail;          
        public TrailRenderer lHandTrail;          
        public TrailRenderer rHandTrail;
        public Material trailMat;
        public Texture2D trailTex;
        public bool parented = false;
        private Vector3 bodyOffset;
        private Vector3 lHandOffset;
        private Vector3 rHandOffset;
        private void Awake()
        {
            Logger = base.Logger;
            // Your plugin's ID, Name, and Version are available here.
            //Logger.LogMessage($"Hello, world! Sent from {Id} {Name} {Version}");
            Logger.LogMessage($"~ {Name} Version {Version} by Jalex Interactive - Initalising ~");

            // Config Setup
            cfg_headEnabled = Config.Bind("Toggles", "Head trail On/Off", true, "Turn the head trail on or off.");
            cfg_lHandEnabled = Config.Bind("Toggles", "Left hand trail On/Off", true, "Turn the left hand trail on or off.");
            cfg_rHandEnabled = Config.Bind("Toggles", "Right hand On/Off", true, "Turn the right hand trail on or off.");
            cfg_headOffsetX = Config.Bind("Offsets", "Head Offset X", 0f, "Vector3 for the offset of the head/body left/right position");
            cfg_headOffsetY = Config.Bind("Offsets", "Head Offset Y", -1f, "Vector3 for the offset of the head/body up/down position");
            cfg_headOffsetZ = Config.Bind("Offsets", "Head Offset Z", 0f, "Vector3 for the offset of the head/body forward/back position");
            cfg_lHandOffsetX = Config.Bind("Offsets", "Left Hand Offset X", 0f, "Vector3 for the offset of the left hand left/right position");
            cfg_lHandOffsetY = Config.Bind("Offsets", "Left Hand Offset Y", 0f, "Vector3 for the offset of the left hand up/down position");
            cfg_lHandOffsetZ = Config.Bind("Offsets", "Left Hand Offset Z", -0.21f, "Vector3 for the offset of the left hand forward/back position"); // Might need to change these as this is controlling up/down visuallyin game.
            cfg_rHandOffsetX = Config.Bind("Offsets", "Right Hand Offset X", 0f, "Vector3 for the offset of the right hand left/right position");
            cfg_rHandOffsetY = Config.Bind("Offsets", "Right Hand Offset Y", 0f, "Vector3 for the offset of the right hand up/down position");
            cfg_rHandOffsetZ = Config.Bind("Offsets", "Right Hand Offset Z", -0.21f, "Vector3 for the offset of the right hand forward/back position"); // Might need to change these as this is controlling up/down visuallyin game.
            cfg_trailTex = Config.Bind("Material Properties", "Path to image", "textures/white.png", "Path to the image file used for the trail material");

            // GameObject Setup
            body = new GameObject("BodyTrail");
            lHand = new GameObject("LHandTrail");
            rHand = new GameObject("RHandTrail");

            // Material Setup
            Shader AlloyShader = Shader.Find("Alloy/Core");
            trailMat = new Material(AlloyShader){name = "TrailMaterial"};
            trailTex = new Texture2D(128, 128){name = "TrailTexture"};
            // Render modes - 0 opaque - 1 Cutout - 2 Fade - 3 Transparent
            trailMat.SetFloat("RenderingMode", 3);
                // Metallic to 0
            trailMat.SetFloat("_Metal", 0);
                // Colour to white with no transparency (will be handled by trail renderer)
            string url = "file://" + cfg_trailTex.Value;
            Logger.LogMessage("~ Grabbing texture from: " + url + " ~");
            TextureGrab(url);
            trailMat.SetTexture("_MainTex", trailTex);
            trailMat.SetColor("_Color", new Color32(255, 255, 255, 255));
            // trailMat.SetFloat("Emission", 1);
            // trailMat.SetTexture("_EmissionMap", trailTex);

            // TrailRenderer Setup
            bodyTrail = body.AddComponent(typeof(TrailRenderer)) as TrailRenderer;
            lHandTrail = lHand.AddComponent(typeof(TrailRenderer)) as TrailRenderer;
            rHandTrail = rHand.AddComponent(typeof(TrailRenderer)) as TrailRenderer;

            bodyTrail.startWidth = 0.2f;
            lHandTrail.startWidth = rHandTrail.startWidth = 0.05f;
            bodyTrail.endWidth = lHandTrail.endWidth =  rHandTrail.endWidth = 0f;

            bodyTrail.time = 3f;
            lHandTrail.time = rHandTrail.time = 1f;

            bodyTrail.material = lHandTrail.material = rHandTrail.material = trailMat;

            bodyTrail.startColor = lHandTrail.startColor = rHandTrail.startColor = new Color32(0, 255, 0, 255);
            bodyTrail.endColor = lHandTrail.endColor = rHandTrail.endColor = new Color32(0, 0, 0, 0);

            bodyOffset.x = cfg_headOffsetX.Value;
            bodyOffset.y = cfg_headOffsetY.Value;
            bodyOffset.z = cfg_headOffsetZ.Value;
            lHandOffset.x = cfg_lHandOffsetX.Value;
            lHandOffset.y = cfg_lHandOffsetY.Value;
            lHandOffset.z = cfg_lHandOffsetZ.Value;
            rHandOffset.x = cfg_rHandOffsetX.Value;
            rHandOffset.y = cfg_rHandOffsetY.Value;
            rHandOffset.z = cfg_rHandOffsetZ.Value;

            // Turn off after setup if config says so.
            if (!cfg_headEnabled.Value)
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
        }
        private void LateUpdate()
        {
            if (GM.CurrentPlayerBody && !parented) {
            Logger.LogMessage("~ Binding trails to player ~");
            bodyTrail.transform.SetParent(GM.CurrentPlayerBody.Head, false);
            lHandTrail.transform.SetParent(GM.CurrentPlayerBody.LeftHand, false);
            rHandTrail.transform.SetParent(GM.CurrentPlayerBody.RightHand, false);
            bodyTrail.transform.localPosition = bodyOffset;
            lHandTrail.transform.localPosition = lHandOffset;
            rHandTrail.transform.localPosition = rHandOffset;
            parented = true;
            }
        }

        public IEnumerator TextureGrab(string url)
        {
            WWW textureFile = new(url);
            yield return textureFile;
            textureFile.LoadImageIntoTexture(trailTex);
        }
        
        // The line below allows access to your plugin's logger from anywhere in your code, including outside of this file.
        // Use it with 'YourPlugin.Logger.LogInfo(message)' (or any of the other Log* methods)
        internal new static ManualLogSource Logger { get; private set; }

    }
}
