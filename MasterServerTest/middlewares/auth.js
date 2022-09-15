/**
 * fonction qui verifie l'authentification du user
 * @param {*} req
 * @param {*} res
 * @param {*} next
 * @returns redirection vers la page d'accueil
 */
function checkNotAuthenticated(req, res, next) {
	if (req.isAuthenticated()) {
		return res.redirect("/profil");
	}
	next();
}

/**
 * fonction qui verifie l'authentification du user
 * @param {*} req
 * @param {*} res
 * @param {*} next
 * @returns redirection vers la page de connexion
 */
function checkAuthenticated(req, res, next) {
	if (req.isAuthenticated()) {
		return next();
	}
}

// pour l'acces des fonctions dans les autres fichiers
module.exports = {
	checkNotAuthenticated,
	checkAuthenticated,
};
