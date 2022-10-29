import socket
import time
from select import select

SERVER_GUI = False


class Server:
    def __init__(self, ip, port):
        """
        setting up the class of the base server which handles the socket level
        :param ip: str - server ip to bind
        :param port: int - server port to bind
        """
        self.__ip = ip
        self.__port = port

        self.__clients = []
        self.__client_ids = {}
        self.__messages_to_send = []
        self.__setup_socket()

        self.run = False

        self.server_gui_sock = None

    def __setup_socket(self):
        """
        setting up the server socket object
        :return: None
        """
        self.__server_socket = socket.socket(
            socket.AF_INET, socket.SOCK_STREAM)
        self.__server_socket.bind((self.__ip, self.__port))

    def start(self):
        """
        starting the server's socket and mainloop
        :return: None
        """
        self.__server_socket.listen()

        self.__main_loop()

    def close(self):
        """
        closing server socket
        :return: None
        """
        print(f"[SERVER] server closed")
        self.__server_socket.close()

    def __close_client(self, client):
        """
        closing connection to a client
        :param client: socket - client socket object
        :return: None
        """
        print(f"[SERVER] {client.getpeername()} disconnected")
        self.__clients.remove(client)
        client.close()

    def send_message(self, client_id, msg):
        """
        adding message that need to be sent to the message list
        :param client_id: int - id of a player client
        :param msg: str
        :return: None
        """

        client_sock = [sock for sock,
                       _id in self.__client_ids.items() if _id == client_id]
        client_sock = client_sock[0]

        self.__messages_to_send.append((client_sock, msg))

    def send_to_server_gui(self, msg):
        """
        adding the message that need to be sent to the server gui to the message list
        :param msg: str
        :return:
        """

        self.__messages_to_send.append((self.server_gui_sock, msg))

    def send_all(self, msg):
        """
                adding message that need to be sent to the message list for all players
                :param msg: str
                :return: None
                """

        for client_sock in self.__clients:
            if client_sock is not self.server_gui_sock:
                if msg == "GAME_OVER":
                    print(f"game over(2) for {self.__client_ids[client_sock]}")
                self.__messages_to_send.append((client_sock, msg))

    def _handle_data(self, client_id, msg, msg_type="data"):
        """
        method to be overwritten by handler class
        :return: None
        """
        # example - echo and not closing the server
        if msg_type == "data":
            self.send_message(client_id, msg)

    def __main_loop(self):
        """
        server main loop that handles socket with select
        :return: None
        """
        print("server started")
        self.run = True
        # main server loop
        wlist = []
        while self.run:
            rlist, wlist, _ = select(
                self.__clients + [self.__server_socket], self.__clients, [])

            # handling readable sockets
            for sock in rlist:
                # handling new client
                if sock is self.__server_socket:
                    try:
                        new_client, addr = self.__server_socket.accept()
                    except:
                        self.close()
                        return
                    print(f"[SERVER] new connection from {addr}")
                    self.__clients.append(new_client)

                    if SERVER_GUI:
                        if len(self.__clients) > 1 or self.server_gui_sock:
                            self.__client_ids[new_client] = len(self.__clients) - 1

                            self._handle_data(len(self.__clients) - 1, "", msg_type="new_client")
                        else:
                            self.server_gui_sock = new_client
                            print('CONNECTED TO GUI SERVER',
                                self.__clients, self.__client_ids)
                    else:
                        self.__client_ids[new_client] = len(self.__clients)

                        self._handle_data(len(self.__clients), "", msg_type="new_client")

                # handling client request
                else:
                    msg, success = self.__recv_from_socket(sock)

                    if not success:
                        self.__close_client(sock)
                        self._handle_data(
                            self.__client_ids[sock], "", msg_type="client_disconnected")
                    else:
                        self._handle_data(self.__client_ids[sock], msg)
                    if not self.run:
                        # self.close()
                        # return
                        break
            # print("a")
            # while len(self.__messages_to_send) > 0:
            #     print("b")
            self.__send_messages(wlist)

        # for clients to recv last messages
        while len(self.__messages_to_send) > 0:
            self.__send_messages(wlist)
        time.sleep(5)

        self.close()

    def emergency_send_to_all_clients(self):
        data = "SERVER_DISCONNECTED"

        for client in self.__clients:
            try:
                client.send(str(len(data.encode())).zfill(8).encode() + data.encode())
            except:
                pass
        time.sleep(1)

    def __send_messages(self, wlist):
        """
        this function sends the clients messages that are waiting to be sent by the wanted format
        :param wlist: list[socket] - list of sockets that can be send to
        :return: None
        """
        # print("wlist is:", ",".join([f"{self.__client_ids[so]}" for so in wlist]))
        for message in self.__messages_to_send:
            client, data = message

            # if data == "GAME_OVER":
            #     print(f"game over msg for {self.__client_ids[client]}")

            if client not in self.__clients:
                self.__messages_to_send.remove(message)
                continue
            if client in wlist:
                try:
                    # print(f"sending data to client number {self.__client_ids[client]}")
                    client.send(str(len(data.encode())).zfill(
                        8).encode() + data.encode())
                except:
                    print("error")
                    # pass

                self.__messages_to_send.remove(message)

    def __recv_from_socket(self, sock):
        """
        function that receive data from socket by the wanted format
        :param sock: socket
        :return: tuple - (msg/error - str, status(True for ok, False for error))
        """
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
        # this is a fail - safe -> if the recv not giving the msg in one time
        while len(msg) < msg_size:
            try:
                msg_fragment = sock.recv(msg_size - len(msg))
            except:
                return "recv error", False
            if not msg_fragment:
                return "msg data is none", False
            msg = msg + msg_fragment

        msg = msg.decode(errors="ignore")

        return msg, True


# for testing purposes
if __name__ == "__main__":
    s = Server("0.0.0.0", 55555)
    s.start()
