def get_money(game_state):
    game_state = game_state['Public_Faction_Knowledge_Manager']
    game_state = game_state['Public_Faction_Knowledge']
    game_state = game_state['Bene_Gesserit']
    return game_state['Spice']
    

def get_dead(game_state):
    game_state = game_state['Tleilaxu_Tanks']
    game_state = game_state['Bene_Gesserit_Forces']
    return game_state['Normal']


def calc_bid(game_state):
    my_money = get_money(game_state)   #trebuie sa iau carti in the early game
    last_bid = game_state['last_bid']

    if last_bid['value'] > my_money*70/100:  #sa nu consum totusi prea mult
        return {'action': 'pass'}
    
    return {'action': last_bid['value']+1}

def shipment(game_state):
    if game_state['Round'] < 4:
            if game_state['Reserves']['Bene_Gesserit_Forces']["Forces_Nr"] >= 2 and mySpice >= 4:  #nu stiu de la api unde e stronghold si restul
                for territory in game_state["Map"]["Section_Forces_list"]:
                    #nu am forte acolo
                    if territory["forces"]["Bene_Gesserit_Forces"]["Forces_Nr"] == 0:
                        #nu e ocupata de alte doua factiuni
                        if( (int)(territory["forces"]["Atreides_Forces"]["Forces_Nr"] > 0) + (int)(territory["forces"]["Emperor_Forces"]["Sardaukar"] > 0) + (int)(territory["forces"]["Emperor_Forces"]["Normal"] > 0) + (int)(territory["forces"]["Fremen_Forces"]["Normal"] > 0) + (int)(territory["forces"]["Harkonnen_Forces"]["Forces_Nr"] > 0) + (int)(territory["forces"]["Spacing_Guild_Forces"]["Forces_Nr"] > 0) < 2): 
                            if territory["forces"]["Atreides_Forces"]["Forces_Nr"] <= 2:
                                if territory["forces"]["Emperor_Forces"]["Sardaukar"]  <= 2:
                                    if territory["forces"]["Emperor_Forces"]["Normal"]  <= 2:
                                        if territory["forces"]["Fremen_Forces"]["Normal"]  <= 2:
                                            if territory["forces"]["Harkonnen_Forces"]["Forces_Nr"]  <= 2:
                                                if territory["forces"]["Spacing_Guild_Forces"]["Forces_Nr"] <= 2:
                                                    return {'shippment : forces : ' + 2 + ' id: ' + territory }        


def battle(game_state):
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

    for teritory in game_state['teritory']:
        if teritory['id'] == battleTeritory:
            nr_oponent=0
            for faction in ['Emperor', 'Harkonen', 'Atreides', 'Fremen_Forces', 'Space Guild']:
                nr_oponent += teritory['forces'][faction + "_Forces"]['Normal']

        
    #count my forces
    nr_me = teritory['forces']['Bene Gesserit']['Normal']
 
    ans = {}
    #nu stiu ce generali am
    #ar trebui sa aleg unul care stiu ca nu e traitor
    generals = ['Mother Mohian','Mother Ramallo','Alia','Princess Irulan','Margot Lady Fenring','Wanna Yueh']
    ans['General'] = generals[1]
    ans['Forces'] = max(nr_oponent, nr_me-1)

    #use of cards
    #use of voice
    
    return ans

def get_move(game_state):
    
    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}
    
    if game_state['Phase'] == 'Prediction' or game_state['Phase'] == 0:  
        return {'predict': 'Spacing_Guild, 6'}      #most likely cred, 4-6 best for predictions
    
    if  game_state['Phase'] =='CHOAM Charity' or game_state['Phase'] == 3:
            return {'charity': 2}
    
    if game_state['Phase'] == 'Bidding' or game_state['Phase'] == 4:
        return calc_bid(game_state)
    
    if game_state['Phase'] == 'Revival' or game_state['Phase'] == 5:
        #gene gesserit are low revival rate, dar are 1 free revival
        if get_dead(game_state)>0:
            return {'revive': 1}   
        else: 
            return {'revive': 0}
    

    #de adaugat de la api ca atunci cand ceilalti fac shipment sa pot si eu sa imi pun un force
    if game_state['Phase'] == 'Shipment and movement' or game_state['Phase'] == 6:
        #in the early game get into fights with small numbers of your own forces and voice them to use defenses to get them to reveal what they have to everyone and lose forces pointlessly  
        return shipment(game_state)
        
    if game_state['Phase'] == 'Battle' or game_state['Phase'] == 7:  
        return battle(game_state)
    
    #probabil ar trebui facut cu Atreides (great combat), Harkonen (strong when Harks have a lot of cards), Emperor ( support in shipping/revive/fighting), other if they have a lot of money
    if game_state['Phase'] == 'Aliance':  
        return {'action': 'deny'}

    return {'status': 'phase unknown'}