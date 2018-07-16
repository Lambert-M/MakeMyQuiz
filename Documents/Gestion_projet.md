# Projet MakeMyQuiz 
## Participants 
* Maxime LAMBERT 
* Josie SIGNE
* Léo ROUZIC
* Christophe SAHID
* David RENARD-CALZANT
## Déroulement du projet 
### Sprint 1 : Prise en main des outils et partie "Jeu" _Du 07/05/18 au 20/05/18_ 
* Création des scènes de jeu (UI) [**Maxime**]
* Affichage du timer [**Maxime**]
* Score [**David**]
* Timer [**David**]
* Équipe [**Christophe + Léo**]
* Boucle de jeu (GameManager) [**Tout le monde**] 
* Choix des topics [**David**]
* Structure de données pour les topics, questions, réponses. [**Josie**]
* Système de joker pour augmenter le nombre de points gagnés [**Christophe + Léo**]
* Quiz de type QCM [**Tout le monde**]
### Sprint 2 : Partie "Jeu" _Du 21/05/18 au 31/05/18_ 
* Disparition de réponses fausses en fonction du temps écoulé lors d'une question [**Josie**]
* Affichage automatique de textes [**Josie**]
* Améliorations esthétiques et sonores [**Maxime + Léo**]
* Transition entre les scènes [**Tout le monde**]
* Affectation de touches pour répondre, passer à la scène suivante, mettre en pause [**Christophe + Léo**]
* Ajout d'une scène persistante pour mieux gérer l'accessibilité des données (scores, questions/réponses ...) [**Josie**]
* Réusinage du code relatif à la gestion des scores [**David**]
* Ajout de deux nouveaux types de quiz: Blindtest et Quiz à base d'images [**Maxime**]
* Ajout d'une structure de données pour les manches [**Josie**]
* Réusinage globale du code [**Maxime + Christophe**]
### Sprint 3 : Partie "Édition" _Du 01/06/18 au 24/06/18_ 
* Création des scènes d'édition (UI) [**Tout le monde**]
* Ajout de prefabs pour modifier les scènes dynamiquement en fonction des données éditées par l'utilisateur [**Tout le monde**]
* Listes déroulantes pour améliorer l'ergonomie des scènes d'édition [**David**]
* Éditeurs de QCM, Blindtest, et Quiz images [**Tout le monde**]
* Sauvegarde / Chargement des données [**Léo + David**]
* Sérialisation au format JSON [**David**]
* Lien entre les données de l'interface et les structures de données [**Josie**]
* Boutons de navigation entre les scènes d'édition [**Christophe**]
* Gestion des langues [**Josie + Maxime**]
* Redimensionnement des topics en fonction de leur nombre dans la scène de choix des topics [**Maxime**]
* Implémentation d'un système de gestion de fichiers pour que l'utilisateur puisse sélectionner ses quiz [**Léo + David**]
* Les classes représentant les QCM, Blindtest et Quiz d'images héritent d'une même classe QuestionData [**Maxime**]
## Spécialisation des membres du projet
* Maxime : Game UI, Game Controllers  
* Josie : Communication Controller-> Model -> Vue et inversément
* Léo : Architecture, Supervision
* Christophe : Ancrage des scènes (GUI), interaction entre les classes
* David : Logique, GUI, Sérialisation

Remarque : Il faut bien sûr prendre du recul quant aux spécialisations ci-dessus, elles ne sont en rien représentatives du travail complet effectué par chacun des membres. En effet, bien que l'on puisse dégager une tendance globale, chaque membre du projet a pu appréhender différentes thématiques et travailler sur d'autres parties du code que celle sur laquelle il est spécialisé.