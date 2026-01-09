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

    // ■■■ 設定エリア ■■■
    // ここに統合したい区の組み合わせを定義します。
    // このリストに含まれていない区は生成されません。
    private List<List<string>> targetAreaGroups = new List<List<string>>()
        {
            // 1. 都心部（政治・経済・商業の中心）
            new List<string>() { "中央区", "港区","江東区" },

            // 2. 副都心・山の手（巨大ターミナルと商業エリア）
            new List<string>() { "新宿区", "渋谷区", "中野区" },

            // 3. 城南エリア（品川ナンバーエリア・南部・羽田方面）
            new List<string>() { "品川区", "目黒区", "大田区" },

            // 4. 城西エリア（中央線・井の頭線・小田急線沿線の住宅街）
            new List<string>() { "世田谷区", "杉並区" },

            // 5. 城北西エリア（池袋文化圏・東武/西武線沿線）
            new List<string>() { "豊島区", "練馬区", "板橋区" },

            // 6. 城北・文教エリア（アカデミック・歴史的エリア・北部）
            new List<string>() { "足立区", "北区", "荒川区" },

            // 7. 下町・湾岸エリア（伝統的な下町と新しい湾岸エリア）
            new List<string>() { "台東区", "千代田区", "文京区" },

            // 8. 城東・北東エリア（千葉寄りの住宅エリア）
            new List<string>() { "墨田区", "葛飾区", "江戸川区" }
        };
    // ■■■■■■■■■■■■

    void Start()
    {
        // Playモード時、まだマップが生成されていなければ自動生成する
        if (Application.isPlaying && transform.childCount == 0 && geoJsonFile != null)
        {
            GenerateMap(geoJsonFile.text);
        }
    }

    [ContextMenu("Generate Map Now")]
    public void GenerateMapFromEditor()
    {
        if (geoJsonFile == null)
        {
            Debug.LogError("GeoJSONファイルが設定されていません。");
            return;
        }
        ClearMap();
        GenerateMap(geoJsonFile.text);
        Debug.Log("マップ生成完了");
    }

    [ContextMenu("Clear Map")]
    public void ClearMap()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        foreach (var child in children)
        {
            if (Application.isPlaying) Destroy(child);
            else DestroyImmediate(child);
        }
    }

    void GenerateMap(string jsonText)
    {
        try
        {
            // 1. まず全てのデータを辞書に読み込む (key: 区名, value: パスリスト)
            // これにより、GeoJSONの並び順に依存せず、好きな組み合わせで取り出せるようになります
            Dictionary<string, List<Vector2[]>> rawDataMap = LoadGeoJsonToDictionary(jsonText);
            
            if (rawDataMap == null) return;

            // 2. 指定されたグループリストに基づいて統合・生成
            int groupIndex = 1;
            foreach (var groupList in targetAreaGroups)
            {
                List<Vector2[]> combinedPaths = new List<Vector2[]>();
                string groupDisplayName = "";

                foreach (var areaName in groupList)
                {
                    if (rawDataMap.ContainsKey(areaName))
                    {
                        // その区のすべてのパス（飛び地含む）を統合リストに追加
                        combinedPaths.AddRange(rawDataMap[areaName]);

                        // デバッグ用の名前作成
                        if (groupDisplayName.Length > 0) groupDisplayName += "_";
                        groupDisplayName += areaName;
                    }
                    else
                    {
                        Debug.LogWarning($"警告: 設定リストにある '{areaName}' はGeoJSON内に見つかりませんでした。");
                    }
                }

                // パスが1つでもあれば生成
                if (combinedPaths.Count > 0)
                {
                    // 名前が長くなりすぎる場合の対策
                    string objectName = $"Area{groupIndex}";
                    if (groupDisplayName.Length < 30) objectName += $"_{groupDisplayName}"; // 短ければ区名をつける

                    CreateUnifiedAreaObject(objectName, combinedPaths);
                    groupIndex++;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("マップ生成中にエラーが発生しました: " + e.Message);
        }
    }

    // GeoJSONを解析して辞書化する処理
    Dictionary<string, List<Vector2[]>> LoadGeoJsonToDictionary(string jsonText)
    {
        var result = new Dictionary<string, List<Vector2[]>>();
        JObject root = JObject.Parse(jsonText);
        JArray features = root["features"] as JArray;

        if (features == null)
        {
            Debug.LogError("GeoJSONエラー: 'features' 配列が見つかりません。");
            return null;
        }

        foreach (JToken feature in features)
        {
            string areaName = feature["properties"]["N03_004"]?.ToString() ?? "Unknown Area";
            JObject geometry = feature["geometry"] as JObject;
            if (geometry == null) continue;

            string type = geometry["type"]?.ToString();
            List<Vector2[]> paths = new List<Vector2[]>();

            if (type == "Polygon")
            {
                JArray coordinates = (JArray)geometry["coordinates"];
                paths.Add(ConvertToVector2Array(coordinates[0]));
            }
            else if (type == "MultiPolygon")
            {
                JArray polygons = (JArray)geometry["coordinates"];
                foreach (JToken poly in polygons)
                {
                    paths.Add(ConvertToVector2Array(poly[0]));
                }
            }

            // 辞書に追加（同名の区データが複数行に分かれている場合も考慮して追加）
            if (!result.ContainsKey(areaName))
            {
                result[areaName] = new List<Vector2[]>();
            }
            result[areaName].AddRange(paths);
        }

        return result;
    }

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

    void CreateUnifiedAreaObject(string name, List<Vector2[]> paths)
    {
        GameObject areaObj = new GameObject(name);
        areaObj.transform.SetParent(this.transform);

        PolygonCollider2D col = areaObj.AddComponent<PolygonCollider2D>();
        MeshFilter mf = areaObj.AddComponent<MeshFilter>();
        MeshRenderer mr = areaObj.AddComponent<MeshRenderer>();

        mr.material = areaMaterial != null ? areaMaterial : new Material(Shader.Find("Sprites/Default"));
        mr.material.color = new Color(Random.value, Random.value, Random.value);

        // 複数の区のパスを全てこの1つのコライダに設定
        col.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
        {
            col.SetPath(i, paths[i]);
        }

        mf.mesh = col.CreateMesh(false, false);
    }
}