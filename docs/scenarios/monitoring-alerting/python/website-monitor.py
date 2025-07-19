import requests
import time

sites = ["https://example.com", "https://google.com"]

print("Website Monitor startet...", flush=True)

while True:
    for site in sites:
        try:
            response = requests.get(site, timeout=5)
            if response.status_code == 200:
                print(f"OK: {site} ist erreichbar", flush=True)
            else:
                print(f"ALERT: {site} returned {response.status_code}", flush=True)
        except:
            print(f"ALERT: {site} ist nicht erreichbar!", flush=True)
    
    time.sleep(60)  # Alle 60 Sekunden