const Server = require('../models/connectionToken');

async function rechercheToken(elementuserID, elementtokenID) {
    console.log("le token recherche est : " + elementuserID, elementtokenID);
    const regexUserID = new RegExp(escapeRegex(elementuserID), 'gi');
    const regexTokenId = new RegExp(escapeRegex(elementtokenID), 'gi');
    let resultat =  await Server.find({ userId: regexUserID, tokenId : regexTokenId }).exec()
    return resultat
}

module.exports = {
    rechercheToken: rechercheToken
}