def get_money(game_state):
    game_state = game_state['Public_Faction_Knowledge_Manager']
    game_state = game_state['Public_Faction_Knowledge']
    game_state = game_state['Atreides']
    return game_state['Spice']
    

def get_dead(game_state):
    game_state = game_state['Tleilaxu_Tanks']
    game_state = game_state['Forces']
    game_state = game_state['Atreides_Forces']
    return game_state['Forces_Nr']


def calc_bid(game_state):
    return {'bid': get_money(game_state)}


def get_move(game_state):
    
    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}
    
    if game_state['Phase'] == 'Bidding':
        return calc_bid(game_state)
    
    elif game_state['Phase'] == 'Revival':
        return {'revive': min(3, get_dead(game_state))}
    
    elif game_state['Phase'] == 'Movement':
        return {'action': 'none'}
    
    elif game_state['Phase'] == 'Aliance':
        return {'action': 'deny'}

    else:
        return {'status': 'phase unknown'}

