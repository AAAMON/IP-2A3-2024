extends Control

var default_img = "res://Lobby Scenes/Faction_Information_Pictures/unselected.png"
var faction_info

var player_faction
var selected_faction = "unselected"
var player_ready
var is_ready = false

var ai_players_count = 0
var ready_players = 0

func _ready():
	faction_info=get_node("MarginContainer/HBoxContainer/VBoxContainer2/Faction_Information")
	faction_info.texture=load(default_img)
	
	player_faction=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Host Faction")
	player_ready=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Host Ready")
	
	$MarginContainer/HBoxContainer2/Start_Button.disabled = true
	# Only once everyone is ready can this button be pressed
	$MarginContainer/HBoxContainer2/Ready_Button.disabled = true
	# Only once a faction was selected, will readying up work



func _process(_delta):
	count_players()
	if(6-ai_players_count == ready_players):
		$MarginContainer/HBoxContainer2/Start_Button.disabled = false
	else:
		$MarginContainer/HBoxContainer2/Start_Button.disabled = true



func count_players():
	var username = "[AI]"
	var ready_check
	
	ai_players_count=0
	ready_players=0
	
	ready_check=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Host Ready")
	if(ready_check.text == "YES"):
		ready_players = ready_players + 1
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player1.text):
		ai_players_count = ai_players_count + 1
	else:
		ready_check=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player1")
		if(ready_check.text == "YES"):
			ready_players = ready_players + 1
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player2.text):
		ai_players_count = ai_players_count + 1
	else:
		ready_check=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player2")
		if(ready_check.text == "YES"):
			ready_players = ready_players + 1
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player3.text):
		ai_players_count = ai_players_count + 1
	else:
		ready_check=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player3")
		if(ready_check.text == "YES"):
			ready_players = ready_players + 1
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player4.text):
		ai_players_count = ai_players_count + 1
	else:
		ready_check=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player4")
		if(ready_check.text == "YES"):
			ready_players = ready_players + 1
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player5.text):
		ai_players_count = ai_players_count + 1
	else:
		ready_check=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player5")
		if(ready_check.text == "YES"):
			ready_players = ready_players + 1



func _on_atreides_button_mouse_entered():
	faction_info.texture=load("res://Lobby Scenes/Faction_Information_Pictures/atreides.png")

func _on_atreides_button_mouse_exited():
	faction_info.texture=load(default_img)

func _on_atreides_button_pressed():
	default_img="res://Lobby Scenes/Faction_Information_Pictures/atreides.png"
	faction_info.texture=load(default_img)
	selected_faction = "ATREIDES"
	player_faction.text=selected_faction
	$MarginContainer/HBoxContainer2/Ready_Button.disabled = false



func _on_bene_gesserit_button_mouse_entered():
	faction_info.texture=load("res://Lobby Scenes/Faction_Information_Pictures/bene_gesserit.png")

func _on_bene_gesserit_button_mouse_exited():
	faction_info.texture=load(default_img)

func _on_bene_gesserit_button_pressed():
	default_img="res://Lobby Scenes/Faction_Information_Pictures/bene_gesserit.png"
	faction_info.texture=load(default_img)
	selected_faction = "BENE GESSERIT"
	player_faction.text=selected_faction
	$MarginContainer/HBoxContainer2/Ready_Button.disabled = false



func _on_emperor_button_mouse_entered():
	faction_info.texture=load("res://Lobby Scenes/Faction_Information_Pictures/emperor.png")

func _on_emperor_button_mouse_exited():
	faction_info.texture=load(default_img)

func _on_emperor_button_pressed():
	default_img="res://Lobby Scenes/Faction_Information_Pictures/emperor.png"
	faction_info.texture=load(default_img)
	selected_faction = "EMPEROR"
	player_faction.text=selected_faction
	$MarginContainer/HBoxContainer2/Ready_Button.disabled = false



func _on_fremen_button_mouse_entered():
	faction_info.texture=load("res://Lobby Scenes/Faction_Information_Pictures/fremen.png")

func _on_fremen_button_mouse_exited():
	faction_info.texture=load(default_img)

func _on_fremen_button_pressed():
	default_img="res://Lobby Scenes/Faction_Information_Pictures/fremen.png"
	faction_info.texture=load(default_img)
	selected_faction = "FREMEN"
	player_faction.text=selected_faction
	$MarginContainer/HBoxContainer2/Ready_Button.disabled = false



func _on_spacing_guild_button_mouse_entered():
	faction_info.texture=load("res://Lobby Scenes/Faction_Information_Pictures/spacing_guild.png")

func _on_spacing_guild_button_mouse_exited():
	faction_info.texture=load(default_img)

func _on_spacing_guild_button_pressed():
	default_img="res://Lobby Scenes/Faction_Information_Pictures/spacing_guild.png"
	faction_info.texture=load(default_img)
	selected_faction = "SPACING GUILD"
	player_faction.text=selected_faction
	$MarginContainer/HBoxContainer2/Ready_Button.disabled = false



func _on_harkonnen_button_mouse_entered():
	faction_info.texture=load("res://Lobby Scenes/Faction_Information_Pictures/harkonnen.png")

func _on_harkonnen_button_mouse_exited():
	faction_info.texture=load(default_img)

func _on_harkonnen_button_pressed():
	default_img="res://Lobby Scenes/Faction_Information_Pictures/harkonnen.png"
	faction_info.texture=load(default_img)
	selected_faction = "HARKONNEN"
	player_faction.text=selected_faction
	$MarginContainer/HBoxContainer2/Ready_Button.disabled = false



func _on_return_button_pressed():
	get_tree().change_scene_to_file("res://Lobby Scenes/lobby_picker.tscn")



func _on_ready_button_pressed():
	if(selected_faction!="unselected"):
		if(is_ready == false):
			player_ready.text="YES"
			is_ready = true
			ready_players = ready_players + 1
		else:
			is_ready = false
			player_ready.text="NO"


func _on_start_button_pressed():
	pass # scene will change => scene doesn't exist as of 23/04/24 [to change when it's ready]


func _on_add_ai_button_pressed():
	pass # will go over all of the empty slots in the lobby and
	# assign to each one an AI player of an unselected faction


func _on_remove_ai_button_pressed():
	pass # will clear all the slots occupied by AI players
