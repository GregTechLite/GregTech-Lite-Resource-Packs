# Contributing to GregTech Lite Resource Packs

[**Chinese**](/Docs/SimplifiedChinese/CONTRIBUTING.md) | [**English**](/CONTRIBUTING.md)

If you do not know how to contribute with the GregTech Lite Resource Packs,
then this document is useful for you. The goal of this document is teach the beginner and first-time contributor.

## How to Contribute?

## How to get Artifacts?

The C# project `ResourcePacker` can automatically export Resource Pack artifacts. Run this project (or click button) to
get helps.

For example, if you want to export Jabberwocky Resource Pack artifact, run the following content in the terminal:

```
dotnet run --project ResourcePacker -- -s Jabberwocky/ResourcePack -o Artifacts/Jabberwocky.zip
```

and then it will export at the path `Artifacts` as `Jabberwocky.zip`.