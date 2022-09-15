var express = require("express");
var mongo = require('mongodb');
const mongoose = require("mongoose");
const flash = require("express-flash");
const bcrypt = require('bcryptjs');
var app = express();
const keys = require('./config/keys');
// pour activer le module ejs
app.set("view engine", "ejs");

// pour permettre le parsing des URLs
app.use(express.urlencoded({ extended: true }));

// pour l'acces au dossier "public"
app.use(express.static("public"));

// pour l'acces au dossier "images"
app.use(express.static("images"));

// pour activer le module express-flash
app.use(flash());

var playerQueue = {};
var queueFound = false;  
var serverQueue = null;


var RplayerQueue = {};
var RqueueFound = false;  
var RserverQueue = null;




const User = require("./models/user");
const Server = require("./models/server");
const Token = require("./models/connectionToken");
const { response } = require("express");




app.get("/users", async (req, res) =>{


    var users = await User.find({}, 'username wins losses');

res.send(users);

});


app.post("/user", async (req, res) =>{
	var tokenid = req.body.tokenid;

	var contok = await Token.findOne({tokenId : tokenid});

if(contok){

    var userFound = await User.findOne({_id : contok.userId });
	if(userFound){

		response.code = 0;
		response.msg = "Successful Connection";
		response.token = contok.tokenId;
		response.data =(({username, wins, losses, goldcoins}) => ({username, wins, losses, goldcoins}))(userFound);
	}


}else{
	response.code = -9;
	response.msg = "UserNotConnected";
}

res.send(response);

});



app.post("/queue/leave", async (req, res)=>{
	console.log("accessing queue");
	var response= {};
	const oken = req.body.token;
	var TokenObject = await Token.findOne({tokenId : oken});
	if(TokenObject){
		
		var userFound = await User.findOne({_id : TokenObject.userId});
		if(userFound){
			if(playerQueue.playerOne != undefined){
				if(playerQueue.playerOne.username == userFound.username){
					playerQueue.playerOne = undefined;
					console.log(userFound.username + " has left the online queue as Player One");
					response.code = 0;
					response.message = "user left queue";
					res.send(response);
					return;
				}
			}
			else if(playerQueue.playerTwo != undefined){
				if(playerQueue.playerTwo.username == userFound.username){
					playerQueue.playerTwo = undefined;
					console.log(userFound.username + " has left the online queue as Player Two");
					response.code = 0;
					response.message = "user left queue";
					res.send(response);
					return;
				}
			}else if(RplayerQueue.playerOne != undefined){
				if(RplayerQueue.playerOne.username == userFound.username){
					RplayerQueue.playerOne = undefined;
					console.log(userFound.username + " has left the online queue as Player One");
					response.code = 0;
					response.message = "user left queue";
					res.send(response);
					return;
				}
			}
			else if(RplayerQueue.playerTwo != undefined){
				if(RplayerQueue.playerTwo.username == userFound.username){
					RplayerQueue.playerTwo = undefined;
					console.log(userFound.username + " has left the online queue as Player Two");
					response.code = 0;
					response.message = "user left queue";
					res.send(response);
					return;
				}
			}else{

				response.code = 3;
				response.message = "user wasnt in queue";
				res.send(response);
				return;
			}

			
		}else{
			
			response.code = -9;
			response.message = userFound.username + "user doesnt exist.";
			res.send(response);
			return;
		}
		}else{
	
			response.code = -9;
			response.message = "user isnt connected.";
			res.send(response);
			return;
		}



}); 
app.post("/queue/casual/join", async (req, res)=>{

	console.log("accessing queue");
	var response= {};
	const oken = req.body.token;
	var TokenObject = await Token.findOne({tokenId : oken});

	var time = 0;


	if(TokenObject){
		
	var userFound = await User.findOne({_id : TokenObject.userId});
	if(userFound){
		

	if(playerQueue.playerOne != undefined){
			
	if(playerQueue.playerOne.username == userFound.username){
		response.code = 2;
		response.msg = "Already in queue";
		res.send(response);
		return;
	}
	
	}
	if(playerQueue.playerTwo != undefined){
			
	if(playerQueue.playerTwo.username == userFound.username){
		response.code = 2;
		response.msg = "Already in queue";
		res.send(response);
		return;
	}
	}
	if(queueFound){
				
		var waitForOpenQueue = setInterval(function() {
			if (!queueFound){
				clearInterval(waitForOpenQueue);


			}
		}, 5000); // retry every 5 seconds
	}

	if(playerQueue.playerTwo != undefined && playerQueue.playerOne != undefined){
				
			var waitForFreeLobby = setInterval(function() {
				if (playerQueue.playerTwo == undefined || playerQueue.playerOne == undefined){
					clearInterval(waitForFreeLobby);


				}
			}, 5000); // retry every 5 seconds
	}
		
		if(playerQueue.playerTwo == undefined || playerQueue.playerOne == undefined){
			if(playerQueue.playerOne == undefined) {
				console.log(userFound.username + " has joined the online queue as Player One");
				playerQueue.playerOne = userFound;
			}else if(playerQueue.playerTwo == undefined){
				
				console.log(userFound.username + " has joined the online queue as Player Two");
				playerQueue.playerTwo = userFound;
			}

			var wait = setInterval(async function() {
				if(playerQueue.playerTwo != undefined && playerQueue.playerOne != undefined){
							queueFound = true;
				}
				if (queueFound == true){
					
					if(serverQueue == null){

						serverQueue = await Server.findOne({playerConnected : 0, lobbyStatus : "Open"});
						serverQueue.lobbyType = "Casual";
						serverQueue.save();
						serverQueue = JSON.stringify(serverQueue);
						}
				
					clearInterval(wait);
					response.code = 0;
					response.msg="Send server";
					response.data = serverQueue;

					
					if(playerQueue.playerOne == userFound){
						playerQueue.playerOne = undefined;
					}
					if(playerQueue.playerTwo == userFound){

						playerQueue.playerTwo = undefined;
					}

					if(playerQueue.playerTwo == undefined && playerQueue.playerOne == undefined){
					playerQueue = {};
					queueFound = false;
					serverQueue = null;
					}
					
					res.send(response);
					
						
				}
			}, 1000);
			
		}
		
	}else{
		
		response.code = -9;
		response.message = userFound.username + "user doesnt exist.";
		res.send(response);
	}
	}else{

		response.code = -9;
		response.message = userFound.username + "user isnt connected.";
		res.send(response);
	}




});
app.post("/queue/ranked/join", async (req, res)=>{

	console.log("accessing queue");
	var response= {};
	const oken = req.body.token;
	var TokenObject = await Token.findOne({tokenId : oken});

	var time = 0;


	if(TokenObject){
		
	var userFound = await User.findOne({_id : TokenObject.userId});
	if(userFound){
		

	if(RplayerQueue.playerOne != undefined){
			
	if(RplayerQueue.playerOne.username == userFound.username){
		response.code = 2;
		response.msg = "Already in queue";
		res.send(response);
		return;
	}
	
	}
	if(RplayerQueue.playerTwo != undefined){
			
	if(RplayerQueue.playerTwo.username == userFound.username){
		response.code = 2;
		response.msg = "Already in queue";
		res.send(response);
		return;
	}
	}
	if(RqueueFound){
				
		var waitForOpenQueue = setInterval(function() {
			if (!RqueueFound){
				clearInterval(waitForOpenQueue);


			}
		}, 5000); // retry every 5 seconds
	}

	if(RplayerQueue.playerTwo != undefined && RplayerQueue.playerOne != undefined){
				
			var waitForFreeLobby = setInterval(function() {
				if (RplayerQueue.playerTwo == undefined || RplayerQueue.playerOne == undefined){
					clearInterval(waitForFreeLobby);


				}
			}, 5000); // retry every 5 seconds
	}
		
		if(RplayerQueue.playerTwo == undefined || RplayerQueue.playerOne == undefined){
			if(RplayerQueue.playerOne == undefined) {
				console.log(userFound.username + " has joined the online queue as Player One");
				RplayerQueue.playerOne = userFound;
			}else if(RplayerQueue.playerTwo == undefined){
				
				console.log(userFound.username + " has joined the online queue as Player Two");
				RplayerQueue.playerTwo = userFound;
			}

			var wait = setInterval(async function() {
				if(RplayerQueue.playerTwo != undefined && RplayerQueue.playerOne != undefined){
							RqueueFound = true;
				}
				if (RqueueFound == true){
					
					if(RserverQueue == null){

						RserverQueue = await Server.findOne({playerConnected : 0, lobbyStatus : "Open"});
						RserverQueue.lobbyType = "Ranked";
						RserverQueue.save();
						RserverQueue = JSON.stringify(RserverQueue);
						}
				
					clearInterval(wait);
					response.code = 0;
					response.msg="Send server";
					response.data = RserverQueue;

					
					if(RplayerQueue.playerOne == userFound){
						RplayerQueue.playerOne = undefined;
					}
					if(RplayerQueue.playerTwo == userFound){

						RplayerQueue.playerTwo = undefined;
					}

					if(RplayerQueue.playerTwo == undefined && RplayerQueue.playerOne == undefined){
					RplayerQueue = {};
					RqueueFound = false;
					RserverQueue = null;
					}
					
					res.send(response);
					
						
				}
			}, 1000);
			
		}
		
	}else{
		
		response.code = -9;
		response.message = "user doesnt exist.";
		res.send(response);
	}
	}else{

		response.code = -9;
		response.message = "user isnt connected.";
		res.send(response);
	}




});
       
/* Login And Registrations */
app.post("/account/login", async (req, res)=>{

	var response= {};
	console.log("Connection Attempt...");
	const rUsername = req.body.rUsername;
	const rPassword = req.body.rPassword;
	if(rUsername == null || rPassword==null){
		
	response.code = 1;
	response.msg = "Invalid credentials";
	res.send(response);
		return;
	}

	var userAccount = await User.findOne({username: rUsername}, 'username password wins losses goldcoins');
	if(userAccount != null){
		
		if(await bcrypt.compare(rPassword, userAccount.password)){


					if(await Token.findOne({userId : userAccount._id})){
						response.code = -8;
						response.msg = "This user is already connected somewhere else.";
						res.send(response);
						return;
					}



					userAccount.lastAuthentication = Date.now();
					await userAccount.save();
					const tempToken = new Token({
						userId :userAccount._id,
						tokenId : (await GenerateToken()).toString(),
					});
					await tempToken.save();
					response.code = 0;
					response.msg = "Successful Connection";
					response.token = tempToken.tokenId;
					response.data =(({username, wins, losses, goldcoins}) => ({username, wins, losses, goldcoins}))(userAccount);
					console.log("Retrieving account...");
					res.send(response);
			return;
		}
	}

	response.code = 1;
	response.msg = "Invalid credentials";
	res.send(response);
	return;

});
app.post("/account/create", async (req, res)=>{
	
	var response= {};
	console.log("Someone just accessed the link.");
	const rUsername = req.body.rUsername;
	const rPassword = req.body.rPassword;
	const rEmail = req.body.rEmail;
	if(rUsername == null || rPassword==null || rEmail == null){
		
	response.code = 1;
	response.msg = "Invalid credentials";
	res.send(response);
		return;
	}
	var userAccount = await User.findOne({username: rUsername});
	if(userAccount == null){
		var emailAccount = await User.findOne({email : rEmail});
		if(emailAccount == null){
		try {
			console.log("Creating new account...");
		const hashedPassword = await bcrypt.hash(rPassword,  10);
		const newAccount = new User({
			username: rUsername,
			email: rEmail,
			password: hashedPassword,
			lastAuthentication: Date.now(),
		});
		await newAccount.save();
		res.send(newAccount);
	} catch (error) {
		console.log(error);
	}}else{
		console.log("Email is already in use")
		
	response.code = 2;
	response.msg = "Email already in use";
	res.send(response);
	}
}else{
	console.log("Username is already taken")
	response.code = 3;
	response.msg = "Username already in use";
	res.send(response);
}
	return;

});
/* Login And Registrations */




const getrand = () => {
	return Math.random().toString(36).substr(2);
};
const gettoken = () => {
	return getrand() + getrand();
};
async function GenerateToken (req, res){
	
	var tk = -1;
	while(tk == -1){
		tk = gettoken();
		var temp = await Token.findOne({tokenId : tk});

		if(temp != null){
			tk = -1;
		}
	}
	return tk;
}

/* Items shop and lists */

/* Debug Stuff
app.get("/createItem/:name", async (req, res)=>{


character = await Character.findOne({name : req.params.name});

if(character == null){

	thischaracter = new Character({
		name: req.params.name,
	})


	await thischaracter.save();

	res.send(thischaracter)
}});

app.get("/move", async(req, res)=>{

	var response = {};
	var moveList = await Move.find({});
	var testuser = await User.findOne({username : "Diamax"});

	
	moveList.forEach(async thismove  => {
		existingmove = await OwnedMoves.findOne({ownerid : testuser._id, moveid : thismove._id});



	if(existingmove == null){
		
		thischaracter = new OwnedMoves({
			moveid : thismove._id,
			ownerid : testuser._id,
		})
		await thischaracter.save();
		response.code = 0;
		response.msg = "Item was bought";
	}else{
		
		existingmove.amount = existingmove.amount + 1;
		
		existingmove.save();
		response.code = 0;
		response.msg = "Item was bought";

	}
	});
	res.send(response);
})


app.get("/createMove/:name", async (req, res)=>{


	move = await Move.findOne({name : req.params.name});
	
	if(move == null){
		var temp = await Character.findOne({});
		thismove = new Move({
			name: req.params.name,
			characterName : temp.name,
			moveType : "Special",
			moveRarity : "Common"

		})
	
	
		await thismove.save();
	
		res.send(thismove)
	}});
*/





app.post("/server/Off", async (req, res) =>{
	
	var response= {};
	servport = req.body.port;
	await Server.findOneAndDelete({port : servport});

	res.send(response);
});


app.post("/server/update", async (req, res) =>{

	var response= {};


	const thisserv = await Server.findOne({port : req.body.port});


		thisserv.playerConnected = req.body.connectedPlayer;
		thisserv.maxPlayer = req.body.maxPlayer;
		thisserv.lobbyStatus = req.body.lobbystate;
		thisserv.lobbyType = req.body.lobbytype;
	
	
	
		 await thisserv.save();
		 res.send(response);


});


app.post("/servers", async (req, res) =>{
	var response= {};
	const tokenTemp = req.body.tokenid;
	const thisToken = await Token.findOne({tokenId : tokenTemp});
	if(thisToken != null){
		allitems = await Server.find();
		const servers = JSON.stringify(allitems);
		response.code = 0;
		response.msg="Send servers";
		response.data = servers;
		res.send(response);

	}else{
		response.code = -9;
		response.msg="User not connected";
		res.send(response);
		console.log("User is not connected");
	}


});
app.post("/server", async (req, res) =>{
	var response= {};
	const ports = req.body.port;
	const thisServer = await Server.findOne({port : ports});
	if(thisServer != null){

		switch(thisServer.lobbyType){
			case "Casual" :
				response.code = 0;
				break;

			case "Ranked":
				response.code = 1;
				break;

			case "Friends":
				response.code = 2;
				break;


			default :	
				response.code = 15;
				break;
		}


		response.msg="Server is " + thisServer.lobbyType;
		response.data = thisServer;
		res.send(response);

	}else{
		response.code = -9;
		response.msg="Server doesn't exist";
		res.send(response);
		return;
	}


});

app.post("/server/On", async (req, res) =>{
	var response= {};

		var port = req.body.port;
	if(await Server.findOne({port : port})){

		response.code = -1;
		response.msg = "ServerWithPort already on";
		res.send(response);
		return;


	}



	const thisserv = new Server({

		ip: req.body.ip,
		port: req.body.port,
		playerConnected:req.body.playerConnected,
		maxPlayer: req.body.maxPlayer,
		lobbyStatus : req.body.lobbystate,
 		});

 	await thisserv.save();
	 response.code = 0;
	 response.msg = "Server On";
	 res.send(response);

});

app.post("/deconnexion", async (req, res) => {
	var response= {};
	const tokenid = req.body.tokenid;
	const ConTok = await Token.findOne({tokenId : tokenid});
	if(ConTok != null){

		var userFound = await User.findOne({_id : ConTok.userId});
		if(userFound){
			
		if(playerQueue.playerOne != undefined){
			if(playerQueue.playerOne.username == userFound.username){
				playerQueue.playerOne = undefined;
			}
		}
		else if(playerQueue.playerTwo != undefined){
			if(playerQueue.playerTwo.username == userFound.username){
				playerQueue.playerTwo = undefined;
			}
		}
	}
		await Token.findOneAndDelete({tokenId : tokenid});
		response.code = 0;
		response.msg = userFound.Username +" has disconnected";
	}else{
		response.code = -1;
		response.msg = "Connection non-existant";
	}
	console.log(userFound.Username +" has disconnected");
	res.send("pozz");
});


// Connexion à MongoDB
mongoose
	.connect(keys.mongoURI, {
		useUnifiedTopology: true,
		useNewUrlParser: true,
	})
	.then(() => {
        
        app.listen(keys.port, ()=>{
        console.log("Server has started on port 5500");
    });
	});