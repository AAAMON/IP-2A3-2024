

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


def get_move(game_state):
    
    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}
    
    if game_state['Phase'] == 'Bidding':
        return bid_phase(game_state)
    
    elif game_state['Phase'] == 'Revival':
        return {'revive': min(3, get_dead(game_state))}
    
    elif game_state['Phase'] == 'Movement':
        return {'action': 'none'}
    
    elif game_state['Phase'] == 'Aliance':
        return {'action': 'deny'}

    else:
        return {'status': 'phase unknown'}
