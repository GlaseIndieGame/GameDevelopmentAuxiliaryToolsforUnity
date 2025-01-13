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
        [MenuItem("Tools/GDATforUnity/Restart", priority = 75)]
        static void RestartEditor() => EditorApplication.OpenProject(Directory.GetCurrentDirectory());
    }
}
