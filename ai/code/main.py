from fastapi import FastAPI
from BotManager import BotManager
import json


app = FastAPI()
bot_manager = BotManager()
    

def format_json(json_string):
    
    json_string = json_string[:-1]
    json_string = json_string[1:]
    return json_string



@app.get("/check")
def root():
    return {'status': 'ok'}


@app.get("/get-move")
def get_move(bot_name='bg-easy', game_state='rand state'):
    print('got faction ' + bot_name)
    print('got state ' + game_state)

    return bot_manager.get_move(bot_name, game_state)