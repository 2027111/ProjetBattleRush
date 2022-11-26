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






const User = require("./models/user");
const Server = require("./models/server");
const Friendship = require("./models/friendship");
const Token = require("./models/connectionToken");
const { response } = require("express");
const user = require("./models/user");

var queuelist = [];


app.get("/users", async (req, res) =>{


    var users = await User.find({}, 'username wins losses');

res.send(users);

});

app.post("/user/friend", async (req, res) =>{
	var username = req.body.rUsername;


    var userFound = await User.findOne({username : username });
	if(userFound){
		console.log("User : " + username + " has succesfully been looked up for.");
		response.code = 0;
		response.message = "Successful request";
		response.data = JSON.stringify(userFound.friendlist);
	}else{
		response.code = -1;
		console.log("User : " + username + " does not exist and search is invalid.");
	}

	res.send(response);

});

app.post("/user", async (req, res) =>{
	var username = req.body.rUsername;


    var userFound = await User.findOne({username : username });
	if(userFound){
		console.log("User : " + username + " has succesfully been looked up for.");
		response.code = 0;
		response.message = "Successful Connection";
		response.data =(({username, wins, losses, goldcoins, accounttype}) => ({username, wins, losses, goldcoins, accounttype}))(userFound);
	}else{
		response.code = -1;
		console.log("User : " + username + " does not exist and search is invalid.");
	}

	res.send(response);

});

/* Queue */
app.post("/queue/leave", async (req, res)=>{

	console.log("accessing queue");
	var response= {};
	const oken = req.body.token;
	var TokenObject = await Token.findOne({tokenId : oken});



	if(TokenObject == null){
	
		response.code = -9;
		response.message = "user isnt connected.";
		res.send(response);
	}
		
	var userFound = await User.findOne({_id : TokenObject.userId});
	if(userFound == null){
		
		response.code = -9;
		response.message = userFound.username + "user doesnt exist.";
		res.send(response);
	}

	
	
	if(queueList.includes(userFound)){
		queueList.push(userFound);
		console.log("User : " + userFound.username + " has been added to the queue");
		console.log(queueList);
	}
});

app.post("/account/getRequests", async(req, res)=>{
	var response= {};
	const token = req.body.token;
	var TokenObject = await Token.findOne({tokenId : token});



	if(TokenObject == null){
	
		response.code = -9;
		response.message = "user isnt connected.";
		res.send(response);
		return;
	}
		
	var userFound = await User.findOne({_id : TokenObject.userId});
	if(userFound == null){
		
		response.code = -9;
		response.message =  "Current user doesnt exist.";
		res.send(response);
		return;
	}

	var requests = await Friendship.find({userRecipientID:userFound.username, requestStatus:1});
	response.code = 0;
	response.message = "requests found!";
	response.data = JSON.stringify(requests);
	res.send(response);

});

app.post("/account/acceptRequest", async(req, res)=>{
	var response= {};
	const token = req.body.token;
	const friendname = req.body.friendname;
	const accepted = req.body.accept;
	var TokenObject = await Token.findOne({tokenId : token});



	if(TokenObject == null){
	
		response.code = -9;
		response.message = "user isnt connected.";
		res.send(response);
		return;
	}
		
	var userFound = await User.findOne({_id : TokenObject.userId});
	if(userFound == null){
		
		response.code = -9;
		response.message =  "Current user doesnt exist.";
		res.send(response);
		return;
	}
	var requestUserFound = await User.findOne({username : friendname});
	if(requestUserFound == null){
		
		response.code = -43;
		response.message = friendname + " user doesnt exist.";
		res.send(response);
		return;
	}
	var Request = await Friendship.findOne({userRequestID: userFound.username, userRecipientID: requestUserFound.username});
	if(Request == null){
		response.code = -11;
		response.message = "Requests doesnt exists."
		res.send(response);
		return;
	}
	await AcceptRequest(Request, accepted == "YES");
	response.code = 0;
	response.message = "Request accepted"
	res.send(response);
	return;
});
app.post("/account/sendfriendrequest", async(req, res)=>{
	
	var response= {};
	const token = req.body.token;
	const friendname = req.body.friendname;
	var TokenObject = await Token.findOne({tokenId : token});



	if(TokenObject == null){
	
		response.code = -9;
		response.message = "user isnt connected.";
		res.send(response);
		return;
	}
		
	var userFound = await User.findOne({_id : TokenObject.userId});
	if(userFound == null){
		
		response.code = -9;
		response.message =  "Current user doesnt exist.";
		res.send(response);
		return;
	}

	var requestUserFound = await User.findOne({username : friendname});
	if(requestUserFound == null){
		
		response.code = -43;
		response.message = friendname + " user doesnt exist.";
		res.send(response);
		return;
	}


	try {
		console.log("Creating new request...");
	const newRequest = new Friendship({
		userRequestID: userFound.username,
		userRecipientID: requestUserFound.username,
		requestStatus: 1,
	});

	const i = userFound.friendlist.findIndex(e => e === requestUserFound.username);
	if (i > -1) {
		response.code = 11;
		response.message = "Requests already exists."
		res.send(response);
		return;
	}
	
	var Test = await Friendship.findOne({userRequestID: userFound.username, userRecipientID: requestUserFound.username});
	var Test2 = await Friendship.findOne({userRequestID: requestUserFound.username, userRecipientID: userFound.username});
	if(Test != null){
	response.code = 11;
	response.message = "Requests already exists."
	res.send(response);
	return;
	}
	if(Test2 != null){
		if(Test2.requestStatus == 1){
			await AcceptRequest(Test2, true);
		}
	response.code = 12;
	response.message = "Opposing request has been accepted."
	res.send(response);
	return;
	}

	await newRequest.save();
	response.code = 0;
	response.message = "Request has been sent!"
	res.send(response);
} catch (error) {
	console.log(error);
}
	
})

app.post("/queue/join", async (req, res)=>{

	var response= {};
	const token = req.body.token;
	const queueType = req.body.queueType;
	var TokenObject = await Token.findOne({tokenId : token});



	if(TokenObject == null){
	
		response.code = -9;
		response.message = "user isnt connected.";
		res.send(response);
		return;
	}
		
	var userFound = await User.findOne({_id : TokenObject.userId});
	if(userFound == null){
		
		response.code = -9;
		response.message = userFound.username + "user doesnt exist.";
		res.send(response);
		return;
	}


	console.log("User : " + userFound.username + " is accessing online MatchMaking.")
	queuelist.push(userFound);
	var serverFound;
	serverFound = await Server.findOne({lobbyType : queueType, lobbyStatus:"Open"});

	var waitForGoodServerQueue = setInterval(function() {
		if(serverFound == null){
			response.code = 2;
			response.message = "Empty server";
			queuelist.splice(userFound);
			res.send(response);
			clearInterval(waitForGoodServerQueue);
			return;
		}
		if (serverFound.playerConnected < serverFound.maxPlayer){
			clearInterval(waitForGoodServerQueue);
		}	
	
	}, 5000); // retry every 5 seconds
	const i = queuelist.findIndex(e => e.username === userFound.username);
	if (i > -1) {
		response.code = 0;
		response.message = "ServerFound";
		response.data = serverFound;
		queuelist.splice(userFound);
		res.send(response);
	}else{
		response.code = 1;
		response.message = "user has left queue";
		res.send(response);
	}
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
	response.message = "Invalid credentials";
	res.send(response);
		return;
	}

	var userAccount = await User.findOne({username: {'$regex': `^${rUsername}$`, $options: 'i'}}, 'username password wins losses goldcoins');
	if(userAccount != null){
		
		if(await bcrypt.compare(rPassword, userAccount.password)){


					if(await Token.findOne({userId : userAccount._id})){
						response.code = -8;
						response.message = "This user is already connected somewhere else.";
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
					response.message = "Successful Connection";
					response.token = tempToken.tokenId;
					response.data =(({username, wins, losses, goldcoins}) => ({username, wins, losses, goldcoins}))(userAccount);
					console.log(userAccount.username + " has successfully connected to the Master Server");
					res.send(response);
			return;
		}
	}

	response.code = 1;
	response.message = "Invalid credentials";
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
	response.message = "Invalid credentials";
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
	response.message = "Email already in use";
	res.send(response);
	}
}else{
	console.log("Username is already taken")
	response.code = 3;
	response.message = "Username already in use";
	res.send(response);
}
	return;

});
async function AcceptRequest(Request, accepted) {

	if(accepted == false){
		Request.requestStatus = 3;
		await Friendship.findOneAndDelete({_id: Request._id})
		return;
	}

	var RequesteeUser = await User.findOne({username : Request.userRequestID});
	var RequesterUser = await User.findOne({username : Request.userRecipientID});
	RequesteeUser.friendlist.push(RequesterUser.username);
	RequesterUser.friendlist.push(RequesteeUser.username);
	Request.requestStatus = 2;
	RequesteeUser.save();
	RequesterUser.save();
	await Friendship.findOneAndDelete({_id: Request._id})
  }
function getRandomInt(max) {
	return Math.floor(Math.random() * max);
  }
app.post("/server/winlose", async (req, res) => {
	var response= {};
	USERNAME = req.body.username;
	POSITION = req.body.position;
	SERVERCOUNT = req.body.serverCount
	var userFound = await User.findOne({username : USERNAME});




	if(userFound){
			
		
		gold = getRandomInt(96);
		gold *= ((SERVERCOUNT-POSITION)+1)
		truegold = userFound.goldcoins;
		truegold += gold;
		userFound.goldcoins = truegold;
		await userFound.save();
		response.code = 0;

	}else{
		console.log("Unidentifiable user has disconnected");
		response.code = -1;
		
	}

	
	res.send(response);
});

app.post("/deconnexion", async (req, res) => {
	var response= {};
	const tokenid = req.body.tokenid;
	const ConTok = await Token.findOne({tokenId : tokenid});
	if(ConTok != null){

		var userFound = await User.findOne({_id : ConTok.userId});




		if(userFound){
			
		
			response.message = userFound.username +" has disconnected";
			console.log(userFound.username +" has successfully disconnected from the Master Server");
	}else{
		console.log("Unidentifiable user has disconnected");
	}
		await Token.findOneAndDelete({tokenId : tokenid});
		response.code = 0;
	}else{
		response.code = -1;
		response.message = "Connection non-existant";
	}
	res.send(response);
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

/*Server*/
app.post("/server/On", async (req, res) =>{
	var response= {};

		var port = req.body.port;

		console.log("Server hosting attempt using port : " + port);
	if(await Server.findOne({port : port})){

		response.code = -1;
		response.message = "ServerWithPort already on";
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
	 response.message = "Server On";
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

		if(thisserv){

			thisserv.playerConnected = req.body.connectedPlayer;
			thisserv.maxPlayer = req.body.maxPlayer;
			thisserv.lobbyStatus = req.body.lobbystate;
			thisserv.lobbyType = req.body.lobbytype;
		
		
		
			 await thisserv.save();
			 console.log("Updated info on server using ports : " + thisserv.port)
		 
		}
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
		response.message="Send servers";
		response.data = servers;
		res.send(response);

	}else{
		response.code = -9;
		response.message="User not connected";
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


		response.message="Server is " + thisServer.lobbyType;
		response.data = thisServer;
		res.send(response);

	}else{
		response.code = -9;
		response.message="Server doesn't exist";
		res.send(response);
		return;
	}


});
/*Server*/



// Connexion à MongoDB
mongoose
	.connect(keys.mongoURI, {
		useUnifiedTopology: true,
		useNewUrlParser: true,
	})
	.then(() => {
        
        app.listen(keys.port, ()=>{
        console.log(`Server has started on port ${keys.port}`);
    });
	});