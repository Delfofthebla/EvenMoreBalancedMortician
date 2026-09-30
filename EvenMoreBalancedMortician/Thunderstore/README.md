# Even More Balanced Mortician

A very configurable rebalance of [Mortician](https://thunderstore.io/package/Bog/Mortician/). Every base stat, ability number, or mechanical behavior change is a setting you can tweak. The mod comes with presets that can be modified via config file or [Risk Of Options](https://thunderstore.io/c/riskofrain2/p/Rune580/Risk_Of_Options/) if fiddling with numbers isn't really your cup of tea.

I recently started playing Risk of Rain 2 again after a long break and I found the Mortician mod. It's pretty neat! But holy moly is it overpowered. I wanted to play with it with some friends, but I really don't like overpowered survivors that break vanilla balance and I knew it was going to take some fiddling with to get just right, So I made this mod.

**Incompatible with [BalancedMortician](https://thunderstore.io/package/Bloonjitsu7/BalancedMortician/).** You will need to disable it. Its values are available as a configuration preset instead.

## Configuration Presets

| Preset | Description |
|-|-|
| Original | Mortician with no changes. |
| BalancedMortician | Bloonjitsu7's BalancedMortician, as it actually plays. |
| EvenMoreBalanced | This mod's rebalance. The default. |

Choosing a preset overwrites every other setting with that preset's values.

## Configuration

`BepInEx/config/com.Delfofthebla.EvenMoreBalancedMortician.cfg`, or in-game through Risk Of Options. Changes apply immediately, and skill tooltips update to match.

| Section       | Setting                            | Original  | BalancedMortician | EvenMoreBalanced |
|---------------|------------------------------------|-----------|-------------------|------------------|
| Base Stats    | Health / Per Level                 | 200 / 66  | 200 / 66          | 170 / 51         |
|               | Regen / Per Level                  | 2.5 / 0.5 | 2.5 / 0.5         | 1 / 0.2          |
|               | Armor / Per Level                  | 20 / 0    | 20 / 0            | 10 / 0           |
|               | Damage / Per Level                 | 12 / 2.4  | 12 / 2.4          | 12 / 2.4         |
| Shovel Strike | Counts As Primary Skill Damage     | false     | true              | true             |
|               | Swing Damage                       | 800%      | 360%              | 280%             |
|               | Launch Damage                      | 350%      | 600%              | 350%             |
|               | Launch Proc Coefficient            | 1.0       | 1.0               | 1.0              |
| Raise Dead    | ***Ghoul Limit (0 = none)          | 0         | 0                 | 0                |
|               | Ghoul Base Damage / Per Level      | 12 / 2.4  | 8 / 1.6           | 8 / 1.6          |
|               | Bite Damage                        | 150%      | 150%              | 150%             |
|               | Bite Proc Coefficient              | 1.0       | 1.0               | 0.7              |
|               | Cling Bite Proc Coefficient        | 0.8       | 0.8               | 0.5              |
|               | Spit Damage                        | 100%      | 100%              | 100%             |
|               | Spit Proc Coefficient              | 1.0       | 1.0               | 0.7              |
|               | **Aspect Inherit Chance            | 100%      | 0%                | 25%              |
| Sacrifice     | Detonation Damage                  | 700%      | 925%              | 600%             |
|               | Detonation Proc Coefficient        | 1.0       | 1.0               | 1.0              |
|               | Detonation Radius                  | 18        | 20                | 20               |
|               | Heal                               | 15%       | 15%               | 10%              |
| Tombstone     | Tombstone Duration (0 = unlimited) | 0         | 0                 | 23               |
|               | Tombstone Base Damage / Per Level  | 12 / 2.4  | 12 / 2.4          | 12 / 2.4         |
|               | Soul Orb Damage                    | 350%      | 200%              | 250%             |
|               | Soul Orb Proc Coefficient          | 0.2       | 0.2               | 0.3              |
|               | Ghoul Spawn Interval (seconds)     | 10        | 10                | 10               |


\*\*In the base mod, Ghouls each inherit your equipment slot item but do not use it. The most likely intention for this is so that the ghouls may obtain Elite Aspects. The BalancedMortician mod author felt this was too powerful (or perhaps thought it was items rather than just equipment) and so they disabled this functionality entirely. My mod overrides the base mod's behavior and gives you a more nuanced configuration option that is capable of supporting non-equipment based or otherwise mod-added aspects such as those from [ZetAspects](https://thunderstore.io/package/William758/ZetAspects/) (Specifically, any equipment or normal item with "Aspect" in its name) With my preset, ghouls have a 25% chance to inherit your aspect item(s), rolled individually for each. The more aspects you have, the more likely at least one will be inherited. This is a compromise between the two other presets, and I feel it is a good balance between the two extremes.**

\*\*\*This mod's Ghoul Limit setting replaces the one in Mortician's own config. The base mod's setting is ignored while this mod is enabled.

## Multiplayer

The host's settings apply to every player in the lobby. Your own config is left untouched and applies again whenever you host or play solo.

Every player needs the same version of this mod to join a lobby.

## Credits

Icon: [Tombstone](https://game-icons.net/1x1/lorc/tombstone.html) by Lorc, from [game-icons.net](https://game-icons.net), licensed under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).
