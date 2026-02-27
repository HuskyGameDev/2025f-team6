using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackgroundAuthoringWindow : EditorWindow
{
    private enum Tab
    {
        Library,
        Stages,
        Types,
        Transitions,
        Validate,
    }

    private BackgroundLibrary library;
    private Tab currentTab;

    private Vector2 leftScroll;
    private Vector2 rightScroll;

    private BackgroundStageDefinition selectedStage;
    private BackgroundTypeDefinition selectedType;
    private BackgroundTransitionDefinition selectedTransition;

    //this comment makes the script 667 lines long
    [MenuItem("Tools/Backgrounds/Background Authoring Window")]
    public static void Open()
    {
        GetWindow<BackgroundAuthoringWindow>("Background Authoring");
    }

    private void OnGUI()
    {
        DrawToolbar();
        EditorGUILayout.Space(6f);

        DrawLibraryHeader();

        if (library == null)
        {
            EditorGUILayout.HelpBox(
                "Assign or create a BackgroundLibrary to begin authoring.",
                MessageType.Info
            );
            return;
        }

        EditorGUILayout.Space(6f);

        switch (currentTab)
        {
            case Tab.Library:
                DrawLibraryTab();
                break;
            case Tab.Stages:
                DrawStagesTab();
                break;
            case Tab.Types:
                DrawTypesTab();
                break;
            case Tab.Transitions:
                DrawTransitionsTab();
                break;
            case Tab.Validate:
                DrawValidateTab();
                break;
        }
    }

    private void DrawToolbar()
    {
        currentTab = (Tab)
            GUILayout.Toolbar(
                (int)currentTab,
                new[] { "Library", "Stages", "Types", "Transitions", "Validate" }
            );
    }

    private void DrawLibraryHeader()
    {
        EditorGUILayout.BeginVertical("box");

        BackgroundLibrary newLibrary = (BackgroundLibrary)
            EditorGUILayout.ObjectField("Library", library, typeof(BackgroundLibrary), false);

        if (newLibrary != library)
        {
            library = newLibrary;
            ClearSelection();
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create New Library", GUILayout.Height(24)))
        {
            CreateNewLibraryAsset();
        }

        GUI.enabled = library != null;
        if (GUILayout.Button("Ping Library", GUILayout.Height(24)))
        {
            EditorGUIUtility.PingObject(library);
            Selection.activeObject = library;
        }
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawLibraryTab()
    {
        SerializedObject so = new SerializedObject(library);
        so.Update();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Library Summary", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Stages", library.MutableStages.Count.ToString());
        EditorGUILayout.LabelField("Types", library.MutableBackgroundTypes.Count.ToString());
        EditorGUILayout.LabelField("Transitions", library.MutableTransitions.Count.ToString());
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Library Asset", EditorStyles.boldLabel);

        SerializedProperty stagesProp = so.FindProperty("stages");
        SerializedProperty typesProp = so.FindProperty("backgroundTypes");
        SerializedProperty transitionsProp = so.FindProperty("transitions");

        EditorGUILayout.PropertyField(stagesProp, true);
        EditorGUILayout.PropertyField(typesProp, true);
        EditorGUILayout.PropertyField(transitionsProp, true);

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(library);
        EditorGUILayout.EndVertical();
    }

    private void DrawStagesTab()
    {
        EditorGUILayout.BeginHorizontal();

        DrawStageListPane();
        DrawStageDetailPane();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawTypesTab()
    {
        EditorGUILayout.BeginHorizontal();

        DrawTypeListPane();
        DrawTypeDetailPane();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawTransitionsTab()
    {
        EditorGUILayout.BeginHorizontal();

        DrawTransitionListPane();
        DrawTransitionDetailPane();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawValidateTab()
    {
        library.ValidateEntries(out List<string> errors, out List<string> warnings);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

        if (GUILayout.Button("Run Validation", GUILayout.Height(24)))
        {
            library.ValidateEntries(out errors, out warnings);
        }

        EditorGUILayout.Space(4f);

        if (errors.Count == 0 && warnings.Count == 0)
        {
            EditorGUILayout.HelpBox("No validation issues found.", MessageType.Info);
        }

        if (errors.Count > 0)
        {
            EditorGUILayout.LabelField("Errors", EditorStyles.boldLabel);
            foreach (string error in errors)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }
        }

        if (warnings.Count > 0)
        {
            EditorGUILayout.LabelField("Warnings", EditorStyles.boldLabel);
            foreach (string warning in warnings)
            {
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawStageListPane()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(260));
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Stages", EditorStyles.boldLabel);

        leftScroll = EditorGUILayout.BeginScrollView(
            leftScroll,
            GUILayout.Height(position.height - 160f)
        );

        foreach (BackgroundStageDefinition stage in library.MutableStages)
        {
            if (stage == null)
                continue;

            GUIStyle style =
                selectedStage == stage ? EditorStyles.toolbarButton : EditorStyles.miniButton;

            if (GUILayout.Button(stage.name, style))
            {
                selectedStage = stage;
                selectedType = null;
                selectedTransition = null;
            }
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Stage"))
        {
            selectedStage = CreateSubAsset<BackgroundStageDefinition>("Stage");
            if (selectedStage != null)
            {
                library.MutableStages.Add(selectedStage);
                SaveLibraryAsset();
            }
        }

        GUI.enabled = selectedStage != null;
        if (GUILayout.Button("Remove Selected Stage"))
        {
            RemoveStage(selectedStage);
            selectedStage = null;
            SaveLibraryAsset();
        }
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void DrawStageDetailPane()
    {
        EditorGUILayout.BeginVertical();

        if (selectedStage == null)
        {
            EditorGUILayout.HelpBox("Select a stage to edit.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        rightScroll = EditorGUILayout.BeginScrollView(rightScroll);

        DrawNameEditor(selectedStage, "Stage Name");

        SerializedObject so = new SerializedObject(selectedStage);
        so.Update();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Stage Properties", EditorStyles.boldLabel);
        DrawScriptableObjectBody(so);
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(selectedStage);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawTypeListPane()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(260));
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Background Types", EditorStyles.boldLabel);

        leftScroll = EditorGUILayout.BeginScrollView(
            leftScroll,
            GUILayout.Height(position.height - 160f)
        );

        foreach (BackgroundTypeDefinition type in library.MutableBackgroundTypes)
        {
            if (type == null)
                continue;

            GUIStyle style =
                selectedType == type ? EditorStyles.toolbarButton : EditorStyles.miniButton;

            if (GUILayout.Button(type.name, style))
            {
                selectedType = type;
                selectedStage = null;
                selectedTransition = null;
            }
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Type"))
        {
            selectedType = CreateSubAsset<BackgroundTypeDefinition>("Type");
            if (selectedType != null)
            {
                library.MutableBackgroundTypes.Add(selectedType);
                SaveLibraryAsset();
            }
        }

        GUI.enabled = selectedType != null;
        if (GUILayout.Button("Remove Selected Type"))
        {
            RemoveType(selectedType);
            selectedType = null;
            SaveLibraryAsset();
        }
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void DrawTypeDetailPane()
    {
        EditorGUILayout.BeginVertical();

        if (selectedType == null)
        {
            EditorGUILayout.HelpBox("Select a type to edit.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        rightScroll = EditorGUILayout.BeginScrollView(rightScroll);

        DrawNameEditor(selectedType, "Type Name");

        SerializedObject so = new SerializedObject(selectedType);
        so.Update();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Type Properties", EditorStyles.boldLabel);
        DrawScriptableObjectBody(so);
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(selectedType);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawTransitionListPane()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(260));
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Transitions", EditorStyles.boldLabel);

        leftScroll = EditorGUILayout.BeginScrollView(
            leftScroll,
            GUILayout.Height(position.height - 160f)
        );

        foreach (BackgroundTransitionDefinition transition in library.MutableTransitions)
        {
            if (transition == null)
                continue;

            GUIStyle style =
                selectedTransition == transition
                    ? EditorStyles.toolbarButton
                    : EditorStyles.miniButton;

            if (GUILayout.Button(transition.name, style))
            {
                selectedTransition = transition;
                selectedStage = null;
                selectedType = null;
            }
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Transition"))
        {
            selectedTransition = CreateSubAsset<BackgroundTransitionDefinition>("Transition");
            if (selectedTransition != null)
            {
                library.MutableTransitions.Add(selectedTransition);
                SaveLibraryAsset();
            }
        }

        GUI.enabled = selectedTransition != null;
        if (GUILayout.Button("Remove Selected Transition"))
        {
            RemoveTransition(selectedTransition);
            selectedTransition = null;
            SaveLibraryAsset();
        }
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void DrawTransitionDetailPane()
    {
        EditorGUILayout.BeginVertical();

        if (selectedTransition == null)
        {
            EditorGUILayout.HelpBox("Select a transition to edit.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        rightScroll = EditorGUILayout.BeginScrollView(rightScroll);

        DrawNameEditor(selectedTransition, "Transition Name");

        SerializedObject so = new SerializedObject(selectedTransition);
        so.Update();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Transition Properties", EditorStyles.boldLabel);
        DrawScriptableObjectBody(so);
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(selectedTransition);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawNameEditor(Object targetObject, string label)
    {
        if (targetObject == null)
            return;

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);

        string newName = EditorGUILayout.TextField(label, targetObject.name);
        if (newName != targetObject.name && !string.IsNullOrWhiteSpace(newName))
        {
            Undo.RecordObject(targetObject, $"Rename {targetObject.GetType().Name}");
            targetObject.name = newName;
            EditorUtility.SetDirty(targetObject);
            SaveLibraryAsset();
            Repaint();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawScriptableObjectBody(SerializedObject so)
    {
        SerializedProperty iterator = so.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (iterator.name == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }

    private void CreateNewLibraryAsset()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Background Library",
            "BackgroundLibrary",
            "asset",
            "Choose where to save the BackgroundLibrary asset."
        );

        if (string.IsNullOrWhiteSpace(path))
            return;

        BackgroundLibrary newLibrary = CreateInstance<BackgroundLibrary>();
        AssetDatabase.CreateAsset(newLibrary, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        library = newLibrary;
        ClearSelection();
        EditorGUIUtility.PingObject(library);
        Selection.activeObject = library;
    }

    private T CreateSubAsset<T>(string defaultName)
        where T : ScriptableObject
    {
        if (library == null)
            return null;

        T asset = CreateInstance<T>();
        asset.name = GenerateUniqueSubAssetName(defaultName);

        AssetDatabase.AddObjectToAsset(asset, library);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(library));
        EditorUtility.SetDirty(asset);
        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();

        return asset;
    }

    private string GenerateUniqueSubAssetName(string baseName)
    {
        int index = 1;
        string candidate = baseName;

        Object[] children = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(library));
        HashSet<string> existingNames = new HashSet<string>();
        foreach (Object child in children)
        {
            existingNames.Add(child.name);
        }

        while (existingNames.Contains(candidate))
        {
            index++;
            candidate = $"{baseName}_{index}";
        }

        return candidate;
    }

    private void RemoveStage(BackgroundStageDefinition stage)
    {
        if (library == null || stage == null)
            return;

        library.MutableStages.Remove(stage);
        RemoveSubAsset(stage);
    }

    private void RemoveType(BackgroundTypeDefinition type)
    {
        if (library == null || type == null)
            return;

        library.MutableBackgroundTypes.Remove(type);

        foreach (BackgroundStageDefinition stage in library.MutableStages)
        {
            if (stage == null)
                continue;

            stage.BackgroundTypes.Remove(type);
            EditorUtility.SetDirty(stage);
        }

        foreach (BackgroundTypeDefinition otherType in library.MutableBackgroundTypes)
        {
            if (otherType == null || otherType.AllowedNextTypes == null)
                continue;

            otherType.AllowedNextTypes.RemoveAll(rule => rule != null && rule.TargetType == type);
            EditorUtility.SetDirty(otherType);
        }

        List<BackgroundTransitionDefinition> transitionsToRemove =
            new List<BackgroundTransitionDefinition>();

        foreach (BackgroundTransitionDefinition transition in library.MutableTransitions)
        {
            if (transition == null)
                continue;

            if (transition.FromType == type || transition.ToType == type)
            {
                transitionsToRemove.Add(transition);
            }
        }

        foreach (BackgroundTransitionDefinition transition in transitionsToRemove)
        {
            RemoveTransition(transition);
        }

        RemoveSubAsset(type);
    }

    private void RemoveTransition(BackgroundTransitionDefinition transition)
    {
        if (library == null || transition == null)
            return;

        library.MutableTransitions.Remove(transition);

        foreach (BackgroundStageDefinition stage in library.MutableStages)
        {
            if (stage == null)
                continue;

            stage.Transitions.Remove(transition);
            EditorUtility.SetDirty(stage);
        }

        RemoveSubAsset(transition);
    }

    private void RemoveSubAsset(Object asset)
    {
        if (asset == null || library == null)
            return;

        string libraryPath = AssetDatabase.GetAssetPath(library);

        EditorUtility.SetDirty(library);
        AssetDatabase.RemoveObjectFromAsset(asset);
        DestroyImmediate(asset, true);
        AssetDatabase.ImportAsset(libraryPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SaveLibraryAsset()
    {
        if (library == null)
            return;

        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ClearSelection()
    {
        selectedStage = null;
        selectedType = null;
        selectedTransition = null;
    }
}
