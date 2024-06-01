extends Control


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_cancel_pressed():
	queue_free()


func _on_register_pressed():
		# CHECH IF PASS 1 AND 2 ARE THE SAME DUM DUM
	var username = get_node("NinePatchRect/VBoxContainer/Username")
	var password = get_node("NinePatchRect/VBoxContainer/Password1")
	var mail = get_node("NinePatchRect/VBoxContainer/Email")
	var registerRequest = HTTPRequest.new()
	registerRequest.connect("request_completed", _on_register_completed)
	add_child(registerRequest)
	var error = registerRequest.request(PlayerData.api_url + "register/" + username.get_text() + "/" + password.get_text() + "/" + mail.get_text())
	if error != OK:
		push_error("ERROR: HTTP: LOGIN")

func _on_register_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json["message"] == "ok"):
		print("Registered account!")
		queue_free()
	else:
		print("ERROR: AT REGISTER.")
		
