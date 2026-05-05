# 🛠 Troubleshooting: Build Errors (.NET 10 & C# 14)

If you are using **Visual Studio 2022 (v17.14 or lower)**, you might encounter build errors like `NETSDK1209` or `CS1617` because the project targets .NET 10 and uses experimental C# 14 features (like the `field` keyword).

To fix this and run the project on a stable Visual Studio environment, follow these steps:

### 1. Downgrade Target Framework
If you don't have the .NET 10 SDK installed, change the target to .NET 9.0:
* Right-click on the **pr.net** project -> **Properties**.
* Under **Application** > **General**, change **Target Framework** to `.NET 9.0`.

### 2. Enable C# Preview Features
Since the codebase uses C# 14 features not yet fully stable in older compilers, you need to force the `preview` language version:
* Right-click the project and select **Edit Project File** (`.csproj`).
* Locate the `<PropertyGroup>` section.
* Remove any existing `<LangVersion>` tags and add:
  ```xml
  <LangVersion>preview</LangVersion>