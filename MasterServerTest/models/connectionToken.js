// schema pour user
const mongoose = require("mongoose");
const connectionTokenSchema = new mongoose.Schema({
	userId: {
		type: String,
		required: true,
	},
	tokenId: {
		type: String,
		required: true,
	}
});

const connectionToken = mongoose.model("connectionToken", connectionTokenSchema);

// pour l'acces dans les autres fichiers
module.exports = connectionToken;
