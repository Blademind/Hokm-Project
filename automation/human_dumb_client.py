from socket import *
# import re
# import random


class Client:
    def __init__(self):
        self.cards = []
        self.client = socket(AF_INET, SOCK_STREAM)
        self.init_sock()

        self.strong = ""

    def init_sock(self):
        self.client.connect(("localhost", 55555))

    def start_game(self):
        my_id, work = self.recv()
        if not work:
            print(f"error when recv id {my_id}")
            exit()
        my_id = my_id.split(":")[1]
        print(my_id)

        ruler, work = self.recv()
        if not work:
            print(f"error when recv ruler {ruler}")
            exit()

        print(ruler)
        ruler = ruler.split(":")[1]

        first_5_cards, work = self.recv()

        if not work:
            print(f"error when recv 5 cards {first_5_cards}")
            exit()
        first_5_cards = first_5_cards.split("|")
        if ruler == my_id:
            print(*[f"{i}-{card}" for i, card in enumerate(first_5_cards)])
            strong = input("enter strong index: ")
            strong_to_send = first_5_cards[int(strong)].split("*")[0]
            print(strong_to_send)
            self.send(f"set_strong:{strong_to_send}")
            print("after send suit")
            response, work = self.recv()
            print("after recv response")
            if not work:
                print(f"error when recv response strong {response}")
                exit()
            print(response)

        cards_teams_strong, work = self.recv()
        if not work:
            print(f"error when recv cards_teams_strong {cards_teams_strong}")
            exit()
        print(cards_teams_strong)

        data = cards_teams_strong.split(",")

        # if the data in corrupted then request new turn and data
        if len(data) != 3:
            # dummy recv to remove buffer
            _, work = self.recv()

            # request new info
            self.send(f"request_start_info")

            cards_teams_strong, work = self.recv()
            if not work:
                print(f"error when recv cards_teams_strong number 2 {cards_teams_strong}")
                exit()

            # request new turn
            self.send(f"request_turn")
            print("requested new turn")

        cards, teams, strong = cards_teams_strong.split(",")

        self.cards = [(c.split("*")[0], c.split("*")[1]) for c in cards.split("|")]
        print(teams, strong)

        self.strong = strong.split(":")[1]

        self.start_turn()

    def start_turn(self):
        while True:
            status, work = self.recv()
            if not work:
                print(f"error when recv status {status}")
                exit()

            if status == "GAME_OVER":
                print("game over!!!!!!!!!!!!")
                exit()
            elif status == "PLAYER_DISCONNECTED":
                print("player disconnected")
                exit()
            elif status == "SERVER_DISCONNECTED":
                print("SERVER_DISCONNECTED")
                exit()
            print(status)

            print(*[f"{i}-{card}" for i, card in enumerate(self.cards)])
            # index = input("which card to play: ")
            # card = self.cards[int(index)]

            played_suit = status.split(",")[0].split(":")[1]

            card = self.choose_card(played_suit)
            print(card)

            self.send(f"play_card:{'*'.join(card)}")

            response, work = self.recv()

            if not work:
                print(f"error when recv response play {response}")
                exit()
            print("response:", response)

            if response == "SERVER_DISCONNECTED":
                print("SERVER_DISCONNECTED")
                exit()
            elif response == "ok":
                self.cards.remove(card)
            else:
                print("ERROR!!!!!!!!!!")

            game_status, work = self.recv()
            if not work:
                print(f"error when recv game status {game_status}")
                exit()

            if game_status == "PLAYER_DISCONNECTED":
                print("player disconnected")
                exit()
            elif game_status == "SERVER_DISCONNECTED":
                print("SERVER_DISCONNECTED")
                exit()
            print(game_status)

    def choose_card(self, played_suit):
        if played_suit == "":
            return self.cards[0]

        for card in self.cards:
            if card[0] == played_suit:
                return card

        for card in self.cards:
            if card[0] == self.strong:
                return card

        return self.cards[0]

    def recv(self):
        try:
            msg_size = self.client.recv(8)
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
                msg_fragment = self.client.recv(msg_size - len(msg))
            except:
                return "recv error", False
            if not msg_fragment:
                return "msg data is none", False
            msg = msg + msg_fragment

        msg = msg.decode(errors="ignore")

        return msg, True

    def send(self, data):
        self.client.send(str(len(data.encode())).zfill(8).encode() + data.encode())


client = Client()
client.start_game()
