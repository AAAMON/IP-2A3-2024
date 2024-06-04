import random

projectile_weapons = ["Crysknife", "Maula Pistol", "Slip Tip", "Stunner"]
poison_weapons = ["Chaumas", "Chaumurky", "Ellaca Drug", "Gom Jabbar"]
special_weapons = ["Lasgun"]

projectile_defense = ["Shield"]
poison_defense = ["Snooper"]

weapon_cards = projectile_weapons + poison_weapons + special_weapons + [None]
defense_cards = projectile_defense + poison_defense + [None]

worthless_cards = ["Baliset", "Jubba Cloak", "Kulon", "La_la_la", "Trip to Gamont"]

faction_name = "Harkonnen"
nr_free_revive = 0
other_factions = ['Bene Gesserit', 'Atreides', 'Fremen', 'Spacing_Guild', 'Emperor']

generals = {
    "Atreides": ["Thufir Hawat", "Lady Jessica", "Gurney Halleck", "Duncan Idaho", "Dr. Wellington Yueh"],

    "Harkonnen": ["Feyd-Rautha", "Beast Rabban", "Piter De Vries", "Captain Iakin Nefud", "Umman Kudu"],

    "Bene_Gesserit": ["Alia", "Margot Lady Fenring", "Mother Ramalo", "Princess Irulan", "Wanna Yueh"],

    "Fremen": ["Stilgar", "Chani", "Otheym", "Shadout Mapes", "Jamis"],

    "Spacing_Guild": ["Staban Tuek", "Master Bewt", "Esmar Tuek", "Soo-Soo Sook", "Guild Rep."],

    "Emperor": ["Hasimir", "Fenring", "Captain Aramsham", "Caid", "Burseg", "Bashar"]
}

my_territory_id = 23

def pick_traitor(game_state):
    my_traitor= random.choice(generals["Harkonnen"])
    if random.randint(0,1)==1:
        return {"traitor_chosen" : my_traitor}
    else:
        return {'action': 'none'}


def aliance(game_state):
    my_ally = game_state["Alliances"]["Harkonnen"]
    good = ["Bene Gesserit", "Spacing Guild", "Fremen"]
    weak = ["Atreides", "Emperor"]

    break_alliance = "none"
    try_alliance = "none"

    for x in weak:
        if x in my_ally:
            break_alliance = x
            break

    for x in good:
        if x not in my_ally:
            try_alliance = x
            break

    return {"break_alliance": break_alliance,
            "try_alliance": try_alliance
            }


def bidding(game_state):
    last_bid = game_state['last_bid']['value']
    last_bid_player = game_state['last_bid']['player']

    my_cards = game_state['Faction_Knowledge']['Treachery_Cards']
    my_spice = game_state['Faction_Knowledge']['Spice']

    haveAtk = any(x in set(weapon_cards) for x in my_cards)
    haveDef = any(x in set(defense_cards) for x in my_cards)

    coef = 0.2

    if not haveDef:
        coef += 0.2

    if not haveAtk:
        coef += 0.2

    if my_spice > 8 and random.random() > 0.5:
        coef += 0.3

    if len(my_cards.keys()) >= 2:
        coef -= 0.1

    coef = min(coef, 0.8)
    maxBid = my_spice * coef

    if last_bid >= maxBid:
        return {'action': 'pass'}

    else:
        return {'action': 'bid', 'value': last_bid + 1}


def revival(game_state):
    my_spice = game_state['Faction_Knowledge']['Spice']
    dead_troops = game_state['Tleilaxu_Tanks']['Forces']['Harkonnen']

    if dead_troops >= 8:
        if my_spice >= 3:
            return {'troops_revived': 3}
    elif dead_troops >= 5:
        if my_spice >= 2:
            return {'troops_revived': 2}
    else:
        {'troops_revived': 0}

def storm_move(game_state, territory):
    #vedem daca furtuna loveste acel teritoriu
    storm_place = game_state['Storm_Sector']

    for i in range(1, 7):
        storm_place += 1
        if storm_place == 19:
            storm_place = 1
        for section in territory['Sections']:
            if storm_place == section['Origin_Sector']:
                return True
    return False


def get_territory_id_by_section_id(game_state, section_id):
    for territory in game_state['Map']['Territories']:
        for section in territory['Sections']:
            if section['Id'] == section_id:
                return territory['Id']


def get_forces_by_territory(game_state, territory, faction):
    forces = 0

    for section in territory['Sections']:
        if faction in section['Forces'].keys():
            forces += section['Forces'][faction]
    return forces


def evaluate_territory(game_state, territory):
    stronghold_territories = ['Arrakeen', 'Carthag', 'Sietch Tabr', 'Habbanya Sietch', 'Tuek\'s Sietch']
    my_forces = get_forces_by_territory(game_state, territory, "Harkonnen")
    my_reserves = game_state['Reserves'][faction_name]

    adjacent_opponents = sum(territory['Forces'][faction] for faction in other_factions)
    cnt_oponents = sum(1 for faction in other_factions if territory['Forces'][faction] > 0)
    my_spice = game_state['Faction_Knowledge']['Spice']

    if cnt_oponents == 2:
        return 0

    if territory['name'] in stronghold_territories and cnt_oponents == 0 and my_forces == 0:
        return 5  # Stronghold, unoccupied, highest priority

    if territory['name'] in stronghold_territories and cnt_oponents == 1 and simulate_battle(game_state, territory,
                                                                                             my_forces + min(
                                                                                                     my_reserves,
                                                                                                     my_spice) // 2) > 0.8:

        return 4  # Stronghold, occupied, but i can win

    #see if other factions are near the territory(dist <=2 so they can move here)

    if territory['name'] in stronghold_territories and my_forces <= 3:
        return 3  # Weakly defended, medium priority

    if game_state['Spice_Dict'][str(territory['Id'])]['Avaliable'] > 0 and not storm_move(game_state,
            territory) and cnt_oponents == 0 and my_forces == 0:  # maybe consider the case of battle
        return 2  # i bring them on the map free basically

    if cnt_oponents == 0:
        near_stronghold = False
        for strong_territory in game_state['Map']['Territories']:
            # i am near a stronghold
            if strong_territory['Name'] in stronghold_territories:
                for strong_section in strong_territory['Sections']:
                    for neighbor_section_id in strong_section['Neighboring_Sections_Ids']:
                        if get_territory_id_by_section_id(neighbor_section_id) == territory['Id']:
                            near_stronghold = True

        not_in_storm = True
        for section in territory['Sections']:
            if game_state['Storm_Sector'] == section['Origin_Sector']:
                not_in_storm = False


        # possibility of entering stronghold next round
        if near_stronghold and not_in_storm:
            return 1


    # Default: No special consideration
    return 0


def shipment(game_state):
    my_spice = game_state['Faction_Knowledge']['Spice']
    my_reserves = game_state['Reserves']["Harkonnen"]
    best_score = -1
    best_territory = None
    desired_number_troops = 0

    for territory in game_state['Map']['Territories']:
        curr_score = evaluate_territory(game_state, territory)
        if (curr_score > best_score):
            best_score = curr_score
            best_territory = territory

    if best_score == 5:
        desired_number_troops = min(my_spice // 2, my_reserves // 2)

    if best_score == 4:
        #see if my opponent is after me, if so he can bring more forces
        for number_troops in range(0, min(my_spice, my_reserves) // 2 + 1):
            if simulate_battle(game_state, territory, number_troops) > 0.8:
                desired_number_troops = number_troops
                break

    if best_score == 3:
        desired_number_troops = min(my_spice // 4, my_reserves // 4)

    if best_score == 2:
        desired_number_troops = min(min(2, my_spice), my_reserves)

    if best_score == 1:
        desired_number_troops = desired_number_troops = min(min(3, my_spice), my_reserves)


def movement(game_state):


    for territory in game_state['Map']['Territories']:
        if territory["Id"]==my_territory_id:
            my_forces=territory["Forces"]
            my_neighbours=territory["Neighboring_Sections_Ids"]

    territory_to_move="none"

    for territory in my_neighbours:
        if game_state["Map"]["Territories"][territory]["Forces"]<my_forces:
            territory_to_move=territory
            forces_to_move=game_state["Map"]["Territories"][territory]["Forces"]
            break

    if territory_to_move!="none":
        return {"move to territory": territory_to_move,
                "forces_to_move": forces_to_move}
    else:
        return {
            "action" : "don't move this round"
        }


def simulate_battle(game_state, territory, my_forces):
    best_chance_win = 0
    enemy = None
    enemy_forces = 0

    for faction in other_factions:
        if get_forces_by_territory(game_state, territory, faction) > 0:
            enemy = faction
            enemy_forces = get_forces_by_territory(game_state, territory, faction)

    for general in generals[faction_name]:
        if general in game_state['Tleilaxu_Tanks']['Non_Revivable_Generals'][faction_name]:
            continue

        if general in game_state['Tleilaxu_Tanks']['Revivable_Generals'][faction_name]:
            continue

        my_general_traitor_chance = 1 / 5
        if general in game_state['Faction_Knowledge']['Discarded_Traitors'] or general in \
                game_state['Faction_Knowledge']['Traitors']:
            my_general_traitor_chance = 0

        other_general_traitor_chance = 1 / 5

        my_available_defense = list(set(defense_cards) & set(game_state['Faction_Knowledge'['Treachery_Cards']]))
        my_available_defense.append(None)
        my_available_attack = list(set(weapon_cards) & set(game_state['Faction_Knowledge'['Treachery_Cards']]))
        my_available_attack.append(None)

        for defense in my_available_defense:
            for attack in my_available_attack:
                for forces in range(0, my_forces):
                    count_win = 0
                    count_total = 0
                    for other_general in generals[enemy]:
                        for i in range(0, 10):
                            count_total += 1
                            other_defense = random.choice(defense_cards)
                            other_attack = random.choice(weapon_cards)
                            other_forces = random.randint(0, enemy_forces)
                            zar_my_traitor = random.randint(1, 5)
                            zar_other_traitor = random.randint(1, 5)

                            if zar_my_traitor <= my_general_traitor_chance * 5:
                                continue

                            if zar_other_traitor <= 1:
                                count_win += 1
                                continue

                            my_general_alive = True

                            if other_attack in projectile_weapons and defense not in projectile_defense:
                                my_general_alive = False

                            if other_attack in poison_weapons and defense not in poison_defense:
                                my_general_alive = False

                            other_general_alive = True

                            if attack in projectile_weapons and other_defense not in projectile_defense:
                                other_general_alive = False

                            if attack in poison_weapons and other_defense not in poison_defense:
                                other_general_alive = False

                            my_power = forces
                            if my_general_alive:
                                my_power += 6  #

                            other_power = other_forces
                            if other_general_alive:
                                other_power += 6  #

                            if my_power > other_power:
                                count_win += 1
                    win_chance = count_win / count_total
                    if win_chance > best_chance_win:
                        best_chance_win = win_chance

    return best_chance_win


def battle(game_state, territory):
    my_forces = get_forces_by_territory(game_state, territory, faction_name)
    chance_win = simulate_battle(game_state, territory, my_forces)

    if chance_win > 0.8:
        pass

    elif chance_win > 0.5:
        dead_generals = list(set(game_state['Tleilaxu_Tanks']['Non_Revivable_Generals'][faction_name] | set(
            game_state['Tleilaxu_Tanks']['Revivable_Generals'][faction_name])))
        best_general = None
        for general in generals[faction_name]:
            if general not in dead_generals:
                best_general = general
                break

        attack_card = None
        available_attack_cards = list(set(weapon_cards) & set(game_state['Faction_Knowledge']['Treachery_Cards']))
        if len(available_attack_cards) > 0:
            attack_card = random.choice(available_attack_cards)

        defense_card = None
        available_defense_cards = list(set(defense_cards) & set(game_state['Faction_Knowledge']['Treachery_Cards']))
        if len(available_defense_cards) > 0:
            defense_card = random.choice(available_defense_cards)
        return {'forces': my_forces - 1, 'general': best_general, 'attack_card': attack_card,
                'defense_card': defense_card}

    else:
        #
        dead_generals = list(set(game_state['Tleilaxu_Tanks']['Non_Revivable_Generals'][faction_name] | set(
            game_state['Tleilaxu_Tanks']['Revivable_Generals'][faction_name])))
        # or choose cheapHero
        best_general = None

        for general in generals[faction_name]:
            if general not in dead_generals:
                best_general = general


def get_move(game_state):
    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}

    phase_name = game_state['Phase']['name']

    if phase_name == 'Pick Traitor':
        return pick_traitor(game_state)

    if phase_name == 'Bidding':
        return bidding(game_state)

    if phase_name == 'Revival':
        return revival(game_state)

    if phase_name == 'Shipment':
        return shipment(game_state)

    if phase_name == 'Movement':
        return movement(game_state)

    if phase_name == 'Nexus':
        return aliance(game_state)

    return {'status': 'phase unknown'}