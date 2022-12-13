const db = require('./connexion')
const Server = require('../models/connectionToken')
const rechercheServerMethodes = require('./rechercheToken');


beforeAll(async () => await db.connexion())

describe('Rechercher un token valide', () => {
    it('Doit trouver token qui est valide dans la base de données', async () => {
        const userId = '637a65bca5ff55281c6f9306';
        const tokenId = '638618bc614cf935f854c333';
        const resultats = await rechercheServerMethodes.rechercheToken(userId,tokenId);
        console.log(resultats)

        for( i=0;i>resultats.length;i++){
            expect((userId)).toBe(resultat[i].userId);
            expect((tokenId)).toBe(resultat[i].tokenId);
        }
        
    })
})

describe('Rechercher un token non valide', () => {
    it('Doit trouver token qui est non valide dans la base de données', async () => {
        const userId = 'aaaaaaaaaaaaaaaaaaa';
        const tokenId = 'bbbbbbbbbbbbbbbbbb';
        const resultats = await rechercheServerMethodes.rechercheToken(userId,tokenId);
        console.log(resultats)

        for( i=0;i>resultats.length;i++){
            expect((userId)).toBe(resultat[i].ip);
            expect((tokenId)).toBe(resultat[i].tokenId);
        }
        
    })
})
