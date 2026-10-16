# Even More Balanced Mortician

A rebalance of [Mortician](https://thunderstore.io/package/Bog/Mortician/), with a few new item & mod interactions on top.

I came back to Risk of Rain 2 after a long break and found the Mortician mod. It's pretty neat! But holy moly is he
overpowered. I wanted to play him with friends without him steamrolling vanilla balance, so I made this mod. Expect
semi-frequent updates for a while as I playtest and tweak the numbers.

**Incompatible with [BalancedMortician](https://thunderstore.io/package/Bloonjitsu7/BalancedMortician/).** This mod
disables itself if it finds it, so you'll need to remove one or the other. If you prefer Bloonjitsu7's numbers, they're
included here as a preset.

## Regarding any Future Bugs or Errors

PLEASE post them to my github before you bring them to Bog when using this mod. I actively override much of Mortician's
code and if you start having problems you should assume that it is _THIS MOD_ causing those issues not Mortician itself.
I will need to update after every Mortician update, so the moment you see one stop using this until I can get a fix out.

If a RoR2 patch/update/new dlc breaks things and you start running into issues, disable this mod first, verify the problem
still occurs, and THEN you can report it to Bog. When in doubt, just dump your error on the issues page of my Github and
I will take a look when I have time.

## What's different

![Mortician's skills with this mod's numbers](https://raw.githubusercontent.com/Delfofthebla/EvenMoreBalancedMortician/master/docs/screenshots/MorticianAbilityList.png)

### The Rebalance

#### **Base Stats**
* Starting health dropped from 200 to 180
* Health gain per level dropped from 66 to 54 (to match the 30% per level that every other survivor uses)

#### **Shovel Strike**
* Is now properly considered a primary skill (For Luminous Shot)
* Swing damage dropped from 800% to 340%,
* Launch damage increased from 350% to 400% (to hit the breakpoint for items that need a 400% hit to trigger)

#### **Ghouls**
* Base damage lowered from 12 to 8
* Spit proc coefficient dropped from 1.0 to 0.7
* No longer blindly inherits your equipment slot* (See below for new elite aspect behavior)

#### **Sacrifice**
* Damage now calculated from Mortician instead of the ghoul. (For better scaling and to avoid the ghoul damage nerf)
* Damage increased from 700% to 800%
* Range increased from 18m to 20m

#### **Soul Offering**
* On-Kill effects trigger chance lowered from 100% to 50%. (Affected by luck)

#### **Tombstone**
* Vengeful Soul damage dropped from 350% to 280%
* Raise timer for ghouls now affected by (global) cooldown reduction items
* Now properly supports Lysate Cell, using the same mechanism as Engineer (+1 tombstone maximum)
* Now supports Ancient Scepter* (See below for details)

### New Mechanics / Interactions

#### Elite aspects
* Every spawned ghoul has a 33% chance to inherit an Elite aspect that you're carrying (affected by Luck).

  This covers aspect equipment, and aspect items from [ZetAspects](https://thunderstore.io/package/William758/ZetAspects/), which roll once per copy you hold. If you don't play with ZetAspects, you may want to increase this, but follow your gut.

In the base mod, every ghoul copies whatever is in your equipment slot, regardless of the item. I _suspect_ that this was
meant to propagate passives (like Elite Aspects), but it's hard to know for certain. The problem is that the default
implementation worked on things like Fuel Array as well (lol) and they never activated the item so non-passives did nothing.
BalancedMortician turned that off entirely, which was fair, but I liked ghouls turning elite too much to lose it entirely.

#### Lysate Cell

Despite how strong Mortician is, I was genuinely surprised to learn that Lysate Cell did not work on the tombstone at all.
This felt like a power gap that I could fill rather than one to reign in. Engineer gets an extra turret from a Lysate Cell, 
after all. So now Mortician gets an extra tombstone. With two up, a slain ghoul's vengeful soul goes to whichever tombstone
is nearest to where it died.

#### Ancient Scepter

With [Standalone Ancient Scepter](https://thunderstore.io/package/amogus_lovers/StandaloneAncientScepter/) installed, the Scepter upgrades Tombstone into **Restless Grave**: enemies that
die near one of your tombstones rise as ghouls, up to one every 3 seconds per tombstone. (Affected by cooldown reduction items)

![The Restless Grave Upgrade](https://raw.githubusercontent.com/Delfofthebla/EvenMoreBalancedMortician/master/docs/screenshots/RestlessGrave.png)

The Scepter mod is not required. Without it, Tombstone works as usual.

## Don't like my numbers?

Everything above is a setting, from base stats to cooldowns and proc coefficients, and you can change it in game with
[Risk Of Options](https://thunderstore.io/c/riskofrain2/p/Rune580/Risk_Of_Options/) or in the config file. Each setting's description lists its value in every preset, and skill
tooltips update to match.

If you'd rather not tune things yourself, there are two other presets to fall back on:

- **Original** keeps Mortician's own numbers.
- **BalancedMortician** matches Bloonjitsu7's mod.

Both keep the Lysate Cell and Ancient Scepter support, since neither original mod had any. Everything else, Sacrifice
scaling included, follows the respective source mod.

Changing any setting switches you to the Custom preset. While you stay on a preset, updating this mod moves you to that
preset's new values.

**NOTE: This mod's Ghoul Limit setting replaces the one in Mortician's own config, which will be _ignored_ while this mod is enabled.**

## Multiplayer

The host's config settings apply to everyone in the lobby, and your tooltips _should_ update to account for this, if your
settings differ. Every player needs the same version of this mod.

## Credits

Icon: [Tombstone](https://game-icons.net/1x1/lorc/tombstone.html) by Lorc,
from [game-icons.net](https://game-icons.net), licensed under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).
The "Restless Grave" icon is just dynamically recolored during runtime with magic, so idk, credit to wizards or whatever.
