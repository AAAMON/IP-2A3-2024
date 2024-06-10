import random
import json

projectile_weapons = ["Crysknife", "Maula Pistol", "Slip Tip", "Stunner"]
poison_weapons = ["Chaumas", "Chaumurky", "Ellaca Drug", "Gom Jabbar"]
special_weapons = ["Lasgun"]
special_treachery = ["Tleilaxu_Ghola"]
projectile_defense = ["Shield"]
poison_defense = ["Snooper"]
stronghold_territories = ['Arrakeen', 'Carthag', 'Sietch Tabr', 'Habbanya Sietch', 'Tuek\'s Sietch']

weapon_cards = projectile_weapons + poison_weapons + special_weapons + [None]
defense_cards = projectile_defense + poison_defense + [None]

worthless_cards =  ["Baliset", "Jubba Cloak", "Kulon", "La_la_la", "Trip to Gamont"]


#faction info
faction_name = "Emperor"
nr_free_revive = 1
other_factions = ['Bene Gesserit', 'Atreides', 'Harkonnen', 'Fremen', 'Spacing_Guild']


generals = {
    "Atreides": ["Thufir Hawat", "Lady Jessica", "Gurney Halleck", "Duncan Idaho", "Dr. Wellington Yueh"],
    "Harkonnen": ["Feyd-Rautha", "Beast Rabban", "Piter De Vries", "Captain Iakin Nefud", "Umman Kudu"],
    "Bene_Gesserit": ["Alia", "Margot Lady Fenring", "Mother Ramalo", "Princess Irulan", "Wanna Yueh"],
    "Fremen": ["Stilgar", "Chani", "Otheym", "Shadout Mapes", "Jamis"],
    "Spacing_Guild": ["Staban Tuek", "Master Bewt", "Esmar Tuek", "Soo-Soo Sook", "Guild Rep."],
    "Emperor": ["Hasimir Fenring", "Captain Aramsham", "Caid", "Burseg", "Bashar"]
}

generals_power = {
    "Atreides": {
        "Thufir Hawat": 5,
        "Lady Jessica": 5,
        "Gurney Halleck": 4,
        "Duncan Idaho": 2,
        "Dr. Wellington Yueh": 1
    },
    "Harkonnen": {
        "Feyd-Rautha": 6,
        "Beast Rabban": 4,
        "Piter De Vries": 3,
        "Captain Iakin Nefud": 2,
        "Umman Kudu": 1
    },
    "Bene_Gesserit": {
        "Alia": 5,
        "Margot Lady Fenring": 5,
        "Mother Ramallo": 5,
        "Princess Irulan": 5,
        "Wanna Yueh": 5
    },
    "Fremen": {
        "Stilgar": 7,
        "Chani": 6,
        "Otheym": 5,
        "Shadout Mapes": 3,
        "Jamis": 2
    },
    "Spacing_Guild": {
        "Staban Tuek": 5,
        "Master Bewt": 3,
        "Esmar Tuek": 3,
        "Soo-Soo Sook": 2,
        "Guild Rep.": 1
    },
    "Emperor": {
        "Hasimir Fenring": 6,
        "Captain Aramsham": 5,
        "Caid": 3,
        "Burseg": 3,
        "Bashar": 2
    }
}
def pick_storm(game_state):

 for i in range(3,0,-1):
        hit = False
        for territory in game_state['Map']['Territories']:
            my_forces = get_forces_by_territory(game_state, territory, faction_name)
            if my_forces > 0 and storm_move(game_state, territory, i+3):
                hit = True
        if not hit:
            return{
                "action": "Storm",
                "value": i
            }

 return{
            "action": "Storm",
            "value": 1
        }
    
def sort_generals(game_state):
    # all generals excluding those who are mine
    all_generals = {faction: generals_power[faction] for faction in generals_power if faction != "Emperor"}
    # sort by strength
    sorted_generals = sorted(
    [(name, strength) for faction in all_generals for name, strength in all_generals[faction].items()],
    key=lambda x: x[1],
    reverse=True)
    return sorted_generals


def pick_traitor(game_state):
    leaders_desc=sort_generals(game_state)
    possible_traitors=game_state["Faction_Knowledge"][0]["Traitors"]
    for traitor in leaders_desc:
        if any(traitor_name for traitor_name in possible_traitors if traitor_name["Name"] == traitor[0]):
            return {'action': traitor[0]}
        
    return {'action':game_state["Faction_Knowledge"][0]["Traitors"][0]["Name"]}

def alliance(game_state):
    request_aliance:"Spacing_Guild"
    strong_alliances = ["Spacing_Guild", "Harkonnen", "Atreides"]
    if request_aliance in strong_alliances:
        return {"action":"yes"}
    return {'action' : 'no'}


def pick_storm(game_state):
    for i in range(3,0,-1):
        hit = False
        for territory in game_state['Map']['Territories']:
            my_forces = get_forces_by_territory(game_state, territory, faction_name)
            if my_forces > 0 and storm_move(game_state, territory, i+3):
                hit = True
        if not hit:
            return{
                "action": "Storm",
                "value": i
            }

    return{
                "action": "Storm",
                "value": 1
            }
    
    

def bidding(game_state):

    

    last_bid = game_state['HighestBid']['Value']
    last_bid_player = game_state['HighestBid']['Player']
    my_cards = game_state['Faction_Knowledge'][0]['Treachery_Cards']
    my_spice = game_state['Faction_Knowledge'][0]['Spice']

    haveAtk = any(x in set(weapon_cards) for x in my_cards)
    haveDef = any(x in set(defense_cards) for x in my_cards)

    atreides_aggressive = last_bid_player == "Atreides" and last_bid > 0
    harkonnen_aggressive = last_bid_player == "Harkonnen" and last_bid > 0


    coef = 0.2

    if not haveDef:
        coef += 0.25

    if not haveAtk:
        coef += 0.25

    if my_spice > 8 and random.random() > 0.5:
        coef += 0.2

    if len (my_cards.keys()) >= 2:
        coef -= 0.2
    if atreides_aggressive:
        coef += 0.1
    if harkonnen_aggressive:
        coef += 0.2
  
    coef = min(coef, 0.7)
    maxBid = my_spice * coef

    if last_bid >= maxBid:
        return {'action': 'pass'}
    elif last_bid >= maxBid and len(my_cards)==0:
                return {'action': 'bid', 'value': last_bid+1}
    else:
        return {'action': 'bid', 'value': last_bid+1}
    


def revival(game_state):
    min_spice_for_leaders = 3
    max_forces_per_turn = 3
    my_spice = game_state['Faction_Knowledge'][0]['Spice']
    nr_dead = game_state['Tleilaxu_Tanks'][0]['Forces'][faction_name]
    possible_revival_generals = game_state['Tleilaxu_Tanks'][0]['Revivable_Generals'][faction_name]

    free_revives = min(nr_free_revive, nr_dead)
    remaining_revives = nr_dead - free_revives

    cost_per_force = 2  
    additional_revives = 0

    if my_spice > min_spice_for_leaders and len(possible_revival_generals)>0:
        available_spice_for_forces = my_spice - min_spice_for_leaders
        additional_revives = min(remaining_revives, available_spice_for_forces // cost_per_force,max_forces_per_turn - free_revives)
    total_revives = free_revives + additional_revives

    total_cost = additional_revives * cost_per_force
    my_spice -= total_cost

    all_my_generals = {faction: generals_power[faction] for faction in generals_power if faction == faction_name}
    sorted_generals = sorted(
    [(name, strength) for faction in all_my_generals for name, strength in all_my_generals[faction].items()],
    key=lambda x: x[1],
    reverse=True)

    generals_to_revive = []
    for general in sorted_generals:
        general_name = general[0]
        general_strength = general[1]
        if general_name in possible_revival_generals:
            if my_spice >= general_strength:
                generals_to_revive.append(general_name)
                my_spice -= general_strength
    
    #if "Tleilaxu_Ghola" in special_treachery:
    #    for general in sorted_generals:
    #       general_name = general[0]
    #        general_strength = general[1]
    #        if general_name in possible_revival_generals and general_strength > 4:
    #            generals_to_revive.append(general_name)
    #            break
    
    return {
        "action": "revive",
        "forces": total_revives,
        "generals": generals_to_revive
    }




def storm_move(game_state, territory): 
    #storm hits over
    storm_place = game_state['Storm_Sector']
    for i in range(1,7):
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
    my_forces = get_forces_by_territory(game_state, territory, faction_name)
    my_reserves = game_state['Reserves'][0][faction_name]
    adjacent_opponents = sum(section['Forces'].get(faction, 0) for section in territory['Sections'] for faction in other_factions)
    cnt_oponents = sum(1 for section in territory['Sections'] for faction in other_factions if section['Forces'].get(faction, 0) > 0)
    my_spice = game_state['Faction_Knowledge'][0]['Spice']

    if cnt_oponents == 2:
        return 0

    if territory['Name'] in stronghold_territories and cnt_oponents == 0 and my_forces == 0:
        return 5  #Stronghold, unoccupied, highest priority
    
    if territory['Name'] in stronghold_territories and cnt_oponents == 1 and simulate_battle(game_state, territory, my_forces + min(my_reserves, my_spice) // 2)[0] > 0.8:
        return 4 #Stronghold, occupied, but i can win
    
    # Todo: see if other factions are near the territory(dist <=2 so they can move here)
    
    if territory['Name'] in stronghold_territories and my_forces <= 3:
        return 3  # Weakly defended, medium priority
    
    if str(territory['Id']) in game_state['Map']['Spice_Dict'] and game_state['Map']['Spice_Dict'][str(territory['Id'])]['Avaliable'] > 0 and cnt_oponents == 0 and my_forces == 0:
        return 2 #i bring them on the map free basically
    
    if cnt_oponents == 0:
        near_stronghold = False
        for strong_territory in game_state['Map']['Territories']:
            #i am near a stronghold
            if strong_territory['Name'] in stronghold_territories:
                for strong_section in strong_territory['Sections']:
                    for neighbor_section_id in strong_section['Neighboring_Sections_Ids']:
                        if get_territory_id_by_section_id(neighbor_section_id) == territory['Id']:
                            near_stronghold = True

        #possibility of entering stronghold next round
        if near_stronghold and not storm_move(game_state,territory,0):
            return 1
                    
    # Default: No special consideration
    return 0

def shipment(game_state):
    #todo zic teritoriul, dar nu si zona
    my_spice = game_state['Faction_Knowledge'][0]['Spice']
    my_reserves = game_state['Reserves'][0][faction_name]
    best_score = -1
    best_territory = None
    desired_number_troops = 0
    for territory in game_state['Map']['Territories']:
        if not storm_move(game_state,territory,0):
            curr_score = evaluate_territory(game_state, territory)
            if(curr_score > best_score):
                best_score = curr_score
                best_territory = territory

    if best_score == 5:
        desired_number_troops =  min(my_spice // 2, my_reserves // 2)

    if best_score == 4:
        #todo to see if my opponent is after me, if so he can bring more forces
        for number_troops in  range(0, min(my_spice,my_reserves)//2 + 1):
            if simulate_battle(game_state, territory, number_troops)[0] > 0.8:
                desired_number_troops =  number_troops
                break

    if best_score == 3:
        desired_number_troops =  min(my_spice // 4, my_reserves // 4)

    if best_score == 2:
        desired_number_troops = min(min(2, my_spice),my_reserves)
        
    if best_score == 1:
        desired_number_troops = desired_number_troops = min(min(3, my_spice),my_reserves)

    best_section = random.choice(best_territory['Sections'])

    return {
    "action": "shipment", 
    "value": desired_number_troops, 
    "teritorry": best_territory['Id'], 
    "section": best_section['Id'] #TODO la matei chiriac
    } 


def movement(game_state):
    best_score = 0
    base_territory_id = 0
    best_territory_id = 0
    best_section_id = -1
    best_forces = 0
    for territory in game_state['Map']['Territories']:
        if territory['Name'] in stronghold_territories:
            continue
        if str(territory['Id']) in game_state['Map']['Spice_Dict'] and game_state['Map']['Spice_Dict'][str(territory['Id'])]['Avaliable'] > 0:
            continue

        if storm_move(game_state,territory,0):
            continue
        my_forces = get_forces_by_territory(game_state, territory, faction_name)
        if my_forces == 0:
            continue

        for section in territory['Sections']:
            for neighbor_section_id in section['Neighboring_Sections_Ids']:
                adj_id = get_territory_id_by_section_id(game_state, neighbor_section_id)
                if adj_id == territory['Id']:
                    continue

                for adj_territoy in game_state['Map']['Territories']:
                    if storm_move(game_state,adj_territoy,0):
                        continue
                    if adj_territoy['Id'] == adj_id:
                        score = evaluate_territory(game_state, adj_territoy)
                        if score > best_score:
                            best_score = score
                            best_territory_id = adj_territoy['Id']
                            best_section_id = neighbor_section_id
                            best_forces = my_forces
                            base_territory_id = territory['Id']

    return {
    "action": "movement", 
    "source_terittory": base_territory_id, 
    "destination_terittory": best_territory_id,
    "section": best_section_id,
    "value": best_forces
    }




def simulate_battle(game_state, territory, my_forces):
    best_chance_win = 0
    enemy = None
    enemy_forces = 0
    for faction in other_factions:
        if get_forces_by_territory(game_state,territory,faction) > 0:
            enemy = faction
            enemy_forces = get_forces_by_territory(game_state,territory,faction)

    for general in generals[faction_name]:
        if general in game_state['Tleilaxu_Tanks']['Non_Revivable_Generals'][faction_name]:
            continue

        if general in game_state['Tleilaxu_Tanks']['Revivable_Generals'][faction_name]:
            continue

        my_general_traitor_chance = 1/5
        if general in game_state['Faction_Knowledge']['Discarded_Traitors'] or general in game_state['Faction_Knowledge']['Traitors']:
            my_general_traitor_chance = 0
                    
        other_general_traitor_chance = 1/5

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
                        for i in range(0,10):
                            count_total += 1
                            other_defense = random.choice(defense_cards)
                            other_attack = random.choice(weapon_cards)
                            other_forces = random.randint(0, enemy_forces)
                            zar_my_traitor = random.randint(1,5)
                            zar_other_traitor = random.randint(1,5)
                            
                            if zar_my_traitor <= my_general_traitor_chance*5:
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
                                my_power += 6 #TODO

                            other_power = other_forces
                            if other_general_alive:
                                other_power += 6 #TODO

                            if my_power > other_power:
                                count_win += 1
                    win_chance = count_win/count_total
                    if win_chance > best_chance_win:
                        best_chance_win = win_chance

                    
    return best_chance_win
def get_best_general(game_state):
        dead_generals = set(game_state['Tleilaxu_Tanks'][0]['Non_Revivable_Generals'][faction_name]) | set(game_state['Tleilaxu_Tanks'][0]['Revivable_Generals'][faction_name])
        for general in generals[faction_name]:
            if general not in dead_generals:
                return general
        return None

def get_worst_general(game_state):
        dead_generals = set(game_state['Tleilaxu_Tanks'][0]['Non_Revivable_Generals'][faction_name]) | set(game_state['Tleilaxu_Tanks'][0]['Revivable_Generals'][faction_name])
        for general in reversed(generals[faction_name]):
            if general not in dead_generals:
                return general
        return None
    
def get_attack_card(game_state):
    available_attack_cards = list(set(weapon_cards) & set(game_state['Faction_Knowledge'][0]['Treachery_Cards']))
    if available_attack_cards:
        return random.choice(available_attack_cards)
    worthless_attack_cards = list(set(worthless_cards) & set(game_state['Faction_Knowledge'][0]['Treachery_Cards']))
    if worthless_attack_cards:
            card = random.choice(worthless_attack_cards)
            worthless_cards.remove(card)
            return card
    return None

def get_defense_card(game_state):
    available_defense_cards = list(set(defense_cards) & set(game_state['Faction_Knowledge'][0]['Treachery_Cards']))
    if available_defense_cards:
        return random.choice(available_defense_cards)
    worthless_defense_cards = list(set(worthless_cards) & set(game_state['Faction_Knowledge'][0]['Treachery_Cards']))
    if worthless_defense_cards:
            card = random.choice(worthless_defense_cards)
            worthless_cards.remove(card)
            return card
    return None

def get_territory_by_id(game_state, territory_id):
    for territory in game_state['Map']['Territories']:
        if territory['Id'] == territory_id:
            return territory
    return None

def battle(game_state, territory):
    my_forces = get_forces_by_territory(game_state,territory,faction_name)
    chance_win = simulate_battle(game_state, territory, my_forces)

    if chance_win > 0.8:
        return {
            'forces': max(my_forces - 1, 1),  # use all forces since chance is high
            'general': get_best_general(game_state),
            'attack_card': get_attack_card(game_state),
            'defense_card': get_defense_card(game_state)
        }

    elif chance_win > 0.5:
       return {
            'forces': max(my_forces - 1, 1),  
            'general': get_best_general(game_state),
            'attack_card': get_attack_card(game_state),
            'defense_card': get_defense_card(game_state)
        }
    else:
          worthless_attack_cards = list(set(worthless_cards) & set(game_state['Faction_Knowledge'][0]['Treachery_Cards']))
          atk_card=None
          if worthless_attack_cards:
            atk_card = random.choice(worthless_attack_cards)
            worthless_cards.remove(atk_card)
       
          worthless_defense_cards = list(set(worthless_cards) & set(game_state['Faction_Knowledge'][0]['Treachery_Cards']))
          defense_card=None
          if worthless_defense_cards:
            defense_card = random.choice(worthless_defense_cards)
            worthless_cards.remove(defense_card)

          return {
            'forces': max(my_forces - 1, 1),  
            'general': get_worst_general(game_state),
            'attack_card': atk_card,
            'defense_card': defense_card
        }
    

def get_move(game_state):
    
    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}
    
    phase_name = game_state['Phase'][0]['name']

    if phase_name == 'Pick Traitor':
        return pick_traitor(game_state)
   
    if phase_name == 'Storm':
        return pick_storm(game_state)

    if phase_name == 'Bidding':
        return bidding(game_state)
    
    if phase_name == 'Revival':
        return revival(game_state)
    
    if phase_name == 'Shipment':
        return shipment(game_state)
    
    if phase_name == 'Movement':
        return movement(game_state)
    
    
    if phase_name == 'Battle':
        territory_id=11 #trebuie dat din perspectiva!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        territory=get_territory_by_id(game_state,territory_id)
        return battle(game_state,territory)

    return {'status': 'phase unknown'}