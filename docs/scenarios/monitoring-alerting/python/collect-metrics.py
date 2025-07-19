import time
import random
import psutil  # pip install psutil

print("Metrics Collector startet...", flush=True)

while True:
    # CPU-Auslastung
    cpu_percent = psutil.cpu_percent(interval=1)
    
    # Memory-Auslastung
    memory = psutil.virtual_memory()
    memory_percent = memory.percent
    
    # Disk-Auslastung
    disk = psutil.disk_usage('/')
    disk_percent = disk.percent
    
    # Metriken ausgeben
    print(f"METRIC cpu={cpu_percent:.1f} memory={memory_percent:.1f} disk={disk_percent:.1f}", flush=True)
    
    # Warnung bei hoher Auslastung
    if cpu_percent > 80:
        print(f"ALERT: CPU Auslastung kritisch: {cpu_percent}%", flush=True)
    
    if memory_percent > 90:
        print(f"ALERT: Memory Auslastung kritisch: {memory_percent}%", flush=True)
    
    time.sleep(10)  # Alle 10 Sekunden