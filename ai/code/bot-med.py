import random

projectile_weapons = ["Crysknife", "Maula Pistol", "Slip Tip", "Stunner"]
poison_weapons = ["Chaumas", "Chaumurky", "Ellaca Drug", "Gom Jabbar"]
special_weapons = ["Lasgun"]

projectile_defense = ["Shield"]
poison_defense = ["Snooper"]

weapon_cards = projectile_weapons + poison_weapons + special_weapons
defense_cards = projectile_defense + poison_defense

worthless_cards =  ["Baliset", "Jubba Cloak", "Kulon", "La, la, la", "Trip to Gamont"]


#faction info
faction_name = "todo"
nr_free_revive = 0
other_factions = ['Bene Gesserit', 'Atreides', 'Harkonnen', 'Fremen', 'Spacing_Guild', 'Emperor']



def pick_traitor(game_state):
    #todo
    return {'action' : 'none'}

def aliance(game_state):
    #todo
    return {'action' : 'none'}

def bidding(game_state):

    #de vazut ce pune atreides ultima data 
    #daca emperor este last bid

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

    if len (my_cards.keys()) >= 2:
        coef -= 0.1

    coef = min(coef, 0.8)
    maxBid = my_spice * coef

    if last_bid >= maxBid:
        return {'action': 'pass'}
    
    else:
        return {'action': 'bid', 'value': last_bid+1}
    


def revival(game_state):
    #faction dependent
    my_spice = game_state['Faction_Knowledge']['Spice']
    nr_dead = game_state['Tleilaxu_Tanks']['Forces'][faction_name]

def storm_move(game_state, territory):
    storm_place = game_state['Storm_Sector']
    for i in range(1,7):
        storm_place += 1
        if storm_place == 19:
            storm_place = 1
        for section in territory['Territories']:
            if storm_place == section['Origin_Sector']:
                return False
    return True



def evaluate_territory(game_state, territory):
    stronghold_territories = ['Arrakeen', 'Carthag', 'Sietch Tabr', 'Habbanya Sietch', 'Tuek\'s Sietch']
    my_forces = territory['Forces'][faction_name]
    adjacent_opponents = sum(territory['Forces'][faction] for faction in other_factions)
    cnt_oponents = sum(1 for faction in other_factions if territory['Forces'][faction]>0)
    my_spice = game_state['Faction_Knowledge']['Spice']

    if cnt_oponents == 2:
        return 0

    if territory['name'] in stronghold_territories and cnt_oponents == 0 and my_forces == 0:
        return 5  #Stronghold, unoccupied, highest priority
    
    if territory['name'] in stronghold_territories and cnt_oponents == 1 and simulate_battle(game_state, territory)>0.8:
        return 4 #Stronghold, occupied, but i can win
    
    # Todo: see if other factions are near the territory(dist <=2 so they can move here)
    
    
    if my_forces == 1 or my_forces == 2 :
        return 3  # Weakly defended, medium priority
    
    if  game_state['Spice_Dict'][str(territory['Id'])]['Avaliable'] > 0 and storm_move(game_state,territory) and cnt_oponents == 0: #maybe consider the case of battle
        return 2 #i bring them on the map free basically
    
    if adjacent_opponents > 0 and territory['name'] not in stronghold_territories:
        return random.random() < 0.5
    
    # Default: No special consideration
    return 0

def shipment(game_state):

    my_spice = game_state['Faction_Knowledge']['Spice']

    if my_spice < poverty_line(game_state):
        #my target is getting spice
        pass
    else:
        #my target are strongholds
        pass

def movement(game_state):

    my_spice = game_state['Faction_Knowledge']['Spice']

    if my_spice < poverty_line(game_state):
        #my target is getting spice
        pass
    else:
        #my target are strongholds
        pass



def get_move(game_state):
    
    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}
    
    phase_name = game_state['Phase']['name']

    if phase_name == 'Bidding':
        return bid_phase(game_state)
    
    elif phase_name == 'Revival':
        return {'revive': min(3, get_dead(game_state))}
    
    elif phase_name == 'Movement':
        return movement_general(game_state)
    
    elif phase_name == 'Nexus':
        return aliance_phase(game_state)
    
    elif phase_name == 'CHOAM Charity':
        return {'action': 'CHOAM Charity'}

    else:
        return {'status': 'phase unknown'}
