#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GDAT.Editor
{
    /// <summary>
    /// �ėp�I�ȃA�Z�b�g��ǉ����܂�
    /// </summary>
    public class GenericAssetsCreator : UnityEditor.Editor
    {
        private const string ASSETS_MENU_ROOT = @"Assets/Tools/GDATforUnity/";
        private const string SPECIAL_FOLDERPATH = ASSETS_MENU_ROOT + @"Add Special Folders/";

        /// <summary>
        /// �I���t�H���_�z���ɔėp�I�ȃt�H���_(Animations,Audios��)���쐬���܂��B
        /// ���ɑ��݂���t�H���_�͕ύX���܂���B
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

        #region ���ʃt�H���_�������\�b�h

        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Resources", priority = 15)]
        private static void ResourcesFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Resources", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Editor", priority = 15)]
        private static void EditorFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Editor", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Gizmos", priority = 15)]
        private static void GizmosFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Gizmos", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Plugins", priority = 15)]
        private static void PluginsFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Plugins", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "StreamingAssets", priority = 15)]
        private static void StreamingAssetsFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/StreamingAssets", true);

        #endregion

        /// <summary>
        /// �f�B���N�g���̐����ƂƂ��Ƀ_�~�[�t�@�C�����쐬
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
        /// �I�����ꂽ�t�H���_���œK�؂ȃ_�~�[�t�@�C���̎g�p���@��K�p
        /// �t�H���_���Ƀ_�~�[�t�@�C���ȊO������=����
        /// �t�H���_���Ƀt�@�C�����Ȃ�=���
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
                                UnityEngine.Debug.Log($"{subdirectory}����_�~�[�t�@�C�����폜�I");
                                FileUtil.DeleteFileOrDirectory(dummyFilePath);
                            }
                            continue;
                        }
                        if (Directory.GetFiles(subdirectory).Length == 0)
                        {
                            UnityEngine.Debug.Log($"{subdirectory}�Ƀ_�~�[�t�@�C���𐶐��I");
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
