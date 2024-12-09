#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text;

namespace GDAT.Editor
{
    enum EncodingType
    {
        SHIFT_JIS = 0,
        UTF_8,
        UNICODE,
    }
    /// <summary>
    /// �e�L�X�g�u���G�f�B�^�[
    /// </summary>
	public class TextReplaceEditorWindow : EditorWindow
    {
        [System.Serializable]
        private struct ReplaceSetting
        {
            public string OldText;
            public string NewText;
        }

        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        [SerializeField]
        private List<string> _targetPaths = new();

        [SerializeField]
        private List<ReplaceSetting> _replaceSettings = new()
        {
            new()
        };

        private Encoding[] _encodings;

        [SerializeField]
        private EncodingType _currentEncodingType = EncodingType.SHIFT_JIS;

        private int _findIndex = 0;

        private static TextReplaceEditorWindow window;

        const string MENU_ROOT = @"Tools/GameDevelopmentAuxiliaryToolsforUnity/";

        [MenuItem(MENU_ROOT + "TextReplaceEditorWindow")]
        private static void Window()
        {
            window = GetWindow<TextReplaceEditorWindow>();
            window.minSize = new Vector2(0, 500);
            window.Show();
        }

        // Window�N�����ɌĂ΂��
        private void OnEnable()
        {
            WindowVisualInit();
            OnSelectionChange();
        }

        // UnityEditor�ɍX�V���������Ƃ��Ă΂��
        private void OnSelectionChange()
        {
            UpdateSelectPaths();
            UpdateResultText();
        }

        /// <summary>
        /// Window�̌����ڂ̏�����
        /// </summary>
        private void WindowVisualInit()
        {
            var root = this.rootVisualElement;
            _visualTreeAsset.CloneTree(root);

            var serializeTarget = new SerializedObject(this);
            rootVisualElement.Bind(serializeTarget);

            _encodings = new Encoding[3]
            {
                Encoding.GetEncoding("shift-jis"),
                Encoding.UTF8,
                Encoding.Unicode,
            };
            var encodingTypeField = root.Q<EnumField>("encodingTypeField");
            encodingTypeField.RegisterValueChangedCallback(changeEvent => UpdateResultText());

            var pathList = root.Q<ListView>("targetPaths");
            pathList.selectedIndicesChanged += collection =>
            {
                foreach (var index in collection)
                {
                    _findIndex = index;
                    UpdateResultText();
                    break;
                }
            };

            var listContainer = root.Q("replaceSettings");
            {
                var listView = new MultiColumnListView();
                {
                    listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
                    listView.itemsSource = _replaceSettings;
                    listView.showAddRemoveFooter = true;
                    listView.showBorder = true;
                    listView.reorderable = true;
                    listView.selectionType = SelectionType.Multiple;
                    listView.reorderMode = ListViewReorderMode.Animated;
                    listView.itemsAdded += collection => serializeTarget.Update();
                    listView.style.left = 0;
                    listView.style.right = 0;

                    var oldColumn = new Column()
                    {
                        name = "oldText",
                        title = "�u������������",
                        makeCell = () => new TextField()
                        {
                            label = "",
                            multiline = true
                        },
                        bindCell = (VisualElement elem, int index) =>
                        {
                            if (elem is TextField property)
                            {
                                SerializedProperty fieldProperty = serializeTarget.FindProperty("_replaceSettings").GetArrayElementAtIndex(index).FindPropertyRelative("OldText");
                                property.BindProperty(fieldProperty);
                                property.RegisterValueChangedCallback(changeEvent => UpdateResultText());
                            }
                        },
                        stretchable = true
                    };
                    var newColumn = new Column()
                    {
                        name = "newText",
                        title = "�V�K������",
                        makeCell = () => new TextField()
                        {
                            label = "",
                            multiline = true
                        },
                        bindCell = (VisualElement elem, int index) =>
                        {
                            if (elem is TextField property)
                            {
                                SerializedProperty fieldProperty = serializeTarget.FindProperty("_replaceSettings").GetArrayElementAtIndex(index).FindPropertyRelative("NewText");
                                property.BindProperty(fieldProperty);
                                property.RegisterValueChangedCallback(changeEvent => UpdateResultText());
                            }
                        },
                        stretchable = true
                    };

                    listView.columns.Add(oldColumn);
                    listView.columns.Add(newColumn);
                }

                listContainer.Add(listView);
            }

            var replaceButton = root.Q<Button>("replaceButton");
            replaceButton.clicked += TargetTextReplace;

            root.Q<Button>("textLengthSortButton").clicked += () =>
            {
                serializeTarget.Update();
                _replaceSettings = _replaceSettings.OrderByDescending(settings => settings.OldText.Length).ToList();
            };
        }

        /// <summary>
        /// �e�L�X�g�u�����ʕ\�����X�V
        /// </summary>
        private void UpdateResultText()
        {
            if (_targetPaths?.Count > 0)
            {
                if (_findIndex >= _targetPaths.Count)
                {
                    _findIndex = _targetPaths.Count - 1;
                }
                string text = System.IO.File.ReadAllText(_targetPaths[_findIndex], _encodings[(int)_currentEncodingType]);
                foreach (var item in _replaceSettings)
                {
                    if (string.IsNullOrEmpty(item.OldText)) { continue; }
                    text = text.Replace(item.OldText, item.NewText);
                }
                rootVisualElement.Q<TextField>("resultText").value = text;
            }
        }

        /// <summary>
        /// �I�𒆂̃t�@�C���p�X���X�g���X�V
        /// </summary>
        private void UpdateSelectPaths()
        {
            _targetPaths.Clear();

            foreach (var item in Selection.objects)
            {
                if (item is TextAsset)
                {
                    _targetPaths.Add(AssetDatabase.GetAssetPath(item));
                }
            }
        }

        /// <summary>
        /// �e�L�X�g�����ۂɏ�������
        /// </summary>
        private void TargetTextReplace()
        {
            foreach (var path in _targetPaths)
            {
                string targetText = System.IO.File.ReadAllText(path, _encodings[(int)_currentEncodingType]);

                foreach (var item in _replaceSettings)
                {
                    if (string.IsNullOrEmpty(item.OldText)) { continue; }
                    targetText = targetText.Replace(item.OldText, item.NewText);
                }

                System.IO.File.WriteAllText(path, targetText, _encodings[(int)_currentEncodingType]);
            }

            AssetDatabase.Refresh();

            Debug.Log("���ׂẴt�@�C���̏����������������܂���");
        }
    }
}
#endif
