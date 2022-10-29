from base_classes import *
from linked_list import *


class Game:

    def __init__(self,players,teams):
        self.deck = Deck()
        self.players = players
        self.teams = teams
        self.game_state = GameState(None, self.teams)
        self.round_state = RoundState(None)
        self.winner_tup = (None, None)
        self.game_over = False
        self.game_state = GameState(None, self.teams)
        self.round_state = RoundState(None)
        self.winner_tup = (None, None)
        self.game_over = False
        self.turns = LinkedList()
        self.current_turn = Node(None)
        self.ruler = None
        self.strong_suit = None

    # takes a player and a card, returns (is turn valid, is round over)
    def play_card(self,player:Player,card:Card):
        if not player.player_id == self.current_turn.data.player_id:
            return (False,False)
        '''try:
            print(card)
            print(*player.hand,sep=" , ")
            player.hand.index(card) #checking wether the player has the cards he wants to play
        except ValueError as e:
            print(e)
            return (False, False)'''
        if card not in player.hand:
            return (False, False)
        if self.round_state.played_suit == None:
            self.round_state.played_suit = card.suit
            self.winner_tup = (card, player)
        elif self.round_state.played_suit != card.suit:
            if self.round_state.played_suit in [c.suit for c in player.hand]:
                print(self.round_state.played_suit)
                print(card)
                print(*player.hand,sep=" , ")
                print("card doesn't match round suit while player has matching card")
                return (False, False)
        
        self.round_state.played_cards.append(card)
        self.change_winner(card,player)
        self.current_turn = self.current_turn.next

        player.hand.remove(card)

        if len(self.round_state.played_cards) == 4:
            winning_team = self.increment_points()
            self.round_state.played_cards = []
            self.round_state.played_suit = None
            return (True, winning_team)
        else:
            return (True, False)
    
    def increment_points(self):
        self.current_turn = self.find_current_player_node(self.winner_tup[1])
        for team in self.teams:
            for player in team.players:
                if player.player_id == self.winner_tup[1].player_id:
                    self.game_over = team.add_points()
                    self.game_state.scores[team] += 1
                    return team

    def find_current_player_node(self,player:Player):
        tmp = self.turns.head
        while True:
            if tmp.data.player_id == player.player_id:
                return tmp
            tmp = tmp.next

    def change_winner(self,card:Card,player:Player):
        if card.suit != self.round_state.played_suit and card.suit != self.strong_suit:
            return
        if self.winner_tup[0].suit == self.strong_suit:
            if card.suit == self.strong_suit:
                if card.rank.val > self.winner_tup[0].rank.val:
                    self.winner_tup = (card,player)
            else:
                return
        else:
            if card.suit == self.strong_suit:
                self.winner_tup = (card,player)
            else:
                if card.rank.val > self.winner_tup[0].rank.val:
                    self.winner_tup = (card,player)
            


    def set_strong_suit(self,suit:Suit):
        self.strong_suit = suit
        self.game_state.s_suit = suit
        

    def set_ruler(self,player:Player):
        self.ruler = player
        self.set_turns(player)
        self.current_turn = self.turns.head

    def set_turns(self,player:Player):
        self.turns.head = Node(player)
        opposite_team = None
        own_team = None
        if player in self.teams[0].players:
            opposite_team = self.teams[1]
            own_team = self.teams[0]
        else:
            opposite_team = self.teams[0]
            own_team = self.teams[1]
        teammate = None
        if own_team.players[0].player_id == player.player_id:
            teammate = own_team.players[1]
        else:
            teammate = own_team.players[0]
        player2 = Node(opposite_team.players[0] if player.player_id == 1 or player.player_id == 4 else opposite_team.players[1])
        player3 = Node(teammate)
        player4 = Node(opposite_team.players[1] if player.player_id == 1 or player.player_id == 4 else opposite_team.players[0])
        self.turns.head.next = player2
        player2.next = player3
        player3.next = player4
        player4.next = self.turns.head

    def hand_cards_to_player(self,player:Player):
        if len(player.hand)==0:
            for i in range(5):
                player.hand.append(self.deck.draw_card())
        elif len(player.hand) == 5:
            for i in range(8):
                player.hand.append(self.deck.draw_card())
        '''
        if player doesnt have cards, deal 5, if player does have cards, deal another 8
        '''

    def hand_cards_for_all(self):
        for player in self.players:
            self.hand_cards_to_player(player)

    def get_round_state(self) -> RoundState:
        return self.round_state
        
    def get_game_state(self) -> GameState:
        return self.game_state

    def get_current_player_turn(self) -> Player:
        return self.current_turn.data
        '''
        return the player's instance that has to play next
        keeping track of turns will be done via linked list
        '''

def test_game():
    player1 = Player()
    player2 = Player()
    player3 = Player()
    player4 = Player()
    team1 = Team([player1,player2])
    team2 = Team([player3,player4])
    igame = Game([player1,player2,player3,player4],[team1,team2])
    print(len(igame.players))
    igame.set_ruler(player1)
    igame.set_strong_suit(Suit(1))
    for i in range(2):
        for player in igame.players:
            igame.hand_cards_to_player(player)
    
    for player in igame.players:
        print(f"playerid {player.player_id} ------------------")
        for card in player.hand:
            print(f"suit {card.suit}, rank {card.rank}")
    
    print(igame.play_card(player1,player1.hand[1]))
    print(igame.current_turn.data.player_id)
    print(igame.play_card(player2,player2.hand[1]))    
    print(igame.play_card(player3,player3.hand[1]))
