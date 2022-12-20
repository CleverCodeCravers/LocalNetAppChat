## Situation

- There is a server application running on one host that is known by the dns as "serverhost", on port 8080.

## Action

- "Receiver A" client is started in [[R011 task-receiving mode]] .
```
client.exe --server serverhost --port 8080 --name "Receiver A" --mode task-receiver --tags build --processor "run.ps1"
```

- Another client is used to send a task. E.g. 
```
client.exe --server serverhost --port 8080 --name "Sender" --task "Build something" --tags build --parameters "{ url: 'https://github.com/stho32/something'}" --wait
```


## Expected Result

- Receiver A does nothing really besides connecting until "Sender" starts.
- Sender starts.
- Receiver A receives a "tip" from the server, that there is a new task with an id that he could claim.
- Receiver A sends a message to the server that he would like to claim the task.
  
- The server sends two messages: 
	- It sends a message to "Sender", that the task has been claimed and by whom.
	- It sends a message to "Receiver A" with the complete task as well as the information that it has claimed the task successfully. 
	  
- Receiver A starts "run.ps1" or tries to do so. 
	- Receiver A writes the content of the task into a task-task_id.json - file.
	- Receiver A starts run.ps1 with the first parameter pointing to the absolute path of the json file.
	  
- Depending on the result of run.ps1 it will send a message to the server, related to the task, identified by the id:
	- if the return value from run.ps1 is not 0, an error occured
	- if the return value from run.ps1 is 0, it has been a success
	- the full console output of run.ps1 is redirected and captured during the execution and after that is transmitted to the server in the result.
	  
- Receiver A will clean up
	- Receiver A will remove the task-task_id.json file from its temporary location.
	  
- Sender receives a message that his task is complete with the success information as well as the captured output of run.ps1. it will write the output to the screen and then exit.

- Receiver A should still be running. waiting for the next task.

