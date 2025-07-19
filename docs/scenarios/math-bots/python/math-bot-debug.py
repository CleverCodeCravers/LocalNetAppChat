import random
import time
import sys

print("Math Generator Debug-Version startet...", flush=True)
print("Sende Nachrichten an: CalculatorBot", flush=True)
print("-" * 50, flush=True)

counter = 0
while True:
    counter += 1
    a = random.randint(1, 10)
    b = random.randint(1, 10)
    
    # Generiere die Nachricht
    message = f'/msg CalculatorBot exec calculate.ps1 "{a} + {b}"'
    
    # Debug-Ausgabe
    print(f"[{counter}] Sende: {message}", flush=True)
    
    # Alternativ: Sende die Aufgabe als normale Nachricht (nicht als Bot-Befehl)
    # print(f"{a} + {b} = ?", flush=True)
    
    time.sleep(5)