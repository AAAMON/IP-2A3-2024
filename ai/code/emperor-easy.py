import json

def get_move(game_state):
       
        spice_list = game_state["Map"]["Spice_List"]
        territories = game_state["Map"]["Section_Forces_list"]
        round =game_state["Round"]
        phase =game_state["Phase"]
        mySpice=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Spacing_Guild"]["Spice"]
        deadTroops=game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Spacing_Guild"]["Dead_Troops"]
        off_planet_reserves = game_state["Reserves"]["Spacing_Guild_Forces"]["Forces_Nr"]

        if phase=="Storm" or phase==0:
            return {"action:" "It's Storm phase.Se ocupa api nu voi fi apelat"}
        
        if phase=="Spice Blow and NEXUS" or phase==1:
            return {"action:" "It's Spice Blow and NEXUS phase.Se ocupa api nu voi fi apelat"}
        
        if phase=="“CHOAM Charity" or phase==2:
            if mySpice<2:
                return {"action:" "“CHOAM Charity"}
        
        if phase == "bidding " or phase==3:
             my_spice = game_state["Public_Faction_Knowledge_Manager"]["Public_Faction_Knowledge"]["Emperor"]["Spice"]
    
             min_bid = min(spice_list)
    
             if min_bid <= my_spice:
              return {"action": "bid", "bid_amount": min_bid}
             else:
       
              return {"action": "none"}

            
            
        if phase=="Revival" or phase==4:
            if deadTroops>2:
                if mySpice>2:
                    return {"action:" "Revive 2 ."}
                else:
                    return {"action:" "Revive one force.)"}
            else:
                return {"action:" "No revival."}
        if (mySpice < 5) & (max(spice_list)>5) :
            return {"action": "merg sa colectez spice"}
        
    
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
                if territory["forces"]["Emperor_Forces"]["Forces_Nr"] > 0:
                    if territory["forces"]["Emperor_Forces"]["Forces_Nr"] < 6:
                        if mySpice >= 1: 
                            mov = 'movement : action: consolidate_position cu  2'
         if not mov:
            mov=' and no_move.'
         ans=ans+mov
         return ans
        
        if phase=="Battle" or phase==6:
            return {"action": "Choose leader, weapon, shield and threachery cards"}
        
        if phase=="Spice collection" or phase==7:
            return {"action:" "Spice collection.Se face de API."}
        
        if phase=="Mentat pause" or phase==8:
            return {"action:" "Mentat pause.Se face de API."}
        
        if phase=="Nexus" or phase==9:
            return {"action:" "Accept prima alianta oferita."}
        return {"action": "none"}