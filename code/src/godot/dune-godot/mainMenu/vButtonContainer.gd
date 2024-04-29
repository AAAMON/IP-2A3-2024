extends VBoxContainer


# Called when the node enters the scene tree for the first time.
func _ready():
	if (PlayerData.loggedIn == false):
		var joinLobbyButton = get_node("joinLobby")
		var hostLobbyButton = get_node("hostLobby")
		joinLobbyButton.hide()
		hostLobbyButton.hide()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	var joinLobbyButton = get_node("joinLobby")
	var hostLobbyButton = get_node("hostLobby")
	var loginButton = get_node("login")
	if (PlayerData.loggedIn == false):
		joinLobbyButton.hide()
		hostLobbyButton.hide()
		loginButton.show()
	else:
		joinLobbyButton.show()
		hostLobbyButton.show()
		loginButton.hide()
