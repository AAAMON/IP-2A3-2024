extends VBoxContainer

func _ready():
	add_players()
	add_factions()
	var waiting_label = Label.new()
	add_child(waiting_label)
	waiting_label.text = "Waiting for other players..."

func add_players():
	var container = get_node("VBoxContainer2")
	for i in range(6):
		var player_profile = load("res://lobby/player_profile.tscn").instantiate()
		container.add_child(player_profile)
		player_profile.set_player_data("Player #" + str(i+1), "Faction")
		# this seems like something that should be set with information from the server

func add_factions():
	var container = HBoxContainer.new()
	add_child(container)
	var factions = ["Atreides", "Bene Gesserit", "Emperor", "Fremen", "Spacing Guild", "Harkonnen"]
	for faction in factions:
		var btn = Button.new()
		btn.text = faction
		btn.connect("pressed", _on_faction_button_pressed.bind(faction))
		container.add_child(btn)

func _on_faction_button_pressed(faction):
	print("Selected faction: " + faction)
	# set faction to current player and save the data
