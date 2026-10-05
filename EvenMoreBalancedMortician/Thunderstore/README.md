# Even More Balanced Mortician

A rebalance of [Mortician](https://thunderstore.io/package/Bog/Mortician/), with a few new item interactions on top.

I came back to Risk of Rain 2 after a long break and found the Mortician mod. It's pretty neat! But holy moly is he
overpowered. I wanted to play him with friends without him steamrolling vanilla balance, so I made this mod. Expect
semi-frequent updates for a while as I playtest and tweak the numbers.

**Incompatible with [BalancedMortician](https://thunderstore.io/package/Bloonjitsu7/BalancedMortician/).** This mod
disables itself if it finds it, so you'll need to remove one or the other. If you prefer Bloonjitsu7's numbers, they're
included here as a preset.

## What's different

### The rebalance

![Mortician's skills with this mod's numbers](https://raw.githubusercontent.com/Delfofthebla/EvenMoreBalancedMortician/master/docs/screenshots/MorticianAbilityList.png)

Mortician's early-midgame damage mostly came from his primary that hit like a freight train, so that's where most of the
nerf went. The swing drops from 800% to 340% damage, but now counts as primary skill damage, so items like Luminous Shot
work with it. Flinging a ghoul or tombstone goes up a little, from 350% to 400%, which hits the breakpoint for items
that need a 400% hit.

Ghouls hit a bit softer (8 base damage instead of 12) and their spit's proc coefficient was nerfed a little. In exchange,
Sacrifice now scales with _your_ damage stat instead of the ghoul's. Ghouls don't carry your items, so the original
explosion fell off hard late in a run. It still kinda does, but at least you can _technically_ scale it. Now it deals 800%
of your damage over a slightly bigger radius, and anything that boosts your damage, (like Shaped Glass) boosts it too.

The tombstone crumbles after 30 seconds instead of sticking around forever, and its vengeful souls hit for 280% instead
of 350%.

Mortician himself is a touch less tanky, at 180 health instead of 200. Health gain per level was also reduced to match the
30% per level that every other survivor uses, which will make him noticeably less tanky later on. He's still a slow melee
survivor with no mobility, so I've kept his armor as is, but his health stats were way outside of normal survivor numbers
in the base mod.

### Lysate Cell

Despite how strong Mortician is, I was genuinely surprised to learn that Lysate Cell did not work on the tombstone at all.
This felt like a power gap that I could fill rather than one to reign in. Engineer gets an extra turret from a Lysate Cell, 
after all. So now Mortician gets an extra tombstone. With two up, a slain ghoul's vengeful soul goes to whichever tombstone
is nearest to where it died.

### Ancient Scepter

With [Standalone Ancient Scepter](https://thunderstore.io/package/amogus_lovers/StandaloneAncientScepter/) installed,
the Scepter upgrades Tombstone into **Restless Grave**: enemies that die near one of your tombstones rise as ghouls,
up to one every 3 seconds per tombstone.

![The Restless Grave skill tooltip](https://raw.githubusercontent.com/Delfofthebla/EvenMoreBalancedMortician/master/docs/screenshots/RestlessGrave.png)

A ring on the ground shows the raise radius. Each player can hide it with
_Visuals → Show Raise Radius_, which only affects their own screen.

The Scepter mod is not required. Without it, Tombstone works as usual.

### Elite aspects

In the base mod, every ghoul copies whatever is in your equipment slot, regardless of the item. I _suspect_ that this was
meant to propogate passives like Elite Aspect Equipment, but it's hard to know for certain. The problem is that the default
implementation worked on things like Fuel Array as well (lol). BalancedMortician turned that off entirely, which was fair,
but I liked ghouls turning elite too much to lose it.

Now each new ghoul has a 33% chance to inherit each aspect you're carrying. That covers aspect equipment, and aspect items
from mods like [ZetAspects](https://thunderstore.io/package/William758/ZetAspects/), which roll once per copy you hold.
Nothing else is ever copied. If you don't play with ZetAspects, you may want to increase this, but follow your gut.

## Don't like my numbers?

Everything above is a setting, from base stats to cooldowns and proc coefficients, and you can change it in game with
[Risk Of Options](https://thunderstore.io/c/riskofrain2/p/Rune580/Risk_Of_Options/) or in the config file. Each
setting's description lists its value in every preset, and skill tooltips update to match.

If you'd rather not tune things yourself, there are two other presets to fall back on:

- **Original** keeps Mortician's own numbers.
- **BalancedMortician** matches Bloonjitsu7's mod.

Both keep the Lysate Cell and Ancient Scepter support, since neither original mod had any. Everything else, Sacrifice
scaling included, follows the respective source mod.

Changing any setting switches you to the Custom preset. While you stay on a preset, updating this mod moves you to that
preset's new values.

**NOTE: This mod's Ghoul Limit setting replaces the one in Mortician's own config, which will be _ignored_ while this mod is enabled.**

## Multiplayer

The host's settings apply to everyone in the lobby, and your own config comes back whenever you host or play solo.
Every player needs the same version of this mod.

## Credits

Icon: [Tombstone](https://game-icons.net/1x1/lorc/tombstone.html) by Lorc,
from [game-icons.net](https://game-icons.net), licensed under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).
