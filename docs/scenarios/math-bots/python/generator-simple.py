import random
import time

print("Math Generator (Simple) startet...", flush=True)
while True:
    a = random.randint(1, 10)
    b = random.randint(1, 10)
    
    # Sende nur die Aufgabe
    print(f"{a} + {b} = ?", flush=True)
    
    time.sleep(3)