extends Node
var api_url = "http://localhost:1237/"
var connected = false
var requestCompleted: bool = true

# AUTH DATA ####################################################################
var username : String = "Not Logged In..."
var loggedIn : bool = false
# IN-GAME DATA #################################################################
var turnId : int = -1
var faction : String = "noFaction"
var spice : int = -1
var forcesReserve : int = -1
var forcesDeployed : int = -1
var forcesDead : int = -1
var leaders = []
var territories = []
var traitors = []
var treatcheryCards = []


# hope this will get deleted ###################################################
var leadersAtreides = [
	{"idForApi": -1, "name": "Thufir Hawat", "strength": 5},
	{"idForApi": -1, "name": "Lady Jessica", "strength": 5},
	{"idForApi": -1, "name": "Gurney Halleck", "strength": 4},
	{"idForApi": -1, "name": "Duncan Idaho", "strength": 2},
	{"idForApi": -1, "name": "Dr. Wellington Yueh", "strength": 1}
]

func logout():
	username = "Not Logged In..."
	loggedIn = false



