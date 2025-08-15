using Assets.HSVPicker;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChroMapper_ColorPresetManager.Component
{
    public class ColorPresetController : MonoBehaviour
    {
        public string _settingJsonFile;
        public string _songPresetFile;
        public Dictionary<string, List<Color>> _presetLists = new Dictionary<string, List<Color>>();
        public UIDropdown _presetDropdown;
        public TextBoxComponent _nameTextBox;
        public string _songPresetOption;
        public List<Color> _tempColors;
        public void Start()
        {
            this._settingJsonFile = Path.Combine(Application.persistentDataPath, "ColorPresetManager.json");
            this._songPresetFile = Path.Combine(BeatSaberSongContainer.Instance.Info.Directory, "ChromaColors.json");
            //　UI作成
            var picker = GameObject.Find("MapEditorUI/Chroma Colour Selector/Chroma Colour Selector/Picker 2.0");
            if (picker == null)
                return;
            Debug.Log("Picker 2.0 Found");

            // プリセットツール親オブジェクト作成
            var presetTools = new GameObject("Preset Tools");
            presetTools.transform.SetParent(picker.transform);
            var presetLayoutGroup = presetTools.AddComponent<HorizontalLayoutGroup>();
            presetLayoutGroup.spacing = 10;
            presetLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            presetLayoutGroup.reverseArrangement = false;
            presetLayoutGroup.childControlWidth = false;
            presetLayoutGroup.childControlHeight = false;
            presetLayoutGroup.childScaleWidth = false;
            presetLayoutGroup.childScaleHeight = false;
            presetLayoutGroup.childForceExpandWidth = false;
            presetLayoutGroup.childForceExpandHeight = true;
            var presetLayoutElement = presetTools.AddComponent<LayoutElement>();
            presetLayoutElement.ignoreLayout = false;
            presetLayoutElement.minWidth = 234;
            presetLayoutElement.minHeight = 35;
            presetLayoutElement.preferredWidth = 234;
            presetLayoutElement.preferredHeight = 35;
            presetLayoutElement.flexibleWidth = 0;
            presetLayoutElement.flexibleHeight = 1;
            presetLayoutElement.layoutPriority = 1;
            presetTools.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);
            // Color SelectorのUIサイズ変更
            var colourSelector = picker.transform.parent.GetComponent<RectTransform>();
            var colourSelectorPos = colourSelector.position;
            colourSelector.position = new Vector3(colourSelectorPos.x, colourSelectorPos.y + 40f, colourSelectorPos.z);
            picker.transform.parent.GetComponent<ToggleColourDropdown>().YTop += 25f;

            // ドロップダウンリスト作成
            this._presetDropdown = Instantiate(PersistentUI.Instance.DropdownPrefab, presetTools.transform);
            this._presetDropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 25);
            this.PresetLoad();
            this._presetDropdown.name = "Preset Dropdown";
            this.SongPresetCheck();
            var options = new List<string>() {"New Save", this._songPresetOption};
            foreach (var preset in this._presetLists)
                options.Add(preset.Key);
            this._presetDropdown.SetOptions(options);
            var image = this._presetDropdown.GetComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.35f, 0.35f, 0.35f, 1f);
            image.pixelsPerUnitMultiplier = 1.5f;

            // プリセットボタン作成
            this.AddButton(presetTools.transform, new Vector2(40, 25), "Load", "Load", 14, this.LoadAction);
            this.AddButton(presetTools.transform, new Vector2(35, 25), "Save", "Save", 14, this.SaveAction);
            this.AddButton(presetTools.transform, new Vector2(30, 25), "Del", "Del", 14, this.DeleteAction);
        }
        public UIButton AddButton(Transform parent,Vector2 size, string title, string text, float fontSize, UnityAction onClick)
        {
            var button = Instantiate(PersistentUI.Instance.ButtonPrefab, parent);
            button.GetComponent<RectTransform>().sizeDelta = size;
            button.name = title;
            button.Button.onClick.AddListener(onClick);
            button.SetText(text);
            button.Text.enableAutoSizing = false;
            button.Text.fontSize = fontSize;
            return button;
        }

        public void SongPresetCheck()
        {
            this._songPresetOption = "Song Preset";
            if (File.Exists(_songPresetFile))
                this._songPresetOption += " [Available]";
        }

        public void LoadAction()
        {
            var dropdown = this._presetDropdown.Dropdown;
            if (dropdown.value == 0 || dropdown.value == 1 && !File.Exists(this._songPresetFile))
                return;
            this._tempColors= null;
            if (dropdown.value == 1)
                this._tempColors = this.SongPresetLoad();
            else if (!this._presetLists.TryGetValue(dropdown.options[dropdown.value].text, out this._tempColors))
                this._tempColors = null;
            if (this._tempColors == null)
            {
                PersistentUI.Instance.DisplayMessage("Preset Load Error!", PersistentUI.DisplayMessageType.Center);
                return;
            }
            var title = $"Load the color preset for \"{dropdown.options[dropdown.value].text}\".";
            if (dropdown.value == 1)
                title = $"Load the color preset for Song Preset.";
            var dialogBox = PersistentUI.Instance.CreateNewDialogBox()
                .WithTitle(title);
            dialogBox.AddFooterButton(null, "PersistentUI", "cancel");
            dialogBox.AddFooterButton(this.LoadPreset, "PersistentUI", "ok");
            dialogBox.Open();
        }

        public void SaveAction()
        {
            DialogBox dialogBox;
            switch (this._presetDropdown.Dropdown.value)
            {
                case 0: // New Save
                    dialogBox = PersistentUI.Instance.CreateNewDialogBox()
                        .WithTitle("Save new preset.");
                    this._nameTextBox = dialogBox.AddComponent<TextBoxComponent>()
                        .WithLabel("Preset Name")
                        .WithInitialValue("");
                    dialogBox.AddFooterButton(null, "PersistentUI", "cancel");
                    dialogBox.AddFooterButton(this.NewSave, "PersistentUI", "ok");
                    dialogBox.Open();
                    break;
                case 1: // Song Preset
                    dialogBox = PersistentUI.Instance.CreateNewDialogBox()
                        .WithTitle("Save the Song Preset.");
                    dialogBox.AddFooterButton(null, "PersistentUI", "cancel");
                    dialogBox.AddFooterButton(this.SongPresetSave, "PersistentUI", "ok");
                    dialogBox.Open();
                    break;
                default:
                    var dropdown = this._presetDropdown.Dropdown;
                    dialogBox = PersistentUI.Instance.CreateNewDialogBox()
                        .WithTitle($"Override \"{dropdown.options[dropdown.value].text}\" preset.");
                    dialogBox.AddFooterButton(null, "PersistentUI", "cancel");
                    dialogBox.AddFooterButton(this.OverridePreset, "PersistentUI", "ok");
                    dialogBox.Open();
                    break;
            }
        }
        public void DeleteAction()
        {
            var dropdown = this._presetDropdown.Dropdown;
            if (dropdown.value == 0 || dropdown.value == 1 && !File.Exists(this._songPresetFile))
                return;
            var title = $"Delete \"{dropdown.options[dropdown.value].text}\" preset.";
            if (dropdown.value == 1)
                title = $"Delete Song Preset.";
            var dialogBox = PersistentUI.Instance.CreateNewDialogBox()
                .WithTitle(title);
            dialogBox.AddFooterButton(null, "PersistentUI", "cancel");
            dialogBox.AddFooterButton(this.DeletePreset, "PersistentUI", "ok");
            dialogBox.Open();
        }

        public void NewSave()
        {
            var dropdown = this._presetDropdown.Dropdown;
            var name = this._nameTextBox.Value.Trim();
            if (name == "")
            {
                PersistentUI.Instance.DisplayMessage("Name Empty!", PersistentUI.DisplayMessageType.Center);
                return;
            }
            if (this._presetLists.ContainsKey(name))
            {
                PersistentUI.Instance.DisplayMessage("Name conflict!", PersistentUI.DisplayMessageType.Center);
                return;
            }
            this._presetLists.Add(name, new List<Color>(ColorPresetManager.Get().Colors));
            dropdown.options.Add(new TMP_Dropdown.OptionData(name));
            dropdown.RefreshShownValue();
            this.PresetSave();
            Debug.Log("NewSave");
        }

        public void SongPresetSave()
        {
            var dropdown = this._presetDropdown.Dropdown;
            var obj = new JSONObject();
            var colors = new JSONArray();
            foreach (var color in ColorPresetManager.Get().Colors)
            {
                var node = new JSONObject();
                node.WriteColor(color);
                colors.Add(node);
            }

            obj.Add("colors", colors);
            using (var writer = new StreamWriter(this._songPresetFile, false))
            {
                writer.Write(obj.ToString());
            }
            this.SongPresetCheck();
            dropdown.options[1].text = this._songPresetOption;
            dropdown.RefreshShownValue();
            Debug.Log("Song Preset Colors saved!");
        }

        public void OverridePreset()
        {
            var dropdown = this._presetDropdown.Dropdown;
            this._presetLists[dropdown.options[dropdown.value].text] = new List<Color>(ColorPresetManager.Get().Colors);
            this.PresetSave();
            Debug.Log("OverridePreset");
        }

        public void LoadPreset()
        {
            ColorPresetManager.Get().UpdateList(this._tempColors);
            Debug.Log("LoadPreset");
        }

        public void DeletePreset()
        {
            Debug.Log("DeletePreset");
            var dropdown = this._presetDropdown.Dropdown;
            if (dropdown.value == 1)
            {
                if (File.Exists(this._songPresetFile))
                    File.Delete(this._songPresetFile);
                this.SongPresetCheck();
                dropdown.options[1].text = this._songPresetOption;
                dropdown.RefreshShownValue();
                return;
            }
            if (this._presetLists.Remove(dropdown.options[dropdown.value].text))
            {
                dropdown.options.RemoveAt(dropdown.value);
                dropdown.SetValueWithoutNotify(0);
                dropdown.RefreshShownValue();
                this.PresetSave();
            }
            else
                PersistentUI.Instance.DisplayMessage("Delete Error!", PersistentUI.DisplayMessageType.Center);
        }

        public void PresetSave()
        {
            var obj = new JSONObject();
            foreach (var preset in this._presetLists)
            {
                var colors = new JSONArray();
                foreach (var color in preset.Value)
                {
                    var node = new JSONObject();
                    node.WriteColor(color);
                    colors.Add(node);
                }
                obj.Add(preset.Key, colors);
            }   
            using (var writer = new StreamWriter(this._settingJsonFile, false))
            {
                writer.Write(obj.ToString());
            }
            Debug.Log("Color Preset Manager saved!");
        }

        public void PresetLoad()
        {
            if (!File.Exists(this._settingJsonFile))
            {
                Debug.Log("Color Preset Manager file doesn't exist! Skipping loading...");
                return;
            }
            try
            {
                using (var reader = new StreamReader(this._settingJsonFile))
                {
                    this._presetLists.Clear();
                    var mainNode = JSON.Parse(reader.ReadToEnd());
                    foreach (var n in mainNode)
                    {
                        var colors = new List<Color>();
                        foreach (JSONNode c in n.Value.AsArray)
                        {
                            var color = c.IsObject ? c.ReadColor(Color.black) : ColourManager.ColourFromInt(c.AsInt);
                            colors.Add(color);
                        }
                        this._presetLists.Add(n.Key, colors);
                    }
                }
                Debug.Log($"Loaded {this._presetLists.Count} Presets!");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public List<Color> SongPresetLoad()
        {
            if (!File.Exists(this._songPresetFile))
            {
                Debug.Log("Song Preset Colors file doesn't exist! Skipping loading...");
                return null;
            }
            var colors = new List<Color>();
            try
            {
                using (var reader = new StreamReader(this._songPresetFile))
                {
                    var mainNode = JSON.Parse(reader.ReadToEnd());
                    foreach (JSONNode n in mainNode["colors"].AsArray)
                    {
                        var color = n.IsObject ? n.ReadColor(Color.black) : ColourManager.ColourFromInt(n.AsInt);
                        colors.Add(color);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
            return colors;
        }
    }
}
