const Server = require('../models/server');

async function rechercheServer(elementIp, elementPort) {
    console.log("le serveur rechercher est : " + elementIp, elementPort);
    const regexIp = new RegExp(escapeRegex(elementIp), 'gi');
    const regexPort = new RegExp(escapeRegex(elementPort), 'gi');
    let resultat =  await Server.find({ ip: regexIp, port : regexPort }).exec()
    return resultat
}

module.exports = {
    rechercheServer: rechercheServer
}