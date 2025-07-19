import json
import time

print("Build Scheduler startet...", flush=True)

# Liste der zu bauenden Projekte
projects = [
    "ProjectA.csproj",
    "ProjectB.csproj", 
    "ProjectC.csproj"
]

for project in projects:
    task = {
        "name": f"build_{project}",
        "type": "build",
        "tags": ["build", "compile"],
        "parameters": {
            "project": project,
            "configuration": "Release"
        }
    }
    
    # Task als JSON ausgeben
    print(f"/task create {json.dumps(task)}", flush=True)
    time.sleep(2)

print("Alle Build-Tasks verteilt!", flush=True)