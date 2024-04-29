class_name LobbyCreator
extends Control

var lobby_name = ""
var selected_difficulty = "normal"


func _on_normal_difficulty_button_pressed():
	selected_difficulty = "normal"
	$MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/Normal_Difficulty_Button.modulate = Color(1, 100/255.0, 0)
	$MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/Hard_Difficulty_Button.modulate = Color(1, 1, 1)

func _on_hard_difficulty_button_pressed():
	selected_difficulty = "hard"
	$MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/Hard_Difficulty_Button.modulate = Color(1, 100/255.0, 0)
	$MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/Normal_Difficulty_Button.modulate = Color(1, 1, 1)

func _on_create_button_pressed():
	lobby_name = $MarginContainer/HBoxContainer/VBoxContainer/Lobby_Name_Input.text
	get_tree().change_scene_to_file("res://lobby_host.tscn")
	# add functionality: send over the data created

func _on_return_button_pressed():
	get_tree().change_scene_to_file("res://mainMenu/menu.tscn")

func _ready():
	_on_normal_difficulty_button_pressed()
	# considers "Normal" as the default difficulty that is chosen
