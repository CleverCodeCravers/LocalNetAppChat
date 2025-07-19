import time
import random

print("Simple Monitor startet...", flush=True)

while True:
    # Simulierte Metriken
    cpu = random.uniform(20, 95)
    memory = random.uniform(40, 98)
    disk = random.uniform(30, 85)
    
    print(f"METRIC cpu={cpu:.1f} memory={memory:.1f} disk={disk:.1f}", flush=True)
    
    # Zufällige Alerts
    if cpu > 80:
        print(f"ALERT: CPU Auslastung hoch: {cpu:.1f}%", flush=True)
    
    time.sleep(5)