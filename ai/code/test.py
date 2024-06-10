import requests
import json

url2 = "http://127.0.0.1:8000/check"
url = 'http://localhost:8000/get-move-body'
with open("pov-bene-setup.json", "r") as file:
    game_state=json.load(file)
json_obj = {'botname': 'beneGesserit-med', 'jsonString': json.dumps(game_state)}

x = requests.post(url, json=json_obj)

print(x.text)