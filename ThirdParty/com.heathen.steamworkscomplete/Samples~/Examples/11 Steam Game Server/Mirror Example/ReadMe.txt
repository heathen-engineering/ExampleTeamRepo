The scenes included in this example demonstrate the minimal set up required to initialize a Steam Game Server.
You should select one of these scenes as your build index 0 and build a server build of the game. 
Once built you can run the app to see Steam Game Server initialize.

=========
NOTE
=========

Each scene has its own Steam Settings configuration... "Mirror" uses the Sample Steam Settings common to all other scenes

The reason other HLAPIs have there own is that each Networking HLAPI has different requriements for set up and initalization workflow.

In general the workflow is:
1) Initialize the Steam API
2) Configure the Steam Game Server Settings
3) Initialize your HLAPI's Network Manager or similar concept
4) Start your HLAPI's server ... for example in Mirror we call NetworkManager.StartServer()
5) Call LogOn for the Steam Game Server to notify Steam Game Server endpoint that your server is here and ready to connecct