using UnityEditor;

namespace GDAT.Editor
{
    /// <summary>
    /// テンプレートスクリプトを生成
    /// 人によってよく使うテンプレートは異なるので積極的に改造してください
    /// </summary>
    internal class MenuScriptsSettings : UnityEditor.Editor
    {
        // テンプレートのファイルパス
        private const string TEMPLATE_FILEPATH = @"\ScriptTemplates";
        // エディタ上の表示させる項目の名前
        private const string MENU_ROOT = "Assets/Tools/GDATforUnity/Add New Script/";
        private const string EDITOR_MENU_ROOT = "Assets/Tools/GDATforUnity/Add New Editor Script/";

        private const int SCRIPT_PRIORITY = 15;
        private const int EDITOR_SCRIPT_PRIORITY = 17;

        // インターフェースのテンプレートファイル名
        private const string INTERFACE_TEMPLATE_FILE_NAME = "InterfaceTemplate.txt";
        private const string PURECYSHARP_TEMPLATE_FILE_NAME = "PureCysharpTemplate.txt";
        private const string UNITY_SINGLETON_TEMPLATE_FILE_NAME = "UnitySingletonTemplate.txt";
        private const string SCRIPTABLEOBJECT_TEMPLATE_FILE_NAME = "ScriptableObjectTemplate.txt";

        private const string EDITORWINDOW_TEMPLATE_FILE_NAME = "EditorWindowTemplate.txt";

        // エディタに表示される項目の場所と表示名を設定する
        /// <summary>Interfaceテンプレートからスクリプトを生成</summary>
        [MenuItem(MENU_ROOT + "Interface", priority = SCRIPT_PRIORITY)]
        private static void CreateInterfaceScript() => CreateScriptFile(INTERFACE_TEMPLATE_FILE_NAME, "NewInterfaceScript.cs");

        // エディタに表示される項目の場所と表示名を設定する
        [MenuItem(MENU_ROOT + "PureC#", priority = SCRIPT_PRIORITY)]
        private static void CreatePureCysharpScript() => CreateScriptFile(PURECYSHARP_TEMPLATE_FILE_NAME, "NewPureCysharpScript.cs");

        // エディタに表示される項目の場所と表示名を設定する
        [MenuItem(MENU_ROOT + "UnitySingleton", priority = SCRIPT_PRIORITY)]
        private static void CreateUnitySingletonScript() => CreateScriptFile(UNITY_SINGLETON_TEMPLATE_FILE_NAME, "NewUnitySingletonScript.cs");

        // エディタに表示される項目の場所と表示名を設定する
        [MenuItem(MENU_ROOT + "ScriptableObject", priority = SCRIPT_PRIORITY)]
        private static void CreateScriptableObjectWindowScript() => CreateScriptFile(SCRIPTABLEOBJECT_TEMPLATE_FILE_NAME, "NewScriptableObjectScript.cs");

        // エディタに表示される項目の場所と表示名を設定する
        [MenuItem(EDITOR_MENU_ROOT + "EditorWindow", priority = EDITOR_SCRIPT_PRIORITY)]
        private static void CreateEditorWindowScript() => CreateScriptFile(EDITORWINDOW_TEMPLATE_FILE_NAME, "NewEditorWindowScript.cs");

        /// <summary>
        /// テンプレートファイルを元に新しいスクリプトを作成する
        /// </summary>
        /// <param name="templateFileName">元にするファイル名</param>
        /// <param name="newScriptName">作成するスクリプト名</param>
        private static void CreateScriptFile(string templateFileName, string newScriptName)
        {
            string templateDirectory = System.IO.Path.GetDirectoryName(GetSourceFilePathForUnity());
            // Path.Combineに関して、第一引き数と第二引き数を書くと結合される Combine("A","B") => "A/B" スラッシュは書いても書かなくてもいい
            // 第三引き数を書くと絶対パスになるらしく、第一、第二引き数が無視され、第三引き数の確定パスしか使われなくなる
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile($"{templateDirectory + TEMPLATE_FILEPATH}/{templateFileName}", newScriptName);
        }

        /// <summary>
        /// ソースファイルのパスを取得
        /// </summary>
        /// <param name="sourceFilePath">コンパイル時に解釈</param>
        /// <returns>Unityで利用可能な相対パス</returns>
        private static string GetSourceFilePathForUnity([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (sourceFilePath[0] != '.') { return sourceFilePath; }
            sourceFilePath = sourceFilePath[23..];  // ".\Library\PackageCache\"を削除
            string packageName = sourceFilePath.Substring(0, sourceFilePath.IndexOf('@'));
            int delimiterIndex = sourceFilePath.IndexOf(@"\"[0]);
            sourceFilePath = sourceFilePath.Substring(delimiterIndex, sourceFilePath.Length - delimiterIndex);
            return @"Packages\" + packageName + sourceFilePath;
        }
    }
}
