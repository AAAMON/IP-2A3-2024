class_name MainMenu
extends Control

func _on_exit_button_pressed():
	get_tree().quit()


func _on_login_pressed():
	var loginScene = load("res://authentification/login.tscn").instantiate()
	add_child(loginScene)

func _on_guide_pressed():
	var guideScene = load("res://tutorial/guide.tscn").instantiate()
	add_child(guideScene)
