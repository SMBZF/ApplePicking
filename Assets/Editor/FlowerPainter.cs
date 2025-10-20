using UnityEngine;
using UnityEditor;

public class FlowerPainter : EditorWindow
{
    public GameObject flowerPrefab;
    public float brushRadius = 2f;
    public int density = 10;
    public Vector2 randomScale = new Vector2(0.8f, 1.2f);
    public bool randomRotation = true;

    [MenuItem("Tools/Flower Painter")]
    public static void ShowWindow()
    {
        GetWindow<FlowerPainter>("Flower Painter");
    }

    private void OnGUI()
    {
        GUILayout.Label("?? Flower Brush Settings", EditorStyles.boldLabel);
        flowerPrefab = (GameObject)EditorGUILayout.ObjectField("Flower Prefab", flowerPrefab, typeof(GameObject), false);
        brushRadius = EditorGUILayout.FloatField("Brush Radius", brushRadius);
        density = EditorGUILayout.IntSlider("Density", density, 1, 100);
        randomScale = EditorGUILayout.Vector2Field("Random Scale (Min, Max)", randomScale);
        randomRotation = EditorGUILayout.Toggle("Random Y Rotation", randomRotation);
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (flowerPrefab == null) return;

        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 创建/查找父物体
                GameObject parent = GameObject.Find("FlowerContainer");
                if (parent == null)
                {
                    parent = new GameObject("FlowerContainer");
                    Undo.RegisterCreatedObjectUndo(parent, "Create Flower Container");
                }

                for (int i = 0; i < density; i++)
                {
                    Vector2 randCircle = Random.insideUnitCircle * brushRadius;
                    Vector3 spawnPos = hit.point + new Vector3(randCircle.x, 0, randCircle.y);

                    if (Physics.Raycast(spawnPos + Vector3.up * 5, Vector3.down, out RaycastHit groundHit, 10f))
                    {
                        GameObject flower = (GameObject)PrefabUtility.InstantiatePrefab(flowerPrefab);
                        flower.transform.position = groundHit.point;
                        flower.transform.SetParent(parent.transform);

                        float scale = Random.Range(randomScale.x, randomScale.y);
                        flower.transform.localScale = Vector3.one * scale;

                        if (randomRotation)
                        {
                            flower.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                        }

                        Undo.RegisterCreatedObjectUndo(flower, "Paint Flower");
                    }
                }

                e.Use(); // 阻止事件穿透
            }
        }
    }
}
