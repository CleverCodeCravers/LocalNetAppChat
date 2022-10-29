## Situation

- There is a server application running on one host that is known by the dns as "serverhost", on port 8080.
- There are 2 clients connected to the server in [[R006 continuous listening mode]]. We will call them "Receiver A" and "Receiver B".

## Action

- Another client is called on the command line to send a message to serverhost,  too. E.g. this way:
```
client.exe --server serverhost --port 8080 --mode message --message "Hi there, I am Bob"
```

## Expected Result

- Receiver A and Receiver B have both received the message.
- The result is diplayed in the [[R005 standard display format for messages]]
