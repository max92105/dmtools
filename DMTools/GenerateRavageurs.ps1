Add-Type -Assembly System.IO.Compression

$out = Join-Path ([Environment]::GetFolderPath('Desktop')) 'Ravageurs.dmtm'

# ── GUIDs ────────────────────────────────────────────────────────────────────
$M1 = 'a0000001-0000-0000-0000-000000000000'  # Profanateur
$M2 = 'a0000002-0000-0000-0000-000000000000'  # Occultiste
$M3 = 'a0000003-0000-0000-0000-000000000000'  # Bastion
$M4 = 'a0000004-0000-0000-0000-000000000000'  # Arbaletrier

$STR = '101e14dc-3839-43a0-8a1f-93c37ac68991'
$DEX = '8cd29246-1cf6-4a2f-99b9-f12d65194367'
$CON = 'd0143a99-0c0a-42bd-b03c-3b1ee942f2c1'
$INT = '399c5b0e-8ca9-40f7-8725-d32d91e05e27'
$WIS = '2a992f03-0789-4000-a2d7-880e46f81c8a'
$CHA = '2776a74b-7b1f-4e3d-8ba4-d9355b1ccab5'

# ── Monsters.json ────────────────────────────────────────────────────────────
$monstersJson = @'
[
  {
    "Id": "a0000001-0000-0000-0000-000000000000",
    "Name": "Ravageur Profanateur",
    "Size": "Medium",
    "Type": "Humanoid",
    "Subtype": "Ravageur",
    "Alignment": "Evil",
    "ArmorClass": 17,
    "HitPoints": 95,
    "HitDice": "10d8 + 50",
    "Speed": "30 ft.",
    "DamageVulnerabilities": null,
    "DamageResistances": null,
    "DamageImmunities": null,
    "ConditionImmunities": null,
    "Senses": "passive Perception 16",
    "Languages": "Common",
    "ChallengeRating": 5.0,
    "ProficiencyBonus": 0
  },
  {
    "Id": "a0000002-0000-0000-0000-000000000000",
    "Name": "Ravageur Occultiste",
    "Size": "Medium",
    "Type": "Humanoid",
    "Subtype": "Ravageur",
    "Alignment": "Evil",
    "ArmorClass": 15,
    "HitPoints": 85,
    "HitDice": "10d8 + 30",
    "Speed": "30 ft.",
    "DamageVulnerabilities": null,
    "DamageResistances": null,
    "DamageImmunities": null,
    "ConditionImmunities": null,
    "Senses": "passive Perception 15",
    "Languages": "Common",
    "ChallengeRating": 5.0,
    "ProficiencyBonus": 0
  },
  {
    "Id": "a0000003-0000-0000-0000-000000000000",
    "Name": "Ravageur Bastion",
    "Size": "Medium",
    "Type": "Humanoid",
    "Subtype": "Ravageur",
    "Alignment": "Evil",
    "ArmorClass": 18,
    "HitPoints": 130,
    "HitDice": "12d8 + 72",
    "Speed": "25 ft.",
    "DamageVulnerabilities": null,
    "DamageResistances": null,
    "DamageImmunities": null,
    "ConditionImmunities": null,
    "Senses": "passive Perception 15",
    "Languages": "Common",
    "ChallengeRating": 6.0,
    "ProficiencyBonus": 0
  },
  {
    "Id": "a0000004-0000-0000-0000-000000000000",
    "Name": "Ravageur Arbalétrier",
    "Size": "Medium",
    "Type": "Humanoid",
    "Subtype": "Ravageur",
    "Alignment": "Evil",
    "ArmorClass": 15,
    "HitPoints": 80,
    "HitDice": "10d8 + 30",
    "Speed": "30 ft.",
    "DamageVulnerabilities": null,
    "DamageResistances": null,
    "DamageImmunities": null,
    "ConditionImmunities": null,
    "Senses": "passive Perception 14",
    "Languages": "Common",
    "ChallengeRating": 5.0,
    "ProficiencyBonus": 0
  }
]
'@

# ── Characteritics.json ──────────────────────────────────────────────────────
$characteristicsJson = @'
[
  {"Id":"b1000001-0001-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","CharacteristicTypeId":"101e14dc-3839-43a0-8a1f-93c37ac68991","Score":14,"Save":0},
  {"Id":"b1000001-0002-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","CharacteristicTypeId":"8cd29246-1cf6-4a2f-99b9-f12d65194367","Score":10,"Save":0},
  {"Id":"b1000001-0003-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","CharacteristicTypeId":"d0143a99-0c0a-42bd-b03c-3b1ee942f2c1","Score":18,"Save":7},
  {"Id":"b1000001-0004-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","CharacteristicTypeId":"399c5b0e-8ca9-40f7-8725-d32d91e05e27","Score":10,"Save":0},
  {"Id":"b1000001-0005-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","CharacteristicTypeId":"2a992f03-0789-4000-a2d7-880e46f81c8a","Score":16,"Save":6},
  {"Id":"b1000001-0006-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","CharacteristicTypeId":"2776a74b-7b1f-4e3d-8ba4-d9355b1ccab5","Score":12,"Save":0},

  {"Id":"b2000002-0001-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","CharacteristicTypeId":"101e14dc-3839-43a0-8a1f-93c37ac68991","Score":10,"Save":0},
  {"Id":"b2000002-0002-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","CharacteristicTypeId":"8cd29246-1cf6-4a2f-99b9-f12d65194367","Score":12,"Save":0},
  {"Id":"b2000002-0003-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","CharacteristicTypeId":"d0143a99-0c0a-42bd-b03c-3b1ee942f2c1","Score":16,"Save":6},
  {"Id":"b2000002-0004-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","CharacteristicTypeId":"399c5b0e-8ca9-40f7-8725-d32d91e05e27","Score":12,"Save":0},
  {"Id":"b2000002-0005-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","CharacteristicTypeId":"2a992f03-0789-4000-a2d7-880e46f81c8a","Score":14,"Save":5},
  {"Id":"b2000002-0006-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","CharacteristicTypeId":"2776a74b-7b1f-4e3d-8ba4-d9355b1ccab5","Score":16,"Save":0},

  {"Id":"b3000003-0001-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","CharacteristicTypeId":"101e14dc-3839-43a0-8a1f-93c37ac68991","Score":18,"Save":0},
  {"Id":"b3000003-0002-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","CharacteristicTypeId":"8cd29246-1cf6-4a2f-99b9-f12d65194367","Score":8,"Save":0},
  {"Id":"b3000003-0003-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","CharacteristicTypeId":"d0143a99-0c0a-42bd-b03c-3b1ee942f2c1","Score":20,"Save":8},
  {"Id":"b3000003-0004-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","CharacteristicTypeId":"399c5b0e-8ca9-40f7-8725-d32d91e05e27","Score":8,"Save":0},
  {"Id":"b3000003-0005-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","CharacteristicTypeId":"2a992f03-0789-4000-a2d7-880e46f81c8a","Score":14,"Save":5},
  {"Id":"b3000003-0006-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","CharacteristicTypeId":"2776a74b-7b1f-4e3d-8ba4-d9355b1ccab5","Score":10,"Save":0},

  {"Id":"b4000004-0001-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","CharacteristicTypeId":"101e14dc-3839-43a0-8a1f-93c37ac68991","Score":12,"Save":0},
  {"Id":"b4000004-0002-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","CharacteristicTypeId":"8cd29246-1cf6-4a2f-99b9-f12d65194367","Score":16,"Save":6},
  {"Id":"b4000004-0003-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","CharacteristicTypeId":"d0143a99-0c0a-42bd-b03c-3b1ee942f2c1","Score":14,"Save":0},
  {"Id":"b4000004-0004-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","CharacteristicTypeId":"399c5b0e-8ca9-40f7-8725-d32d91e05e27","Score":10,"Save":0},
  {"Id":"b4000004-0005-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","CharacteristicTypeId":"2a992f03-0789-4000-a2d7-880e46f81c8a","Score":12,"Save":0},
  {"Id":"b4000004-0006-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","CharacteristicTypeId":"2776a74b-7b1f-4e3d-8ba4-d9355b1ccab5","Score":10,"Save":0}
]
'@

# ── SpecialAbilities.json ────────────────────────────────────────────────────
$specialAbilitiesJson = @'
[
  {"Id":"c1000001-0001-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Dark Devotion","Description":"Advantage on saves vs charmed and frightened.","AttackBonus":0,"SortOrder":0},
  {"Id":"c1000001-0002-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Aura of Rot (Passive)","Description":"Créatures ennemies à 10 ft :\nNe peuvent regagner que la moitié des HP","AttackBonus":0,"SortOrder":1},

  {"Id":"c2000002-0001-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","Name":"Dark Devotion","Description":"Advantage on saves vs charmed and frightened.","AttackBonus":0,"SortOrder":0},

  {"Id":"c3000003-0001-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","Name":"Dark Devotion","Description":"Advantage on saves vs charmed and frightened.","AttackBonus":0,"SortOrder":0},
  {"Id":"c3000003-0002-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","Name":"Grave Bulk","Description":"Le Ravageur a résistance aux dégâts non magiques\net ne peut pas être poussé ou renversé.","AttackBonus":0,"SortOrder":1},

  {"Id":"c4000004-0001-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Dark Devotion","Description":"Advantage on saves vs charmed and frightened.","AttackBonus":0,"SortOrder":0},
  {"Id":"c4000004-0002-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Blood Tracker","Description":"Le Ravageur a avantage sur ses attaques à distance contre toute créature blessée.","AttackBonus":0,"SortOrder":1},
  {"Id":"c4000004-0003-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Carrion Sight","Description":"Ignore demi-couvert et trois-quarts de couvert contre les cibles blessées.","AttackBonus":0,"SortOrder":2}
]
'@

# ── Actions.json ─────────────────────────────────────────────────────────────
$actionsJson = @'
[
  {"Id":"d1000001-0001-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Mace","Description":"+6 to hit, 1d6 + 2 bludgeoning + 1d6 necrotic","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},
  {"Id":"d1000001-0002-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Profane Strike (Recharge 4–6)","Description":"+6 to hit\nHit: 3d8 + 3 necrotic, la cible ne peut pas regagner de HP jusqu'au début du prochain tour du Ravageur","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":1},
  {"Id":"d1000001-0003-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Condemn the Weak (Recharge 5–6)","Description":"WIS save DC 14 (30 ft) Stunned jusqu'à la fin de son prochain tour","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":2},
  {"Id":"d1000001-0004-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Wave of Consumption (Recharge 6)","Description":"Toutes les créatures ennemies à 15 ft :\n3d6 necrotic damage\nle Ravageur peux guérir les autre ravageur à porter égal au total infligé.","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":3},
  {"Id":"d1000001-0005-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Feast (1/turn, corpse à 5 ft)","Description":"• Heal 45 HP\n• recharge immédiatement Profane Strike","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":4},
  {"Id":"d1000001-0006-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Bite","Description":"+6 to hit, 1d6 + 2 piercing + 1d6 necrotic","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":true,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},
  {"Id":"d1000001-0007-0000-0000-000000000000","MonsterId":"a0000001-0000-0000-0000-000000000000","Name":"Blood Rush","Description":"Quand une créature tombe à 0 HP :\nmove complet sans OA\npeut faire Bite immédiatement à la fin du mouvement","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":true,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},

  {"Id":"d2000002-0001-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","Name":"Necrotic Bolt","Description":"Ranged Attack: +6 to hit, 60 ft\nHit: 2d10 necrotic damage","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},
  {"Id":"d2000002-0002-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","Name":"Grasp of the Grave (Recharge 4–6)","Description":"Une créature à 30 ft fait un CON save DC 14\n→ 3d8 necrotic damage\n→ et poison jusqu'à la fin de son prochain tour","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":1},
  {"Id":"d2000002-0003-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","Name":"Life Transfusion (Recharge 5-6)","Description":"Choisit une créature à 30 ft :\nOPTION 1 – Drain (offensif)\n• cible fait CON save DC 14\n• prend 4d8 necrotic\n• le Ravageur récupère la moitié\nOPTION 2 – Sacrifice (allié ou lui-même)\n• une créature volontaire perd 20 HP\n• une autre créature à 30 ft récupère 40 HP\nOPTION 3 – Corpse Channel\n• consomme un corps à 30 ft\n• heal 45 HP\n• Grasp of the Grave","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":2},
  {"Id":"d2000002-0004-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","Name":"Feast (1/turn, corpse à 5 ft)","Description":"• Heal 45 HP\n• recharge immédiatement Profane Strike","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":3},
  {"Id":"d2000002-0005-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","Name":"Blood Lash","Description":"+6 to hit, 1d6 + 3 necrotic DC 14 STR or Grappled","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":true,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},
  {"Id":"d2000002-0006-0000-0000-000000000000","MonsterId":"a0000002-0000-0000-0000-000000000000","Name":"Blood Rush","Description":"Quand une créature tombe à 0 HP :\nmove complet sans OA","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":true,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},

  {"Id":"d3000003-0001-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","Name":"Heavy Cleaver","Description":"+7 to hit, reach 5 ft\nHit: 1d10 + 4 slashing + 1d6 necrotic","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},
  {"Id":"d3000003-0002-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","Name":"Crushing Slam (Recharge 4–6)","Description":"Toutes les créatures à 5 ft :\n→ 2d8 + 4 bludgeoning\n→ STR save DC 15 ou prone","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":1},
  {"Id":"d3000003-0003-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","Name":"Mark of the Glutton (Recharge 5–6)","Description":"Une créature à 30 ft :\n→ WIS save DC 14\n→ désavantage pour attaquer une autre cible que le Ravageur\n→ si elle ignore → prend 2d8 necrotic","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":2},
  {"Id":"d3000003-0004-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","Name":"Feast (1/turn, corpse à 5 ft)","Description":"• Heal 35 HP\n• Jusqu'à la fin du prochain tour :\n→ résistance à TOUS les dégâts\n→ +1d8 dégâts","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":3},
  {"Id":"d3000003-0005-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","Name":"Devouring Grip","Description":"+7 to hit\n→ grapple (escape DC 15)\n→ cible prend 1d6 necrotic au début de chaque tour du Ravageur","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":true,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},
  {"Id":"d3000003-0006-0000-0000-000000000000","MonsterId":"a0000003-0000-0000-0000-000000000000","Name":"Intercept Flesh","Description":"Quand un allié à 5 ft est touché :\n→ le Ravageur prend les dégâts à sa place\n→ réduit les dégâts de 10","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":true,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},

  {"Id":"d4000004-0001-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Necrotic Crossbow","Description":"Ranged Weapon Attack: +6 to hit, range 80/320 ft\nHit: 1d10 + 3 piercing + 1d6 necrotic","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},
  {"Id":"d4000004-0002-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Rending Shot (Recharge 4–6)","Description":"+6 to hit\nHit: 2d10 + 3 necrotic\n→ la cible subit 1d6 necrotic au début de chacun de ses tours (3 tours)\n→ CON save DC 14 pour stopper l'effet","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":1},
  {"Id":"d4000004-0003-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Death Volley (Recharge 5–6)","Description":"Jusqu'à 3 cibles à 60 ft\n→ attaque séparée pour chaque cible\n→ dégâts : 1d8 + 3 necrotic","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":2},
  {"Id":"d4000004-0004-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Feast (1/turn, corpse à 5 ft)","Description":"• Heal 25 HP\n• Jusqu'à la fin du prochain tour :\n→ ses attaques infligent +1d8 necrotic\n→ ignore totalement le couvert","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":3},
  {"Id":"d4000004-0005-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Reposition","Description":"Se déplace de 15 ft sans provoquer d'opportunity attacks","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":true,"IsReaction":false,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0},
  {"Id":"d4000004-0006-0000-0000-000000000000","MonsterId":"a0000004-0000-0000-0000-000000000000","Name":"Opportunistic Shot","Description":"Quand une créature à 30 ft tombe à 0 HP :\n→ fait immédiatement une attaque à distance contre une autre cible","AttackBonus":0,"DamageDice":null,"DamageBonus":0,"IsLegendary":false,"IsBonus":false,"IsReaction":true,"Range":null,"DamageType":null,"AttackAbility":null,"OverrideAttackBonus":false,"SortOrder":0}
]
'@

# ── Build zip ─────────────────────────────────────────────────────────────────
function Add-ZipEntry([System.IO.Compression.ZipArchive]$archive, [string]$name, [string]$content) {
    $entry  = $archive.CreateEntry($name)
    $stream = $entry.Open()
    $writer = New-Object System.IO.StreamWriter($stream, [System.Text.Encoding]::UTF8)
    $writer.Write($content)
    $writer.Dispose()
    $stream.Dispose()
}

$ms      = New-Object System.IO.MemoryStream
$archive = New-Object System.IO.Compression.ZipArchive($ms, [System.IO.Compression.ZipArchiveMode]::Create, $true)

Add-ZipEntry $archive 'Monsters.json'          $monstersJson
Add-ZipEntry $archive 'Characteritics.json'    $characteristicsJson
Add-ZipEntry $archive 'SpecialAbilities.json'  $specialAbilitiesJson
Add-ZipEntry $archive 'Skills.json'            '[]'
Add-ZipEntry $archive 'Actions.json'           $actionsJson

$archive.Dispose()

[System.IO.File]::WriteAllBytes($out, $ms.ToArray())
$ms.Dispose()

Write-Host "Created: $out"
