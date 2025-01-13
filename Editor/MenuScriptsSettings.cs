using System.Runtime.CompilerServices;
using UnityEditor;

namespace GDAT.Editor
{
    /// <summary>
    /// �e���v���[�g�X�N���v�g�𐶐�
    /// �l�ɂ���Ă悭�g���e���v���[�g�͈قȂ�̂ŐϋɓI�ɉ������Ă�������
    /// </summary>
    internal class MenuScriptsSettings : UnityEditor.Editor
    {
        // �e���v���[�g�̃t�@�C���p�X
        private const string TEMPLATE_FILEPATH = @"\ScriptTemplates";
        // �G�f�B�^��̕\�������鍀�ڂ̖��O
        private const string MENU_ROOT = "Assets/Tools/GDATforUnity/Add New Script/";
        private const string EDITOR_MENU_ROOT = "Assets/Tools/GDATforUnity/Add Editor Script/";

        // �C���^�[�t�F�[�X�̃e���v���[�g�t�@�C����
        private const string INTERFACE_TEMPLATE_FILE_NAME = "InterfaceTemplate.txt";
        private const string PURECYSHARP_TEMPLATE_FILE_NAME = "PureCysharpTemplate.txt";
        private const string UNITY_SINGLETON_TEMPLATE_FILE_NAME = "UnitySingletonTemplate.txt";
        private const string EDITORWINDOW_TEMPLATE_FILE_NAME = "EditorWindowTemplate.txt";
        private const string SCRIPTABLEOBJECT_TEMPLATE_FILE_NAME = "ScriptableObjectTemplate.txt";

        // �G�f�B�^�ɕ\������鍀�ڂ̏ꏊ�ƕ\������ݒ肷��
        [MenuItem(MENU_ROOT + "Interface", priority = 15)]
        private static void CreateInterfaceScript() => CreateScriptFile(INTERFACE_TEMPLATE_FILE_NAME, "NewInterfaceScript.cs");

        // �G�f�B�^�ɕ\������鍀�ڂ̏ꏊ�ƕ\������ݒ肷��
        [MenuItem(MENU_ROOT + "PureC#", priority = 15)]
        private static void CreatePureCysharpScript() => CreateScriptFile(PURECYSHARP_TEMPLATE_FILE_NAME, "NewPureCysharpScript.cs");

        // �G�f�B�^�ɕ\������鍀�ڂ̏ꏊ�ƕ\������ݒ肷��
        [MenuItem(MENU_ROOT + "UnitySingleton", priority = 15)]
        private static void CreateUnitySingletonScript() => CreateScriptFile(UNITY_SINGLETON_TEMPLATE_FILE_NAME, "NewUnitySingletonScript.cs");

        // �G�f�B�^�ɕ\������鍀�ڂ̏ꏊ�ƕ\������ݒ肷��
        [MenuItem(EDITOR_MENU_ROOT + "EditorWindow", priority = 15)]
        private static void CreateEditorWindowScript() => CreateScriptFile(EDITORWINDOW_TEMPLATE_FILE_NAME, "NewEditorWindowScript.cs");

        // �G�f�B�^�ɕ\������鍀�ڂ̏ꏊ�ƕ\������ݒ肷��
        [MenuItem(MENU_ROOT + "ScriptableObject", priority = 15)]
        private static void CreateScriptableObjectWindowScript() => CreateScriptFile(SCRIPTABLEOBJECT_TEMPLATE_FILE_NAME, "NewScriptableObjectScript.cs");

        /// <summary>
        /// �e���v���[�g�t�@�C�������ɐV�����X�N���v�g���쐬����
        /// </summary>
        /// <param name="templateFileName">���ɂ���t�@�C����</param>
        /// <param name="newScriptName">�쐬����X�N���v�g��</param>
        private static void CreateScriptFile(string templateFileName, string newScriptName)
        {
            // Path.Combine�Ɋւ��āA���������Ƒ��������������ƌ�������� Combine("A","B") => "A/B" �X���b�V���͏����Ă������Ȃ��Ă�����
            // ��O�������������Ɛ�΃p�X�ɂȂ�炵���A���A������������������A��O�������̊m��p�X�����g���Ȃ��Ȃ�
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile($"{System.IO.Path.GetDirectoryName(GetSourceFilePath()) + TEMPLATE_FILEPATH}/{templateFileName}", newScriptName);
        }

        // �������g�̃t�@�C���p�X
        private static string GetSourceFilePath([CallerFilePath] string sourceFilePath = "")
        {
            return sourceFilePath;
        }
    }
}
