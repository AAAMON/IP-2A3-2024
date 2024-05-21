import json
import random

def get_move(game_state):
    spice_list = game_state["Map"]["Spice_List"]
    territories = game_state["Map"]["Section_Forces_list"]
    round = game_state["Round"]
    phase = game_state["Phase"]
    mySpice = game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Harkonnen"]["Spice"]
    deadTroops = game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Harkonnen"][
        "Dead_Troops"]
    storm_sector = game_state["Map"]["Storm_Sector"]
    off_planet_reserves = game_state["Reserves"]["Harkonnen_Forces"]["Forces_Nr"]
    battle_wheels_last_player1=game_state["Battle_Wheels"]["Item1"]["_last_player"]
    battle_wheels_last_player2=game_state["Battle_Wheels"]["Item2"]["_last_player"]
    last_bid=["Special_Faction_Knowledge"]["Last_Bid"]
    tleilaxu_tanks_forces=game_state["Tleilaxu_Tanks"]["Forces"]["Harkonnen_Forces"]["Forces_Nr"]
    tleilaxu_tanks_generals=game_state["Tleilaxu_Tanks"]["Generals"]["Harkonnen"]
    disputed_territory_for_battle_phase=game_state["Map"]["Section_Forces_list"]["origin_territory_id"]
    disputed_territory_forces_for_battle_phase=game_state["Map"]["Section_Forces_list"]["forces"]["Harkonnen_Forces"]["Forces_Nr"]
    treachery_cards=["Special_Faction_Knowledge"]["Treachery_Cards"]
    my_ally=["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Harkonnen"]["Ally"]
    leaders = ["Umman Kudu", "Captain Iakin Nefud", "Piter de Vries", "Beast Rabban", "Feyd-Rautha"]

    if phase == "CHOAM Charity":
        if mySpice < 2:
            return {'mySpice' : 2}

    if phase == "Storm":
        if round == 1:
            random_number_for_first_storm=random.randint(0,20)
            return {'random_number_for_first_storm' : random_number_for_first_storm}
        elif round>1:
            if battle_wheels_last_player1=="Harkonnen" or battle_wheels_last_player2=="Harkonnen":
                random_number_for_subsequent_storm_phases=random.randint(1,3)
                return {'random_number_for_subsequent_storm_phases' : random_number_for_subsequent_storm_phases}

    if phase=="Bidding":
        if mySpice<last_bid:
            my_bidding_offer=last_bid+1
        else:
            my_bidding_offer=0
        return {'my_bidding_offer' : my_bidding_offer}


    if phase == "Revival":
        if deadTroops>=8:
            if mySpice >=3:
                return {'troops_revived': 3}
        elif deadTroops >=5:
            if mySpice>=2:
                return {'troops_revived': 2}
        else:
            {'troops_revived': 0}

    if phase == "Battle":
        chosen_leader_for_battle=random.choice(leaders)
        number_of_forces_for_battle=random.randint(0, disputed_territory_forces_for_battle_phase)
        if random.randint(0,1)==1:
            treachery_card_for_battle=random.choice(treachery_cards)
        else:
            treachery_card_for_battle='none'
        return {
            'chosen_leader_for_battle': chosen_leader_for_battle,
            'number_of_forces_for_battle': number_of_forces_for_battle,
            'treachery_card_for_battle' : treachery_card_for_battle
        }

    if phase == "Spice collection":
        return {"action:" "Spice collection.Se face de API."}

    if phase == "Mentat pause":
        return {"action:" "Mentat pause.Se face de API."}

    if phase == "Nexus":
        good=["Bene Gesserit", "Spacing Guild", "Fremen"]
        weak=["Atreides", "Emperor"]
        break_alliance="none"
        try_alliance="none"

        for x in weak:
            if x in my_ally:
                break_alliance=x

        for x in good:
            if x not in my_ally:
                try_alliance=x

        return {"break_alliance" : break_alliance,
                "try_alliance" : try_alliance
                }

    if phase == "Shipment and movement":
        ans = "none"
        if off_planet_reserves > 0 and mySpice >= 7:
            for territory in territories:
                if territory["forces"]["Spacing_Guild_Forces"]["Forces_Nr"] == 0:
                    if territory["forces"]["Atreides_Forces"]["Forces_Nr"] == 0:
                        if territory["forces"]["Bene_Gesserit_Forces"]["Forces_Nr"] == 0:
                            if territory["forces"]["Emperor_Forces"]["Sardaukar"] == 0:
                                if territory["forces"]["Emperor_Forces"]["Normal"] == 0:
                                    if territory["forces"]["Fremen_Forces"]["Normal"] == 0:
                                        if territory["forces"]["Harkonnen_Forces"]["Forces_Nr"] == 0:
                                            ans = territory
        return {
            "territory_to_occupy": ans
        }
    
    return {"status": "unknown phase"}
