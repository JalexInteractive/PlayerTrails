using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FistVR;
using UnityEngine;

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
        // private ConfigEntry<UnityEngine.Vector3> headOffset; apparently no vector3s allowed
        // private ConfigEntry<Vector3> lHandOffset;
        // private ConfigEntry<Vector3> rHandOffset;
        // Head
        private ConfigEntry<float> cfg_headOffsetX;
        private ConfigEntry<float> cfg_headOffsetY;
        private ConfigEntry<float> cfg_headOffsetZ;
        // Left Hand
        private ConfigEntry<float> cfg_lHandOffsetX;
        private ConfigEntry<float> cfg_lHandOffsetY;
        private ConfigEntry<float> cfg_lHandOffsetZ;
        // Right Hand
        private ConfigEntry<float> cfg_rHandOffsetX;
        private ConfigEntry<float> cfg_rHandOffsetY;
        private ConfigEntry<float> cfg_rHandOffsetZ;

        // GameObject Globals
        private GameObject body;
        private GameObject lHand;
        private GameObject rHand;
        private Vector3 bodyOffset;
        private Vector3 lHandOffset;
        private Vector3 rHandOffset;
        private void Awake()
        {
            Logger = base.Logger;
            // Your plugin's ID, Name, and Version are available here.
            //Logger.LogMessage($"Hello, world! Sent from {Id} {Name} {Version}");
            Logger.LogMessage($"PlayerTrails Version {Version} by {Name}. Initalising.");

            // Config Setup
            cfg_headOffsetX = Config.Bind("Offsets", "Head Offset X", 0f, "Vector3 for the offset of the head/body left/right position");
            cfg_headOffsetY = Config.Bind("Offsets", "Head Offset Y", -1f, "Vector3 for the offset of the head/body up/down position");
            cfg_headOffsetZ = Config.Bind("Offsets", "Head Offset Z", 0f, "Vector3 for the offset of the head/body forward/back position");
            cfg_lHandOffsetX = Config.Bind("Offsets", "Left Hand Offset X", 0f, "Vector3 for the offset of the left hand left/right position");
            cfg_lHandOffsetY = Config.Bind("Offsets", "Left Hand Offset Y", 0f, "Vector3 for the offset of the left hand up/down position");
            cfg_lHandOffsetZ = Config.Bind("Offsets", "Left Hand Offset Z", -0.21f, "Vector3 for the offset of the left hand forward/back position"); // Might need to change these as this is controlling up/down visuallyin game.
            cfg_rHandOffsetX = Config.Bind("Offsets", "Right Hand Offset X", 0f, "Vector3 for the offset of the right hand left/right position");
            cfg_rHandOffsetY = Config.Bind("Offsets", "Right Hand Offset Y", 0f, "Vector3 for the offset of the right hand up/down position");
            cfg_rHandOffsetZ = Config.Bind("Offsets", "Right Hand Offset Z", -0.21f, "Vector3 for the offset of the right hand forward/back position"); // Might need to change these as this is controlling up/down visuallyin game.

            // GameObject Setup
            body = new GameObject("BodyTrail");
            TrailRenderer bodyTrail = body.AddComponent(typeof(TrailRenderer)) as TrailRenderer;
            lHand = new GameObject("LHandTrail");
            TrailRenderer lHandTrail = lHand.AddComponent(typeof(TrailRenderer)) as TrailRenderer;
            rHand = new GameObject("RHandTrail");
            TrailRenderer rHandTrail = rHand.AddComponent(typeof(TrailRenderer)) as TrailRenderer;

            bodyTrail.transform.SetParent(GM.CurrentPlayerBody.Head, false);
            lHandTrail.transform.SetParent(GM.CurrentPlayerBody.LeftHand, false);
            rHandTrail.transform.SetParent(GM.CurrentPlayerBody.RightHand, false);
            bodyOffset.x = cfg_headOffsetX.Value;
            bodyOffset.y = cfg_headOffsetY.Value;
            bodyOffset.z = cfg_headOffsetZ.Value;
            lHandOffset.x = cfg_lHandOffsetX.Value;
            lHandOffset.y = cfg_lHandOffsetY.Value;
            lHandOffset.z = cfg_lHandOffsetZ.Value;
            rHandOffset.x = cfg_rHandOffsetX.Value;
            rHandOffset.y = cfg_rHandOffsetY.Value;
            rHandOffset.z = cfg_rHandOffsetZ.Value;
            bodyTrail.transform.localPosition = bodyOffset;
            lHandTrail.transform.localPosition = lHandOffset;
            rHandTrail.transform.localPosition = rHandOffset;
        }
        
        // The line below allows access to your plugin's logger from anywhere in your code, including outside of this file.
        // Use it with 'YourPlugin.Logger.LogInfo(message)' (or any of the other Log* methods)
        internal new static ManualLogSource Logger { get; private set; }

    }
}
