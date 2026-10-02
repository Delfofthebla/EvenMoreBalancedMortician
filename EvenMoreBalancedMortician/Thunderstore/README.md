# Even More Balanced Mortician

A very configurable rebalance of [Mortician](https://thunderstore.io/package/Bog/Mortician/). Every base stat, ability number, or mechanical behavior change is a setting you can tweak. The mod comes with presets that can be modified via config file or [Risk Of Options](https://thunderstore.io/c/riskofrain2/p/Rune580/Risk_Of_Options/) if fiddling with numbers isn't really your cup of tea.

I recently started playing Risk of Rain 2 again after a long break and I found the Mortician mod. It's pretty neat! But holy moly is it overpowered. I wanted to play with it with some friends, but I really don't like overpowered survivors that break vanilla balance and I knew it was going to take some fiddling with to get just right, So I made this mod.

In addition to the balance presets, I've also expanded upon some item interaction capabilities such as the Lysate Cell or Elite Aspects.

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
|               | Regen / Per Level                  | 2.5 / 0.5 | 2.5 / 0.5         | 2.5 / 0.5        |
|               | Armor / Per Level                  | 20 / 0    | 20 / 0            | 20 / 0           |
|               | Damage / Per Level                 | 12 / 2.4  | 12 / 2.4          | 12 / 2.4         |
| Shovel Strike | Counts As Primary Skill Damage     | false     | true              | true             |
|               | Swing Damage                       | 800%      | 360%              | 280%             |
|               | Launch Damage                      | 350%      | 600%              | 400%             |
|               | Launch Proc Coefficient            | 1.0       | 1.0               | 1.0              |
| Raise Dead    | Cooldown (seconds)                 | 7         | 7                 | 7                |
|               | Ghoul Limit (0 = none)             | 0         | 0                 | 0                |
|               | Ghoul Health / Per Level           | 150 / 45  | 150 / 45          | 150 / 45         |
|               | Ghoul Base Damage / Per Level      | 12 / 2.4  | 8 / 1.6           | 8 / 1.6          |
|               | Ghoul Degen / Per Level            | 10 / 3    | 10 / 3            | 10 / 3           |
|               | Bite Damage                        | 150%      | 150%              | 150%             |
|               | Bite Proc Coefficient              | 1.0       | 1.0               | 1.0              |
|               | Cling Bite Proc Coefficient        | 0.8       | 0.8               | 0.8              |
|               | Spit Damage                        | 100%      | 100%              | 100%             |
|               | Spit Proc Coefficient              | 1.0       | 1.0               | 0.7              |
|               | Aspect Inherit Chance              | 100%      | 0%                | 25%              |
| Sacrifice     | Cooldown (seconds)                 | 6         | 6                 | 6                |
|               | Detonation Scales With Mortician   | false     | false             | true             |
|               | Detonation Damage                  | 700%      | 925%              | 800%             |
|               | Detonation Proc Coefficient        | 1.0       | 1.0               | 1.0              |
|               | Detonation Radius                  | 18        | 20                | 20               |
|               | Heal                               | 15%       | 15%               | 15%              |
| Tombstone     | Cooldown (seconds)                 | 30        | 30                | 30               |
|               | Tombstone Duration (0 = unlimited) | 0         | 0                 | 30               |
|               | Lysate Cell Adds Tombstone         | false     | false             | true             |
|               | Soul Recipient                     | Newest    | Newest            | Nearest          |
|               | Tombstone Base Damage / Per Level  | 12 / 2.4  | 12 / 2.4          | 12 / 2.4         |
|               | Soul Orb Damage                    | 350%      | 200%              | 250%             |
|               | Soul Orb Proc Coefficient          | 0.2       | 0.2               | 0.2              |
|               | Ghoul Spawn Interval (seconds)     | 10        | 10                | 10               |
| Restless Grave (Ancient Scepter) | Enabled         | false     | false             | true             |
|               | Raise Radius (meters)              | 25        | 25                | 25               |
|               | Raise Cooldown (seconds)           | 3         | 3                 | 3                |
|               | Risen Ghoul Limit Per Tombstone (0 = none) | 0 | 0               | 0                |


### Notes

**Ghoul Limit** replaces the "Ghoul limit" setting in Mortician's own config, which is ignored while this mod is enabled.

**Detonation Scales With Mortician.** A sacrificed ghoul's explosion normally scales with the ghoul's damage stat. Ghouls don't copy your items, so the explosion falls behind as your build grows. With this on, it scales with your own damage stat instead, picking up anything that raises it, such as Shaped Glass or Chronic Expansion. It always used your crit chance and on-hit items.

**Aspect Inherit Chance.** In the base mod, every ghoul copies whatever is in your equipment slot. Ghouls never activate equipment, so the only thing this really does is pass on an elite aspect, making the ghoul that elite. (Or the fuel array, lol) BalancedMortician turned the copying off entirely, which was a solid call, but I rather liked the idea of ghouls being able to inherit aspects.

This setting replaces both with a chance per aspect. Each new ghoul rolls once for an elite aspect in your equipment slot, and once for every copy of an aspect item you hold, from mods like [ZetAspects](https://thunderstore.io/package/William758/ZetAspects/) (any item with "Aspect" in its name). Holding 3 of one aspect item and 1 of another gives 4 rolls, and each success passes on one copy. No other items or equipment are ever copied.

The EvenMoreBalanced preset uses 25%, so the more aspects you carry, the more likely a ghoul inherits at least one. It's a middle ground between the base mod's "always" and BalancedMortician's "never". The Original preset's 100% matches the base mod for equipment, but also passes on aspect items, which the base mod never did. If you do not use ZetAspects, you may want to increase this a little.

**Restless Grave.** With [StandaloneAncientScepter](https://thunderstore.io/package/amogus_lovers/StandaloneAncientScepter/) installed, the Ancient Scepter upgrades Tombstone into Restless Grave: enemies slain near one of your tombstones rise as ghouls. Each tombstone has its own cooldown, and a kill goes to the nearest tombstone in range that is ready. Risen ghouls are ordinary ghouls, so they decay, count toward the Ghoul Limit, can inherit aspects, and give vengeful souls when slain. The Scepter is not required; without it, these settings do nothing. With Enabled off, the Scepter treats Mortician as a survivor it can't upgrade, the same as before this feature existed.

While you have Restless Grave, your tombstones show a ring on the ground marking the raise radius, visible to every player. Each player can hide it with **Visuals → Show Raise Radius**, which only affects their own screen.

## Multiplayer

The host's settings apply to every player in the lobby. Your own config is left untouched and applies again whenever you host or play solo.

Every player needs the same version of this mod to join a lobby.

## Credits

Icon: [Tombstone](https://game-icons.net/1x1/lorc/tombstone.html) by Lorc, from [game-icons.net](https://game-icons.net), licensed under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).
