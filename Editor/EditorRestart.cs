using System.IO;
using UnityEditor;

namespace GDAT.Editor
{
    /// <summary>
    /// UnityEditorを再起動するためのクラス
    /// </summary>
    public class EditorRestart
    {
        /// <summary>
        /// UnityEditorを再起動
        /// </summary>
        [MenuItem("Tools/GameDevelopmentAuxiliaryToolsforUnity/Restart", priority = 11)]
        static void RestartEditor() => EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }
}
