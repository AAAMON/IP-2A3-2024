extends Node2D

var selectedArea
var mapInfoRequest
var requestCompleted : bool = true
@onready var timer: Timer = $Timer

@onready var mapImage = $Sprite2D
@onready var regionArea = preload("res://baseGame/region_area.tscn")



var dragging: bool = false
var last_mouse_position: Vector2

# Define zoom limits
const MIN_ZOOM: float = 0.5
const MAX_ZOOM: float = 3.0
const ZOOM_STEP: float = 0.1

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_MIDDLE:
			if event.is_pressed():
				dragging = true
				last_mouse_position = event.position
			else:
				dragging = false
	if event is InputEventMouseMotion:
		if dragging:
			var delta: Vector2 = event.position - last_mouse_position
			position += delta
			last_mouse_position = event.position
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_WHEEL_UP:
			_zoom_in(event.position)
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
			_zoom_out(event.position)
func _zoom_in(mouse_position: Vector2) -> void:
	var new_scale = scale - Vector2(ZOOM_STEP, ZOOM_STEP)
	new_scale = new_scale.clamp(Vector2(MIN_ZOOM, MIN_ZOOM), Vector2(MAX_ZOOM, MAX_ZOOM))
	_apply_zoom(mouse_position, new_scale)

func _zoom_out(mouse_position: Vector2) -> void:
	var new_scale = scale + Vector2(ZOOM_STEP, ZOOM_STEP)
	new_scale = new_scale.clamp(Vector2(MIN_ZOOM, MIN_ZOOM), Vector2(MAX_ZOOM, MAX_ZOOM))
	_apply_zoom(mouse_position, new_scale)

func _apply_zoom(mouse_position: Vector2, new_scale: Vector2) -> void:
	var old_scale = scale
	scale = new_scale
	var offset = (mouse_position - position) * (new_scale - old_scale) / old_scale
	position -= offset
	
# Called when the node enters the scene tree for the first time.
func _ready():
	load_regions()
	# GET PHASE INFO REQUEST ###################################################
	mapInfoRequest = HTTPRequest.new()
	mapInfoRequest.connect("request_completed", _on_map_info_request_completed)
	add_child(mapInfoRequest)
	var error = mapInfoRequest.request(PlayerData.api_url + PlayerData.username + "/get_map_info")
	if error != OK:
		push_error("ERROR: HTTP: GET_MAP_INFO")
	 # Configure and start the timer#############################################
	timer.wait_time = 0.9  # 100 milliseconds
	timer.connect("timeout", _on_timer_timeout)
	timer.start()
	

func _on_timer_timeout():
	if (requestCompleted):
		requestCompleted = false;
		var error = mapInfoRequest.request(PlayerData.api_url + "get_map_info/" + PlayerData.username)
		if error != OK:
			push_error("ERROR: HTTP: GET_MAP_INFO")

func _on_map_info_request_completed(_result, _response_code, _headers, body):
	var response_string = body.get_string_from_utf8()
	var json = JSON.parse_string(response_string)
	if (json == null):
		push_error("ERROR: NULL RESPONSE FROM SERVER")
	else:
		#print(json["forces"])
		MapData.spice = json["spice"];
		MapData.stormPosition = json["storm"];
		MapData.shieldDestroyed = json["shieldDestroyed"]
		MapData.forces = json["forces"]
		map_update_labels()
	requestCompleted = true;

# Function to hide all children and sub-children of type Label
func hide_labels_recursively(node: Node):
	for child in node.get_children():
		if child.is_class("Label") || child.is_class("Sprite2D"):
			child.visible = false
		hide_labels_recursively(child) 

func map_update_labels():
	#var node_name = "Nice/MapLabels/" #+ meridian
	#for region_name in MapData.territories.keys():
		#node_name = "Nice/MapLabels/" + region_name + "/" + region_name + "-spice"
		#var territoryLabel = get_node(node_name)
		#territoryLabel.text = str(MapData.territories[region_name]["spice"])
	# first hide all of them...
	for territory in get_node("Nice/MapLabels").get_children():
		hide_labels_recursively(territory)
	var mySum = 0
	for force in MapData.forces:
		if (force != null):
			var forcesLabelNode = "Nice/MapLabels/" + MapData.territory_dict[MapData.sections_goofy_dict[int(force["GoofySectionId"])]]["origin_sector"] + '/' + MapData.sections_goofy_dict[int(force["GoofySectionId"])] + '/' + str(PlayerData.goofy_faction_dict[force["Faction"]])
			#print(forcesLabelNode)
			get_node(forcesLabelNode).text = str(force["Forces"])# + 'f' + str(PlayerData.goofy_faction_dict[force["Faction"]])
			get_node(forcesLabelNode).show()
			var spriteNode = "Nice/MapLabels/" + MapData.territory_dict[MapData.sections_goofy_dict[int(force["GoofySectionId"])]]["origin_sector"] + '/' + MapData.sections_goofy_dict[int(force["GoofySectionId"])] + '/s' + str(PlayerData.goofy_faction_dict[force["Faction"]])
			if (get_node(spriteNode) != null):
				get_node(spriteNode).show()
			# count my forces
			print(force["Faction"])
			if (force["Faction"] == PlayerData.faction_to_goofy_dict[PlayerData.faction_dict[PlayerData.faction]]):
				mySum = mySum + 1
			PlayerData.forcesDeployed = mySum
	for spice in MapData.spice:
		if (spice != null && spice["Spice"] != 0):
			var spiceLabelNode = "Nice/MapLabels/" + MapData.territory_dict[MapData.sections_goofy_dict[int(spice["GoofySectionId"])]]["origin_sector"] + '/' + MapData.territory_dict[MapData.sections_goofy_dict[int(spice["GoofySectionId"])]]["origin_sector"] + "-spice"
			get_node(spiceLabelNode).text = str(spice["Spice"])
			get_node(spiceLabelNode).show()
			var spriteNode = "Nice/MapLabels/" + MapData.territory_dict[MapData.sections_goofy_dict[int(spice["GoofySectionId"])]]["origin_sector"] + '/s'
			if (get_node(spriteNode) != null):
				get_node(spriteNode).show()
#var traitorsLabel = get_node("playerHUD/buttonExit14/TraitorCards")
	#traitorsLabel.text = ' '
	#for traitor in PlayerData.traitors:
		#if (traitor != null):
			#traitorsLabel.text = traitorsLabel.text + str(traitor) + ' '  #'\n'
	#if (traitorsLabel.text == ' '):
		#traitorsLabel.text = "None"
func load_regions():
	var image = mapImage.get_texture().get_image()
	var pixel_color_dict = get_pixel_color_dict(image)
	var regions_dict = import_file_map_dict("res://assets/newMap/map-dictionary.txt")
	
	for region_color in regions_dict:
		var region  = regionArea.instantiate()
		region.region_name = regions_dict[region_color]
		region.set_name(region.region_name)
		get_node("Regions").add_child(region)
		
		var polygons = get_polygons(image, region_color, pixel_color_dict)
	
		for polygon in polygons:
			var region_collision = CollisionPolygon2D.new()
			var region_polygon = Polygon2D.new()
			
			region_collision.polygon = polygon
			region_polygon.polygon = polygon
			
			region.add_child(region_collision)
			region.add_child(region_polygon)

func get_polygons(image, region_color, pixel_color_dict):
	var targetImage = Image.create(image.get_size().x, image.get_size().y, false, Image.FORMAT_RGBA8)
	for value in pixel_color_dict[region_color]:
		targetImage.set_pixel(value.x, value.y, "#ffffff")
	
	var bitmap = BitMap.new()
	bitmap.create_from_image_alpha(targetImage)
	var polygons = bitmap.opaque_to_polygons(Rect2(Vector2(0,0), bitmap.get_size()), 0.1)
	return polygons
	


func get_pixel_color_dict(image):
	if not image:
		print("Error: Image is null.")
		return {}
	
	var pixel_color_dict = {}
	var width = image.get_width()
	var height = image.get_height()
	
	for y in range(height):
		for x in range(width):
			var pixel_color = "#" + image.get_pixel(x, y).to_html(false)
			if pixel_color not in pixel_color_dict:
				pixel_color_dict[pixel_color] = []
			pixel_color_dict[pixel_color].append(Vector2(x, y))
	
	print("Pixel color dictionary created with ", len(pixel_color_dict), " unique colors.")
	return pixel_color_dict
# Called every frame. 'delta' is the elapsed time since the previous frame.

func import_file_map_dict(filepath):
	var file = FileAccess.open(filepath, FileAccess.READ)
	if file != null:
		return JSON.parse_string(file.get_as_text().replace("_", " "))
	else:
		print("Failed to open file: ", filepath)
		return null
	
func _process(_delta):
	#map_update_labels()
	if (MapData.selectedRegion):
		get_node("SelectedSection").text = MapData.selectedRegion.region_name + ' ' + str(MapData.sections_dict[MapData.selectedRegion.region_name])
	else:
		get_node("SelectedSection").text = "No selected section."
