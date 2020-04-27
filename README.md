# GLR-bot
Discord bot for GLR

# Invite

If you want the bot in your server, without hosting it yourself, click on either link below.

### Link with required permissions
https://discordapp.com/api/oauth2/authorize?client_id=696342679012900894&permissions=378944&scope=bot

### Link with all permissions (recommended)
https://discordapp.com/api/oauth2/authorize?client_id=696342679012900894&permissions=8&scope=bot

# Install & Use

If you want to host the bot yourself locally, so you can add custom commands and have a custom name / avatar, you can follow the steps below.

## Prerequisites 

- VPS / local server
- commandline access (preferably admin/sudo access)
- dotnet installed
- git installed
- screen installed (if more experienced, set it up as a service)
  
## Installation

First, you'll need to clone the github repo:  

`git clone https://github.com/svr333/GLR-bot.git`  

This will be cloned wherever your commandline was, change your commandline location by doing `cd location/to/go/to` and then clone

Once this is done, you'll have a folder called GLR-bot with all the source files in there.
After cloning, head in to the folder by executing

`cd GLR-bot`

Now, you can edit the code freely, though if you want to do that it's best to fork and then clone, but I'm not explaining that.

No we will publish the app so it's in a single file, and easy to run. To publish, execute this command:

`dotnet publish src/GLR.Core/GLR.Core.csproj -c Release -r <RuntimeID> /p:PublishSingleFile=true`

*Maybe I'll one day make it so you don't have to do this and can just download the release file. Might be better.*

You have to replace \<RuntimeID> with whatever you want to publish the bot on. A list of <RuntimeID's> can be found [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).

Your files are outputted to `./src/GLR.Core/bin/release/netcoreapp3.1/<RuntimeID>/publish` 

I'd move the files to a different folder by using the  
`mv from/path to/path`  
command, however that isn't necessary.

On linux, you need to give the output file called `GLR.Core` execution permission, by executing `chmod +x path/to/GLR.Core`

Once that's done you can run it by doing `./path/to/GLR.Core` and all is done, however, if you want to logout of your remote session tahat you use to connect to your server, you need to use screen or something that can do the same.

Once you have screen installed, just do:  

`screen -dmS GLR-bot ./path/to/GLR.Core`  

I recommend watching a tutorial on screen to figure out how it works exactly.

On Windows / mac: Install linux and follow the steps above.  
This is a very brief explanation and I didn't spend too much time on it, you'll probably have to look a few things up, but the gist is there.
I also don't think it is necessary to build the bot yourself, since it is still in development and not the most stable.  
Also, yes, this paragraph is only done so this file has 69 lines of 'code'.
