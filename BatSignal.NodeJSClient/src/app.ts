import * as signalR from "@microsoft/signalr";
import * as readline from "readline/promises";

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5174/hub/bat-signal")
    .build();

// we cannot do top-level awaits in nodejs yet, so we need to wrap it in a function
doStuff().then(()=> console.log('done'));

async function doStuff()
{
    await connection.start().catch(err => console.error(err.toString()));
    if (connection.state !== signalR.HubConnectionState.Connected) {
        console.log('not connected');
        return;
    }

    connection.on("ReceiveMessage", (user: string,  message: string) => {
        console.log(`[${new Date()}] ${user}: ${message}`);
    });
    
    const user: string = await rl.question('Enter your name: ');
    
    // welcome to JS land where everything is stringly-typed...
    
    await connection.send('SendMessage', user, 'Hello from nodejs client');
    
    while(true){
        await new Promise(resolve => setTimeout(resolve, 2000));
        await connection.send('SendMessage', user, "I'm still here!");
    }
}
