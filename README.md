# AudioJDR - README

## Description

**AudioJDR** est une application Windows permettant de jouer à des jeux de rôle textuels uniquement via des interactions vocales. Ce projet a pour objectif de rendre les jeux de rôle accessibles aux personnes déficientes visuelles en utilisant la synthèse vocale et la reconnaissance vocale. Le joueur pourra naviguer et progresser dans l'histoire en parlant à l'application, sans avoir besoin d'interface graphique.

L'application comporte deux parties principales :
1. **Un moteur de jeu** qui permet de jouer à des jeux de rôle audio.
2. **Un éditeur de jeu** qui permet aux créateurs de concevoir des jeux de rôle textuels, jouables par les non-voyants.

## Fonctionnalités

### Partie Joueur :
- **Synthèse vocale** pour lire les descriptions et actions du jeu.
- **Reconnaissance vocale** pour les interactions du joueur.
- **Système de sauvegarde** automatique des parties en cours.
- **Langues multiples** disponibles pour une meilleure accessibilité.

### Partie Éditeur :
- **Création de jeux audio** (histoires interactives) à partir de textes.
- **Interface intuitive** pour créer et modifier des histoires, événements et options.
- **Exportation et importation d’histoires** au format compatible avec le moteur de jeu.
- **Gestion d’événements et d’options** pour un gameplay non-linéaire.
- **Interface multilingue** pour une utilisation en plusieurs langues.

## Technologies Utilisées
- **.NET MAUI** : Framework de développement multiplateforme.
- **C#** : Langage de développement principal.
- **Synthèse vocale Windows** : API pour lire les descriptions textuelles.
- **Reconnaissance vocale Windows** : API pour comprendre les ordres du joueur.
- **JSON** : Format utilisé pour exporter et importer les jeux (fichiers d’histoires).

## Avancement

- **Partie éditeur** : La fonctionnalité de base de l'éditeur est terminée. Il est désormais possible de créer une histoire simple avec des événements et des options associées. Ces histoires peuvent être exportées et importées sous un format compatible avec le moteur de jeu. L'interface de l'éditeur est **entièrement responsive**, assurant une utilisation fluide sur différentes résolutions et tailles d'écran.

- **Prochaine étape** : Le développement se concentrera désormais sur la **partie joueur**, avec pour objectif de permettre une expérience immersive en utilisant uniquement la voix pour naviguer dans l'histoire et interagir avec le jeu.

## Installation

### Prérequis
- **Windows 10** ou version supérieure.
- .NET MAUI installé.

### Instructions
1. Clonez ce dépôt GitHub.
   ```bash
   git clone https://github.com/dept-info-iut-dijon/S5_B1_TalcColombien.git
   ```
2. Ouvrez le projet dans Visual Studio.
3. Compilez et lancez l'application.

## Contributeurs

- **Alexandre Guidet** (Client)
- **Florence Mendes** (Chef de projet)
- **Le Talc Colombien** (Équipe de développement)
