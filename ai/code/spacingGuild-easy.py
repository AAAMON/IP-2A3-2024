import json
import random

def get_move(game_state):
        spice_list = game_state["Map"]["Spice_List"]
        territories = game_state["Map"]["Section_Forces_list"]
        round =game_state["Round"]
        phase =game_state["Phase"]
        mySpice=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Spacing_Guild"]["Spice"]
        deadTroops=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Spacing_Guild"]["Dead_Troops"]
        storm_sector = game_state["Map"]["Storm_Sector"]
        my_sector=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Spacing_Guild"]["Player_Marker"]
        off_planet_reserves = game_state["Reserves"]["Spacing_Guild_Forces"]["Forces_Nr"]
    

        if phase=="Storm" or phase==1:
            answer=""
            if round==1 and abs(storm_sector-my_sector)==1: 
                #adica practic prima tura cand sunt in apropierea sectorului de cerc al furtunii
                random_number = random.randint(0, 20)
                answer="storm_initializer: "+ str(random_number)+" , traitor: "
            if round==1:
                leaders_desc=["Stilgar","Chani","Feyd-Rautha","Hasimir Fenring","Thufir Hawat","Lady Jessica",
                              "Alia","Margot Lady Fenring","Mother Ramallo","Princess Irulan","Wanna Yueh"
                              "Captain Aramsham","Otheym","Beast Rabban","Gurney Halleck","Caid","Burseg","Shadout Mapes","Piter de Vries"
                              "Captain Iakin Nefud","Jamis","Bashar","Duncan Idaho"
                              "Umman Kudu","Dr. Wellington Yueh"]
                possible_traitors=game_state["Special_Faction_Knowledge"]["Traitors"]
                for traitor in leaders_desc:
                    if traitor in possible_traitors:
                        break
                answer+=traitor #aleg cel mai puternic care nu e in echipa mea;daca toti sunt lideri de la mine il aleg pe ultimul
                return{"action ": answer}
                
            #daca nu sunt in advanced ca altfel decide fremen -va trebui sa iau asta din perspective mai incolo
            player1=game_state['Battle_Wheels']['Item1']['_last_player']
            player2=game_state['Battle_Wheels']['Item1']['_last_player']
            if player1=="spacingGuild" or player2=="spacingGuild":
                random_number = random.randint(1, 3)
                return {"action " "storm_movement ": str(random_number)}
            else:
                return {"action" : "It's Storm phase but I do nothing."}
        
        if phase=="Spice Blow and NEXUS" or phase==2:
            return {"action" : "It's Spice Blow and NEXUS phase.I do nothing except if there will be alliances."}
        
        if phase=="CHOAM Charity" or phase==3:
            if mySpice<2:
                return {"action": "CHOAM Charity"}
        
        if phase=="Bidding" or phase==4: #eventual daca pot mitui cu 1 spice pe arteides sa imi arate cartile
            number_threachery=len(game_state["Special_Faction_Knowledge"]["Treachery_Cards"])
            ans="treachery_cards_number: "+str(number_threachery)
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
                    if number_threachery==3 and round!=1 and mySpice*50/100>=last_bid+1:
                        ans+=" , bid: "+str(last_bid+1)
                    else: 
                        ans+=" , bid: pass"           
            return {"action" :ans}
        
        if phase=="Revival" or phase==5:
            if deadTroops>2:
                if mySpice>10:
                    return {"action ": "revive 3"}
            if deadTroops>1:
                if mySpice>5:
                    return {"action ": "revive 2"}
            if deadTroops>0:
                return {"action ": "revive 1"}
            
            return {"action:" "No revival."}
        
        if phase == "Shipment and movement" or phase == 6: #ship extra tokens (maybe 5) into Tuek’s Sietch.   // 1 spice per force in strongholds
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
                if territory["forces"]["Spacing_Guild_Forces"]["Forces_Nr"] > 0:
                    if territory["forces"]["Spacing_Guild_Forces"]["Forces_Nr"] < 6:
                        if mySpice >= 1: 
                            mov = 'movement : action: consolidate_position cu  2'
         if not mov:
            mov=' and no_move.'
         ans=ans+mov
         return ans
       
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
            
        
        
        if phase=="Spice collection" or phase==8:
            return {"action ": "nothing"}
        
        if phase=="Mentat pause" or phase==9:
            return {"action ": "nothing"}
        
        if phase=="Nexus" or phase==9:
            return {"action " :"accept"}

        
        return {"action ": "none"}
