// schema pour user
const mongoose = require("mongoose");
const serverSchema = new mongoose.Schema({
	ip: {
		type: String,
		required: true,
	},
	port: {
		type: String,
		required: true,
	},
	lobbyType: {
		type: String,
		enum: ["Casual", "Friends", "Ranked", "Offline"],
		default: "Casual",
		required: true,
	},
	lobbyStatus: {
		type: String,
		enum: ["Open", "InGame", "Battle", "Closed"],
		default: "Open",
		required: true,
	},
	playerConnected:{
		type:Number,
		default:0,
		required: true,
	},
	maxPlayer:{
		type:Number,
		default:2,
		required: true,
	}
});

const server = mongoose.model("server", serverSchema);

// pour l'acces dans les autres fichiers
module.exports = server;
