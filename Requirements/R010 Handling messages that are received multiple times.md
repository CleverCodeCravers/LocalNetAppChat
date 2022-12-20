## Situation

- There is a server application running on one host that is known by the dns as "serverhost", on port 8080.
- There is a client connected to the server in [[R006 continuous listening mode]]. We will call it "Receiver A".

## Action

- Another client is called on the command line to send a message to serverhost,  too. E.g. this way:
```
client.exe --server serverhost --port 8080 --message "Hi there, I am Bob" --repeat 2 --id "id-override"
```

## Expected Result

- The client sends the message twice.
- Receiver A receives the message only one time.
- The result is diplayed in the [[R005 standard display format for messages]]

## Explanaition

- Should a client why-o-ever try to send the same message multiple times, the server still knows for an hour that a message with that id has already been sent to him and it will not relay the message to the other clients.
- This way we ensure, that clients really create their message ids appropriatly, so they can be told apart from each other.