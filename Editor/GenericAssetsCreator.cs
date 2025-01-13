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
        private const int MENU_PRIORITY = 20;

        /// <summary>
        /// �I���t�H���_�z���ɔėp�I�ȃt�H���_(Animations,Audios��)���쐬���܂��B
        /// ���ɑ��݂���t�H���_�͕ύX���܂���B
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

        #region ���ʃt�H���_�������\�b�h

        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Resources", priority = MENU_PRIORITY)]
        private static void ResourcesFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Resources", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Editor", priority = MENU_PRIORITY)]
        private static void EditorFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Editor", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Gizmos", priority = MENU_PRIORITY)]
        private static void GizmosFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Gizmos", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Plugins", priority = MENU_PRIORITY)]
        private static void PluginsFolderCreate() => CreateDirectoryWithDummyFile(@"Assets/Plugins", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "StreamingAssets", priority = MENU_PRIORITY)]
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
        /// �w��p�X���ɓK�؂ȃ_�~�[�t�@�C���̎g�p��Ԃ�K�p
        /// </summary>
        /// <param name="path"></param>
        private static void ApplyBestDummyFileUsageWithPath(string path)
        {
            string dummyFilePath = $@"{path}/dummy.txt";

            if (File.Exists(dummyFilePath))
            {
                // ���^�t�@�C�������邽��2���傫���Ƃ��ɍ폜
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
        /// �I�����ꂽ�t�H���_���ɓK�؂ȃ_�~�[�t�@�C���̎g�p��Ԃ�K�p
        /// �t�H���_���Ƀ_�~�[�t�@�C���ȊO������=����
        /// �t�H���_���Ƀt�@�C�����Ȃ�=���
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
