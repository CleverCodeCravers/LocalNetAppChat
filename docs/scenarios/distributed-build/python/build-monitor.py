import re
import json
from datetime import datetime

class BuildMonitor:
    def __init__(self):
        self.builds = {}
        self.completed = 0
        self.failed = 0
        
    def process_message(self, message):
        # Extract build status from messages
        if "BUILD STARTED:" in message:
            match = re.search(r"BUILD STARTED: (.+)", message)
            if match:
                project = match.group(1)
                self.builds[project] = {"status": "started", "start_time": datetime.now()}
                
        elif "BUILD SUCCESS:" in message:
            match = re.search(r"BUILD SUCCESS: (.+)", message)
            if match:
                project = match.group(1)
                if project in self.builds:
                    self.builds[project]["status"] = "completed"
                    self.builds[project]["end_time"] = datetime.now()
                    self.completed += 1
                    
        elif "BUILD FAILED:" in message:
            match = re.search(r"BUILD FAILED: (.+)", message)
            if match:
                project = match.group(1)
                if project in self.builds:
                    self.builds[project]["status"] = "failed"
                    self.builds[project]["end_time"] = datetime.now()
                    self.failed += 1
    
    def print_summary(self):
        print("\n=== BUILD SUMMARY ===", flush=True)
        print(f"Total: {len(self.builds)}", flush=True)
        print(f"Completed: {self.completed}", flush=True)
        print(f"Failed: {self.failed}", flush=True)
        print(f"In Progress: {len(self.builds) - self.completed - self.failed}", flush=True)
        
        print("\nDetails:", flush=True)
        for project, info in self.builds.items():
            status = info['status']
            if 'end_time' in info and 'start_time' in info:
                duration = (info['end_time'] - info['start_time']).total_seconds()
                print(f"  {project}: {status} ({duration:.1f}s)", flush=True)
            else:
                print(f"  {project}: {status}", flush=True)

# Main monitoring loop
monitor = BuildMonitor()
print("Build Monitor startet. Überwache Build-Status...", flush=True)

try:
    while True:
        line = input()
        monitor.process_message(line)
        
        # Print summary after each status update
        if "BUILD" in line:
            monitor.print_summary()
            
except KeyboardInterrupt:
    print("\nMonitor beendet.", flush=True)
    monitor.print_summary()