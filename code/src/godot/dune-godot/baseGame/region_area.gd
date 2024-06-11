extends Area2D

var region_name = ""
# Called when the node enters the scene tree for the first time.
func _ready():
	pass

func _process(delta):
	if (GameData.phase == "Storm"): 
		if (MapData.sector_dict[region_name] == MapData.stormPosition+1):
						#print(region_name)
						#print(str(MapData.stormPosition+1))
			for node in get_children():
				if node.is_class("Polygon2D"):
					node.color = Color(0.8,0.3,0.3, 0.2)
		else:
			for node in get_children():
				if node.is_class("Polygon2D"):
					node.color =Color(0,0,0,0)
# Called every frame. 'delta' is the elapsed time since the previous frame.




func _on_child_entered_tree(node):
	if node.is_class("Polygon2D"):
		if (MapData.sector_dict[region_name] == MapData.stormPosition+1):
			node.color = Color(0.8,0.3,0.3, 0.2)
		else:
			node.color =Color(0,0,0,0)


func _on_mouse_entered():
	#print(region_name)
	if (MapData.selectedRegion == null || MapData.selectedRegion.region_name != region_name):
		for node in get_children():
			if node.is_class("Polygon2D"):
				node.color = Color(0.8,0.1,0.6, 0.2)
	if ((MapData.selectedRegion == null || MapData.selectedRegion.region_name != region_name) && MapData.sector_dict[region_name] == MapData.stormPosition+1):
		#print(region_name)
		#print(str(MapData.stormPosition+1))
		for node in get_children():
			if node.is_class("Polygon2D"):
				node.color = Color(0.8,0.3,0.3, 0.2)


func _on_input_event(viewport, event, shape_idx):
	if event is InputEventMouse and event.button_mask == MOUSE_BUTTON_MASK_LEFT and event.is_pressed():
		if (MapData.selectedRegion == null || MapData.selectedRegion.region_name != region_name):
			if (MapData.selectedRegion != null):
				# deselect previous selected region
				var prevRegion = MapData.selectedRegion
				print("Deselected " + str(MapData.selectedRegion.region_name))
				MapData.selectedRegion = null
				
				if (MapData.sector_dict[region_name] == MapData.stormPosition+1):
					#print(region_name)
					#print(str(MapData.stormPosition+1))
					for node in prevRegion.get_children():
						if node.is_class("Polygon2D"):
							node.color = Color(0.8,0.3,0.3, 0.2)
				else:
					for node in prevRegion.get_children():
						if node.is_class("Polygon2D"):
							node.color =Color(0,0,0,0)
			MapData.selectedRegion = self
			print("Selected " + str(MapData.selectedRegion))
			for node in get_children():
				if node.is_class("Polygon2D"):
					node.color = Color(0.3,0.1,0.1, 0.7)
		else:
			print("Deselected " + str(MapData.selectedRegion.region_name))
			MapData.selectedRegion = null
			for node in get_children():
				if node.is_class("Polygon2D"):
					node.color = Color(0.8,0.1,0.6, 0.2)


func _on_mouse_exited():
	if (MapData.selectedRegion == null || MapData.selectedRegion.region_name != region_name):
		if (MapData.sector_dict[region_name] == MapData.stormPosition+1):
					#print(region_name)
					#print(str(MapData.stormPosition+1))
			for node in get_children():
				if node.is_class("Polygon2D"):
					node.color = Color(0.8,0.3,0.3, 0.2)
		else:
			for node in get_children():
				if node.is_class("Polygon2D"):
					node.color =Color(0,0,0,0)
