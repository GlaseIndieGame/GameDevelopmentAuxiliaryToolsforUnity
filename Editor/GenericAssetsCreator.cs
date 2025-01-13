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

        /// <summary>
        /// 選択フォルダ配下に汎用的なフォルダ(Animations,Audios等)を作成します。
        /// 既に存在するフォルダは変更しません。
        /// </summary>
        [MenuItem(ASSETS_MENU_ROOT + "Add Generic Folders", priority = 15)]
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
        [MenuItem(SPECIAL_FOLDERPATH + "Resources", priority = 15)]
        private static void ResourcesFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Resources", true);
        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Editor", priority = 15)]
        private static void EditorFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Editor", true);
        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Gizmos", priority = 15)]
        private static void GizmosFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Gizmos", true);
        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Plugins", priority = 15)]
        private static void PluginsFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Plugins", true);
        /// <summary> 特別なフォルダを生成します </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "StreamingAssets", priority = 15)]
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
        /// 選択されたフォルダ内で適切なダミーファイルの使用方法を適用
        /// フォルダ内にダミーファイル以外がある=消す
        /// フォルダ内にファイルがない=作る
        /// </summary>
        [MenuItem(ASSETS_MENU_ROOT + "ApplyBestDummyFileUsage", priority = 15)]
        private static void ApplyBestDummyFileUsage()
        {
            foreach (var asset in Selection.objects)
            {
                if (asset is DefaultAsset)
                {
                    string rootFolderPath = AssetDatabase.GetAssetPath(asset);

                    string[] subdirectories = Directory.GetDirectories(rootFolderPath, "*", SearchOption.AllDirectories);

                    foreach (string subdirectory in subdirectories)
                    {
                        string dummyFilePath = $@"{subdirectory}/dummy.txt";

                        if (File.Exists(dummyFilePath))
                        {
                            if (Directory.GetFiles(subdirectory).Length > 2)
                            {
                                UnityEngine.Debug.Log($"{subdirectory}からダミーファイルを削除！");
                                FileUtil.DeleteFileOrDirectory(dummyFilePath);
                            }
                            continue;
                        }
                        if (Directory.GetFiles(subdirectory).Length == 0)
                        {
                            UnityEngine.Debug.Log($"{subdirectory}にダミーファイルを生成！");
                            using (File.Create(dummyFilePath)) { }
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
#endif
