extends Node
var api_url = "http://localhost:1236/"
var connected = false
var requestCompleted: bool = true

# AUTH DATA ####################################################################
var username : String = "Not Logged In..."
var loggedIn : bool = false
# IN-GAME DATA #################################################################
var turnId : int = -1
var myTurn : bool = false
var faction : int = -1
var spice : int = -1
var lastSpice : int = -1
var forcesReserve : int = -1
var forcesDeployed : int = 0
var forcesDead : int = -1
var leaders = []
var territories = []
var traitors = []
var treatcheryCards = []

# for bid
var whoLastBidded : int = -1
# for spice collect
var oldSpice : int = -1

# hope this will get deleted ###################################################
var myLeaders = []

var leadersAtreides = [
	{"idForApi": 4, "name": "Thufir Hawat", "strength": 5},
	{"idForApi": 3, "name": "Lady Jessica", "strength": 5},
	{"idForApi": 2, "name": "Gurney Halleck", "strength": 4},
	{"idForApi": 1, "name": "Duncan Idaho", "strength": 2},
	{"idForApi": 0, "name": "Dr. Wellington Yueh", "strength": 1}
]
var leadersBene = [
	{"idForApi": 9, "name": "Alia", "strength": 5},
	{"idForApi": 8, "name": "Margot Lady Fenring", "strength": 5},
	{"idForApi": 7, "name": "Mother Ramallo", "strength": 5},
	{"idForApi": 6, "name": "Princess Irulan", "strength": 5},
	{"idForApi": 5, "name": "Wanna Yueh", "strength": 5}
]
var leadersEmperor = [
	{"idForApi": 14, "name": "Hasimir Fenring", "strength": 6},
	{"idForApi": 13, "name": "Capitan Aramsham", "strength": 5},
	{"idForApi": 12, "name": "Caid", "strength": 3},
	{"idForApi": 11, "name": "Burseg", "strength": 3},
	{"idForApi": 10, "name": "Bashar", "strength": 2}
]

var leadersFremen = [
	{"idForApi": 19, "name": "Stilgar", "strength": 7},
	{"idForApi": 18, "name": "Chani", "strength": 6},
	{"idForApi": 17, "name": "Otheym", "strength": 5},
	{"idForApi": 16, "name": "Shadout Mapes", "strength": 3},
	{"idForApi": 15, "name": "Jamis", "strength": 2}
]

var leadersGuild = [
	{"idForApi": 24, "name": "Staban Tuek", "strength": 5},
	{"idForApi": 23, "name": "Master Bewt", "strength": 3},
	{"idForApi": 22, "name": "Esmar Tuek", "strength": 3},
	{"idForApi": 21, "name": "Soo-Soo Sook", "strength": 2},
	{"idForApi": 20, "name": "Guild Rep", "strength": 1}
]

var leadersHarkonnen = [
	{"idForApi": 29, "name": "Feyd-Rautha", "strength": 6},
	{"idForApi": 28, "name": "Beast Rabban", "strength": 4},
	{"idForApi": 27, "name": "Pitter De Vries", "strength": 3},
	{"idForApi": 26, "name": "Capitan Iakin Nefud", "strength": 2},
	{"idForApi": 25, "name": "Umman Kudu", "strength": 1}
]

var faction_dict = {
		-1: "NoFaction",
		1: "Atreides",
		2: "BeneGesserit",
		3: "Emperor",
		4: "Fremen",
		5: "SpaceGuild",
		6: "Harkonnen"
}

var faction_to_goofy_dict = {
	"Atreides" : "Atreides",
	"BeneGesserit" : "Bene_Gesserit",
	"Emperor" : "Emperor",
	"Fremen" : "Fremen",
	"SpaceGuild" : "Spacing_Guild",
	"Harkonnen" : "Harkonnen"
}

var goofy_faction_dict = {
	"Atreides": 1,
	"Bene_Gesserit": 2,
	"Emperor": 3,
	"Fremen": 4,
	"Spacing_Guild": 5,
	"Harkonnen": 6
}

	
var leaders_dict = {
	0: "Dr_Wellington_Yueh",
	1: "Duncan_Idaho",
	2: "Gurney_Halleck",
	3: "Lady_Jessica",
	4: "Thufir_Hawat",
	5: "Wanna_Yueh",
	6: "Princess_Irulan",
	7: "Mother_Ramallo",
	8: "Margot_Lady_Fenring",
	9: "Alia",
	10: "Bashar",
	11: "Burseg",
	12: "Caid",
	13: "Captain_Aramsham",
	14: "Hasimir_Fenring",
	15: "Jamis",
	16: "Shadout_Mapes",
	17: "Otheym",
	18: "Chani",
	19: "Stilgar",
	20: "Guild_Rep",
	21: "Soo-Soo_Sook",
	22: "Esmar_Tuek",
	23: "Master_Bewt",
	24: "Staban_Tuek",
	25: "Umman_Kudu",
	26: "Captain_Iakin_Nefud",
	27: "Piter_de_Vries",
	28: "Beast_Rabban",
	29: "Feyd_Rautha"
}

var treachery = {
	"Crysknife": {"type": "projectile", "mainType": "weapon"},
	"Maula_Pistol": {"type": "projectile", "mainType": "weapon"},
	"Slip_Tip": {"type": "projectile", "mainType": "weapon"},
	"Stunner": {"type": "projectile", "mainType": "weapon"},
	"Chaumas": {"type": "poison", "mainType": "weapon"},
	"Chaumurky": {"type": "poison", "mainType": "weapon"},
	"Ellaca_Drug": {"type": "poison", "mainType": "weapon"},
	"Gom_Jabbar": {"type": "poison", "mainType": "weapon"},
	"Lasgum": {"type": "poison", "mainType": "weapon"},
	"Shield": {"type": "shield", "mainType": "defense"},
	"Snooper": {"type": "shield", "mainType": "defense"},
	"Cheap_Hero": {"type": "leader", "mainType": "special"},
	"Family_Atomics": {"type": "storm", "mainType": "special"},
	"Weather_Control": {"type": "storm", "mainType": "special"},
	"Hajr": {"type": "movement", "mainType": "special"},
	"Karama": {"type": "special", "mainType": "special"},
	"Tleilaxu_Ghola": {"type": "special", "mainType": "special"},
	"Truthtrance": {"type": "special", "mainType": "special"},
	"Baliset": {"type": "worthless", "mainType": "worthless"},
	"Jubba_Cloak": {"type": "worthless", "mainType": "worthless"},
	"Kulon": {"type": "worthless", "mainType": "worthless"},
	"La_La_La": {"type": "worthless", "mainType": "worthless"},
	"Trip_To_Gamont": {"type": "worthless", "mainType": "worthless"}
}

func logout():
	username = "Not Logged In..."
	loggedIn = false



