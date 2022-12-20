# Usage Scenario: CI/CD-Pipeline

## Description

- a bot watches a repository server and when a change is seen, it send a change notification for the repository into the chat. It will send the message until a build-bot claims it. Lets call that message #fuehaks223
- in the chat 1..n bots look out for such a message and try to claim it.
- A build-bot on a build machine claims the notification. 
  - It clones the repository, builds it and exports artifacts, which it will upload to the central storage. 
  - It will then send a commit message for #fuehaks223 saying that the build has been completed successfully.
  - It will possibly, as a result, send a "deploy now" message for some of the artifacts
- A registry bot will update his registry for the deployment-bots assigned to it (e.g. one of the artifacts
- The deployment-bots send the registry bot their list of applications that they are hosting right now receiving an update notification for all artifacts that they need to change.
  - The deployment-bots stop the running app process, download and install the update and then restart the app process again.

## What do you need?

- A bot that watches the repository server reporting changes.
- A claiming system
    - Master-Bot sends "Who wants #fuehaks223"
    - Client-Bots answer "I am `whoever` and want to claim #fuehaks223"
    - Master-Bot sends "I give #fuehaks223 to `whoever`."
    - Client-Bot `whoever` can start processing.
- A build-bot that is running on a build machine
- A deployment-registry bot
- A deployment bot

