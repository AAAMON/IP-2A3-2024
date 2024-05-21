import json
import random
def get_move(game_state):
        with open("perspective3.json", "r") as file:
            game_state = json.load(file)
        spice_list = game_state["Map"]["Spice_List"]
        territories = game_state["Map"]["Section_Forces_list"]
        round =game_state["Round"]
        phase =game_state["Phase"]
        mySpice=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Emperor"]["Spice"]
        deadTroops=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Emperor"]["Dead_Troops"]
        off_planet_reserves = game_state["Reserves"]["Emperor_Forces"]["Normal"]
        storm_sector = game_state["Map"]["Storm_Sector"]
        my_sector=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Spacing_Guild"]["Player_Marker"]
        faction_bids = game_state.get("Faction_Bids", {})
        alliance_offers = game_state.get("Alliance_Offers", {})
        current_alliance = game_state.get("Current_Alliance", None)
        if phase=="Storm" or phase==1:
            answer=""
            if round==1 and abs(storm_sector-my_sector)==1: 
                #adica practic prima tura cand sunt in apropierea sectorului de cerc al furtunii
                random_number = random.randint(0, 20)
                answer="storm_initializer: "+ str(random_number)+" traitor: "
            if round==1:
                leaders_desc=["Stilgar","Chani","Feyd-Rautha","Thufir Hawat","Lady Jessica",
                              "Alia","Margot Lady Fenring","Mother Ramallo","Princess Irulan","Wanna Yueh"
                              "Staban Tuek","Otheym","Beast Rabban","Gurney Halleck","Master Bewt","Esmar Tuek","Shadout Mapes","Piter de Vries"
                              "Captain Iakin Nefud","Jamis","Soo-Soo Sook","Duncan Idaho"
                              "Umman Kudu","Dr. Wellington Yueh","Guild Rep"]
                possible_traitors=game_state["Special_Faction_Knowledge"]["Traitors"]
                for traitor in leaders_desc:
                    if traitor in possible_traitors:
                        break
                answer+=traitor #aleg cel mai puternic care nu e in echipa mea;daca toti sunt lideri de la mine il aleg pe ultimul
                return{"action ": answer}
                
            #daca nu sunt in advanced ca altfel decide fremen -va trebui sa iau asta din perspective mai incolo
            player1=game_state['Battle_Wheels']['Item1']['_last_player']
            player2=game_state['Battle_Wheels']['Item1']['_last_player']
            if player1=="Emperor" or player2=="Emperor":
                random_number = random.randint(1, 3)
                return {"action " "storm_movement ": str(random_number)}
            else:
                return {"action" : "It's Storm phase but I do nothing."}
        if phase == "Spice Blow and NEXUS" or phase == 2:
         factions = ["Fremen", "Harkonnen", "Spacing Guild", "Atreides", "Bene Gesserit"]
        offer_made = False

        for faction in factions:
            if faction not in alliance_offers or not alliance_offers[faction]:
                return {"action": f"Offer alliance to {faction}", "spice_offer": offered_spice}
        
        if not offer_made:
            return {"action": "No alliances accepted, try next round with more spice"}

        
        if phase=="“CHOAM Charity" or phase==3:
            if mySpice<2:
                return {"action:" "“CHOAM Charity"}
        
        if phase == "Bidding" or phase == 4:
            number_threachery=len(game_state["Special_Faction_Knowledge"]["Treachery_Cards"])
            ans="threachery_cards_number: "+str(number_threachery)
            last_bid=game_state["Last_bid"]
            if round==1:
                if mySpice>=last_bid+1:
                    ans+=" , bid: "+str(last_bid+1)
                else:
                    ans+=" , bid: pass"
            else:
                if number_threachery<3 and round!=1 and mySpice*70/100>=last_bid+1:
                    ans+=" , bid: "+str(last_bid+1)
                else:
                  if number_threachery==3 and round!=1 and mySpice*20/100>=last_bid+1:
                     ans+=" , bid: "+str(last_bid+1)
                  else:
                     ans+=" , bid:pass"
            return{"action" :ans}

        if phase=="Battle" or phase==6:
           leaders=["Hasimir Fenring","Captain Aramsham","Caid","Burseg","Bashar"]            
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
           battle_id=game_state["Battle_id"]

           for territory in territories:
                if territory["id"] == battle_id:
                    #am gasit unde ma bat
                    if game_state['Battle_Wheels']['Item1']['_last_player']=="Emperor":
                        player=game_state['Battle_Wheels']['Item2']['_last_player']
                    else: 
                        player=game_state['Battle_Wheels']['Item1']['_last_player']
                    #stiu cu cine ma bat
                    
                    if player=="Fremen":
                            number_opponent_forces=territory["forces"]["Fremen_Forces"]["Fedaykin"]*2+territory["forces"]["Fremen_Forces"]["Normal"]
                    else:
                        number_opponent_forces=territory["forces"][player]["Forces_Nr"]
                    #stiu cate forte are jucatorul
                    number_forces=territory["forces"]["Emperor"]["Normal"]

                    if number_opponent_forces-number_forces<=3 and number_forces-1>0:
                        #imi fac plan de lupta calumea
                         my_generals=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Emperor"]["Generals"]
                         for general in my_generals:
                             my_general=general #aleg cel mai puternic general disponibil
                             break
                         if not my_general:
                             if "Special-Leader" in my_trachery_card:
                                 my_general="Special-Leader"
                         if not my_general:
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
                                    break
                                else:
                                    chosen_cards["defense"]=my_card
                            if chosen_cards["weapon"] and chosen_cards["defense"]:
                                break
                            answer="general: "+my_general
                            answer+=" , forces: "+str(number_forces-1)+" ,weapon :"+chosen_cards["weapon"]+" ,defense: "
                            answer+=str(chosen_cards["defense"])
                         return {"action" : answer}
                    else:
                        my_generals=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Emperor"]["Generals"]
                        if "Special leader" in my_trachery_card:
                            my_general="Special-Leader"
                        else:
                            for general in my_generals:
                             my_general=general #aleg cel mai slab general disponibil
                             forces=number_forces-1


                         
                                               
                         #cumva dupa ce mi am pus ce battle plan am si ce battle plan are oponentul, ar trebui sa zic @Traitor@ daca si l-a pus pe el
           return {"action ": "Choose leader, weapon, shield and threachery cards"}
            
        

       # if phase == "Bidding" or phase == 4:
        # harkonnen_bid = faction_bids.get("Harkonnen", 0)
        # atreides_bid = faction_bids.get("Atreides", 0)

         #if harkonnen_bid > 0:
          #  bid_amount = harkonnen_bid + 1
        #elif atreides_bid > 0:
         #   bid_amount = atreides_bid + 1
    
        #else:
         #   min_bid = min(spice_list)
          #  bid_amount = min_bid if min_bid <= mySpice else 0

        # Nu licita mai mult de 70% din spice-ul disponibil
        #if bid_amount > mySpice * 0.7:
         #   bid_amount = 0

        #if bid_amount > 0:
         #   return {"action": "bid", "bid_amount": bid_amount}
        #else:
         #   return {"action": "no bid"}
            
       
            
        if phase=="Revival" or phase==5:
            if deadTroops>2:
                if mySpice>10:
                    return {"action:" "Revive 3 ."}
                elif mySpice>5:
                    return {"action:" "Revive 2.)"}
            else:
                return {"action:" "revive 1."}
                   
        if phase == "Shipment and movement" or phase == 5:
         ans = {}
         if off_planet_reserves > 0 and mySpice >= 7:
            for territory in territories:
                if territory["forces"]["Spacing_Guild_Forces"]["Forces_Nr"] == 0:
                    if territory["forces"]["Atreides_Forces"]["Forces_Nr"] == 0:
                        if territory["forces"]["Bene_Gesserit_Forces"]["Forces_Nr"] == 0:
                            if territory["forces"]["Emperor_Forces"]["Sardaukar"] == 0:
                                if territory["forces"]["Emperor_Forces"]["Normal"] == 0:
                                    if territory["forces"]["Fremen_Forces"]["Normal"] == 0:
                                        if territory["forces"]["Harkonnen_Forces"]["Forces_Nr"] == 0:
                                            ans= 'shippment: action: occupy_territory with 4 forces'
         if not ans:
                ans = 'no_shippment'
            
         mov={}
         if mySpice < 5 and min(spice_list) > 2:
            mov = 'movement: action : collect spice unde e mai aproape daca pot'
         else:
            for territory in territories:
                if territory["forces"]["Emperor_Forces"]["Normal"] > 0:
                    if territory["forces"]["Emperor_Forces"]["Normal"] < 6:
                        if mySpice >= 1: 
                            mov = 'movement : action: consolidate_position cu  2'
         if not mov:
            mov=' and no_move.'
         ans=ans+mov
         return ans
        
        
        if phase=="Spice collection" or phase==7:
            return {"action:" "Spice collection.Se face de API."}
        
        if phase=="Mentat pause" or phase==8:
            return {"action:" "Mentat pause.Se face de API."}
        
        if phase=="Nexus" or phase==9:
            return {"action:" "Accept prima alianta oferita."}
        return {"action": "none"}
