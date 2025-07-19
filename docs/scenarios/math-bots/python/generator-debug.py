import random
import time
import datetime

print("Math Generator DEBUG startet...", flush=True)
print(f"Zeit: {datetime.datetime.now()}", flush=True)
print("Bot-Name: CalcBot", flush=True)
print("-" * 50, flush=True)

counter = 0
while True:
    counter += 1
    a = random.randint(1, 10)
    b = random.randint(1, 10)
    
    # Zeige was gesendet wird
    message = f'/msg CalcBot exec calculate.ps1 "{a} + {b}"'
    timestamp = datetime.datetime.now().strftime("%H:%M:%S")
    
    print(f"[{timestamp}] #{counter} Sende: {message}", flush=True)
    
    time.sleep(5)