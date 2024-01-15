# ChroMapper-ColorPresetManager

BeatSaberの作譜ツールの[ChroMapper](https://github.com/Caeden117/ChroMapper)で、Chromaカラーのプリセットを切り替えられるようになるプラグインです。

プリセットに名前をつけて管理できる他、譜面フォルダに専用の`ChromaColors.json`作成して保存・読み込みが可能です。


This plugin allows you to switch Chroma color presets in BeatSaber's notation tool, [ChroMapper](https://github.com/Caeden117/ChroMapper).

You can name and manage presets, and create a special `ChromaColors.json` file in the score folder for saving and loading.

![image](https://github.com/rynan4818/ChroMapper-ColorPresetManager/assets/14249877/e87851fb-b900-4f99-9ea9-28b44e9e1913)


# インストール方法 (How to Install)

1. [リリースページ](https://github.com/rynan4818/ChroMapper-ColorPresetManager/releases)から、最新のプラグインのzipファイルをダウンロードして下さい。

    Download the latest plug-in zip file from the [release page](https://github.com/rynan4818/ChroMapper-ColorPresetManager/releases).

2. ダウンロードしたzipファイルを解凍してChroMapperのインストールフォルダにある`Plugins`フォルダに`ChroMapper-ColorPresetManager.dll`をコピーします。

    Unzip the downloaded zip file and copy `ChroMapper-ColorPresetManager.dll` to the `Plugins` folder in the ChroMapper installation folder.

# 使用方法 (How to use)

ChromaカラーのパレットのUIの下部に、プリセット管理のドロップダウンリストとLoad,Save,Delボタンが追加されます。

ドロップダウンリストを`New Save`にして、`Save`ボタンを押すと名前を決めるダイアログが表示されます。

保存したプリセットを選択して`Load`ボタンで読み込み、`Del`ボタンで削除が可能です。

ドロップダウンリストを`Song Preset`にして、`Save`ボタンを押すと譜面フォルダに`ChromaColors.json`を作成して保存します。

譜面フォルダに`ChromaColors.json`がある場合は、ドロップダウンの表示が`Song Preset [Available]`となり`Load`で読み込みできます。

譜面フォルダに保存される`ChromaColors.json`のフォーマットは、ChroMapperオリジナルと同じです。



At the bottom of the UI for the Chroma color palette, a drop-down list for managing presets and a Load, Save, Del button will be added.

Set the drop-down list to `New Save` and press the `Save` button to open a dialog to input a name.

Select a saved preset and press the `Load` button to load it, or press the `Del` button to delete it.

Set the drop-down list to `Song Preset` and press `Save` button to create and save `ChromaColors.json` in the map folder.

If `ChromaColors.json` exists in the map folder, the drop-down list will show `Song Preset [Available]` and you can load it with `Load`.

The format of `ChromaColors.json` saved in the map folder is the same as the ChroMapper original.

# 開発者情報 (Developers)
このプロジェクトをビルドするには、ChroMapperのインストールパスを指定する`ChroMapper-ColorPresetManager\ChroMapper-ColorPresetManager.csproj.user`ファイルを作成する必要があります。

To build this project, you must create a `ChroMapper-ColorPresetManager\ChroMapper-ColorPresetManager.csproj.user` file that specifies the ChroMapper installation path.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ChroMapperDir>C:\TOOL\ChroMapper\chromapper</ChroMapperDir>
  </PropertyGroup>
</Project>
```
