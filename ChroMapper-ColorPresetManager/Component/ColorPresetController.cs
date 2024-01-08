using Assets.HSVPicker;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChroMapper_ColorPresetManager.Component
{
    public class ColorPresetController : MonoBehaviour
    {
        public readonly string settingJsonFile = Path.Combine(Application.persistentDataPath, "ColorPresetManager.json");
        public readonly string songPresetFile = Path.Combine(BeatSaberSongContainer.Instance.Song.Directory, "SongColorPreset.json");
        public Dictionary<string, List<Color>> presetLists = new Dictionary<string, List<Color>>();
        public UIDropdown presetDropdown;
        public void Start()
        {
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
            colourSelector.position = new Vector3(colourSelectorPos.x, colourSelectorPos.y + 25f, colourSelectorPos.z);
            picker.transform.parent.GetComponent<ToggleColourDropdown>().YTop += 25f;

            // ドロップダウンリスト作成
            this.presetDropdown = Instantiate(PersistentUI.Instance.DropdownPrefab, presetTools.transform);
            this.presetDropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 25);
            this.presetDropdown.name = "Preset Dropdown";
            var options = new List<string>() {"New Save", "Song Preset", "Test" };
            this.presetDropdown.SetOptions(options);
            this.presetDropdown.Dropdown.SetValueWithoutNotify(0);
            var image = this.presetDropdown.GetComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.35f, 0.35f, 0.35f, 1f);
            image.pixelsPerUnitMultiplier = 1.5f;

            // プリセットボタン作成
            this.AddButton(presetTools.transform, new Vector2(40, 25), "Load", "Load", 14, this.LoadPreset);
            this.AddButton(presetTools.transform, new Vector2(35, 25), "Save", "Save", 14, this.SavePreset);
            this.AddButton(presetTools.transform, new Vector2(30, 25), "Del", "Del", 14, this.DeletePreset);
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

        public void LoadPreset()
        {
            Debug.Log("Load");
        }

        public void SavePreset()
        {
            Debug.Log("Save");
        }

        public void DeletePreset()
        {
            Debug.Log("Del");
        }

        public void Save()
        {
            var obj = new JSONObject();
            var colors = new JSONArray();
            foreach (var color in ColorPresetManager.Get().Colors)
            {
                var node = new JSONObject();
                node.WriteColor(color);
                colors.Add(node);
            }

            obj.Add("colors", colors);
            using (var writer = new StreamWriter(settingJsonFile, false))
            {
                writer.Write(obj.ToString());
            }

            Debug.Log("Chroma Colors saved!");
        }

        public void Load()
        {
            if (!File.Exists(settingJsonFile))
            {
                Debug.Log("Chroma Colors file doesn't exist! Skipping loading...");
                return;
            }

            try
            {
                ColorPresetManager.Presets.Clear();
                var presetList = new ColorPresetList("default");
                using (var reader = new StreamReader(settingJsonFile))
                {
                    var mainNode = JSON.Parse(reader.ReadToEnd());
                    foreach (JSONNode n in mainNode["colors"].AsArray)
                    {
                        var color = n.IsObject ? n.ReadColor(Color.black) : ColourManager.ColourFromInt(n.AsInt);
                        presetList.Colors.Add(color);
                    }
                }

                Debug.Log($"Loaded {presetList.Colors.Count} colors!");
                ColorPresetManager.Presets.Add("default", presetList);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

    }
}
