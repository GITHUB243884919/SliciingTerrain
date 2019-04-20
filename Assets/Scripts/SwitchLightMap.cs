using UnityEngine;
using System.Collections;

public class SwitchLightMap : MonoBehaviour
{
    //烘培烘培贴图1
    public Texture2D greenLightMap;
    public Texture2D greenShadowMask;
    public Texture2D greenDir;
    //烘培贴图2
    public Texture2D redLightMap;
    public Texture2D redShadowMask;
    public Texture2D redDir;
    void OnGUI()
    {
        if (GUILayout.Button("green"))
        {
            LightmapData data = new LightmapData();
            data.lightmapColor = greenLightMap;
            data.shadowMask = greenShadowMask;
            data.lightmapDir = greenDir;
            LightmapSettings.lightmaps = new LightmapData[1] { data };
        }

        if (GUILayout.Button("red"))
        {
            LightmapData data = new LightmapData();
            data.lightmapColor = redLightMap;
            data.shadowMask = redShadowMask;
            data.lightmapDir = redDir;
            LightmapSettings.lightmaps = new LightmapData[1] { data };
        }
    }
}
