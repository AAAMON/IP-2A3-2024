extends Node


var socket = WebSocketPeer.new()
var server_url = "ws://localhost:8080/"
var connectedSocket : bool = false

func _ready():
	var menuScene = load("res://mainMenu/menu.tscn").instantiate()
	add_child(menuScene)
	# maybe this will need to be moved, for now no problems
	var result = socket.connect_to_url(server_url)



func _process(delta):
	if (PlayerData.loggedIn):
		socket.poll()
		var state = socket.get_ready_state()
		if connectedSocket == false && state == WebSocketPeer.STATE_OPEN:
			socket.send_text(PlayerData.username)
			connectedSocket = true;
		elif state == WebSocketPeer.STATE_CLOSING:
			# Keep polling to achieve proper close.
			pass
		elif state == WebSocketPeer.STATE_CLOSED:
			var code = socket.get_close_code()
			var reason = socket.get_close_reason()
			print("WebSocket closed with code: %d, reason %s. Clean: %s" % [code, reason, code != -1])
			set_process(false) # Stop processing.
