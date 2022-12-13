const mongoose = require('mongoose');

function connexionBD() {
    var url='mongodb://127.0.0.1:27017/brproject';
    
    mongoose.connect(url);
    mongoose.connection.once('open', () => {
        console.log('la connextion a ete etabli');
    }).on('error', (error) => console.log(error));
}

async function deconnexionBD() {
    await mongoose.connection.dropDatabase();
    await mongoose.connection.close();
}

// //Efface toutes les données
async function effacerBD() {
    const collections = mongoose.connection.collection;
    for (const key in collections) {
        const collection = collections[key];
        await collection.deleteMany();
    }
}


module.exports = {
    connexionBD: connexionBD,
    effacerBD: effacerBD,
    deconnexionBD: deconnexionBD
}
