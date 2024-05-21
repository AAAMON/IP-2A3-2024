import json
import random

def get_move(game_state):
    spice_list = game_state["Map"]["Spice_List"]
    territories = game_state["Map"]["Section_Forces_list"]
    round = game_state["Round"]
    phase = game_state["Phase"]
    my_spice = game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Atreides"]["Spice"]
    deadTroops = game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Atreides"]["Dead_Troops"]
    storm_sector = game_state["Map"]["Storm_Sector"]
    my_sector = game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Atreides"]["Player_Marker"]
    off_planet_reserves = game_state["Reserves"]["Atreides_Forces"]["Forces_Nr"]
    current_hand = game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Atreides"]["Current_Hand"]

    if phase=="Storm" or phase==1:
        answer=""
        if round==1 and abs(storm_sector-my_sector)==1: 
            #adica practic prima tura cand sunt in apropierea sectorului de cerc al furtunii
            random_number = random.randint(0, 20)
            answer="storm_initializer: "+ str(random_number)+" traitor: "
        if round==1:
            leaders_desc=["Stilgar","Chani","Feyd-Rautha","Hasimir Fenring","Alia",
                          "Margot Lady Fenring","Mother Ramallo","Princess Irulan","Wanna Yueh",
                          "Captain Aramsham","Otheym","Staban Tuek","Beast Rabban","Shadout Mapes","Esmar Tuek",
                          "Master Bewt","Piter de Vries","Caid","Burseg","Bashar","Jamis","Soo-Soo Sook",
                          "Captain Iakin Nefud","Umman Kudu","Guild Rep"]        
            possible_traitors=game_state["Special_Faction_Knowledge"]["Traitors"]
            for traitor in leaders_desc:
                if traitor in possible_traitors:
                    break
            answer+=traitor #aleg cel mai puternic care nu e in echipa mea;daca toti sunt lideri de la mine il aleg pe ultimul
            return{"action ": answer}

        #daca nu sunt in advanced ca altfel decide fremen -va trebui sa iau asta din perspective mai incolo
        player1=game_state['Battle_Wheels']['Item1']['_last_player']
        player2=game_state['Battle_Wheels']['Item1']['_last_player']
        if player1=="Atreides" or player2=="Atreides":
            random_number = random.randint(1, 3)
            return {"action " "storm_movement ": str(random_number)}
        else:
            return {"action" : "It's Storm phase but I do nothing."}

    if phase == "Spice Blow and NEXUS" or phase == 2:
         factions = ["Fremen", "Harkonnen", "Spacing Guild", "Emperor", "Bene Gesserit"]
         offer_made = False

         for faction in factions:
            if faction not in alliance_offers or not alliance_offers[faction]:
                return {"action": f"Offer alliance to {faction}", "spice_offer": offered_spice}
        
         if not offer_made:
            return {"action": "No alliances accepted, try next round with more spice"}

    
    if phase=="CHOAM Charity" or phase==3:
            if my_spice<2:
                return {"action": "CHOAM Charity"}

    if phase == "Bidding" or phase == 4:
            if len(current_hand) >= 4:
                return "no bid: hand full"
            answer = "bidding: "
            # Atreides can look at the first card before bidding
            first_card = game_state["Treachery_Cards"][0]
            important_cards = ["Lasgun", "Shield", "Snooper", "Hajr", "Weather Control", "Tleilaxu Ghola"]

            if first_card in important_cards:
                if my_spice > 10:
                    bid_increase = max(1, int(0.30 * my_spice))
                elif my_spice < 5:
                    bid_increase = 1
                else:
                    bid_increase = 2
                last_bid = game_state.get("Last_Bid", 0)
                my_bid = last_bid + bid_increase
                answer += f"looked at {first_card}, bid {my_bid} spice"
            else:
                if game_state.get("Is_First_Bidder", False):
                    answer += f"looked at {first_card}, bid 1 spice"
                else:
                    last_bid = game_state.get("Last_Bid", 0)
                    max_bid = int(0.30 * my_spice)
                    if last_bid + 1 < max_bid:
                        if first_card in ["Crysknife","Maula Pistol","Slip Tip","Stunner","Chaumas","Chaumurky","Ellaca Drug","Gom Jabbar","Cheap Hero","Family Atomics","Karama","Truthtrance"]:
                            my_bid = last_bid + 1
                            answer += f"looked at {first_card}, bid {my_bid} spice"
                    else:
                        answer += f"looked at {first_card}, no bid"

            return answer

    if phase=="Revival" or phase==5:
            if deadTroops>2:
                if my_spice>10:
                    return {"action ": "revive 3"}
            if deadTroops>1:
                if my_spice>5:
                    return {"action ": "revive 2"}
            if deadTroops>0:
                return {"action ": "revive 1"}
            
            return {"action:" "No revival."}
    #trb verificat si daca pot si daca am bani
    if phase == "Shipment and Movement" or phase == 6:
        answer = "shipment_and_movement: "
        # Look at the top card of the Spice Deck
        top_spice_card = game_state["Spice_Deck"][0]
        top_spice_location = top_spice_card["Location"]
        storm_distance = abs(storm_sector - top_spice_location)
         # Shipment logic
        if off_planet_reserves > 0 and my_spice >= 7:
            for territory in territories:
                if territory["forces"]["Spacing_Guild_Forces"]["Forces_Nr"] == 0 and \
                   territory["forces"]["Atreides_Forces"]["Forces_Nr"] == 0 and \
                   territory["forces"]["Bene_Gesserit_Forces"]["Forces_Nr"] == 0 and \
                   territory["forces"]["Emperor_Forces"]["Sardaukar"] == 0 and \
                   territory["forces"]["Emperor_Forces"]["Normal"] == 0 and \
                   territory["forces"]["Fremen_Forces"]["Normal"] == 0 and \
                   territory["forces"]["Harkonnen_Forces"]["Forces_Nr"] == 0:
                    ans = 'shippment: action: occupy_territory with 4 forces'
                    break

        if not answer:
            answer = 'no_shippment'
        # Move forces logic for Atreides
        if my_spice < 5 and min(spice_list) > 2:
            mov = 'movement: action: collect spice at closest location if possible'
        else:
            for sector in territories:
                if sector['forces']['Atreides_Forces']['Forces_Nr'] > 0:
                    if (storm_distance > 4 or territories[top_spice_location]['type'] != 'sand'):
                        answer += f"moved to sector {top_spice_location}, "
                        break
                    # else:
                    #     # Move normally
                    #     destination_sector = (sector['id'] + 1) % len(territories)
                    #     answer += f"moved from sector {sector['id']} to {destination_sector}, "
        if not mov:
            mov=' and no_move.'
        answer=answer+mov
        return answer.strip(', ')

    if phase=="Battle" or phase==7:
            battle_id=game_state["Battle_id"]
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

            for territory in territories:
                if territory["id"] == battle_id:
                    #am gasit unde ma bat
                    if game_state['Battle_Wheels']['Item1']['_last_player']=="spacingGuild":
                        player=game_state['Battle_Wheels']['Item2']['_last_player']
                    else: 
                        player=game_state['Battle_Wheels']['Item1']['_last_player']
                    #stiu cu cine ma bat
                    if player=="Emperor":
                        number_opponent_forces=territory["forces"]["Emperor_Forces"]["Sardaukar"]*2+territory["forces"]["Emperor_Forces"]["Normal"]
                    elif player=="Fremen":
                            number_opponent_forces=territory["forces"]["Fremen_Forces"]["Fedaykin"]*2+territory["forces"]["Fremen_Forces"]["Normal"]
                    else:
                        number_opponent_forces=territory["forces"][player]["Forces_Nr"]
                    #stiu cate forte are jucatorul
                    number_forces=territory["forces"]["Spacing_Guild_Forces"]["Forces_Nr"]
                    my_generals=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Spacing_Guild"]["Generals"]

                    if number_opponent_forces-number_forces<=3 and number_forces-1>0:
                        #imi fac plan de lupta calumea
                         my_general=""
                         for general in my_generals:
                             my_general=general #aleg cel mai puternic general disponibil
                             break
                         if my_general=="":
                             if "Special-Leader" in my_trachery_card:
                                 my_general="Special-Leader"
                         if my_general=="":
                             forces=number_forces-1
                             answer="general:none , forces: "+str(forces)+" ,weapon: none , defense: none"
                             return {"action ": answer}
                         #imi aleg weapon si defense
                         chosen_cards = {"weapon": None, "defense": None}
                         for my_card in my_trachery_card:
                            if not chosen_cards["weapon"] and my_card in weapon_projectile+weapon_poison+weapon_special:
                                chosen_cards["weapon"]=my_card
                            elif not chosen_cards["defense"] and my_card in defense_posion+defense_projectile:
                                if chosen_cards["weapon"]=="Lasgun":
                                    chosen_cards["defense"]=" none"
                                else:
                                    chosen_cards["defense"]=my_card
                                    #print(chosen_cards["defense"])
                                    break
                            if chosen_cards["weapon"] and chosen_cards["defense"]:
                                break
                         answer="general: "+my_general
                         print(chosen_cards["defense"])
                         answer+=" , forces: "+str(number_forces-1)+" ,weapon :"+chosen_cards["weapon"]+" ,defense: "+chosen_cards["defense"]
                         return {"action" : answer}
                    else:
                        #fac sa pierd cat mai putin
                        my_general=""
                        if "Special-Leader" in my_trachery_card:
                                 my_general="Special-Leader"
                        if my_general=="":
                            for general in my_generals:
                             my_general=general #aleg cel mai slab general disponibil
                        if my_general=="":
                            return {"action ": "no general no nothing"}
                        else:
                            card1=""
                            card2=""
                            for useles_card in worthless_card:
                                if  useles_card in my_trachery_card:
                                    if card1=="":
                                        card1=useles_card
                                    else:
                                        if card2=="":
                                            card2=useles_card
                                        else:
                                            break
                            ans="general: "+my_general+" , weapon: "
                            if not card1:
                                ans+=card1+" , defense: "
                            else:
                                ans+="none , defense: "
                            if not card2:
                                ans+=card2
                            else:
                                ans+="none"
                            return {"action" :ans}
                    break
                                                          
            #cumva dupa ce mi am pus ce battle plan am si ce battle plan are oponentul, ar trebui sa zic @Traitor@ daca si l-a pus pe el
            return {"action ": "Choose leader, weapon, shield and threachery cards"}
    if phase == "Spice Collection" or phase == 8:
            return {"action": "Spice collection. API will handle it."}
    if phase == "Mentat Pause" or phase == 9:
            answer = "mentat_pause: "
            # Strategic pause for Atreides
            answer += "strategizing"
            return answer

    return "unknown phase"
