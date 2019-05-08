using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New MineTile", menuName = "Tiles/MineTile")]
    public class MineTile : TileBase
    {  
        public Sprite sprite;       
        public Tile.ColliderType colliderType = Tile.ColliderType.Sprite;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = sprite;
            tileData.colliderType = colliderType;
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
        }
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(MineTile))]
public class MineFieldEditor : Editor
{
    private MineTile tile { get { return (target as MineTile); } }

    SerializedProperty sprite;   
    SerializedProperty colliderType;

    private void OnEnable()
    { 
        sprite = serializedObject.FindProperty("sprite");
        colliderType = serializedObject.FindProperty("colliderType");
    }

    public override void OnInspectorGUI()
    {
        MineTile e = (MineTile)target;
        serializedObject.Update();

        DrawSpritePreview(sprite, tile.sprite);
        EditorGUILayout.PropertyField(colliderType);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSpritePreview(SerializedProperty propertyOfSpriteType, Sprite sprite)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(propertyOfSpriteType);
        Texture2D normalTexture = sprite ? AssetPreview.GetAssetPreview(sprite) : AssetPreview.GetMiniTypeThumbnail(typeof(Sprite));
        GUILayout.Label(normalTexture, GUILayout.Width(32), GUILayout.Height(32));
        EditorGUILayout.EndHorizontal();
    }

    public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
    {
        Texture2D sourceTex = AssetPreview.GetAssetPreview(tile.sprite);
        if (sourceTex)
        {
            var pix = sourceTex.GetPixels32();
            var destTex = new Texture2D(sourceTex.width, sourceTex.height);
            destTex.SetPixels32(pix);
            destTex.Apply();
            return destTex;
        }
        else
        {
            return null;
        }
    }



    #region Change icon in Project window

    [InitializeOnLoadMethod]
    static void  CustomMatch3TileIcon()
    {
        EditorApplication.projectWindowItemOnGUI += OnChangeMatch3TileIcon;
    }

    static void OnChangeMatch3TileIcon(string GUID, Rect iconRect)
    {
        string assetpath = AssetDatabase.GUIDToAssetPath(GUID);
        string extension = System.IO.Path.GetExtension(assetpath);

        if (extension == ".asset")
        {
            Sprite sprite = null;
            MineTile tile = AssetDatabase.LoadAssetAtPath<MineTile>(assetpath);
            if (!tile)
                return;

            sprite = tile.sprite;

            if (!sprite)
                return;

            iconRect = CalculateOffsetOfIcon(iconRect);

            Texture2D icon = sprite.texture;
            iconRect.width = iconRect.height;
            iconRect.height = iconRect.height;
            if (icon)
            {
                GUI.DrawTexture(iconRect, icon);
            }
        }
    }

    private static Rect CalculateOffsetOfIcon(Rect rect)
    {
        float width = rect.width;
        float height = rect.height;
        rect.width = rect.height;

        if (rect.width > 64)
        {
            rect.height = width;
            rect.width = width;
            AddOffsetToIconPosition(ref rect, new Rect(1, 1, 0, 0));
        }
        else if (rect.width > 37)
        {
            rect.height = width;
            rect.width = width;
            AddOffsetToIconPosition(ref rect, new Rect(1, 2, 0, 0));
        }
        else if (rect.width >= 32)
        {
            rect.height = width;
            rect.width = width;
            AddOffsetToIconPosition(ref rect, new Rect(2, 1f, 0, 0));
        }
        else
        if (rect.width == 16)
        {
            rect.height = height;
            rect.width = height;

            AddOffsetToIconPosition(ref rect, new Rect(4, 1, 0, 0));
        }
        return rect;
    }

    private static void AddOffsetToIconPosition(ref Rect rect, Rect offset)
    {
        rect = new Rect(rect.x + offset.x, rect.y + offset.y, rect.height + offset.width, rect.height + offset.height);
    }
    #endregion

}
#endif








