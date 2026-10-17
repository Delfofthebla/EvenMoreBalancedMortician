#### 1.6.0

Requires R2API RecalculateStats.

**Added**
- New Config Setting "Ghoul Degen Growth Percent" (Raise Dead): a ghoul's degen grows the longer it lives. Healing still keeps ghouls alive longer, but no longer forever. 20% in the EvenMoreBalanced preset, 0% (unchanged) in the others.
- New Config Setting "Minimum Cooldown" (Raise Dead): the shortest Raise Dead's cooldown can get from cooldown reduction. Brainstalks would otherwise drop it to 0.5 seconds, the game's minimum for every skill. 2 seconds in the EvenMoreBalanced preset, 0.5 (unchanged) in the others.

**Changed** (EvenMoreBalancedMortician preset)
- Ghoul degen lowered from 10 (+3 per level) to 4 (+1.2 per level) to go with the new degen growth. An unhealed ghoul still lives about 15 seconds. I suspect this one might be a little divisive amongst those who like an immortal army, but hey, that's what configs are for.

**Fixed**
- A dying elite ghoul no longer counts as an elite kill for your on-kill items. Brainstalks, Defense Nucleus and the like could be triggered by your own ghouls, letting Brainstalks keep itself going with no enemies around.
- A dying elite ghoul's death effect (such as Mending's healing core) no longer happens twice.
- Ghouls no longer inherit ZetAspects' hidden items.

Players on 1.5.0 cannot join a lobby hosted on 1.6.0, or the reverse.

#### 1.5.0

Updated for the Hallowed Concepts update. Requires Mortician v0.1.6.

**Added**
- New Config Setting "Ghoul Raising Uses Cooldown Reduction" (Tombstone): the cooldown reduction items the tombstone copies from you when placed speed up its ghoul spawns and Restless Grave's raise cooldown. On in every preset.

**Removed**
- This mod's fix for ghoul and tombstone ownership after a revive or death. Mortician 0.1.6 fixes both in the base mod.

**Changed** (EvenMoreBalancedMortician preset)
- Tombstone duration removed. (I decided that I hated this)

**Fixed**
- The assembly version is now properly written to the mod's DLL. (I screwed up and it was not properly being set, which could cause the game to think everyone was on the same version even though they weren't. Likely not a big issue for most people, but it could have been)

Players on 1.4.0 cannot join a lobby hosted on 1.5.0, or the reverse.

#### 1.4.0

**Added**
- New Config Setting "Guarantees On-Kill Trigger" (Sacrifice): a sacrificed ghoul always triggers your on-kill items, skipping the On-Kill Trigger Chance roll. On in every preset. The passive's tooltip mentions it while the chance is below 100%.

~~Players on 1.3.0 cannot join a lobby hosted on 1.4.0, or the reverse.~~ (He lied, lyingly)

#### 1.3.0

**Added**
- New Config Setting "On-Kill Trigger Chance Percent": the chance for a dying ghoul to trigger your on-kill items. Mortician's passive triggers them on every ghoul death, including sacrificed and decayed ghouls. 50% in the EvenMoreBalanced preset, 100% (unchanged) in the others. The passive's tooltip follows the setting.
- Luck (57 Leaf Clover, Purity) now applies to the on-kill trigger roll and to every aspect inheritance roll.

**Changed** (Original and BalancedMortician presets)

Neither original mod supported the Lysate Cell or the Ancient Scepter, so these presets now include this mod's support for both. Their balance numbers are unchanged.
- Lysate Cell Adds Tombstone: off → on
- Soul Recipient: Newest → Nearest
- Restless Grave: off → on
- Restless Grave Raise Radius: 25 → 30

**Fixed** (base mod bugs)
- After Mortician died, his tombstone raised ghouls with no owner and logged an error for each one. Ghouls raised while you're dead now belong to you.
- After a revive (such as Dio's Best Friend), your existing ghouls and tombstones stopped crediting you for their attacks, and your tombstone stopped collecting vengeful souls. They now properly reconnect to you when you're revived.

#### 1.2.0

**Added**
- New Feature/Config Setting "Lysate Cell Adds Tombstone": holding a Lysate Cell lets you keep 2 tombstones up at once, the same way it gives Engineer 1 additional turret. Defaults to on in the EvenMoreBalanced preset.
- Soul Recipient: which tombstone a slain ghoul's vengeful soul goes to while you have more than one up. The newest tombstone (as in the base mod), the one nearest the ghoul, or every one. Defaults to Nearest in the EvenMoreBalanced preset.
- New Feature/Config Setting "Detonation Scales With Mortician": a sacrificed ghoul's explosion scales with your damage stat instead of the ghoul's, so it benefits from items that raise your damage. Defaults to on in the EvenMoreBalanced preset.
- New Config Settings "Ghoul Base Health" and "Ghoul Health Per Level".
- Ancient Scepter support: with [StandaloneAncientScepter](https://thunderstore.io/package/amogus_lovers/StandaloneAncientScepter/) installed, the Scepter upgrades Tombstone into Restless Grave. Enemies slain near a tombstone rise as ghouls, at most once every 3 seconds per tombstone. On in the EvenMoreBalanced preset, off in the others.
- New Config Settings "Enabled", "Raise Radius", "Raise Cooldown" and "Risen Ghoul Limit Per Tombstone" for Restless Grave.
- Updating the mod now moves your settings to the new preset values if you're on a preset. Previously, an update kept your old values and switched you to Custom. This applies to this update too, so EvenMoreBalanced players get the changes below automatically.
- Tombstones show a ring marking the Restless Grave raise radius. New local Config Setting "Show Raise Radius" hides it on your own screen; the host's settings never override it.

**Changed** (EvenMoreBalanced preset)

Walked back a bunch of my prior nerfs in the preset after some more play with him. I was a bit too harsh on the survivability and damage reductions. He is a melee survivor with zero mobility, after all.
- Health / Per Level: 170 / 51 → 180 / 54
- Armor: 10 → 20
- Regen / Per Level: 1 / 0.2 → 2.5 / 0.5
- Shovel Damage: 280% → 340%
- Launch Damage: 360% → 400% (Mostly just to hit the breakpoint for items that require 400%)
- Aspect Inherit Chance: 25% → 33%
- Detonation Damage: 880% of the ghoul's damage → 800% of Mortician's damage (Should help it scale better late game)
- Heal: 10% → 15% (This nerf was silly of me)
- Soul Orb Explosion: 250% → 280%

**Fixed**
- 1.1.0 reported itself as 1.0.0, so players on those two versions were not kept out of each other's lobbies. Unlikely to have happened to anyone but has been fixed regardless.

~~Players on 1.1.0 or earlier cannot join a lobby hosted on 1.2.0, or the reverse, they'll need to update.~~ (He lied, lyingly)

#### 1.1.0

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

~~Players on 1.0.0 cannot join a lobby hosted on 1.1.0, or the reverse.~~ (He lied, lyingly)

#### 1.0.0

- Initial release.
