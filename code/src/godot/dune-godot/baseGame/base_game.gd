extends Node

var phase1 = preload("res://phases/phase_1_storm.tscn")
var phase2 = preload("res://phases/phase_2_spice.tscn")
var phase3 = preload("res://phases/phase_3.tscn")
var phase4 = preload("res://phases/phase_4.tscn")
var phase5 = preload("res://phases/phase_5.tscn")
var phase6 = preload("res://phases/phase_6.tscn")
var phase7 = preload("res://phases/phase_7.tscn")
var phase8 = preload("res://phases/phase_8.tscn")
var phase9 = preload("res://phases/phase_9.tscn")
var phase = 1
var instance
# Called when the node enters the scene tree for the first time.
func _ready():
	instance = phase1.instantiate()
	add_child(instance)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if (phase != GameData.phase):
		instance.queue_free();
		phase = GameData.phase
		if (phase == 1):
			instance = phase1.instantiate()
			add_child(instance)
		if (phase == 2):
			instance = phase2.instantiate()
			add_child(instance)
		if (phase == 3):
			instance = phase3.instantiate()
			add_child(instance)
		if (phase == 4):
			instance = phase4.instantiate()
			add_child(instance)
		if (phase == 5):
			instance = phase5.instantiate()
			add_child(instance)
		if (phase == 6):
			instance = phase6.instantiate()
			add_child(instance)
		if (phase == 7):
			instance = phase7.instantiate()
			add_child(instance)
		if (phase == 8):
			instance = phase8.instantiate()
			add_child(instance)
		if (phase == 9):
			instance = phase9.instantiate()
			add_child(instance)
