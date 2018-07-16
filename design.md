
# Projet : MakeMyQuiz

====================================================================

### Participants :
* Maxime LAMBERT
* Josie SIGNE
* Christophe SAHID
* Léo ROUZIC
* David RENARD-CALZANT

## SOMMAIRE
1. Dictionnaire
2. Introduction
3.  Contraintes d'analyse
	1. Les contraintes liées à l'utilisation
	2. Le système de sauvegarde
	3. Partage / Internationalisation
4. Principes et classes
     1. Unity
     2. Modèle-Vue-Contrôleur
     3. Héritage
5. Vue des cas d'utilisation
	1. Diagramme d'utilisation
	2. Diagramme de séquence
	
# 1.Dictionnaire 

MVC: Modèle Vue Contrôleur
API : Application Programming Interface
GUI : Graphical User Interface

# 2. Introduction

MakeMyQuiz est un logiciel créé pour accompagner un utilisateur souhaitant animer un Quiz en mettant à sa disposition un outil simple pour créer et éditer des quiz, le tout grâce à une interface ludique. Développé sous Unity, nous avons conçu et mis en place diverses fonctionnalités adaptées à l'organisation de ce genre d’événements (cf Gestion_Projet). Le principe est simple : plusieurs équipes s'affrontent dans une ambiance décontractée et un présentateur anime la soirée. 

Nous avons déjà participé à plusieurs Quiz et constaté un manque de modernité dans l'animation. En effet, peu de logiciels permettent d'organiser ces événements, ainsi les présentateurs sont souvent contraints de "bricoler" une présentation, ce qui est contraignant et peut aboutir à des problèmes d'organisation. 
Après consultation de notre entourage, plusieurs personnes ont montré leur intérêt. Ainsi, nous avons décidé de développer notre propre application pour répondre à ce besoin.

## 3. Contraintes d'analyse

### 3.1 Les contraintes liées à l'utilisation

Notre logiciel s'adresse à des animateurs qui souhaitent s'en servir comme support pour présenter des quiz. Cette donnée impose plusieurs contraintes pour la partie jeu du logiciel : par expérience, nous avons décidé de limiter le nombre d'équipes participantes à 8 . De plus, nous avons fait défiler nos scènes uniquement par l'intervention d'un tiers pour que l'animateur puisse gérer son rythme. L'interface du jeu a été épurée, sans surcharge d'indications, puisque le jeu est destiné a être projeté.

Les quiz étant destiné à l'animation, il doit être possible de pouvoir les préparer à l'avance, de ce fait un système de sauvegarde et de chargement est nécessaire pour la partie Édition

### 3.2 Le système de sauvegarde
Afin d'avoir un système de sauvegarde opérationnel, nous avons implémenté un gestionnaire de fichier ou "FileBrowser". Cependant, le temps consacré à réaliser un FileBrowser étant élevé, nous avons opté pour un plugin gratuit disponible sur Unity. Ayant besoin d'un format de sauvegarde adaptable dans le temps, nous nous sommes tournés vers le format JSON. Pour cela, nous avons utilisé la bibliothèque SimpleJSON possédant des méthodes basiques de sérialisation.

L'ajout de cette fonctionnalité participe au confort d'utilisation pour le présentateur. Par ailleurs, ne sachant pas la langue native de l'utilisateur nous avons dû mettre en place un système d'internationalisation.

### 3.3 Partage / Internationalisation

La volonté de partager notre projet librement implique une internationalisation de ce dernier. Nous supportons le Français et l'Anglais mais il est possible d'ajouter facilement de nouvelles langues.
C'est aussi le format JSON qui a servit à l'implémentation de la base de donnée pour l'internationalisation.

Toutes ces contraintes nous ont amené à structurer notre projet selon des modèles et principes que nous détaillons dans la partie suivante.

## 4. Principes et classes

### 4.1 Unity

Ce projet est la première application que nous réalisons et nous avons choisi de travailler sur Unity.  
C'est un moteur de jeu populaire puisqu'il permet un prototypage rapide et le développement d'applications facilement déployables sur plusieurs supports (Windows, Mac OS X, iOS, Android ...).
L'un des principaux avantages d'Unity réside dans la simplicité pour créer une GUI. L'instanciation des objets graphiques peut être réalisée directement dans le framework à partir d'éléments préfabriqués.
L'abondance de tutoriels et de documentations détaillées sur le web nous a conforté dans ce choix.
De plus, il est possible de repasser sur une programmation plus classique grâce à l'ajout de scripts.
Notre logiciel exploite les deux principes et consiste en un enchaînement de scènes qui ont chacune un script qui leurs est associé.

Les scripts sont rédigés en C#, c'est une contrainte inhérente au framework puisque Unity accepte uniquement des scripts rédigés dans ce langage et en JavaScript (aussi appelé UnityScript qui n'a rien à voir avec le JavaScript). UnityScript est un langage plus simple à prendre en main mais plus limité. En effet, UnityScript est utilisé uniquement dans Unity. De ce fait, il est beaucoup plus difficile de trouver des informations sur ce langage et cela aurait pu constituer une perte de temps. De plus, possédant déjà des compétences en programmation impérative, prendre en main le C# ne constituait pas une difficulté.

Plusieurs de nos scripts se basent sur le concept de contrôleur du modèle d'architecture MVC qui est abordé dans la partie suivante.


### 4.2 Modèle-Vue-Contrôleur

Pour résumer ce concept la partie Vue est composée des différentes scènes que nous avons mis au point. La partie Contrôleur quant à elle est représentée par les scripts qui manipulent nos différentes données qui impactent par la suite notre GUI. Les données constituent le Modèle. Il peut être modifié grâce par le biais du Contrôleur suite aux interactions effectuées entre l'utilisateur et le GUI.
Dans notre cas ce sont les données du quiz qui sont impactées.

Parmi ces données nous avons créé une classe abstraite pour les questions.

### 4.3 Héritage

La classe de question à été exploitée grâce au principe d'héritage afin de généraliser la notion de question. On la spécialise ensuite à l'aide des sous-classes représentant chacune un type de question donné (Blindtest, Quiz d'images, QCM). Cela permettra une évolution plus facile pour les différents types de questionnaires.
![Imgur](https://i.imgur.com/HvLeedO.jpg)

Après avoir détaillé les principales fonctionnalités de notre logiciel, voici quelques diagrammes pour donner une vue d'ensemble de certains cas d'utilisation.

## 5. Vue des Cas d'utilisation

### 5.1 Diagramme d'utilisation :
![Imgur](https://i.imgur.com/ivGPPlF.jpg)

### 5.2 Diagramme de séquence : 
Création d'un quiz
![Imgur](https://i.imgur.com/XN32U2l.jpg)
