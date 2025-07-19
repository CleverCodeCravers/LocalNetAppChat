import random
import time

print("Math Generator mit Bot-Kommunikation startet...", flush=True)
print("Sende Befehle an CalculatorBot", flush=True)
print("-" * 50, flush=True)

while True:
    a = random.randint(1, 10)
    b = random.randint(1, 10)
    
    # WICHTIG: Bot-Name muss mit --clientName beim Bot-Start uebereinstimmen!
    print(f'/msg CalculatorBot exec calculate.ps1 "{a} + {b}"', flush=True)
    
    time.sleep(5)