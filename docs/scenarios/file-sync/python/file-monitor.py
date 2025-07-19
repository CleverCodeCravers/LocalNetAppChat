import os
import time
import hashlib

print("File Monitor startet...", flush=True)

watch_dir = "C:/SyncFolder"  # Anpassen!
file_hashes = {}

def get_file_hash(filepath):
    """Berechnet MD5 Hash einer Datei"""
    hasher = hashlib.md5()
    try:
        with open(filepath, 'rb') as f:
            hasher.update(f.read())
        return hasher.hexdigest()
    except:
        return None

def scan_directory():
    """Scannt Verzeichnis nach Änderungen"""
    current_files = {}
    
    for root, dirs, files in os.walk(watch_dir):
        for file in files:
            filepath = os.path.join(root, file)
            file_hash = get_file_hash(filepath)
            if file_hash:
                current_files[filepath] = file_hash
    
    # Neue oder geänderte Dateien
    for filepath, file_hash in current_files.items():
        if filepath not in file_hashes:
            print(f"NEW_FILE: {filepath}", flush=True)
        elif file_hashes[filepath] != file_hash:
            print(f"CHANGED_FILE: {filepath}", flush=True)
        file_hashes[filepath] = file_hash
    
    # Gelöschte Dateien
    for filepath in list(file_hashes.keys()):
        if filepath not in current_files:
            print(f"DELETED_FILE: {filepath}", flush=True)
            del file_hashes[filepath]

# Hauptschleife
while True:
    scan_directory()
    time.sleep(5)  # Alle 5 Sekunden scannen