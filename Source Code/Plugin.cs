using Utilla;
using System;
using BepInEx;
using System.IO;
using Bepinject;
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using UnityEngine.Networking;
using System.Collections.Generic;
using GorillaSigns.ComputerInterface;

namespace GorillaSigns.Main
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [Description("HauntedModMenu")]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static int res;
        public static int current;
        public static int showSign;
        public static int imageMode;
        public static string imagePath;
        public static GameObject signObject;
        public static readonly List<string> pngImagesPublic = new List<string>();
        public static readonly List<string> pngImageNames = new List<string>();

        public void Awake()
        {
            Zenjector.Install<MainInstaller>().OnProject();
            Events.GameInitialized += OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            GetFolder();
            CreateObject();
            LoadImage();
            GetData();
        }

        void GetData()
        {
            showSign = PlayerPrefs.GetInt("GorillaSignsEnabled", 1);
            res = PlayerPrefs.GetInt("GorillaSignsImageResu", 1);
            imageMode = PlayerPrefs.GetInt("GorillaSignsImageMode", 1);

            if (showSign == 1)
                signObject.SetActive(true);
            else
                signObject.SetActive(false);
        }

        void GetFolder()
        {
            imagePath = Path.Combine(Directory.GetCurrentDirectory(), "BepInEx", "Plugins", PluginInfo.Name.ToString(), "Images");
            if (!Directory.Exists(imagePath))
            {
                Debug.LogError("Failed to find the Image folder, the mod will create the directory.");
                Directory.CreateDirectory(imagePath);
            }

            string[] files = Directory.GetFiles(imagePath);
            string[] fileName = new string[files.Length];
            for (int i = 0; i < fileName.Length; i++)
            {
                fileName[i] = Path.GetFileName(files[i]);
                pngImageNames.Add(fileName[i]);
                pngImagesPublic.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Images\\" + fileName[i]);
            }
            pngImageNames.Add("Mirror");
            pngImagesPublic.Add("Mirror");
        }

        void CreateObject()
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GorillaSigns.Resources.imagesign");
            AssetBundle assetBundle = AssetBundle.LoadFromStream(manifestResourceStream);
            GameObject signTemporaryObject = assetBundle.LoadAsset<GameObject>("mirrorsign"); // im so dumb
            signObject = Instantiate(signTemporaryObject); // keep your eyes peeled nachoengine

            signObject.transform.position = new Vector3(0.001f, 0.01f, 0.003f);
            signObject.transform.rotation = Quaternion.identity; // fuck quaternions - if you used Euler it would be easier, but yeah i agree xD
            signObject.transform.localScale = new Vector3(1.22f, 1.22f, 1.22f);
            signObject.transform.SetParent(GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/").transform, false);
        }

        // Shortened down the code a little, thought it would help ^v^
        public static void LoadImage()
        {
            Texture2D tex = new Texture2D(1024 * res, 1024 * res, TextureFormat.RGB24, false);
            tex.filterMode = (FilterMode)imageMode;

            byte[] bytes = File.ReadAllBytes(pngImagesPublic[current]);
            tex.LoadImage(bytes);
            tex.Apply();

            signObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().materials[1].mainTexture = tex;
            Console.WriteLine("Sucessfully applied texture to the very cool sign"); // keeping this in because very cool sign 😎
        }
    }
}
