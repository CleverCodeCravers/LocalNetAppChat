import json
import time
import sys
from datetime import datetime

class BuildScheduler:
    def __init__(self):
        self.projects = []
        self.build_order = []
        
    def add_project(self, name, dependencies=None, configuration="Release"):
        """Add a project with optional dependencies"""
        project = {
            "name": name,
            "dependencies": dependencies or [],
            "configuration": configuration
        }
        self.projects.append(project)
    
    def calculate_build_order(self):
        """Calculate build order based on dependencies"""
        built = set()
        order = []
        
        while len(built) < len(self.projects):
            for project in self.projects:
                if project["name"] not in built:
                    # Check if all dependencies are built
                    if all(dep in built for dep in project["dependencies"]):
                        order.append(project)
                        built.add(project["name"])
        
        self.build_order = order
        return order
    
    def schedule_builds(self):
        """Schedule builds in dependency order"""
        print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] Build Scheduler Advanced startet...", flush=True)
        
        if not self.build_order:
            self.calculate_build_order()
        
        print(f"Build order: {[p['name'] for p in self.build_order]}", flush=True)
        
        # Group projects by dependency level for parallel builds
        levels = {}
        for project in self.build_order:
            level = len(project["dependencies"])
            if level not in levels:
                levels[level] = []
            levels[level].append(project)
        
        # Schedule builds level by level
        for level in sorted(levels.keys()):
            level_projects = levels[level]
            print(f"\nScheduling level {level} projects: {[p['name'] for p in level_projects]}", flush=True)
            
            for project in level_projects:
                task = {
                    "name": f"build_{project['name']}",
                    "type": "build",
                    "tags": ["build", "compile", f"level-{level}"],
                    "parameters": {
                        "project": project["name"],
                        "configuration": project["configuration"],
                        "dependencies": project["dependencies"]
                    }
                }
                
                # Send task
                print(f"/task create {json.dumps(task)}", flush=True)
                time.sleep(0.5)  # Small delay between tasks in same level
            
            # Wait longer between levels to ensure dependencies are built
            if level < max(levels.keys()):
                print(f"Waiting for level {level} builds to complete...", flush=True)
                time.sleep(5)
        
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] Alle Build-Tasks verteilt!", flush=True)

# Example usage
scheduler = BuildScheduler()

# Add projects with dependencies
scheduler.add_project("ProjectB.csproj")  # No dependencies
scheduler.add_project("ProjectA.csproj")  # No dependencies
scheduler.add_project("ProjectC.csproj", dependencies=["ProjectB.csproj"])  # Depends on ProjectB

# Schedule the builds
scheduler.schedule_builds()