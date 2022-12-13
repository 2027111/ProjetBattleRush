const db = require('./connexion')
const Token = require('../models/server')
const rechercheServeurMethodes = require('./rechercheServer');


beforeAll(async () => await db.connexion())

describe('Rechercher un serveur valide', () => {
    it('Doit trouver serverur qui est valide dans la base de données, cela veut dire le serveur roule', async () => {
        const ip = '127.0.0.1';
        const port = '63698';
        const resultats = await rechercheServeurMethodes.rechercheServer(ip,port);
        console.log(resultats)

        for( i=0;i>resultats.length;i++){
            expect((ip)).toBe(resultat[i].ip);
            expect((port)).toBe(resultat[i].port);
        }
        
    })
})

describe('Rechercher un serveur non valide', () => {
    it('Doit trouver un serveur qui est pas valide dans la base de données', async () => {
        const ip = '1.0.0.1';
        const port = '11111';
        const resultats = await rechercheServeurMethodes.rechercheServer(ip,port);
        console.log(resultats)

        for( i=0;i>resultats.length;i++){
            expect((ip)).toBe(resultat[i].ip);
            expect((port)).toBe(resultat[i].port);
        }
    })
})
