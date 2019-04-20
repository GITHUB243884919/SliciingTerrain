using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class TerrainLoad
{
    public static string TERRAIN_PATH = "TerrainSlicing/";
    public static string TERRAIN_NAME = "Slicing";

}
public class TerrainSlicing : Editor
{
    public static string TerrainSavePath = "Assets/Resources/" + TerrainLoad.TERRAIN_PATH;
    //分割大小
    public static int SLICING_SIZE = 4;

    //开始分割地形
    [MenuItem("Terrain/Slicing")]
    private static void Slicing()
    {
        Terrain terrain = GameObject.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("找不到地形!");
            return;
        }

        if (Directory.Exists(TerrainSavePath)) Directory.Delete(TerrainSavePath, true);
        Directory.CreateDirectory(TerrainSavePath);

        TerrainData terrainData = terrain.terrainData;

        //这里我分割的宽和长度是一样的.这里求出循环次数,TerrainLoad.SIZE要生成的地形宽度,长度相同
        //高度地图的分辨率只能是2的N次幂加1,所以SLICING_SIZE必须为2的N次幂
        //SLICING_SIZE = (int)terrainData.size.x / TerrainLoad.SIZE;
        SLICING_SIZE = 4;
        Vector3 oldSize = terrainData.size;
        Debug.LogError("terrainData.size " + terrainData.size + " " +
            terrainData.heightmapWidth + " " + terrainData.heightmapHeight);

        //得到新地图分辨率
        int newHeightmapResolution = (terrainData.heightmapResolution - 1) / SLICING_SIZE;
        int newAlphamapResolution = terrainData.alphamapResolution / SLICING_SIZE;
        int newbaseMapResolution = terrainData.baseMapResolution / SLICING_SIZE;
        SplatPrototype[] splatProtos = terrainData.splatPrototypes;

        //循环宽和长,生成小块地形
        for (int x = 0; x < SLICING_SIZE; ++x)
        {
            for (int y = 0; y < SLICING_SIZE; ++y)
            {
                //创建资源
                TerrainData newData = new TerrainData();
                string terrainName = TerrainSavePath + TerrainLoad.TERRAIN_NAME + y + "_" + x + ".asset";
                AssetDatabase.CreateAsset(newData, TerrainSavePath + TerrainLoad.TERRAIN_NAME + y + "_" + x + ".asset");
                EditorUtility.DisplayProgressBar("正在分割地形", terrainName, (float)(x * SLICING_SIZE + y) / (float)(SLICING_SIZE * SLICING_SIZE));

                //设置分辨率参数
                newData.heightmapResolution = (terrainData.heightmapResolution - 1) / SLICING_SIZE;
                newData.alphamapResolution = terrainData.alphamapResolution / SLICING_SIZE;
                newData.baseMapResolution = terrainData.baseMapResolution / SLICING_SIZE;

                //设置大小
                newData.size = new Vector3(oldSize.x / SLICING_SIZE, oldSize.y, oldSize.z / SLICING_SIZE);

                //设置地形原型
                SplatPrototype[] newSplats = new SplatPrototype[splatProtos.Length];
                for (int i = 0; i < splatProtos.Length; ++i)
                {
                    newSplats[i] = new SplatPrototype();
                    newSplats[i].texture = splatProtos[i].texture;
                    newSplats[i].tileSize = splatProtos[i].tileSize;

                    float offsetX = (newData.size.x * x) % splatProtos[i].tileSize.x + splatProtos[i].tileOffset.x;
                    float offsetY = (newData.size.z * y) % splatProtos[i].tileSize.y + splatProtos[i].tileOffset.y;
                    newSplats[i].tileOffset = new Vector2(offsetX, offsetY);
                }
                newData.splatPrototypes = newSplats;


                //设置混合贴图
                float[,,] alphamap = new float[newAlphamapResolution, newAlphamapResolution, splatProtos.Length];
                alphamap = terrainData.GetAlphamaps(x * newData.alphamapWidth, y * newData.alphamapHeight, newData.alphamapWidth, newData.alphamapHeight);
                newData.SetAlphamaps(0, 0, alphamap);

                //设置高度
                int xBase = terrainData.heightmapWidth / SLICING_SIZE;
                int yBase = terrainData.heightmapHeight / SLICING_SIZE;
                float[,] height = terrainData.GetHeights(xBase * x, yBase * y, xBase + 1, yBase + 1);
                newData.SetHeights(0, 0, height);
            }
        }

        EditorUtility.ClearProgressBar();
    }
}