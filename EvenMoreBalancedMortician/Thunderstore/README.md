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
|               | Launch Damage                      | 350%      | 600%              | 360%             |
|               | Launch Proc Coefficient            | 1.0       | 1.0               | 1.0              |
| Raise Dead    | Cooldown (seconds)                 | 7         | 7                 | 7                |
|               | Ghoul Limit (0 = none)             | 0         | 0                 | 0                |
|               | Ghoul Base Damage / Per Level      | 12 / 2.4  | 8 / 1.6           | 8 / 1.6          |
|               | Ghoul Degen / Per Level            | 10 / 3    | 10 / 3            | 10 / 3           |
|               | Bite Damage                        | 150%      | 150%              | 150%             |
|               | Bite Proc Coefficient              | 1.0       | 1.0               | 1.0              |
|               | Cling Bite Proc Coefficient        | 0.8       | 0.8               | 0.8              |
|               | Spit Damage                        | 100%      | 100%              | 100%             |
|               | Spit Proc Coefficient              | 1.0       | 1.0               | 0.7              |
|               | Aspect Inherit Chance              | 100%      | 0%                | 25%              |
| Sacrifice     | Cooldown (seconds)                 | 6         | 6                 | 6                |
|               | Detonation Damage                  | 700%      | 925%              | 880%             |
|               | Detonation Proc Coefficient        | 1.0       | 1.0               | 1.0              |
|               | Detonation Radius                  | 18        | 20                | 20               |
|               | Heal                               | 15%       | 15%               | 10%              |
| Tombstone     | Cooldown (seconds)                 | 30        | 30                | 30               |
|               | Tombstone Duration (0 = unlimited) | 0         | 0                 | 30               |
|               | Tombstone Base Damage / Per Level  | 12 / 2.4  | 12 / 2.4          | 12 / 2.4         |
|               | Soul Orb Damage                    | 350%      | 200%              | 250%             |
|               | Soul Orb Proc Coefficient          | 0.2       | 0.2               | 0.2              |
|               | Ghoul Spawn Interval (seconds)     | 10        | 10                | 10               |


### Notes

**Ghoul Limit** replaces the "Ghoul limit" setting in Mortician's own config, which is ignored while this mod is enabled.

**Aspect Inherit Chance.** In the base mod, every ghoul copies whatever is in your equipment slot. Ghouls never activate equipment, so the only thing this really does is pass on an elite aspect, making the ghoul that elite. (Or the fuel array, lol) BalancedMortician turned the copying off entirely, which was a solid call, but I rather liked the idea of ghouls being able to inherit aspects.

This setting replaces both with a chance per aspect. Each new ghoul rolls once for an elite aspect in your equipment slot, and once for every stack of aspect items you hold, from mods like [ZetAspects](https://thunderstore.io/package/William758/ZetAspects/) (any item with "Aspect" in its name). No other items or equipment are ever copied.

The EvenMoreBalanced preset uses 25%, so the more aspects you carry, the more likely a ghoul inherits at least one. It's a middle ground between the base mod's "always" and BalancedMortician's "never". The Original preset's 100% matches the base mod for equipment, but also passes on aspect items, which the base mod never did. If you do not use ZetAspects, you may want to increase this a little.

## Multiplayer

The host's settings apply to every player in the lobby. Your own config is left untouched and applies again whenever you host or play solo.

Every player needs the same version of this mod to join a lobby.

## Changelog

### 1.1.0

**Added**
- Cooldown settings for Raise Dead, Sacrifice, and Tombstone.
- Ghoul Degen and Ghoul Degen Per Level settings, for how fast ghouls lose health.

**Fixed**
- Raising a ghoul logged an error every time, which stopped the Ghoul Limit setting and aspect inheritance from ever taking effect. Both should now work properly.

**Changed** (EvenMoreBalanced preset)
- Launch Damage: 350% → 360%
- Bite Proc Coefficient: 0.5 → 1.0
- Cling Bite Proc Coefficient: 0.4 → 0.8
- Spit Proc Coefficient: 0.5 → 0.7
- Detonation Damage: 600% → 880%
- Tombstone Duration: 23 → 30

Players on 1.0.0 cannot join a lobby hosted on 1.1.0, or the reverse.

### 1.0.0

- Initial release.

## Credits

Icon: [Tombstone](https://game-icons.net/1x1/lorc/tombstone.html) by Lorc, from [game-icons.net](https://game-icons.net), licensed under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).
