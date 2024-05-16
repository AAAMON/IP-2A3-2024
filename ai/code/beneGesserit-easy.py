

def get_money(game_state):
    game_state = game_state['Public_Faction_Knowledge_Manager']
    game_state = game_state['Public_Faction_Knowledge']
    game_state = game_state['Bene_Gesserit']
    return game_state['Spice']
    

def get_dead(game_state):
    game_state = game_state['Tleilaxu_Tanks']
    game_state = game_state['Bene_Gesserit_Forces']
    return game_state['Normal']


def calc_bid():
    return {'bid': get_money()}     #inca nu pot sa stiu cat au pus ceilalti


def get_move(game_state):
    
    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}
    
    if game_state['Phase'] == 'Prediction':  
        return {'predict': 'Spacing_Guild'}      #most likely cred
    
    if  game_state['Phase'] =='CHOAM Charity':
            return {'charity': 2}
    
    if game_state['Phase'] == 'Bidding':
        return calc_bid()
    
    if game_state['Phase'] == 'Revival':
        if get_dead(game_state)>0:
            return {'revive': 1}    #gene gesserit are low revival rate, dar are 1 free revival
        else: 
            return {'revive': 0}
    
    elif game_state['Phase'] == 'Movement':  #de adaugat de la api ca atunci cand ceilalti fac shipment sa pot si eu sa imi pun un force
        return {'action': 'none'}
    
    elif game_state['Phase'] == 'Aliance':  #probabil ar trebui facut cu Atreides (great combat)
        return {'action': 'deny'}

    else:
        return {'status': 'phase unknown'}