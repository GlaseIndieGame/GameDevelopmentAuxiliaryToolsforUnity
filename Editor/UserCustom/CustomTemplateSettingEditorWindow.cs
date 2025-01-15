using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDAT.Editor
{
    public class CustomTemplateSettingEditorWindow : EditorWindow
    {
        [System.Serializable]
        private class TemplateSettings
        {
            public bool IsEditor;
            public string Name;
            public string Path;
            public override string ToString()
            {
                return $"TemplateSettings: {IsEditor}, {Name}, {Path}";
            }
        }

        const string MENU_ROOT = @"Tools/GDATforUnity/";

        private readonly Dictionary<bool, string> ROOT_VARIABLE_NAME_DICTIONARY = new()
        {
            { false, "SCRIPT_MENU_ROOT" },
            { true, "EDITOR_SCRIPT_MENU_ROOT" },
        };

        private readonly Dictionary<bool, string> PRIORITY_VARIABLE_NAME_DICTIONARY = new()
        {
            { false, "SCRIPT_PRIORITY" },
            { true, "EDITOR_SCRIPT_PRIORITY" },
        };

        private const string INDENT = "        ";


        private readonly Encoding SHIFTJIS_ENCODING = Encoding.GetEncoding("shift-jis");

        private Label _usedTemplateLabel;

        [SerializeField]
        private MonoScript _targetScript;

        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        [SerializeField]
        private List<TemplateSettings> _templateSettings = new()
        {
            new TemplateSettings()
        };

        private int _selectIndex = 0;

        [MenuItem(MENU_ROOT + "CustomTemplateSettingEditorWindow", priority = 50)]
        private static void Window()
        {
            var window = GetWindow<CustomTemplateSettingEditorWindow>();
            window.Show();
        }

        // Window�N�����ɌĂ΂��
        private void OnEnable()
        {
            Init();
        }

        // UnityEditor�ɍX�V���������Ƃ��Ă΂��
        private void OnSelectionChange()
        {
            Update();
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Init()
        {
            var root = this.rootVisualElement;
            _visualTreeAsset.CloneTree(root);

            _usedTemplateLabel = root.Q<Label>("usedTemplateLabel");

            var serializeTarget = new SerializedObject(this);
            rootVisualElement.Bind(serializeTarget);

            var settingsContainer = root.Q("settingsContainer");
            {
                MultiColumnListView listView = new();
                {
                    listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
                    listView.itemsSource = _templateSettings;
                    listView.showAddRemoveFooter = true;
                    listView.showBorder = true;
                    listView.reorderable = true;
                    listView.selectionType = SelectionType.Multiple;
                    listView.reorderMode = ListViewReorderMode.Animated;
                    listView.itemsAdded += collection => serializeTarget.Update();
                    listView.itemsRemoved += collection =>
                    {
                        foreach (var index in collection)
                        {
                            if (index <= _selectIndex) { --_selectIndex; }
                            break;
                        }
                    };
                    listView.style.left = 0;
                    listView.style.right = 0;
                    listView.selectedIndicesChanged += collection =>
                    {
                        foreach (var index in collection)
                        {
                            _selectIndex = index;
                            break;
                        }
                    };

                    var isEditorColumn = CreateColumn("IsEditor", "�G�f�B�^�[?",
                        () => new Toggle(), (VisualElement elem, int index) =>
                        {
                            if (elem is Toggle property)
                            {
                                SerializedProperty fieldProperty = serializeTarget.FindProperty("_templateSettings").GetArrayElementAtIndex(index).FindPropertyRelative("IsEditor");
                                property.BindProperty(fieldProperty);
                            }
                        });
                    isEditorColumn.minWidth = 60;
                    isEditorColumn.maxWidth = 60;

                    var nameColumn = CreateColumn("Name", "�e���v���[�g��",
                        () => new TextField()
                        {
                            label = "",

                            multiline = true
                        },
                        (VisualElement elem, int index) =>
                        {
                            if (elem is TextField property)
                            {
                                SerializedProperty fieldProperty = serializeTarget.FindProperty("_templateSettings").GetArrayElementAtIndex(index).FindPropertyRelative("Name");
                                property.BindProperty(fieldProperty);
                            }
                        });
                    nameColumn.minWidth = 120;
                    nameColumn.maxWidth = 120;

                    var pathColumn = CreateColumn("Path", "�e���v���[�g�p�X",
                        () => new TextField()
                        {
                            label = "",
                            multiline = true
                        },
                        (VisualElement elem, int index) =>
                        {
                            if (elem is TextField property)
                            {
                                SerializedProperty fieldProperty = serializeTarget.FindProperty("_templateSettings").GetArrayElementAtIndex(index).FindPropertyRelative("Path");
                                property.BindProperty(fieldProperty);
                            }
                        });

                    listView.columns.Add(isEditorColumn);
                    listView.columns.Add(nameColumn);
                    listView.columns.Add(pathColumn);
                }

                settingsContainer.Add(listView);
            }

            UpdateUsedTemplateLabel();

            var addTemplateButton = root.Q<Button>("addTemplateButton");
            {
                addTemplateButton.clicked += AddTemplate;
            }

            var removeTemplateButton = root.Q<Button>("removeTemplateButton");
            {
                removeTemplateButton.clicked += RemoveTemplate;
            }

            Selection.selectionChanged += UpdateSettingPath;

            Column CreateColumn(string name, string title, System.Func<VisualElement> makeCell, System.Action<VisualElement, int> bindCell, bool stretchable = true)
            {
                return new Column()
                {
                    name = name,
                    title = title,
                    makeCell = makeCell,
                    bindCell = bindCell,
                    stretchable = stretchable
                };
            }
        }

        /// <summary>
        /// �g�p���̃e���v���[�g�����x���ɕ\��
        /// </summary>
        private void UpdateUsedTemplateLabel() => _usedTemplateLabel.text = $"\"Templates\": [{string.Join(", ", GetUsedTemplateName(ReadTargetCode()[0]))}]";

        /// <summary>
        /// �X�V
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// �I�𒆂̃p�X��ݒ�
        /// </summary>
        private void UpdateSettingPath()
        {
            if (!(_templateSettings?.Count > 0)) { return; }
            foreach (var item in Selection.objects)
            {
                if (item is TextAsset)
                {
                    _templateSettings[_selectIndex].Path = AssetDatabase.GetAssetPath(item);
                    break;
                }
            }
        }

        private string[] GetUsedTemplateName(string data)
        {
            data = data[(data.IndexOf(':') + 1)..];
            return data.Split(',');
        }

        /// <summary>
        /// �e���v���[�g��ǉ�
        /// </summary>
        private void AddTemplate()
        {
            const string NEW_KEYWORD = "// #NEW#";
            const string SCRIPT_TEMPLATE_FORMAT =
               "#INDENT#/// <summary>#NAME#�e���v���[�g����X�N���v�g�𐶐�</summary>#/n#" +
               "#INDENT#[MenuItem(#ROOT# + \"#NAME#\", priority = #PRIORITY#)]#/n#" +
               "#INDENT#private static void Create#NAME#Script() => CreateScriptFile(\"#PATH#\", \"New#NAME#Script.cs\");#/n#";

            var data = ReadTargetCode();

            string temp;
            string[] readTemplateNames = GetUsedTemplateName(data[0]);

            foreach (var settings in _templateSettings)
            {
                if (string.IsNullOrEmpty(settings.Name)
                    || string.IsNullOrEmpty(settings.Path))
                {
                    Debug.Log("�ݒ荀�ڂ��s�����Ă��܂�");
                    continue;
                }
                if (!File.Exists(settings.Path)
                    || Path.GetExtension(settings.Path) != ".txt")
                {
                    Debug.Log("�p�X�������ł�");
                    continue;
                }
                if (readTemplateNames.Contains(settings.Name))
                {
                    Debug.Log($"���̖��O�͎g���܂���={settings.Name}");
                    continue;
                }

                temp = SCRIPT_TEMPLATE_FORMAT;
                temp = temp.Replace("#INDENT#", INDENT);
                temp = temp.Replace("#NAME#", settings.Name);
                temp = temp.Replace("#ROOT#", ROOT_VARIABLE_NAME_DICTIONARY[settings.IsEditor]);
                temp = temp.Replace("#PRIORITY#", PRIORITY_VARIABLE_NAME_DICTIONARY[settings.IsEditor]);
                temp = temp.Replace("#PATH#", settings.Path);

                data[0] += ',' + settings.Name;
                data.InsertRange(data.IndexOf(INDENT + NEW_KEYWORD), temp.Split("#/n#"));
            }

            WriteTargetCode(data);
            UpdateUsedTemplateLabel();
        }

        /// <summary>
        /// �e���v���[�g���폜
        /// </summary>
        private void RemoveTemplate()
        {
            const string SEARCH_FORMAT = "/// <summary>#NAME#�e���v���[�g����X�N���v�g�𐶐�</summary>";

            var data = ReadTargetCode();

            string temp = data[0];
            temp = temp[(temp.IndexOf(':') + 1)..];
            var readTemplateNames = temp.Split(',');

            int index;

            foreach (var settings in _templateSettings)
            {
                temp = INDENT + SEARCH_FORMAT.Replace("#NAME#", settings.Name);
                index = data.IndexOf(temp);

                if (readTemplateNames.Contains(settings.Name)
                    && index >= 0)
                {
                    data[0] = data[0].Replace(',' + settings.Name, "");
                    data.RemoveRange(index, 4); // �������ݓ��e��4�s�ł��邽��
                }
            }

            WriteTargetCode(data);
            UpdateUsedTemplateLabel();
        }

        /// <summary>�^�[�Q�b�g�X�N���v�g�̃p�X���擾</summary>
        /// <returns>���΃p�X</returns>
        private string GetTargetPath() => AssetDatabase.GetAssetPath(_targetScript);

        /// <summary>�^�[�Q�b�g�X�N���v�g�̃R�[�h��ǂݎ��</summary>
        /// <returns>�ǂݎ��f�[�^</returns>
        private List<string> ReadTargetCode() => File.ReadAllLines(GetTargetPath(), SHIFTJIS_ENCODING).ToList();

        /// <summary>�^�[�Q�b�g�X�N���v�g�ɃR�[�h����������</summary>
        /// <param name="text">�������݃f�[�^</param>
        private void WriteTargetCode(List<string> text)
        {
            File.WriteAllLines(GetTargetPath(), text, SHIFTJIS_ENCODING);
            AssetDatabase.Refresh();
        }
    }
}