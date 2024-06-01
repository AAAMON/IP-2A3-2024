extends Control

var player_name = ""
var player_faction = ""

@onready var name_label = get_node("HBoxContainer/NameLabel")
@onready var faction_label = get_node("HBoxContainer/FactionLabel")

func set_player_data(name_input, faction_input):
	player_name = name_input
	player_faction = faction_input
	name_label.text = name_input
	faction_label.text = faction_input
