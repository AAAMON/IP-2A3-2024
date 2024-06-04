extends Node
var api_url = "http://localhost:1236/"
var connected = false
var requestCompleted: bool = true

# AUTH DATA ####################################################################
var username : String = "Not Logged In..."
var loggedIn : bool = false
# IN-GAME DATA #################################################################
var turnId : int = -1
var faction : int = -1
var spice : int = -1
var forcesReserve : int = -1
var forcesDeployed : int = -1
var forcesDead : int = -1
var leaders = []
var territories = []
var traitors = []
var treatcheryCards = []


# hope this will get deleted ###################################################
var myLeaders = []

var leadersAtreides = [
	{"idForApi": -1, "name": "Thufir Hawat", "strength": 5},
	{"idForApi": -1, "name": "Lady Jessica", "strength": 5},
	{"idForApi": -1, "name": "Gurney Halleck", "strength": 4},
	{"idForApi": -1, "name": "Duncan Idaho", "strength": 2},
	{"idForApi": -1, "name": "Dr. Wellington Yueh", "strength": 1}
]
var leadersBene = [
	{"idForApi": -1, "name": "Alia", "strength": 5},
	{"idForApi": -1, "name": "Margot Lady Fenring", "strength": 5},
	{"idForApi": -1, "name": "Mother Ramallo", "strength": 5},
	{"idForApi": -1, "name": "Princess Irulan", "strength": 5},
	{"idForApi": -1, "name": "Wanna Yueh", "strength": 5}
]
var leadersEmperor = [
	{"idForApi": -1, "name": "Hasimir Fenring", "strength": 6},
	{"idForApi": -1, "name": "Capitan Aramsham", "strength": 5},
	{"idForApi": -1, "name": "Caid", "strength": 3},
	{"idForApi": -1, "name": "Burseg", "strength": 3},
	{"idForApi": -1, "name": "Bashar", "strength": 2}
]

var leadersFremen = [
	{"idForApi": -1, "name": "Stilgar", "strength": 7},
	{"idForApi": -1, "name": "Chani", "strength": 6},
	{"idForApi": -1, "name": "Otheym", "strength": 5},
	{"idForApi": -1, "name": "Shadout Mapes", "strength": 3},
	{"idForApi": -1, "name": "Jamis", "strength": 2}
]

var leadersGuild = [
	{"idForApi": -1, "name": "Staban Tuek", "strength": 5},
	{"idForApi": -1, "name": "Master Bewt", "strength": 3},
	{"idForApi": -1, "name": "Esmar Tuek", "strength": 3},
	{"idForApi": -1, "name": "Soo-Soo Sook", "strength": 2},
	{"idForApi": -1, "name": "Guild Rep", "strength": 1}
]

var leadersHarkonnen = [
	{"idForApi": -1, "name": "Feyd-Rautha", "strength": 6},
	{"idForApi": -1, "name": "Beast Rabban", "strength": 4},
	{"idForApi": -1, "name": "Pitter De Vries", "strength": 3},
	{"idForApi": -1, "name": "Capitan Iakin Nefud", "strength": 2},
	{"idForApi": -1, "name": "Umman Kudu", "strength": 1}
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

func logout():
	username = "Not Logged In..."
	loggedIn = false



