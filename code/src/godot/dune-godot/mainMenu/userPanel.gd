extends HBoxContainer


# Called when the node enters the scene tree for the first time.
func _ready():
	var usernameLabel = get_node("username")
	usernameLabel.text = PlayerData.username
	if (PlayerData.loggedIn == false):
		var logoutButton = get_node("logout")
		logoutButton.hide()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	var usernameLabel = get_node("username")
	usernameLabel.text = PlayerData.username
	var logoutButton = get_node("logout")
	if (PlayerData.loggedIn == false):
		logoutButton.hide()
	else:
		logoutButton.show()


func _on_logout_pressed():
	PlayerData.logout()
