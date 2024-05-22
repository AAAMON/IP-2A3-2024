extends Node



# MAP DATA #####################################################################
var selectedRegion


# Define the dictionary with zones and their properties
var territories = {
	"funeral-plain": {"spice": 2, "forces": 3},
	"the-great-flat": {"spice": 3, "forces": 4},
}

var territorySpice = [
	{"sector": "funeral-plain", "spice": "4"},
	{"sector": "the-great-flat", "spice": "2"}
]

var territoryForces = [
	{"name": "the-great-flat", "forces": {"harkonnen": 4, "space_guild": 2}},
	{"name": "funeral-plain", "forces": {"atreides": 4, "emperor": 10}}
]

var territoryData = [
	{"name": "meridian-1", "origin_sector": "meridian", "neighbours": [1, 3]},
	{"name": "cielago-west-1", "origin_sector": "cielago-west", "neighbours": [1, 3]},
	{"name": "cielago-depression-1", "origin_sector": "cielago-depression", "neighbours": [1, 3]},
	{"name": "cielago-north-1", "origin_sector": "cielago-north", "neighbours": [1, 3]},
	{"name": "meridian-2", "origin_sector": "meridian", "neighbours": [1, 3]},
	{"name": "cielago-south-2", "origin_sector": "cielago-south", "neighbours": [1, 3]},
	{"name": "cielago-depression-2", "origin_sector": "cielago-depression", "neighbours": [1, 3]},
	{"name": "cielago-north-2", "origin_sector": "cielago", "neighbours": [1, 3]},
	{"name": "cielago-south-3", "origin_sector": "cielago", "neighbours": [1, 3]},
	{"name": "cielago-east-3", "origin_sector": "cielago", "neighbours": [1, 3]},
	{"name": "cielago-depression-3", "origin_sector": "cielago-depression", "neighbours": [1, 3]},
	{"name": "cielago-north-3", "origin_sector": "cielago", "neighbours": [1, 3]},
	{"name": "cielago-east-4", "origin_sector": "cielago", "neighbours": [1, 3]},
	{"name": "south-mesa-4", "origin_sector": "mesa", "neighbours": [1, 3]},
	{"name": "false-wall-south-4", "origin_sector": "false_wall", "neighbours": [1, 3]},
	{"name": "harg-pass-4", "origin_sector": "harg_pass", "neighbours": [1, 3]},
	{"name": "south-mesa-5", "origin_sector": "mesa", "neighbours": [1, 3]},
	{"name": "tueks-sietch", "origin_sector": "sietch", "neighbours": [1, 3]},
	{"name": "pasty-mesa-5", "origin_sector": "mesa", "neighbours": [1, 3]},
	{"name": "false-wall-south-5", "origin_sector": "false_wall", "neighbours": [1, 3]},
	{"name": "harg-pass-5", "origin_sector": "harg_pass", "neighbours": [1, 3]},
	{"name": "minor-erg-5", "origin_sector": "erg", "neighbours": [1, 3]},
	{"name": "false-wall-east-5", "origin_sector": "false_wall", "neighbours": [1, 3]},
	{"name": "south-mesa-6", "origin_sector": "mesa", "neighbours": [1, 3]},
	{"name": "pasty-mesa-6", "origin_sector": "mesa", "neighbours": [1, 3]},
	{"name": "minor-erg-6", "origin_sector": "erg", "neighbours": [1, 3]},
	{"name": "false-wall-east-6", "origin_sector": "false_wall", "neighbours": [1, 3]},
	{"name": "ked-chasm", "origin_sector": "chasm", "neighbours": [1, 3]},
	{"name": "pasty-mesa-7", "origin_sector": "mesa", "neighbours": [1, 3]},
	{"name": "minor-erg-7", "origin_sector": "erg", "neighbours": [1, 3]},
	{"name": "false-wall-east-7", "origin_sector": "false_wall", "neighbours": [1, 3]},
	{"name": "gara-kulon", "origin_sector": "gara", "neighbours": [1, 3]},
	{"name": "pasty-mesa-8", "origin_sector": "mesa", "neighbours": [1, 3]},
	{"name": "shield-wall-8", "origin_sector": "shield_wall", "neighbours": [1, 3]},
	{"name": "minor-erg-8", "origin_sector": "erg", "neighbours": [1, 3]},
	{"name": "false-wall-east-8", "origin_sector": "false_wall", "neighbours": [1, 3]},
	{"name": "sihaya-ridge", "origin_sector": "ridge", "neighbours": [1, 3]},
	{"name": "basin", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "old-gap-9", "origin_sector": "gap", "neighbours": [1, 3]},
	{"name": "rim-wall-west", "origin_sector": "rim_wall", "neighbours": [1, 3]},
	{"name": "hole-in-rock", "origin_sector": "hole", "neighbours": [1, 3]},
	{"name": "shield-wall-9", "origin_sector": "shield_wall", "neighbours": [1, 3]},
	{"name": "imperial-basin-9", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "false-wall-east-9", "origin_sector": "false_wall", "neighbours": [1, 3]},
	{"name": "old-gap-10", "origin_sector": "gap", "neighbours": [1, 3]},
	{"name": "arrakeen", "origin_sector": "arrakeen", "neighbours": [1, 3]},
	{"name": "imperial-basin-10", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "broken-land-11", "origin_sector": "broken_land", "neighbours": [1, 3]},
	{"name": "old-gap-11", "origin_sector": "gap", "neighbours": [1, 3]},
	{"name": "tsimpo-11", "origin_sector": "tsimpo", "neighbours": [1, 3]},
	{"name": "imperial-basin-11", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "carthag", "origin_sector": "carthag", "neighbours": [1, 3]},
	{"name": "arsunt-11", "origin_sector": "arsunt", "neighbours": [1, 3]},
	{"name": "broken-land-12", "origin_sector": "broken_land", "neighbours": [1, 3]},
	{"name": "plastic-basin-12", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "tsimpo-12", "origin_sector": "tsimpo", "neighbours": [1, 3]},
	{"name": "hagga-basin-12", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "arsunt-12", "origin_sector": "arsunt", "neighbours": [1, 3]},
	{"name": "rock-outcroppings-13", "origin_sector": "rock", "neighbours": [1, 3]},
	{"name": "plastic-basin-13", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "tsimpo-13", "origin_sector": "tsimpo", "neighbours": [1, 3]},
	{"name": "hagga-basin-13", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "rock-outcroppings-14", "origin_sector": "rock", "neighbours": [1, 3]},
	{"name": "bight-cliff-14", "origin_sector": "cliff", "neighbours": [1, 3]},
	{"name": "sietch-tabr", "origin_sector": "sietch", "neighbours": [1, 3]},
	{"name": "plastic-basin-14", "origin_sector": "basin", "neighbours": [1, 3]},
	{"name": "wind-pass-14", "origin_sector": "pass", "neighbours": [1, 3]},
	{"name": "bight-cliff-15", "origin_sector": "cliff", "neighbours": [1, 3]},
	{"name": "funeral-plain", "origin_sector": "funeral_plain", "neighbours": [1, 3]},
	{"name": "great-flat", "origin_sector": "great_flat", "neighbours": [1, 3]},
	{"name": "wind-pass-15", "origin_sector": "wind_pass", "neighbours": [1, 3]},
	{"name": "greater-flat", "origin_sector": "greater_flat", "neighbours": [1, 3]},
	{"name": "wind-pass-16", "origin_sector": "wind_pass", "neighbours": [1, 3]},
	{"name": "habbanya-erg-16", "origin_sector": "habbanya_erg", "neighbours": [1, 3]},
	{"name": "false-wall-west-16", "origin_sector": "false_wall_west", "neighbours": [1, 3]},
	{"name": "wind-pass-north-17", "origin_sector": "wind_pass_north", "neighbours": [1, 3]},
	{"name": "wind-pass-17", "origin_sector": "wind_pass", "neighbours": [1, 3]},
	{"name": "false-wall-west-17", "origin_sector": "false_wall_west", "neighbours": [1, 3]},
	{"name": "habbanya-erg-17", "origin_sector": "habbanya_erg", "neighbours": [1, 3]},
	{"name": "habbanya-ridge-flat-17", "origin_sector": "habbanya_ridge_flat", "neighbours": [1, 3]},
	{"name": "habbanya-sietch", "origin_sector": "habbanya_sietch", "neighbours": [1, 3]},
	{"name": "wind-pass-north-18", "origin_sector": "wind_pass_north", "neighbours": [1, 3]},
	{"name": "cielago-west-18", "origin_sector": "cielago_west", "neighbours": [1, 3]},
	{"name": "false-wall-west-18", "origin_sector": "false_wall_west", "neighbours": [1, 3]},
	{"name": "habbanya-ridge-flat-18", "origin_sector": "habbanya_ridge_flat", "neighbours": [1, 3]},
	{"name": "polar-sink", "origin_sector": "polar_sink", "neighbours": [1, 3]}
]






