# Even More Balanced Mortician

Source for the [Even More Balanced Mortician](EvenMoreBalancedMortician/Thunderstore/README.md) Risk of Rain 2 mod.

## Building

Requires the .NET SDK (6 or later), Risk of Rain 2, and an r2modman profile with [Mortician](https://thunderstore.io/package/Bog/Mortician/) and its dependencies installed. The build references the game's and the profile's DLLs directly.

```
dotnet build EvenMoreBalancedMortician.sln
```

Every build copies the mod into the r2modman profile. A Release build (`-c Release`) also writes a Thunderstore package to `dist/`.

The default paths are a Steam install on `C:` and the r2modman profile `Main`. To use others, create `Directory.Build.props.user` next to the solution:

```xml
<Project>
  <PropertyGroup>
    <GameDir>D:\SteamLibrary\steamapps\common\Risk of Rain 2</GameDir>
    <ProfileName>Default</ProfileName>
  </PropertyGroup>
</Project>
```
