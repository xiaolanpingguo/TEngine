#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public abstract class CustomObjectSelectionWindow<TWindow, TObject> : EditorWindow
    where TWindow : CustomObjectSelectionWindow<TWindow, TObject>
    where TObject : Object
{
    private const string k_itemsScaleSaveKey = "CustomObjectSelectionWindow.ItemsScale";
    private const string k_searchTextSaveKey = "CustomObjectSelectionWindow.SearchText";

    private const string k_clearSearchTextButtonName = "ClearSearchButton";
    private const float k_objectItemWidth = 100;
    private const int k_buttonsSpacing = 5;
    private const float k_itemsDrawAsListThreshold = 0.3f;

    private List<TObject> m_entries = null;
    private List<int> m_filteredEntriesIndices = null;
    private string m_searchText = "";
    private Vector2 m_objectScrollViewPos = Vector2.zero;
    private double m_pressTime = 0;
    private int m_pressItemIndex = -1;
    private float m_itemsScale = 1f;

    protected GUIStyle m_itemButtonDefaultStyle = null;
    protected GUIStyle m_itemButtonSelectedStyle = null;
    protected GUIStyle m_itemLabelDefaultStyle = null;
    protected GUIStyle m_itemLabelSelectedStyle = null;
    protected GUIStyle m_listDefaultItemStyle = null;
    protected GUIStyle m_listSelectedItemStyle = null;

    protected Dictionary<int, Texture> m_objectsIcons = null;
    protected bool m_SelectionIsValid = false;

    public TObject SelectedObject { get; private set; } = null;
    public int FieldId { get; private set; } = -1;

    protected abstract bool DelayClosingCall { get; }
    protected abstract string SelectButtonText { get; }
    protected abstract string ObjectSearchFilterString { get; }

    protected abstract string[] ObjectSearchFolders { get; }

    protected abstract bool IsValidObject(TObject objet);

    protected abstract void InitializeWindow(params object[] extraParams);

    public static TWindow Get(int fieldId, string title, TObject selectedObj, params object[] extraParams)
    {
        TWindow window = CreateInstance<TWindow>();
        window.FieldId = fieldId;
        window.titleContent = new GUIContent(title);
        window.SelectedObject = selectedObj;
        window.TryInitialize(extraParams);
        return window;
    }

    void OnEnable()
    {
        m_itemsScale = EditorPrefs.GetFloat(k_itemsScaleSaveKey, 1f);
        m_searchText = EditorPrefs.GetString(k_searchTextSaveKey);
    }

    void OnDestroy()
    {
        if (m_SelectionIsValid == false)
        {
            //user closed with 'X', act as if it is cancelled.
            SelectedObject = null;
        }
        EditorPrefs.SetFloat(k_itemsScaleSaveKey, m_itemsScale);
        EditorPrefs.SetString(k_searchTextSaveKey, m_searchText);
    }

    void OnGUI()
    {
        TryInitializeStyles();
        EditorGUILayout.BeginVertical();
        {
            DrawSearchBar();
            DrawInfoBarAndScaleSlider();
            DrawObjectsPanel();
            DrawSelectedObjectPanel();
            DrawSelectCancelButton();
        }
        EditorGUILayout.EndVertical();
    }

    protected virtual void TryInitializeStyles()
    {
        if (m_itemButtonDefaultStyle == null)
        {
            m_itemButtonDefaultStyle = new GUIStyle("button");
            m_itemButtonDefaultStyle.normal.background = MakeColorTexture(Color.clear);
            m_itemButtonDefaultStyle.hover.background = m_itemButtonDefaultStyle.normal.background;
            m_itemButtonDefaultStyle.focused.background = m_itemButtonDefaultStyle.normal.background;
            m_itemButtonDefaultStyle.active.background = m_itemButtonDefaultStyle.normal.background;
        }

        if (m_itemButtonSelectedStyle == null)
        {
            m_itemButtonSelectedStyle = new GUIStyle(m_itemButtonDefaultStyle);
            m_itemButtonSelectedStyle.normal.background = MakeColorTexture(GUI.skin.settings.selectionColor);
        }

        if (m_itemLabelDefaultStyle == null)
        {
            m_itemLabelDefaultStyle = new GUIStyle("label");
            m_itemLabelDefaultStyle.wordWrap = true;
            m_itemLabelDefaultStyle.alignment = TextAnchor.MiddleCenter;
            m_itemLabelDefaultStyle.normal.background = m_itemButtonDefaultStyle.normal.background;
            m_itemLabelDefaultStyle.fontSize = 14;
        }

        if (m_itemLabelSelectedStyle == null)
        {
            m_itemLabelSelectedStyle = new GUIStyle(m_itemLabelDefaultStyle);
            m_itemLabelSelectedStyle.normal.background = m_itemButtonSelectedStyle.normal.background;
        }

        if (m_listDefaultItemStyle == null)
        {
            m_listDefaultItemStyle = new GUIStyle("label");
            m_listDefaultItemStyle.margin = new RectOffset(0, 0, 0, 0);
            m_listDefaultItemStyle.padding = new RectOffset(15, 0, 0, 0);
        }

        if (m_listSelectedItemStyle == null)
        {
            m_listSelectedItemStyle = new GUIStyle(m_listDefaultItemStyle);
            m_listSelectedItemStyle.normal.background = m_itemButtonSelectedStyle.normal.background;
        }
    }

    private void TryInitialize(params object[] extraParams)
    {
        m_entries = new List<TObject>();
        m_objectsIcons = new Dictionary<int, Texture>();
        m_filteredEntriesIndices = new List<int>();

        InitializeWindow(extraParams);

        string[] allAssetsGuids = AssetDatabase.FindAssets(ObjectSearchFilterString, ObjectSearchFolders);

        foreach (var guid in allAssetsGuids)
        {
            TObject obj = AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(guid));

            if (IsValidObject(obj))
            {
                m_entries.Add(obj);
            }
        }

        UpdateFilteredObjects();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.BeginHorizontal("box");
        {
            GUILayout.Label("Search:", GUILayout.Width(60));

            EditorGUI.BeginChangeCheck();
            m_searchText = EditorGUILayout.TextField(m_searchText);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateFilteredObjects();
            }

            GUIStyle clearButtonStyle = new GUIStyle("button") {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            var margin = clearButtonStyle.margin;
            margin.bottom += 1;
            clearButtonStyle.margin = margin;

            GUI.SetNextControlName(k_clearSearchTextButtonName);
            if (GUILayout.Button("X", clearButtonStyle, GUILayout.Width(30)))
            {
                m_searchText = "";
                GUI.FocusControl(k_clearSearchTextButtonName);
                UpdateFilteredObjects();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawInfoBarAndScaleSlider()
    {
        EditorGUILayout.BeginHorizontal("box");
        {
            var width = GUILayout.Width(160);
            GUILayout.Label($"Total Assets Found: {m_entries.Count}", width);
            GUILayout.Label($"Assets Displayed:   {m_filteredEntriesIndices.Count}", width);
            GUILayout.FlexibleSpace();
            m_itemsScale = GUILayout.HorizontalSlider(m_itemsScale, 0f, 1f, GUILayout.Width(50));
            GUILayout.Space(5);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void UpdateFilteredObjects()
    {
        m_filteredEntriesIndices = new List<int>();

        if (string.IsNullOrEmpty(m_searchText) || string.IsNullOrWhiteSpace(m_searchText))
        {
            for (int i = 0; i < m_entries.Count; ++i)
            {
                m_filteredEntriesIndices.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < m_entries.Count; ++i)
            {
                string entryName = m_entries[i].name.ToLower();
                string filter = m_searchText.ToLower();

                if (entryName.Contains(filter) || filter.Contains(entryName))
                {
                    m_filteredEntriesIndices.Add(i);
                }
            }
        }
    }

    private void DrawObjectsPanel()
    {
        if (m_itemsScale > k_itemsDrawAsListThreshold)
        {
            DrawItemsAsGrid();
        }
        else
        {
            DrawItemsAsList();
        }
    }

    private void DrawItemsAsGrid()
    {
        int numOfColumns = Mathf.Max((int)(position.width / ((k_objectItemWidth * m_itemsScale) + k_buttonsSpacing)), 1);
        int startIndex = 0;
        int endIndex = numOfColumns;

        m_objectScrollViewPos = GUILayout.BeginScrollView(m_objectScrollViewPos);
        {
            bool keepDrawing = true;

            while (keepDrawing)
            {
                EditorGUILayout.BeginVertical();
                {
                    keepDrawing = DrawRowOfObjects(startIndex, endIndex - 1);
                    startIndex = endIndex;
                    endIndex += numOfColumns;
                }
                EditorGUILayout.EndVertical();
            }
        }
        GUILayout.EndScrollView();
    }

    private void DrawItemsAsList()
    {
        EditorGUILayout.BeginVertical("box");
        {
            m_objectScrollViewPos = GUILayout.BeginScrollView(m_objectScrollViewPos);
            {
                for (int i = 0; i < m_filteredEntriesIndices.Count; ++i)
                {
                    DrawObjectAsListItem(m_entries[m_filteredEntriesIndices[i]], i);
                }
            }
            GUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }

    private bool DrawRowOfObjects(int startIndex, int endIndex)
    {
        bool keepDrawing = true;

        EditorGUILayout.BeginHorizontal();
        {
            for (int i = startIndex; i <= endIndex; ++i)
            {
                if (i >= m_filteredEntriesIndices.Count)
                {
                    keepDrawing = false;
                    break;
                }

                DrawObjectAsButton(m_entries[m_filteredEntriesIndices[i]], i);
            }
        }
        EditorGUILayout.EndHorizontal();

        return keepDrawing;
    }

    private void DrawObjectAsButton(TObject obj, int itemIndex)
    {
        bool isSelected = SelectedObject == obj;
        var widthOpt = GUILayout.Width(k_objectItemWidth * m_itemsScale);

        GUIStyle backgroundStyle = new GUIStyle("box");
        backgroundStyle.margin = new RectOffset(k_buttonsSpacing, 0, k_buttonsSpacing, 0);
        backgroundStyle.normal.background = GUI.skin.button.normal.background;

        EditorGUILayout.BeginVertical(backgroundStyle);
        {
            var buttonStyle = isSelected ? m_itemButtonSelectedStyle : m_itemButtonDefaultStyle;
            if (GUILayout.Button(GetIcon(obj), buttonStyle, widthOpt, GUILayout.Height(k_objectItemWidth * m_itemsScale)))
            {
                HandleDoublePressed(itemIndex);
                SelectedObject = obj;
            }

            var labelStyle = isSelected ? m_itemLabelSelectedStyle : m_itemLabelDefaultStyle;
            GUILayout.Label(obj.name, labelStyle, widthOpt);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawObjectAsListItem(TObject obj, int itemIndex)
    {
        GUIContent content = new GUIContent(obj.name, GetIcon(obj));
        var style = SelectedObject == obj ? m_listSelectedItemStyle : m_listDefaultItemStyle;

        if (GUILayout.Button(content, style, GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            HandleDoublePressed(itemIndex);
            SelectedObject = obj;
        }
    }

    private void DrawSelectedObjectPanel()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(65));
        {
            if (SelectedObject != null)
            {
                DrawSelectedObjectDetails();
            }
            else
            {
                GUILayout.Label("Make a selection above for details to show...");
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawSelectCancelButton()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(SelectedObject == null);
        if (GUILayout.Button(SelectButtonText, GUILayout.Height(35)))
        {
            m_SelectionIsValid = true;
            CloseNowOrDelay();
        }
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("Cancel", GUILayout.Height(35)))
        {
            m_SelectionIsValid = false;
            SelectedObject = null;
            CloseNowOrDelay();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void CloseNowOrDelay()
    {
        if (DelayClosingCall)
        {
            EditorApplication.delayCall += Close;
        }
        else
        {
            Close();
        }
    }

    private void HandleDoublePressed(int itemIndex)
    {
        if (EditorApplication.timeSinceStartup - m_pressTime <= 0.5 && m_pressItemIndex == itemIndex)
        {
            m_SelectionIsValid = true;
            CloseNowOrDelay();
        }

        m_pressTime = EditorApplication.timeSinceStartup;
        m_pressItemIndex = itemIndex;
    }

    protected virtual Texture GetIcon(TObject obj)
    {
        int id = obj.GetInstanceID();

        if (!m_objectsIcons.ContainsKey(id))
        {
            m_objectsIcons.Add(id, AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(obj)));
        }

        return m_objectsIcons[id];
    }

    protected virtual void DrawSelectedObjectDetails()
    {
        GUILayout.Label(SelectedObject.name);
        GUILayout.Label(SelectedObject.GetType().Name);
        GUILayout.Label(AssetDatabase.GetAssetPath(SelectedObject));
    }

    private Texture2D MakeColorTexture(Color col)
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.SetPixel(0, 0, col);
        tex.Apply();
        return tex;
    }
}

#endif