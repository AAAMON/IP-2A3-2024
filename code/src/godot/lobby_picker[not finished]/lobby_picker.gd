class_name LobbyPicker
extends Control

func _on_return_button_pressed():
	if Input.is_action_pressed("ui_accept"):
		var menu_scene = preload("menu.tscn")
		get_tree().change_scene(menu_scene)
