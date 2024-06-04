from fastapi import FastAPI
from BotManager import BotManager
from pydantic import BaseModel
import json


app = FastAPI()
bot_manager = BotManager()

class GameStateObject(BaseModel):
    botname : str
    jsonString: str
    

def format_json(json_string):
    
    json_string = json_string[:-1]
    json_string = json_string[1:]
    return json_string



@app.get("/check")
def root():
    return {'status': 'ok'}


@app.post("/get-move-body")
def get_move(game_state:GameStateObject):

    return bot_manager.get_move(game_state.botname, json.loads(game_state.jsonString))



@app.get("/get-move-header")
def get_move(bot_name="spacingGuild-med", game_state= "perspective3.json"):
    print('got faction ' + bot_name)
    print('got state ' + game_state)
    with open("perspective3.json", "r") as file:
        game_state=json.load(file)

    return bot_manager.get_move(bot_name, game_state)
