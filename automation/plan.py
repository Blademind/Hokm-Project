from socket import *

'''
classes to implement, base idea
'''

class game_state:
    def __init__(self):
       pass
    '''
    attr:
        ruler : socket
        strong_card : Card
        points_for_team : Dict{Team,int}
    '''

class round_state:
    def __init__(self):
        pass
    
    '''
    attr:
        cards_played : Dict{Player(tmp),Card}
    '''

class Card:
    '''
    ??
    '''

class game:
    def __init__():
        '''
        get essintial info for game from server
        '''
        pass

    def start_game():
        pass
        '''
        - deal deck to each player
        - seperate into teams
        '''

    def play_round():
        pass
        '''
        get card-played from each player
        validate card
        real-time delay
        '''

class demo_player:
    '''
    ??
    '''

class user_api:
    '''
    user functions:
        get_deck - called once at the start of the round
        get_game_state - can be called whenever
        get_round_state - can be called whenever
        play_card - server needs to manage timing of getting card
                    from each player(need to notify the)
    '''

class team:
    '''
    ??
    '''