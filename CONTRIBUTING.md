# Contributing to GregTech Lite Resource Packs

[**Chinese**](/Docs/SimplifiedChinese/CONTRIBUTING.md) | [**English**](/CONTRIBUTING.md)

If you do not know how to contribute with the GregTech Lite Resource Packs,
then this document is useful for you. The goal of this document is teach the beginner and first-time contributor.

## How to Contribute?

## How to get Artifacts?

### Resource Pack Folder Structure

In the standard Resource Pack folder, it's folder structure has the following format:
```
ResourcePackName/
│
├── ResourcePack/
│   ├── assets/
│   ├── pack.mcmeta
│   └── pack.png
│
└── Sources/
    └── assets/
```
The `ResourcePack` folder is the standard Resource Pack content folder, and the `Sources` folder consists all work files
or template files.

### Resource Packer

The C# project `ResourcePacker` can automatically export Resource Pack artifacts. Run this project (or click button) to
get helps.

For example, if you want to export Jabberwocky Resource Pack artifact, run the following content in the terminal:

```
dotnet run --project ResourcePacker -- -s Jabberwocky/ResourcePack -o Artifacts/Jabberwocky.zip
```

and then it will export at the path `Artifacts` as `Jabberwocky.zip`.