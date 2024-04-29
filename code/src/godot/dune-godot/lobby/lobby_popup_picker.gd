extends Control

func _on_return_button_pressed():
	get_tree().change_scene_to_file("res://mainMenu/menu.tscn")

# A LOBBY SCENE MUST BE ADDED, WHERE YOU PICK YOUR FACTION
# NORMALLY, WHEN THE ADMIN CLICKS STARTS YOU RECEIVE YOUR GAME SPECIFIC INFO
# BUT FOR NOW YOU WILL RECEIVE IT FROM THIS BUTTON
func _on_join_lobby_button_pressed():
	var playerInfoRequest = HTTPRequest.new()
	playerInfoRequest.connect("request_completed", _on_player_info_request_completed)
	add_child(playerInfoRequest)
	var error = playerInfoRequest.request(PlayerData.api_url + "get_player_data")
	if error != OK:
		push_error("ERROR: HTTP: GET_PLAYER_DATA")

func _on_player_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		print("Got other players data from api")
		PlayerData.turnId = json["turnId"]
		PlayerData.faction = json["faction"]
		PlayerData.spice = json["spice"]
		PlayerData.forcesReserve = json["forcesReserve"]
		PlayerData.forcesDeployed = json["forcesDeployed"]
		PlayerData.forcesDead = json["forcesDead"]
	# if we switch the scene sooner, the request won't be able to finish
	get_tree().change_scene_to_file("res://baseGame/base_game.tscn")


func _on_create_lobby_button_pressed():
	pass # Replace with function body.


func _on_refresh_list_button_pressed():
	get_tree().change_scene_to_file("res://lobby_popup_picker.tscn")


#va trebui de schimbat pentru atunci cand vom avea acces la datele pe care va trebui sa le integram si va fi ceva gen:
#var lobby_data = [
	#{"owner": "Gabi", "name": "X12F52", players": "1/4", "difficulty": "Easy"},
	#{"owner": "Alexia", "name": "AL957e", players": "2/4", "difficulty": "Medium"},
	#etc..
#]

#func create_table(lobby_data):
	#for child in get_children():
		#child.queue_free()
	#
	#for lobby in lobby_data:
		#var row = HBoxContainer.new()
		#add_child(row)
		#create_cell(row, lobby.owner)
		#create_cell(row, lobby.name)
		#create_cell(row, lobby.players)
		#create_cell(row, lobby.difficulty)
#
#func create_cell(parent, text, is_header=false):
	#var panel = Panel.new()
	#parent.add_child(panel)
	#var style = StyleBoxFlat.new()
	#style.border_width_top = 1
	#style.border_width_bottom = 1
	#style.border_width_left = 1
	#style.border_width_right = 1
	#style.border_color = Color(0.8, 0.8, 0.8)
	#if is_header:
		#style.bg_color = Color(0.2, 0.2, 0.2)  
	#panel.add_stylebox_override("panel", style)
	#
	#var label = Label.new()
	#label.text = text
	#label.align = Label.ALIGN_CENTER
	#label.valign = Label.VALIGN_CENTER
	#label.clip_text = true
	#label.rect_min_size = Vector2(100, 30) 
	#panel.add_child(label)
	#panel.rect_min_size = label.rect_min_size
#
#func create_row_for_lobby(lobby):
	#var row = HBoxContainer.new()
	#var owner_cell = create_cell(lobby["owner"])
	#row.add_child(owner_cell)
	#var name_cell = create_cell(lobby["name"])
	#row.add_child(name_cell)
	#var players_cell = create_cell(lobby["players"])
	#row.add_child(players_cell)
	#var difficulty_cell = create_cell(lobby["difficulty"])
	#row.add_child(difficulty_cell)
	#$MarginContainer/VBoxContainer/VBoxContainer2/PanelContainer2/ScrollContainer/VBoxContainer.add_child(row)
	#row.size_flags_horizontal = SizeFlags.ExpandFill    
	#return row

#func populate_table_with_lobbies():
	#for child in $MarginContainer/VBoxContainer/VBoxContainer2/PanelContainer2/ScrollContainer/VBoxContainer.get_children():
		#child.queue_free()
	#for lobby in lobby_data:
		#var new_row = create_row_for_lobby(lobby)


#signal row_selected(lobby_data)
#
#func _ready():
		#self.connect("gui_input", HBoxContainer, "_on_gui_input")
#
#func set_data(data):
	#lobby_data = data
	#
#func _on_gui_input(event):
	#if event is InputEventMouseButton and event.pressed and event.button_index == BUTTON_LEFT:
		#emit_signal("row_selected", lobby_data)
		#
#func _on_Row_row_selected(data):
		#current_lobby = data
#
#var row = create_row_for_lobby(lobby)
#row.connect("row_selected", self, "_on_Row_row_selected")
#
#func _on_JoinLobbyButton_pressed():
	#if current_lobby:
		#join_lobby(current_lobby)
	#else:
		#print("No lobby selected!")
#etc...



