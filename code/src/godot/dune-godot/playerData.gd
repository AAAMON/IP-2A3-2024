extends Node
var api_url = "http://localhost:1234/"
var connected = false


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
# holds the id's for each card
var treatcheryCards = []


func logout():
	username = "Not Logged In..."
	loggedIn = false



