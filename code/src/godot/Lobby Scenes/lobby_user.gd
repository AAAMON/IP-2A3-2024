extends Control

var default_img = "res://Lobby Scenes/Faction_Information_Pictures/unselected.png"
var faction_info

var username = ""
var player_faction
var selected_faction = "unselected"
var player_ready
var is_ready = false

func _ready():
	faction_info=get_node("MarginContainer/HBoxContainer/VBoxContainer2/Faction_Information")
	faction_info.texture=load(default_img)
	
	username=$MarginContainer/HBoxContainer3/Username.text
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player1.text):
		player_faction=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Faction_Player1")
		player_ready=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player1")
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player2.text):
		player_faction=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Faction_Player2")
		player_ready=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player2")
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player3.text):
		player_faction=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Faction_Player3")
		player_ready=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player3")
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player4.text):
		player_faction=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Faction_Player4")
		player_ready=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player4")
	
	if(username == $MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Username_Player5.text):
		player_faction=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Faction_Player5")
		player_ready=get_node("MarginContainer/HBoxContainer/VBoxContainer/GridContainer/Ready_Player5")
	
	$MarginContainer/HBoxContainer2/Start_Button.disabled = true
	# User cannot click on the Start button
	$MarginContainer/HBoxContainer2/Ready_Button.disabled = true
	# Only once a faction was selected, will readying up work



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
		else:
			is_ready = false
			player_ready.text="NO"
