#### 1.2.0

**Added**
- New Feature/Config Setting "Lysate Cell Adds Tombstone": holding a Lysate Cell lets you keep 2 tombstones up at once, the same way it gives Engineer 1 additional turret. Defaults to on in the EvenMoreBalanced preset.
- Soul Recipient: which tombstone a slain ghoul's vengeful soul goes to while you have more than one up. The newest tombstone (as in the base mod), the one nearest the ghoul, or every one. Defaults to Nearest in the EvenMoreBalanced preset.
- New Feature/Config Setting "Detonation Scales With Mortician": a sacrificed ghoul's explosion scales with your damage stat instead of the ghoul's, so it benefits from items that raise your damage. Defaults to on in the EvenMoreBalanced preset.
- New Config Settings "Ghoul Base Health" and "Ghoul Health Per Level".
- Ancient Scepter support: with [StandaloneAncientScepter](https://thunderstore.io/package/amogus_lovers/StandaloneAncientScepter/) installed, the Scepter upgrades Tombstone into Restless Grave. Enemies slain near a tombstone rise as ghouls, at most once every 3 seconds per tombstone. On in the EvenMoreBalanced preset, off in the others.
- New Config Settings "Enabled", "Raise Radius", "Raise Cooldown" and "Risen Ghoul Limit Per Tombstone" for Restless Grave.
- Tombstones show a ring marking the Restless Grave raise radius. New local Config Setting "Show Raise Radius" hides it on your own screen; the host's settings never override it.

**Changed** (EvenMoreBalanced preset)

Reverted a bunch of my prior nerfs in the preset after some more play with him. I was a bit too harsh on the survivability reduction.
- Armor: 10 → 20
- Regen / Per Level: 1 / 0.2 → 2.5 / 0.5
- Launch Damage: 360% → 400% (Mostly to hit the breakpoint for items that require 400%)
- Detonation Damage: 880% of the ghoul's damage → 800% of Mortician's damage (Should help it scale better late game)
- Heal: 10% → 15% (This nerf was silly of me)

**Fixed**
- 1.1.0 reported itself as 1.0.0, so players on those two versions were not kept out of each other's lobbies. Unlikely to have happened to anyone but has been fixed regardless.

Players on 1.1.0 or earlier cannot join a lobby hosted on 1.2.0, or the reverse, they'll need to update.

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

Players on 1.0.0 cannot join a lobby hosted on 1.1.0, or the reverse.

#### 1.0.0

- Initial release.
