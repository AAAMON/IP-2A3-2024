from fastapi import FastAPI
from BotManager import BotManager
from pydantic import BaseModel
import json


app = FastAPI()
bot_manager = BotManager()

class GameStateObject(BaseModel):
    jsonString: str
    

def format_json(json_string):
    
    json_string = json_string[:-1]
    json_string = json_string[1:]
    return json_string



@app.get("/check")
def root():
    return {'status': 'ok'}


@app.get("/get-move-body")
def get_move(bot_name:str, game_state: GameStateObject):
    print('got faction ' + bot_name)
    print('got state ' + game_state)

    return bot_manager.get_move(bot_name, json.loads(game_state.jsonString))


@app.get("/get-move-header")
def get_move(bot_name:str, game_state: str):
    print('got faction ' + bot_name)
    print('got state ' + game_state)

    return bot_manager.get_move(bot_name, json.loads(game_state))
