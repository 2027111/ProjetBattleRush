var express = require("express");
var mongo = require('mongodb');
const mongoose = require("mongoose");
const flash = require("express-flash");
const bcrypt = require('bcryptjs');
var app = express();
const keys = require('./config/keys');
var List = require("collections/list");
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

/* Queue */


app.post("/queue/join", async (req, res)=>{

	console.log("accessing queue");
	var response= {};
	const oken = req.body.token;
	var TokenObject = await Token.findOne({tokenId : oken});

	var time = 0;


	if(!TokenObject){
	
		response.code = -9;
		response.message = userFound.username + "user isnt connected.";
		res.send(response);
	}
		
	var userFound = await User.findOne({_id : TokenObject.userId});
	if(!userFound){
		
		response.code = -9;
		response.message = userFound.username + "user doesnt exist.";
		res.send(response);
	}
	serverQueue = null;
	queueList = [];
	maxed = false;

	
	var wait = setInterval(async function() {
	if(serverQueue == null){

		serverQueue = await Server.findOne({playerConnected : 0, lobbyStatus : "Open"});
		serverQueue.lobbyType = "Casual";
		serverQueue.save();
		serverQueue = JSON.stringify(serverQueue);
	}
	
	if(queueList.length == serverQueue.maxPlayer && maxed == false){
		maxed = true;
			console.log("Bof");
			queueList.delete(userFound);
			clearInterval(wait);
			if(queueList.length == 0){
				serverQueue = null;
				maxed = false;
			}
			
			response.code = 0;
			response.msg="Send server";
			response.data = serverQueue;
			res.send(response);
	}else if(maxed == false && queueList.length != serverQueue.maxPlayer){
			queueList.add(userFound);

	}


}, 1000);

});

/* Queue */

/* Login And Registrations */



app.post("/account/login", async (req, res)=>{

	var response= {};
	const rUsername = req.body.rUsername;
	const rPassword = req.body.rPassword;
	console.log("Connection Attempt on account : " + rUsername);
	if(rUsername == null || rPassword==null){
		
	response.code = 1;
	response.msg = "Invalid credentials";
	res.send(response);
		return;
	}

	var userAccount = await User.findOne({username: {'$regex': `^${rUsername}$`, $options: 'i'}}, 'username password wins losses goldcoins');
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
					console.log(userAccount.username + " has successfully connected to the Master Server");
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
	var userAccount = await User.findOne({username: {'$regex': `^${rUsername}$`, $options: 'i'}});
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


/* Math Methods */

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

/* Math Methods */

app.post("/server/On", async (req, res) =>{
	var response= {};

		var port = req.body.port;

		console.log("Server hosting attempt using port : " + port);
	if(await Server.findOne({port : port})){

		response.code = -1;
		response.msg = "ServerWithPort already on";
		res.send(response);
		console.log("Server with open ports : " + port + " already exists.")
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
	 console.log("Server with open ports : " + port + " has successfully been created and connected to the Master Server.")

});


app.post("/server/Off", async (req, res) =>{
	
	var response= {};
	servport = req.body.port;
	await Server.findOneAndDelete({port : servport});
	console.log("Server using port " + servport + " has successfully closed");
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
		 console.log("Updated info on server using ports : " + thisserv.port)
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


app.post("/deconnexion", async (req, res) => {
	var response= {};
	const tokenid = req.body.tokenid;
	const ConTok = await Token.findOne({tokenId : tokenid});
	if(ConTok != null){

		var userFound = await User.findOne({_id : ConTok.userId});




		if(userFound){
			
		
			response.msg = userFound.username +" has disconnected";
			console.log(userFound.username +" has successfully disconnected from the Master Server");
	}else{
		console.log("Unidentifiable user has disconnected");
	}
		await Token.findOneAndDelete({tokenId : tokenid});
		response.code = 0;
	}else{
		response.code = -1;
		response.msg = "Connection non-existant";
	}
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