using System.IO;
using UnityEditor;

namespace GDAT.Editor
{
    /// <summary>
    /// UnityEditor���ċN�����邽�߂̃N���X
    /// </summary>
    public class EditorRestart
    {
        /// <summary>
        /// UnityEditor���ċN��
        /// </summary>
        [MenuItem("Tools/GameDevelopmentAuxiliaryToolsforUnity/Restart", priority = 11)]
        static void RestartEditor() => EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }
}
