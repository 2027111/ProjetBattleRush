
const mongoose = require("mongoose");
const friendshipSchema = new mongoose.Schema({
    userRequestID: {
        type: String, 
        ref: 'User'
    },
    userRecipientID: {
        type: String,
        ref: 'User'
    },
    requestStatus: {
        type: Number, 
        default: 1 //1 = Requested, 2 = Accepted, 3 = Rejected
    }
});

const friendship = mongoose.model("friendship", friendshipSchema);

// pour l'acces dans les autres fichiers
module.exports = friendship;