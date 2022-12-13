const db = require('./connexion')
const User = require('../models/user')
const rechercheUserMethodes = require('./rechercheUser');


beforeAll(async () => await db.connexion())

describe('Rechercher un utilisateur valide', () => {
    it('Doit trouver un utilisateur valide dans la base de données', async () => {
        const mot = 'Omar';
        const resultats = await rechercheUserMethodes.rechercheUser(mot);
        console.log(resultats)

        for( i=0;i>resultats.length;i++){
            expect((mot)).toBe(resultat[i].username);
        }
        
    })
})

describe('Rechercher un utilisateur non valide', () => {
    it('Doit trouver un utilisateur qui est pas valide dans la base de données', async () => {
        const mot = 'gibberish';
        const resultats = await rechercheUserMethodes.rechercheUser(mot);
        console.log(resultats)

        for( i=0;i>resultats.length;i++){
            expect((mot)).toBe(resultat[i].username);
        }
    
    })
})
