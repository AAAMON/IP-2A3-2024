import importlib.util
import sys
import string
import secrets

from Bot import Bot

class BotManager:
    
    def load_module(self, source, module_name=None):

        if module_name is None:
            module_name = source[:-3] + 'mod'

        print('loading module from source ' + source)

        spec = importlib.util.spec_from_file_location(module_name, source)
        module = importlib.util.module_from_spec(spec)
        sys.modules[module_name] = module
        spec.loader.exec_module(module)

        return module
    
        
    def get_move(self, bot_name, game_state):

        if bot_name != "bg-easy":
            return "bot not found"
        
        my_module = self.load_module('bg-easy.py')
        return my_module.get_move(game_state)