import json
import random


projectile_weapons = ["Crysknife", "Maula Pistol", "Slip Tip", "Stunner"]
poison_weapons = ["Chaumas", "Chaumurky", "Ellaca Drug", "Gom Jabbar"]
special_weapons = ["Lasgun"]

projectile_defense = ["Shield"]
poison_defense = ["Snooper"]

weapon_cards = projectile_weapons + poison_weapons + special_weapons + [None]
defense_cards = projectile_defense + poison_defense + [None]

worthless_cards =  ["Baliset", "Jubba Cloak", "Kulon", "La_la_la", "Trip to Gamont"]


#faction info
faction_name = "Atreides"
nr_free_revive = 2
other_factions = ['Bene Gesserit',  'Harkonnen', 'Fremen', 'Spacing_Guild', 'Emperor']
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
############################################################################################################
def pick_storm(game_state):
    #todo sa vad daca furtuna se afla la dist de 5 de ex sa aleg maxim 1 (cred)
    player1=game_state['Battle_Wheels']['Item1']['_last_player'][0]["Id"]
    player2=game_state['Battle_Wheels']['Item1']['_last_player'][0]["Id"]
    if player1=="Atreides" or player2=="Atreides":
        random_number = random.randint(1, 3)
        return {"action " "storm_movement ": str(random_number)}
    return {"action" : "none"}
############################################################################################################
def sort_generals(game_state):
    # all generals excluding those who are mine
    all_generals = {faction: generals_power[faction] for faction in generals_power if faction != "Spacing_Guild"}
    # sort by strength
    sorted_generals = sorted(
    [(name, strength) for faction in all_generals for name, strength in all_generals[faction].items()],
    key=lambda x: x[1],
    reverse=True)
    return sorted_generals
############################################################################################################
def pick_traitor(game_state):
    leaders_desc=sort_generals(game_state)
    possible_traitors=game_state["Faction_Knowledge"][0]["Traitors"]
    for traitor in leaders_desc:
        if any(traitor_name for traitor_name in possible_traitors if traitor_name["Name"] == traitor[0]):
            return {'action': traitor[0]}
    #daca am cumva eroare il aleg pe primul
    return {'action':game_state["Faction_Knowledge"][0]["Traitors"][0]["Name"]}
############################################################################################################
def aliance(game_state):
    request_aliance="Emperor" #somewhere in the json not yet
    strong_aliances=["Fremen","Emperor","Harkonnen"]
    if request_aliance in strong_aliances:
        return {"action":"yes"}
    return {'action' : 'no'}
############################################################################################################
def bidding(game_state):
            my_spice=game_state['Faction_Knowledge'][0]['Spice']
            if len(game_state['Faction_Knowledge'][0]['Treachery_Cards']) >= 4:
                return {"action":"pass"}
            # Atreides can look at the first card before bidding
            first_card = game_state["Treachery_Cards"][0]
            current_hand = game_state['Faction_Knowledge'][0]['Treachery_Cards']
            need_card = False
            if my_spice<=last_bid+1:
                return {"action":"pass"}
            # Check if the first card is a weapon or defense we need
            if first_card in projectile_weapons and not any(card in projectile_weapons for card in current_hand):
                need_card = True
            elif first_card in poison_weapons and not any(card in poison_weapons for card in current_hand):
                need_card = True
            elif first_card in special_weapons and not any(card in special_weapons for card in current_hand):
                need_card = True
            elif first_card in projectile_defense and not any(card in projectile_defense for card in current_hand):
                need_card = True
            elif first_card in poison_defense and not any(card in poison_defense for card in current_hand):
                need_card = True

            if need_card:
                # Determine the bid amount based on available spice
                if my_spice > 10:
                    bid_amount = max(1, int(0.30 * my_spice))
                elif my_spice < 5:
                    bid_amount = 1
                else:
                    bid_amount = 2
                last_bid = game_state.get("Last_Bid", 0)
                my_bid = last_bid + bid_amount
                if(my_bid > my_spice):
                    my_bid = last_bid+1
                if game_state.get("Is_First_Bidder", False):
                    return {"action":"bid","value":1}
                else:    
                    return {"action":"bid","value":my_bid}
            else:
                     # Check if the Emperor is the next player and has fewer than 4 cards
                    player_positions = game_state["Player_Markers"][0][0]["Right"]["Player_Marker_Positions"]
                    current_position = player_positions.index(game_state["Player_Markers"][0][0]["Right"]["Faction_To_Marker"]["Atreides"])
                    next_position = (current_position + 1) % len(player_positions)
                    next_faction = game_state["Player_Markers"][0][0]["Right"]["Marker_To_Faction"][str(player_positions[next_position])]

                    if next_faction == "Emperor":
                        emperor_hand_size = game_state['Faction_Knowledge'][0]['Number_Of_Treachery_Cards_Of_Other_Factions']["Emperor"]
                        if emperor_hand_size < 4:
                            if last_bid+1 <=my_spice*0.3 and my_spice>=5 and last_bid+1<5:
                                my_bid = last_bid+1
                                return {"action":"bid","value":my_bid}
                            else:
                                return {"action":"pass"}
                        else:
                            return {"action":"bid","value":1}
                    else:
                            return {"action":"pass"}


def revival(game_state):

    # Retrieve relevant game state information
    my_spice = game_state['Faction_Knowledge'][0]['Spice']
    nr_dead = game_state['Tleilaxu_Tanks'][0]['Forces'].get(faction_name, 0)
    revivable_generals = game_state['Tleilaxu_Tanks'][0]['Revivable_Generals'].get(faction_name, [])
    traitors = game_state['Faction_Knowledge'][0]['Traitors']

    # Determine the number of forces to revive
    free_revives = 2
    if my_spice >=10:
        forces_to_revive = min(nr_dead, 3)
    else:
        forces_to_revive=min(2,nr_dead)
    all_my_generals = {faction: generals_power[faction] for faction in generals_power if faction == "Atreides"}
    sorted_generals = sorted(
    [(name, strength) for faction in all_my_generals for name, strength in all_my_generals[faction].items()],
    key=lambda x: x[1],
    reverse=True)
    max_spice_for_generals = my_spice - 2*(forces_to_revive-min(2,nr_dead))
    # Check for a revivable leader who is not a traitor
    possible_revival_generals = game_state['Tleilaxu_Tanks'][0]['Revivable_Generals'][faction_name]
    generals_to_revive = []
    for general in sorted_generals:
        general_name = general[0]
        general_strength = general[1]
        if general_name in possible_revival_generals:
            if max_spice_for_generals >= general_strength:
                generals_to_revive.append(general_name)
                max_spice_for_generals -= general_strength

    return {
        "action": "revive",
        "forces": forces_to_revive,
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

def evaluate_territory(game_state, territory):
    stronghold_territories = ['Arrakeen', 'Carthag', 'Sietch Tabr', 'Habbanya Sietch', 'Tuek\'s Sietch']
    my_forces = get_forces_by_territory(game_state, territory, faction_name)
    my_reserves = game_state['Reserves'][faction_name]
    adjacent_opponents = sum(territory['Forces'][faction] for faction in other_factions)
    cnt_oponents = sum(1 for faction in other_factions if territory['Forces'][faction]>0)
    my_spice = game_state['Faction_Knowledge']['Spice']

    if cnt_oponents == 2:
        return 0

    if territory['name'] in stronghold_territories and cnt_oponents == 0 and my_forces == 0:
        return 5  #Stronghold, unoccupied, highest priority
    
    if territory['name'] in stronghold_territories and cnt_oponents == 1 and simulate_battle(game_state, territory, my_forces + min(my_reserves, my_spice) // 2) > 0.8:
        return 4 #Stronghold, occupied, but i can win
    
    # Todo: see if other factions are near the territory(dist <=2 so they can move here)
    
    if territory['name'] in stronghold_territories and my_forces <= 3:
        return 3  # Weakly defended, medium priority
    
    if  game_state['Spice_Dict'][str(territory['Id'])]['Avaliable'] > 0 and not storm_move(game_state,territory) and cnt_oponents == 0 and my_forces == 0: #maybe consider the case of battle
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
        
        not_in_storm = True
        for section in territory['Sections']:
            if game_state['Storm_Sector'] == section['Origin_Sector']:
                not_in_storm = False

        #possibility of entering stronghold next round
        if near_stronghold and not_in_storm:
            return 1
                    
    # Default: No special consideration
    return 0

def shipment(game_state):

    my_spice = game_state['Faction_Knowledge']['Spice']
    my_reserves = game_state['Reserves'][faction_name]
    best_score = -1
    best_territory = None
    desired_number_troops = 0
    for territory in game_state['Map']['Territories']:
        curr_score = evaluate_territory(game_state, territory)
        if(curr_score > best_score):
            best_score = curr_score
            best_territory = territory

    if best_score == 5:
        desired_number_troops =  min(my_spice // 2, my_reserves // 2)

    if best_score == 4:
        #todo to see if my opponent is after me, if so he can bring more forces
        for number_troops in  range(0, min(my_spice,my_reserves)//2 + 1):
            if simulate_battle(game_state, territory, number_troops) > 0.8:
                desired_number_troops =  number_troops
                break

    if best_score == 3:
        desired_number_troops =  min(my_spice // 4, my_reserves // 4)

    if best_score == 2:
        desired_number_troops = min(min(2, my_spice),my_reserves)
        
    if best_score == 1:
        desired_number_troops = desired_number_troops = min(min(3, my_spice),my_reserves)
    return {
    "action": "shipment", 
    "value": desired_number_troops, "teritorry": best_territory, 
    "section": 100 #todo ales sectiune
    } 


def movement(game_state):
    my_spice = game_state['Faction_Knowledge'][0]['Spice']
    territories = game_state['Map']['Territories']
    storm_sector = game_state['Map']['Storm_Sector']
    off_planet_reserves = game_state['Reserves'][0]['Atreides']
    top_spice_card = game_state['Spice_Deck'][0]
    top_spice_location = top_spice_card['Location']
    storm_distance = abs(storm_sector - top_spice_location)
    arrakeen_forces = next((territory for territory in territories if territory['Name'] == 'Arrakeen'), None)
    carthag_forces = next((territory for territory in territories if territory['Name'] == 'Carthag'), None)
    
    answer = ""
    can_use_ornithopters = arrakeen_forces or carthag_forces

    # Helper function to find a territory by name
    def find_territory(name):
        return next((territory for territory in territories if territory['Name'] == name), None)

    # Helper function to determine if a territory is a stronghold
    def is_stronghold(name):
        return name in ["Arrakeen", "Carthag", "Tuek's Sietch", "Habbanya Sietch", "Sietch Tabr"]

    # Helper function to check if a territory is occupied by other players
    def is_occupied_by_others(territory, exclude_faction="Atreides"):
        for section in territory["Sections"]:
            for faction, forces in section["Forces"].items():
                if faction != exclude_faction and forces > 0:
                    return True
        return False

    # Helper function to get the forces in a territory
    def get_forces_in_territory(territory, faction="Atreides"):
        return sum(section["Forces"].get(faction, 0) for section in territory["Sections"])

    # Determine if the storm is far enough away and if the territory is not sand
    can_move_to_spice = storm_distance > 4 or territories[top_spice_location // len(territories[0]['Sections'])]['Name'] != 'Sand'

    # Movement logic for Atreides
    for territory in territories:
        if territory['Name'] == 'Arrakeen':
            arrakeen_forces_count = get_forces_in_territory(territory)
            if arrakeen_forces_count > 0:
                if can_use_ornithopters:
                    # Move using ornithopters (up to 3 territories)
                    for section in territory['Sections']:
                        for neighbor_id in section['Neighboring_Sections_Ids']:
                            neighbor = next((t for t in territories if any(s['Id'] == neighbor_id for s in t['Sections'])), None)
                            if neighbor and not is_occupied_by_others(neighbor):
                                # Move to the strongest unoccupied territory or to spice location if possible
                                if can_move_to_spice and get_forces_in_territory(find_territory('Arrakeen')) > 1:
                                   return{"action":"movement","source_territory":territory['Id'],"destination_territory":top_spice_location,"value":0.5*arrakeen_forces_count}
                                elif is_stronghold(neighbor['Name']):
                                    return{"action":"movement","source_territory":territory['Id'],"destination_territory":neighbor['Id'],"value":0.5*arrakeen_forces_count}
                                else:
                                    return{"action":"pass"}
                else:
                    best_score = 0
                    base_territory_id = 0
                    best_territory_id = 0
                    best_section_id = -1
                    best_forces = 0
                    for territory in game_state['Map']['Territories']:
                        if territory['Name'] in stronghold_territories:
                            continue
                        my_forces = get_forces_by_territory(game_state, territory, faction_name)
                        if my_forces > 0:
                            for section in territory['Sections']:
                                    for neighbor_section_id in section['Neighboring_Sections_Ids']:
                                        adj_id = get_territory_id_by_section_id(neighbor_section_id)
                                        if adj_id != territory['Id']:
                                            for adj_territoy in game_state['Map']['Territories']:
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
                            
                            if other_attack in projectile_weapons and defense not in projectile_defense
                                my_general_alive = False

                            if other_attack in poison_weapons and defense not in poison_defense:
                                my_general_alive = False

                            other_general_alive = True
                            
                            if attack in projectile_weapons and other_defense not in projectile_defense
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
        #TODO check faction
        return {
            "action": "battle", 
            'used_general': get_best_general(game_state),
            'attack_card': get_attack_card(game_state),
            'defense_card': get_defense_card(game_state),
            'troops': max(my_forces - 1, 1),  # use all forces since chance is high
        }

    elif chance_win > 0.5: #TODO
        dead_generals = list(set(game_state['Tleilaxu_Tanks']['Non_Revivable_Generals'][faction_name] | set(game_state['Tleilaxu_Tanks']['Revivable_Generals'][faction_name])))
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
        return {
            "action": "battle", 
            'used_general': best_general,
            'attack_card': attack_card,
            'defense_card': defense_card,
            'troops': my_forces-1,  
        }

    else:
        #TODO check zic
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
            "action": "battle",   
            'used_general': get_worst_general(game_state),
            'attack_card': atk_card,
            'defense_card': defense_card,
            'troops': max(my_forces - 1, 1)
        }    

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
