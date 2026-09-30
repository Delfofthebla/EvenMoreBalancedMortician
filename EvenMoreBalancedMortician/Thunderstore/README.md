# Even More Balanced Mortician

A configurable rebalance of [Mortician](https://thunderstore.io/package/Bog/Mortician/). Every body stat, skill number, and mechanical change is a setting, and a preset dropdown switches them all at once.

**Incompatible with [BalancedMortician](https://thunderstore.io/package/Bloonjitsu7/BalancedMortician/).** You will need to disable it. Its values are available as a configuration preset instead.

## Configuration Presets

| Preset | Description |
|-|-|
| EvenMoreBalanced | This mod's rebalance. The default. |
| BalancedMortician | Bloonjitsu7's BalancedMortician, as it actually plays. |
| Original | Mortician with no changes. |

Choosing a preset overwrites every other setting with that preset's values.

## Configuration

`BepInEx/config/com.Delfofthebla.EvenMoreBalancedMortician.cfg`, or in-game through Risk Of Options. Changes apply immediately, and skill tooltips update to match.

| Section | Setting | EvenMoreBalanced | BalancedMortician | Original |
|-|-|-|-|-|
| Body Stats | Base Health / Per Level | 170 / 51 | 200 / 66 | 200 / 66 |
| | Base Regen / Per Level | 1 / 0.2 | 2.5 / 0.5 | 2.5 / 0.5 |
| | Base Armor / Per Level | 10 / 0 | 20 / 0 | 20 / 0 |
| | Base Damage / Per Level | 12 / 2.2 | 12 / 2.4 | 12 / 2.4 |
| Shovel Strike | Swing Damage Percent | 280 | 360 | 800 |
| | Launch Damage Percent | 350 | 600 | 350 |
| | Counts As Primary Skill Damage | true | true | false |
| Raise Dead | Ghoul Limit (0 = none) | 0 | 0 | 0 |
| | Ghoul Base Damage / Per Level | 8 / 1.6 | 8 / 1.6 | 12 / 2.4 |
| | Bite Damage Percent | 150 | 150 | 150 |
| | Spit Damage Percent | 100 | 100 | 100 |
| | Ghouls Inherit Equipment | false | false | true |
| Sacrifice | Detonation Damage Percent | 600 | 925 | 700 |
| | Detonation Radius | 20 | 20 | 18 |
| | Heal Percent | 10 | 15 | 15 |
| Tombstone | Tombstone Base Damage / Per Level | 12 / 2.4 | 12 / 2.4 | 12 / 2.4 |
| | Soul Orb Damage Percent | 200 | 200 | 350 |
| | Ghoul Spawn Interval | 10 | 10 | 10 |
| | Tombstone Lifetime (0 = unlimited) | 23 | 0 | 0 |

This mod's Ghoul Limit setting replaces the one in Mortician's own config. The base mod's setting is ignored while this mod is enabled.

## Multiplayer

Every player should use the same config. Minion damage follows the host's config; each player's shovel follows their own.
