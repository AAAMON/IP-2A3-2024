extends Node2D

var selectedArea

@onready var mapImage = $Sprite2D
@onready var regionArea = preload("res://baseGame/region_area.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
	load_regions()



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
	pass
