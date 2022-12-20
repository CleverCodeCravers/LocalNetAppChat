## Situation

- There is a server application running on one host that is known by the dns as "serverhost", on port 8080.
- There is no client activly listening.

## Action

- A client is called on the command line to send a message to serverhost,  too. E.g. this way:
```
client.exe --server serverhost --port 8080 --mode message --message "Hi there, I am Bob"
```
- Start three clients in [[R006 continuous listening mode]], one after the other.

## Expected Result

- All three clients, although started after the message already has been sent and after each other, still receive the message.
- The result is diplayed in the [[R005 standard display format for messages]]

## Note 

- [[R003 Persistent messages]]