extends Control

func _on_back_to_menu_pressed():
	var menuScene = load("res://mainMenu/menu.tscn").instantiate()
	add_child(menuScene)
	
