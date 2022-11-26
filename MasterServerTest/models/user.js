// schema pour user
const mongoose = require("mongoose");
const userSchema = new mongoose.Schema({
	username: {
		type: String,
		required: true,
	},
	email: {
		type: String,
		required: true,
	},
	password: {
		type: String,
		required: true,
	},
	wins:{
		type:Number,
		required:true,
		default:0,
	},
	losses:{
		type:Number,
		required:true,
		default:0,
	},
	elo:{
		type:Number,
		default:1000,
		required:true,
	},
	goldcoins:{
		type:Number,
		default:100,
		required:true,
	},
	accounttype:{
		type: String,
		enum: ["Player", "Moderator", "Dev", "Bot"],
		default: "Player",
		required: true,
	},
	lastAuthentication:{
		required:true,
		type:Date,
	},
	friendlist:[{
		required:true,
		type:String,
	}]
});

const user = mongoose.model("user", userSchema);

// pour l'acces dans les autres fichiers
module.exports = user;
