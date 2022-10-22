# LocalNetAppChat

## Step 1

- [ ] There are several options that such a chat could use but we probably need a "server" and "clients" either way
- [ ] Options would be:
  - [ ] Direct TCP communication
  - [ ] ASP.Net Web API (Web Application with asp.net core), 
  - [ ] ASP.Net Web Application using websockets
- [ ] there is an endpoint for sending a message and one to receive messages

- Server.exe is executed at a known location `server.exe --port 8080`

- client.exe 1 connects ( `client.exe --server 192.168.88.15` ) 
- client.exe 2 connects  ( `client.exe --server 192.168.88.15` ) 
- client.exe 2 sends a message  ( `client.exe --server 192.168.88.15` ) 
- client.exe 1 receives the message and displays it on the screen  ( `client.exe --server 192.168.88.15` ) 

- "clients are able to chat with each other"
- clients generate a guid which serves as their "nickname", it can be a new guid with every app start
- messages have information about who sent them (which client)

## Future lookout

- [ ] Clients scan the local net for their server counterpart, they remember him as best as they can, but when they cannot find him, they scan the local net again
- [ ] The local net is defined by the netmask, and in our case, it is max. 254 computers/ips that need scanning.
- [ ] Clients can send messages. The messages are broadcasted to every other client, that is connected
- [ ] client authentication

## Even further future

### usecase 1
- the vm can send a request to be shut down by force
- the host-client receives the message and executes the appropriate powershell command as an administrator

### usecase 2
- the build hosts messaging systems with which they babble all the time with the users is replaced by this new communication service

