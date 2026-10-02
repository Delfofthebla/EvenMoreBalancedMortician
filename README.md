# Even More Balanced Mortician

Source for the [Even More Balanced Mortician](EvenMoreBalancedMortician/Thunderstore/README.md) Risk of Rain 2 mod.

## Building

Requires the .NET SDK (6 or later) and an r2modman profile with [Mortician](https://thunderstore.io/package/Bog/Mortician/) and [StandaloneAncientScepter](https://thunderstore.io/package/amogus_lovers/StandaloneAncientScepter/) installed. Game, BepInEx, and R2API references come from NuGet (`NuGet.config` adds the BepInEx feed). Neither mod is on NuGet, so `Morris.dll` and `AncientScepter.dll` are referenced from the profile and never redistributed. The Scepter is only needed to build; at runtime it is optional.

```
dotnet build EvenMoreBalancedMortician.sln
```

Every build copies the mod into the r2modman profile. A Release build (`-c Release`) also writes a Thunderstore package to `dist/`.

The default profile is `Main`. To use another, create `Directory.Build.props.user` next to the solution:

```xml
<Project>
  <PropertyGroup>
    <ProfileName>Default</ProfileName>
  </PropertyGroup>
</Project>
```
