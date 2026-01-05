using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq; // Package Managerから導入が必要

public class GeoJsonMapGenerator : MonoBehaviour
{
    [Header("GeoJSONファイル (.json / .txt)")]
    public TextAsset geoJsonFile;

    [Header("表示設定")]
    public float scale = 1000f; // 緯度経度は数値が小さいので拡大する
    public Vector2 centerCoordinates = new Vector2(139.7528f, 35.6852f); // 皇居付近を原点(0,0)にする

    [Header("区ごとのマテリアル")]
    public Material areaMaterial;

    void Start()
    {
        // Playモード時、まだマップが生成されていなければ自動生成する
        // (エディタですでに生成済みの場合は重複しないようにスキップ)
        if (Application.isPlaying && transform.childCount == 0 && geoJsonFile != null)
        {
            GenerateMap(geoJsonFile.text);
        }
    }

    // エディタのコンテキストメニュー（右クリック）から実行できるようにする属性
    [ContextMenu("Generate Map Now")]
    public void GenerateMapFromEditor()
    {
        if (geoJsonFile == null)
        {
            Debug.LogError("GeoJSONファイルが設定されていません。");
            return;
        }

        // 重複を防ぐため、一度クリアしてから生成
        ClearMap();
        GenerateMap(geoJsonFile.text);
        Debug.Log("マップ生成完了");
    }

    [ContextMenu("Clear Map")]
    public void ClearMap()
    {
        // 子オブジェクトをすべて削除する
        // エディタモード中はDestroyではなくDestroyImmediateを使う必要がある
        var children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        foreach (var child in children)
        {
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child); // エディタ実行時はこちらが呼ばれる
        }
    }

    void GenerateMap(string jsonText)
    {
        try
        {
            // JSONを解析
            JObject root = JObject.Parse(jsonText);
            JArray features = root["features"] as JArray;

            if (features == null)
            {
                Debug.LogError("GeoJSONエラー: 'features' 配列が見つかりません。");
                return;
            }

            // 1. エリア名ごとにパス（頂点リスト）をまとめる辞書を作成
            // key: エリア名, value: 飛び地を含むパスのリスト
            Dictionary<string, List<Vector2[]>> areaDataMap = new Dictionary<string, List<Vector2[]>>();

            foreach (JToken feature in features)
            {
                // 区の名前を取得
                string areaName = feature["properties"]["N03_004"]?.ToString() ?? "Unknown Area";

                // 区の名前が含まれていない場合はスキップ
                if(!areaName.Contains("区")) continue;
                
                // 形状データの取得
                JObject geometry = feature["geometry"] as JObject;

                // geometryがnull（形状データなし）の場合はスキップ
                if (geometry == null) continue;

                string type = geometry["type"]?.ToString();

                if (type == "Polygon")
                {
                    JArray coordinates = (JArray)geometry["coordinates"];
                    // Polygonの0番目が外周
                    Vector2[] path = ConvertToVector2Array(coordinates[0]);
                    
                    if (!areaDataMap.ContainsKey(areaName))
                    {
                        areaDataMap[areaName] = new List<Vector2[]>();
                    }
                    areaDataMap[areaName].Add(path);
                }
                else if (type == "MultiPolygon")
                {
                    JArray polygons = (JArray)geometry["coordinates"];
                    foreach (JToken poly in polygons)
                    {
                        Vector2[] path = ConvertToVector2Array(poly[0]);

                        if (!areaDataMap.ContainsKey(areaName))
                        {
                            areaDataMap[areaName] = new List<Vector2[]>();
                        }
                        areaDataMap[areaName].Add(path);
                    }
                }
            }

            // 2. まとまったデータからオブジェクトを生成
            foreach (var kvp in areaDataMap)
            {
                CreateUnifiedAreaObject(kvp.Key, kvp.Value);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("マップ生成中にエラーが発生しました: " + e.Message);
        }
    }

    // 座標配列への変換ヘルパーメソッド
    Vector2[] ConvertToVector2Array(JToken coordinateArray)
    {
        List<Vector2> points = new List<Vector2>();
        foreach (JToken coord in coordinateArray)
        {
            float lon = (float)coord[0];
            float lat = (float)coord[1];

            float x = (lon - centerCoordinates.x) * scale;
            float y = (lat - centerCoordinates.y) * scale;

            points.Add(new Vector2(x, y));
        }
        return points.ToArray();
    }

    // 統合されたオブジェクト生成メソッド
    void CreateUnifiedAreaObject(string name, List<Vector2[]> paths)
    {
        GameObject areaObj = new GameObject(name);
        areaObj.transform.SetParent(this.transform);

        PolygonCollider2D col = areaObj.AddComponent<PolygonCollider2D>();
        MeshFilter mf = areaObj.AddComponent<MeshFilter>();
        MeshRenderer mr = areaObj.AddComponent<MeshRenderer>();

        mr.material = areaMaterial != null ? areaMaterial : new Material(Shader.Find("Sprites/Default"));
        mr.material.color = new Color(Random.value, Random.value, Random.value);

        // PolygonCollider2Dに複数のパスを設定（これが飛び地対応のキモです）
        col.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            col.SetPath(i, paths[i]);
        }

        // まとめてメッシュ生成
        mf.mesh = col.CreateMesh(false, false);
    }
}