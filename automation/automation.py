import os
import threading
from threading import Thread
import subprocess
from handler import Handler
from dumb_client_new import Client
import sys
import time
from threading import Event
from multiprocessing import Process

wins = 0
failures = 0


def run_command(cmd):
    """
    runs cmd command in the command prompt and returns the output
    arg: cmd
    ret: the output of the command
    """
    return subprocess.Popen([cmd], stdout=subprocess.PIPE,
                            stderr=subprocess.PIPE,
                            stdin=subprocess.PIPE,
                            encoding="utf-8")


def start_handler():
    a = Handler()
    a.start()


def start_client():
    client = Client()
    client.start_game()


def close_process(p):
    p.kill()


def start_our_main_client():
    global wins, failures
    exe = r"C:\Users\alonl\OneDrive\מסמכים\Hokm\UI\Hokm\bin\Debug\netcoreapp3.1\Hokm.exe"
    process = run_command(
        exe)
    for line in process.stdout:
        if "raw_message: round_winner" in line:
            a = line.split(",")[1].split(":")[1].split("|")[0]
            b = line.split(",")[1].split(":")[1].split("|")[1]
            if "7" in a.split("*")[1]:
                failures += 1
            elif "7" in b.split("*")[1]:
                wins += 1
        if "GAME_OVER" in line:
            close_process(process)


def start_our_second_client():
    exe = r"C:\Users\alonl\OneDrive\מסמכים\Hokm\UI\Hokm\bin\Debug\netcoreapp3.1\Hokm.exe"
    process = run_command(
        exe)
    for line in process.stdout:
        if "GAME_OVER" in line:
            close_process(process)


def start_all():
    t = Thread(target=start_handler)
    t.daemon = True
    t.start()
    for i in range(2):
        t1 = Thread(target=start_client)
        t1.daemon = True
        t1.start()
    threading.Thread(target=start_our_second_client).start()
    start_our_main_client()

start_all()
print(wins, ".and.", failures)