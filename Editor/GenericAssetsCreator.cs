#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GDAT.Editor
{
    /// <summary>
    /// 汎用的なアセットを追加します
    /// </summary>
    public class GenericAssetsCreator : UnityEditor.Editor
    {
        private const string ASSETS_MENU_ROOT = @"Assets/Tools/GDATforUnity/";
        private const string SPECIAL_FOLDERPATH = ASSETS_MENU_ROOT + @"Add Special Folders/";
        private const int MENU_PRIORITY = 20;

        /// <summary>
        /// 選択フォルダ配下に汎用的なフォルダ(Animations,Audios等)を作成します。
        /// 既に存在するフォルダは変更しません。
        /// </summary>
        [MenuItem(ASSETS_MENU_ROOT + "Add Generic Folders", priority = MENU_PRIORITY)]
        private static void CreateGenericFoldersUnderSelectedFolders()
        {
            foreach (var asset in Selection.objects)
            {
                if (asset is DefaultAsset)
                {
                    string folderPath = AssetDatabase.GetAssetPath(asset);

                    string[] defaultFolders =
                    {
                    "Animations",
                    "Audios",
                    "AnimatorControllers",
                    "Datas",
                    "Fonts",
                    "Materials",
                    "Prefabs",
                    "Scenes",
                    "Scripts",
                    "ScriptableObjects",
                    "Shaders",
                    "Textures",
                };

                    string filePath;
                    foreach (var item in defaultFolders)
                    {
                        filePath = Path.Combine(folderPath, item);
                        if (AssetDatabase.IsValidFolder(filePath))
                        {
                            continue;
                        }
                        CreateDirectoryWithDummyFile(filePath);
                    }

                    AssetDatabase.Refresh();
                }
            }
        }

        #region 特別フォルダ生成メソッド

        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Resources", priority = MENU_PRIORITY)]
        private static void ResourcesFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Resources", true);
        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Editor", priority = MENU_PRIORITY)]
        private static void EditorFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Editor", true);
        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Gizmos", priority = MENU_PRIORITY)]
        private static void GizmosFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Gizmos", true);
        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Plugins", priority = MENU_PRIORITY)]
        private static void PluginsFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Plugins", true);
        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "StreamingAssets", priority = MENU_PRIORITY)]
        private static void StreamingAssetsFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/StreamingAssets", true);

        #endregion

        /// <summary>
        /// ディレクトリの生成とともにダミーファイルも作成
        /// </summary>
        /// <param name="createDirectory"></param>
        /// <param name="isRefresh"></param>
        private static void CreateDirectoryWithDummyFile(string createDirectory, bool isRefresh = false)
        {
            string path = createDirectory;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                using (File.Create($@"{path}/dummy.txt")) { }
                if (isRefresh) { AssetDatabase.Refresh(); }
            }
        }

        /// <summary>
        /// 指定パス内に適切なダミーファイルの使用状態を適用
        /// </summary>
        /// <param name="path"></param>
        private static void ApplyBestDummyFileUsageWithPath(string path)
        {
            string dummyFilePath = $@"{path}/dummy.txt";

            if (File.Exists(dummyFilePath))
            {
                // メタファイルがあるため2より大きいときに削除
                if (Directory.GetFiles(path).Length > 2)
                {
                    FileUtil.DeleteFileOrDirectory(dummyFilePath);
                }
                return;
            }
            if (Directory.GetFiles(path).Length == 0)
            {
                using (File.Create(dummyFilePath)) { }
            }
        }

        /// <summary>
        /// 選択されたフォルダ内に適切なダミーファイルの使用状態を適用
        /// フォルダ内にダミーファイル以外がある=消す
        /// フォルダ内にファイルがない=作る
        /// </summary>
        [MenuItem(ASSETS_MENU_ROOT + "ApplyBestDummyFileUsage", priority = MENU_PRIORITY)]
        private static void ApplyBestDummyFileUsage()
        {
            foreach (var asset in Selection.objects)
            {
                if (asset is DefaultAsset)
                {
                    string rootDirectory = AssetDatabase.GetAssetPath(asset);
                    ApplyBestDummyFileUsageWithPath(rootDirectory);

                    string[] subDirectories = Directory.GetDirectories(rootDirectory, "*", SearchOption.AllDirectories);

                    foreach (string subDirectory in subDirectories)
                    {
                        ApplyBestDummyFileUsageWithPath(subDirectory);
                    }
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
#endif
