Add-Type -Assembly System.IO.Compression

$out = Join-Path ([Environment]::GetFolderPath('Desktop')) 'RedMist.dmtm'

# ── Monsters.json ─────────────────────────────────────────────────────────────
$monstersJson = @'
[
  {
    "Id": "b1000000-0000-0000-0000-000000000000",
    "Name": "Red Mist",
    "Size": "Medium",
    "Type": "Humanoid",
    "Subtype": "",
    "Alignment": "Chaotic Evil",
    "ArmorClass": 17,
    "HitPoints": 250,
    "HitDice": null,
    "Speed": "35 ft.",
    "DamageVulnerabilities": null,
    "DamageResistances": null,
    "DamageImmunities": null,
    "ConditionImmunities": null,
    "Senses": "darkvision 60 ft., passive Perception 15",
    "Languages": "Common",
    "ChallengeRating": 13,
    "ProficiencyBonus": 0
  }
]
'@

# ── Characteritics.json (typo matches importer) ───────────────────────────────
$characteristicsJson = @'
[
  {"Id":"b1000001-0000-0000-0000-000000000000","MonsterId":"b1000000-0000-0000-0000-000000000000","CharacteristicTypeId":"101e14dc-3839-43a0-8a1f-93c37ac68991","Score":18,"Save":7},
  {"Id":"b1000002-0000-0000-0000-000000000000","MonsterId":"b1000000-0000-0000-0000-000000000000","CharacteristicTypeId":"8cd29246-1cf6-4a2f-99b9-f12d65194367","Score":16,"Save":0},
  {"Id":"b1000003-0000-0000-0000-000000000000","MonsterId":"b1000000-0000-0000-0000-000000000000","CharacteristicTypeId":"d0143a99-0c0a-42bd-b03c-3b1ee942f2c1","Score":18,"Save":7},
  {"Id":"b1000004-0000-0000-0000-000000000000","MonsterId":"b1000000-0000-0000-0000-000000000000","CharacteristicTypeId":"399c5b0e-8ca9-40f7-8725-d32d91e05e27","Score":14,"Save":0},
  {"Id":"b1000005-0000-0000-0000-000000000000","MonsterId":"b1000000-0000-0000-0000-000000000000","CharacteristicTypeId":"2a992f03-0789-4000-a2d7-880e46f81c8a","Score":12,"Save":3},
  {"Id":"b1000006-0000-0000-0000-000000000000","MonsterId":"b1000000-0000-0000-0000-000000000000","CharacteristicTypeId":"2776a74b-7b1f-4e3d-8ba4-d9355b1ccab5","Score":16,"Save":0}
]
'@

# ── SpecialAbilities.json ─────────────────────────────────────────────────────
$specialAbilitiesJson = @'
[
  {
    "Id": "b1000010-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Open Wounds",
    "Description": "When Red Mist deals slashing or piercing damage with her Butcher Knife or Bite, the target must succeed on a DC 15 Constitution saving throw or gain 1 Bleed.\n• A creature with Bleed takes 3 (1d6) necrotic damage at the start of its turn for each stack of Bleed.\n• A creature can have up to 5 Bleed.\n• A creature, or an adjacent ally, can use an action to make a Wisdom (Medicine) check to remove Bleed: DC 12 – Remove 1 Stack / DC 16 – Remove 2 Stacks / DC 20 – Remove 3 Stacks.\n• Any magical healing also removes 1 Bleed per 4 hit points healed, but the healing is reduced by the same amount.",
    "SortOrder": 0
  },
  {
    "Id": "b1000011-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Blood in the Air",
    "Description": "Red Mist always knows the location of creatures within 30 feet that currently have at least 1 Bleed.",
    "SortOrder": 1
  },
  {
    "Id": "b1000012-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Red Mist Form",
    "Description": "When Red Mist is reduced to half of her hit points or fewer for the first time, she erupts into a swirling crimson haze. Each creature within 10 feet must make a DC 15 Constitution saving throw or become poisoned until the end of its next turn.\nUntil the end of the encounter, Red Mist gains these benefits:\n• She can move through creatures’ spaces as if they were difficult terrain.\n• Opportunity attacks against her are made with disadvantage.\n• Once on each of her turns, when she moves through a creature’s space, that creature takes Bleed Damage for each stack.\n• She gains +4 to AC.\n• She becomes lightly obscured while moving, as a shifting red vapor surrounds her.",
    "SortOrder": 2
  }
]
'@

# ── Skills.json (empty — resolved by SkillTypeName on import) ─────────────────
$skillsJson = '[]'

# ── ArmorClassEntries.json ────────────────────────────────────────────────────
$armorClassEntriesJson = @'
[
  {
    "Id": "b1000060-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Label": "Default",
    "Value": 17,
    "SortOrder": 0
  }
]
'@

# ── Speeds.json ───────────────────────────────────────────────────────────────
$speedsJson = @'
[
  {
    "Id": "b1000061-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "SpeedType": "",
    "Value": 35,
    "SortOrder": 0
  }
]
'@

# ── Senses.json ───────────────────────────────────────────────────────────────
$sensesJson = @'
[
  {
    "Id": "b1000062-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "SenseType": "Darkvision",
    "Value": 60,
    "SortOrder": 0
  }
]
'@

# ── DamageModifiers.json (none for Red Mist) ──────────────────────────────────
$damageModifiersJson = '[]'

# ── Actions.json ──────────────────────────────────────────────────────────────
# Note: Legendary actions preamble ("Red Mist can take 3 legendary actions...")
# is not supported by the stat block generator — add as a note if needed.
$actionsJson = @'
[
  {
    "Id": "b1000020-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Multiattack",
    "Description": "Red Mist makes two Butcher Knife attacks.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": false, "IsReaction": false, "SortOrder": 0
  },
  {
    "Id": "b1000021-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Butcher Knife",
    "Description": "Melee Weapon Attack: +9 to hit, reach 5 ft., one target. Hit: 8 (1d8 + 6) slashing damage.\nIf the target already has Bleed, Red Mist can choose one of the following effects:\n• Hamstring. The target’s speed is reduced by 10 ft. until the end of its next turn.\n• Open the Artery. The target gains 1 additional Bleed.\n• Twist the Knife. The target cannot take reactions until the start of its next turn.\nOn a Critical Hit: The creature must make a DC 15 Constitution saving throw or lose a limb.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": false, "IsReaction": false, "SortOrder": 1
  },
  {
    "Id": "b1000022-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Bite",
    "Description": "Melee Weapon Attack: +9 to hit, reach 5 ft., one target. Hit: 7 (1d6 + 4) piercing damage, and Red Mist regains 5 hit points for each Bleed on the target.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": false, "IsReaction": false, "SortOrder": 2
  },
  {
    "Id": "b1000023-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Butcher’s Sweep (Recharge 5–6)",
    "Description": "Red Mist sweeps her blade in a savage arc. Each creature of her choice within 5 feet must make a DC 15 Dexterity saving throw.\nOn a failure: 14 (3d8) slashing damage and gain 1 Bleed.\nOn a success: half damage and no Bleed.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": false, "IsReaction": false, "SortOrder": 3
  },
  {
    "Id": "b1000024-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Feast on the Fallen",
    "Description": "Red Mist devours a creature at 0 hit points within 5 feet. The creature suffers one failed death saving throw. Red Mist regains 20 hit points and gains advantage on her next attack roll before the end of her next turn.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": false, "IsReaction": false, "SortOrder": 4
  },
  {
    "Id": "b1000030-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Blood Step",
    "Description": "Red Mist teleports up to 15 feet to an unoccupied space she can see adjacent to a creature with Bleed.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": true, "IsReaction": false, "SortOrder": 0
  },
  {
    "Id": "b1000031-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Crimson Exhale (Recharge 4–6)",
    "Description": "Red Mist exhales a cloud of foul red vapor in a 15-foot cone. Each creature in that area must make a DC 15 Constitution saving throw.\nOn a failure: takes 7 (2d6) poison damage and cannot regain hit points until the start of Red Mist’s next turn.\nOn a success: takes half damage and no healing prevention.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": true, "IsReaction": false, "SortOrder": 1
  },
  {
    "Id": "b1000040-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Blood Rush",
    "Description": "When a creature within 30 feet drops to 0 hit points, Red Mist moves up to her speed toward it. Opportunity attacks against her have disadvantage.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": false, "IsReaction": true, "SortOrder": 0
  },
  {
    "Id": "b1000041-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Slip Into Vapor (Only in Red Mist Form)",
    "Description": "When Red Mist is hit by an attack, she reduces the damage by 1d10 + 3 and can move 10 feet without provoking opportunity attacks.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": false, "IsBonus": false, "IsReaction": true, "SortOrder": 1
  },
  {
    "Id": "b1000050-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Knife Flick",
    "Description": "Red Mist makes one Bite attack, or one Butcher Knife attack against a creature within reach.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": true, "IsBonus": false, "IsReaction": false, "SortOrder": 0
  },
  {
    "Id": "b1000051-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Blood Step",
    "Description": "Red Mist uses Blood Step.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": true, "IsBonus": false, "IsReaction": false, "SortOrder": 1
  },
  {
    "Id": "b1000052-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Rend the Wounded (Costs 2 Actions)",
    "Description": "Red Mist targets one creature she can see within 30 feet that has Bleed. The target takes 6 (1d12) necrotic damage per Bleed stack, then loses 1 Bleed.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": true, "IsBonus": false, "IsReaction": false, "SortOrder": 2
  },
  {
    "Id": "b1000053-0000-0000-0000-000000000000",
    "MonsterId": "b1000000-0000-0000-0000-000000000000",
    "Name": "Become the Red Mist (Costs 3 Actions)",
    "Description": "Red Mist dissolves into crimson vapor and reforms in an unoccupied space she can see within 40 feet. Each creature she passes through during this movement must succeed on a DC 15 Constitution saving throw or take 7 (2d6) necrotic damage and gain 1 Bleed.",
    "Range": null, "DamageDice": null, "DamageBonus": 0, "DamageType": null,
    "AttackBonus": 0, "AttackAbility": null, "OverrideAttackBonus": false,
    "IsLegendary": true, "IsBonus": false, "IsReaction": false, "SortOrder": 3
  }
]
'@

# ── Build ZIP ─────────────────────────────────────────────────────────────────
$ms      = [System.IO.MemoryStream]::new()
$archive = [System.IO.Compression.ZipArchive]::new($ms, [System.IO.Compression.ZipArchiveMode]::Create, $true)

function Write-Entry($zip, $name, $text) {
    $entry  = $zip.CreateEntry($name)
    $stream = $entry.Open()
    $bytes  = [System.Text.Encoding]::UTF8.GetBytes($text)
    $stream.Write($bytes, 0, $bytes.Length)
    $stream.Close()
}

Write-Entry $archive 'Monsters.json'          $monstersJson
Write-Entry $archive 'Characteritics.json'    $characteristicsJson
Write-Entry $archive 'SpecialAbilities.json'  $specialAbilitiesJson
Write-Entry $archive 'Skills.json'            $skillsJson
Write-Entry $archive 'Actions.json'           $actionsJson
Write-Entry $archive 'ArmorClassEntries.json' $armorClassEntriesJson
Write-Entry $archive 'Speeds.json'            $speedsJson
Write-Entry $archive 'Senses.json'            $sensesJson
Write-Entry $archive 'DamageModifiers.json'   $damageModifiersJson

$archive.Dispose()
[System.IO.File]::WriteAllBytes($out, $ms.ToArray())
$ms.Dispose()

Write-Host "Created: $out"
