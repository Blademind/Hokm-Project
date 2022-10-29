from enum import Enum
from operator import truediv
import random

class Suit(Enum):
    SPADES = 1
    CLUBS = 2
    DIAMONDS = 3
    HEARTS = 4

    def __init__(self,val) -> None:
        super().__init__()
        self.val = val

class Rank(Enum):
    rank_2=1
    rank_3=2
    rank_4=3
    rank_5=4
    rank_6=5
    rank_7=6
    rank_8=7
    rank_9=8
    rank_10=9
    rank_J=10
    rank_Q=11
    rank_K=12
    rank_A=13
    
    def __init__(self,val) -> None:
        super().__init__()
        self.val = val

class Card:
    def __init__(self,suit:Suit,rank:Rank):
        self.suit = suit
        self.rank=rank

    def __str__(self):
        return f"{self.suit.name}*{self.rank.name}"
    
    def __eq__(self,other):
        if self.rank == other.rank and self.suit == other.suit:
            return True
        return False

class Deck:
    def __init__(self):
        self.cards = []
        self.create_deck()
        random.shuffle(self.cards)

    def create_deck(self):
        self.cards = []
        for i in range(4):
            for j in range(13):
                self.cards.append(Card(Suit(i+1),Rank(j+1)))

    def draw_card(self) ->Card:
        return self.cards.pop(0)

class Player:
    player_id = 1

    def __init__(self):
        self.player_id = Player.player_id
        Player.player_id+=1
        self.hand = []

    def add_card(self,card:Card):
        self.hand.append(card)
    
    def add_cards(self,cards):
        for card in cards:
            self.hand.append(card)
    
    def remove_card(self,card:Card):
        self.hand.remove(card)

class Team:
    def __init__(self,players):
        self.players = players
        self.points = 0

    def check_if_player_in_team(self,player:Player) -> bool:
        found = False
        for i in self.players:
            if i.player_id==player.player_id:
                found = True
        return found

    def add_points(self) -> bool:
        self.points+=1
        if self.points == 7:
            return True
        return False

    def __str__(self):  # print like this for player id 1 and 3 -> "1+3"
        return "+".join([str(player.player_id) for player in self.players])

class RoundState:
    def __init__(self,suit:Suit):
        self.played_suit = suit
        self.played_cards = []
    
class GameState:
    def __init__(self, s_suit:Suit, teams):
        self.s_suit = s_suit
        self.teams = teams
        self.scores = {team: 0 for team in teams}
