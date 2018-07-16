
# PROJET L3 INFORMATIQUE - RENNES 1 2017/2018

## MakeMyQuiz - Document post-mortem
### SOMMAIRE

0. Introduction
	1. Objet du document
	2. État du projet
1. Reprise de code
	1. Gestion de fichiers
	2. Environnement
		1. Bibliothèque SimpleJSON
		2. Bibliothèque TextMeshPro
	3. Fonctionnalités
		1. Internationalisation
		2. Gestion des équipes
		3. Gestion des données
	4. À faire
		1. Personnalisation
		2. Gestion d'erreur
		3. Ergonomie
2. Critique des choix
	1. Environnement 
	2. Interface et gestion de données
	3. Structure de données

### Introduction

#### Objet du document
Ce document sert à rendre compte de l'état du projet. Dans un premier temps, une partie reprise de code donne des informations sur l'organisation statique du projet. Elle contient des informations sur les outils de développement à utiliser ainsi que des informations sur le code/package. Elle contient également toute les fonctionnalités restantes à développer et des indices sur comment procéder.
Une deuxième partie est consacrée à l'état des choix fait dans ce projet.

#### État du projet
Il est nécessaire de pousser la personnalisation plus loin afin que l'outil attire le plus de monde possible mais , à ce jour, la majorité des fonctionnalités essentielles ont été implantées et testées, à savoir :
* Jusqu'à 8 équipes participantes
* Calcul de points et gestion de bonus
* Édition et organisation en manches-sujet-questions
* Différents types de question
* Sauvegarde des quiz créés
* Internationalisation

Par la suite, on pourra rajouter de nouvelles fonctionnalités au fur et à mesure des idées qui viendront (cf. README). Cependant des explications "techniques" sur le projets sont avant tout nécessaires à la bonne implantation des fonctionnalités encore manquantes. Elles sont recensées dans la section suivante.

### I - Reprise de code

#### 1. Système de fichier
Le dossier **Documents** contient tous les documents d'informations du projet.
Les dossiers **Images, Saves & Sounds** sont créés et utilisés par le programme MakeMyQuiz lors du Runtime.
Les fichiers de développement se trouve dans le dossier **Assets**.
Nous avons séparé ce qui concerne le jeu et ce qui concerne les éditeurs en des packages différents : 
* **View/Editor & View/Game** : respectivement les scènes pour la partie édition et la partie jeu
* **Controller/EditorScripts & Controller/Game**: respectivement les scripts pour la partie édition et la partie jeu 
* **Model/EditorModel & Model/Game** : données partagés et uniques utilisés
* **Data** : structure de donnée
* **StreamingAssets** : dossier des langues .json facilement accessible dans Unity
* **Plugins** : plugins utilisés

On dispose d’un package de **Ressources** avec différent sous packages : 
* **Fonts** : Les polices d’écritures utilisés.
* **Images** : Les images attachés à certains objets GUI (comme les drapeau du menu de langue).
* **Prefabs** : Les prefabs qui sont des squelettes d’objets que nous instancions dans différents scripts et que nous pouvons modifié grâce aux scripts.
* **Sounds** : les sons fixes utilisés lors de certaines scène.

**design.md** est le document d'architecture 
**README.md** contient l'avancement du projet

La classe principale est le DataModel qui contient toutes les informations concernant le quiz joué ou en phase d'édition.

Elle possède un nombre important de variables qui définissent tous les paramètres dont nous avons besoin pour personnaliser le quiz et ses informations.

Le reste des dossiers sont crées par Unity lors de la création du projet.
Cette organisation est basée sur l'utilisation du moteur de jeu décrit par la suite.

#### 2. Environnement
Ce projet a été développé avec Unity Community 2018 disponible [ici](https://unity3d.com/fr).
Les éditeurs Microsoft Visual Studio (disponible [ici](https://visualstudio.microsoft.com/fr/vs/)) et Monodevelop sont conseillés afin de reconnaître toute les fonctions de Unity.
En plus des outils de base de Unity, nous avons utilisé différentes bibliothèques additionnelles gratuites décrites dans la suite.
La majorité de nos scripts dérivent d'une classe native d'Unity : la classe **MonoBehaviour**. Cette dernière possède plusieurs méthodes basiques notamment : 
* **`Start()`** : initialise la scène appelée.
* **`Update()`** : relance tous les évènements appelés à chaque frame.
* **`Invoke(string methodName, float time)`** : appelle une méthode *methodeName* après *time* seconde.


##### 2.1 Bibliothèque SimpleJSON

Nous avons utilisé la bibliothèque SimpleJSON ([Documentation](http://wiki.unity3d.com/index.php/SimpleJSON)) qui est utilisé dans le système de sauvegarde et de chargement de données.
Dans le cadre de l'implémentation d'un nouveau type de quiz, il est nécessaire de modifier le code de la classe *DataController* afin d'y ajouter les changements dans les méthodes `Save(string filepath)` et `Load(string filepath)`. Basiquement, il suffit d'ajouter un nouveau case dans la partie suivante du code (ligne 118) : 
``` 
switch (qd.GetType().ToString()) { 
case "MusicQuestion": 
	MusicQuestion mq = (MusicQuestion)qd; questionJson.Add("MusicPath", mq.MusicPath);
	questionJson.Add("StartTrack", mq.StartTrack); 
	questionJson.Add("Volume", mq.Volume); 
	questionJson.Add("Fade", mq.Fade); 
	break; 
case "ImageQuestion": 
	ImageQuestion iq = (ImageQuestion)qd; 
	questionJson.Add("ImagePath", iq.ImagePath); 
	break; 
case "TextQuestion": 
	TextQuestion tq = (TextQuestion)qd;
	questionJson.Add("Question", tq.Question);
	break; 
default: 
	Debug.LogError("Type de question non-reconnu"); 
	break; } 
```
Un procédé similaire s'applique pour le chargement des données dans la méthode `Load(string filepath)`. Par ailleurs, nous avons utilisé le plugin [Simple File Browser](https://gracesgames.github.io/SimpleFileBrowser/), nous permettant ainsi d'avoir un système de gestion de fichiers basique mais fonctionnel. 

De nombreuses méthodes sont référencées dans la documentation, personnellement nous nous sommes limités aux fonctionnalités de base, à savoir ouvrir une fenêtre d'explorateur de fichiers pour sauvegarder et charger des données d'un fichier JSON. 

À titre d'exemple, voici une partie du code permettant d'instancier un objet représentant le système de gestion de fichiers (en mode paysage) : 
```
GameObject fb = (GameObject)Instantiate(Resources.Load("Prefabs/FileBrowser")); 
fb.GetComponent<FileBrowser>().SetupFileBrowser(ViewMode.Landscape,pathsave);
 ```
Comme le montre cet exemple, la fenêtre est en fait un Prefab instancié à partir du dossier Ressources, il est possible de le modifier si besoin pour diverses utilisations.

##### 2.2 Bibliothèque TextMeshPro

La deuxième bibliothèque utilisée est TextMeshPro (TMP) afin d'avoir des textes plus élégants.
TextMeshPro rajoute des objets préfabriqués dans le menu contextuel de Unity.
Les différentes polices utilisés se trouvent dans le dossier Ressources/Fonts.
L'intégralité des boîtes de dialogues (Input Field) et textes du projet sont ceux fournis par la bibliothèque [TMP](https://docs.unity3d.com/Packages/com.unity.textmeshpro@1.2/manual/index.html).
Pour l'importer : `using TMPro;`

Les bibliothèques sont utilisés dans certaines classes qui permettent d’implémenter les fonctionnalités décrites ci-dessous.

#### 3. Les fonctionnalités

##### 3.1 Internationalisation  
La bibliothèque JSON décrite plus haut nous à servit notamment pour l’internationalisation.
Nous avons créé un modèle de table de hachage en format JSON (pour pouvoir être modifiées facilement à la main). C'est un ensemble de clés ( l'idée du texte à quoi il correspond en anglais ) avec une value correspondante au texte qui lui est associée dans la langue désirée. 
Exemple : 
* Fichier anglais :
`{"key":"green_team","value":"The green team"}, 
{"key":"orange_team","value":"The orange team"}`
* Fichier français : 
`{"key":"green_team","value":"L'equipe verte"},
{"key":"orange_team","value":"L'equipe orange"}`
 
Chaque texte dans les scènes ont donc une clé qui leur est associée dans les fichiers `localizedText`. Pensez-y si vous ajoutez de nouvelles scènes.
L'ajout d'une nouvelle langue implique la création d'un nouveau fichier json " `localizedText_"votre_langue"`" dans le package StreamingAssets. Il suffit alors de copier coller le fichier d'une langue et remplacer les value par celles qui correspondent à la langue désirée. Il reste alors à modifier la scène "Language" de l'éditeur en y ajoutant un bouton avec le drapeau qui chargera le fichier json à l'aide de la fonction `LanguageManager.LoadLocalizedtext(nom du fichier)`.

##### 3.2 Gestion des équipes
Les équipes sont contrôlées par le script PlayerController.cs. Ce script possède les fonctionnalités suivantes : 
* La gestion des touches (Inputs/KeyBinds)
* Enregistrement des réponses 
* Calcul du score en fonction du temps
* Activation des jokers
* Mise à jour de l'affichage des réponses des équipes

Chaque instance d'équipe est associée à un PlayerController.


##### 3.3 Gestion des données

Toutes les données sont situées dans DataModel.

Un quiz contient une Liste de RoundData avec un type (QCM, Blind test, Images) et une liste de TopicData.

Chaque TopicData est représenté par un nom, et contient une liste de QuestionData.

Le type QuestionData est abstrait et ne contient qu'une liste d'AnswerData.

Les types qui l'instancient sont TextQuestion, ImageQuestion et MusicQuestion et on chacun leurs propres priorités et scènes d'édition associées.

Les AnswerData sont sous forme d'un texte et d'un booléen qui renseigne si la réponse est considérée comme correcte ou non.

Pour ajouter un type de question, il faut :
* Ajouter un nouveau type qui instancie QuestionData
* Une scène dans le jeu qui sera adaptée à ce nouveau type de questions 
* Une scène d'édition correspondante
* Un nouveau type de RoundData 
* Mettre à jour le dropdown de la scène d'édition de Rounds

Bien que ces fonctionnalités aient été implémentées et testées, nous avions également quelques suggestions pour de futurs ajouts pouvant améliorer l'utilisation de notre logiciel.

#### 4. À faire

##### 4.1 Personnalisation
La personnalisation du quiz telle qu'elle est aujourd'hui est insuffisante. Par exemple, les fonctionnalités suivantes sont à faire  :
   * Le nombre de réponses aux questions
   * Le nombre de points donnés
   * Le temps d'une question
   * Le nombre de bonnes réponses 
   * Ajout d'un système de rapidité
   * Personnalisation des images importées
   * ...

À l'heure actuelle le code se base sur des données fixes pour ces éléments qu'il faut remplacer par des variables.
Ces données devront être stockées dans le DataModel afin qu'elles puissent être accessibles en permanence par chaque classe les utilisant.
C'est pourquoi le DataModel est constitué uniquement de variables `public static` ( == partagée et unique)
Le DataModel est un objet persistant n'étant jamais détruit.

##### 4.2 Gestion d'erreur
Presque aucun système de détection d'erreurs n'a encore été mis en place. Voici des exemples d'erreurs a détecter :
   * Le client doit bien remplir chaque champ avant de lancer son quiz.
   * Afficher des messages pour confirmer certains choix de retour arrière ou pour quitter
   * Ce qui a été précisé dans le readme.md

##### 4.3 Ergonomie
On souhaite également mettre en place un système de tutoriel afin d'améliorer la prise en main. Il serait pertinent d'ajouter un système de bulles d'informations qui guident le client.
Voici quelques fonctionnalités ergonomiques manquantes : 
* Ajouter des raccourcis pratique dans la navigation des différents menus
* Animations visuelles
* Améliorer l'interface du FileBrowser
* ...

Ayant détaillé l'état actuel du projet, ainsi que les différentes manières de reprendre le code source, il est également important de critiquer nos choix d'implémentation afin que toute personne souhaitant reprendre le projet Make My Quiz puisse éviter certains écueils auxquels nous avons pu être confronté.

### II - Critique des choix

#### 1. Environnement

La version gratuite de Unity est une API limitée : 
* Les assets intéressants sont payants
* Les formats des musiques lisibles par le lecteur Unity inclut le MP3 uniquement pour les plateformes Android & iOS

À cette heure, nous n'avons créé qu'un exécutable Windows. À l'avenir, il faudra prévoir des versions exploitables sur MacOS et Linux.
Unity possède des outils de personnalisation pour l'interface, ce qui nous a orienté vers des méthodes de développement critiquable comme ci-dessous.

#### 2. Interface et gestion de données

* L'utilisation d'un même script pour plusieurs instances d'objets ralentie le logiciel, une solution serait de concevoir un unique script contrôlant toutes les instances d'un même objet.
* Le chargement des musiques n'est pas optimal. En effet, un enchaînement de boucles de chargement ralenti le processus et rend la navigation instable. Aucune alternative n'a été implémenté pour le moment.
* Aucun pré-chargement de scène ce qui rend les transitions moins fluides, particulièrement dans l'éditeur.

Le dernier point de critique concerne certains choix d'emplois de bibliothèque externes qui se sont révélés obsolètes.

#### 3. Structure de données

* Une réflexion plus approfondie sur les différents formats de sérialisation nous aurait fait gagner du temps. En effet, nous avons d’abord choisi d’utiliser la sérialisation en format binaire disponible sur Unity alors que la sérialisation en JSON présentait de nombreux avantages (multi-langage, compréhensible, facilement modifiable …).
* L'utilisation d'une bibliothèque extérieure (SimpleJSON) plutôt que celle inclut nativement dans Unity a rajouté une certaine complexité concernant la sérialisation des données.

## Conclusion
Aujourd'hui, nous avons produit un logiciel fonctionnel dans un intervalle de temps assez réduit (8 semaines). Cependant, il reste améliorable en terme d'ergonomie et de possibilités d'édition pour que le client soit entièrement satisfait. Nous souhaitons continuer à travailler sur le projet afin de le perfectionner en nous aidant de retours clients.