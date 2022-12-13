const Users = require('../models/user');

async function rechercheUser(elementRechercher) {
    console.log("le terme rechercher est : " + elementRechercher);
    const regex = new RegExp(escapeRegex(elementRechercher), 'gi');
    let resultat =  await Users.find({ username: regex }).exec()
    return resultat
}

module.exports = {
    rechercheUser: rechercheUser
}