// �g����e���v���[�g:Interface,PureC#,PureCysharp,UnitySingleton,ScriptableObject,EditorWindow
using UnityEditor;

namespace GDAT.Editor
{
    /// <summary>
    /// �e���v���[�g�X�N���v�g�𐶐�
    /// �ҏW���Ȃ��ł�������
    /// </summary>
    internal class CustomMenuScriptsSettings : UnityEditor.Editor
    {
        private const string SCRIPT_MENU_ROOT = "Assets/Tools/GDATforUnity/Add New Script/";
        private const string ESCRIPT_CRIPT_MENU_ROOT = "Assets/Tools/GDATforUnity/Add New Editor Script/";

        private const int SCRIPT_PRIORITY = 15;
        private const int EDITOR_SCRIPT_PRIORITY = 17;

        // #NEW#

        /// <summary>�e���v���[�g�t�@�C�������ɐV�����X�N���v�g���쐬����</summary>
        /// <param name="templateFilePath">���ɂ���t�@�C����</param>
        /// <param name="newScriptName">�쐬����X�N���v�g��</param>
        private static void CreateScriptFile(string templateFilePath, string newScriptName) => ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templateFilePath, newScriptName);
    }
}
