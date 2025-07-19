import random
import time

print("Math Generator startet...", flush=True)
while True:
    a = random.randint(1, 10)
    b = random.randint(1, 10)
    
    # WICHTIG: Der Bot-Name muss exakt mit dem --clientName des Bots uebereinstimmen!
    # Format: /msg BotName befehl parameter
    print(f'/msg CalculatorBot exec calculate.ps1 "{a} + {b}"', flush=True)
    
    time.sleep(5)