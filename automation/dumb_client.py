from socket import *
import re
import random

class client:
    def __init__(self) -> None:
        self.sock = socket(AF_INET,SOCK_STREAM)
        self.sock.connect(("localhost", 55555))
        self.get_id_pattern = re.compile(r":([1-9])$")
        self.player_id = int(re.findall(string = self.recv_from_socket()[0], pattern= self.get_id_pattern)[0])
        self.hand = []
        print(f"my id is {self.player_id}")
    

    def recv_from_socket(self):
        sock = self.sock
        try:
            msg_size = sock.recv(8)
        except:
            return "recv error", False
        if not msg_size:
            return "msg length error", False
        try:
            msg_size = int(msg_size)
        except:  # not an integer
            return "msg length error", False

        msg = b''
        while len(msg) < msg_size:  # this is a fail - safe -> if the recv not giving the msg in one time
            try:
                msg_fragment = sock.recv(msg_size - len(msg))
            except:
                return "recv error", False
            if not msg_fragment:
                return "msg data is none", False
            msg = msg + msg_fragment

        msg = msg.decode(errors="ignore")

        return msg, True

    def send_msg(self,data):
        self.sock.send(str(len(data.encode())).zfill(8).encode() + data.encode())

    def game_loop(self):
        data = self.recv_from_socket()[0]
        print(data)
        self.ruler = int(re.findall(string =data ,pattern=self.get_id_pattern)[0])

        cards_string = self.recv_from_socket()[0]
        self.add_cards_to_hand(cards_string)
        if self.ruler == self.player_id:
            self.set_strong_suit()

        data = self.recv_from_socket()[0].split(",")
        cards = data[0]
        teams_and_strong = ",".join(data[1:])
        self.add_cards_to_hand(cards)
        self.get_strong_suit_and_teams(teams_and_strong)
        
        while True:
            self.play_turn()
            if self.handle_end_of_round():
                break


    def add_cards_to_hand(self, cards_string):
        self.hand = []
        # cards_string = self.recv_from_socket()[0]
        print("hello "+cards_string)
        cards = cards_string.split("|")    
        for card in cards:
            self.hand.append(card.split("*"))

    def set_strong_suit(self):
        print("I AM THE RULER")
        suit = random.choice(self.hand)[0]
        self.send_msg(f"set_strong:{suit}")
        if self.recv_from_socket()[0] == "bad":
            self.set_strong_suit()
    
    def get_strong_suit_and_teams(self, data):
        # data = self.recv_from_socket()[0]
        print(data)
        self.strong_suit = re.findall(string = data,pattern=r"strong:(.+?)$")[0]
    
    def handle_end_of_round(self):
        data = self.recv_from_socket()[0]
        print("************* "+data)
        for team_point in re.findall(string = data, pattern= r"scores:(.+?)$")[0].split("|"):
            team,points = team_point.split("*")
            if points == 7:
                print("GAME OVER!!!")
                won = False
                for played_id in team.split("+"):
                    if played_id ==self.player_id:
                        won = True
                if won:
                    print("YOU WON!!!!")
                else:
                    print("you lost :(")
                return True
        return False

    def print_hand(self):
        print("my hand is:")
        for card in self.hand:
            print(f"suit: {card[0]}, rank: {card[1]}")
    
    def play_turn(self):
        self.print_hand()
        status = self.recv_from_socket()[0]
        print(status)
        played_suit = re.findall(string = status, pattern= r"^played_suit:(.{0,}?),")[0]
        cards_played = []
        print(f"-------------> {played_suit}")
        while True:
            played = False
            if played_suit == "," or played_suit == None or played_suit == "null":
                cards_played.append(self.hand.pop(0))
                self.send_msg(f"play_card:{cards_played[-1][0]}*{cards_played[-1][1]}")
                played= True
            if not played:
                for card in self.hand:
                    if card[0] == played_suit:
                        cards_played.append(self.hand.pop(self.hand.index(card)))
                        self.send_msg(f"play_card:{card[0]}*{card[1]}")
                        played = True
            if not played:
                for card in self.hand:
                    if card[0] == self.strong_suit:
                        cards_played.append(self.hand.pop(self.hand.index(card)))
                        self.send_msg(f"play_card:{card[0]}*{card[1]}")
                        played = True
            if not played:
                cards_played.append(self.hand.pop(0))
                self.send_msg(f"play_card:{cards_played[-1][0]}*{cards_played[-1][1]}")
            
            server_respone = self.recv_from_socket()[0]
            print(server_respone)
            if server_respone == "ok":
                for i in range(len(cards_played)-1):
                    self.hand.append(cards_played[i])
                return



c = client()
c.game_loop()