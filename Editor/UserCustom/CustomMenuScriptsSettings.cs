// 使えるテンプレート:Interface,PureC#,PureCysharp,UnitySingleton,ScriptableObject,EditorWindow
using UnityEditor;

namespace GDAT.Editor
{
    /// <summary>
    /// テンプレートスクリプトを生成
    /// 編集しないでください
    /// </summary>
    internal class CustomMenuScriptsSettings : UnityEditor.Editor
    {
        private const string SCRIPT_MENU_ROOT = "Assets/Tools/GDATforUnity/Add New Script/";
        private const string ESCRIPT_CRIPT_MENU_ROOT = "Assets/Tools/GDATforUnity/Add New Editor Script/";

        private const int SCRIPT_PRIORITY = 15;
        private const int EDITOR_SCRIPT_PRIORITY = 17;

        // #NEW#

        /// <summary>テンプレートファイルを元に新しいスクリプトを作成する</summary>
        /// <param name="templateFilePath">元にするファイル名</param>
        /// <param name="newScriptName">作成するスクリプト名</param>
        private static void CreateScriptFile(string templateFilePath, string newScriptName) => ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templateFilePath, newScriptName);
    }
}
