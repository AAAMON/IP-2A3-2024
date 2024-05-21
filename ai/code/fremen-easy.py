

def get_money(game_state):
    game_state = game_state['Public_Faction_Knowledge_Manager']
    game_state = game_state['Public_Faction_Knowledge']
    game_state = game_state['Fremen']
    return game_state['Spice']
    

def get_dead(game_state):
    game_state = game_state['Tleilaxu_Tanks']
    game_state = game_state['Fremen_Forces']
    return game_state['Normal']


def bid_phase(game_state):
    #fremen is generally poor, so bidding too much is generally bad

    my_money = get_money(game_state)
    last_bid = game_state['last_bid']

    if last_bid['value'] > max(2, my_money):
        return {'action': 'pass'}
    
    return {'action': last_bid['value']+1}


def aliance_phase(game_state):
    #fremen is generally left last for aliance we agree to any aliance we can get
    #but we prefeer emperor aliance
    #source: https://boardgamegeek.com/thread/595039/best-alliances-in-your-opinion-and-why

    if 'aliance_request' in game_state.keys():
        return {'action': 'accept'}
    
    ans = {}
    ans['action'] = 'propose'
    ans['target'] = 'Emperor'
    return ans


def movement_phase(game_state):

    return {'action': 'pass'}   


def battle_phase(game_state):

    battleTeritory = game_state['battle']['teritory_id']

    weapon_projectile=["Crysknife","Maula Pistol","Slip Tip","Stunner"] #shield
    weapon_poison=["Chaumas","Chaumurky","Ellaca Drug","Gom Jabbar"] #snooper
    weapon_special=["Lasgun"]
    #Automatically kills opponent's leader regardless of defense card used.You may keep this card if you win this battle.If anyone plays a Shield in this battle, all forces, leaders, and spice in this battle's territory are lost to the Tleilaxu Tanks and Spice Bank. Both players lose this battle, no spice is paid for leaders, and all cards played are discarded
    defense_projectile=["Shield"] #projectile weapon
    defense_posion=["Snooper"] #poison weapon
    special_leader=["Special-Leader"]
    #Play as a leader with zero strength on your Battle Plan and discard after the battle.You may also play a weapon and a defense. The cheap hero may be played in place of a leader or when you have no leaders available.
    special_storm=["Family Atomics","Weather Control"]
    special_movement=["Hajr"]
    special=["Karama","Tleilaxy Ghola","Truthtrance"]
    worthless_card=["Baliset","Jubba Cloak","Kulon","La,la,la","Trip to Gamont"]

    my_trachery_card=game_state["Special_Faction_Knowledge"]["Treachery_Cards"]

    #count opponent forces
    for teritory in game_state['teritory']:

        if teritory['id'] != battleTeritory:
            continue

        cnt_oponent = 0

        for faction in ['Emperor', 'Harkonen', 'Atreides', 'Bene Gesserit', 'Space Guild']:
            cnt_oponent += teritory['forces'][faction + "_Forces"]['Normal']

        
    #count my forces
    cnt_me = teritory['forces']['Fremen_Forces']['Normal']
 
    ans = {}
    ans['General'] = 'Stilgar'
    ans['Forces'] = max(cnt_oponent, cnt_me-1)
    
    return ans

def get_move(game_state):
    
    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}
    
    if game_state['Phase'] == 'Bidding':
        return bid_phase(game_state)
    
    elif game_state['Phase'] == 'Revival':
        return {'revive': min(3, get_dead(game_state))}
    
    elif game_state['Phase'] == 'Movement':
        return movement_phase(game_state)
    
    elif game_state['Phase'] == 'Aliance':
        return aliance_phase(game_state)
    
    elif game_state['Phase'] == 'CHOAM Charity':
        return {'action': 'CHOAM Charity'}

    else:
        return {'status': 'phase unknown'}
