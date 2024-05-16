def get_money(game_state):
    game_state = game_state['Public_Faction_Knowledge_Manager']
    game_state = game_state['Public_Faction_Knowledge']
    game_state = game_state['Harkonnen']
    return game_state['Spice']


def get_dead(game_state):
    game_state = game_state['Tleilaxu_Tanks']
    game_state = game_state['Harkonnen_Forces']
    return game_state['Normal']


def calc_bid():
    return {'bid': get_money()}


def get_move(game_state):
    mySpice = game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Harkonnen"]["Spice"]

    if 'Phase' not in game_state.keys():
        return {'status': 'bad format'}

    if game_state['Phase'] == 'Storm':
        return {'action': 'none, se ocupa API'}

    if game_state['Phase'] == 'CHOAM Charity':
        if mySpice == 0:
            return {'action': 'Get 2 spices'}
        elif mySpice == 1:
            return {'action': 'Get 1 spice'}
        else:
            return {'action': 'No more spices'}

    if game_state['Phase'] == 'Bidding':
        return calc_bid()

    elif game_state['Phase'] == 'Revival':
        return {'revive': min(3, get_dead(game_state))}

    elif game_state['Phase'] == 'Movement':
        return {'action': 'none'}

    elif game_state['Phase'] == 'Aliance':
        return {'action': 'Draw 4 Traitor Cards'}

    else:
        return {'status': 'phase unknown'}