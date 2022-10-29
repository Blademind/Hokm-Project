import subprocess
import psutil
# import time
wins = 0
failures = 0


def kill(proc_pid):
    process = psutil.Process(proc_pid)
    for proc in process.children(recursive=True):
        proc.kill()
    process.kill()


def run_command(cmd):
    """
    runs cmd command in the command prompt and returns the output
    arg: cmd
    ret: the output of the command
    """
    s_lines = ""
    a = subprocess.Popen(cmd, shell=True, stdout=subprocess.PIPE,
                            stderr=subprocess.PIPE,
                            stdin=subprocess.PIPE)
    for line in iter(a.stdout.readline, ""):
        decoded_line = str(line)
        print(decoded_line)
        if not line:
            break
        if ".and." in decoded_line:
            s_lines += decoded_line
            break
        if "bad_play" in decoded_line:
            return None
        if "card doesn't match round suit while player has matching card" in decoded_line:
            print("BUG!")
            return s_lines
        s_lines += decoded_line

    return s_lines


def run(a):
    global wins, failures
    if a:
        #print(a)
        a = a.replace("'b'", "")
        won_or_failed = a.split("\\r\\n")[-2].split(".and.")
        if int(won_or_failed[0]) == 0 and int(won_or_failed[1]) == 0:
            a = run_command("python automation.py")
            run(a)
        else:
            wins += int(won_or_failed[0])
            failures += int(won_or_failed[1])
            print(f"-----Finished Game {i + 1}----")
    else:
        a = run_command("python automation.py")
        run(a)


n = 200
for i in range(n):
    a = run_command("python automation.py")
    run(a)

print(str(wins * 100 / (wins + failures)) + "%", "won") # percentage
