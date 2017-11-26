<p align="center">
<h1 align="center">Morbot</h1>
</p>
<p align="center">
  
<img src="https://discordapp.com/api/guilds/363965764987912192/embed.png?style=banner4">
<a href="https://discord.gg/Gneap49"></a>
</img>
<img src="https://github.com/TheMorc/imgs/blob/master/morbot.png?raw=true">
<a href="https://discordapp.com/api/oauth2/authorize?client_id=219188936940060684&scope=bot"></a>
</img>
</p>


A *"bigger"* example of bot made with **[DSharpPlus API](https://github.com/NaamloosDT/DSharpPlus)** on programming language **C#**

**This bot includes** *(to date of 18.November 2017 and in random order(it does not matter anyway..))*

**public void Commands(){**

- **--randomnorrisjoke** also known as **--norris**, **--norrisjoke**, **--chucknorris**, **--chuck**, **--chuckjoke** and **--randomchuckjoke** | this command gets random chuck norris joke from **[Chuck Norris API](https://api.chucknorris.io/)**. Example: **--norris** or if you even want to translate it: **--norris {language you want to translate it, for best results use en- and you language, ex:en-sk}**

- ~~**--whoami** | *command that tells you about the bot and the programmer*~~ **This command was replaced with new command --bot**

- **--latestvideo** also with these aliases **--latestmorcvideo**, **--morcvideo**, **--lastvideo** and  **--lastvideobymorc** | this command pulls video list from my channel **[Morc](https://youtube.com/riskoautobus)** and sends link of latest video. Example: **--latestvideo**

- **--weather** | this command takes weather of town/village that you can choose.If you dont specify any town then it will be defaulted to Topoľčany(small town near Biskupová, village where Morc lives).Weather is taken from **[OpenWeather Api](https://openweathermap.org/api)** in JSON format deserialzed with Json.NET to send temperature and look outside. Example: **--weather {town of your choice, ex:Moscow}**

- **--time** | pulls time from my pc(one of simplest commands with --bot, i think this command can have use if someone on the other side of world doesnt know where i live and wants to know the time here.) Example: **--time**

- **--randomwindows** also has these aliases **--randwind**, **--randw**, **--rwin**, **--randomw** and **--ranwin** | sends picture of random selected windows Example: **--ranwin**

- **--cat** also known as **--meow** and **--kitty** | sends cute picture of cat from **https://random.cat/** Example: **--cat**

- **--dog** also known as **--woof** and **--puppy** | sends cute picture of dog from **https://random.dog/** Example: **--dog**

- **--mode** or longer alias **--changemode** | changes the mode of bot to Listening To, Playing, Streaming, or Watching. Example: **--mode {mode, ex:stream}**

- **--randomgif** also known as **--randgif** | This command pulls random gif from **[Giphy](https://giphy.com)** with their **[Giphy GIF API](https://developers.giphy.com/docs/)** If nothing is specified with command, bot responds with url of gif(it searches for dog or cat(dog or cat gets selected by random, gif by Giphy)). If you specify for ex. tractor,bot again responds with url of gif(again searches for tractor(gif gets selected by Giphy)) Example **--gif {gif you are searching, ex:hotdog}**

- **--compress** | This command compresses image that you supply by attaching it or by adding url to it. You can set your own compress ratio 0worst 100best. Example usage: **--compress 45 {image url}**, **--compress {image url}** or **--compress** and your attached image

- **--message** | Simple command that uses Magick.NET library to create fake discord message in .png Example: **--message {text}**

- **--emoji** | Command that doesn't use Magick.NET but CoreCompat's port of System.Drawing for .NET Core and creates black image with white Windows 10 emojis. Example: **--emoji {emojis of your choice. ex:🤔}**

- **--anime** or longer alias **--top10animedeaths** | Command using CoreCompat's port of System.Drawing for making the Top 10 Anime Deaths meme that was going thru world. Example: **--anime {image url}** or **--anime** and attached image

- **--screenshot** | command that uses Internet Explorer to screenshot sites. Example: **--screenshot {url}**

- **--age** | command that sends age of bot(from the first commit here on GitHub) + sends last commit's name and age of it.

**}**

**public void Voice_Commands() {**

- **--join** also known as **--vchjoin**, **--voicechanneljoin**, **--voicejoin**, **--channeljoin** and **--voicechjoin** | this command connects bot to users voice channel. - user must be on voice channel

- **--leave** also known as **--vchleave**, **--voicechannelleave**, **--voiceleave**, **--channelleave** and **--voicechleave** | this command disconnect bot from voice channel - user doesnt need to be in any voice channel

- **--play** also known as **--vchplay**, **--voicechannelplay**, **--voiceplay**, **--channelplay** and **--voicechplay** | command can play youtube video(just audio and only using provided link), can play mp3s from url and can play music from M:/.

- **--music** | this simple command returns list of music on my hdd more specified on M:/ partition.

- **--speak** | this command speaks everything you send to it. but you need to specify language first **--speak {sk/cs/fi} {text}** 

- **--dude** also known as **--lookatthisdude**, **--latd**, **--lookatdude** | command that uses VoiceNext extension to play that guy which was laughing on the pepe frog.

- **This bot includes one or more hidden commands which can be found in bots code.**

**}**

# Credits:
- People that made **[DSharpPlus API](https://github.com/NaamloosDT/DSharpPlus)**
- Emzi0767 for his great help source code **[DSharpPlus Example Bot](https://github.com/Emzi0767/DSharpPlus-Example-Bot)**
- Guys on **[Discord API](https://discord.gg/discord-api)**, **[Official D#+ Discord Server](https://discord.gg/KeAS3pU)** and **[The Sp00ky Loli Farm](https://discordapp.com/invite/0oZpaYcAjfvkDuE4)** servers.
