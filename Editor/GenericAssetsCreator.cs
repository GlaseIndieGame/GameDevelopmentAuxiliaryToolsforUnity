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
        const string MENU_ROOT = @"Tools/GameDevelopmentAuxiliaryToolsforUnity/";
        /// <summary>
        /// �I���t�H���_�z���ɔėp�I�ȃt�H���_(Animations,Audios��)���쐬���܂��B
        /// ���ɑ��݂���t�H���_�͕ύX���܂���B
        /// </summary>
        [MenuItem(MENU_ROOT + "CreateGenericFolders", priority = 0)]
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
                        FolderCreate(filePath);
                    }

                    AssetDatabase.Refresh();
                }
            }
        }

        private const string SPECIAL_FOLDERPATH = MENU_ROOT + @"SpecialFoleders/";

        #region ���ʃt�H���_�������\�b�h

        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Resources", priority = 0)]
        private static void ResourcesFolderCreate() => FolderCreate(@"Assets/Resources", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Editor", priority = 0)]
        private static void EditorFolderCreate() => FolderCreate(@"Assets/Editor", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Gizmos", priority = 0)]
        private static void GizmosFolderCreate() => FolderCreate(@"Assets/Gizmos", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "Plugins", priority = 0)]
        private static void PluginsFolderCreate() => FolderCreate(@"Assets/Plugins", true);
        /// <summary> ���ʂȃt�H���_�𐶐����܂� </summary>
        [MenuItem(SPECIAL_FOLDERPATH + "StreamingAssets", priority = 0)]
        private static void StreamingAssetsFolderCreate() => FolderCreate(@"Assets/StreamingAssets", true);

        #endregion

        private static void FolderCreate(string createDirectory, bool isRefresh = false)
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
        /// �K�؂ȃ_�~�[�t�@�C���̎g�p���@��K�p
        /// �t�H���_���Ƀ_�~�[�t�@�C���ȊO������=����
        /// �t�H���_���Ƀt�@�C�����Ȃ�=���
        /// </summary>
        [MenuItem(MENU_ROOT + "ApplyBestDummyFileUsage", priority = 0)]
        private static void ApplyBestDummyFileUsage()
        {
            string rootFolderPath = "Assets";

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

            AssetDatabase.Refresh();
        }
    }
}
#endif
