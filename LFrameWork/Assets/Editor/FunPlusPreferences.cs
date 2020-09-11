using UnityEditor;

/// <summary>
/// Preference工具拓展
/// </summary>
public class FunPlusPreferences
{
    [PreferenceItem("FunPlus Tools")]
    private static void PreferencesGUI()
    {
        TextEditorTool.DrawToolGUI();
        EditorGUILayout.Space();
    }
}